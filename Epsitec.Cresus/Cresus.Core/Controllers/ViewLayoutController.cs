//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;

using Epsitec.Cresus.Core.Widgets;

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Support;

namespace Epsitec.Cresus.Core.Controllers
{
	public class ViewLayoutController : CoreController
	{
		public ViewLayoutController(string name, Widget container)
			: base (name)
		{
			this.container = container;
			this.columns = new Stack<TileContainer> ();
		}

		public override IEnumerable<CoreController> GetSubControllers()
		{
			yield break;
		}

		public int ColumnCount
		{
			get
			{
				return this.columns.Count;
			}
		}

		public TileContainer LastColumn
		{
			get
			{
				return this.columns.Peek ();
			}
		}

		public TileContainer CreateColumn()
		{
			var column = new TileContainer ()
			{
				Name = string.Format ("Column{0}", this.ColumnCount),
			};

			this.columns.Push (column);
			this.UpdateColumnLayout ();

			return column;
		}

		public TileContainer DeleteColumn()
		{
			if (this.ColumnCount > 0)
			{
				var column = this.columns.Pop ();

				column.Dispose ();
				this.UpdateColumnLayout ();

				return column;
			}
			else
			{
				return null;
			}
		}

		public double TotalWidth
		{
			get
			{
				return this.totalWidth;
			}
		}

		private void UpdateColumnLayout()
		{
			List<Visual> visuals = new List<Visual> ();

			if (this.columns.Count > 0)
			{
				double x = 0;
				double overlap = Widgets.TileArrow.Breadth - 3;
				int columnIndex = 0;

				//	Positionne les colonnes de gauche à droite.
				foreach (var column in this.columns.Reverse ())
				{
					column.Anchor = AnchorStyles.TopAndBottom | AnchorStyles.Left;
					column.PreferredWidth = ViewLayoutController.GetPreferredColumnWidth (this.columns.Count, columnIndex++);
					column.Margins = new Margins (x, 0, 0, 0);
					x += column.PreferredWidth - overlap;
				}

				//	Ajoute les colonnes au parent, mais de droite à gauche.
				foreach (var column in this.columns)
				{
					visuals.Add (column);
				}

				this.totalWidth = x + overlap;
			}

			this.container.Children.Change (collection => visuals);
			this.OnLayoutChanged ();
		}

		private static double GetPreferredColumnWidth(int columnCount, int columnIndex)
		{
#if false
			double width = 250 - 250*(columnCount-columnIndex-2)*0.5;

			//	A part la première colonne de gauche, les autres colonnes doivent soit avoir une
			//	largeur supérieure ou égale à minimalWidth, soit reduceWidth pour ne montrer que
			//	l'icône, à cause de problèmes de layout !
			//	TODO: Voir ce problème avec Pierre (mail du 30.04.10 12:07)
			if (columnIndex != 0 && width < ViewLayoutController.minimalWidth)
			{
				width = ViewLayoutController.reducedWidth;
			}

			return width;
#else
			double width = 300 - 300*(columnCount-columnIndex-1)*0.2;

			if (width < ViewLayoutController.minimalWidth)
			{
				//?width = ViewLayoutController.reducedWidth;
			}
			
			return width;
#endif
		}


		private void OnLayoutChanged()
		{
			var handler = this.LayoutChanged;

			if (handler != null)
			{
				handler (this);
			}
		}

		public event EventHandler LayoutChanged;

		private static readonly double minimalWidth = 220;
		private static readonly double reducedWidth = Widgets.Tiles.TitleTile.MinimumTileWidth + Widgets.TileArrow.Breadth;


		private readonly Widget container;
		private readonly Stack<TileContainer> columns;
		private double totalWidth;
	}
}
