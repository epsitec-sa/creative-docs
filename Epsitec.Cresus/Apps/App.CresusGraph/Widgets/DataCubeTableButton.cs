//	Copyright © 2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Drawing;
using Epsitec.Common.Graph.Widgets;
using Epsitec.Common.UI;
using Epsitec.Common.Support;
using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Types;
using Epsitec.Common.Widgets;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Graph.Widgets
{
	public class DataCubeTableButton : FrameBox
	{
		public DataCubeTableButton()
		{
			this.sources = new List<string> ();

			this.CreateUI ();
		}

		public GraphDataCube Cube
		{
			get
			{
				return this.cube;
			}
			set
			{
				if (this.cube != value)
				{
					this.cube = value;
					this.AnalyzeCube ();
					this.InvalidateUI ();
				}
			}
		}

		public DataCubeCardinality Cardinality
		{
			get
			{
				switch (this.sources.Count)
				{
					case 0:
						return DataCubeCardinality.Empty;
					case 1:
						return DataCubeCardinality.Table;
					default:
						return DataCubeCardinality.Cube;
				}
			}
		}

		public IList<string> Sources
		{
			get
			{
				return this.sources.AsReadOnly ();
			}
		}

		public static Color HiliteColor
		{
			get
			{
				return Color.FromRgb (1.0, 186.0/255.0, 1.0/255.0);
			}
		}

		protected override void PaintBackgroundImplementation(Graphics graphics, Rectangle clipRect)
		{
			var rect = this.Client.Bounds;

			if (this.IsEnabled)
			{
				if (this.IsSelected)
				{
					graphics.AddFilledRectangle (rect);
					graphics.RenderSolid (Color.FromBrightness (1));

					graphics.AddFilledRectangle (0, 0, 3, rect.Height);
					graphics.RenderSolid (DataCubeTableButton.HiliteColor);
				}
				else
				{
					graphics.AddFilledRectangle (rect);
					graphics.RenderSolid (Color.FromRgb (0.95, 0.95, 0.98));
					
					var adorner = Epsitec.Common.Widgets.Adorners.Factory.Active;
					var border  = adorner.ColorBorder;

					graphics.AddFilledRectangle (rect.Width-1, 0, 1, rect.Height);
					graphics.RenderSolid (border);
				}
			}
			else
			{
				if (this.IsSelected)
				{
					graphics.AddFilledRectangle (rect);
					graphics.RenderSolid (Color.FromBrightness (1));
				}
				else
				{
					graphics.AddFilledRectangle (rect);
					graphics.RenderSolid (Color.FromRgb (0.95, 0.95, 0.95));
					
					var adorner = Epsitec.Common.Widgets.Adorners.Factory.Active;
					var border  = adorner.ColorBorder;

					graphics.AddFilledRectangle (rect.Width-1, 0, 1, rect.Height);
					graphics.RenderSolid (border);
				}
			}
		}
		
		protected override void PaintForegroundImplementation(Graphics graphics, Rectangle clipRect)
		{
			base.PaintForegroundImplementation (graphics, clipRect);

			if (this.IsSelected)
			{
				//	No need for a note with the source name, since it will appear in the
				//	details widget.
			}
			else
			{
				var rect = this.Client.Bounds;
				var label = this.Sources.FirstOrDefault () ?? "";
				var width = 54.0;

				rect = new Rectangle (rect.Right - width, rect.Bottom, width, rect.Height);

				MiniChartView.PaintNote (graphics, rect, label, MiniChartView.StickyNoteYellow);
			}
		}


		private void AnalyzeCube()
		{
			this.sources.Clear ();

			if (this.cube == null)
			{
				return;
			}

			this.sources.AddRange (this.cube.GetDimensionValues ("Source"));
		}

		private void InvalidateUI()
		{
			this.DeleteUI ();
			this.CreateUI ();
		}


		private void CreateUI()
		{
			var image = new StaticImage (this)
			{
				ImageName = "manifest:Epsitec.Cresus.Graph.Images.Cube.Table1.icon",
				Anchor = AnchorStyles.TopLeft,
				Margins = new Margins (4, 4, 0, 0),
				PreferredWidth = DataCubeTableButton.TableIconImageLength,
				PreferredHeight = DataCubeTableButton.TableIconImageLength,
			};

			var text1 = new StaticText (this)
			{
				Text = DataCubeTableButton.GetCubeDescription (this.cube),
				Anchor = AnchorStyles.Top | AnchorStyles.LeftAndRight,
				Margins = new Margins (DataCubeTableButton.TableIconLength, 4, 0, 0),
				ContentAlignment = ContentAlignment.TopLeft,
				PreferredHeight = DataCubeTableButton.TableIconLength,
			};

			var text2 = new StaticText (this)
			{
				Text = string.Concat ("<font size=\"80%\">", FormattedText.Escape (DataCubeTableButton.GetCubeLoadPath (this.cube)) ?? "", "</font>"),
				Anchor = AnchorStyles.Top | AnchorStyles.LeftAndRight,
				Margins = new Margins (4, 4, DataCubeTableButton.TableIconLength, 0),
				ContentAlignment = ContentAlignment.TopLeft,
				PreferredHeight = 16,
			};
		}

		private void DeleteUI()
		{
			this.Children.Widgets.ForEach (widget => widget.Dispose ());
		}

		private static string GetCubeDescription(GraphDataCube cube)
		{
			if (cube == null)
			{
				return "&lt;null&gt;";
			}

			var names1 = cube.NaturalTableDimensionNames;
			var names2 = from name in cube.DimensionNames
						 where !names1.Contains (name)
						 select name;

			var buffer = new System.Text.StringBuilder ();

			buffer.Append (FormattedText.Escape (cube.Title));
			buffer.Append ("<br/>");
			buffer.AppendJoin (", ", names1.Select (x => string.Concat ("<b>", FormattedText.Escape (x), "</b>")));

			if (!buffer.EndsWith ("<br/>"))
			{
				buffer.Append (", ");
			}

			buffer.AppendJoin (", ", names2.Select (x => FormattedText.Escape (x)));

			buffer.Append ("<br/><font size=\"80%\">");
			buffer.Append (FormattedText.Escape (DataCubeTableButton.GetCubeLoadPath (cube)));
			buffer.Append ("</font>");

			return buffer.ToString ();
		}

		private static string GetCubeLoadPath(GraphDataCube cube)
		{
			if (cube == null)
            {
				return null;
            }
			
			var path = cube.LoadPath;

			if (string.IsNullOrEmpty (path))
			{
				return "<inconnu>";
			}
			else if (path == GraphDataCube.LoadPathClipboard)
			{
				return "Presse-papier";
			}
			else
			{
				var p1 = System.IO.Path.GetDirectoryName (path);
				var p2 = System.IO.Path.GetFileName (path);

				int len = System.Math.Max (0, 60 - p2.Length);

				if (p1.Length > len)
				{
					return string.Concat (p1.Substring (0, len), "...", System.IO.Path.DirectorySeparatorChar.ToString (), p2);
				}
				else
				{
					return path;
				}
			}
		}
		
		const double TableIconImageLength = 31;
		const double TableIconLength = DataCubeTableButton.TableIconImageLength + 2*4;

		private GraphDataCube cube;
		private readonly List<string> sources;
	}
}
