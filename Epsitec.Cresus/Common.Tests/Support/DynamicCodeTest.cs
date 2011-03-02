//	Copyright © 2006-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

using NUnit.Framework;
using Epsitec.Common.Support;

namespace Epsitec.Common.Tests.Support
{
	[TestFixture]
	public class DynamicCodeTest
	{
		[Test]
		public void CheckAllocators()
		{
			Allocator<Dummy> allocator1 = DynamicCodeFactory.CreateAllocator<Dummy> ();
			Allocator<Dummy, string> allocator2 = DynamicCodeFactory.CreateAllocator<Dummy, string> ();

			Dummy instance;
			
			instance = allocator1 ();

			Assert.IsNotNull (instance);
			Assert.AreEqual (typeof (Dummy), instance.GetType ());

			instance = allocator2 ("Bill");

			Assert.IsNotNull (instance);
			Assert.AreEqual (typeof (Dummy), instance.GetType ());
			Assert.AreEqual ("Bill", instance.Name);
		}

		[Test]
		public void CheckPropertyComparerForClassProperties()
		{
			PropertyComparer nameComparer = DynamicCodeFactory.CreatePropertyComparer (typeof (Dummy), "Name");

			Dummy d1 = new Dummy ("Abc");
			Dummy d2 = new Dummy ("Xyz");
			Dummy d3 = new Dummy ("Abc");
			Dummy d4 = new Dummy (null);

			Assert.AreEqual (-1, nameComparer (d1, d2));
			Assert.AreEqual (1, nameComparer (d2, d1));
			Assert.AreEqual (0, nameComparer (d1, d3));
			Assert.AreEqual (-1, nameComparer (d4, d1));
			Assert.AreEqual (1, nameComparer (d1, d4));
			Assert.AreEqual (0, nameComparer (d4, d4));

			System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch ();

			watch.Reset ();
			watch.Start ();

			const int count = 1000000;
			int n = 0;

			for (int i = 0; i < count; i++)
			{
				n += nameComparer (d2, d1);
			}

			watch.Stop ();
			Assert.AreEqual (n, count);

			System.Console.Out.WriteLine ("{0} dynamic property CompareTo take {1} ms", count, watch.ElapsedMilliseconds);

			{
				System.IComparable<string> comparable = d2.Name;
				n += comparable.CompareTo (d1.Name);
			}

			n = 0;
			watch.Reset ();
			watch.Start ();

			for (int i = 0; i < count; i++)
			{
				System.IComparable<string> comparable = d2.Name;
				n += comparable.CompareTo (d1.Name);
			}

			watch.Stop ();
			Assert.AreEqual (n, count);

			System.Console.Out.WriteLine ("{0} optimal property CompareTo take {1} ms", count, watch.ElapsedMilliseconds);

			n = 0;
			watch.Reset ();
			watch.Start ();

			System.Collections.Comparer comp = System.Collections.Comparer.Default;

			for (int i = 0; i < count; i++)
			{
				n += comp.Compare (d2.Name, d1.Name);
			}

			watch.Stop ();
			Assert.AreEqual (n, count);

			System.Console.Out.WriteLine ("{0} Comparer-based property CompareTo take {1} ms", count, watch.ElapsedMilliseconds);
		}

		[Test]
		public void CheckPropertyComparerForValueTypeProperties()
		{
			PropertyComparer ageComparer = DynamicCodeFactory.CreatePropertyComparer (typeof (Dummy), "Age");
			PropertyComparer dateComparer = DynamicCodeFactory.CreatePropertyComparer (typeof (Dummy), "Date");
			PropertyComparer valueComparer = DynamicCodeFactory.CreatePropertyComparer (typeof (Dummy), "Value");

			Dummy d1 = new Dummy ();
			Dummy d2 = new Dummy ();
			
			d1.Age = 30;
			d2.Age = 31;

			d1.Date = new System.DateTime (2006, 10, 18);
			d2.Date = new System.DateTime (2006, 10, 19);

			Assert.AreEqual (-1, ageComparer (d1, d2));
			Assert.AreEqual ( 1, ageComparer (d2, d1));
			Assert.AreEqual ( 0, ageComparer (d1, d1));
			Assert.AreEqual (-1, dateComparer (d1, d2));
			Assert.AreEqual ( 1, dateComparer (d2, d1));
			Assert.AreEqual ( 0, dateComparer (d1, d1));

			Value v1 = new Value (1, 2, "A");
			Value v2 = new Value (1, 2, "X");

			d1.Value = v1;
			d2.Value = v2;

			Assert.AreEqual (-1, valueComparer (d1, d2));
			Assert.AreEqual ( 1, valueComparer (d2, d1));
			Assert.AreEqual ( 0, valueComparer (d1, d1));
		}

		[Test]
		public void CheckValueTypePropertyAccess()
		{
			PropertyGetter   articleGetter   = DynamicCodeFactory.CreatePropertyGetter (typeof (Record), "Article");
			PropertyComparer articleComparer = DynamicCodeFactory.CreatePropertyComparer (typeof (Record), "Article");
			PropertyComparer stockComparer   = DynamicCodeFactory.CreatePropertyComparer (typeof (Record), "Stock");

			Record record1 = new Record ("Harddisk", 4, 402.00M);
			Record record2 = new Record ("Computer", 1, 1580.00M);

			Assert.AreEqual (record1.Article, articleGetter (record1));
			Assert.AreEqual (record2.Article, articleGetter (record2));

			Assert.AreEqual ( 0, articleComparer (record1, record1));
			Assert.AreEqual ( 0, articleComparer (record2, record2));
			Assert.AreEqual ( 1, articleComparer (record1, record2));
			Assert.AreEqual (-1, articleComparer (record2, record1));

			Assert.AreEqual (1, (record1.Stock).CompareTo (record2.Stock));
			
			Assert.AreEqual ( 1, stockComparer (record1, record2));
			Assert.AreEqual (-1, stockComparer (record2, record1));
		}

