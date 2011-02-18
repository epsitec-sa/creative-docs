//	Copyright © 2006-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Common.Types.Collections;
using Epsitec.Common.UI;
using Epsitec.Common.Widgets;
using Epsitec.Common.Widgets.Layouts;

using System.Collections.Generic;

[assembly: DependencyClass (typeof (ItemPanelColumnHeader))]

namespace Epsitec.Common.UI
{
	/// <summary>
	/// The <c>ItemPanelColumnHeader</c> ...
	/// </summary>
	public class ItemPanelColumnHeader : Widgets.FrameBox
	{
		public ItemPanelColumnHeader()
		{
			this.columns = new List<Column> ();
			this.sorts = new List<SortRecord> ();
			this.gridLayout = new GridLayoutEngine ();

			this.gridLayout.RowDefinitions.Add (new RowDefinition ());
			
			LayoutEngine.SetLayoutEngine (this, this.gridLayout);
		}

		public ItemPanelColumnHeader(Widget embedder)
			: this ()
		{
			this.SetEmbedder (embedder);
		}

		public ItemPanel ItemPanel
		{
			get
			{
				return (ItemPanel) this.GetValue (ItemPanelColumnHeader.ItemPanelProperty);
			}
			set
			{
				this.SetValue (ItemPanelColumnHeader.ItemPanelProperty, value);
			}
		}

		public int ColumnCount
		{
			get
			{
				return this.columns.Count;
			}
		}

		public void AddColumn(StructuredTypeField field)
		{
			this.AddColumn (-1, field.Id, field.CaptionId);
		}

		public void AddColumn(int id, StructuredTypeField field)
		{
			this.AddColumn (id, field.Id, field.CaptionId);
		}

		public void AddColumn(string propertyName, Support.Druid captionId)
		{
			this.AddColumn (-1, propertyName, captionId);
		}

		public void AddColumn(int id, string propertyName, Support.Druid captionId)
		{
			this.gridLayout.ColumnDefinitions.Add (new ColumnDefinition ());

			Column column = new Column (this, propertyName, captionId, id);

			column.Button.SizeChanged += this.HandleColumnWidthChanged;
			column.Button.Clicked     += this.HandleColumnClicked;
			column.Slider.DragStarted += this.HandleDragStarted;
			column.Slider.DragMoved   += this.HandleDragMoved;
			column.Slider.DragEnded   += this.HandleDragEnded;
			
			this.columns.Add (column);

			this.UpdateSliderZOrder ();
			this.UpdateSliderPositions ();
		}

		public void RemoveColumn(int index)
		{
			Column column = this.columns[index];

			this.columns.RemoveAt (index);
			this.gridLayout.ColumnDefinitions.RemoveAt (index);

			column.Button.SizeChanged -= this.HandleColumnWidthChanged;
			column.Button.Clicked     -= this.HandleColumnClicked;
			column.Slider.DragStarted -= this.HandleDragStarted;
			column.Slider.DragMoved   -= this.HandleDragMoved;
			column.Slider.DragEnded   -= this.HandleDragEnded;
			column.Button.Dispose ();
		}

		public void ClearColumns()
		{
			while (this.columns.Count > 0)
			{
				this.RemoveColumn (this.columns.Count-1);
			}
		}

		public string GetColumnPropertyName(int index)
		{
			return this.columns[index].PropertyName;
		}

		public int GetColumnId(int index)
		{
			return this.columns[index].Id;
		}

		public void SetColumnComparer(int index, Support.PropertyComparer propertyComparer)
		{
			ItemTableColumn.SetComparer (this.columns[index].Button, propertyComparer);
		}

		public void SetColumnText(int index, string text)
		{
			this.columns[index].Button.CaptionId = Support.Druid.Empty;
			this.columns[index].Button.Text      = text;
		}

		public Support.PropertyComparer GetColumnComparer(int index)
		{
			return ItemTableColumn.GetComparer (this.columns[index].Button);
		}

		public bool IsColumnSortable(int index)
		{
			return this.columns[index].Button.IsSortable;
		}

		public void SetColumnSortable(int index, bool sortable)
		{
			this.columns[index].Button.IsSortable = sortable;
		}

