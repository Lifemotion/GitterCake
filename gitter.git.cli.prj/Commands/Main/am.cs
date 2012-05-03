﻿namespace gitter.Git.AccessLayer.CLI
{
	using System;
	using System.Collections.Generic;

	/// <summary>Apply a series of patches from a mailbox.</summary>
	public sealed class AmCommand : Command
	{
		/// <summary>Add a Signed-off-by: line to the commit message, using the committer identity of yourself.</summary>
		public static CommandArgument SignOff()
		{
			return CommandArgument.SignOff();
		}

		/// <summary>Pass -k flag to git-mailinfo (see git-mailinfo(1)).</summary>
		public static CommandArgument Keep()
		{
			return new CommandArgument("--keep");
		}

		/// <summary>Be quiet. Only print error messages.</summary>
		public static CommandArgument Quiet()
		{
			return CommandArgument.Quiet();
		}

		/// <summary>
		///	Pass -u flag to git-mailinfo (see git-mailinfo(1)). The proposed commit log message taken from the e-mail
		///	is re-coded into UTF-8 encoding (configuration variable i18n.commitencoding can be used to specify project's
		///	preferred encoding if it is not UTF-8). 
		/// </summary>
		public static CommandArgument Utf8()
		{
			return new CommandArgument("--utf8");
		}

		/// <summary>
		///	Pass -n flag to git-mailinfo (see git-mailinfo(1)).
		/// </summary>
		public static CommandArgument NoUtf8()
		{
			return new CommandArgument("--no-utf8");
		}

		/// <summary>
		///	When the patch does not apply cleanly, fall back on 3-way merge if the patch records the identity of blobs it
		///	is supposed to apply to and we have those blobs available locally.
		/// </summary>
		public static CommandArgument FallbackOn3WayMerge()
		{
			return new CommandArgument("--3way");
		}

		/// <summary>Run interactively.</summary>
		public static CommandArgument Interactive()
		{
			return new CommandArgument("--interactive");
		}

		public AmCommand()
			: base("am")
		{
		}

		public AmCommand(params CommandArgument[] args)
			: base("am", args)
		{
		}

		public AmCommand(IList<CommandArgument> args)
			: base("am", args)
		{
		}
	}
}