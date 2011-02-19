//	Copyright © 2008, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;

using NUnit.Framework;

using System.Collections.Generic;

namespace Epsitec.Common.Reporting
{
	[TestFixture]
	public class CollectionSettingTest
	{
		[Test]
		public void Check01CreateList()
		{
			Settings.CollectionSetting setting = new Settings.CollectionSetting ()
			{
				Filter = null,
				Sort = null
			};

			List<AbstractEntity> entities = setting.CreateList (CollectionSettingTest.GetSampleEntities ());

			System.Console.Out.WriteLine ("Unsorted\n--------------------\n{0}", entities.FlattenSamples ("\n"));

			Assert.AreEqual ("Pierre Arnaud 1972;Daphné Arnaud 2008;Clarisse Arnaud 2006;Daniel Roux 1958;Denis Dumoulin 1957;Jérôme André 1994;", entities.FlattenSamples (";"));
		}
		
		[Test]
		public void Check02CreateList()
		{
			Settings.CollectionSetting setting = new Settings.CollectionSetting ()
			{
				Filter = e => e.GetField<string> ("last") == "Arnaud",
				Sort = null
			};

			List<AbstractEntity> entities = setting.CreateList (CollectionSettingTest.GetSampleEntities ());

			System.Console.Out.WriteLine ("Filtered\n--------------------\n{0}", entities.FlattenSamples ("\n"));

			Assert.AreEqual ("Pierre Arnaud 1972;Daphné Arnaud 2008;Clarisse Arnaud 2006;", entities.FlattenSamples (";"));
		}

		[Test]
		public void Check03CreateList()
		{
			Settings.CollectionSetting setting = new Settings.CollectionSetting ()
			{
				Filter = null,
				Sort = (a, b) => string.Compare (a.GetField<string> ("last"), b.GetField<string> ("last"))*10 + string.Compare (a.GetField<string> ("first"), b.GetField<string> ("first"))
			};

			List<AbstractEntity> entities = setting.CreateList (CollectionSettingTest.GetSampleEntities ());

			System.Console.Out.WriteLine ("Sorted\n--------------------\n{0}", entities.FlattenSamples ("\n"));

			Assert.AreEqual ("Jérôme André 1994;Clarisse Arnaud 2006;Daphné Arnaud 2008;Pierre Arnaud 1972;Denis Dumoulin 1957;Daniel Roux 1958;", entities.FlattenSamples (";"));
		}

		
		
		private static IEnumerable<AbstractEntity> GetSampleEntities()
		{
			yield return CollectionSettingTest.CreateEntity ("Pierre", "Arnaud", 1972);
			yield return CollectionSettingTest.CreateEntity ("Daphné", "Arnaud", 2008);
			yield return CollectionSettingTest.CreateEntity ("Clarisse", "Arnaud", 2006);
			yield return CollectionSettingTest.CreateEntity ("Daniel", "Roux", 1958);
			yield return CollectionSettingTest.CreateEntity ("Denis", "Dumoulin", 1957);
			yield return CollectionSettingTest.CreateEntity ("Jérôme", "André", 1994);
		}

		private static AbstractEntity CreateEntity(string firstName, string lastName, int year)
		{
			GenericEntity entity = new GenericEntity (Druid.Empty);

			using (entity.DefineOriginalValues ())
			{
				entity.SetField<string> ("first", firstName);
				entity.SetField<string> ("last", lastName);
				entity.SetField<int> ("year", year);
			}

			return entity;
		}
	}
}
