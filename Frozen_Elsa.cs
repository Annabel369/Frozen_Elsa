using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Admin;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Core.Translations;
using CounterStrikeSharp.API.Modules.Utils;
using Frozen_Elsa.Repository;
using System.Text.Json.Serialization;
using System.Drawing;





namespace Frozen_Elsa;

public partial class Frozen_Elsa : BasePlugin
{ 
    public override string ModuleName => "Frozen_Elsa";
    public override string ModuleAuthor => "Annabel369";
    public override string ModuleDescription => "Adds Grenades Special Effects.";
    public override string ModuleVersion => "V. 2.1.9";

    public bool IsHooked { get; set; }

    public System.Numerics.Vector3 ForwardVector { get; set; }
    public byte LIFE_ALIVE { get; private set; }
    private static readonly Vector VectorZero = new Vector(0, 0, 0);
    private static readonly QAngle RotationZero = new QAngle(0, 0, 0);
    private bool shouldShowImage = false;
    public CBeam? BeamEntity { get; set; }
    //private bool isCatAnimationOn = false;//on or off 
    public bool bombsiteAnnouncer;

    public override void Load(bool hotReload)
    {
        RegisterListener<Listeners.OnTick>(OnTick);
    }




    [GameEventHandler]
    public HookResult OnRoundEnd(EventPlayerDeath @event, GameEventInfo info)
    {
        bombsiteAnnouncer = false;
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

private bool HasPermission(CCSPlayerController? player, string id)
    {
        string permission = string.Empty;
        string team = string.Empty;

        switch (id)
        {
            case "Permission":
                permission = "@css/custom-permission";
                team = "all";// t or ct or all
                break;
            case "Permission2":
                permission = "@css/custom-permission2";
                team = "all";
                break;
        }

        return (string.IsNullOrEmpty(permission) || AdminManager.PlayerHasPermissions(player, permission)) &&
               isTeamValid(player, team.ToLower());
    }

     public bool isTeamValid(CCSPlayerController? player, string team)
    {
        return (team == "t" || team == "terrorist") && player?.Team == CsTeam.Terrorist ||
               (team == "ct" || team == "counterterrorist") && player?.Team == CsTeam.CounterTerrorist ||
               string.IsNullOrEmpty(team) || team == "both" || team == "all";
    }


 private void CreateEffect(string effectName, CCSPlayerController player, Vector Position, string effectFile, string colorValue = "", float width = 0, float lifetime = 1.0f)
    {
        Vector bulletDestination = new Vector(Position.X, Position.Y, Position.Z);

        switch (effectName.ToLower())
        {
            case "impact":
                effectName = string.IsNullOrEmpty(effectFile) ? "impact" : "impactparticle";
                break;
            case "hiteffect":
                Position.Z += 32;
                break;
            case "killeffect":
                Position.Z += 32;
                break;
        }

        if (effectName == "tracer" || effectName == "impact")
        {
            var tracer = Utilities.CreateEntityByName<CBeam>("env_beam")!;

            Color color = ParseColor(colorValue);
            tracer.Render = color;

            tracer.Width = width;
            tracer.DispatchSpawn();

            if (effectName == "tracer")
                Position = GetEyePosition(player);

            if (effectName == "impact")
            {
                Position.Z += width;
                bulletDestination.Z -= width;
            }

            tracer.Teleport(Position);

            tracer.EndPos.X = bulletDestination.X;
            tracer.EndPos.Y = bulletDestination.Y;
            tracer.EndPos.Z = bulletDestination.Z;

            Utilities.SetStateChanged(tracer, "CBeam", "m_vecEndPos");

            AddTimer(lifetime, tracer.Remove);
        }
        else
        {
            var particle = Utilities.CreateEntityByName<CParticleSystem>("info_particle_system")!;

            particle.EffectName = effectFile;
            particle.DispatchSpawn();
            particle.AcceptInput("Start");

            particle.Teleport(bulletDestination);

            AddTimer(1.0f, particle.Remove);
        }
    }
public static Vector GetEyePosition(CCSPlayerController player)
    {
        Vector absorigin = player.PlayerPawn.Value!.AbsOrigin!;
        CPlayer_CameraServices camera = player.PlayerPawn.Value!.CameraServices!;

        return new Vector(absorigin.X, absorigin.Y, absorigin.Z + camera.OldPlayerViewOffsetZ);
    }

    public static QAngle GetNormalizedAngles(CCSPlayerController player)
    {
        QAngle AbsRotation = player.PlayerPawn.Value!.AbsRotation!;
        return new QAngle(
            AbsRotation.X,
            (float)Math.Round(AbsRotation.Y / 10.0) * 10,
            AbsRotation.Z
        );
    }

private int colorIndex = 0;

    private Color ParseColor(string colorValue)
    {
        if (string.IsNullOrEmpty(colorValue) || colorValue.ToLower() == "random")
        {
            var color = RainbowColors[colorIndex];
            colorIndex = (colorIndex + 1) % RainbowColors.Length;
            return color;
        }
        var colorParts = colorValue.Split(' ');
        if (colorParts.Length == 3 &&
            int.TryParse(colorParts[0], out var r) &&
            int.TryParse(colorParts[1], out var g) &&
            int.TryParse(colorParts[2], out var b))
        {
            return Color.FromArgb(255, r, g, b);
        }
        return Color.FromArgb(255, 255, 255, 255);
    }

Color[] RainbowColors = {
        
        Color.FromArgb(255, 255, 0, 0),     // Red
        Color.FromArgb(255, 0, 255, 0),     // Green
        Color.FromArgb(255, 0, 0, 255),      // Blue
        Color.FromArgb(255, 0, 255, 255),   // Cyan
        Color.FromArgb(255, 255, 255, 0),   // Yellow
        Color.FromArgb(255, 250, 250, 250), // White
        Color.FromArgb(255, 255, 0, 255),   // Magenta
    };


    [GameEventHandler]
    public HookResult OnDecoyStarted(EventDecoyStarted @event, GameEventInfo info)
    {
        //CCSPlayerController player = @event.Userid;

        
       if (shouldShowImage)
        {
            foreach (CCSPlayerController player in Utilities.GetPlayers())
            {
                if (player != null && player.IsValid)
                {
                    player?.ExecuteClientCommand($"play sounds/frozen_music2/frozen-go.vsnd_c");
                     SphereEntity sphereEntity = new SphereEntity(new Vector(@event.X, @event.Y, @event.Z), 200);
                     DrawLaserBetween(sphereEntity.circleInnerPoints, sphereEntity.circleOutterPoints, 5);
                    
                }   
            }
        }   

        return HookResult.Continue;
    }

    

    [GameEventHandler]
    public HookResult PlayerDeath(EventPlayerDeath @event, GameEventInfo info)
    {
        if (@event.Userid == null || @event.Attacker == null)
            return HookResult.Continue;
        //if (!shouldShowImage)return HookResult.Continue; //kabooo

        if (HasPermission(@event.Attacker, "Permission"))
        CreateEffect("killeffect", @event.Attacker, @event.Userid.PlayerPawn.Value!.AbsOrigin!, "particles/explosions_fx/explosion_basic.vpcf");

        return HookResult.Continue;
    }

   [GameEventHandler]
    public HookResult PlayerHurt(EventPlayerHurt @event, GameEventInfo info)
    {
        if (@event.Userid == null || @event.Attacker == null)
            return HookResult.Continue;

            if (HasPermission(@event.Attacker, "Permission"))
            CreateEffect("hiteffect", @event.Attacker, @event.Userid.PlayerPawn.Value!.AbsOrigin!, "particles/weapons/cs_weapon_fx/weapon_taser_glow.vpcf");

        return HookResult.Continue;
    }

    public (int, CBeam?) DrawLaserBetween(Vector startPos, Vector endPos, Color color, float life, float width)
    {
        if (startPos == null || endPos == null)
        {
            return (-1, null);
        }

        CBeam? beam = Utilities.CreateEntityByName<CBeam>("beam");

        if (beam == null)
        {
            return (-1, null);
        }

        beam.Render = color;
        beam.Width = width / 2.0f;

        beam.Teleport(startPos, RotationZero, VectorZero);
        beam.EndPos.X = endPos.X;
        beam.EndPos.Y = endPos.Y;
        beam.EndPos.Z = endPos.Z;
        beam.DispatchSpawn();

        AddTimer(life, () => { beam.Remove(); });

        return ((int)beam.Index, beam);
    }

    [GameEventHandler(HookMode.Pre)]
    public HookResult BulletImpact(EventBulletImpact @event, GameEventInfo info)
    {
        CCSPlayerController? player = @event.Userid;

        Random random = new Random();
        int NumberRandom = random.Next(1, 8);// Random 1 to 7

        Vector? PlayerPosition = player?.Pawn?.Value?.AbsOrigin;
        Vector? BulletOrigin = new Vector(PlayerPosition?.X, PlayerPosition?.Y, PlayerPosition?.Z + 57);
        Vector? bulletDestination = new Vector(@event.X, @event.Y, @event.Z);

        ///if (Config.Tracer.Enable && HasPermission(player, "tracer"))

        if (player?.TeamNum == 3 && HasPermission(player, "Permission"))
        {
            DrawLaserBetween(BulletOrigin, bulletDestination, RainbowColors[NumberRandom], 0.2f, 1.0f);//default Color.Blue or RGB RainbowColors[NumberRandom]
        }
        else if (player?.TeamNum == 2 && HasPermission(player, "Permission"))
        {
            DrawLaserBetween(BulletOrigin, bulletDestination, RainbowColors[NumberRandom], 0.2f, 1.0f);//default Color.Red
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
