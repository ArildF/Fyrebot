﻿using System;
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
		bool WantsToHandle(string command);
		void ExecuteCommand(int roomId, string command);
	}

	public interface ICampfireRoom
	{
	}
}
