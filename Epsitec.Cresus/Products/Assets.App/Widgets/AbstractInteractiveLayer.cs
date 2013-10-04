//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;

namespace Epsitec.Cresus.Assets.App.Widgets
{
	public abstract class AbstractInteractiveLayer
	{
		public AbstractInteractiveLayer(TreeTable treeTable)
		{
			this.treeTable = treeTable;
		}


		public void CreateUI()
		{
			this.foreground = new Foreground
			{
				Parent  = this.treeTable,
				Anchor  = AnchorStyles.All,
				Margins = new Margins (0, 0, 0, AbstractScroller.DefaultBreadth),
			};
		}


		public virtual bool IsDragging
		{
			get
			{
				return false;
			}
		}

		public virtual void BeginDrag(Point pos)
		{
		}

		public virtual void ProcessDrag(Point pos)
		{
		}

		public virtual void EndDrag(Point pos)
		{
		}


		protected void ClearForeground()
		{
			this.foreground.ClearZones ();
			this.foreground.Invalidate ();
		}


		protected FrameBox LeftContainer
		{
			get
			{
				return this.treeTable.LeftContainer;
			}
		}

		protected Scrollable ColumnsContainer
		{
			get
			{
				return this.treeTable.ColumnsContainer;
			}
		}

		protected int HeaderHeight
		{
			get
			{
				return this.treeTable.HeaderHeight;
			}
		}

		protected int ColumnCount
		{
			get
			{
				return this.treeTable.TreeTableColumns.Count;
			}
		}

		protected AbstractTreeTableColumn GetColumn(int rank)
		{
			return this.treeTable.TreeTableColumns[rank];
		}


		protected readonly TreeTable			treeTable;
		protected Foreground					foreground;
	}
}