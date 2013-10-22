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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace gitter
{
	public partial class MainForm : Form
	{
		public MainForm()
		{
			InitializeComponent();

			var args = Environment.GetCommandLineArgs();

			if(args.Length >= 2)
			{
				_lblPrompt.Text = args[1];
			}

			Font = SystemFonts.MessageBoxFont;
            try
            {
                var homedir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                 var password = System.IO.File.ReadAllText(System.IO.Path.Combine(homedir, ".git_" +GetPasswordId(_lblPrompt.Text)), System.Text.Encoding.UTF8);

                _txtPassword.Text = password;
            }
            catch
            { }

            
		}

        private static string GetPasswordId(string prompt)
        {
            long code = 0;
            var bytes = System.Text.Encoding.UTF8.GetBytes(prompt);
            long multiplier = 1;
            foreach (byte c in bytes)
            {
                code += multiplier*(long)c;
                multiplier *= 2;
            }
            return code.ToString();
        }

		private static void SendPassword(string password)
		{
			Console.Write(password);
		}


		private void _btnOk_Click(object sender, EventArgs e)
		{
            if (_cbSave.Checked)
            {
                var homedir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                System.IO.File.WriteAllText(System.IO.Path.Combine(homedir,".git_"+GetPasswordId(_lblPrompt.Text)), _txtPassword.Text, System.Text.Encoding.UTF8);
            }
			SendPassword(_txtPassword.Text);
			Close();
		}

		private void _btnCancel_Click(object sender, EventArgs e)
		{
			Close();
		}

        private void _cbSave_CheckedChanged(object sender, EventArgs e)
        {

        }
	}
}
