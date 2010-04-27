//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using System.Text;
using Epsitec.Common.Support;
using Epsitec.Common.Types;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;

namespace Epsitec.Cresus.Core.Controllers
{
	public class ViewLayoutController : CoreController
	{
		public ViewLayoutController(string name, Widget container)
			: base (name)
		{
			this.container = container;
			this.columns = new Stack<Widget> ();
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

		public Widget CreateColumn()
		{
			var column = new Widgets.TilesContainer ()
			{
				Name = string.Format ("Column{0}", this.ColumnCount),
			};

			this.columns.Push (column);
			this.UpdateColumnLayout ();

			return column;
		}

		public void DeleteColumn()
		{
			if (this.ColumnCount > 0)
			{
				var column = this.columns.Pop ();

				column.Dispose ();
				this.UpdateColumnLayout ();
			}
		}

		private void UpdateColumnLayout()
		{
			this.container.Children.Clear ();

			if (this.columns.Count > 0)
			{
				double x = 0;
				double overlap = Widgets.AbstractTile.ArrowBreadth - 3;

				foreach (var column in this.columns.Skip (1).Reverse ())
				{
					column.Anchor = AnchorStyles.TopAndBottom | AnchorStyles.Left;
					column.PreferredWidth = 200;
					//?column.PreferredWidth = Widgets.AbstractTile.WidthWithOnlyIcon + Widgets.AbstractTile.ArrowBreadth;
					column.Margins = new Margins (x, 0, 0, 0);
					x += column.PreferredWidth - overlap;
					ViewLayoutController.SetRightColumn (column, false);
				}

				var lastColumn = this.columns.Peek ();

				lastColumn.Anchor = AnchorStyles.TopAndBottom | AnchorStyles.LeftAndRight;
				lastColumn.Margins = new Margins (x, 0, 0, 0);
				ViewLayoutController.SetRightColumn (lastColumn, true);

				foreach (var column in this.columns)
				{
					this.container.Children.Add (column);
				}
			}
		}

		private static void SetRightColumn(Widget column, bool value)
		{
			if (column is Widgets.TilesContainer)
			{
				var tilesContainer = column as Widgets.TilesContainer;
				tilesContainer.IsRightColumn = value;
			}
		}


		private readonly Widget container;
		private readonly Stack<Widget> columns;
	}
}
