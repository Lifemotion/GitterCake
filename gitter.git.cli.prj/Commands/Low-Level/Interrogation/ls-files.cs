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
	using System.Collections.Generic;

	/// <summary>Show information about files in the index and the working tree.</summary>
	public sealed class LsFilesCommand : Command
	{
		/// <summary>Show cached files in the output (default).</summary>
		public static CommandArgument Cached()
		{
			return new CommandArgument("--cached");
		}

		/// <summary>Show deleted files in the output.</summary>
		public static CommandArgument Deleted()
		{
			return new CommandArgument("--deleted");
		}

		/// <summary>Show modified files in the output.</summary>
		public static CommandArgument Modified()
		{
			return new CommandArgument("--modified");
		}

		/// <summary>Show other files in the output.</summary>
		public static CommandArgument Others()
		{
			return new CommandArgument("--others");
		}

		/// <summary>Show ignored files in the output. Note that this also reverses any exclude list present.</summary>
		public static CommandArgument Ignored()
		{
			return new CommandArgument("--ignored");
		}

		/// <summary>Show staged contents' object name, mode bits and stage number in the output.</summary>
		public static CommandArgument Stage()
		{
			return new CommandArgument("--stage");
		}

		/// <summary>If a whole directory is classified as "other", show just its name (with a trailing slash) and not its whole contents.</summary>
		public static CommandArgument Directory()
		{
			return new CommandArgument("--directory");
		}

		/// <summary>Do not list empty directories. Has no effect without --directory.</summary>
		public static CommandArgument NoEmptyDirectory()
		{
			return new CommandArgument("--no-empty-directory");
		}

		/// <summary>Show unmerged files in the output (forces --stage).</summary>
		public static CommandArgument Unmerged()
		{
			return new CommandArgument("--unmerged");
		}

		/// <summary>Show files on the filesystem that need to be removed due to file/directory conflicts for checkout-index to succeed.</summary>
		public static CommandArgument Killed()
		{
			return new CommandArgument("--killed");
		}

		/// <summary>\0 line termination on output.</summary>
		public static CommandArgument ZeroLineTermination()
		{
			return new CommandArgument("-z");
		}

		/// <summary>Skips files matching pattern. Note that pattern is a shell wildcard pattern.</summary>
		public static CommandArgument Exclude(string pattern)
		{
			return new CommandArgument("--exclude", "\"" + pattern + "\"");
		}

		/// <summary>Exclude patterns are read from <paramref name="file"/>; 1 per line.</summary>
		public static CommandArgument ExcludeFrom(string file)
		{
			return new CommandArgument("--exclude-from", "\"" + file + "\"");
		}

		/// <summary>Add the standard git exclusions: .git/info/exclude, .gitignore in each directory, and the user's global exclusion file.</summary>
		public static CommandArgument ExcludeStandard()
		{
			return new CommandArgument("--exclude-standard");
		}

		/// <summary>If any file does not appear in the index, treat this as an error (return 1).</summary>
		public static CommandArgument ErrorUnmatch()
		{
			return new CommandArgument("--error-unmatch");
		}

		/// <summary>
		/// When using --error-unmatch to expand the user supplied file (i.e. path pattern) arguments to paths,
		/// pretend that paths which were removed in the index since the named tree-ish are still present.
		/// Using this option with -s or -u options does not make any sense.
		/// </summary>
		public static CommandArgument WithTree(string tree)
		{
			return new CommandArgument("--with-tree", tree);
		}

		/// <summary>Identify the file status with tags (followed by a space) at the start of each line.</summary>
		public static CommandArgument ShowStatus()
		{
			return new CommandArgument("-t");
		}

		/// <summary>When run from a subdirectory, the command usually outputs paths relative to the current directory. This option forces paths to be output relative to the project top directory.</summary>
		public static CommandArgument FullName()
		{
			return new CommandArgument("--full-name");
		}

		/// <summary>Do not interpret any more arguments as options.</summary>
		public static CommandArgument NoMoreOptions()
		{
			return CommandArgument.NoMoreOptions();
		}

		public LsFilesCommand()
			: base("ls-files")
		{
		}

		public LsFilesCommand(params CommandArgument[] args)
			: base("ls-files", args)
		{
		}

		public LsFilesCommand(IList<CommandArgument> args)
			: base("ls-files", args)
		{
		}
	}
}
