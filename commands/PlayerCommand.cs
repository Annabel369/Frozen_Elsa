using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Timers;
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

        player?.PrintToCenterHtml($"[{"<img src='http://26.67.120.79/skinplayersweb/img/657ca211c7f7d.jpg' height='50'>"}]</img><br />");





        //player.GiveNamedItem("weapon_m4a1");
        //player.GiveNamedItem("item_kevlar");
    }


    [ConsoleCommand("css_a", "a")]// !dc
    public void OnCommandAItems(CCSPlayerController? player, CommandInfo commandInfo)
    {
        if (player == null) return;
        if (!player.IsValid) return;

        var callerName = player == null ? "Console" : player.PlayerName;
        player?.ExecuteClientCommand($"play sounds/marius_music/ala-se-amari-yah-aaa-baba-yah-abadon.vsnd");


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
    public void OnGlow(CCSPlayerController? controller, CommandInfo command)
    {
        // Create a glow effect for the player
        AddTimer(0.1f, () =>
        {
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
                prop?.AcceptInput("FollowEntity", caller: prop, activator: controller?.PlayerPawn?.Value, value: "!activator");
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

        }, TimerFlags.REPEAT);

       

        isCatAnimationOn = !isCatAnimationOn;

    }


}
