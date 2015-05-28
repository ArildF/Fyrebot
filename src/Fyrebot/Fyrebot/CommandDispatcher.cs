using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using Castle.Core;
using ReactiveUI;
using Rogue.MetroFire.CampfireClient;

namespace Rogue.Fyrebot
{
	public class CommandDispatcher : IStartable
	{
		private readonly IMessageBus _bus;
		private readonly IList<IFyreModule> _modules;
		private IDisposable _subscription;

		public CommandDispatcher(IMessageBus bus, IList<IFyreModule> modules)
		{
			_bus = bus;
			_modules = modules;
		}

		public void Start()
		{
			var query =
				from message in _bus.Listen<FyreBotCommandMessage>()
				let components = message.Command.Split(new char[]{' '}, StringSplitOptions.RemoveEmptyEntries)
				let firstCommand = components.FirstOrDefault()
				let args = message.Command.Trim().Substring(firstCommand != null ? firstCommand.Length : 0).Trim()
				let handler = _modules.FirstOrDefault(m => m.HandlesCommands.Contains(firstCommand))
				select new {Handler = handler, Command = firstCommand, message.RoomId, Args = args};

			_subscription = query.Subscribe(handler =>
			{
				if (handler.Handler != null)
				{
					handler.Handler.ExecuteCommand(handler.RoomId, handler.Command, handler.Args);
				}
			});
		}

		public void Stop()
		{
			_subscription.Dispose();
		}
	}
}
