using System;
using System.Collections.Generic;
using Exiled.API.Features;
using Exiled.Events.EventArgs;
using MEC;
using UnityEngine;

namespace BetterDisarming
{
    public class EventHandlers
    {
        private readonly Plugin plugin;
        public EventHandlers(Plugin plugin) => this.plugin = plugin;
        public bool First = true;
        public List<Player> RecentEscapes = new List<Player>();

        public void OnHandcuffing(HandcuffingEventArgs ev)
        {
            if (!plugin.Config.CanDisarmItems && ev.Target.CurrentItem.id != ItemType.None)
                ev.IsAllowed = false;
        }

        public void OnEscaping(EscapingEventArgs ev)
        {
            try
            {
                if (!ev.Player.IsCuffed)
                {
                    Log.Debug($"{ev.Player.Nickname} is not cuffed.", plugin.Config.Debug);
                    return;
                }

                if (RecentEscapes.Contains(ev.Player))
                {
                    Log.Debug($"{ev.Player.Nickname} is in the recent escapes list.", plugin.Config.Debug);
                    return;
                }
                
                RecentEscapes.Add(ev.Player);
                Log.Debug($"{ev.Player.Nickname} added to recent escapes.", plugin.Config.Debug);

                if (plugin.Config.TeamSwapping && (ev.Player.Role == RoleType.Scientist || ev.Player.Role == RoleType.ClassD))
                {
                    Log.Debug($"{ev.Player.Nickname} teamswap enabled. Current: {ev.NewRole}", plugin.Config.Debug);
                    if (plugin.Config.RoleConversions.ContainsKey(ev.Player.Role))
                        ev.NewRole = plugin.Config.RoleConversions[ev.Player.Role];
                    else
                        Log.Warn(
                            $"{ev.Player.Nickname} escaped while cuffed, and Team Swapping is enabled, but there was no role conversion defined for {ev.Player.Role}. They will not swap teams.");

                    Log.Debug($"{ev.Player.Nickname} teamswap New: {ev.NewRole}", plugin.Config.Debug);
                }

                if (plugin.Config.RespawnAtEscape)
                {
                    Log.Debug($"{ev.Player.Nickname} respawn at escape.", plugin.Config.Debug);
                    ev.IsAllowed = false;
                    ev.Player.DropItems();
                    ev.Player.SetRole(ev.NewRole, true);

                    foreach (ItemType startItem in ev.Player.ReferenceHub.characterClassManager.Classes
                        .SafeGet(ev.NewRole).startItems)
                        ev.Player.AddItem(startItem);
                }
                
                Player player = ev.Player;
                Timing.CallDelayed(5f, () =>
                {
                    if (player == null)
                    {
                        Log.Debug($"Player is null, skipping removal.", plugin.Config.Debug);
                        return;
                    }

                    RecentEscapes.Remove(player);
                    Log.Debug($"{player.Nickname} removed from recent escapes.", plugin.Config.Debug);
                });
            }
            catch (Exception e)
            {
                Log.Error($"{e}\n{e.StackTrace}");
            }
        }

        public void OnWaitingForPlayers()
        {
            if (First)
            {
                Timing.RunCoroutine(plugin.Methods.CheckEscape(), "CheckEscape");
                First = false;
            }
        }
    }
}