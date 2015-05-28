using System;
using System.Linq;
using System.Reactive.Linq;
using Raven.Client;
using ReactiveUI;
using Rogue.MetroFire.CampfireClient;

namespace Rogue.Fyrebot.NotifiedOnLogin
{
	public class AutoJoinRooms : IAmNotifiedOnLogin
	{
		private readonly IMessageBus _bus;
		private readonly ISettings _settings;
		private readonly IConsole _console;
		private readonly IDocumentStore _store;

		public AutoJoinRooms(IMessageBus bus, ISettings settings, IConsole console, IDocumentStore store)
		{
			_bus = bus;
			_settings = settings;
			_console = console;
			_store = store;

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
			var rooms = GetRoomsToAutoJoin();
			_bus.RegisterMessageSource(
				message.Rooms.Where(r => rooms.Rooms.Contains(r.Name))
					.ToObservable()
					.Do(r => _console.WriteLine("Joining room '{0}'", r.Name))
					.Select(r => new RequestJoinRoomMessage(r.Id)));
		}

		private AutoJoinRoomsData GetRoomsToAutoJoin()
		{
			using (var session = _store.OpenSession())
			{
				var data = session.Load<AutoJoinRoomsData>("core/autojoinrooms");
				if (data == null)
				{
					data = new AutoJoinRoomsData {Id = "core/autojoinrooms", Rooms = new string[] {}};
					session.Store(data);
				}
				session.SaveChanges();
				return data;
			}
		}
	}

	public class AutoJoinRoomsData
	{
		public string Id { get; set; }
		public string[] Rooms { get; set; }
	}
}
