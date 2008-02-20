//	Copyright � 2006-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Common.Types.Collections;

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
			Assert.AreEqual (0, view.Items.Count);

			CollectionTest.AddRecords (source1);

			Assert.AreEqual (0, view.Count);
			Assert.AreEqual (0, view.Items.Count);

			view.Refresh ();

			Assert.AreEqual (6, view.Count);
			Assert.AreEqual (6, view.Items.Count);

			source2 = new Collections.ObservableList<Record> ();
			view    = new CollectionView (source2);

			Assert.AreEqual (0, view.Count);
			Assert.AreEqual (0, view.Items.Count);

			CollectionTest.AddRecords (source2);

			Assert.AreEqual (6, view.Count);
			Assert.AreEqual (6, view.Items.Count);

			source2 = new Collections.ObservableList<Record> ();
			view    = new CollectionView (source2);
			
			Assert.AreEqual (0, view.Count);
			Assert.AreEqual (0, view.Items.Count);

			using (view.DeferRefresh ())
			{
				CollectionTest.AddRecords (source2);

				Assert.AreEqual (0, view.Count);
				Assert.AreEqual (0, view.Items.Count);
			}
			
			Assert.AreEqual (6, view.Count);
			Assert.AreEqual (6, view.Items.Count);
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
			Assert.AreEqual ("Cl� M3", records[0].Article);
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
			view.Refresh ();

			records = Collection.ToArray<Record> (view.Items);

			Assert.AreEqual (6, records.Length);
			Assert.AreEqual ("Vis M3", records[0].Article);
			Assert.AreEqual ("Ecrou M3", records[1].Article);
			Assert.AreEqual ("Rondelle", records[2].Article);
			Assert.AreEqual ("Cl� M3", records[3].Article);
			Assert.AreEqual ("Tournevis", records[4].Article);
			Assert.AreEqual ("Tournevis", records[5].Article);

			view.SortDescriptions.Add (new SortDescription (ListSortDirection.Ascending, "Article"));
			view.SortDescriptions.Add (new SortDescription (ListSortDirection.Ascending, "Stock"));
			
			records = Collection.ToArray<Record> (view.Items);

			Assert.AreEqual (6, records.Length);
			Assert.AreEqual ("Cl� M3", records[0].Article);
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
			Assert.AreEqual ("Cl� M3", records[0].Article);
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
			Assert.AreEqual ("Cl� M3", records[4].Article);
			Assert.AreEqual ("Tournevis", records[5].Article);

			Assert.AreEqual (2, records[3].Stock);
			Assert.AreEqual (7, records[5].Stock);
		}

		[Test]
		public void CheckCollectionViewSortAndGroup()
		{
			Record[] records;
			IList<CollectionViewGroup> path;

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

			Assert.AreEqual ("Cl� M3",    ((Record) view.Items[0]).Article);
			Assert.AreEqual ("Ecrou M3",  ((Record) view.Items[1]).Article);
			Assert.AreEqual ("Rondelle",  ((Record) view.Items[2]).Article);
			Assert.AreEqual ("Tournevis", ((Record) view.Items[3]).Article);
			Assert.AreEqual ("Tournevis", ((Record) view.Items[4]).Article);
			Assert.AreEqual ("Vis M3",    ((Record) view.Items[5]).Article);

			path = CollectionView.GetGroupPath (view, source[0]);

			Assert.AreEqual (0, path.Count);

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

			Assert.AreEqual ("Cl� M3", records[0].Article);
			Assert.AreEqual ("Tournevis", records[1].Article);
			Assert.AreEqual ("Tournevis", records[2].Article);

			records = Collection.ToArray<Record> (group2.Items);

			Assert.AreEqual ("Ecrou M3", records[0].Article);
			Assert.AreEqual ("Rondelle", records[1].Article);
			Assert.AreEqual ("Vis M3", records[2].Article);
			
			path = CollectionView.GetGroupPath (view, source[0]);

			Assert.AreEqual (1, path.Count);
			Assert.AreEqual ("Part", path[0].Name);

			path = CollectionView.GetGroupPath (view, source[5]);

			Assert.AreEqual (1, path.Count);
			Assert.AreEqual ("Tool", path[0].Name);

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

			Assert.AreEqual ("Cl� M3", subgroup1.Name);
			Assert.AreEqual ("Tournevis", subgroup2.Name);

			path = CollectionView.GetGroupPath (view, source[0]);

			Assert.AreEqual (2, path.Count);
			Assert.AreEqual ("Part", path[0].Name);
			Assert.AreEqual ("Vis M3", path[1].Name);

			path = CollectionView.GetGroupPath (view, source[5]);

			Assert.AreEqual (2, path.Count);
			Assert.AreEqual ("Tool", path[0].Name);
			Assert.AreEqual ("Tournevis", path[1].Name);
			
			path = CollectionView.GetGroupPath (view, new StructuredData ());

			Assert.IsNull (path);
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

			Assert.IsNull (resolver.GetCollectionView (binding1, list1, false));
			Assert.IsNull (resolver.GetCollectionView (binding1, list2, false));
			
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
			list.Add (new Article ("Cl� M4"));

			data.SetValue ("Articles", list);
			data.SetValue ("InvoiceId", "abc");

			UserInterface ui = new UserInterface ();
			Binding context = new Binding (data);

			ui.SetBinding (UserInterface.InvoiceIdProperty, new Binding (BindingMode.TwoWay, "InvoiceId"));
			ui.SetBinding (UserInterface.ArticleProperty, new Binding (BindingMode.OneWay, "Articles"));
			ui.SetBinding (UserInterface.ArticleNameProperty, new Binding (BindingMode.TwoWay, "Articles.Name"));

			//	No DataContext defined for the UserInterface object, so the binding
			//	won't have any effect yet:

			Assert.IsNull (ui.InvoiceId);
			Assert.IsNull (ui.Article);
			Assert.IsNull (ui.ArticleName);

			//	Attach a DataContext to the UserInterface object; the bindings become
			//	active:

			DataObject.SetDataContext (ui, context);

			Assert.AreEqual (data.GetValue ("InvoiceId"), ui.InvoiceId);

			ICollectionView cv = DataObject.GetCollectionView (ui, list);

			Assert.IsNotNull (cv);
			Assert.IsNotNull (cv.CurrentItem);

			Assert.AreEqual ("Vis M3", ((Article) cv.CurrentItem).Name);
			Assert.AreEqual (cv.CurrentItem, ui.Article);
			Assert.AreEqual ("Vis M3", ui.ArticleName);

			cv.MoveCurrentToNext ();

			Assert.AreEqual ("Vis M4", ((Article) cv.CurrentItem).Name);
			Assert.AreEqual (cv.CurrentItem, ui.Article);
			Assert.AreEqual ("Vis M4", ui.ArticleName);

			ui.ArticleName = "Vis M4-X";

			Assert.AreEqual ("Vis M4-X", ui.ArticleName);
			Assert.AreEqual ("Vis M4-X", ((Article) cv.CurrentItem).Name);
		}

		[Test]
		public void CheckCollectionBinding2()
		{
			StructuredData data = new StructuredData ();
			ArticleList list = new ArticleList ();

			list.Add (new Article ("Vis M3"));
			list.Add (new Article ("Vis M4"));
			list.Add (new Article ("Boulon M3"));
			list.Add (new Article ("Tournevis"));
			list.Add (new Article ("Cl� M4"));

			data.SetValue ("Articles", list);
			data.SetValue ("InvoiceId", "abc");

			UserInterface ui = new UserInterface ();
			Binding context = new Binding (data);

			ui.SetBinding (UserInterface.ArticlesProperty, new Binding (BindingMode.OneWay, "Articles"));
			
			DataObject.SetDataContext (ui, context);

			ICollectionView cv = Internal.CollectionViewResolver.Default.GetCollectionView (context, list);

			Assert.IsNotNull (cv);
			Assert.AreEqual (cv, ui.GetValue (UserInterface.ArticlesProperty));
		}

		[Test]
		public void CheckCollectionBinding3()
		{
			StructuredData data = new StructuredData ();
			ArticleList list = new ArticleList ();

			list.Add (new Article ("Vis M3"));
			list.Add (new Article ("Vis M4"));
			list.Add (new Article ("Boulon M3"));
			list.Add (new Article ("Tournevis"));
			list.Add (new Article ("Cl� M4"));

			data.SetValue ("Articles", list);
			data.SetValue ("InvoiceId", "abc");

			UserInterface ui = new UserInterface ();
			Binding context = new Binding (data);

			ui.SetBinding (UserInterface.InvoiceIdProperty, new Binding (BindingMode.TwoWay, "InvoiceId"));
			ui.SetBinding (UserInterface.ArticleProperty, new Binding (BindingMode.OneWay, "Articles"));
			ui.SetBinding (UserInterface.ArticleNameProperty, new Binding (BindingMode.TwoWay, "*.Articles.*.Name"));

			//	No DataContext defined for the UserInterface object, so the binding
			//	won't have any effect yet:

			Assert.IsNull (ui.InvoiceId);
			Assert.IsNull (ui.Article);
			Assert.IsNull (ui.ArticleName);

			//	Attach a DataContext to the UserInterface object; the bindings become
			//	active:

			DataObject.SetDataContext (ui, context);

			Assert.AreEqual (data.GetValue ("InvoiceId"), ui.InvoiceId);

			ICollectionView cv = DataObject.GetCollectionView (ui, list);

			Assert.IsNotNull (cv);
			Assert.IsNotNull (cv.CurrentItem);

			Assert.AreEqual ("Vis M3", ((Article) cv.CurrentItem).Name);
			Assert.AreEqual (cv.CurrentItem, ui.Article);
			Assert.AreEqual ("Vis M3", ui.ArticleName);

			cv.MoveCurrentToNext ();

			Assert.AreEqual ("Vis M4", ((Article) cv.CurrentItem).Name);
			Assert.AreEqual (cv.CurrentItem, ui.Article);
			Assert.AreEqual ("Vis M4", ui.ArticleName);

			ui.ArticleName = "Vis M4-X";

			Assert.AreEqual ("Vis M4-X", ui.ArticleName);
			Assert.AreEqual ("Vis M4-X", ((Article) cv.CurrentItem).Name);
		}

		[Test]
		public void CheckCollectionType()
		{
			CollectionType type = new CollectionType ();

			type.DefineItemType (IntegerType.Default);

			string[] stringArray = new string[] { "a", "b" };
			int[]    intArray    = new int[] { 1, 2, 3 };

			Assert.IsFalse (type.IsValidValue (null));
			Assert.IsFalse (type.IsValidValue ("xyz"));
			Assert.IsFalse (type.IsValidValue (stringArray));

			Assert.IsTrue (type.IsValidValue (intArray));
		}

		[Test]
		[Ignore ("Implementation of IsValidValue validation based on generic IEnumerable<> type is missing")]
		public void CheckCollectionTypeWithEmptyEnumeration()
		{
			CollectionType type = new CollectionType ();

			type.DefineItemType (IntegerType.Default);
			
			Assert.IsFalse (type.IsValidValue (new string[0]));
		}


		[Test]
		public void CheckObservableListCopyOnWrite()
		{
			CowList<int> original = new CowList<int> ();
			ObservableList<int> copy = original.GetCopy ();

			Assert.IsNull (copy);

			original.AddRange (new int[] { 1, 2, 3 });

			Assert.IsFalse (original.HasCopy);

			original.CollectionChanged += delegate { Assert.Fail (); };
			original.CollectionChanging += delegate { Assert.Fail (); };

			original.Lock ();
			original.Add (4);

			copy = original.GetCopy ();

			Assert.IsTrue (original.HasCopy);
			Assert.IsTrue (Collection.CompareEqual (new int[] { 1, 2, 3 }, original));
			Assert.IsTrue (Collection.CompareEqual (new int[] { 1, 2, 3, 4 }, copy));

			int changeCount = 0;

			copy.CollectionChanging += delegate { changeCount++; };

			original.Remove (2);
			original.Insert (0, 8);

			Assert.AreEqual (2, changeCount);
			
			Assert.IsTrue (Collection.CompareEqual (new int[] { 1, 2, 3 }, original));
			Assert.IsTrue (Collection.CompareEqual (new int[] { 8, 1, 3, 4 }, copy));

			original.Sort (
				delegate (int a, int b)
				{
					if (a < b)
					{
						return -1;
					}
					else if (a > b)
					{
						return 1;
					}
					else
					{
						return 0;
					}
				});

			Assert.IsTrue (Collection.CompareEqual (new int[] { 1, 2, 3 }, original));
			Assert.IsTrue (Collection.CompareEqual (new int[] { 1, 3, 4, 8 }, copy));
		}

		[Test]
		public void CheckObservableListNotifyBeforeChange()
		{
			ObservableList<int> list = new ObservableList<int> ();

			int changeCount = 0;

			list.CollectionChanging +=
				delegate
				{
					changeCount++;
				};

			list.Add (1);
			Assert.AreEqual (1, changeCount);
			
			list.AddRange (new int[] { 2, 3, 4 });
			Assert.AreEqual (2, changeCount);

			list.Remove (4);
			Assert.AreEqual (3, changeCount);
			list.Remove (4);
			Assert.AreEqual (3, changeCount);

			list.RemoveAt (0);
			Assert.AreEqual (4, changeCount);

			list.Clear ();
			Assert.AreEqual (5, changeCount);
			list.Clear ();
			Assert.AreEqual (5, changeCount);

			list.Insert (0, -1);
			list.Insert (0, -2);
			list.Insert (0, -3);
			
			Assert.AreEqual (8, changeCount);

			int[] numbers = new int[] { 5, 1, 2, 3, 4 };

			list.ReplaceWithRange (numbers);
			Assert.AreEqual (9, changeCount);

			Assert.IsTrue (Collection.CompareEqual (numbers, list));

			list.Sort (
				delegate (int a, int b)
				{
					if (a < b)
					{
						return -1;
					}
					else if (a > b)
					{
						return 1;
					}
					else
					{
						return 0;
					}
				});

			Assert.AreEqual (10, changeCount);
			Assert.IsTrue (Collection.CompareEqual (new int[] { 1, 2, 3, 4, 5 }, list));
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
			source.Add (new Record ("Cl� M3", 7, 15.00M, Category.Tool));
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

			public string ArticleName
			{
				get
				{
					return (string) this.GetValue (UserInterface.ArticleNameProperty);
				}
				set
				{
					this.SetValue (UserInterface.ArticleNameProperty, value);
				}
			}

			public static readonly DependencyProperty InvoiceIdProperty		= DependencyProperty.Register ("InvoiceId", typeof (string), typeof (UserInterface));
			public static readonly DependencyProperty ArticleProperty		= DependencyProperty.Register ("Article", typeof (Article), typeof (UserInterface));
			public static readonly DependencyProperty ArticleNameProperty	= DependencyProperty.Register ("ArticleName", typeof (string), typeof (UserInterface));
			public static readonly DependencyProperty ArticlesProperty		= DependencyProperty.Register ("Articles", typeof (ICollectionView), typeof (UserInterface));
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

		private class CowList<T> : ObservableList<T>
		{
			public CowList()
			{
			}

			public bool HasCopy
			{
				get
				{
					return this.copy != null;
				}
			}

			public ObservableList<T> GetCopy()
			{
				return this.copy;
			}

			protected override ObservableList<T> GetWorkingList()
			{
				if (this.IsReadOnly)
				{
					if (this.copy == null)
					{
						this.copy = new ObservableList<T> ();
						this.copy.AddRange (this);
					}

					return this.copy;
				}
				else
				{
					return base.GetWorkingList ();
				}
			}

			private ObservableList<T> copy;
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
