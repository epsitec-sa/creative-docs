using Epsitec.Common.Support.Extensions;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Collections.Generic;

using System.Linq;


namespace Epsitec.Common.Support.UnitTests.Extensions
{


	[TestClass]
	public sealed  class UnitTestEnumerableExtensions
	{


		[TestMethod]
		public void AppendTest()
		{
			Assert.IsTrue (this.GetSequence (11).SequenceEqual (this.GetSequence (10).Append (10)));
			Assert.IsTrue (this.GetSequence (10).SequenceEqual (this.GetSequence (5).Append(5).Concat (this.GetSequence (10).Skip (6))));

			Assert.IsTrue (this.GetSequence (10).SequenceEqual (this.GetSequence (5).Append (5, 6, 7, 8, 9)));
			Assert.IsTrue (this.GetSequence (10).SequenceEqual (this.GetSequence (5).Append (5, 6).Concat (this.GetSequence (10).Skip (7))));
		}


		private IEnumerable<int> GetSequence(int length)
		{
			List<int> list = new List<int> ();

			for (int i = 0; i < length; i++)
			{
				list.Add (i);
			}

			return list;
		}
            

	}


}
