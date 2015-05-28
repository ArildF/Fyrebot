using System;
using Rogue.MetroFire.CampfireClient;

namespace Rogue.Fyrebot
{
	public interface ISettings
	{
		string[] AutoJoinRooms { get; }
		string CommandPrefix { get; }
	}


	public interface IConsole
	{
		void WriteLine(ConsoleColor color, string message, params object[] parms);
		void WriteLine(string message, params object[] parms);
	}

	public interface IAmNotifiedOnLogin
	{
		void LoggedIn(LoginInfo info);
	}

	public interface IFyreModule
	{
		string[] HandlesCommands { get; }
		void ExecuteCommand(int roomId, string command, string args);
	}
}