		[Test]
		[ExpectedException (typeof (System.InvalidOperationException))]
		public void CheckValueTypePropertyAccessEx1()
		{
			PropertySetter articleSetter = DynamicCodeFactory.CreatePropertySetter (typeof (Record), "Article");
		}

		[Test]
		public void CheckPropertyGet()
		{
			PropertyGetter nameGetter = DynamicCodeFactory.CreatePropertyGetter (typeof (Dummy), "Name");
			PropertyGetter ageGetter  = DynamicCodeFactory.CreatePropertyGetter (typeof (Dummy), "Age");
			PropertyGetter dateGetter = DynamicCodeFactory.CreatePropertyGetter (typeof (Dummy), "Date");

			System.DateTime date = new System.DateTime (2006, 10, 17);
			Dummy dummy = new Dummy ();

			dummy.Name = "Bill";
			dummy.Age = 50;
			dummy.Date = date;

			Assert.AreEqual ("Bill", nameGetter (dummy));
			Assert.AreEqual (50, ageGetter (dummy));
			Assert.AreEqual (date, dateGetter (dummy));

			System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch ();

			watch.Reset ();
			watch.Start ();

			const int count = 1000000;
			int ok = 0;

			for (int i = 0; i < count; i++)
			{
				if (((string) nameGetter (dummy) == "Bill") &&
					((int) ageGetter (dummy) == 50) &&
					((System.DateTime) dateGetter (dummy) == date))
				{
					ok++;
				}
			}

			watch.Stop ();

			Assert.AreEqual (count, ok);

			System.Console.Out.WriteLine ("{0} dynamic 'get' take {1} ms", count, watch.ElapsedMilliseconds);

			watch.Reset ();
			watch.Start ();

			ok = 0;

			for (int i = 0; i < count; i++)
			{
				if ((dummy.Name == "Bill") &&
					(dummy.Age == 50) &&
					(dummy.Date == date))
				{
					ok++;
				}
			}

			watch.Stop ();

			Assert.AreEqual (count, ok);

			System.Console.Out.WriteLine ("{0} static 'get' take {1} ms", count, watch.ElapsedMilliseconds);
		}

		[Test]
		public void CheckPropertySet()
		{
			PropertySetter nameSetter = DynamicCodeFactory.CreatePropertySetter (typeof (Dummy), "Name");
			PropertySetter ageSetter  = DynamicCodeFactory.CreatePropertySetter (typeof (Dummy), "Age");
			PropertySetter dateSetter = DynamicCodeFactory.CreatePropertySetter (typeof (Dummy), "Date");

			System.DateTime date = new System.DateTime (2006, 10, 17);
			Dummy dummy = new Dummy ();

			nameSetter (dummy, "Bill");
			ageSetter (dummy, 50);
			dateSetter (dummy, date);

			Assert.AreEqual ("Bill", dummy.Name);
			Assert.AreEqual (50, dummy.Age);
			Assert.AreEqual (date, dummy.Date);

			System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch ();

			watch.Reset ();
			watch.Start ();

			const int count = 1000000;

			for (int i = 0; i < count; i++)
			{
				nameSetter (dummy, "Bill");
				ageSetter (dummy, 50);
				dateSetter (dummy, date);
			}

			watch.Stop ();

			System.Console.Out.WriteLine ("{0} dynamic 'set' take {1} ms", count, watch.ElapsedMilliseconds);

			watch.Reset ();
			watch.Start ();

			for (int i = 0; i < count; i++)
			{
				dummy.Name = "Bill";
				dummy.Age = 50;
				dummy.Date = date;
			}

			watch.Stop ();

			System.Console.Out.WriteLine ("{0} static 'set' take {1} ms", count, watch.ElapsedMilliseconds);
		}

		#region Record Structure

		private struct Record
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

		#endregion

		#region Value Structure

		struct Value : System.IComparable<Value>
		{
			public Value(int a, int b, string c)
			{
				this.a = a;
				this.b = b;
				this.c = c;
			}
			
			private int a;
			private int b;
			private string c;

			#region IComparable<Value> Members

			public int CompareTo(Value other)
			{
				if ((this.a == other.a) &&
					(this.b == other.b) &&
					(this.c == other.c))
				{
					return 0;
				}
				else
				{
					return this.c.CompareTo (other.c);
				}
			}

			#endregion
		}

		#endregion

		#region Dummy Class

		private class Dummy
		{
			public Dummy()
			{
			}

			public Dummy(string name)
			{
				this.name = name;
			}

			public string Name
			{
				get
				{
					return this.name;
				}
				set
				{
					this.name = value;
				}
			}

			public int Age
			{
				get
				{
					return this.age;
				}
				set
				{
					this.age = value;
				}
			}

			public System.DateTime Date
			{
				get
				{
					return this.date;
				}
				set
				{
					this.date = value;
				}
			}

			public Value Value
			{
				get
				{
					return this.value;
				}
				set
				{
					this.value = value;
				}
			}

			private string name;
			private int age;
			private System.DateTime date;
			private Value value;
		}

		#endregion
	}
}