		public SortDescription GetColumnSortDescription(int index)
		{
			string propertyName = this.columns[index].PropertyName;

			foreach (SortDescription sort in this.ItemPanel.Items.SortDescriptions)
			{
				if (sort.PropertyName == propertyName)
				{
					return sort;
				}
			}

			return SortDescription.Empty;
		}

		public void SetColumnVisibility(int index, bool visible)
		{
			this.columns[index].Button.Visibility = visible;
			this.columns[index].Slider.Visibility = visible & !this.columns[index].FixedWidth;
			this.gridLayout.ColumnDefinitions[index].Visibility = visible;
		}

		public void SetColumnFixedWidth(int index, bool fixedWidth)
		{
			this.columns[index].FixedWidth = fixedWidth;
		}

		public void SetColumnContentAlignment(int index, Drawing.ContentAlignment alignment)
		{
			this.columns[index].Button.ContentAlignment = alignment;
		}

		public Drawing.ContentAlignment GetColumnContentAlignment(int index)
		{
			return this.columns[index].Button.ContentAlignment;
		}
		
		public void SetColumnSort(int index, ListSortDirection sortDirection)
		{
			if (sortDirection != ListSortDirection.None)
			{
				this.SetColumnSort (index, new SortDescription (sortDirection, this.columns[index].PropertyName, this.GetColumnComparer (index)));
			}
		}

		public void SetColumnSort(int index, SortDescription sortDescription)
		{
			List<SortRecord> sorts = new List<SortRecord> ();

			foreach (SortRecord record in this.sorts)
			{
				if (record.Column != index)
				{
					sorts.Add (record);
				}
			}

			if (!sortDescription.IsEmpty)
			{
				sorts.Add (new SortRecord (index, sortDescription));
			}

			this.sorts = sorts;
			this.UpdateColumnSorts ();
		}

		public void UpdateColumnSorts()
		{
			//Column column = this.columns[index];
			//string propertyName = column.PropertyName;
			//Support.PropertyComparer comparer = this.GetColumnComparer (index);
			//new SortDescription (sortDescription.Direction, propertyName, comparer)
			List<SortDescription> sorts = new List<SortDescription> ();

			foreach (SortRecord record in this.sorts)
			{
				sorts.Insert (0, record.Sort);
			}

			for (int i = 0; i < this.columns.Count; i++)
			{
				this.columns[i].Button.SortMode = SortMode.None;
			}

			for (int i = this.sorts.Count; --i >= 0; )
			{
				SortRecord record = this.sorts[i];

				int index = record.Column;

				if (this.columns[index].Button.IsSortable)
				{
					this.columns[index].Button.SortMode = record.Sort.Direction == ListSortDirection.Ascending ? SortMode.Down : SortMode.Up;
					break;
				}
			}

			if (this.ItemPanel.Items != null)
			{
				this.ItemPanel.Items.SortDescriptions.Clear ();
				this.ItemPanel.Items.SortDescriptions.AddRange (sorts);
			}
		}

		private struct SortRecord
		{
			public SortRecord(int column, SortDescription sort)
			{
				this.column = column;
				this.sort   = sort;
			}

			public int Column
			{
				get
				{
					return this.column;
				}
			}

			public SortDescription Sort
			{
				get
				{
					return this.sort;
				}
			}

			private int column;
			private SortDescription sort;
		}

		public ColumnDefinition GetColumnDefinition(int index)
		{
			return this.gridLayout.ColumnDefinitions[index];
		}

		public string GetColumnText(int index, object item)
		{
			return this.columns[index].GetColumnText (item);
		}

		public int DetectColumn(Drawing.Point pos)
		{
			return -1;
		}

		public double GetColumnWidth(int index)
		{
			return this.columns[index].Button.PreferredWidth;
		}

		public void SetColumnWidth(int index, double width)
		{
			double oldWidth = this.columns[index].Button.PreferredWidth;
			double newWidth = width;
			
			if (oldWidth != newWidth)
			{
				ColumnWidthChangeEventArgs e = new ColumnWidthChangeEventArgs (index, oldWidth, newWidth);

				this.OnColumnWidthChanging (e);

				width = e.Width;

				if (width != oldWidth)
				{
					this.columns[index].Button.PreferredWidth = width;
					this.OnColumnWidthChanged (new ColumnWidthChangeEventArgs (index, oldWidth, width));
				}
			}
		}

		public void AdjustColumnWidth(int index)
		{
			this.SetColumnWidth (index, this.columns[index].Button.GetBestFitSize ().Width);
		}

