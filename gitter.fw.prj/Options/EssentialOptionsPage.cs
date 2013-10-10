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

            if (GitterApplication.ComplexityMode.Mode == ComplextyModeVariants.simple)
			{
				_levelSimple.Checked = true;
			}
            else if (GitterApplication.ComplexityMode.Mode == ComplextyModeVariants.standard)
			{
                _levelStandard.Checked = true;
			}
            else if (GitterApplication.ComplexityMode.Mode == ComplextyModeVariants.advanced)
            {
                _levelAdvanced.Checked = true;
            }

    	}

		public bool Execute()
		{
			if(_levelSimple.Checked)
			{
                GitterApplication.ComplexityMode.Mode = ComplextyModeVariants.simple;
			}
			else if(_levelStandard.Checked)
			{
                GitterApplication.ComplexityMode.Mode = ComplextyModeVariants.standard;
			}
            else if (_levelStandard.Checked)
            {
                GitterApplication.ComplexityMode.Mode = ComplextyModeVariants.advanced;
            }
			return true;
		}
	}
}
