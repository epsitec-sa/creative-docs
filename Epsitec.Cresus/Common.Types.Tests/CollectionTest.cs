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
		public void CheckCollectionViewAutoUpdate()
		{
			List<Record> source1;
			Collections.ObservableList<Record> source2;

			CollectionView view;

			source1 = new List<Record> ();
			view    = new CollectionView (source1);

			Assert.AreEqual (0, view.Count);

			CollectionTest.AddRecords (source1);

			Assert.AreEqual (0, view.Count);
			Assert.AreEqual (6, view.Items.Count);

			source2 = new Collections.ObservableList<Record> ();
			view    = new CollectionView (source2);

			Assert.AreEqual (0, view.Count);

			CollectionTest.AddRecords (source2);

			Assert.AreEqual (6, view.Count);
			Assert.AreEqual (6, view.Items.Count);

			source2 = new Collections.ObservableList<Record> ();
			view    = new CollectionView (source2);
			
			Assert.AreEqual (0, view.Count);

			using (view.DeferRefresh ())
			{
				CollectionTest.AddRecords (source2);

				Assert.AreEqual (0, view.Count);
				Assert.AreEqual (6, view.Items.Count);
			}
			
			Assert.AreEqual (6, view.Count);
		}

		[Test]
		public void CheckCollectionViewCurrentItem()
		{
			List<Record> source = new List<Record> ();
			CollectionView view = new CollectionView (source);

			Assert.IsNull (view.CurrentItem);
			Assert.AreEqual (-1, view.CurrentPosition);
			Assert.IsTrue (view.IsCurrentBeforeFirst);
			Assert.IsFalse (view.IsCurrentAfterLast);

			CollectionTest.AddRecords (source);

			Assert.IsNull (view.CurrentItem);
			Assert.AreEqual (-1, view.CurrentPosition);
			Assert.IsTrue (view.IsCurrentBeforeFirst);
			Assert.IsFalse (view.IsCurrentAfterLast);

			view.Refresh ();

			Assert.IsNull (view.CurrentItem);
			Assert.AreEqual (-1, view.CurrentPosition);
			Assert.IsTrue (view.IsCurrentBeforeFirst);
			Assert.IsFalse (view.IsCurrentAfterLast);

			view = new CollectionView (source);

			Assert.AreEqual (view.Items[0], view.CurrentItem);
			Assert.AreEqual (0, view.CurrentPosition);
			Assert.IsFalse (view.IsCurrentBeforeFirst);
			Assert.IsFalse (view.IsCurrentAfterLast);

			Assert.IsFalse (view.MoveCurrentToPrevious ());
			Assert.AreEqual (-1, view.CurrentPosition);
			Assert.IsFalse (view.MoveCurrentToPrevious ());
			Assert.AreEqual (-1, view.CurrentPosition);
			Assert.IsTrue (view.MoveCurrentToNext ());
			Assert.AreEqual (0, view.CurrentPosition);
			Assert.IsTrue (view.MoveCurrentToPosition (5));
			Assert.AreEqual (5, view.CurrentPosition);
			Assert.IsFalse (view.MoveCurrentToPosition (6));
			Assert.AreEqual (6, view.CurrentPosition);
			Assert.IsFalse (view.MoveCurrentToNext ());
			Assert.AreEqual (6, view.CurrentPosition);
			Assert.IsTrue (view.MoveCurrentToPrevious ());
			Assert.AreEqual (5, view.CurrentPosition);
			Assert.IsTrue (view.MoveCurrentToPrevious ());
			Assert.AreEqual (4, view.CurrentPosition);
			Assert.IsTrue (view.MoveCurrentToNext ());
			Assert.AreEqual (5, view.CurrentPosition);

			Assert.AreEqual (view.Items[5], view.CurrentItem);

			Assert.IsTrue (view.MoveCurrentTo (view.Items[3]));
			Assert.AreEqual (3, view.CurrentPosition);

			source.Add (new Record ("Mouse", 8, 45.0M, Category.ElectronicEquipment));
			view.Refresh ();

			Assert.AreEqual (7, view.Count);
			Assert.AreEqual (view.Items[3], view.CurrentItem);
			Assert.AreEqual (3, view.CurrentPosition);
			
			source.Insert (2, new Record ("Keyboard", 12, 19.0M, Category.ElectronicEquipment));
			view.Refresh ();

			Assert.AreEqual (8, view.Count);
			Assert.AreEqual (view.Items[4], view.CurrentItem);
			Assert.AreEqual (4, view.CurrentPosition);

			source.RemoveAt (2);
			view.Refresh ();

			Assert.AreEqual (7, view.Count);
			Assert.AreEqual (view.Items[3], view.CurrentItem);
			Assert.AreEqual (3, view.CurrentPosition);

			//	Removing the current item moves the current item to the
			//	next available item in the view...
			
			source.RemoveAt (3);
			view.Refresh ();

			Assert.AreEqual (6, view.Count);
			Assert.AreEqual (view.Items[3], view.CurrentItem);
			Assert.AreEqual (3, view.CurrentPosition);

			Assert.IsTrue (view.MoveCurrentToLast ());
			Assert.IsFalse (view.MoveCurrentToNext ());

			source.RemoveAt (3);
			view.Refresh ();
			
			Assert.AreEqual (5, view.Count);
			Assert.IsNull (view.CurrentItem);
			Assert.AreEqual (5, view.CurrentPosition);

			Assert.IsTrue (view.MoveCurrentToPosition (3));
			Assert.AreEqual (view.Items[3], view.CurrentItem);
			Assert.AreEqual (3, view.CurrentPosition);

			source.RemoveAt (3);
			view.Refresh ();

			Assert.AreEqual (4, view.Count);
			Assert.AreEqual (view.Items[3], view.CurrentItem);
			Assert.AreEqual (3, view.CurrentPosition);

			source.RemoveAt (3);
			view.Refresh ();

			Assert.AreEqual (3, view.Count);
			Assert.AreEqual (view.Items[2], view.CurrentItem);
			Assert.AreEqual (2, view.CurrentPosition);

			Assert.IsTrue (view.MoveCurrentToPosition (0));
			Assert.AreEqual (view.Items[0], view.CurrentItem);
			Assert.AreEqual (0, view.CurrentPosition);

			source.RemoveAt (1);
			view.Refresh ();

			Assert.AreEqual (2, view.Count);
			Assert.AreEqual (view.Items[0], view.CurrentItem);
			Assert.AreEqual (0, view.CurrentPosition);

			source.RemoveAt (0);
			view.Refresh ();

			Assert.AreEqual (1, view.Count);
			Assert.AreEqual (view.Items[0], view.CurrentItem);
			Assert.AreEqual (0, view.CurrentPosition);

			source.RemoveAt (0);
			view.Refresh ();

			Assert.AreEqual (0, view.Count);
			Assert.IsNull (view.CurrentItem);
			Assert.AreEqual (-1, view.CurrentPosition);
			Assert.IsTrue (view.IsCurrentBeforeFirst);
			Assert.IsFalse (view.IsCurrentAfterLast);
		}

		[Test]
		[ExpectedException (typeof (System.ArgumentOutOfRangeException))]
		public void CheckCollectionViewCurrentItemEx1()
		{
			List<Record> source = new List<Record> ();

			CollectionTest.AddRecords (source);

			CollectionView view = new CollectionView (source);

			Assert.AreEqual (view.Items[0], view.CurrentItem);
			Assert.AreEqual (0, view.CurrentPosition);
			Assert.IsFalse (view.IsCurrentBeforeFirst);
			Assert.IsFalse (view.IsCurrentAfterLast);

			Assert.IsFalse (view.MoveCurrentToPosition (7));
		}
		
		[Test]
		[ExpectedException (typeof (System.ArgumentOutOfRangeException))]
		public void CheckCollectionViewCurrentItemEx2()
		{
			List<Record> source = new List<Record> ();

			CollectionTest.AddRecords (source);

			CollectionView view = new CollectionView (source);

			Assert.AreEqual (view.Items[0], view.CurrentItem);
			Assert.AreEqual (0, view.CurrentPosition);
			Assert.IsFalse (view.IsCurrentBeforeFirst);
			Assert.IsFalse (view.IsCurrentAfterLast);

			Assert.IsFalse (view.MoveCurrentToPosition (-2));
		}

		[Test]
		public void CheckCollectionViewFilter()
		{
			List<Record> source = new List<Record> ();
			CollectionView view = new CollectionView (source);

			CollectionTest.AddRecords (source);

			Assert.AreEqual (0, view.Count);
			view.Refresh ();
			Assert.AreEqual (6, view.Count);

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

			Record[] records = Collection.ToArray<Record> (view.Items);

			Assert.AreEqual (3, records.Length);
			Assert.AreEqual ("Clé M3", records[0].Article);
			Assert.AreEqual ("Tournevis", records[1].Article);
			Assert.AreEqual ("Tournevis", records[2].Article);
		}

		[Test]
		public void CheckCollectionViewSort()
		{
			Record[] records;
			
			List<Record> source = new List<Record> ();
			CollectionView view = new CollectionView (source);

			CollectionTest.AddRecords (source);

			records = Collection.ToArray<Record> (view.Items);

			Assert.AreEqual (6, records.Length);
			Assert.AreEqual ("Vis M3", records[0].Article);
			Assert.AreEqual ("Ecrou M3", records[1].Article);
			Assert.AreEqual ("Rondelle", records[2].Article);
			Assert.AreEqual ("Clé M3", records[3].Article);
			Assert.AreEqual ("Tournevis", records[4].Article);
			Assert.AreEqual ("Tournevis", records[5].Article);

			view.SortDescriptions.Add (new SortDescription (ListSortDirection.Ascending, "Article"));
			view.SortDescriptions.Add (new SortDescription (ListSortDirection.Ascending, "Stock"));
			
			records = Collection.ToArray<Record> (view.Items);

			Assert.AreEqual (6, records.Length);
			Assert.AreEqual ("Clé M3", records[0].Article);
			Assert.AreEqual ("Ecrou M3", records[1].Article);
			Assert.AreEqual ("Rondelle", records[2].Article);
			Assert.AreEqual ("Tournevis", records[3].Article);
			Assert.AreEqual ("Tournevis", records[4].Article);
			Assert.AreEqual ("Vis M3", records[5].Article);

			Assert.AreEqual (2, records[3].Stock);
			Assert.AreEqual (7, records[4].Stock);

			view.SortDescriptions.RemoveAt (1);
			view.SortDescriptions.Add (new SortDescription (ListSortDirection.Descending, "Stock"));
			
			records = Collection.ToArray<Record> (view.Items);

			Assert.AreEqual (6, records.Length);
			Assert.AreEqual ("Clé M3", records[0].Article);
			Assert.AreEqual ("Ecrou M3", records[1].Article);
			Assert.AreEqual ("Rondelle", records[2].Article);
			Assert.AreEqual ("Tournevis", records[3].Article);
			Assert.AreEqual ("Tournevis", records[4].Article);
			Assert.AreEqual ("Vis M3", records[5].Article);

			Assert.AreEqual (7, records[3].Stock);
			Assert.AreEqual (2, records[4].Stock);

			view.SortDescriptions.Clear ();
			view.SortDescriptions.Add (new SortDescription ("Price"));
			
			records = Collection.ToArray<Record> (view.Items);

			Assert.AreEqual (6, records.Length);
			Assert.AreEqual ("Rondelle", records[0].Article);
			Assert.AreEqual ("Ecrou M3", records[1].Article);
			Assert.AreEqual ("Vis M3", records[2].Article);
			Assert.AreEqual ("Tournevis", records[3].Article);
			Assert.AreEqual ("Clé M3", records[4].Article);
			Assert.AreEqual ("Tournevis", records[5].Article);

			Assert.AreEqual (2, records[3].Stock);
			Assert.AreEqual (7, records[5].Stock);
		}

		[Test]
		public void CheckCollectionViewSortAndGroup()
		{
			Record[] records;

			List<Record> source = new List<Record> ();
			CollectionView view = new CollectionView (source);

			CollectionTest.AddRecords (source);

			view.SortDescriptions.Add (new SortDescription ("Article"));
			view.SortDescriptions.Add (new SortDescription ("Price"));

			Assert.IsNotNull (view.Groups);
			Assert.AreEqual (0, view.Groups.Count);
			Assert.AreEqual (6, view.Items.Count);

			records = new Record[8];
			view.Items.CopyTo (records, 2);

			Assert.AreEqual (records[2], ((Record) view.Items[0]));
			Assert.AreEqual (records[7], ((Record) view.Items[5]));
			Assert.IsTrue (view.Items.Contains (records[4]));
			Assert.AreEqual (2, view.Items.IndexOf (records[4]));
			Assert.AreEqual (-1, view.Items.IndexOf (new Record ("*", 1, 1, Category.Unknown)));

			Assert.AreEqual ("Clé M3",    ((Record) view.Items[0]).Article);
			Assert.AreEqual ("Ecrou M3",  ((Record) view.Items[1]).Article);
			Assert.AreEqual ("Rondelle",  ((Record) view.Items[2]).Article);
			Assert.AreEqual ("Tournevis", ((Record) view.Items[3]).Article);
			Assert.AreEqual ("Tournevis", ((Record) view.Items[4]).Article);
			Assert.AreEqual ("Vis M3",    ((Record) view.Items[5]).Article);
			
			view.GroupDescriptions.Add (new PropertyGroupDescription ("Category"));
			
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

			records = Collection.ToArray<Record> (group1.Items);

			Assert.AreEqual ("Clé M3", records[0].Article);
			Assert.AreEqual ("Tournevis", records[1].Article);
			Assert.AreEqual ("Tournevis", records[2].Article);

			records = Collection.ToArray<Record> (group2.Items);

			Assert.AreEqual ("Ecrou M3", records[0].Article);
			Assert.AreEqual ("Rondelle", records[1].Article);
			Assert.AreEqual ("Vis M3", records[2].Article);

			view.GroupDescriptions.Add (new PropertyGroupDescription ("Article"));
			
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

		[Test]
		public void CheckInternalCollectionViewResolver()
		{
			Internal.CollectionViewResolver resolver = Internal.CollectionViewResolver.Default;

			Binding binding1 = new Binding ();
			Binding binding2 = new Binding ();

			Assert.IsNull (resolver.GetCollectionView (binding1, null));

			List<DependencyObject> list1 = new List<DependencyObject> ();
			Collections.ObservableList<DependencyObject> list2 = new Collections.ObservableList<DependencyObject> ();

			ICollectionView cv1 = resolver.GetCollectionView (binding1, list1);
			ICollectionView cv2 = resolver.GetCollectionView (binding1, list2);
			
			Assert.AreSame (cv1, resolver.GetCollectionView (binding1, list1));
			Assert.AreSame (cv2, resolver.GetCollectionView (binding1, list2));
			
			Assert.AreNotSame (cv1, resolver.GetCollectionView (binding2, list1));
			Assert.AreNotSame (cv2, resolver.GetCollectionView (binding2, list2));
		}

		[Test]
		public void CheckCollectionBinding1()
		{
			StructuredData data = new StructuredData ();
			ArticleList list = new ArticleList ();

			list.Add (new Article ("Vis M3"));
			list.Add (new Article ("Vis M4"));
			list.Add (new Article ("Boulon M3"));
			list.Add (new Article ("Tournevis"));
			list.Add (new Article ("Clé M4"));
			
			data.SetValue ("Articles", list);
			data.SetValue ("InvoiceId", "abc");

			UserInterface ui = new UserInterface ();
			Binding  context = new Binding (data);

			ui.SetBinding (UserInterface.InvoiceIdProperty, new Binding (BindingMode.TwoWay, "InvoiceId"));
			ui.SetBinding (UserInterface.ArticleProperty, new Binding (BindingMode.OneWay, "Articles"));

			//	No DataContext defined for the UserInterface object, so the binding
			//	won't have any effect yet:
			
			Assert.IsNull (ui.InvoiceId);
			
			//	Attach a DataContext to the UserInterface object; the bindings become
			//	active:
			
			DataObject.SetDataContext (ui, context);

			Assert.AreEqual (data.GetValue ("InvoiceId"), ui.InvoiceId);

			ICollectionView cv = Internal.CollectionViewResolver.Default.GetCollectionView (context, list);

			Assert.IsNotNull (cv);
			Assert.IsNotNull (cv.CurrentItem);

			Assert.AreEqual (cv.CurrentItem, ui.Article);
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

		private static void AddRecords(IList<Record> source)
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

		private class UserInterface : DependencyObject
		{
			public string InvoiceId
			{
				get
				{
					return (string) this.GetValue (UserInterface.InvoiceIdProperty);
				}
				set
				{
					this.SetValue (UserInterface.InvoiceIdProperty, value);
				}
			}

			public Article Article
			{
				get
				{
					return (Article) this.GetValue (UserInterface.ArticleProperty);
				}
				set
				{
					this.SetValue (UserInterface.ArticleProperty, value);
				}
			}

			public static readonly DependencyProperty InvoiceIdProperty = DependencyProperty.Register ("InvoiceId", typeof (string), typeof (UserInterface));
			public static readonly DependencyProperty ArticleProperty = DependencyProperty.Register ("Article", typeof (Article), typeof (UserInterface));
		}

		private class ArticleList : Collections.ObservableList<Article>
		{
		}
		
		private class Article : DependencyObject
		{
			public Article(string name)
			{
				this.Name = name;
			}
			
			public string Name
			{
				get
				{
					return (string) this.GetValue (Article.NameProperty);
				}
				set
				{
					this.SetValue (Article.NameProperty, value);
				}
			}


			public static readonly DependencyProperty NameProperty = DependencyProperty.Register ("Name", typeof (string), typeof (Article));
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
