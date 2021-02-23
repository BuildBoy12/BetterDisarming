using System.Collections.Generic;
using System.ComponentModel;
using Exiled.API.Interfaces;

namespace BetterDisarming
{
    public class Config : IConfig
    {
        [Description("Whether or not this plugin is enabled.")]
        public bool IsEnabled { get; set; } = true;

        [Description("Whether or not players holding an item in their hand are able to be disarmed.")]
        public bool CanDisarmItems { get; set; } = true;

        [Description("Whether or not disarmed MTF/CI will swap teams when escaping.")]
        public bool TeamSwapping { get; set; } = true;

        [Description(
            "Whether or not those who escape respawn in the escape zone. False means they go to their normal role spawn location")]
        public bool RespawnAtEscape { get; set; } = true;

        [Description(
            "A list of all the roles that can escape, and what they turn into while disarmed. No effect if TeamSwapping is disabled.")]
        public Dictionary<RoleType, RoleType> RoleConversions { get; set; } = new Dictionary<RoleType, RoleType>
        {
            { RoleType.ClassD, RoleType.NtfCadet },
            { RoleType.ChaosInsurgency, RoleType.NtfCadet },
            { RoleType.Scientist, RoleType.ChaosInsurgency },
            { RoleType.FacilityGuard , RoleType.ChaosInsurgency },
            { RoleType.NtfCadet, RoleType.ChaosInsurgency },
            { RoleType.NtfLieutenant, RoleType.ChaosInsurgency },
            { RoleType.NtfCommander, RoleType.ChaosInsurgency }
        };

        public bool Debug { get; set; }
    }
}