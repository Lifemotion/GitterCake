#region Copyright Notice
/*
 * gitter - VCS repository management tool
 * Copyright (C) 2013  Popovskiy Maxim Vladimirovitch <amgine.gitter@gmail.com>
 * 
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */
#endregion

namespace gitter.Git.AccessLayer.CLI
{
	using System;
	using System.Collections.Generic;

	/// <summary>Join two or more development histories together.</summary>
	public sealed class MergeCommand : Command
	{
		public static CommandArgument Stat()
		{
			return new CommandArgument("--stat");
		}

		public static CommandArgument NoStat()
		{
			return new CommandArgument("--no-stat");
		}

		public static CommandArgument Log()
		{
			return new CommandArgument("--log");
		}

		public static CommandArgument NoLog()
		{
			return new CommandArgument("--no-log");
		}

		public static CommandArgument Commit()
		{
			return new CommandArgument("--commit");
		}

		public static CommandArgument NoCommit()
		{
			return new CommandArgument("--no-commit");
		}

		public static CommandArgument Squash()
		{
			return new CommandArgument("--squash");
		}

		public static CommandArgument NoSquash()
		{
			return new CommandArgument("--no-squash");
		}

		public static CommandArgument FastForward()
		{
			return new CommandArgument("--ff");
		}

		public static CommandArgument NoFastForward()
		{
			return new CommandArgument("--no-ff");
		}

		public static CommandArgument Message(string msg)
		{
			return new CommandArgument("-m", "\"" + msg + "\"", ' ');
		}

		public static CommandArgument Strategy(string strategy)
		{
			return new CommandArgument("--strategy", strategy, '=');
		}

		public static CommandArgument Strategy(MergeStrategy strategy)
		{
			switch(strategy)
			{
				case MergeStrategy.Octopus:
					return new CommandArgument("--strategy", "octopus", '=');
				case MergeStrategy.Ours:
					return new CommandArgument("--strategy", "ours", '=');
				case MergeStrategy.Recursive:
					return new CommandArgument("--strategy", "recursive", '=');
				case MergeStrategy.Resolve:
					return new CommandArgument("--strategy", "resolve", '=');
				case MergeStrategy.Subtree:
					return new CommandArgument("--strategy", "subtree", '=');
				default:
					return null;
			}
		}

		public static CommandArgument StrategyOption(string option)
		{
			return new CommandArgument("--strategy-option", option, '=');
		}

		public static CommandArgument Quiet()
		{
			return new CommandArgument("--quiet");
		}

		public static CommandArgument Verbose()
		{
			return new CommandArgument("--verbose");
		}

		public MergeCommand()
			: base("merge")
		{
		}

		public MergeCommand(params CommandArgument[] args)
			: base("merge", args)
		{
		}

		public MergeCommand(IList<CommandArgument> args)
			: base("merge", args)
		{
		}
	}
}
