using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using Castle.Core;
using ReactiveUI;
using Rogue.MetroFire.CampfireClient;
using Rogue.MetroFire.CampfireClient.Serialization;

namespace Rogue.Fyrebot
{
	public class CampfireRoomCreator : IStartable
	{
		private readonly IConsole _console;

		private readonly IDictionary<int, ICampfireRoom> _activeRooms = 
			new Dictionary<int, ICampfireRoom>();

		public CampfireRoomCreator(IMessageBus bus, IConsole console, Func<Room, ICampfireRoom> roomFactory)
		{
			_console = console;

			bus.Listen<RoomInfoReceivedMessage>()
				.Where(msg => !_activeRooms.Keys.Contains(msg.Room.Id))
				.Do(msg => _console.WriteLine(ConsoleColor.Green, "Joined room {0}", msg.Room.Name))
				.Subscribe(msg =>
				{
					_activeRooms.Add(msg.Room.Id, roomFactory(msg.Room));
				});
			bus.RegisterMessageSource(bus.Listen<UserJoinedRoomMessage>()
			
				.Select(msg => new RequestRoomInfoMessage(msg.Id)));
		}

		public void Start()
		{
		}

		public void Stop()
		{
		}
	}
}
