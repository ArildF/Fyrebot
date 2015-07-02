using System;
using System.Collections.Generic;
using System.Linq;

namespace Rogue.Fyrebot
{
	public class CommandMap
	{
		private readonly IDictionary<string, MappingEntry> _entries = new Dictionary<string, MappingEntry>();

		public string[] HandlesCommands{get{ return _entries.Keys.ToArray();}}

		public IHandleOptions<TOptions> MapCommand<TOptions>(string command)
		{
			return new HandleOptions<TOptions>(this, command);
		}

		public bool CanHandle(string command)
		{
			return _entries.ContainsKey(command);
		}

		public void Handle(int roomId, string commandLine)
		{
			var split = Utilities.SplitCommandLine(commandLine).ToArray();
			string command = split.First();

			var entry = _entries[command];
			dynamic options = Activator.CreateInstance(entry.OptionsType);
			var result = CommandLine.Parser.Default.ParseArguments(split.Skip(1).ToArray(), options);

			if (result)
			{
				dynamic handler = entry.Handler;
				handler.Execute(roomId, options);
			}
		}

		public interface IHandleOptions<out TOptions>
		{
			void To<THandler>(THandler command) where THandler : IFyreCommand<TOptions>;
		}

		private class HandleOptions<TOptions> : IHandleOptions<TOptions>
		{
			private readonly CommandMap _commandMap;
			private readonly string _command;

			public HandleOptions(CommandMap commandMap, string command)
			{
				_commandMap = commandMap;
				_command = command;
			}

			public void To<THandler>(THandler command) where THandler : IFyreCommand<TOptions>
			{
				_commandMap._entries.Add(_command, new MappingEntry(typeof(TOptions), command));
			}
		}

		private class MappingEntry
		{
			public Type OptionsType { get; private set; }
			public object Handler { get; private set; }

			public MappingEntry(Type optionsType, object handler)
			{
				OptionsType = optionsType;
				Handler = handler;
			}
		}
	}


	public interface IFyreCommand<in TArguments>
	{
		void Execute(int roomId, TArguments arguments);
	}
}
