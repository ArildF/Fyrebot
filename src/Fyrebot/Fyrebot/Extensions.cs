using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Castle.Core.Internal;
using ReactiveUI;
using Rogue.MetroFire.CampfireClient;

namespace Rogue.Fyrebot
{
	public static class Extensions
	{
		public static IDisposable RegisterSourceAndHandleReply<TRequest, TResponse>(
			this IMessageBus self,
			IObservable<TRequest> req,
			Action<TResponse> responseHandler, Action<Exception> errorHandler = null)
			where TRequest : CorrelatedMessage
			where TResponse : CorrelatedReply
		{
			var coll = new CompositeDisposable();
			Guid currentCorrelation = Guid.Empty;
			if (errorHandler != null)
			{
				coll.Add(self.Listen<CorrelatedExceptionMessage>().Where(msg => msg.CorrelationId == currentCorrelation)
					.Subscribe(msg => errorHandler(msg.Exception)));
			}
			coll.Add(self.Listen<TResponse>().Where(msg => msg.CorrelationId == currentCorrelation).Subscribe(responseHandler));

			coll.Add(self.RegisterMessageSource(req.Do(msg => currentCorrelation = msg.CorrelationId)));

			return coll;
		}

		public static void SubscribeOnce<T>(this IObservable<T> self, Action<T> onNext)
		{
			IDisposable disposable = null;
			disposable = self.Subscribe(v =>
			{
				onNext(v);
				if (disposable != null)
				{
					disposable.Dispose();
				}
			});
		}

		public static string AsKiloBytes(this long d)
		{
			return String.Format("{0:0.0}", d / 1024);
		}

		public static string AsMegaBytes(this long d)
		{
			return String.Format("{0:0.0}", d / 1024 / 1024);
		}

		public static long MegaBytes(this long megaBytes)
		{
			return megaBytes * 1024 * 1024;
		}

		public static long MegaBytes(this int megaBytes)
		{
			return ((long)megaBytes).MegaBytes();
		}

		public static long KiloBytes(this long kiloBytes)
		{
			return kiloBytes * 1024;
		}

		// based on code from http://stackoverflow.com/a/298990
		public static IEnumerable<string> Split(this string str,
											Func<char, bool> controller)
		{
			int nextPiece = 0;

			for (int c = 0; c < str.Length; c++)
			{
				if (controller(str[c]))
				{
					yield return str.Substring(nextPiece, c - nextPiece);
					nextPiece = c + 1;
				}
			}

			yield return str.Substring(nextPiece);
		}

		public static string TrimMatchingQuotes(this string input, char quote)
		{
			if ((input.Length >= 2) &&
				(input[0] == quote) && (input[input.Length - 1] == quote))
				return input.Substring(1, input.Length - 2);

			return input;
		}

	}
}
