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

namespace gitter.Framework.Options
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Drawing;
	using System.Text;
	using System.Windows.Forms;

	using Resources = gitter.Framework.Properties.Resources;

	[ToolboxItem(false)]
    public partial class EssentialOptionsPage : PropertyPage, IExecutableDialog
	{
        public static readonly new Guid Guid = new Guid("7682FA7B-AC7C-4953-8FBB-2F93149F50CB");

		public EssentialOptionsPage()
			: base(PropertyPageFactory.AppearanceGroupGuid)
		{
			InitializeComponent();

            if (GitterApplication.ComplexityManager.Mode == Complexty.simple){_levelSimple.Checked = true;}
            if (GitterApplication.ComplexityManager.Mode == Complexty.standard){_levelStandard.Checked = true;}
            if (GitterApplication.ComplexityManager.Mode == Complexty.advanced){_levelAdvanced.Checked = true;}

            if (GitterApplication.Language == "ru") { _langRu.Checked = true; }
            if (GitterApplication.Language == "en") { _langEn.Checked = true; }
            if (GitterApplication.Language == "auto") { _langAuto.Checked = true; }

            _langAuto.CheckedChanged += Controls_CheckedChanged;
            _langEn.CheckedChanged += Controls_CheckedChanged;
            _langRu.CheckedChanged += Controls_CheckedChanged;
            _levelSimple.CheckedChanged += Controls_CheckedChanged;
            _levelStandard.CheckedChanged += Controls_CheckedChanged;
            _levelAdvanced.CheckedChanged += Controls_CheckedChanged;
    	}

        private void Controls_CheckedChanged(object sender, EventArgs e)
        {
            Execute();
        }

		public bool Execute()
		{

            if (_levelSimple.Checked) { GitterApplication.ComplexityManager.Mode = Complexty.simple; }
            if (_levelStandard.Checked) { GitterApplication.ComplexityManager.Mode = Complexty.standard; }
            if (_levelAdvanced.Checked) { GitterApplication.ComplexityManager.Mode = Complexty.advanced; }

            if (_langRu.Checked) { GitterApplication.Language = "ru"; }
            if (_langEn.Checked) { GitterApplication.Language = "en"; }
            if (_langAuto.Checked) { GitterApplication.Language = "auto"; }

            _pnlRestartRequiredWarning.Visible = true;

			return true;
		}


	}
}
