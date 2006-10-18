//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using NUnit.Framework;

namespace Epsitec.Common.Support
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

			private string name;
			private int age;
			private System.DateTime date;
		}
	}
}
