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

			
			//get all places with streets and houses
			var query =  from s in model.PlacesStreetsAndHouses
						 select s;

			Assert.IsTrue (query.Count() > 0);
		}
	}
}
