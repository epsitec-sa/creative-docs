//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Assets.Server.BusinessLogic
{
	/// <summary>
	/// Une cellule d'une timeline, correspondant à un ou plusieurs événements.
	/// </summary>
	public class TimelineCell
	{
		public TimelineCell(TimelineGlyph glyph, DataCellFlags flags = DataCellFlags.None, string tooltip = null)
		{
			this.Glyphs = new List<TimelineGlyph> ();
			this.Glyphs.Add (glyph);

			this.Flags   = flags;
			this.Tooltip = tooltip;
		}

		public TimelineCell(IEnumerable<TimelineGlyph> glyphs, DataCellFlags flags = DataCellFlags.None, string tooltip = null)
		{
			this.Glyphs = new List<TimelineGlyph> ();
			this.Glyphs.AddRange (glyphs);

			this.Flags   = flags;
			this.Tooltip = tooltip;
		}

		public TimelineCell(TimelineCell merged, TimelineCell added)
		{
			this.Glyphs = new List<TimelineGlyph> ();

			if (!merged.IsEmpty)
			{
				this.Glyphs.AddRange (merged.Glyphs);
			}

			if (!added.IsEmpty)
			{
				this.Glyphs.AddRange (added.Glyphs);
			}

			this.Flags = merged.Flags | added.Flags;

			//	Fusionne les tooltips de façon à ne conserver que la première ligne
			//	de chacun.
			string t1, t2;

			if (merged.Glyphs.Count == 1)
			{
				t1 = TimelineCell.GetFirstLine (merged.Tooltip);
			}
			else
			{
				t1 = merged.Tooltip;
			}

			t2 = TimelineCell.GetFirstLine (added.Tooltip);

			this.Tooltip = string.Concat (t1, "<br/>", t2);
		}


		public bool IsEmpty
		{
			get
			{
				return this.Glyphs.Count == 1 && this.Glyphs[0].IsEmpty;
			}
		}


		public static TimelineCell Empty = new TimelineCell (TimelineGlyph.Empty);

		private static string GetFirstLine(string text)
		{
			if (!string.IsNullOrEmpty (text))
			{
				var i = text.IndexOf ("<br/>");
				if (i != -1)
				{
					text = text.Substring (0, i);
				}
			}

			return text;
		}


		public readonly List<TimelineGlyph>		Glyphs;
		public readonly DataCellFlags			Flags;
		public readonly string					Tooltip;
	}
}