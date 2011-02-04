using Epsitec.Common.Support.UnitTests.Entities;

using Epsitec.Common.Types;

using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace Epsitec.Common.Support.UnitTests.EntityEngine
{


	[TestClass]
	public sealed class UnitTestEntities
	{


		[ClassInitialize]
		public static void Initialize(TestContext testContext)
		{
			TestHelper.Initialize ();
		}


		[TestMethod]
		public void InitializeValueToDefault()
		{
			ValueDataEntity valueData = new ValueDataEntity ();

			Assert.IsTrue (UndefinedValue.IsUndefinedValue (valueData.InternalGetValue (Druid.Parse ("[L0AJ3]").ToResourceId ())));
			Assert.IsTrue (UndefinedValue.IsUndefinedValue (valueData.InternalGetValue (Druid.Parse ("[L0AO3]").ToResourceId ())));

			valueData.Value = default (int);
			valueData.NullableValue = default (int?);

			Assert.AreEqual (default (int), valueData.InternalGetValue (Druid.Parse ("[L0AJ3]").ToResourceId ()));
			Assert.AreEqual (default (int?), valueData.InternalGetValue (Druid.Parse ("[L0AO3]").ToResourceId ()));
		}


		[TestMethod]
		public void InitializeReferenceToDefault()
		{
			ReferenceDataEntity referenceData = new ReferenceDataEntity ();

			Assert.IsTrue (UndefinedValue.IsUndefinedValue (referenceData.InternalGetValue (Druid.Parse ("[L0AL3]").ToResourceId ())));

			referenceData.Reference = null;

			Assert.AreEqual (default (int?), referenceData.InternalGetValue (Druid.Parse ("[L0AL3]").ToResourceId ()));
		}


	}


}
