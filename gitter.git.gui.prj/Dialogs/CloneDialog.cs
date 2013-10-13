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

namespace gitter.Git.Gui.Dialogs
{
	using System;
	using System.IO;
	using System.ComponentModel;
	using System.Windows.Forms;

	using gitter.Framework;
	using gitter.Framework.Services;

	using Resources = gitter.Git.Gui.Properties.Resources;

	[ToolboxItem(false)]
	partial class CloneDialog : GitDialogBase, IExecutableDialog
	{
		private readonly IGitRepositoryProvider _gitRepositoryProvider;
		private string _acceptedPath;

		public CloneDialog(IGitRepositoryProvider gitRepositoryProvider)
		{
			Verify.Argument.IsNotNull(gitRepositoryProvider, "gitRepositoryProvider");

			InitializeComponent();

			_gitRepositoryProvider = gitRepositoryProvider;

			Text = Resources.StrCloneRepository;

			_lblPath.Text = Resources.StrPath.AddColon();
			_lblUrl.Text = Resources.StrUrl.AddColon();

			_grpOptions.Text = Resources.StrOptions;

			_chkAppendRepositoryNameFromUrl.Text = Resources.StrsAppendRepositoryNameFromURL;
			_lblWillBeClonedInto.Text = Resources.StrsWillBeClonedInto.AddColon();
			_chkUseTemplate.Text = Resources.StrTemplate.AddColon();
			_lblRemoteName.Text = Resources.StrsRemoteName.AddColon();
			_txtRemoteName.Text = GitConstants.DefaultRemoteName;
			_chkBare.Text = Resources.StrBare;
			_chkMirror.Text = Resources.StrMirror;
			_chkShallowClone.Text = Resources.StrsShallowClone;
			_chkRecursive.Text = Resources.StrRecursive;
			_lblDepth.Text = Resources.StrDepth.AddColon();
			_chkNoCheckout.Text = Resources.StrsNoCheckout;

            if (!GitterApplication.ComplexityManager.CurrentModeBiggerThan(Complexty.advanced))
            {
                _chkBare.Visible = false;
                _chkMirror.Visible = false;
                _chkShallowClone.Visible = false;
                _lblDepth.Visible = false;
                _numDepth.Visible = false;
            }

            if (!GitterApplication.ComplexityManager.CurrentModeBiggerThan(Complexty.standard))
            {
                _chkUseTemplate.Visible = false;
                _chkRecursive.Visible = false;
                _chkNoCheckout.Visible = false;
                _txtTemplate.Visible = false;
                _btnSelectTemplate.Visible = false;
            }

			UpdateTargetPathText();

			GitterApplication.FontManager.InputFont.Apply(_txtUrl, _txtPath, _txtRemoteName);
		}

		protected override string ActionVerb
		{
			get { return Resources.StrClone; }
		}

		private IGitRepositoryProvider GitRepositoryProvider
		{
			get { return _gitRepositoryProvider; }
		}

		public string RepositoryPath
		{
			get { return _txtPath.Text; }
			set { _txtPath.Text = value; }
		}

		public bool AppendRepositoryPathFromUrl
		{
			get { return _chkAppendRepositoryNameFromUrl.Checked; }
			set { _chkAppendRepositoryNameFromUrl.Checked = value; }
		}

		public string Url
		{
			get { return _txtUrl.Text; }
			set { _txtUrl.Text = value; }
		}

		public bool Bare
		{
			get { return _chkBare.Checked; }
			set { _chkBare.Checked = value; }
		}

		public bool Mirror
		{
			get { return _chkMirror.Checked; }
			set { _chkMirror.Checked = value; }
		}

		public bool UseTemplate
		{
			get { return _chkUseTemplate.Checked; }
			set { _chkUseTemplate.Checked = value; }
		}

		public string RemoteName
		{
			get { return _txtRemoteName.Text; }
			set { _txtRemoteName.Text = value; }
		}

		public string Template
		{
			get { return _txtTemplate.Text; }
			set { _txtTemplate.Text = value; }
		}

		public bool ShallowClone
		{
			get { return _chkShallowClone.Checked; }
			set { _chkShallowClone.Checked = value; }
		}

		public int Depth
		{
			get { return (int)_numDepth.Value; }
			set { _numDepth.Value = value; }
		}

		public bool Recursive
		{
			get { return _chkRecursive.Checked; }
			set { _chkRecursive.Checked = value; }
		}

