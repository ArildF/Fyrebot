using System;
using System.Linq;
using System.Reactive.Linq;
using Raven.Client;
using ReactiveUI;
using Rogue.MetroFire.CampfireClient;
using Rogue.MetroFire.CampfireClient.Serialization;

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
		}

		public void LoggedIn(LoginInfo info)
		{
			_bus.Listen<RoomListMessage>().SubscribeOnce(HandleRoomListMessage);
			_bus.SendMessage(new RequestRoomListMessage());
		}

		private void HandleRoomListMessage(RoomListMessage message)
		{
			DisplayRoomList(message.Rooms);

			var rooms = GetRoomsToAutoJoin();

			message.Rooms.Where(r => rooms.Rooms.Contains(r.Name))
				.ToObservable()
				.Do(r => _console.WriteLine("Autojoining room '{0}'", r.Name))
				.Subscribe(r => _bus.SendMessage(new RequestJoinRoomMessage(r.Id)));
		}

		private void DisplayRoomList(Room[] rooms)
		{
			var names = String.Join(Environment.NewLine, rooms.Select(r => " " + r.Name));
			_console.WriteLine("Available rooms:\r\n" + names);
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
