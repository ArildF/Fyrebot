
using System;
using System.Linq;
using System.Threading;
using Rogue.MetroFire.CampfireClient;

 // ReSharper disable once CheckNamespace
namespace Rogue.Fyrebot.Properties
{
	partial class Settings : ISettings, Rogue.MetroFire.CampfireClient.ISettings, INetworkSettings
	{
		private readonly Lazy<string[]> _autoJoinRooms;

		public Settings()
		{
			_autoJoinRooms = new Lazy<string[]>(() => AutoJoinRooms.Cast<string>().ToArray());
		}

		string[] ISettings.AutoJoinRooms
		{
			get { return _autoJoinRooms.Value; }
		}

		public INetworkSettings Network
		{
			get { return this; }
		}
	}
}
