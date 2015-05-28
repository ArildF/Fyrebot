using System;
using System.Reactive.Linq;
using Castle.Core;
using ReactiveUI;
using Rogue.MetroFire.CampfireClient;
using Rogue.MetroFire.CampfireClient.Serialization;

namespace Rogue.Fyrebot
{
	public class CommandListener : IStartable
	{
		private readonly IMessageBus _bus;
		private readonly ISettings _settings;
		private IDisposable _subscription;

		public CommandListener(IMessageBus bus, ISettings settings)
		{
			_bus = bus;
			_settings = settings;
		}

		public void Start()
		{
			_subscription = _bus.RegisterMessageSource(_bus.Listen<MessagesReceivedMessage>()
				.SelectMany(msg => msg.Messages)
				.Where(msg => msg.Type == MessageType.TextMessage && msg.Body.StartsWith(_settings.CommandPrefix))
				.Select(msg => new FyreBotCommandMessage(msg.RoomId, msg.Body.Substring(_settings.CommandPrefix.Length))));
		}

		public void Stop()
		{
			_subscription.Dispose();
		}
	}
}
