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
			Assert.AreEqual ("MyItem=X", string.Join (":", group.GetGroupNamesForItem (item, System.Globalization.CultureInfo.InvariantCulture)));

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

			Record record = new Record ("Computer", 1, 1234.50M, Category.ElectronicEquipment);

			group.PropertyName = "Category";

			Assert.AreEqual (1, group.GetGroupNamesForItem (record, System.Globalization.CultureInfo.InvariantCulture).Length);
			Assert.AreEqual ("ElectronicEquipment", string.Join (":", group.GetGroupNamesForItem (record, System.Globalization.CultureInfo.InvariantCulture)));
		}

		[Test]
		public void CheckCollectionViewSort()
		{
			Record[] records;
			
			List<Record> source = new List<Record> ();
			CollectionView view = new CollectionView (source);

			CollectionTest.AddRecords (source);

			view.Refresh ();
			records = Collection.ToArray<Record> (view.Items);

			Assert.AreEqual (6, records.Length);
			Assert.AreEqual ("Vis M3", ((Record) records[0]).Article);
			Assert.AreEqual ("Ecrou M3", ((Record) records[1]).Article);
			Assert.AreEqual ("Rondelle", ((Record) records[2]).Article);
			Assert.AreEqual ("Clé M3", ((Record) records[3]).Article);
			Assert.AreEqual ("Tournevis", ((Record) records[4]).Article);
			Assert.AreEqual ("Tournevis", ((Record) records[5]).Article);

			view.SortDescriptions.Add (new SortDescription (ListSortDirection.Ascending, "Article"));
			view.SortDescriptions.Add (new SortDescription (ListSortDirection.Ascending, "Stock"));
			view.Refresh ();
			records = Collection.ToArray<Record> (view.Items);

			Assert.AreEqual (6, records.Length);
			Assert.AreEqual ("Clé M3", ((Record) records[0]).Article);
			Assert.AreEqual ("Ecrou M3", ((Record) records[1]).Article);
			Assert.AreEqual ("Rondelle", ((Record) records[2]).Article);
			Assert.AreEqual ("Tournevis", ((Record) records[3]).Article);
			Assert.AreEqual ("Tournevis", ((Record) records[4]).Article);
			Assert.AreEqual ("Vis M3", ((Record) records[5]).Article);

			Assert.AreEqual (2, ((Record) records[3]).Stock);
			Assert.AreEqual (7, ((Record) records[4]).Stock);

			view.SortDescriptions.RemoveAt (1);
			view.SortDescriptions.Add (new SortDescription (ListSortDirection.Descending, "Stock"));
			view.Refresh ();
			records = Collection.ToArray<Record> (view.Items);

			Assert.AreEqual (6, records.Length);
			Assert.AreEqual ("Clé M3", ((Record) records[0]).Article);
			Assert.AreEqual ("Ecrou M3", ((Record) records[1]).Article);
			Assert.AreEqual ("Rondelle", ((Record) records[2]).Article);
			Assert.AreEqual ("Tournevis", ((Record) records[3]).Article);
			Assert.AreEqual ("Tournevis", ((Record) records[4]).Article);
			Assert.AreEqual ("Vis M3", ((Record) records[5]).Article);

			Assert.AreEqual (7, ((Record) records[3]).Stock);
			Assert.AreEqual (2, ((Record) records[4]).Stock);

			view.SortDescriptions.Clear ();
			view.SortDescriptions.Add (new SortDescription ("Price"));
			view.Refresh ();
			records = Collection.ToArray<Record> (view.Items);

			Assert.AreEqual (6, records.Length);
			Assert.AreEqual ("Rondelle", ((Record) records[0]).Article);
			Assert.AreEqual ("Ecrou M3", ((Record) records[1]).Article);
			Assert.AreEqual ("Vis M3", ((Record) records[2]).Article);
			Assert.AreEqual ("Tournevis", ((Record) records[3]).Article);
			Assert.AreEqual ("Clé M3", ((Record) records[4]).Article);
			Assert.AreEqual ("Tournevis", ((Record) records[5]).Article);

			Assert.AreEqual (2, ((Record) records[3]).Stock);
			Assert.AreEqual (7, ((Record) records[5]).Stock);
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
			Record[] records = Collection.ToArray<Record> (view.Items);
			
			Assert.AreEqual (3, records.Length);
			Assert.AreEqual ("Clé M3", ((Record) records[0]).Article);
			Assert.AreEqual ("Tournevis", ((Record) records[1]).Article);
			Assert.AreEqual ("Tournevis", ((Record) records[2]).Article);
		}

		[Test]
		public void CheckCollectionViewSortAndGroup()
		{
			List<Record> source = new List<Record> ();
			CollectionView view = new CollectionView (source);

			CollectionTest.AddRecords (source);

			view.SortDescriptions.Add (new SortDescription ("Article"));
			view.SortDescriptions.Add (new SortDescription ("Price"));
			view.GroupDescriptions.Add (new PropertyGroupDescription ("Category"));
			view.Refresh ();

			Assert.AreEqual (2, view.Groups.Count);

			CollectionViewGroup group1 = view.Groups[0] as CollectionViewGroup;
			CollectionViewGroup group2 = view.Groups[1] as CollectionViewGroup;

			Assert.IsNotNull (group1);
			Assert.IsNotNull (group2);

			Assert.AreEqual ("Tool", group1.Name);
			Assert.AreEqual ("Part", group2.Name);

			Assert.IsFalse (group1.HasSubgroups);
			Assert.IsFalse (group2.HasSubgroups);
			
			Assert.AreEqual (3, group1.ItemCount);
			Assert.AreEqual (3, group2.ItemCount);

			Assert.AreEqual ("Clé M3", ((Record) group1.Items[0]).Article);
			Assert.AreEqual ("Tournevis", ((Record) group1.Items[1]).Article);
			Assert.AreEqual ("Tournevis", ((Record) group1.Items[2]).Article);
			
			Assert.AreEqual ("Ecrou M3", ((Record) group2.Items[0]).Article);
			Assert.AreEqual ("Rondelle", ((Record) group2.Items[1]).Article);
			Assert.AreEqual ("Vis M3", ((Record) group2.Items[2]).Article);

			view.GroupDescriptions.Add (new PropertyGroupDescription ("Article"));
			view.Refresh ();

			Assert.AreEqual (2, view.Groups.Count);

			group1 = view.Groups[0] as CollectionViewGroup;
			group2 = view.Groups[1] as CollectionViewGroup;

			Assert.IsNotNull (group1);
			Assert.IsNotNull (group2);

			Assert.AreEqual ("Tool", group1.Name);
			Assert.AreEqual ("Part", group2.Name);

			Assert.IsTrue (group1.HasSubgroups);
			Assert.IsTrue (group2.HasSubgroups);

			Assert.AreEqual (2, group1.Subgroups.Count);
			Assert.AreEqual (3, group2.Subgroups.Count);

			CollectionViewGroup subgroup1 = group1.Subgroups[0];
			CollectionViewGroup subgroup2 = group1.Subgroups[1];

			Assert.AreEqual ("Clé M3", subgroup1.Name);
			Assert.AreEqual ("Tournevis", subgroup2.Name);
		}

		private static void AddRecords(List<Record> source)
		{
			source.Add (new Record ("Vis M3", 10, 0.15M, Category.Part));
			source.Add (new Record ("Ecrou M3", 19, 0.10M, Category.Part));
			source.Add (new Record ("Rondelle", 41, 0.05M, Category.Part));
			source.Add (new Record ("Clé M3", 7, 15.00M, Category.Tool));
			source.Add (new Record ("Tournevis", 2, 8.45M, Category.Tool));
			source.Add (new Record ("Tournevis", 7, 25.70M, Category.Tool));
		}

		private enum Category
		{
			Unknown,
			Part,
			Tool,
			ElectronicEquipment
		}

		private class Record
		{
			public Record(string article, int stock, decimal price, Category category)
			{
				this.article = article;
				this.stock = stock;
				this.price = price;
				this.category = category;
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

			public Category Category
			{
				get
				{
					return this.category;
				}
				set
				{
					this.category = value;
				}
			}
			
			private string article;
			private int stock;
			private decimal price;
			private Category category;
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
					return item == null ? null : string.Concat ("MyItem=", item.Name);
				}
			}

			#endregion

			public static readonly DependencyProperty NameProperty = DependencyProperty.Register ("Name", typeof (string), typeof (MyItem));
		}

		#endregion
	}
}
