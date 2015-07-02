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

		public bool WantsToHandle(string command)
		{
			return command.StartsWith("say ");
		}


		public void ExecuteCommand(int roomId, string command)
		{
			string say = command.Substring("say ".Length);
			_bus.SendMessage(new RequestSpeakInRoomMessage(roomId, say));
		}
	}
}
