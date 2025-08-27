using System.Text.Json;
using System.Text.Json.Serialization;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Modules.Utils;
using CounterStrikeSharp.API.Modules.Timers;
using CounterStrikeSharp.API.Modules.Commands;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using static CounterStrikeSharp.API.Core.Listeners;
using CounterStrikeSharp.API.Modules.Admin;
using CounterStrikeSharp.API.Core.Translations;
using CounterStrikeSharp.API.Core.Attributes;


namespace Frozen_Elsa;

public class Config : BasePluginConfig
{
    public bool SiteImage { get; set; } = true;

    [JsonPropertyName("show-player-counter")]
    public bool PlayerCounter { get; set; } = true;

    [JsonPropertyName("ConfigVersion")]
    public override int Version { get; set; } = 2;

    public bool IsHooked { get; set; }

    public System.Numerics.Vector3 ForwardVector { get; set; } = new(0, 0, 0);
}

public partial class Frozen_Elsa : BasePlugin, IPluginConfig<Config>
{
    public override string ModuleName => "Frozen_Elsa";
    public override string ModuleAuthor => "Astral + Copilot";
    public override string ModuleDescription => "Adds Grenades Special Effects with Frozen-style magic.";
    public override string ModuleVersion => "V. 4.0.4";

    public required Config Config { get; set; }
    public CBeam? BeamEntity { get; set; }
    private readonly Random _random = new();
    private bool shouldShowImage = false;

    private static readonly Vector VectorZero = new(0, 0, 0);
    private static readonly QAngle RotationZero = new(0, 0, 0);

    public override void Load(bool hotReload)
    {
        EnsureConfigFileExists();
        RegisterListener<Listeners.OnTick>(OnTick);
        base.Load(hotReload);
    }

    public void OnConfigParsed(Config config)
    {
        Config = config;
    }

