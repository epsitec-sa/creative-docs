//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Drawing;
using Epsitec.Common.Support.Extensions;

using Epsitec.Cresus.Core.Controllers.DataAccessors;
using Epsitec.Cresus.Core.Widgets.Tiles;

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Widgets;

namespace Epsitec.Cresus.Core.Controllers.ActionControllers
{
	public sealed class ActionItemLayout : System.IComparable<ActionItemLayout>
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

		public double							TextWidth
		{
			get
			{
				return this.width;
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
			layout.ComputeWidth ();

			return layout;
		}

		private void ComputeWidth()
		{
			var textLayout = this.CreateTextLayout ();
			var textSize   = textLayout.GetSingleLineSize ();
			
			this.width = System.Math.Ceiling (textSize.Width);
		}


		private TextLayout CreateTextLayout()
		{
			return new TextLayout ()
			{
				FormattedText   = this.Item.Label,
				DefaultFont     = ActionItemLayout.DefaultFont,
				DefaultFontSize = ActionItemLayout.DefaultFontSize
			};
		}

		private void Classify()
		{
			int rowA = 0;
			int rowB = (this.actionTarget == ActionTarget.Primary) ? 1 : 0;

			var actionClass = this.Item.ActionClass.Class;

			switch (actionClass)
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
					this.row = rowA;
					break;

				default:
					throw new System.NotSupportedException (string.Format ("{0} not supported", actionClass.GetQualifiedName ()));
			}
		}

		private void Prioritize()
		{
			this.priority = ActionItemLayout.GetPriority (this.Item.ActionClass.Class);
		}

		private static int GetPriority(ActionClasses actionClass)
		{
			switch (actionClass)
			{
				case ActionClasses.Create:
					return 100;
				case ActionClasses.Validate:
					return 105;
				case ActionClasses.Delete:
					return 110;
				case ActionClasses.Clear:
					return 120;
				
				case ActionClasses.Start:
					return 200;
				case ActionClasses.NextStep:
					return 300;
				case ActionClasses.Cancel:
					return 310;
				case ActionClasses.Stop:
					return 320;
				
				case ActionClasses.Output:
					return 400;
				case ActionClasses.Input:
					return 500;
				case ActionClasses.None:
					return 600;
				
				default:
					throw new System.NotSupportedException (string.Format ("{0} not supported", actionClass.GetQualifiedName ()));
			}
		}

		
		private void Invalidate()
		{
			this.bounds = Rectangle.Empty;
		}

		private enum ActionTarget
		{
			None,
			Primary,
			CollectionItem,
		}

		#region IComparable<ActionItemLayout> Members

		public int CompareTo(ActionItemLayout other)
		{
			if (this.row != other.row)
			{
				return this.row - other.row;
			}
			if (this.priority != other.priority)
			{
				return this.priority - other.priority;
			}

			//	TODO: benchmark this and see if ToSimpleText should optimize for cases where the simple text equals the rich text

			string thisLabel  = this.Item.Label.ToSimpleText ();
			string otherLabel = other.Item.Label.ToSimpleText ();

			return string.CompareOrdinal (thisLabel, otherLabel);
		}

		#endregion


		public static readonly Font				DefaultFont     = Font.DefaultFont;
		public static readonly double			DefaultFontSize = Font.DefaultFontSize;
		
		private readonly ActionItem				item;
		private ControllerTile					container;
		private TitleTile						titleTile;
		private ActionTarget					actionTarget;
		private int								row;
		private int								priority;
		private double							width;
		
		private Rectangle						bounds;
	}
}
