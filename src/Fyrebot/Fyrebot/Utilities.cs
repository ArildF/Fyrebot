using System.Collections.Generic;
using System.Linq;

namespace Rogue.Fyrebot
{
	public static class Utilities
	{
		// based on code from http://stackoverflow.com/a/298990
		public static IEnumerable<string> SplitCommandLine(string commandLine)
		{
			bool inQuotes = false;

			return commandLine.Split(c =>
			{
				if (c == '\"')
					inQuotes = !inQuotes;

				return !inQuotes && c == ' ';
			}).Select(arg => arg.Trim().TrimMatchingQuotes('\"'))
							  .Where(arg => !string.IsNullOrEmpty(arg));
		}
	}
}
