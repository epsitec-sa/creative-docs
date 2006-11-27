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
			
			this.columns.Add (column);
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

		private struct Column
		{
			public Column(ItemPanelColumnHeader header, string propertyName)
			{
				this.property = new PropertyGroupDescription (propertyName);
				this.widget   = new HeaderButton (header);
				
				GridLayoutEngine.SetColumn (header, header.columns.Count);
				GridLayoutEngine.SetRow (header, 0);
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

		private List<Column> columns;
		private GridLayoutEngine gridLayout;
	}
}
