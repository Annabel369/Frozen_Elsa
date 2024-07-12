using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Core.Translations;
using CounterStrikeSharp.API.Modules.Admin;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Commands.Targeting;
using CounterStrikeSharp.API.Modules.Timers;
using CounterStrikeSharp.API.Modules.Entities.Constants;
using CounterStrikeSharp.API.Modules.Memory;
using CounterStrikeSharp.API.Modules.Utils;


using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Drawing;



namespace Frozen_Elsa;

public partial class Frozen_Elsa
{

    bool isCatAnimationOn = false;

    [ConsoleCommand("css_dc", "dc")]// !dc
    public void OnCommandGiveItems(CCSPlayerController? player, CommandInfo commandInfo)
    {
        if (player == null) return;
        if (!player.IsValid) return;


        var callerName = player == null ? "Console" : player.PlayerName;
        

        //Server.ExecuteCommand($"css_freeze {callerName} 9");
        //player?.PrintToChat($"Freeze {callerName} 9 secord");


        //player?.ExecuteClientCommand($"play sounds/ui/counter_beep.vsnd");
        
        player?.ExecuteClientCommand($"play sounds/frozen_music2/frozen-ice.vsnd_c");

        player?.GiveNamedItem("weapon_Decoy");
        Globals.SiteImage = "https://raw.githubusercontent.com/oqyh/cs2-MVP-Sounds-GoldKingZ/def5df4f333fc95da1e6de92a5c137fa5006ebad/Resources/9mm.gif";
        RegisterListener<Listeners.OnTick>(OnTick);
        shouldShowImage = true;
        AddTimer(7, () =>
        {
            shouldShowImage = false;
        });
    }

    [ConsoleCommand("css_he", "he")]// !he
    [RequiresPermissions("@css/root")]
     public void OnCommandGiveHe(CCSPlayerController? player, CommandInfo commandInfo)
    {
        if (player == null) return;
        if (!player.IsValid) return;
        //player?.PlayerPawn.Value?.LifeState == (byte)LifeState_t.LIFE_ALIVE;
        var callerName = player == null ? "Console" : player.PlayerName;
        
        player?.ExecuteClientCommand($"play sounds/frozen_music2/frozen-ice.vsnd_c");
        player?.GiveNamedItem("weapon_hegrenade");
        Server.ExecuteCommand($"ammo_grenade_limit_total 5");
        Server.ExecuteCommand($"sv_grenade_trajectory_prac_pipreview 1");

        Globals.SiteImage = "https://raw.githubusercontent.com/oqyh/cs2-MVP-Sounds-GoldKingZ/main/Resources/skull1.gif";
        RegisterListener<Listeners.OnTick>(OnTick);
        shouldShowImage = true;
        AddTimer(10, () =>
        {
            shouldShowImage = false;
        });
    }

    [ConsoleCommand("css_smoke", "smoke")]// !smoke
    [RequiresPermissions("@css/root")]
     public void OnCommandGiveSmoke(CCSPlayerController? player, CommandInfo commandInfo)
    {
        if (player == null) return;
        if (!player.IsValid) return;

        var callerName = player == null ? "Console" : player.PlayerName;
        
        player?.ExecuteClientCommand($"play sounds/frozen_music2/frozen-ice.vsnd_c");
        player?.GiveNamedItem("weapon_smokegrenade");
        Server.ExecuteCommand($"ammo_grenade_limit_total 5");
        Server.ExecuteCommand($"sv_grenade_trajectory_prac_pipreview 1");

        Globals.SiteImage = "https://gifman.net/wp-content/uploads/2019/06/coninga-batendo-palmas.gif";
        RegisterListener<Listeners.OnTick>(OnTick);
        shouldShowImage = true;
        AddTimer(7, () =>
        {
            shouldShowImage = false;
        });
    }

    [ConsoleCommand("css_molotov", "molotov")]// !molotov
    [RequiresPermissions("@css/root")]
     public void OnCommandGiveMolotov(CCSPlayerController? player, CommandInfo commandInfo)
    {
        if (player == null) return;
        if (!player.IsValid) return;

        var callerName = player == null ? "Console" : player.PlayerName;
        
        player?.ExecuteClientCommand($"play sounds/frozen_music2/frozen-ice.vsnd_c");
        player?.GiveNamedItem("weapon_smokegrenade");
        Server.ExecuteCommand($"ammo_grenade_limit_total 5");
        Server.ExecuteCommand($"sv_grenade_trajectory_prac_pipreview 1");

        Globals.SiteImage = "https://gifman.net/wp-content/uploads/2019/07/bob-esponja-32.gif";
        RegisterListener<Listeners.OnTick>(OnTick);
        shouldShowImage = true;
        AddTimer(7, () =>
        {
            shouldShowImage = false;
        });
    }

