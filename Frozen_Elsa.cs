using System.Text.Json.Serialization;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Utils;
using System.Drawing;




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

        
       if (shouldShowImage)
        {
            foreach (CCSPlayerController player in Utilities.GetPlayers())
            {
                if (player != null && player.IsValid)
                {
                    player?.ExecuteClientCommand($"play sounds/frozen_music2/frozen-go.vsnd_c");
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

        Vector? PlayerPosition = player?.Pawn?.Value?.AbsOrigin;
        Vector? BulletOrigin = new Vector(PlayerPosition?.X, PlayerPosition?.Y, PlayerPosition?.Z + 57);
        Vector? bulletDestination = new Vector(@event.X, @event.Y, @event.Z);

        if (player?.TeamNum == 3)
        {
            DrawLaserBetween(BulletOrigin, bulletDestination, Color.Blue, 0.2f, 1.0f);
        }
        else if (player?.TeamNum == 2)
        {
            DrawLaserBetween(BulletOrigin, bulletDestination, Color.Red, 0.2f, 1.0f);
        }

        return HookResult.Continue;
    }









}
