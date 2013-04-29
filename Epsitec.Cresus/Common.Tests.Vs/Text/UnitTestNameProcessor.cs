using Epsitec.Common.Text;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;

using System.Collections.Generic;


namespace Epsitec.Common.Tests.Vs.Text
{


	[TestClass]
	public class UnitTestNameProcessor
	{


		[TestMethod]
		public void NameProcessorTest()
		{
			var samples = new List<Tuple<string, string>>
			{
				Tuple.Create ((string) null, (string) null),
				Tuple.Create ("", ""),
				Tuple.Create ("Marc", "M."),
				Tuple.Create ("Marc andré", "M."),
				Tuple.Create ("Marc-André", "M.-A."),
				Tuple.Create ("Märc andré", "M."),
				Tuple.Create ("Märc-André", "M.-A."),
				Tuple.Create ("Märc-André Albert", "M.-A."),
				Tuple.Create ("Märc-André-Albert", "M.-A.-A."),
				Tuple.Create ("M'Ärc", "M'Ä."),
				Tuple.Create ("G-Rémi", "G-R."),
				Tuple.Create ("Lou-K", "L.-K"),
			};

			foreach (var sample in samples)
			{
				var actual = NameProcessor.GetAbbreviatedFirstname(sample.Item1);
				var expected = sample.Item2;

				Assert.AreEqual (expected, actual);
			}
		}


	}


}
