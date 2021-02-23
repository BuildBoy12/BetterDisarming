using System.Collections.Generic;
using Exiled.API.Features;
using Exiled.Events.EventArgs;
using MEC;
using UnityEngine;

namespace BetterDisarming
{
    public class Methods
    {
        private readonly Plugin plugin;
        public Methods(Plugin plugin) => this.plugin = plugin;
        
        private Vector3 worldPosition = Vector3.zero;

        public Vector3 GetWorldPosition(GameObject obj)
        {
            if (worldPosition == Vector3.zero)
                worldPosition = obj.GetComponent<Escape>().worldPosition;
            return worldPosition;
        }

        private List<Player> recentEscapes = new List<Player>();

        public IEnumerator<float> CheckEscape()
        {
            for (;;)
            {
                yield return Timing.WaitForSeconds(1f);

                foreach (Player player in Player.List)
                {
                    if (recentEscapes.Contains(player))
                        continue;
                    if (player.Role == RoleType.ClassD || player.Role == RoleType.Scientist)
                        continue;
                    if (!player.IsCuffed)
                        continue;
                    if (!plugin.Config.RoleConversions.ContainsKey(player.Role))
                        continue;
                    if (Vector3.Distance(player.Position, GetWorldPosition(player.GameObject)) > Escape.radius * 1.25f)
                        continue;

                    recentEscapes.Add(player);
                    EscapingEventArgs ev = new EscapingEventArgs(player, plugin.Config.RoleConversions[player.Role]);
                    Exiled.Events.Handlers.Player.OnEscaping(ev);

                    Timing.CallDelayed(5f, () => recentEscapes.Remove(player));
                }
            }
        }
    }
}