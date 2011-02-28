//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Widgets;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Widgets.Tiles
{
	public class ControllerTile : Tile
	{
		protected ControllerTile(Direction direction)
			: base (direction)
		{
		}

		public override bool IsDraggable
		{
			get
			{
				return (this.Controller == null || !this.IsDragAndDropEnabled || this.IsSelected) ? false : true;
			}
		}
		public virtual Controllers.ITileController Controller
		{
			get
			{
				return null;
			}
			set
			{
				throw new System.InvalidOperationException ();
			}
		}
		protected override int GroupedItemIndex
		{
			get
			{
				var grouped = this.Controller as Epsitec.Cresus.Core.Controllers.IGroupedItem;

				if (grouped == null)
				{
					return -1;
				}
				else
				{
					return grouped.GroupedItemIndex;
				}
			}
			set
			{
				var grouped = this.Controller as Epsitec.Cresus.Core.Controllers.IGroupedItem;

				if (grouped != null)
				{
					grouped.GroupedItemIndex = value;
				}
			}
		}

		protected override string GroupId
		{
			get
			{
				var grouped = this.Controller as Epsitec.Cresus.Core.Controllers.IGroupedItem;

				if (grouped == null)
				{
					return null;
				}
				else
				{
					return grouped.GetGroupId ();
				}
			}
		}
	}
}
