﻿namespace gitter.Framework
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Drawing;

	using gitter.Framework.Services;

	public sealed class TextWithHyperlinks
	{
		#region Data

		private readonly string _text;
		private readonly StringFormat _sf;
		private readonly HyperlinkGlyph[] _glyphs;
		private RectangleF _cachedRect;

		#endregion

		#region Events

		public event EventHandler InvalidateRequired;

		#endregion

		private sealed class HyperlinkGlyph
		{
			private readonly Hyperlink _href;
			private Region _region;

			public HyperlinkGlyph(Hyperlink href)
			{
				Verify.Argument.IsNotNull(href, "href");

				_href = href;
			}

			public Hyperlink Hyperlink
			{
				get { return _href; }
			}

			public int Start
			{
				get { return _href.Text.Start; }
			}

			public int End
			{
				get { return _href.Text.End; }
			}

			public int Length
			{
				get { return _href.Text.Length; }
			}

			public Region Region
			{
				get { return _region; }
				set
				{
					if(_region != null) _region.Dispose();
					_region = value;
				}
			}

			public bool IsHovered { get; set; }
		}

		private TrackingService<HyperlinkGlyph> _hoveredLink;

		public TextWithHyperlinks(string text, HyperlinkExtractor extractor = null)
		{
			_text = text;
			_sf = (StringFormat)(StringFormat.GenericTypographic.Clone());
			_sf.FormatFlags = StringFormatFlags.FitBlackBox | StringFormatFlags.MeasureTrailingSpaces;
			if(extractor == null) extractor = new HyperlinkExtractor();
			_glyphs = extractor.ExtractHyperlinks(text)
							   .Select(h => new HyperlinkGlyph(h))
							   .ToArray();
			_sf.SetMeasurableCharacterRanges(
				_glyphs.Select(l => new CharacterRange(l.Start, l.Length)).ToArray());

			_hoveredLink = new TrackingService<HyperlinkGlyph>();
			_hoveredLink.Changed += OnHoveredLinkChanged;
		}

		public Hyperlink HoveredHyperlink
		{
			get
			{
				if(_hoveredLink.IsTracked)
				{
					return _hoveredLink.Item.Hyperlink;
				}
				else
				{
					return null;
				}
			}
		}

		private void OnHoveredLinkChanged(object sender, TrackingEventArgs<TextWithHyperlinks.HyperlinkGlyph> e)
		{
			e.Item.IsHovered = e.IsTracked;
			InvalidateRequired.Raise(this);
		}

		public string Text
		{
			get { return _text; }
		}

		public void Render(Graphics graphics, Font font, Rectangle rect)
		{
			bool useCache = _cachedRect == rect;
			if(useCache)
			{
				for(int i = 0; i < _glyphs.Length; ++i)
				{
					graphics.ExcludeClip(_glyphs[i].Region);
				}
			}
			else
			{
				var cr = graphics.MeasureCharacterRanges(_text, font, rect, _sf);
				for(int i = 0; i < _glyphs.Length; ++i)
				{
					_glyphs[i].Region = cr[i];
					graphics.ExcludeClip(cr[i]);
				}
			}
			GitterApplication.TextRenderer.DrawText(
				graphics, _text, font, Brushes.Black, rect, _sf);
			graphics.ResetClip();
			bool clipIsSet = false;
			foreach(var g in _glyphs)
			{
				if(g != _hoveredLink.Item)
				{
					if(clipIsSet)
					{
						graphics.SetClip(g.Region, System.Drawing.Drawing2D.CombineMode.Union);
					}
					else
					{
						graphics.Clip = g.Region;
						clipIsSet = true;
					}
				}
			}
			if(clipIsSet)
			{
				GitterApplication.TextRenderer.DrawText(
					graphics, _text, font, Brushes.Blue, rect, _sf);
			}
			if(_hoveredLink.IsTracked)
			{
				graphics.Clip = _hoveredLink.Item.Region;
				using(var f = new Font(font, FontStyle.Underline))
				{
					GitterApplication.TextRenderer.DrawText(
						graphics, _text, f, Brushes.Blue, rect, _sf);
				}
			}
			graphics.ResetClip();
			_cachedRect = rect;
		}

		private int HitTest(RectangleF rect, Point p)
		{
			p.X += (int)(_cachedRect.X - rect.X);
			p.Y += (int)(_cachedRect.Y - rect.Y);
			for(int i = 0; i < _glyphs.Length; ++i)
			{
				var glyph = _glyphs[i];
				if(glyph.Region != null && glyph.Region.IsVisible(p))
				{
					return i;
				}
			}
			return -1;
		}

		public void OnMouseMove(RectangleF rect, Point p)
		{
			var index = HitTest(rect, p);
			if(index != -1)
				_hoveredLink.Track(index, _glyphs[index]);
			else
				_hoveredLink.Drop();
		}

		public void OnMouseDown(RectangleF rect, Point p)
		{
			var index = HitTest(rect, p);
			if(index != -1) _glyphs[index].Hyperlink.Navigate();
		}

		public void OnMouseLeave()
		{
			_hoveredLink.Drop();
		}

		public override string ToString()
		{
			return _text;
		}
	}
}
