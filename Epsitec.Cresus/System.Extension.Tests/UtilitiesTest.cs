using NUnit.Framework;

namespace System.Tests
{
	[TestFixture]
	public class UtilitiesTest
	{
		[Test] public void CheckCheckForDuplicates1()
		{
			string[] data = new string[] { "A", "B", "C", "C", "D" };
			
			Assertion.Assert (System.Utilities.CheckForDuplicates (data));
			Assertion.Assert (System.Utilities.CheckForDuplicates (data, false));
		}
		
		[Test] public void CheckCheckForDuplicates2()
		{
			string[] data = new string[] { "C", "B", "A", "C", "D" };
			
			Assertion.AssertEquals (true,  System.Utilities.CheckForDuplicates (data));
			Assertion.AssertEquals (false, System.Utilities.CheckForDuplicates (data, false));
		}
		
		[Test] public void CheckCheckForDuplicates3()
		{
			object[] data1 = new object[] { new SpecialData(1, "A"), new SpecialData (2, "B"), new SpecialData (3, "C") };
			object[] data2 = new object[] { new SpecialData(1, "C"), new SpecialData (2, "B"), new SpecialData (3, "C") };
			object[] data3 = new object[] { new SpecialData(3, "A"), new SpecialData (2, "B"), new SpecialData (3, "C") };
			
			Assertion.AssertEquals (false, System.Utilities.CheckForDuplicates (data1, SpecialData.NumComparer));
			Assertion.AssertEquals (false, System.Utilities.CheckForDuplicates (data1, SpecialData.TextComparer));
			
			Assertion.AssertEquals (false, System.Utilities.CheckForDuplicates (data2, SpecialData.NumComparer));
			Assertion.AssertEquals (true,  System.Utilities.CheckForDuplicates (data2, SpecialData.TextComparer));
			
			Assertion.AssertEquals (true,  System.Utilities.CheckForDuplicates (data3, SpecialData.NumComparer));
			Assertion.AssertEquals (false, System.Utilities.CheckForDuplicates (data3, SpecialData.TextComparer));
		}
		
		class SpecialData
		{
			public SpecialData(int num, string text)
			{
				this.num  = num;
				this.text = text;
			}
			
			
			private int		num;
			private string	text;
			
			public static System.Collections.IComparer NumComparer { get { return new SpecialCompare1 (); } }
			public static System.Collections.IComparer TextComparer { get { return new SpecialCompare2 (); } }
			
			class SpecialCompare1 : System.Collections.IComparer
			{
				#region IComparer Members

				public int Compare(object x, object y)
				{
					SpecialData ox = x as SpecialData;
					SpecialData oy = y as SpecialData;
					if (ox == oy) return 0;
					if (ox == null) return -1;
					if (oy == null) return 1;
					
					if (ox.num < oy.num) return -1;
					if (ox.num > oy.num) return 1;
					return 0;
				}

				#endregion
			}
			class SpecialCompare2 : System.Collections.IComparer
			{
				#region IComparer Members

				public int Compare(object x, object y)
				{
					SpecialData ox = x as SpecialData;
					SpecialData oy = y as SpecialData;
					if (ox == oy) return 0;
					if (ox == null) return -1;
					if (oy == null) return 1;
					return System.String.Compare (ox.text, oy.text);
				}

				#endregion
			}
		}
	}
}
