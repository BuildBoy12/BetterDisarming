using System;
using Exiled.API.Features;
using MEC;
using MapEvents = Exiled.Events.Handlers.Map;
using PlayerEvents = Exiled.Events.Handlers.Player;
using Scp049Events = Exiled.Events.Handlers.Scp049;
using Scp079Events = Exiled.Events.Handlers.Scp079;
using Scp096Events = Exiled.Events.Handlers.Scp096;
using Scp106Events = Exiled.Events.Handlers.Scp106;
using Scp914Events = Exiled.Events.Handlers.Scp914;
using ServerEvents = Exiled.Events.Handlers.Server;
using WarheadEvents = Exiled.Events.Handlers.Warhead;

namespace BetterDisarming
{
    public class Plugin : Plugin<Config>
    {
        public override string Author { get; } = "Galaxy119";
        public override string Name { get; } = "BetterDisarming";
        public override string Prefix { get; } = "BetterDisarming";
        public override Version Version { get; } = new Version(1, 0, 0);
        public override Version RequiredExiledVersion { get; } = new Version(2, 1, 30);

        public Methods Methods { get; private set; }
        public EventHandlers EventHandlers { get; private set; }
        public static Plugin Singleton;

        public override void OnEnabled()
        {
            Singleton = this;
            EventHandlers = new EventHandlers(this);
            Methods = new Methods(this);

            Exiled.Events.Handlers.Player.Escaping += EventHandlers.OnEscaping;
            Exiled.Events.Handlers.Player.Handcuffing += EventHandlers.OnHandcuffing;
            Exiled.Events.Handlers.Server.WaitingForPlayers += EventHandlers.OnWaitingForPlayers;
            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            Timing.KillCoroutines("CheckEscape");
            Exiled.Events.Handlers.Player.Escaping -= EventHandlers.OnEscaping;
            Exiled.Events.Handlers.Player.Handcuffing -= EventHandlers.OnHandcuffing;
            Exiled.Events.Handlers.Server.WaitingForPlayers -= EventHandlers.OnWaitingForPlayers;
            EventHandlers = null;
            Methods = null;
            
            base.OnDisabled();
        }
    }
}