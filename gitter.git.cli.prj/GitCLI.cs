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
	using System.IO;
	using System.Collections.Generic;

	using gitter.Framework.Services;
	using gitter.Framework.Configuration;

	/// <summary>Performs repository-independent git operations.</summary>
	internal sealed partial class GitCLI : IGitAccessor
	{
		private static readonly Version _minVersion = new Version(1, 7, 0, 2);

		#region Data

		private readonly IGitAccessorProvider _provider;
		private readonly ICommandExecutor _executor;
		private Version _gitVersion;
		private bool _autodetectGitExePath;
		private string _gitExePath;

		#endregion

		/// <summary>Initializes a new instance of the <see cref="GitCLI"/> class.</summary>
		/// <param name="provider">Provider of this accessor.</param>
		public GitCLI(IGitAccessorProvider provider)
		{
			Verify.Argument.IsNotNull(provider, "provider");

			_provider = provider;
			_executor = new GitCommandExecutor(this);
			_autodetectGitExePath = true;
			_gitExePath = string.Empty;

			GitProcess.UpdateGitExePath(this);
		}

		/// <summary>Returns provider of this accessor.</summary>
		/// <value>Provider of this accessor</value>
		public IGitAccessorProvider Provider
		{
			get { return _provider; }
		}

		public bool LogCLICalls
		{
			get;
			set;
		}

		public Version MinimumRequiredGitVersion
		{
			get { return _minVersion; }
		}

		public bool EnableAnsiCodepageFallback
		{
			get { return GitProcess.EnableAnsiCodepageFallback; }
			set { GitProcess.EnableAnsiCodepageFallback = value; }
		}

		public bool AutodetectGitExePath
		{
			get { return _autodetectGitExePath; }
			set
			{
				if(_autodetectGitExePath != value)
				{
					_autodetectGitExePath = value;
					GitProcess.UpdateGitExePath(this);
				}
			}
		}

		public string ManualGitExePath
		{
			get { return _gitExePath; }
			set
			{
				if(_gitExePath != value)
				{
					_gitExePath = value;
					if(!AutodetectGitExePath)
					{
						GitProcess.UpdateGitExePath(this);
					}
				}
			}
		}

		/// <summary>Returns git version.</summary>
		/// <value>git version.</value>
		public Version GitVersion
		{
			get
			{
				if(_gitVersion == null)
				{
					_gitVersion = QueryVersion();
				}
				return _gitVersion;
			}
		}

		/// <summary>Forces re-check of git version.</summary>
		public void RefreshGitVersion()
		{
			_gitVersion = null;
		}

		/// <summary>Returns git version.</summary>
		/// <returns>git version.</returns>
		private Version QueryVersion()
		{
			var gitOutput = _executor.ExecCommand(new Command("--version"));
			gitOutput.ThrowOnBadReturnCode();
			var parser = new GitParser(gitOutput.Output);
			return parser.ReadVersion();
		}

		public IRepositoryAccessor CreateRepositoryAccessor(IGitRepository repository)
		{
			Verify.Argument.IsNotNull(repository, "repository");

			return new RepositoryCLI(this, repository);
		}

		public bool IsValidRepository(string path)
		{
			var gitPath = Path.Combine(path, GitConstants.GitDir);
			if(Directory.Exists(gitPath) || File.Exists(gitPath))
			{
				var executor = new RepositoryCommandExecutor(this, path);
				var gitOutput = executor.ExecCommand(new RevParseCommand(RevParseCommand.GitDir()));
				return gitOutput.ExitCode == 0;
			}
			return false;
		}

		/// <summary>Create an empty git repository or reinitialize an existing one.</summary>
		/// <param name="parameters"><see cref="InitRepositoryParameters"/>.</param>
		/// <exception cref="ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		public void InitRepository(InitRepositoryParameters parameters)
		{
			Verify.Argument.IsNotNull(parameters, "parameters");

			var args = new List<CommandArgument>(3);
			if(parameters.Bare)
			{
				args.Add(InitCommand.Bare());
			}
			if(!string.IsNullOrEmpty(parameters.Template))
			{
				args.Add(InitCommand.Template(parameters.Template));
			}
			var cmd = new InitCommand(args);
			var executor = new RepositoryCommandExecutor(this, parameters.Path);
			var output = executor.ExecCommand(cmd);
			output.ThrowOnBadReturnCode();
		}

		/// <summary>Clone existing repository.</summary>
		/// <param name="parameters"><see cref="CloneRepositoryParameters"/>.</param>
		/// <exception cref="ArgumentNullException"><paramref name="parameters"/> == <c>null</c>.</exception>
		public void CloneRepository(CloneRepositoryParameters parameters)
		{
			/*
			 * git clone [--template=<template_directory>] [-l] [-s] [--no-hardlinks]
			 * [-q] [-n] [--bare] [--mirror] [-o <name>] [-b <name>] [-u <upload-pack>]
			 * [--reference <repository>] [--depth <depth>] [--recursive] [--] <repository> [<directory>]
			*/
			Verify.Argument.IsNotNull(parameters, "parameters");

			var args = new List<CommandArgument>();

			if(!string.IsNullOrEmpty(parameters.Template))
			{
				args.Add(CloneCommand.Template(parameters.Template));
			}
			if(parameters.NoCheckout)
			{
				args.Add(CloneCommand.NoCheckout());
			}
			if(parameters.Bare)
			{
				args.Add(CloneCommand.Bare());
			}
			if(parameters.Mirror)
			{
				args.Add(CloneCommand.Mirror());
			}
			if(!string.IsNullOrEmpty(parameters.RemoteName))
			{
				args.Add(CloneCommand.Origin(parameters.RemoteName));
			}
			if(parameters.Shallow)
			{
				args.Add(CloneCommand.Depth(parameters.Depth));
			}
			if(parameters.Recursive)
			{
				args.Add(CloneCommand.Recursive());
			}
			if(parameters.Monitor != null && GitFeatures.ProgressFlag.IsAvailableFor(this))
			{
				args.Add(CloneCommand.Progress());
			}

			args.Add(CloneCommand.NoMoreOptions());
			args.Add(new PathCommandArgument(parameters.Url));
			args.Add(new PathCommandArgument(parameters.Path));

			var cmd = new CloneCommand(args);

			if(!Directory.Exists(parameters.Path))
			{
				Directory.CreateDirectory(parameters.Path);
			}
			if(parameters.Monitor == null || !GitFeatures.ProgressFlag.IsAvailableFor(this))
			{
				var output = _executor.ExecCommand(cmd);
				output.ThrowOnBadReturnCode();
			}
			else
			{
				using(var async = _executor.ExecAsync(cmd))
				{
					var mon = parameters.Monitor;
					mon.Canceled += (sender, e) => async.Kill();
					async.ErrorReceived += (sender, e) =>
					{
						if(e.Data != null && e.Data.Length != 0)
						{
							var parser = new GitParser(e.Data);
							var progress = parser.ParseProgress();
							progress.Notify(mon);
						}
						else
						{
							mon.SetProgressIndeterminate();
						}
					};
					async.Start();
					async.WaitForExit();
					if(async.ExitCode != 0)
					{
						throw new GitException(async.StdErr);
					}
				}
			}
		}

		/// <summary>Save parameters to the specified <paramref name="section"/>.</summary>
		/// <param name="section">Section to store parameters.</param>
		public void SaveTo(Section section)
		{
			Verify.Argument.IsNotNull(section, "section");

			section.SetValue("Path", ManualGitExePath);
			section.SetValue("Autodetect", AutodetectGitExePath);
			section.SetValue("LogCLICalls", LogCLICalls);
			section.SetValue("EnableAnsiCodepageFallback", EnableAnsiCodepageFallback);
		}

		/// <summary>Load parameters from the specified <paramref name="section"/>.</summary>
		/// <param name="section">Section to look for parameters.</param>
		public void LoadFrom(Section section)
		{
			Verify.Argument.IsNotNull(section, "section");

			ManualGitExePath			= section.GetValue("Path", string.Empty);
			AutodetectGitExePath		= section.GetValue("Autodetect", true);
			LogCLICalls					= section.GetValue("LogCLICalls", false);
			EnableAnsiCodepageFallback	= section.GetValue("EnableAnsiCodepageFallback", false);

			GitProcess.UpdateGitExePath(this);
		}
	}
}
