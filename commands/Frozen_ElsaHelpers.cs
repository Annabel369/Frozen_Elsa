using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Admin;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Menu;
using CounterStrikeSharp.API.Modules.Utils;
using System.Linq;
using System;

namespace Frozen_Elsa;

internal static class Frozen_ElsaHelpers
{
    private const int MaxOptionsPerPage = 6;

    public static void ShowMainMenu(CCSPlayerController player, Frozen_Elsa plugin)
    {
        if (player == null || !player.IsValid)
            return;

        if (!AdminManager.PlayerHasPermissions(player, "@css/admin"))
        {
            player.PrintToChat(plugin.Localizer["Frozen_Elsa.NoPermission"]);
            return;
        }

        var mainMenu = new ChatMenu("🧊 Menu de Administração");

        mainMenu.AddMenuOption("👥 Jogadores e Bots", (p, opt) => ShowPlayerList(p, plugin, 0));
        mainMenu.AddMenuOption("🚫 Banir Jogador", (p, opt) => ShowBanMenu(p, plugin));
        mainMenu.AddMenuOption("➕ Dar Admin", (p, opt) => ShowGiveAdminMenu(p, plugin));
        mainMenu.AddMenuOption("➖ Remover Admin", (p, opt) => ShowRemoveAdminMenu(p, plugin));
        mainMenu.AddMenuOption("❌ Sair", (p, opt) => MenuManager.CloseActiveMenu(p));

        MenuManager.OpenChatMenu(player, mainMenu);
    }

    private static void ShowPlayerList(CCSPlayerController player, Frozen_Elsa plugin, int page)
    {
        var allPlayers = Utilities.GetPlayers().Where(p => p.IsValid).ToList();
        var totalPages = (int)Math.Ceiling(allPlayers.Count / (double)MaxOptionsPerPage);
        var pagedPlayers = allPlayers.Skip(page * MaxOptionsPerPage).Take(MaxOptionsPerPage);

        var listMenu = new ChatMenu($"👥 Jogadores e Bots (Página {page + 1}/{totalPages})");

        foreach (var target in pagedPlayers)
        {
            var tipo = target.IsBot ? "BOT" : "Player";
            var time = target.Team == CsTeam.Terrorist ? "TR" : target.Team == CsTeam.CounterTerrorist ? "CT" : "Spec";
            var isAdmin = AdminManager.PlayerHasPermissions(target, "@css/admin") ? "✅ Admin" : "❌";
            listMenu.AddMenuOption($"{target.PlayerName} [{tipo} | {time}] {isAdmin}", (p, opt) =>
            {
                ShowPlayerActionsMenu(p, plugin, target);
            });
        }

        if (page > 0)
            listMenu.AddMenuOption("⬅ Página anterior", (p, opt) => ShowPlayerList(p, plugin, page - 1));
        if (page < totalPages - 1)
            listMenu.AddMenuOption("➡ Próxima página", (p, opt) => ShowPlayerList(p, plugin, page + 1));

        listMenu.AddMenuOption("🔙 Voltar", (p, opt) => ShowMainMenu(p, plugin));
        MenuManager.OpenChatMenu(player, listMenu);
    }

