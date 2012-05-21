using Epsitec.Common.Support;

using Epsitec.Common.Tests.Vs.Entities;

using Epsitec.Common.Types;
using Epsitec.Common.Types.Exceptions;

using Epsitec.Common.UnitTesting;

using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace Epsitec.Common.Tests.Vs.Support.EntityEngine
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

			Assert.IsTrue (UndefinedValue.IsUndefinedValue (valueData.InternalGetValue (Druid.Parse ("[I1A1]").ToResourceId ())));
			Assert.IsTrue (UndefinedValue.IsUndefinedValue (valueData.InternalGetValue (Druid.Parse ("[I1A2]").ToResourceId ())));

			valueData.Value = default (int);
			valueData.NullableValue = default (int?);

			Assert.AreEqual (default (int), valueData.InternalGetValue (Druid.Parse ("[I1A1]").ToResourceId ()));
			Assert.AreEqual (default (int?), valueData.InternalGetValue (Druid.Parse ("[I1A2]").ToResourceId ()));
		}


		[TestMethod]
		public void InitializeReferenceToDefault()
		{
			ReferenceDataEntity referenceData = new ReferenceDataEntity ();

			Assert.IsTrue (UndefinedValue.IsUndefinedValue (referenceData.InternalGetValue (Druid.Parse ("[I1A4]").ToResourceId ())));

			referenceData.Reference = null;

			Assert.AreEqual (default (int?), referenceData.InternalGetValue (Druid.Parse ("[I1A4]").ToResourceId ()));
		}


		[TestMethod]
		public void FrozenEntityValueTest()
		{
			ValueDataEntity entity = new ValueDataEntity ();

			entity.Value = 1;
			entity.NullableValue = 2;

			entity.Freeze ();

			ExceptionAssert.Throw<ReadOnlyException>
			(
				() => entity.Value = 3
			);

			ExceptionAssert.Throw<ReadOnlyException>
			(
				() => entity.NullableValue = null
			);
		}


		[TestMethod]
		public void FrozenEntityReferenceTest()
		{
			ReferenceDataEntity entity = new ReferenceDataEntity ();

			entity.Reference = new ValueDataEntity ();

			entity.Freeze ();

			ExceptionAssert.Throw<ReadOnlyException>
			(
				() => entity.Reference = new ValueDataEntity ()
			);

			ExceptionAssert.Throw<ReadOnlyException>
			(
				() => entity.Reference = null
			);
		}


		[TestMethod]
		public void FrozenEntityCollectionTest()
		{
			CollectionDataEntity entity = new CollectionDataEntity ();
			ValueDataEntity target = new ValueDataEntity ();

            entity.Collection.Add (target);

			entity.Freeze ();

			ExceptionAssert.Throw<ReadOnlyException>
			(
				() => entity.Collection.Add (new ValueDataEntity ())
			);

			ExceptionAssert.Throw<ReadOnlyException>
			(
				() => entity.Collection.Clear ()
			);

			ExceptionAssert.Throw<ReadOnlyException>
			(
				() => entity.Collection.Insert (0, new ValueDataEntity ())
			);

			ExceptionAssert.Throw<ReadOnlyException>
			(
				() => entity.Collection.Remove (target)
			);

			ExceptionAssert.Throw<ReadOnlyException>
			(
				() => entity.Collection.RemoveAt (0)
			);
		}


		[TestMethod]
		public void FrozenEntityValueBackDoorTest()
		{
			ValueDataEntity entity = new ValueDataEntity ();

			entity.Freeze ();

			using (entity.DisableReadOnlyChecks ())
			{
				entity.Value = 3;
				entity.NullableValue = null;
			}
		}


		[TestMethod]
		public void FrozenEntityReferenceBackDoorTest()
		{
			ReferenceDataEntity entity = new ReferenceDataEntity ();

			entity.Reference = new ValueDataEntity ();

			entity.Freeze ();

			using (entity.DisableReadOnlyChecks ())
			{
				entity.Reference = new ValueDataEntity ();
				entity.Reference = null;
			}
		}


		[TestMethod]
		public void FrozenEntityCollectionBackDoorTest()
		{
			CollectionDataEntity entity = new CollectionDataEntity ();
			ValueDataEntity target = new ValueDataEntity ();

			entity.Collection.Add (target);

			entity.Freeze ();

			using (entity.DisableReadOnlyChecks ())
			{
				entity.Collection.Insert (0, new ValueDataEntity ());
				entity.Collection.Remove (target);
				entity.Collection.RemoveAt (0);
				entity.Collection.Add (new ValueDataEntity ());
				entity.Collection.Clear ();			
			}
		}


	}


}
