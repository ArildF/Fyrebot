using System.Linq;
using Rogue.MetroFire.CampfireClient;

namespace Rogue.Fyrebot
{
	class Program
	{
		static void Main(string[] args)
		{
			var accountName = args.First();
			var token = args.Skip(1).First();
			var info = new LoginInfo(accountName, token);

			var bootstrapper = new Bootstrapper();
			var processor = bootstrapper.Bootstrap();

			processor.Login(info);

			processor.ApplicationRunning.WaitOne();
		}
	}
}
