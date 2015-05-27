using System;
using Rogue.MetroFire.CampfireClient;

namespace Rogue.Fyrebot
{
	public interface ISettings
	{
		string[] AutoJoinRooms { get; }
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
}
