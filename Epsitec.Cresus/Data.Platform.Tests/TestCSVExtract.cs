using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Epsitec.Data.Platform;
using System.Linq;

namespace Data.Platform.Tests
{
	[TestClass]
	public class ExtractTest
	{
		[TestMethod]
		public void TestCSVExtract()
		{
			var model = MatchSortEtl.Current;

			var query = from p in model.Places
						join s in model.Streets on p.placeId equals s.placeId
						join h in model.Houses on s.streetId equals h.streetId
						select new
						{
							p.canton,
							p.cityLine18,
							s.streetName,
							h.houseNumber
						};
			
			Assert.IsTrue (query.Count () > 1);
			

			
		}
	}
}
