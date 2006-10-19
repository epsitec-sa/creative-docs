//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;
using NUnit.Framework;

namespace Epsitec.Common.Types
{
	[TestFixture]
	public class CollectionTest
	{
		[Test]
		public void CheckSortDescription()
		{
			SortDescription sort1 = new SortDescription (ListSortDirection.Ascending, "x");
			SortDescription sort2 = new SortDescription (ListSortDirection.Descending, "y");

			Assert.AreEqual (ListSortDirection.Ascending, sort1.Direction);
			Assert.AreEqual (ListSortDirection.Descending, sort2.Direction);
			Assert.AreEqual ("x", sort1.PropertyName);
			Assert.AreEqual ("y", sort2.PropertyName);

			ISerializationConverter conv = InvariantConverter.GetSerializationConverter (typeof (SortDescription));

			Assert.AreEqual ("A;x", conv.ConvertToString (sort1, null));
			Assert.AreEqual ("D;y", conv.ConvertToString (sort2, null));

			Assert.AreEqual (sort1, conv.ConvertFromString ("A;x", null));
			Assert.AreEqual (sort2, conv.ConvertFromString ("D;y", null));
		}

		[Test]
		public void CheckPropertyGroupDescription()
		{
			PropertyGroupDescription group = new PropertyGroupDescription ();

			group.PropertyName = null;
			group.Converter = Converters.AutomaticValueConverter.Instance;

			//	Convert simple types to string
			
			Assert.AreEqual ("1.1", string.Join (":", group.GetGroupNamesForItem (1.1, System.Globalization.CultureInfo.GetCultureInfoByIetfLanguageTag ("en-US"))));
			Assert.AreEqual ("1,1", string.Join (":", group.GetGroupNamesForItem (1.1, System.Globalization.CultureInfo.GetCultureInfoByIetfLanguageTag ("fr-FR"))));

			Assert.AreEqual (0, group.GetGroupNamesForItem (null, System.Globalization.CultureInfo.InvariantCulture).Length);
			
			//	Convert DependencyObject to string
			
			MyItem item = new MyItem ("X");

			//	Automatic converter, no property :

			Assert.AreEqual (1, group.GetGroupNamesForItem (item, System.Globalization.CultureInfo.InvariantCulture).Length);
			Assert.AreEqual ("MyItem:X", string.Join (":", group.GetGroupNamesForItem (item, System.Globalization.CultureInfo.InvariantCulture)));

			//	No converter, no property :
			
			group.Converter = null;

			Assert.AreEqual (1, group.GetGroupNamesForItem (item, System.Globalization.CultureInfo.InvariantCulture).Length);
			Assert.AreEqual ("Epsitec.Common.Types.CollectionTest+MyItem", string.Join (":", group.GetGroupNamesForItem (item, System.Globalization.CultureInfo.InvariantCulture)));

			//	Property, no converter :
			
			group.PropertyName = "Name";
			
			Assert.AreEqual (1, group.GetGroupNamesForItem (item, System.Globalization.CultureInfo.InvariantCulture).Length);
			Assert.AreEqual ("X", string.Join (":", group.GetGroupNamesForItem (item, System.Globalization.CultureInfo.InvariantCulture)));

			Assert.AreEqual (System.StringComparison.Ordinal, group.StringComparison);

			Assert.IsTrue (group.NamesMatch ("A", "A"));
			Assert.IsFalse (group.NamesMatch ("A", "a"));

			group.StringComparison = System.StringComparison.InvariantCultureIgnoreCase;
			
			Assert.IsTrue (group.NamesMatch ("A", "a"));
		}

