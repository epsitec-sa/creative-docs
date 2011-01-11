using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Collections.Generic;


namespace Epsitec.Common.Types.UnitTests
{


	[TestClass]
	public class UnitTestINameComparer
	{


		[TestMethod]
		public void TestEquals()
		{
			INameComparer inc = new INameComparer ();

			List<string> names = new List<string> ()
			{
				"coucou",
				"blabla",
				"super",
			};

			foreach (string name1 in names)
			{
				foreach (string name2 in names)
				{
					NamedClass c1 = new NamedClass (name1);
					NamedClass c2 = new NamedClass (name2);

					Assert.AreEqual (name1.Equals (name2), inc.Equals (c1, c2));
				}
			}

			Assert.IsFalse (inc.Equals (new NamedClass ("name"), new NamedClass (null)));
			Assert.IsFalse (inc.Equals (new NamedClass (null), new NamedClass ("name")));
			Assert.IsFalse (inc.Equals (new NamedClass ("name"), null));
			Assert.IsFalse (inc.Equals (null, new NamedClass ("name")));
			Assert.IsFalse (inc.Equals (null, null));
		}


		[TestMethod]
		public void TestHashCode()
		{
			INameComparer inc = new INameComparer ();

			List<string> names = new List<string> ()
			{
				"coucou",
				"blabla",
				"super",
			};

			foreach (string name in names)
			{
				NamedClass nc = new NamedClass (name);

				Assert.AreEqual (name.GetHashCode (), inc.GetHashCode (nc));
			}

			Assert.AreEqual (0, inc.GetHashCode (null));
			Assert.AreEqual (0, inc.GetHashCode (new NamedClass (null)));
		}


		private class NamedClass : IName
		{


			public NamedClass(string name)
			{
				this.Name = name;
			}


			#region IName Members


			public string Name
			{
				get;
				set;
			}


			#endregion
		
		
		}


	}


}