		public bool NoCheckout
		{
			get { return _chkNoCheckout.Checked; }
			set { _chkNoCheckout.Checked = value; }
		}

		private static string AppendUrlToPath(string path, string url)
		{
			if(url.Length != 0)
			{
				try
				{
					path = Path.Combine(path, GitUtils.GetHumanishName(url));
				}
				catch { }
			}
			return path;
		}

		public string TargetPath
		{
			get
			{
				if(_chkAppendRepositoryNameFromUrl.Checked)
				{
					string path = _txtPath.Text.Trim();
					string url = _txtUrl.Text.Trim();
					return AppendUrlToPath(path, url);
				}
				return _txtPath.Text.Trim();
			}
		}

		public string AcceptedPath
		{
			get { return _acceptedPath; }
		}

		#region Event Handlers

		private void _btnSelectDirectory_Click(object sender, EventArgs e)
		{
			var path = Utility.ShowPickFolderDialog(this);
			if(path != null)
			{
				_txtPath.Text = path;
			}
		}

		private void _btnSelectTemplate_Click(object sender, EventArgs e)
		{
			var path = Utility.ShowPickFolderDialog(this);
			if(path != null)
			{
				_txtTemplate.Text = path;
			}
		}

		private void UpdateTargetPathText()
		{
			var path = TargetPath;
			if(path.Length == 0)
			{
				_lblRealPath.Text = Resources.StrlNoPathSpecified.SurroundWith("<", ">");
			}
			else
			{
				_lblRealPath.Text = path;
			}
		}

		private void _txtUrl_TextChanged(object sender, EventArgs e)
		{
			if(_chkAppendRepositoryNameFromUrl.Checked)
				UpdateTargetPathText();
		}

		private void _txtPath_TextChanged(object sender, EventArgs e)
		{
			UpdateTargetPathText();
		}

		private void _chkAppendRepositoryNameFromUrl_CheckedChanged(object sender, EventArgs e)
		{
			UpdateTargetPathText();
		}

		private void _chkUseTemplate_CheckedChanged(object sender, EventArgs e)
		{
			bool enabled = _chkUseTemplate.Checked;
			_txtTemplate.Enabled = enabled;
			_btnSelectTemplate.Enabled = enabled;
		}

		private void _chkShallowClone_CheckedChanged(object sender, EventArgs e)
		{
			bool enabled = _chkShallowClone.Checked;
			_lblDepth.Enabled = enabled;
			_numDepth.Enabled = enabled;
		}

		private void _chkBare_CheckedChanged(object sender, EventArgs e)
		{
			_chkMirror.Enabled = _chkBare.Checked;
		}

		#endregion

		public bool Execute()
		{
			var url = _txtUrl.Text.Trim();
			var path = _txtPath.Text.Trim();
			if(!ValidateUrl(url, _txtUrl))
			{
				return false;
			}
			if(!ValidateAbsolutePath(path, _txtPath))
			{
				return false;
			}
			if(_chkAppendRepositoryNameFromUrl.Checked)
			{
				path = AppendUrlToPath(path, url);
			}
			var remoteName = _txtRemoteName.Text.Trim();
			if(!ValidateRemoteName(remoteName, _txtRemoteName))
			{
				return false;
			}
			bool shallow = _chkShallowClone.Checked;
			int depth = shallow ? (int)_numDepth.Value : -1;
			string template = _chkUseTemplate.Checked ? _txtTemplate.Text.Trim() : null;
			if(!string.IsNullOrWhiteSpace(template) && !ValidateAbsolutePath(template, _txtTemplate))
			{
				return false;
			}
			bool bare = _chkBare.Checked;
			bool mirror = bare && _chkMirror.Checked;
			bool noCheckout = _chkNoCheckout.Checked;
			bool recursive = _chkRecursive.Checked;
			try
			{
				Repository.CloneAsync(
					GitRepositoryProvider.GitAccessor,
					url, path, template, remoteName,
					shallow, depth, bare, mirror, recursive, noCheckout).Invoke<ProgressForm>(this);
			}
			catch(GitException exc)
			{
				GitterApplication.MessageBoxService.Show(
					this,
					exc.Message,
					Resources.ErrFailedToClone.UseAsFormat(url),
					MessageBoxButton.Close,
					MessageBoxIcon.Error);
				return false;
			}
			_acceptedPath = path;
			return true;
		}
	}
}
