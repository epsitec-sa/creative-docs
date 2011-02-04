using Epsitec.Common.Support;
using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Support.UnitTests.Entities;

using Epsitec.Common.Types;

using Epsitec.Common.UnitTesting;

using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace Epsitec.Common.Support.UnitTests.Extensions
{


	[TestClass]
	public sealed class UnitTestEntityModifications
	{

		
		[ClassInitialize]
		public static void Initialize(TestContext testContext)
		{
			TestHelper.Initialize ();
		}


		[TestMethod]
		public void HasCollectionChangedTest1()
		{
			ValueDataEntity valueData1 = new ValueDataEntity ();
			ValueDataEntity valueData2 = new ValueDataEntity ();
			ValueDataEntity valueData3 = new ValueDataEntity ();

			CollectionDataEntity collectionData = new CollectionDataEntity ();
			Assert.IsFalse (collectionData.HasValueChanged (Druid.Parse ("[L0AN3]")));

			using (collectionData.DefineOriginalValues ())
			{
				collectionData.Collection.Add (valueData1);
				collectionData.Collection.Add (valueData2);
			}
			Assert.IsFalse (collectionData.HasCollectionChanged (Druid.Parse ("[L0AN3]")));

			collectionData.Collection.Add (valueData3);
			Assert.IsTrue (collectionData.HasCollectionChanged (Druid.Parse ("[L0AN3]")));

			collectionData.Collection.Remove (valueData3);
			Assert.IsFalse (collectionData.HasCollectionChanged (Druid.Parse ("[L0AN3]")));

			collectionData.Collection.Remove (valueData2);
			Assert.IsTrue (collectionData.HasCollectionChanged (Druid.Parse ("[L0AN3]")));

			collectionData.Collection.Add (valueData2);
			Assert.IsFalse (collectionData.HasCollectionChanged (Druid.Parse ("[L0AN3]")));
		}


		[TestMethod]
		public void HasCollectionChangedTest2()
		{
			ValueDataEntity valueData = new ValueDataEntity ();

			CollectionDataEntity collectionData = new CollectionDataEntity ();
			Assert.IsFalse (collectionData.HasValueChanged (Druid.Parse ("[L0AN3]")));

		    collectionData.Collection.Add (valueData);
			Assert.IsTrue (collectionData.HasCollectionChanged (Druid.Parse ("[L0AN3]")));

			collectionData.Collection.Remove (valueData);
			Assert.IsFalse (collectionData.HasCollectionChanged (Druid.Parse ("[L0AN3]")));
		}


		[TestMethod]
		public void HasCollectionChangedTest3()
		{
			ValueDataEntity valueData1 = new ValueDataEntity ();
			ValueDataEntity valueData2 = new ValueDataEntity ();

			CollectionDataEntity collectionData = new CollectionDataEntity ();

			Assert.IsFalse (collectionData.HasCollectionChanged (Druid.Parse ("[L0AN3]")));

			using (collectionData.DefineOriginalValues ())
			{
				collectionData.Collection.Add (valueData1);
				collectionData.Collection.Add (valueData2);
			}
			Assert.IsFalse (collectionData.HasCollectionChanged (Druid.Parse ("[L0AN3]")));

			collectionData.Collection[0] = valueData2;
			collectionData.Collection[1] = valueData1;
			Assert.IsTrue (collectionData.HasCollectionChanged (Druid.Parse ("[L0AN3]")));

			collectionData.Collection[0] = valueData1;
			collectionData.Collection[1] = valueData2;
			Assert.IsFalse (collectionData.HasCollectionChanged (Druid.Parse ("[L0AN3]")));
		}


		[TestMethod]
		public void HasCollectionChangedTest4()
		{
			ValueDataEntity valueData1 = new ValueDataEntity ();
			ValueDataEntity valueData2 = new ValueDataEntity ();

			CollectionDataEntity collectionData = new CollectionDataEntity ();
			Assert.IsFalse (collectionData.HasCollectionChanged (Druid.Parse ("[L0AN3]")));

			collectionData.Collection.Add (valueData1);
			collectionData.Collection.Add (valueData2);
			Assert.IsTrue (collectionData.HasCollectionChanged (Druid.Parse ("[L0AN3]")));

			collectionData.Collection[0] = valueData2;
			collectionData.Collection[1] = valueData1;
			Assert.IsTrue (collectionData.HasCollectionChanged (Druid.Parse ("[L0AN3]")));

			collectionData.Collection[0] = valueData1;
			collectionData.Collection[1] = valueData2;
			Assert.IsTrue (collectionData.HasCollectionChanged (Druid.Parse ("[L0AN3]")));

			collectionData.Collection.Remove (valueData1);
			collectionData.Collection.Remove (valueData2);
			Assert.IsFalse (collectionData.HasCollectionChanged (Druid.Parse ("[L0AN3]")));
		}


		[TestMethod]
		public void HasCollectionChangedTest5()
		{
			CollectionDataEntity collectionData = null;

			ExceptionAssert.Throw<System.ArgumentNullException>
			(
				() => collectionData.HasCollectionChanged (Druid.Parse ("[L0AN3]"))
			);
		}


		[TestMethod]
		public void HasReferenceChangedTest1()
		{
			ValueDataEntity valueData1 = new ValueDataEntity ();
			ValueDataEntity valueData2 = new ValueDataEntity ();

			ReferenceDataEntity referenceData = new ReferenceDataEntity ();
			Assert.IsFalse (referenceData.HasValueChanged (Druid.Parse ("[L0AL3]")));

			using (referenceData.DefineOriginalValues ())
			{
				referenceData.Reference = valueData1;
			}
			Assert.IsFalse (referenceData.HasReferenceChanged (Druid.Parse ("[L0AL3]")));

			referenceData.Reference = valueData2;
			Assert.IsTrue (referenceData.HasReferenceChanged (Druid.Parse ("[L0AL3]")));

			referenceData.Reference = valueData1;
			Assert.IsFalse (referenceData.HasReferenceChanged (Druid.Parse ("[L0AL3]")));
		}


		[TestMethod]
		public void HasReferenceChangedTest2()
		{
			ValueDataEntity valueData1 = new ValueDataEntity ();
			ValueDataEntity valueData2 = new ValueDataEntity ();

			ReferenceDataEntity referenceData = new ReferenceDataEntity ();
			Assert.IsFalse (referenceData.HasReferenceChanged (Druid.Parse ("[L0AL3]")));

			referenceData.Reference = valueData1;
			Assert.IsTrue (referenceData.HasReferenceChanged (Druid.Parse ("[L0AL3]")));

			referenceData.Reference = valueData2;
			Assert.IsTrue (referenceData.HasReferenceChanged (Druid.Parse ("[L0AL3]")));

			referenceData.Reference = valueData1;
			Assert.IsTrue (referenceData.HasReferenceChanged (Druid.Parse ("[L0AL3]")));
		}


		[TestMethod]
		public void HasReferenceChangedTest3()
		{
			ValueDataEntity valueData1 = new ValueDataEntity ();
			ValueDataEntity valueData2 = new ValueDataEntity ();

			ReferenceDataEntity referenceData = new ReferenceDataEntity ();
			using (referenceData.DefineOriginalValues ())
			{
				referenceData.Reference = valueData1;
			}
			Assert.IsFalse (referenceData.HasReferenceChanged (Druid.Parse ("[L0AL3]")));

			referenceData.Reference = valueData2;
			Assert.IsTrue (referenceData.HasReferenceChanged (Druid.Parse ("[L0AL3]")));

			referenceData.Reference = valueData1;
			Assert.IsFalse (referenceData.HasReferenceChanged (Druid.Parse ("[L0AL3]")));

			referenceData.Reference = null;
			Assert.IsTrue (referenceData.HasReferenceChanged (Druid.Parse ("[L0AL3]")));
		}


		[TestMethod]
		public void HasReferenceChangedTest4()
		{
			ValueDataEntity valueData = new ValueDataEntity ();

			ReferenceDataEntity referenceData = new ReferenceDataEntity ();
			Assert.IsFalse (referenceData.HasReferenceChanged (Druid.Parse ("[L0AL3]")));

			referenceData.Reference = valueData;
			Assert.IsTrue (referenceData.HasReferenceChanged (Druid.Parse ("[L0AL3]")));

			referenceData.Reference = null;
			Assert.IsTrue (referenceData.HasReferenceChanged (Druid.Parse ("[L0AL3]")));
		}


		[TestMethod]
		public void HasReferenceChangedTest5()
		{
			ReferenceDataEntity referenceData = null;

			ExceptionAssert.Throw<System.ArgumentNullException>
			(
				() => referenceData.HasReferenceChanged (Druid.Parse ("[L0AL3]"))
			);
		}


		[TestMethod]
		public void HasValueChangedTest1()
		{
			ValueDataEntity valueData = new ValueDataEntity ();
			Assert.IsFalse (valueData.HasValueChanged (Druid.Parse ("[L0AJ3]")));

			using (valueData.DefineOriginalValues ())
			{
				valueData.Value = 1;
			}
			Assert.IsFalse (valueData.HasValueChanged (Druid.Parse ("[L0AJ3]")));

			valueData.Value = 2;
			Assert.IsTrue (valueData.HasValueChanged (Druid.Parse ("[L0AJ3]")));

			valueData.Value = 1;
			Assert.IsFalse (valueData.HasValueChanged (Druid.Parse ("[L0AJ3]")));
		}


		[TestMethod]
		public void HasValueChangedTest2()
		{
			ValueDataEntity valueData = new ValueDataEntity ();
			Assert.IsFalse (valueData.HasValueChanged (Druid.Parse ("[L0AJ3]")));

			valueData.Value = 1;
			Assert.IsTrue (valueData.HasValueChanged (Druid.Parse ("[L0AJ3]")));

			valueData.Value = 2;
			Assert.IsTrue (valueData.HasValueChanged (Druid.Parse ("[L0AJ3]")));

			valueData.Value = 1;
			Assert.IsTrue (valueData.HasValueChanged (Druid.Parse ("[L0AJ3]")));
		}


		[TestMethod]
		public void HasValueChangedTest3()
		{
			ValueDataEntity valueData = new ValueDataEntity ();
			using (valueData.DefineOriginalValues ())
			{
				valueData.NullableValue = 1;
			}
			Assert.IsFalse (valueData.HasValueChanged (Druid.Parse ("[L0AO3]")));

			valueData.NullableValue = 2;
			Assert.IsTrue (valueData.HasValueChanged (Druid.Parse ("[L0AO3]")));

			valueData.NullableValue = 1;
			Assert.IsFalse (valueData.HasValueChanged (Druid.Parse ("[L0AO3]")));

			valueData.NullableValue = null;
			Assert.IsTrue (valueData.HasValueChanged (Druid.Parse ("[L0AO3]")));
		}


		[TestMethod]
		public void HasValueChangedTest4()
		{
			ValueDataEntity valueData = new ValueDataEntity ();
			Assert.IsFalse (valueData.HasValueChanged (Druid.Parse ("[L0AO3]")));

			valueData.NullableValue = 1;
			Assert.IsTrue (valueData.HasValueChanged (Druid.Parse ("[L0AO3]")));

			valueData.NullableValue = null;
			Assert.IsTrue (valueData.HasValueChanged (Druid.Parse ("[L0AO3]")));
		}


		[TestMethod]
		public void HasValueChangedTest5()
		{
			ValueDataEntity valueData = null;

			ExceptionAssert.Throw<System.ArgumentNullException>
			(
				() => valueData.HasValueChanged (Druid.Parse ("[L0AJ3]"))
			);
		}


	}


}
