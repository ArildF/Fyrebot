using System;
using System.Collections;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Threading;
using ReactiveUI;
using Rogue.MetroFire.CampfireClient;

namespace Rogue.Fyrebot
{
	public class LoginProcessor
	{
		private readonly ISettings _settings;
		private readonly IMessageBus _bus;
		private readonly IConsole _console;
		private readonly IEnumerable<IAmNotifiedOnLogin> _notifiedOnLogin;
		private readonly ManualResetEvent _applicationRunning = new ManualResetEvent(false);

		public LoginProcessor(ISettings settings, IMessageBus bus, IConsole console, 
			IEnumerable<IAmNotifiedOnLogin> notifiedOnLogin)
		{
			_settings = settings;
			_bus = bus;
			_console = console;
			_notifiedOnLogin = notifiedOnLogin;
		}

		public void Login(LoginInfo info)
		{

			_bus.RegisterSourceAndHandleReply<RequestLoginMessage, RequestLoginResponse>(
				Observable.Return(new RequestLoginMessage(info), RxApp.TaskpoolScheduler),
				res => HandleLoginResponse(res, info));
		}

		public WaitHandle ApplicationRunning
		{
			get{return _applicationRunning;}
		}


		private void HandleLoginResponse(RequestLoginResponse obj, LoginInfo info)
		{
			if (!obj.SuccessFul)
			{
				_console.WriteLine(ConsoleColor.Red, "Login failed");
				_applicationRunning.Set();
			}
			_console.WriteLine(ConsoleColor.Green, "Login successful");
			foreach (var amNotifiedOnLogin in _notifiedOnLogin)
			{
				amNotifiedOnLogin.LoggedIn(info);
			}
		}
	}
}