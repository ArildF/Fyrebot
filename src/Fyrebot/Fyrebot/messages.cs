namespace Rogue.Fyrebot
{
	public class FyreBotCommandMessage
	{
		public int RoomId { get; private set; }
		public string Command { get; private set; }

		public FyreBotCommandMessage(int roomId, string command)
		{
			RoomId = roomId;
			Command = command;
		}
	}
}
