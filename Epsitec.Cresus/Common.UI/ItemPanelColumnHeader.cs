//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
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

		public void AddColumn(string propertyName)
		{
			this.gridLayout.ColumnDefinitions.Add (new ColumnDefinition ());

			Column column = new Column (this, propertyName);

			column.Button.SizeChanged += this.HandleColumnWidthChanged;
			column.Button.Clicked     += this.HandleColumnClicked;
			column.Slider.DragStarted += this.HandleDragStarted;
			column.Slider.DragMoved   += this.HandleDragMoved;
			column.Slider.DragEnded   += this.HandleDragEnded;
			
			this.columns.Add (column);

			this.UpdateSliderZOrder ();
			this.UpdateSliderPositions ();
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
				offset += column.Button.PreferredWidth;
				column.Slider.Margins = new Drawing.Margins (offset-1, 0, 0, 0);
			}
		}

		public void RemoveColumn(int index)
		{
			Column column = this.columns[index];
			
			this.columns.RemoveAt (index);

			column.Button.SizeChanged -= this.HandleColumnWidthChanged;
			column.Button.Clicked     -= this.HandleColumnClicked;
			column.Slider.DragStarted -= this.HandleDragStarted;
			column.Slider.DragMoved   -= this.HandleDragMoved;
			column.Slider.DragEnded   -= this.HandleDragEnded;
			column.Button.Dispose ();
		}
		
		public string GetColumn(int index)
		{
			return this.columns[index].PropertyName;
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
			this.columns[index].Button.PreferredWidth = width;
		}

		public double GetTotalWidth()
		{
			double width = 0;
			
			foreach (Column column in this.columns)
			{
				width += column.Button.PreferredWidth;
			}
			
			return width;
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
			Widget widget = sender as Widget;
			Column column = this.columns[widget.Index];

			List<SortDescription> sorts = new List<SortDescription> ();
			SortDescription newSort = new SortDescription (column.Button.SortMode == SortMode.Down ? ListSortDirection.Descending : ListSortDirection.Ascending, column.PropertyName);
			
			foreach (SortDescription sort in this.ItemPanel.Items.SortDescriptions)
			{
				if (sort.PropertyName != newSort.PropertyName)
				{
					sorts.Add (sort);
				}
			}

			foreach (Column item in this.columns)
			{
				if (item.Button == column.Button)
				{
					item.Button.SortMode = newSort.Direction == ListSortDirection.Ascending ? SortMode.Down : SortMode.Up;
				}
				else
				{
					item.Button.SortMode = SortMode.None;
				}
			}

			this.ItemPanel.Items.SortDescriptions.Clear ();
			this.ItemPanel.Items.SortDescriptions.Add (newSort);
			this.ItemPanel.Items.SortDescriptions.AddRange (sorts);
		}

		private void HandleDragStarted(object sender, MessageEventArgs e)
		{
			HeaderSlider slider = sender as HeaderSlider;

			this.isDragging = true;
			this.dragPos = e.Message.Cursor.X;
			this.dragDim = this.GetColumnWidth (slider.Index);
		}

		private void HandleDragMoved(object sender, MessageEventArgs e)
		{
			HeaderSlider slider = sender as HeaderSlider;

			this.SetColumnWidth (slider.Index, this.dragDim + e.Message.Cursor.X - this.dragPos);
		}

		private void HandleDragEnded(object sender, MessageEventArgs e)
		{
			HeaderSlider slider = sender as HeaderSlider;
			
			this.isDragging = false;
			this.DispatchDummyMouseMoveEvent ();
		}

		private struct Column
		{
			public Column(ItemPanelColumnHeader header, string propertyName)
			{
				this.property = new PropertyGroupDescription (propertyName);
				this.button   = new HeaderButton (header);
				this.slider   = new HeaderSlider (header);

				this.button.Style     = HeaderButtonStyle.Top;
				this.button.IsDynamic = true;
				this.button.Text      = propertyName;
				this.button.Index     = header.columns.Count;

				this.slider.Style          = HeaderSliderStyle.Top;
				this.slider.Index          = header.columns.Count;
				this.slider.Anchor         = AnchorStyles.Left | AnchorStyles.TopAndBottom;
				this.slider.PreferredWidth = 3;
				
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

			public string GetColumnText(object item)
			{
				string[] names = this.property.GetGroupNamesForItem (item, System.Globalization.CultureInfo.CurrentCulture);
				return (names.Length == 0) ? null : names[0];
			}

			private PropertyGroupDescription property;
			private HeaderButton button;
			private HeaderSlider slider;
		}

		private static void NotifyItemPanelChanged(DependencyObject o, object oldValue, object newValue)
		{
			ItemPanelColumnHeader header = (ItemPanelColumnHeader) o;
			header.HandleItemPanelChanged ((ItemPanel) oldValue, (ItemPanel) newValue);
		}

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
		
		public static readonly DependencyProperty ColumnHeaderProperty = DependencyProperty.RegisterAttached ("ColumnHeader", typeof (ItemPanelColumnHeader), typeof (ItemPanelColumnHeader), new DependencyPropertyMetadataWithInheritance ());
		public static readonly DependencyProperty ItemPanelProperty = DependencyProperty.Register ("ItemPanel", typeof (ItemPanel), typeof (ItemPanelColumnHeader), new DependencyPropertyMetadata (ItemPanelColumnHeader.NotifyItemPanelChanged));

		private List<Column> columns;
		private GridLayoutEngine gridLayout;

		private bool isDragging;
		private double dragPos;
		private double dragDim;
	}
}