		public override Drawing.Size GetBestFitSize()
		{
			double width = this.GetTotalWidth ();
			
			if (this.columns.Count > 0)
			{
				width += (this.columns[0].Slider.PreferredWidth + 1) / 2;
			}

			return new Drawing.Size (width, this.PreferredHeight);
		}

		public double GetTotalWidth()
		{
			double width = 0;
			
			foreach (Column column in this.columns)
			{
				if (column.Button.Visibility)
				{
					width += column.Button.PreferredWidth;
				}
			}
			
			return width;
		}

		protected virtual void OnColumnWidthChanging(ColumnWidthChangeEventArgs e)
		{
			Support.EventHandler<ColumnWidthChangeEventArgs> handler = this.GetUserEventHandler<ColumnWidthChangeEventArgs> (ItemPanelColumnHeader.ColumnWidthChangingEventName);

			if (handler != null)
			{
				handler (this, e);
			}
		}

		protected virtual void OnColumnWidthChanged(ColumnWidthChangeEventArgs e)
		{
			Support.EventHandler<ColumnWidthChangeEventArgs> handler = this.GetUserEventHandler<ColumnWidthChangeEventArgs> (ItemPanelColumnHeader.ColumnWidthChangedEventName);

			if (handler != null)
			{
				handler (this, e);
			}
		}

		private void UpdateSliderZOrder()
		{
			foreach (Column column in this.columns)
			{
				column.Slider.ZOrder = 0;
			}
		}

		private void UpdateSliderPositions()
		{
			double offset = 0;

			foreach (Column column in this.columns)
			{
				if (column.Button.Visibility)
				{
					offset += column.Button.PreferredWidth;
					column.Slider.Margins = new Drawing.Margins (offset-(column.Slider.PreferredWidth-1)/2, 0, 0, 0);
				}
			}
		}

		private void HandleItemPanelChanged(ItemPanel oldValue, ItemPanel newValue)
		{
			if (oldValue != null)
			{
				ItemPanelColumnHeader.SetColumnHeader (oldValue, null);
			}
			
			if (newValue != null)
			{
				ItemPanelColumnHeader.SetColumnHeader (newValue, this);
			}
		}

		private void HandleColumnWidthChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			this.UpdateSliderPositions ();
			this.ItemPanel.AsyncRefresh ();
		}

		private void HandleColumnClicked(object sender, MessageEventArgs e)
		{
			if (e.Message.Button != MouseButtons.Left)
			{
				return;
			}

			Widget widget = sender as Widget;
			int    index  = widget.Index;
			Column column = this.columns[index];

			if (!column.Button.IsSortable)
			{
				return;
			}

			e.Message.Consumer = this;
			
			if (string.IsNullOrEmpty (column.PropertyName))
			{
				return;
			}

			this.SetColumnSort (index, column.Button.SortMode == SortMode.Down ? ListSortDirection.Descending : ListSortDirection.Ascending);
		}

		private void HandleDragStarted(object sender, MessageEventArgs e)
		{
			HeaderSlider slider = sender as HeaderSlider;

			double x = Widgets.Helpers.VisualTree.MapParentToScreen (this, e.Message.Cursor).X;

			this.dragPos = x;
			this.dragDim = this.GetColumnWidth (slider.Index);
		}

		private void HandleDragMoved(object sender, MessageEventArgs e)
		{
			HeaderSlider slider = sender as HeaderSlider;

			double x = Widgets.Helpers.VisualTree.MapParentToScreen (this, e.Message.Cursor).X;

			this.SetColumnWidth (slider.Index, System.Math.Max (0, this.dragDim + x - this.dragPos));
		}

		private void HandleDragEnded(object sender, MessageEventArgs e)
		{
			HeaderSlider slider = sender as HeaderSlider;
			
			this.DispatchDummyMouseMoveEvent ();
		}


		#region Column Class

