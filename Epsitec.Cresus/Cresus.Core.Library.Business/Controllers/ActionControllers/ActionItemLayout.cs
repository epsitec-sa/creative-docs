//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Drawing;

using Epsitec.Cresus.Core.Controllers.DataAccessors;
using Epsitec.Cresus.Core.Widgets.Tiles;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.ActionControllers
{
	public sealed class ActionItemLayout
	{
		public ActionItemLayout(ActionItem item)
		{
			this.item = item;
		}


		public ActionItem						Item
		{
			get
			{
				return this.item;
			}
		}

		public TitleTile						TitleTile
		{
			get
			{
				return this.titleTile;
			}
			set
			{
				if (this.titleTile != value)
				{
					this.titleTile = value;
					this.Invalidate ();
				}
			}
		}

		public ControllerTile					Container
		{
			get
			{
				return this.container;
			}
			set
			{
				if (this.container != value)
				{
					this.container = value;
					this.Invalidate ();
				}
			}
		}

		public bool								IsValid
		{
			get
			{
				return this.bounds.IsValid;
			}
		}

		public bool								IsInvalid
		{
			get
			{
				return this.bounds.IsEmpty;
			}
		}

		public Rectangle						Bounds
		{
			get
			{
				return this.bounds;
			}
		}



		public static ActionItemLayout Create(TileDataItem tileDataItem, ActionItem actionItem)
		{
			ActionItemLayout layout = new ActionItemLayout (actionItem);

			switch (tileDataItem.DataType)
			{
				case TileDataType.CollectionItem:
					layout.actionTarget = ActionTarget.CollectionItem;
					layout.titleTile    = tileDataItem.TitleTile;
					layout.container    = tileDataItem.Tile;
					break;
				
				case TileDataType.SimpleItem:
					layout.actionTarget = ActionTarget.Primary;
					layout.titleTile    = tileDataItem.TitleTile;
					layout.container    = tileDataItem.TitleTile;
					break;

				default:
					return null;
			}

			layout.Classify ();
			layout.Prioritize ();

			return layout;
		}


		private void Classify()
		{
			int rowA = 0;
			int rowB = 0;
			
			if (this.actionTarget == ActionTarget.Primary)
			{
				rowB = 1;
			}

			switch (this.Item.ActionClass.Class)
			{
				case ActionClasses.Create:
				case ActionClasses.Start:
				case ActionClasses.NextStep:
				case ActionClasses.Output:
				case ActionClasses.Input:
					this.row = rowA;
					break;

				case ActionClasses.Delete:
				case ActionClasses.Clear:
				case ActionClasses.Validate:
				case ActionClasses.Cancel:
				case ActionClasses.Stop:
					this.row = rowB;	
					break;

				case ActionClasses.None:
				default:
					this.row = rowA;
					break;
			}
		}

		private void Prioritize()
		{
			switch (this.Item.ActionClass.Class)
			{
				case ActionClasses.Create:
				case ActionClasses.Start:
				case ActionClasses.NextStep:
				case ActionClasses.Output:
				case ActionClasses.Input:
				case ActionClasses.Delete:
				case ActionClasses.Clear:
				case ActionClasses.Validate:
				case ActionClasses.Cancel:
				case ActionClasses.Stop:
				case ActionClasses.None:
				default:
					break;
			}
		}

		
		private void Invalidate()
		{
			this.bounds = Rectangle.Empty;
		}

		enum ActionTarget
		{
			None,
			Primary,
			CollectionItem,
		}

		
		private readonly ActionItem				item;
		private ControllerTile					container;
		private TitleTile						titleTile;
		private Rectangle						bounds;
		private ActionTarget					actionTarget;
		private int								row;
		private int								priority;
	}
}
