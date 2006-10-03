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
