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

namespace gitter
{
	using System;
	using System.IO;
	using System.Collections.Generic;
	using System.Drawing;
	using System.Text;
	using System.Windows.Forms;

	using gitter.Framework;
	using gitter.Framework.Controls;
	using gitter.Framework.Services;

	using Resources = gitter.Properties.Resources;

	internal sealed class RepositoryListItem : CustomListBoxItem<RepositoryLink>
	{
		private static readonly Bitmap ImgRepositorySmall = CachedResources.Bitmaps["ImgRepository"];
		private static readonly Bitmap ImgRepositoryLarge = CachedResources.Bitmaps["ImgRepositoryLarge"];

		private static readonly StringFormat PathStringFormat;

		static RepositoryListItem()
		{
			PathStringFormat = new StringFormat(GitterApplication.TextRenderer.LeftAlign);
			PathStringFormat.Trimming = StringTrimming.EllipsisPath;
			PathStringFormat.FormatFlags |= StringFormatFlags.NoClip;
		}

		public RepositoryListItem(RepositoryLink rlink)
			: base(rlink)
		{
			Verify.Argument.IsNotNull(rlink, "rlink");
		}

		private string Name
		{
			get
			{
				if(string.IsNullOrEmpty(DataContext.Description))
				{
					if(DataContext.Path.EndsWithOneOf(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar))
					{
						return Path.GetFileName(DataContext.Path.Substring(0, DataContext.Path.Length - 1));
					}
					else
					{
						return Path.GetFileName(DataContext.Path);
					}
				}
				return DataContext.Description;
			}
		}

		protected override Size OnMeasureSubItem(SubItemMeasureEventArgs measureEventArgs)
		{
			switch(measureEventArgs.SubItemId)
			{
				case 0:
					return measureEventArgs.MeasureImageAndText(ImgRepositorySmall, DataContext.Path);
				case 1:
					return measureEventArgs.MeasureImageAndText(ImgRepositoryLarge, DataContext.Path);
				default:
					return Size.Empty;
			}
		}

        private Color ColorFromRepositoryName(string name)
        {
            int seed = 7;
            int startValue = 80;
            int maxValue = 150;
            int r=startValue, g=startValue, b=startValue;
            int k=100;   
            var bytes = System.Text.Encoding.ASCII.GetBytes(name.ToUpper());
            for (int i = 0; i < bytes.Length; i++)
            {
                var rnd = new Random(bytes[i] + seed);
                r += rnd.Next(-k, k);
                g += rnd.Next(-k, k);
                b += rnd.Next(-k, k);
                k = (int)((double)k * 0.8);
                if (r < 0) { r = 0; } if (r > maxValue) { r = maxValue; }
                if (g < 0) { g = 0; } if (g > maxValue) { g = maxValue; }
                if (b < 0) { b = 0; } if (b > maxValue) { b = maxValue; }
            }

           return Color.FromArgb(r, g, b);
        }

		protected override void OnPaintSubItem(SubItemPaintEventArgs paintEventArgs)
		{
			switch(paintEventArgs.SubItemId)
			{
				case 0:
					paintEventArgs.PaintImageAndText(ImgRepositorySmall, DataContext.Path, paintEventArgs.Brush, PathStringFormat);
					break;
				case 1:
                    Brush brush = paintEventArgs.Brush;
                    brush = new SolidBrush(ColorFromRepositoryName(Name));
					paintEventArgs.PaintImage(ImgRepositoryLarge);
					var cy = paintEventArgs.Bounds.Y + 2;
					GitterApplication.TextRenderer.DrawText(
                        paintEventArgs.Graphics, Name, paintEventArgs.Font, brush, 36, cy);
					cy += 16;
					var rc = new Rectangle(36, cy, paintEventArgs.Bounds.Width - 42, 16);
					if((paintEventArgs.State & ItemState.Selected) == ItemState.Selected && GitterApplication.Style.Type == GitterStyleType.DarkBackground)
					{
						GitterApplication.TextRenderer.DrawText(
                            paintEventArgs.Graphics, DataContext.Path, paintEventArgs.Font, brush, rc, PathStringFormat);
					}
					else
					{
						using(var textBrush = new SolidBrush(GitterApplication.Style.Colors.GrayText))
						{
							GitterApplication.TextRenderer.DrawText(
								paintEventArgs.Graphics, DataContext.Path, paintEventArgs.Font, textBrush, rc, PathStringFormat);
						}
					}
					break;
			}
		}

		public override ContextMenuStrip GetContextMenu(ItemContextMenuRequestEventArgs requestEventArgs)
		{
			var menu = new RepositoryMenu(this);
			Utility.MarkDropDownForAutoDispose(menu);
			return menu;
		}
	}
}
