//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.EntityEngine;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.UnitTests.Helpers;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Epsitec.Cresus.Core.UnitTests
{
	[TestClass]
	public class UnitTestEntities
	{
		[ClassInitialize]
		public static void Initialize(TestContext testContext)
		{
			TestHelper.Initialize ();
		}
		
		[TestMethod]
		public void Test01CreateEntity()
		{
			var context = EntityContext.Current;

			PersonTitleEntity title = context.CreateEmptyEntity<PersonTitleEntity> ();

			Assert.IsNull (title.Name);
			Assert.IsNull (title.ShortName);
		}

		[TestMethod]
		public void Test02CreateEntity()
		{
			var context = EntityContext.Current;

			NaturalPersonEntity person = context.CreateEmptyEntity<NaturalPersonEntity> ();

			Assert.IsNull (person.Title);
			Assert.IsFalse (person.BirthDate.HasValue);
			Assert.IsNotNull (person.Contacts);
			Assert.AreEqual (0, person.Contacts.Count);
		}
	}
}
