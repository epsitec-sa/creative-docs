//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;
using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Widgets;

using Epsitec.Cresus.Core.Widgets;

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Support;

namespace Epsitec.Cresus.Core.Controllers
{
	public sealed class ViewLayoutController : CoreController, IWidgetUpdater
	{
		public ViewLayoutController(string name, Widget container)
			: base (name)
		{
			this.container = container;
			this.columns = new Stack<TileContainer> ();
		}

		
		public int								ColumnCount
		{
			get
			{
				return this.columns.Count;
			}
		}

		public TileContainer					LastColumn
		{
			get
			{
				return this.columns.Peek ();
			}
		}

		public double							TotalWidth
		{
			get
			{
				return this.totalWidth;
			}
		}

		
		public TileContainer CreateColumn(CoreViewController controller)
		{
			var column = new TileContainer (controller)
			{
				Name = string.Format ("Column{0}", this.ColumnCount),
				DrawFrameEdges = FrameEdges.Left | FrameEdges.Right,
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


		public override IEnumerable<CoreController> GetSubControllers()
		{
			yield break;
		}
		
		
		internal TileContainer GetColumn(int columnIndex)
		{
			return this.columns.ElementAtOrDefault (columnIndex);
		}

		internal int GetColumnIndex(TileContainer column)
		{
			return this.columns.ToList ().IndexOf (column);
		}

		internal IEnumerable<TileContainer> GetColumns()
		{
			return this.columns.ToList ();
		}


		#region IWidgetUpdater Members

		public void Update()
		{
			this.columns.ForEach (column => column.Update ());
		}

		#endregion
		
		
		private void UpdateColumnLayout()
		{
			List<Visual> visuals = new List<Visual> ();

			if (this.columns.Count > 0)
			{
				double x = 0;
				double overlap = Widgets.Tiles.TileArrow.Breadth + 1;
				int columnIndex = 0;

				var leftToRightTileContainerColumns = this.columns.Reverse ();

				foreach (var column in leftToRightTileContainerColumns)
				{
					column.Anchor = AnchorStyles.TopAndBottom | AnchorStyles.Left;
					column.PreferredWidth = this.GetPreferredColumnWidth (columnIndex++, column);
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

		private double GetPreferredColumnWidth(int columnIndex, TileContainer column)
		{
			return column.GetPreferredWidth (columnIndex, this.columns.Count);
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


		private readonly Widget					container;
		private readonly Stack<TileContainer>	columns;
		private double							totalWidth;
	}
}