    [ConsoleCommand("css_bang", "bang")]// !bang
    [RequiresPermissions("@css/root")]
     public void OnCommandGiveBang(CCSPlayerController? player, CommandInfo commandInfo)
    {
        if (player == null) return;
        if (!player.IsValid) return;

        var callerName = player == null ? "Console" : player.PlayerName;
        
        player?.ExecuteClientCommand($"play sounds/frozen_music2/frozen-ice.vsnd_c");
        player?.GiveNamedItem("weapon_flashbang");
        Server.ExecuteCommand($"ammo_grenade_limit_total 5");
        Server.ExecuteCommand($"sv_grenade_trajectory_prac_pipreview 1");

        Globals.SiteImage = "https://gifman.net/wp-content/uploads/2019/07/lula-molusco-02.gif";
        RegisterListener<Listeners.OnTick>(OnTick);
        shouldShowImage = true;
        AddTimer(7, () =>
        {
            shouldShowImage = false;
        });
    }


    [ConsoleCommand("css_a", "a")]// !dc
    [RequiresPermissions("@css/root")]
    public void OnCommandAItems(CCSPlayerController? player, CommandInfo commandInfo)
    {
        if (player == null) return;
        if (!player.IsValid) return;

        var callerName = player == null ? "Console" : player.PlayerName;
        player?.ExecuteClientCommand($"play sounds/marius_music/ala-se-amari-yah-aaa-baba-yah-abadon.vsnd");


    }

    [ConsoleCommand("css_spec")]
    [RequiresPermissions("@css/root")]
    [CommandHelper(minArgs: 1, usage: "<#userid or name>", whoCanExecute: CommandUsage.CLIENT_AND_SERVER)]
    public void OnMariusCommand(CCSPlayerController? caller, CommandInfo command)
    {
            var callerName = caller == null ? "Console" : caller.PlayerName;
            if (command== null) return;
            Server.ExecuteCommand($"spec_player {callerName}");
            
    }

    [ConsoleCommand("css_q", "q")]// !dc
    public void Onwarmup_end(CCSPlayerController? player, CommandInfo commandInfo)
    {
        if (player == null) return;
        if (!player.IsValid) return;

        var callerName = player == null ? "Console" : player.PlayerName;
        Server.ExecuteCommand("mp_warmup_end");



    }
    [ConsoleCommand("css_u", "u")]
    [RequiresPermissions("@css/root")]
    public void OnGlow(CCSPlayerController? controller, CommandInfo command)
    {
        // Create a glow effect for the player
       
            var prop = Utilities.CreateEntityByName<CCSPlayerPawn>("prop_dynamic");
            if (prop == null)
            {
                return;
            }
           
            if (isCatAnimationOn)
            {
                // Stop the cat animation
                // ... (Implement animation stopping logic) ...

                // Remove the prop itself
                prop.Remove();
                prop.AcceptInput("FollowEntity", caller: prop, activator: controller?.PlayerPawn?.Value, value: "!activator");
            }
            else
            {
                // Start the cat animation
                // ... (Implement animation starting logic) ...
                prop?.SetModel("characters/models/nozb1/skeletons_player_model/skeleton_player_model_1/skeleton_nozb1_pm.vmdl");

                // Set position and input acceptance
                prop?.Teleport(controller?.PlayerPawn?.Value?.AbsOrigin, new QAngle(0, 0, 0), new Vector(0, 0, 0));
                prop!.DispatchSpawn();

                // Configure glow effect
                prop.Render = Color.White;
                prop.Render = Color.FromArgb(1, 255, 255, 255);
                prop.Glow.GlowColorOverride = Color.Red;
                prop.Spawnflags = 256U;
                prop.RenderMode = RenderMode_t.kRenderGlow;
                prop.Glow.GlowRange = 5000;
                prop.Glow.GlowTeam = -1;
                prop.Glow.GlowType = 3;
                prop.Glow.GlowRangeMin = 3;

            }

        isCatAnimationOn = !isCatAnimationOn;
        return;

    }


}
