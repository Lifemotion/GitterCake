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
	using System.Drawing;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Windows.Forms;

	using gitter.Framework;
	using gitter.Framework.Controls;

	using Resources = gitter.Git.Gui.Properties.Resources;

	[ToolboxItem(false)]
	public partial class SelectResetModeDialog : GitDialogBase
	{
		#region Static

		private static readonly ResetMode[] ResetModes =
			new ResetMode[]
			{
				ResetMode.Soft,
				ResetMode.Mixed,
				ResetMode.Hard,
				ResetMode.Merge,
				ResetMode.Keep,
			};

		#endregion

		#region Data

		private readonly ResetMode _availableModes;
		private ResetMode _resetMode;
		private List<CommandLink> _buttons;

		#endregion

		#region .ctor

		public SelectResetModeDialog(ResetMode availableModes)
		{
			InitializeComponent();

			Text = Resources.StrReset;

			_availableModes = availableModes;
			_resetMode = ResetMode.Mixed;

			_buttons = new List<CommandLink>(ResetModes.Length);
			foreach(var resetMode in ResetModes)
			{
				if((_availableModes & resetMode) == resetMode)
				{
					_buttons.Add(CreateResetButton(resetMode));
				}
			}

			const int margin = 16;

			SuspendLayout();
			var location = new Point(margin, margin);
			var h = margin;
			foreach(var button in _buttons)
			{
				button.Location = location;
				button.Parent = this;
				h += button.Height + margin;
				location.Y += button.Height + margin;
			}

			Height = h;
			AutoScaleDimensions = new SizeF(96F, 96F);
			ResumeLayout(false);
			PerformLayout();
		}

		public SelectResetModeDialog()
			: this(ResetMode.Soft | ResetMode.Mixed | ResetMode.Hard)
		{
		}

		#endregion

		#region Properties

		public ResetMode AvailableModes
		{
			get { return _availableModes; }
		}

		public ResetMode ResetMode
		{
			get { return _resetMode; }
			set { _resetMode = value; }
		}

		protected override string ActionVerb
		{
			get { return Resources.StrReset; }
		}

		public override DialogButtons OptimalButtons
		{
			get { return DialogButtons.Cancel; }
		}

		#endregion

		#region Methods

		private CommandLink CreateResetButton(ResetMode mode)
		{
			string text = string.Empty;
			string desc = string.Empty;

			switch(mode)
			{
				case ResetMode.Soft:
					text = Resources.StrSoft;
					desc = Resources.TipSoftReset;
					break;
				case ResetMode.Mixed:
					text = Resources.StrMixed;
					desc = Resources.TipMixedReset;
					break;
				case ResetMode.Hard:
					text = Resources.StrHard;
					desc = Resources.TipHardReset;
					break;
				case ResetMode.Merge:
					text = Resources.StrMerge;
					desc = Resources.TipMergeReset;
					break;
				case ResetMode.Keep:
					text = Resources.StrKeep;
					desc = Resources.TipKeepReset;
					break;
				default:
					throw new NotSupportedException();
			}
            var dh = new DpiHelper(this);

			var btn = new CommandLink()
			{
				Size		= new Size(dh.ScaleIntX( 319), dh.ScaleIntY( 66)),
				Text		= text,
				Description	= desc,
				Tag			= mode,
			};

			btn.Click += (s, e) =>
				{
					ResetMode = (ResetMode)((Control)s).Tag;
					ClickOk();
				};

			return btn;
		}

		protected override void OnShown()
		{
			base.OnShown();

			if(_buttons != null && _buttons.Count != 0)
			{
				foreach(var btn in _buttons)
				{
					if(((ResetMode)btn.Tag) == _resetMode)
					{
						btn.Focus();
						return;
					}
				}
				_buttons[0].Focus();
			}
		}

		#endregion
	}
}