    private void EnsureConfigFileExists()
    {
        if (string.IsNullOrEmpty(ModuleDirectory))
        {
            Console.WriteLine("[Frozen_Elsa] ModuleDirectory is not initialized.");
            return;
        }

        string configPath = Path.Combine(ModuleDirectory, $"{ModuleName}.json");

        try
        {
            if (!File.Exists(configPath) || string.IsNullOrWhiteSpace(File.ReadAllText(configPath)))
            {
                var defaultConfig = new Config();
                var options = new JsonSerializerOptions { WriteIndented = true };
                string json = JsonSerializer.Serialize(defaultConfig, options);
                File.WriteAllText(configPath, json);
                Console.WriteLine($"[Frozen_Elsa] Default config file created at: {configPath}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Frozen_Elsa] Failed to write config: {ex.Message}");
        }
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
                    player.PrintToCenterHtml($"<img src=\"{gifUrl}\">", 10);
                }
            }
        }
    }

    [ConsoleCommand("frozen_activate")]
    public void OnFrozenActivate(CCSPlayerController player, CommandInfo info)
    {
        player.PrintToChat($"❄️ {player.PlayerName} invocou o poder da Elsa!");
        player.ExecuteClientCommand("play sounds/frozen_music2/frozen-go.vsnd_c");

        var origin = player.Pawn?.Value?.AbsOrigin ?? VectorZero;
        var sphere = new SphereEntity(origin, 250);
        DrawLaserBetween(sphere.circleInnerPoints, sphere.circleOutterPoints, 6);

        FreezeEnemiesAt(origin, 300);
    }

    [GameEventHandler]
    public HookResult OnDecoyStarted(EventDecoyStarted @event, GameEventInfo info)
    {
        var decoyOrigin = new Vector(@event.X, @event.Y, @event.Z);
        foreach (var player in Utilities.FindAllEntitiesByDesignerName<CCSPlayerController>("cs_player_controller"))
        {
            if (player?.IsValid == true)
            {
                player.PrintToChat("❄️ Uma granada decoy liberou magia congelante!");
                player.ExecuteClientCommand("play sounds/frozen_music2/frozen-go.vsnd_c");
            }
        }

        var sphereEntity = new SphereEntity(decoyOrigin, 200);
        DrawLaserBetween(sphereEntity.circleInnerPoints, sphereEntity.circleOutterPoints, 5);

        FreezeEnemiesAt(decoyOrigin, 300);
        return HookResult.Continue;
    }

    private void FreezeEnemiesAt(Vector origin, float radius)
    {
        foreach (var player in Utilities.FindAllEntitiesByDesignerName<CCSPlayerController>("cs_player_controller"))
        {
            if (player?.IsValid != true || player.Team == CsTeam.Terrorist)
                continue;

            var targetOrigin = player.Pawn?.Value?.AbsOrigin ?? VectorZero;
            if ((origin - targetOrigin).Length() <= radius)
            {
                player.PrintToChat("💙 Você foi congelado pela magia da Frozen!");
                player.ExecuteClientCommand("play sounds/frozen_music2/freeze.vsnd_c");

                var pawn = player.Pawn?.Value;
                if (pawn != null)
                {
                    pawn.Render = Color.LightBlue;
                    pawn.Teleport(null, null, VectorZero);

                    AddTimer(3.0f, () =>
                    {
                        pawn.Render = Color.White;
                        player.ExecuteClientCommand("play sounds/frozen_music2/unfreeze.vsnd_c");
                        player.PrintToChat("🧊 Você foi descongelado!");
                    });
                }
            }
        }
    }

    [GameEventHandler(HookMode.Pre)]
    public HookResult BulletImpact(EventBulletImpact @event, GameEventInfo info)
    {
        var player = @event.Userid;
        var pawn = player?.Pawn?.Value;

        if (pawn == null || player == null || !HasPermission(player, "Permission"))
            return HookResult.Continue;

        var playerPos = pawn.AbsOrigin;
        if (playerPos == null)
            return HookResult.Continue;

        var bulletOrigin = GetEyePosition(player);
        var bulletDestination = new Vector(@event.X, @event.Y, @event.Z);

        var color = RainbowColors[_random.Next(RainbowColors.Length)];

        DrawLaserBetween(bulletOrigin, bulletDestination, color, 0.2f, 1.0f);
        return HookResult.Continue;
    }

    [GameEventHandler]
    public HookResult PlayerDeath(EventPlayerDeath @event, GameEventInfo info)
    {
        if (@event.Userid == null || @event.Attacker == null)
            return HookResult.Continue;

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

    private void DrawLaserBetween(Vector[] startPos, Vector[] endPos, float duration)
    {
        for (int i = 0; i < endPos.Length; i++)
        {
            var beam = Utilities.CreateEntityByName<CBeam>("beam");
            if (beam == null)
                continue;

            beam.Render = Color.Aqua;
            beam.Width = 2.0f;
            beam.Teleport(startPos[i], RotationZero, VectorZero);
            beam.Teleport(endPos[i], RotationZero, VectorZero);
            beam.DispatchSpawn();
            AddTimer(duration, () => beam.Remove());
        }
    }

    public (int, CBeam?) DrawLaserBetween(Vector startPos, Vector endPos, Color color, float life, float width)
    {
        var beam = Utilities.CreateEntityByName<CBeam>("beam");
        if (beam == null)
            return (-1, null);

        beam.Render = color;
        //beam.Width = width / 2.0f;
        beam.Width = width;
        beam.Teleport(startPos, RotationZero, VectorZero);
        beam.Teleport(endPos, RotationZero, VectorZero);
        beam.DispatchSpawn();
        AddTimer(life, () => beam.Remove());

        BeamEntity = beam;
        return ((int)beam.Index, beam);
    }

    // Antigas Funções
    private bool HasPermission(CCSPlayerController? player, string id)
    {
        string permission = string.Empty;
        string team = string.Empty;

        switch (id)
        {
            case "Permission":
                permission = "@css/custom-permission";
                team = "all";
                break;
            case "Permission2":
                permission = "@css/custom-permission2";
                team = "all";
                break;
        }

        return (string.IsNullOrEmpty(permission) || AdminManager.PlayerHasPermissions(player, permission)) &&
               IsTeamValid(player, team.ToLower());
    }

    private bool IsTeamValid(CCSPlayerController? player, string team)
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
        Color.FromArgb(255, 255, 0, 0),      // Red
        Color.FromArgb(255, 0, 255, 0),      // Green
        Color.FromArgb(255, 0, 0, 255),      // Blue
        Color.FromArgb(255, 0, 255, 255),    // Cyan
        Color.FromArgb(255, 255, 255, 0),    // Yellow
        Color.FromArgb(255, 250, 250, 250), // White
        Color.FromArgb(255, 255, 0, 255),    // Magenta
    };
}