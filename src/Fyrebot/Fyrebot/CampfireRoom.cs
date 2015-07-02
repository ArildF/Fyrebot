using System;
using System.Linq;
using System.Reactive.Linq;
using ReactiveUI;
using Rogue.MetroFire.CampfireClient;
using Rogue.MetroFire.CampfireClient.Serialization;

namespace Rogue.Fyrebot
{
	public class CampfireRoom : ICampfireRoom
	{
		private readonly Room _room;
		private readonly IMessageBus _bus;
		private readonly IConsole _console;

		public CampfireRoom(Room room, IMessageBus bus, IConsole console)
		{
			_room = room;
			_bus = bus;
			_console = console;

			bus.Listen<MessagesReceivedMessage>().Where(m => m.RoomId == room.Id)
				.Subscribe(msg => WriteReceivedRoomMessages(msg.Messages));
			bus.SendMessage(new RequestStartStreamingMessage(room.Id));
		}

		private void WriteReceivedRoomMessages(Message[] messages)
		{
			foreach (var message in messages.Where(msg => msg.Type == MessageType.TextMessage))
			{
				Console.WriteLine("{0}: {1}", message.UserId, message.Body);
			}
		}
	}
}