		[Test]
		public void CheckCollectionViewSort()
		{
			List<Record> source = new List<Record> ();
			CollectionView view = new CollectionView (source);

			CollectionTest.AddRecords (source);

			view.Refresh ();

			Assert.AreEqual (6, view.Groups.Count);
			Assert.AreEqual ("Vis M3", ((Record) view.Groups[0]).Article);
			Assert.AreEqual ("Ecrou M3", ((Record) view.Groups[1]).Article);
			Assert.AreEqual ("Rondelle", ((Record) view.Groups[2]).Article);
			Assert.AreEqual ("Clé M3", ((Record) view.Groups[3]).Article);
			Assert.AreEqual ("Tournevis", ((Record) view.Groups[4]).Article);
			Assert.AreEqual ("Tournevis", ((Record) view.Groups[5]).Article);

			view.SortDescriptions.Add (new SortDescription (ListSortDirection.Ascending, "Article"));
			view.SortDescriptions.Add (new SortDescription (ListSortDirection.Ascending, "Stock"));
			view.Refresh ();

			Assert.AreEqual (6, view.Groups.Count);
			Assert.AreEqual ("Clé M3", ((Record) view.Groups[0]).Article);
			Assert.AreEqual ("Ecrou M3", ((Record) view.Groups[1]).Article);
			Assert.AreEqual ("Rondelle", ((Record) view.Groups[2]).Article);
			Assert.AreEqual ("Tournevis", ((Record) view.Groups[3]).Article);
			Assert.AreEqual ("Tournevis", ((Record) view.Groups[4]).Article);
			Assert.AreEqual ("Vis M3", ((Record) view.Groups[5]).Article);

			Assert.AreEqual (2, ((Record) view.Groups[3]).Stock);
			Assert.AreEqual (7, ((Record) view.Groups[4]).Stock);

			view.SortDescriptions.RemoveAt (1);
			view.SortDescriptions.Add (new SortDescription (ListSortDirection.Descending, "Stock"));
			view.Refresh ();

			Assert.AreEqual (6, view.Groups.Count);
			Assert.AreEqual ("Clé M3", ((Record) view.Groups[0]).Article);
			Assert.AreEqual ("Ecrou M3", ((Record) view.Groups[1]).Article);
			Assert.AreEqual ("Rondelle", ((Record) view.Groups[2]).Article);
			Assert.AreEqual ("Tournevis", ((Record) view.Groups[3]).Article);
			Assert.AreEqual ("Tournevis", ((Record) view.Groups[4]).Article);
			Assert.AreEqual ("Vis M3", ((Record) view.Groups[5]).Article);

			Assert.AreEqual (7, ((Record) view.Groups[3]).Stock);
			Assert.AreEqual (2, ((Record) view.Groups[4]).Stock);

			view.SortDescriptions.Clear ();
			view.SortDescriptions.Add (new SortDescription ("Price"));
			view.Refresh ();

			Assert.AreEqual (6, view.Groups.Count);
			Assert.AreEqual ("Rondelle", ((Record) view.Groups[0]).Article);
			Assert.AreEqual ("Ecrou M3", ((Record) view.Groups[1]).Article);
			Assert.AreEqual ("Vis M3", ((Record) view.Groups[2]).Article);
			Assert.AreEqual ("Tournevis", ((Record) view.Groups[3]).Article);
			Assert.AreEqual ("Clé M3", ((Record) view.Groups[4]).Article);
			Assert.AreEqual ("Tournevis", ((Record) view.Groups[5]).Article);

			Assert.AreEqual (2, ((Record) view.Groups[3]).Stock);
			Assert.AreEqual (7, ((Record) view.Groups[5]).Stock);
		}

		[Test]
		public void CheckCollectionViewFilter()
		{
			List<Record> source = new List<Record> ();
			CollectionView view = new CollectionView (source);

			CollectionTest.AddRecords (source);

			view.Filter = delegate (object item)
			{
				Record record = item as Record;
				if ((record == null) ||
					(record.Price < 1))
				{
					return false;
				}
				else
				{
					return true;
				}
			};

			view.Refresh ();

			Assert.AreEqual (3, view.Groups.Count);
			Assert.AreEqual ("Clé M3", ((Record) view.Groups[0]).Article);
			Assert.AreEqual ("Tournevis", ((Record) view.Groups[1]).Article);
			Assert.AreEqual ("Tournevis", ((Record) view.Groups[2]).Article);
		}

		private static void AddRecords(List<Record> source)
		{
			source.Add (new Record ("Vis M3", 10, 0.15M));
			source.Add (new Record ("Ecrou M3", 19, 0.10M));
			source.Add (new Record ("Rondelle", 41, 0.05M));
			source.Add (new Record ("Clé M3", 7, 15.00M));
			source.Add (new Record ("Tournevis", 2, 8.45M));
			source.Add (new Record ("Tournevis", 7, 25.70M));
		}


		private class Record
		{
			public Record(string article, int stock, decimal price)
			{
				this.article = article;
				this.stock = stock;
				this.price = price;
			}
			 
			public string Article
			{
				get
				{
					return this.article;
				}
				set
				{
					this.article = value;
				}
			}

			public int Stock
			{
				get
				{
					return this.stock;
				}
				set
				{
					this.stock = value;
				}
			}

			public decimal Price
			{
				get
				{
					return this.price;
				}
				set
				{
					this.price = value;
				}
			}
			
			private string article;
			private int stock;
			private decimal price;
		}

		#region MyItem Class

		[System.ComponentModel.TypeConverter (typeof (MyItem.Converter))]
		private class MyItem : DependencyObject
		{
			public MyItem(string name)
			{
				this.Name = name;
			}

			public string Name
			{
				get
				{
					return (string) this.GetValue (MyItem.NameProperty);
				}
				set
				{
					this.SetValue (MyItem.NameProperty, value);
				}
			}

			#region Converter Class

			public class Converter : Epsitec.Common.Types.AbstractStringConverter
			{
				public override object ParseString(string value, System.Globalization.CultureInfo culture)
				{
					return new MyItem (value.Substring (7));
				}

				public override string ToString(object value, System.Globalization.CultureInfo culture)
				{
					MyItem item = value as MyItem;
					return item == null ? null : string.Concat ("MyItem:", item.Name);
				}
			}

			#endregion

			public static readonly DependencyProperty NameProperty = DependencyProperty.Register ("Name", typeof (string), typeof (MyItem));
		}

		#endregion
	}
}