		private class Column
		{
			public Column(ItemPanelColumnHeader header, string propertyName, Support.Druid captionId, int id)
			{
				this.property   = new PropertyGroupDescription (propertyName);
				this.button     = new HeaderButton (header);
				this.slider     = new HeaderSlider (header);
				this.id         = id;
				this.fixedWidth = false;

				this.button.Style     = HeaderButtonStyle.Top;
				this.button.IsDynamic = true;
				this.button.CaptionId = captionId;
				this.button.Index     = header.columns.Count;
				this.button.PreferredHeight *= ItemTable.HeaderHeightFactor;

				if (captionId.IsEmpty)
				{
					this.button.Text = "";
					this.button.Name = propertyName;
				}

				this.slider.Style          = HeaderSliderStyle.Top;
				this.slider.Index          = header.columns.Count;
				this.slider.Anchor         = AnchorStyles.Left | AnchorStyles.TopAndBottom;
				this.slider.PreferredWidth = 5;
				this.slider.PreferredHeight *= ItemTable.HeaderHeightFactor;
				this.slider.AutoFocus      = false;
				
				GridLayoutEngine.SetColumn (this.button, header.columns.Count);
				GridLayoutEngine.SetRow (this.button, 0);
			}

			public string PropertyName
			{
				get
				{
					return this.property.PropertyName;
				}
			}

			public HeaderButton Button
			{
				get
				{
					return this.button;
				}
			}

			public HeaderSlider Slider
			{
				get
				{
					return this.slider;
				}
			}

			public int Id
			{
				get
				{
					return this.id;
				}
			}

			public bool FixedWidth
			{
				get
				{
					return this.fixedWidth;
				}
				set
				{
					if (this.fixedWidth != value)
					{
						this.fixedWidth = value;
						this.slider.Visibility = this.button.Visibility & !this.fixedWidth;
					}
				}
			}

			public string GetColumnText(object item)
			{
				string[] names = this.property.GetGroupNamesForItem (item, System.Globalization.CultureInfo.CurrentCulture);
				return (names.Length == 0) ? null : names[0];
			}

			private PropertyGroupDescription property;
			private HeaderButton button;
			private HeaderSlider slider;
			private int id;
			private bool fixedWidth;
		}

		#endregion

		private static void NotifyItemPanelChanged(DependencyObject o, object oldValue, object newValue)
		{
			ItemPanelColumnHeader header = (ItemPanelColumnHeader) o;
			header.HandleItemPanelChanged ((ItemPanel) oldValue, (ItemPanel) newValue);
		}


		public event Support.EventHandler<ColumnWidthChangeEventArgs> ColumnWidthChanging
		{
			add
			{
				this.AddUserEventHandler (ItemPanelColumnHeader.ColumnWidthChangingEventName, value);
			}
			remove
			{
				this.RemoveUserEventHandler (ItemPanelColumnHeader.ColumnWidthChangingEventName, value);
			}
		}

		public event Support.EventHandler<ColumnWidthChangeEventArgs> ColumnWidthChanged
		{
			add
			{
				this.AddUserEventHandler (ItemPanelColumnHeader.ColumnWidthChangedEventName, value);
			}
			remove
			{
				this.RemoveUserEventHandler (ItemPanelColumnHeader.ColumnWidthChangedEventName, value);
			}
		}


		private static string ColumnWidthChangingEventName = "ColumnWidthChanging";
		private static string ColumnWidthChangedEventName  = "ColumnWidthChanged";

		
		public static void SetColumnHeader(DependencyObject obj, ItemPanelColumnHeader header)
		{
			if (header == null)
			{
				obj.ClearValue (ItemPanelColumnHeader.ColumnHeaderProperty);
			}
			else
			{
				obj.SetValue (ItemPanelColumnHeader.ColumnHeaderProperty, header);
			}
		}

		public static ItemPanelColumnHeader GetColumnHeader(DependencyObject obj)
		{
			return (ItemPanelColumnHeader) obj.GetValue (ItemPanelColumnHeader.ColumnHeaderProperty);
		}
		
		public static readonly DependencyProperty ColumnHeaderProperty = DependencyProperty.RegisterAttached ("ColumnHeader", typeof (ItemPanelColumnHeader), typeof (ItemPanelColumnHeader), new DependencyPropertyMetadata ());
		public static readonly DependencyProperty ItemPanelProperty = DependencyProperty.Register ("ItemPanel", typeof (ItemPanel), typeof (ItemPanelColumnHeader), new DependencyPropertyMetadata (ItemPanelColumnHeader.NotifyItemPanelChanged));

		private List<Column> columns;
		private List<SortRecord> sorts;
		private GridLayoutEngine gridLayout;

		private double dragPos;
		private double dragDim;
	}
}
