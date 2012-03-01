//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Drawing;
using Epsitec.Common.Support;
using Epsitec.Common.Types;
using Epsitec.Common.Widgets;
using Epsitec.Common.Widgets.Behaviors;

namespace Epsitec.Common.Widgets
{
	public class SearchPicker : FrameBox
	{
		public SearchPicker()
		{
			this.searchBox = new SearchBox ()
			{
				Parent = this,
				Dock = DockStyle.Stacked,
				Margins = new Margins (0, 0, 0, 3),
				TabIndex = 1,
			};

			this.spacer = new StaticText ()
			{
				Parent = this,
				Dock = DockStyle.Stacked,
			};

			this.searchResults = new ScrollList ()
			{
				Parent = this,
				Dock = DockStyle.Stacked,
				Margins = new Margins (0, 0, 3, 0),
				PreferredHeight = 120,
				TabIndex = 2,
			};

			this.spacer.PaintForeground += this.HandleSpacerPaintForeground;
		}


		private void HandleSpacerPaintForeground(object sender, PaintEventArgs e)
		{
			var graphics = e.Graphics;
			var bounds   = this.spacer.Client.Bounds;

			double h = bounds.Height;
			double ox = System.Math.Floor (bounds.Center.X);
			double oy = System.Math.Floor ((bounds.Height - 12) / 2);

			using (Path path = new Path ())
			{
				path.MoveTo (ox + 0.5, oy + 0.5);
				path.LineToRelative (20, 5);
				path.LineToRelative (-10, 0);
				path.LineToRelative (0, 5);
				path.LineToRelative (-20, 0);
				path.LineToRelative (0, -5);
				path.LineToRelative (-10, 0);
				path.LineToRelative (20, -5);
				path.Close ();

				graphics.Color = Color.FromRgb (0.8, 0.8, 1);
				graphics.PaintSurface (path);

				graphics.LineWidth = 1.0;
				graphics.Color = Color.FromRgb (0.5, 0.5, 0.8);
				graphics.PaintOutline (path);
			}
		}


		private readonly SearchBox				searchBox;
		private readonly StaticText				spacer;
		private readonly ScrollList				searchResults;
	}
}
