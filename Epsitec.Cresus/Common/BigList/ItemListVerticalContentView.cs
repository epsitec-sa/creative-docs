//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.BigList;
using Epsitec.Common.BigList.Processors;
using Epsitec.Common.Drawing;
using Epsitec.Common.Support;
using Epsitec.Common.Types;
using Epsitec.Common.Widgets;

using System.Collections.Generic;
using System.Linq;

[assembly: DependencyClass (typeof (ItemListVerticalContentView))]

namespace Epsitec.Common.BigList
{
	public partial class ItemListVerticalContentView : Widget
	{
		public ItemListVerticalContentView()
		{
			this.DefaultLineHeight = 20;
			this.processor = new ItemListVerticalContentView.EventProcessor (this);
		}


		public ItemList							ItemList
		{
			get
			{
				return this.list;
			}
			set
			{
				if (this.list != value)
				{
					this.list = value;
					this.UpdateListHeight ();
				}
			}
		}

		public IItemDataRenderer				ItemRenderer
		{
			get;
			set;
		}

		public IItemMarkRenderer				MarkRenderer
		{
			get;
			set;
		}

		public int								DefaultLineHeight
		{
			get;
			set;
		}

		public int								ActiveIndex
		{
			get
			{
				return this.ItemList.ActiveIndex;
			}
		}


		public void ActivateRow(int index)
		{
			var oldIndex = this.ActiveIndex;

			this.ItemList.SetActiveIndex (index);

			var newIndex = this.ActiveIndex;

			if (oldIndex != newIndex)
			{
				this.InvalidateProperty (ItemListVerticalContentView.ActiveIndexProperty, oldIndex, newIndex);
			}

			this.Invalidate ();
		}

		public void FocusRow(int index)
		{
			this.ItemList.SetVisibleIndex (index);
			this.Invalidate ();
		}

		public void SelectRow(int index, ItemSelection selection)
		{
			if (this.ItemList.Select (index, selection))
			{
				this.OnSelectionChanged ();
				this.Invalidate ();
			}
		}

		public void Scroll(double amplitude)
		{
			amplitude *= this.DefaultLineHeight;

			this.ItemList.MoveVisibleContent ((int) (amplitude));
			this.Invalidate ();
		}

		
		public Rectangle GetRowBounds(ItemListRow row)
		{
			double dx = this.Client.Width;
			double dy = this.Client.Height;

			double y2 = dy - row.Offset - row.Height.MarginBefore;
			double y1 = y2 - row.Height.Height;

			return new Rectangle (0, y1, dx, y2-y1);
		}

		public Rectangle GetMarkBounds(ItemListMark mark)
		{
			var offset = this.list.GetOffset (mark);

			if (offset.IsVisible == false)
			{
				return Rectangle.Empty;
			}

			double dx = this.Client.Width;
			double dy = this.Client.Height;

			double y2 = dy - offset.Offset + mark.Breadth / 2.0;
			double y1 = y2 - mark.Breadth;

			return new Rectangle (0, y1, dx, y2-y1);
		}


		
		protected override void UpdateClientGeometry()
		{
			base.UpdateClientGeometry ();

			this.UpdateListHeight ();
		}

		protected override void PaintBackgroundImplementation(Graphics graphics, Rectangle clipRect)
		{
			this.PaintContents (graphics, clipRect);
		}

		protected override void ProcessMessage(Message message, Point pos)
		{
			if (this.processor.ProcessMessage (message, pos))
			{
				message.Consumer = this;
			}
		}


		protected virtual void OnSelectionChanged()
		{
			this.GetUserEventHandler (ItemListVerticalContentView.SelectionChangedEvent).Raise (this);
		}


		
		private void PaintContents(Graphics graphics, Rectangle clipRect)
		{
			this.PaintRows (graphics, clipRect);
			this.PaintMarks (graphics, clipRect);
		}

		private void PaintRows(Graphics graphics, Rectangle clipRect)
		{
			if (this.ItemRenderer == null)
			{
				return;
			}

			foreach (var row in this.list.VisibleRows)
			{
				var bounds = this.GetRowBounds (row);

				if (bounds.IntersectsWith (clipRect) == false)
				{
					continue;
				}

				var data  = this.list.Cache.GetItemData (row.Index);
				var state = this.list.GetItemState (row.Index);

				this.ItemRenderer.Render (data, state, row, graphics, bounds);
			}
		}

		private void PaintMarks(Graphics graphics, Rectangle clipRect)
		{
			if (this.MarkRenderer == null)
			{
				return;
			}

			foreach (var mark in this.list.Marks)
			{
				Rectangle bounds = this.GetMarkBounds (mark);

				if (bounds.IntersectsWith (clipRect) == false)
				{
					continue;
				}
				
				var offset = this.list.GetOffset (mark);

				this.MarkRenderer.Render (mark, offset, graphics, bounds);
			}
		}

		private void UpdateListHeight()
		{
			if ((this.IsActualGeometryValid) &&
				(this.list != null))
			{
				this.list.VisibleHeight = (int) System.Math.Floor (this.Client.Height);
			}
		}




		public event EventHandler<DependencyPropertyChangedEventArgs> ActiveIndexChanged
		{
			add
			{
				this.AddEventHandler (ItemListVerticalContentView.ActiveIndexProperty, value);
			}
			remove
			{
				this.RemoveEventHandler (ItemListVerticalContentView.ActiveIndexProperty, value);
			}
		}

		public event EventHandler				SelectionChanged
		{
			add
			{
				this.AddUserEventHandler (ItemListVerticalContentView.SelectionChangedEvent, value);
			}
			remove
			{
				this.RemoveUserEventHandler (ItemListVerticalContentView.SelectionChangedEvent, value);
			}
		}

		#region Strings

		private const string					SelectionChangedEvent = "SelectionChanged";

		#endregion

		public static DependencyProperty		ActiveIndexProperty = DependencyProperty<ItemListVerticalContentView>.RegisterReadOnly<int> (x => x.ActiveIndex, x => x.ActiveIndex);

		private ItemList						list;
		private ItemListVerticalContentView.EventProcessor					processor;
	}
}
