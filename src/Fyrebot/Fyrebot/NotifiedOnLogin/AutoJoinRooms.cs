using System;
using System.Linq;
using System.Reactive.Linq;
using ReactiveUI;
using Rogue.MetroFire.CampfireClient;

namespace Rogue.Fyrebot.NotifiedOnLogin
{
	public class AutoJoinRooms : IAmNotifiedOnLogin
	{
		private readonly IMessageBus _bus;
		private readonly ISettings _settings;
		private readonly IConsole _console;

		public AutoJoinRooms(IMessageBus bus, ISettings settings, IConsole console)
		{
			_bus = bus;
			_settings = settings;
			_console = console;

			_bus.Listen<RoomInfoReceivedMessage>().Subscribe(
				msg => _console.WriteLine(ConsoleColor.Green, "Joined room '{0}'", msg.Room.Name));
			_bus.RegisterMessageSource(_bus.Listen<UserJoinedRoomMessage>()
				.Select(msg => new RequestRoomInfoMessage(msg.Id)));
		}

		public void LoggedIn(LoginInfo info)
		{
			_bus.Listen<RoomListMessage>().Subscribe(HandleRoomListMessage);
			_bus.SendMessage(new RequestRoomListMessage());
		}

		private void HandleRoomListMessage(RoomListMessage message)
		{
			_bus.RegisterMessageSource(
				message.Rooms.Where(r => _settings.AutoJoinRooms.Contains(r.Name))
					.ToObservable()
					.Do(r => _console.WriteLine("Joining room '{0}'", r.Name))
					.Select(r => new RequestJoinRoomMessage(r.Id)));
		}
	}
}
