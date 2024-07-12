using System.Text.Json.Serialization;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Modules.Utils;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using static CounterStrikeSharp.API.Core.Listeners;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Reflection.Metadata;

using CounterStrikeSharp.API.Modules.Timers;
using CounterStrikeSharp.API.Modules.Memory;
using Frozen_music.Config;
using CounterStrikeSharp.API.Modules.Commands.Targeting;
using CounterStrikeSharp.API.Modules.Commands;



namespace Frozen_Elsa;
public class Config : BasePluginConfig
{

    
    [JsonPropertyName("show-player-counter")]
    public bool PlayerCounter { get; set; } = true;
    [JsonPropertyName("ConfigVersion")]
    public override int Version { get; set; } = 2;

    public bool IsHooked { get; set; }
    public CBeam? BeamEntity { get; set; }
    public System.Numerics.Vector3 ForwardVector { get; set; }
}
public partial class Frozen_Elsa : BasePlugin, IPluginConfig<Config>
{
    public override string ModuleName => "Frozen_Elsa";
    public override string ModuleAuthor => "Astral & kenoxyd laser custom";
    public override string ModuleDescription => "Adds Grenades Special Effects.";
    public override string ModuleVersion => "V. 2.1.4";

    public required Config Config { get; set; }
    public byte LIFE_ALIVE { get; private set; }
    private static readonly Vector VectorZero = new Vector(0, 0, 0);
    private static readonly QAngle RotationZero = new QAngle(0, 0, 0);
    private bool shouldShowImage = false;
    public bool bombsiteAnnouncer;

    public override void Load(bool hotReload)
    {
        RegisterListener<Listeners.OnTick>(OnTick);
    }

    public void OnConfigParsed(Config config)
    {

        Config = config;
    }

    public System.Numerics.Vector3 QAngleToForwardVector(QAngle angle)
    {
        var pitch = (Math.PI / 180) * angle.X;
        var yaw = (Math.PI / 180) * angle.Y;

        var cosPitch = Math.Cos(pitch);
        var sinPitch = Math.Sin(pitch);
        var cosYaw = Math.Cos(yaw);
        var sinYaw = Math.Sin(yaw);

        return new System.Numerics.Vector3((float)(cosPitch * cosYaw), (float)(cosPitch * sinYaw), (float)-sinPitch);
    }

    [GameEventHandler]
    public HookResult OnRoundEnd(EventPlayerDeath @event, GameEventInfo info)
    {
        bombsiteAnnouncer = false;
        if (Config is not null)
        {
            // sphere ent
            foreach (var player in Utilities.FindAllEntitiesByDesignerName<CCSPlayerController>("cs_player_controller"))
            {

                if (player != null && player.IsValid)//&& !player.IsBot
                {
                    if (player.Team == CsTeam.Terrorist)
                    {
                        if (player?.PlayerPawn != null && player?.PlayerPawn.Value != null)
                        {
                            player.PlayerPawn.Value.Render = Color.FromArgb(255, 255, 255);//defalt
                        }
                    }
                }
            }

        }
        return HookResult.Continue;
    }

    [GameEventHandler]
    public HookResult OnDecoyStarted(EventDecoyStarted @event, GameEventInfo info)
    {
        //CCSPlayerController player = @event.Userid;

        
        if (Config is not null)
        {
            // sphere ent
            foreach (var player in Utilities.FindAllEntitiesByDesignerName<CCSPlayerController>("cs_player_controller"))
            {

                if (player != null && player.IsValid)//&& !player.IsBot
                {
                    if (player.Team == CsTeam.Terrorist)
                    {
                        if (player?.PlayerPawn != null && player?.PlayerPawn.Value != null)
                        {
                           
                            var callerName = player == null ? "Console" : player.PlayerName;
                            player?.ExecuteClientCommand($"play sounds/frozen_music2/frozen-go.vsnd_c");

                            SphereEntity sphereEntity = new SphereEntity(new Vector(@event.X, @event.Y, @event.Z), 200);
        
                            DrawLaserBetween(sphereEntity.circleInnerPoints, sphereEntity.circleOutterPoints, 5);
                            //Server.ExecuteCommand($"css_freeze {callerName} 5");
                            //player?.PrintToChat($"Freeze {callerName} 5 secord");
                            //player.PlayerPawn.Value.Render = Color.FromArgb(0, 0, 255);//Azul
                            // The character emits light, and its effect is not satisfactory, but it can indeed achieve character illumination.Here, we need to obtain the player's model path and SetModel, but I have not found a way to do so
                           


                            
                        }
                    }
                }
            }

        }



        return HookResult.Continue;
    }



    public void OnTick()
    {    
        string gifUrl = Globals.SiteImage;

        if (shouldShowImage)
        {
            foreach (CCSPlayerController player in Utilities.GetPlayers())
            {
                if (player != null && player.IsValid)
                {
                    player.PrintToCenterHtml($"<img src=\"{gifUrl}\">",10);
                }   
            }
        }
    }

    public (int, CBeam) DrawLaserBetween(Vector startPos, Vector endPos, Color color, float life, float width)
    {

        CBeam? beam = Utilities.CreateEntityByName<CBeam>("beam");

        if (beam == null)
        {
            return (-1, null);
        }

        beam.Render = color;

        // Set the desired width for a thinner tracer
        beam.Width = width / 2.0f; // Adjust this value to control thickness

        beam.Teleport(startPos, RotationZero, VectorZero);
        beam.EndPos.X = endPos.X;
        beam.EndPos.Y = endPos.Y;
        beam.EndPos.Z = endPos.Z;
        beam.DispatchSpawn();

        AddTimer(life, () => { beam.Remove(); }); // Destroy beam after specific time

        return ((int)beam.Index, beam);
    }

    [GameEventHandler(HookMode.Pre)]
    public HookResult BulletImpact(EventBulletImpact @event, GameEventInfo info)
    {
        CCSPlayerController player = @event.Userid;
        
        Vector PlayerPosition = player?.Pawn.Value.AbsOrigin;
        Vector BulletOrigin = new Vector(PlayerPosition.X, PlayerPosition.Y, PlayerPosition.Z + 57); // Adjust Z offset if needed
        Vector bulletDestination = new Vector(@event.X, @event.Y, @event.Z);

        if (player?.TeamNum == 3)
        {
            DrawLaserBetween(BulletOrigin, bulletDestination, Color.Blue, 0.2f, 1.0f); // Adjust width and color as desired
        }
        else if (player.TeamNum == 2)
        {
            DrawLaserBetween(BulletOrigin, bulletDestination, Color.Red, 0.2f, 1.0f); // Adjust width and color as desired
        }

        return HookResult.Continue;
    }





    private void DrawLaserBetween(Vector[] startPos, Vector[] endPos, float duration)
    {

        for (int i = 0; i < endPos.Length; i++)
        {

            CBeam? beam = Utilities.CreateEntityByName<CBeam>("beam");

            //var pawn = player?.PlayerPawn.Get();
            //var activeWeapon = pawn?.WeaponServices?.ActiveWeapon.Get();


            if (beam == null)
            {
                return;
            }
            
                beam.Render = Color.Blue;
                beam.Width = 2.0f;

                beam.Teleport(startPos[i], new QAngle(0), new Vector(0, 0, 0));
                beam.EndPos.X = endPos[i].X;
                beam.EndPos.Y = endPos[i].Y;
                beam.EndPos.Z = endPos[i].Z;



            beam.DispatchSpawn();
            AddTimer(duration, () => { beam.Remove(); });

        }


    }



   





}
