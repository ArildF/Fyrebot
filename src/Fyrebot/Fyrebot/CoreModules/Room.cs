using System;
using System.Reactive.Linq;
using CommandLine;
using ReactiveUI;
using Rogue.MetroFire.CampfireClient;

namespace Rogue.Fyrebot.CoreModules
{
	public class JoinRoomOptions
	{
		[ValueOption(0)]
		public string Room { get; set; }
	}

	public class LeaveRoomOptions
	{
		[ValueOption(0)]
		public string Room { get; set; }
	}

	public class Room : IFyreModule, IFyreCommand<JoinRoomOptions>, IFyreCommand<LeaveRoomOptions>
	{
		private readonly CommandMap _map;
		private readonly IMessageBus _bus;

		public Room(CommandMap map, IMessageBus bus)
		{
			_map = map;
			_bus = bus;
			_map.MapCommand<JoinRoomOptions>("join").To(this);
			_map.MapCommand<LeaveRoomOptions>("leave").To(this);
		}

		

		public bool WantsToHandle(string command)
		{
			return command.StartsWith("room ");
		}

		public void ExecuteCommand(int roomId, string command)
		{
			_map.Handle(roomId, command.Substring("room ".Length));
		}

		public void Execute(int roomId, JoinRoomOptions arguments)
		{
			_bus.Listen<RoomListMessage>().SelectMany(msg => msg.Rooms)
				.Where(r => r.Name.Equals(arguments.Room, StringComparison.InvariantCultureIgnoreCase))
				.Take(1)
				.Subscribe(r => _bus.SendMessage(new RequestJoinRoomMessage(r.Id)));

			_bus.SendMessage(new RequestRoomListMessage());
			
		}

		public void Execute(int roomId, LeaveRoomOptions arguments)
		{
			_bus.Listen<RoomListMessage>().SelectMany(msg => msg.Rooms)
				.Where(r => r.Name.Equals(arguments.Room, StringComparison.InvariantCultureIgnoreCase))
				.Take(1)
				.Subscribe(r => _bus.SendMessage(new RequestLeaveRoomMessage(r.Id)));

			_bus.SendMessage(new RequestRoomListMessage());
		}
	}
}