    private static void ShowPlayerActionsMenu(CCSPlayerController admin, Frozen_Elsa plugin, CCSPlayerController target)
    {
        var actionsMenu = new ChatMenu($"⚙ Ações para {target.PlayerName}");

        actionsMenu.AddMenuOption("👢 Kickar", (p, opt) =>
        {
            Server.ExecuteCommand($"kickid {target.UserId} \"Kickado pelo admin\"");
            LogAction(admin, $"Kickou {target.PlayerName}");
        });

        actionsMenu.AddMenuOption("🔇 Mutar", (p, opt) =>
        {
            Server.ExecuteCommand($"mute {target.UserId}");
            LogAction(admin, $"Mutou {target.PlayerName}");
        });

        actionsMenu.AddMenuOption("🌀 Teleportar até admin", (p, opt) =>
        {
            if (admin.PlayerPawn?.Value != null && target.PlayerPawn?.Value != null)
            {
                target.PlayerPawn.Value.Teleport(admin.PlayerPawn.Value.AbsOrigin, admin.PlayerPawn.Value.AbsRotation);
                LogAction(admin, $"Teleportou {target.PlayerName} até si");
            }
        });

        actionsMenu.AddMenuOption("❄ Congelar", (p, opt) =>
        {
            if (target.PlayerPawn?.Value != null)
            {
                target.PlayerPawn.Value.MoveType = MoveType_t.MOVETYPE_NONE;
                LogAction(admin, $"Congelou {target.PlayerName}");
            }
        });

        actionsMenu.AddMenuOption("🔥 Descongelar", (p, opt) =>
        {
            if (target.PlayerPawn?.Value != null)
            {
                target.PlayerPawn.Value.MoveType = MoveType_t.MOVETYPE_WALK;
                LogAction(admin, $"Descongelou {target.PlayerName}");
            }
        });

        actionsMenu.AddMenuOption("🔁 Mover para CT", (p, opt) =>
        {
            target.SwitchTeam(CsTeam.CounterTerrorist);
            LogAction(admin, $"Moveu {target.PlayerName} para CT");
        });

        actionsMenu.AddMenuOption("🔁 Mover para TR", (p, opt) =>
        {
            target.SwitchTeam(CsTeam.Terrorist);
            LogAction(admin, $"Moveu {target.PlayerName} para TR");
        });

        actionsMenu.AddMenuOption("🔙 Voltar", (p, opt) => ShowPlayerList(p, plugin, 0));
        MenuManager.OpenChatMenu(admin, actionsMenu);
    }

    private static void LogAction(CCSPlayerController admin, string action)
    {
        Console.WriteLine($"[Frozen_Elsa] {admin.PlayerName} => {action}");
        admin.PrintToChat($"✅ Ação realizada: {action}");
    }

    private static void ShowBanMenu(CCSPlayerController player, Frozen_Elsa plugin)
    {
        var banMenu = new ChatMenu("🚫 Banir Jogador");

        foreach (var target in Utilities.GetPlayers().Where(p => p.IsValid && !p.IsBot && p.SteamID != player.SteamID))
        {
            banMenu.AddMenuOption($"{target.PlayerName} ({target.SteamID})", (p, opt) =>
            {
                Server.ExecuteCommand($"css_ban {target.SteamID} \"Banido pelo menu\"");
                LogAction(player, $"Baniu {target.PlayerName}");
            });
        }

        banMenu.AddMenuOption("🔙 Voltar", (p, opt) => ShowMainMenu(p, plugin));
        MenuManager.OpenChatMenu(player, banMenu);
    }

    private static void ShowGiveAdminMenu(CCSPlayerController player, Frozen_Elsa plugin)
    {
        var adminMenu = new ChatMenu("➕ Dar Admin");

        foreach (var target in Utilities.GetPlayers().Where(p => p.IsValid && !p.IsBot && p.SteamID != player.SteamID))
        {
            adminMenu.AddMenuOption($"{target.PlayerName} ({target.SteamID})", (p, opt) =>
            {
                Server.ExecuteCommand($"css_admin {target.SteamID}");
                LogAction(player, $"Deu admin para {target.PlayerName}");
            });
        }

        adminMenu.AddMenuOption("🔙 Voltar", (p, opt) => ShowMainMenu(p, plugin));
        MenuManager.OpenChatMenu(player, adminMenu);
    }

    private static void ShowRemoveAdminMenu(CCSPlayerController player, Frozen_Elsa plugin)
    {
        var removeAdminMenu = new ChatMenu("➖ Remover Admin");

        foreach (var target in Utilities.GetPlayers().Where(p => p.IsValid && !p.IsBot && p.SteamID != player.SteamID))
        {
            removeAdminMenu.AddMenuOption($"{target.PlayerName} ({target.SteamID})", (p, opt) =>
            {
                Server.ExecuteCommand($"css_removeadmin {target.SteamID}");
                LogAction(player, $"Removeu admin de {target.PlayerName}");
            });
        }

        removeAdminMenu.AddMenuOption("🔙 Voltar", (p, opt) => ShowMainMenu(p, plugin));
        MenuManager.OpenChatMenu(player, removeAdminMenu);
    }
}