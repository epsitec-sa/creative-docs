//	Copyright © 2012-2017, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Marc BETTEX

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
		public void GetAbbreviatedFirstNameTest()
		{
			var samples = new List<Tuple<string, string>>
			{
				Tuple.Create ((string) null, (string) null),
				Tuple.Create ("", ""),
				Tuple.Create ("Charles", "Ch."),
				Tuple.Create ("Theodor", "Th."),
				Tuple.Create ("Philippe", "Ph."),
				Tuple.Create ("Claire-Lise", "Cl.-L."),
				Tuple.Create ("Christian", "Chr."),
				Tuple.Create ("Ghislaine", "Gh."),
				Tuple.Create ("Françoise", "Fr."),
				Tuple.Create ("Brandon", "Br."),
				Tuple.Create ("Sharon", "Sh."),
				Tuple.Create ("Schorsch", "Sch."),
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
				Tuple.Create ("Marina Suzy", "M."),   // Non breaking space
				Tuple.Create ("Marina	Suzy", "M."), // Tab
				Tuple.Create ("Marina\nSuzy", "M.")   // New line
			};

			foreach (var sample in samples)
			{
				var actual = NameProcessor.GetAbbreviatedFirstName(sample.Item1);
				var expected = sample.Item2;

				Assert.AreEqual (expected, actual);
			}
		}

		[TestMethod]
		public void GetShortenedLastNameTest()
		{
			var samples = new List<Tuple<string, string>>
			{
				Tuple.Create ((string) null, (string) null),
				Tuple.Create ("", ""),
				Tuple.Create ("Dupond", "Dupond"),
				Tuple.Create ("Dupond Durand", "Dupond"),
				Tuple.Create ("de Busset", "de Busset"),
				Tuple.Create ("de Busset Durand", "de Busset"),
				Tuple.Create ("von Allmen", "von Allmen"),
				Tuple.Create ("von Allmen Durand", "von Allmen"),
				Tuple.Create ("de la Colline", "de la Colline"),
				Tuple.Create ("de la Colline Dupuis", "de la Colline"),
				Tuple.Create ("von der Mühler", "von der Mühler"),
				Tuple.Create ("von der Mühler SparkassenLeiter", "von der Mühler"),
				Tuple.Create ("von der Mühler SparkassenLeiter am Das Rhein über Die Wolke", "von der Mühler"),
				Tuple.Create ("de De La Harpe", "de De La Harpe"),
				Tuple.Create ("de De La Colline", "de De La Colline"),
				Tuple.Create ("de De La Colline de la Montagne", "de De La Colline"),
			};

			foreach (var sample in samples)
			{
				var actual = NameProcessor.GetShortenedLastName (sample.Item1);
				var expected = sample.Item2;

				Assert.AreEqual (expected, actual);
			}
		}
	}
}
