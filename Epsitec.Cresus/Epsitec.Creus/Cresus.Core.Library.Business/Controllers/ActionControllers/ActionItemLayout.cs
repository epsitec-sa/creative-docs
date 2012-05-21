//	Copyright © 2011-2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Drawing;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Widgets;

using Epsitec.Cresus.Core.Controllers.DataAccessors;
using Epsitec.Cresus.Core.Widgets;
using Epsitec.Cresus.Core.Widgets.Tiles;

using System.Collections.Generic;
using System.Linq;

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

		public StaticTitleTile					TitleTile
		{
			get
			{
				return this.titleTile;
			}
		}

		public ControllerTile					Container
		{
			get
			{
				return this.container;
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

		public bool								IsDuplicate
		{
			get
			{
				return this.isDuplicate;
			}
		}

		public Rectangle						Bounds
		{
			get
			{
				return this.bounds;
			}
		}

		public bool								IsIcon
		{
			get
			{
				return ActionItem.IsIcon (this.Item.Label);
			}
		}

		public bool								IsTextTooLarge
		{
			get
			{
				return this.isTextTooLarge;
			}
		}

		public long								SerialId
		{
			get
			{
				return this.serialId;
			}
		}


		public static ActionItemLayout Create(TileDataItem tileDataItem, ActionItem actionItem, long serialId)
		{
			ActionItemLayout layout = new ActionItemLayout (actionItem);

			layout.serialId = serialId;

			switch (tileDataItem.DataType)
			{
				case TileDataType.EmptyItem:
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

		/// <summary>
		/// Updates the layout of all <see cref="ActionItemLayout"/> items.
		/// </summary>
		/// <param name="items">The items.</param>
		public static void UpdateLayout(IEnumerable<ActionItemLayout> items)
		{
			var bucket = new TileActionsBucket ();

			//	Sort all items by title tile and then by rank (i.e. by row, priority and label).

			foreach (var item in items)
			{
				var tileActions = bucket.GetTileActions (item.TitleTile);

				if (tileActions.Add (item) == false)
				{
					item.MarkAsDuplicate ();
				}
			}

			//	Layout every title tile, one after the other:

			foreach (var item in bucket.Items)
			{
				ActionItemLayout.UpdateLayoutsInTile (item.TitleTile, item);
			}
		}

		private void MarkAsDuplicate()
		{
			this.isDuplicate = true;
		}

		/// <summary>
		/// Updates the layouts in the specified tile. The layouts will be processed row by
		/// row and their bounds will be set using root relative coordinates.
		/// </summary>
		/// <param name="tile">The title tile.</param>
		/// <param name="sortedLayouts">The sorted layouts.</param>
		private static void UpdateLayoutsInTile(StaticTitleTile tile, SortedActionItemLayouts sortedLayouts)
		{
			int rowCount = sortedLayouts.RowCount;
			var topRight = ActionItemLayout.GetTitleTileTopRightPointRelativeToRoot (tile);

			for (int row = 0; row < rowCount; row++)
			{
				var layouts  = sortedLayouts.GetItemsInRow (row);
				var position = topRight;

				ActionItemLayout.AdaptWidths (tile, layouts);

				foreach (var item in layouts)
				{
					ActionItemLayout.SetActionItemLayoutBounds (item, position);
					position -= new Point (item.bounds.Width, 0);
				}

				topRight -= new Point (0, ActionItemLayout.DefaultHeight);
			}
		}

		private static void AdaptWidths(StaticTitleTile tile, IEnumerable<ActionItemLayout> layouts)
		{
			//	Si nécessaire, réduit "intelligement" la largeurs des boutons, pour tout caser dans l'espace disponible.
			foreach (var item in layouts)
			{
				if (item.IsIcon)  // icône ?
				{
					item.finalWidth = item.textWidth;
				}
				else  // texte ?
				{
					item.finalWidth = System.Math.Max (item.textWidth + ActionItemLayout.AdditionalTextWidth * 2.0, ActionItemLayout.MinTextWidth);
				}

				int modulo = (int) ActionItemLayout.WidthModulo;
				item.finalWidth = ((int) item.finalWidth + modulo -1) / modulo * modulo;

				item.isTextTooLarge = false;
			}

			var frameWidth = ActionItemLayout.GetTitleTileUsableWidth (tile);

			//	On essaie en premier de réduire la largeur des icônes.
			double actualWidth = layouts.Sum (x => x.finalWidth);

			if (actualWidth > frameWidth)
			{
				//	Les icônes "importantes" sont simplement ramenées au stade d'icônes "normales".
				layouts.Where (x => x.IsIcon).ForEach (x => x.finalWidth = ActionItemLayout.DefaultIconWidth);
			}

			//	On essaie ensuite de réduire la largeur des textes.
			bool changed;
			do
			{
				//	On réduit linéairement la largeur de tous les textes, sans toutefois descendre au-dessous de la
				//	limite inférieure. Comme certains boutons sont ignorés (icônes et textes ayant la largeur minimale),
				//	il faut répéter l'opération tant qu'une réduction quelconque a eu lieu.
				changed = false;
				actualWidth = layouts.Sum (x => x.finalWidth);

				if (actualWidth > frameWidth)
				{
					double fixedWidth = layouts.Where (x => !x.IsIcon && x.finalWidth <= ActionItemLayout.MinTextWidth).Sum (x => x.finalWidth);

					if (actualWidth == fixedWidth)
					{
						break;
					}

					double factor = (frameWidth-fixedWidth) / (actualWidth-fixedWidth);

					layouts.Where (x => !x.IsIcon).ForEach (x => ActionItemLayout.SetFinalWidth (x, factor, ref changed));
				}
			}
			while (changed);
		}

		private static void SetFinalWidth(ActionItemLayout item, double factor, ref bool changed)
		{
			double width = System.Math.Max (System.Math.Floor (item.finalWidth * factor), ActionItemLayout.MinTextWidth);

			if (item.finalWidth != width)
			{
				item.finalWidth = width;
				changed = true;
			}
		}

		private static Point GetTitleTileTopRightPointRelativeToRoot(StaticTitleTile tile)
		{
			return tile.MapClientToRoot (tile.Client.Bounds.TopRight - new Point (14, 3), x => x.IsFence);
		}

		private static double GetTitleTileUsableWidth(StaticTitleTile tile)
		{
			return tile.Client.Size.Width - 4 - Library.UI.Constants.RightMargin;
		}

		private static void SetActionItemLayoutBounds(ActionItemLayout item, Point position)
		{
			double width = item.finalWidth;
			double height = ActionItemLayout.DefaultHeight;

			item.bounds = new Rectangle (position.X - width, position.Y - height, width, height);
		}

		private void ComputeWidth()
		{
			if (this.IsIcon)  // icône ?
			{
				this.textWidth = ActionItemLayout.DefaultIconWidth;

				if (this.item.ActionClass.Class == ActionClasses.Create)  // icône importante ?
				{
					this.textWidth *= 2.0;  // 2x plus large
				}
			}
			else  // texte ?
			{
				var textLayout = this.CreateTextLayout ();
				var textSize   = textLayout.GetSingleLineSize ();

				this.textWidth = System.Math.Ceiling (textSize.Width);
			}
		}
		
		private TextLayout CreateTextLayout()
		{
			return new TextLayout ()
			{
				FormattedText   = this.Item.Label,
				DefaultFont     = ActionButton.DefaultFont,
				DefaultFontSize = ActionButton.DefaultFontSize
			};
		}

		private void Classify()
		{
			int rowA = 0;
			int rowB = (this.actionTarget == ActionTarget.Primary) ? 0 : this.Container.Index + 1;
			int rowC = (this.actionTarget == ActionTarget.Primary) ? 1 : this.Container.Index + 1;

			var actionClass = this.Item.ActionClass.Class;

			switch (actionClass)
			{
				case ActionClasses.Create:
				case ActionClasses.Start:
				case ActionClasses.NextStep:
					this.row = rowA;
					break;

				case ActionClasses.Output:
				case ActionClasses.Input:
					this.row = rowB;
					break;

				case ActionClasses.Delete:
				case ActionClasses.Clear:
				case ActionClasses.Validate:
				case ActionClasses.Cancel:
				case ActionClasses.Stop:
					this.row = rowC;
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

		#region TileActionsBucket Class

		private sealed class TileActionsBucket : Dictionary<long, SortedActionItemLayouts>
		{
			public IEnumerable<SortedActionItemLayouts>		Items
			{
				get
				{
					return this.Values;
				}
			}

			public SortedActionItemLayouts GetTileActions(StaticTitleTile tile)
			{
				var tileId = tile.GetVisualSerialId ();
				SortedActionItemLayouts actions;

				if (this.TryGetValue (tileId, out actions) == false)
				{
					actions = new SortedActionItemLayouts ();
					this[tileId] = actions;
				}
				
				return actions;
			}
		}

		#endregion

		#region SortedActionItemLayouts Class

		private class SortedActionItemLayouts : IEnumerable<ActionItemLayout>
		{
			public SortedActionItemLayouts()
			{
				this.list = new SortedSet<ActionItemLayout> ();
			}

			public StaticTitleTile				TitleTile
			{
				get
				{
					return this.list.Count == 0 ? null : this.list.Min.TitleTile;
				}
			}

			public int							Count
			{
				get
				{
					return this.list.Count;
				}
			}

			public int							RowCount
			{
				get
				{
					if (this.Count == 0)
					{
						return 0;
					}
					else
					{
						return this.list.Select (x => x.row).Max () + 1;
					}
				}
			}

			public IEnumerable<ActionItemLayout> GetItemsInRow(int row)
			{
				return this.list.Where (x => x.row == row);
			}

			public bool Add(ActionItemLayout item)
			{
				return this.list.Add (item);
			}

			#region IEnumerable<ActionItemLayout> Members

			public IEnumerator<ActionItemLayout> GetEnumerator()
			{
				return this.list.GetEnumerator ();
			}

			#endregion

			#region IEnumerable Members

			System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
			{
				return this.list.GetEnumerator ();
			}

			#endregion

			private readonly SortedSet<ActionItemLayout> list;
		}

		#endregion

		#region ActionTarget Enumeration

		private enum ActionTarget
		{
			None,
			Primary,
			CollectionItem,
		}

		#endregion

		#region IComparable<ActionItemLayout> Members

		public int CompareTo(ActionItemLayout other)
		{
			if (this.serialId != other.serialId)
			{
				return (this.serialId < other.serialId) ? -1 : 1;
			}
			if (this.row != other.row)
			{
				return this.row - other.row;
			}
			if (this.priority != other.priority)
			{
				return this.priority - other.priority;
			}
			
			int compareWeights = this.item.Weight.CompareTo (other.item.Weight);
			
			if (compareWeights != 0)
			{
				return compareWeights;
			}

			//	TODO: benchmark this and see if ToSimpleText should optimize for cases where the simple text equals the rich text

			string thisLabel  = this.Item.Label.ToSimpleText ();
			string otherLabel = other.Item.Label.ToSimpleText ();

			return string.CompareOrdinal (thisLabel, otherLabel);
		}

		#endregion


		public static readonly double			DefaultHeight       = 16.0;
		public static readonly double			DefaultIconWidth    = ActionItemLayout.DefaultHeight*1.5;  // bouton icône au format 3:2
		public static readonly double			AdditionalTextWidth = 6.0;   // largeur additionnelle de part et d'autre du texte
		public static readonly double			MinTextWidth        = 24.0;  // largeur minimale d'un bouton textuel
		public static readonly double			WidthModulo         = 24.0;  // multiple pour la largeur de tous les boutons
		
		private readonly ActionItem				item;
		private ControllerTile					container;
		private StaticTitleTile					titleTile;
		private ActionTarget					actionTarget;
		private long							serialId;
		private int								row;
		private int								priority;
		private double							textWidth;
		private double							finalWidth;
		private bool							isTextTooLarge;
		
		private Rectangle						bounds;
		private bool							isDuplicate;
	}
}
