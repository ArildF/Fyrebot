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
		private readonly IConsole _console;
		private IDisposable _subscription;

		public CommandDispatcher(IMessageBus bus, IList<IFyreModule> modules, IConsole console)
		{
			_bus = bus;
			_modules = modules;
			_console = console;
		}

		public void Start()
		{
			_console.WriteLine("Command dispatcher starting");
			var query =
				from message in _bus.Listen<FyreBotCommandMessage>()
				from handler in _modules.Where(m => m.WantsToHandle(message.Command))
				select new {Handler = handler, message.RoomId, message.Command};

			_subscription = query.Subscribe(handler =>
			{
				try
				{
					if (handler.Handler != null)
					{
						handler.Handler.ExecuteCommand(handler.RoomId, handler.Command);
					}
				}
				catch (Exception e)
				{
					_console.WriteLine(ConsoleColor.Red, "Exception: {0}", e);
					_bus.SendMessage(new RequestSpeakInRoomMessage(handler.RoomId, "wat"));
				}
			});
		}

		public void Stop()
		{
			_subscription.Dispose();
		}
	}
}
