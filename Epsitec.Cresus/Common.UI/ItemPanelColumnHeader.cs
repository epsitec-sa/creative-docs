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

			column.Widget.SizeChanged += this.HandleColumnWidthChanged;
			
			this.columns.Add (column);
		}

		public void RemoveColumn(int index)
		{
			Column column = this.columns[index];
			
			this.columns.RemoveAt (index);

			column.Widget.SizeChanged -= this.HandleColumnWidthChanged;
			column.Widget.Dispose ();
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
			return this.columns[index].Widget.ActualWidth;
		}

		public double GetTotalWidth()
		{
			double width = 0;
			
			foreach (Column column in this.columns)
			{
				width += column.Widget.ActualWidth;
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
			this.ItemPanel.AsyncRefresh ();
		}

		private struct Column
		{
			public Column(ItemPanelColumnHeader header, string propertyName)
			{
				this.property = new PropertyGroupDescription (propertyName);
				this.widget   = new HeaderButton (header);
				
				this.widget.Text = propertyName;
				
				GridLayoutEngine.SetColumn (this.widget, header.columns.Count);
				GridLayoutEngine.SetRow (this.widget, 0);
			}

			public string PropertyName
			{
				get
				{
					return this.property.PropertyName;
				}
			}

			public HeaderButton Widget
			{
				get
				{
					return this.widget;
				}
			}

			public string GetColumnText(object item)
			{
				string[] names = this.property.GetGroupNamesForItem (item, System.Globalization.CultureInfo.CurrentCulture);
				return (names.Length == 0) ? null : names[0];
			}

			private PropertyGroupDescription property;
			private HeaderButton widget;
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
	}
}
