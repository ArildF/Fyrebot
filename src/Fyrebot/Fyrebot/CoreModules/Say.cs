using ReactiveUI;
using Rogue.MetroFire.CampfireClient;

namespace Rogue.Fyrebot.CoreModules
{
	public class Say : IFyreModule
	{
		private readonly IMessageBus _bus;

		public Say(IMessageBus bus)
		{
			_bus = bus;
		}


		public string[] HandlesCommands
		{
			get { return new []{"say"}; }
		}

		public void ExecuteCommand(int roomId, string command, string args)
		{
			_bus.SendMessage(new RequestSpeakInRoomMessage(roomId, args));
		}
	}
}
