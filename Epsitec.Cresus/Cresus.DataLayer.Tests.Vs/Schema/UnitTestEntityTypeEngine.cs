using Epsitec.Common.Support;
using Epsitec.Common.Types;

using Epsitec.Cresus.DataLayer.Schema;
using Epsitec.Cresus.DataLayer.Tests.Vs.Helpers;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;

using System.Collections.Generic;

using System.Linq;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.UnitTesting;


namespace Epsitec.Cresus.DataLayer.Tests.Vs.Schema
{


	[TestClass]
	public sealed class UnitTestEntityTypeEngine
	{


		[ClassInitialize]
		public static void ClassInitialize(TestContext testContext)
		{
			TestHelper.Initialize ();
		}


		[TestMethod]
		public void EntityTypesCacheTest()
		{
			EntityTypeEngine ete = new EntityTypeEngine (DataInfrastructureHelper.GetEntityIds ());

			var expected = this.GetEntityTypes ().OrderBy (t => t.CaptionId.ToLong ()).ToList ();
			var actual = ete.GetEntityTypes ().OrderBy (t => t.CaptionId.ToLong ()).ToList ();

			CollectionAssert.AreEqual (expected, actual);
		}


		[TestMethod]
		public void EntityTypeCacheArgumentCheck()
		{
			EntityTypeEngine ete = new EntityTypeEngine (DataInfrastructureHelper.GetEntityIds ());

			ExceptionAssert.Throw<System.ArgumentException>
			(
				() => ete.GetEntityType (Druid.FromLong (999999))
			);
		}


		[TestMethod]
		public void EntityTypeCacheTest()
		{
			EntityTypeEngine ete = new EntityTypeEngine (DataInfrastructureHelper.GetEntityIds ());

			foreach (var type in this.GetEntityTypes ())
			{
				var expected = type;
				var actual = ete.GetEntityType (type.CaptionId);

				Assert.AreEqual (expected, actual);
			}
		}


		[TestMethod]
		public void BaseTypesCacheArgumentCheck()
		{
			EntityTypeEngine ete = new EntityTypeEngine (DataInfrastructureHelper.GetEntityIds ());

			ExceptionAssert.Throw<System.ArgumentException>
			(
				() => ete.GetBaseTypes (Druid.FromLong (999999))
			);
		}


		[TestMethod]
		public void BaseTypesCacheTest()
		{
			EntityTypeEngine ete = new EntityTypeEngine (DataInfrastructureHelper.GetEntityIds ());

			foreach (var type in this.GetEntityTypes ())
			{
				var expected = this.GetBaseTypes (type);
				var actual = ete.GetBaseTypes (type.CaptionId);

				CollectionAssert.AreEqual (expected, actual);
			}
		}


		[TestMethod]
		public void RootTypeCacheArgumentCheck()
		{
			EntityTypeEngine ete = new EntityTypeEngine (DataInfrastructureHelper.GetEntityIds ());

			ExceptionAssert.Throw<System.ArgumentException>
			(
				() => ete.GetRootType (Druid.FromLong (999999))
			);
		}


		[TestMethod]
		public void RootTypeCacheTest()
		{
			EntityTypeEngine ete = new EntityTypeEngine (DataInfrastructureHelper.GetEntityIds ());

			foreach (var type in this.GetEntityTypes ())
			{
				var actual = this.GetRootType (type);
				var exepected = ete.GetRootType (type.CaptionId);

				Assert.AreEqual (actual, exepected);
			}
		}


		[TestMethod]
		public void FieldsCacheArgumentCheck()
		{
			EntityTypeEngine ete = new EntityTypeEngine (DataInfrastructureHelper.GetEntityIds ());

			ExceptionAssert.Throw<System.ArgumentException>
			(
				() => ete.GetFields (Druid.FromLong (999999))
			);
		}


		[TestMethod]
		public void FieldsCacheTest()
		{
			EntityTypeEngine ete = new EntityTypeEngine (DataInfrastructureHelper.GetEntityIds ());

			foreach (var type in this.GetEntityTypes ())
			{
				var expected = this.GetFields (type).OrderBy (f => f.CaptionId.ToLong ()).ToList ();
				var actual = ete.GetFields (type.CaptionId).OrderBy (f => f.CaptionId.ToLong ()).ToList ();

				CollectionAssert.AreEqual (expected, actual);
			}
		}


		[TestMethod]
		public void FieldCacheArgumentCheck()
		{
			EntityTypeEngine ete = new EntityTypeEngine (DataInfrastructureHelper.GetEntityIds ());

			ExceptionAssert.Throw<System.ArgumentException>
			(
				() => ete.GetField (Druid.FromLong(999999), Druid.FromLong (999999))
			);
		}


		[TestMethod]
		public void FieldCacheTest()
		{
			EntityTypeEngine ete = new EntityTypeEngine (DataInfrastructureHelper.GetEntityIds ());

			foreach (var type in this.GetEntityTypes ())
			{
				foreach (var field in this.GetFields (type))
				{
					var actual = field;
					var expected = ete.GetField (type.CaptionId, field.CaptionId);

					Assert.AreEqual (actual, expected);
				}
			}
		}


		[TestMethod]
		public void LocalTypeCacheArgumentCheck()
		{
			EntityTypeEngine ete = new EntityTypeEngine (DataInfrastructureHelper.GetEntityIds ());

			ExceptionAssert.Throw<System.ArgumentException>
			(
				() => ete.GetLocalType (Druid.FromLong (999999), Druid.FromLong (999999))
			);
		}


		[TestMethod]
		public void LocalTypeCacheTest()
		{
			EntityTypeEngine ete = new EntityTypeEngine (DataInfrastructureHelper.GetEntityIds ());

			foreach (var type in this.GetEntityTypes())
			{
				foreach (var field in this.GetFields (type))
				{
					var actual = this.GetLocalType (type, field);
					var expected = ete.GetLocalType (type.CaptionId, field.CaptionId);

					Assert.AreEqual (actual, expected);
				}
			}
		}


		[TestMethod]
		public void ValueFieldsCacheArgumentCheck()
		{
			EntityTypeEngine ete = new EntityTypeEngine (DataInfrastructureHelper.GetEntityIds ());

			ExceptionAssert.Throw<System.ArgumentException>
			(
				() => ete.GetValueFields (Druid.FromLong (999999))
			);
		}


		[TestMethod]
		public void ValueFieldsCacheTest()
		{
			EntityTypeEngine ete = new EntityTypeEngine (DataInfrastructureHelper.GetEntityIds ());

			foreach (var type in this.GetEntityTypes ())
			{
				var expected = this.GetValueFields (type).OrderBy (f => f.CaptionId.ToLong ()).ToList ();
				var actual = ete.GetValueFields (type.CaptionId).OrderBy (f => f.CaptionId.ToLong ()).ToList ();

				CollectionAssert.AreEqual (expected, actual);
			}
		}


		[TestMethod]
		public void ReferenceFieldsCacheArgumentCheck()
		{
			EntityTypeEngine ete = new EntityTypeEngine (DataInfrastructureHelper.GetEntityIds ());

			ExceptionAssert.Throw<System.ArgumentException>
			(
				() => ete.GetReferenceFields (Druid.FromLong (999999))
			);
		}


		[TestMethod]
		public void ReferenceFieldsCacheTest()
		{
			EntityTypeEngine ete = new EntityTypeEngine (DataInfrastructureHelper.GetEntityIds ());

			foreach (var type in this.GetEntityTypes ())
			{
				var expected = this.GetReferenceFields (type).OrderBy (f => f.CaptionId.ToLong ()).ToList ();
				var actual = ete.GetReferenceFields (type.CaptionId).OrderBy (f => f.CaptionId.ToLong ()).ToList ();

				CollectionAssert.AreEqual (expected, actual);
			}
		}


		[TestMethod]
		public void CollectionFieldsCacheArgumentCheck()
		{
			EntityTypeEngine ete = new EntityTypeEngine (DataInfrastructureHelper.GetEntityIds ());

			ExceptionAssert.Throw<System.ArgumentException>
			(
				() => ete.GetCollectionFields (Druid.FromLong (999999))
			);
		}


		[TestMethod]
		public void CollectionFieldsCacheTest()
		{
			EntityTypeEngine ete = new EntityTypeEngine (DataInfrastructureHelper.GetEntityIds ());

			foreach (var type in this.GetEntityTypes ())
			{
				var expected = this.GetCollectionFields (type).OrderBy (f => f.CaptionId.ToLong ()).ToList ();
				var actual = ete.GetCollectionFields (type.CaptionId).OrderBy (f => f.CaptionId.ToLong ()).ToList ();

				CollectionAssert.AreEqual (expected, actual);
			}
		}


		[TestMethod]
		public void LocalFieldsCacheArgumentCheck()
		{
			EntityTypeEngine ete = new EntityTypeEngine (DataInfrastructureHelper.GetEntityIds ());

			ExceptionAssert.Throw<System.ArgumentException>
			(
				() => ete.GetLocalFields (Druid.FromLong (999999))
			);
		}


		[TestMethod]
		public void LocalFieldsCacheTest()
		{
			EntityTypeEngine ete = new EntityTypeEngine (DataInfrastructureHelper.GetEntityIds ());

			foreach (var type in this.GetEntityTypes ())
			{
				var expected = this.GetLocalFields (type).OrderBy (f => f.CaptionId.ToLong ()).ToList ();
				var actual = ete.GetLocalFields (type.CaptionId).OrderBy (f => f.CaptionId.ToLong ()).ToList ();

				CollectionAssert.AreEqual (expected, actual);
			}
		}


		[TestMethod]
		public void LocalValueFieldsCacheArgumentCheck()
		{
			EntityTypeEngine ete = new EntityTypeEngine (DataInfrastructureHelper.GetEntityIds ());

			ExceptionAssert.Throw<System.ArgumentException>
			(
				() => ete.GetLocalValueFields (Druid.FromLong (999999))
			);
		}


		[TestMethod]
		public void LocalValueFieldsCacheTest()
		{
			EntityTypeEngine ete = new EntityTypeEngine (DataInfrastructureHelper.GetEntityIds ());

			foreach (var type in this.GetEntityTypes ())
			{
				var expected = this.GetLocalValueFields (type).OrderBy (f => f.CaptionId.ToLong ()).ToList ();
				var actual = ete.GetLocalValueFields (type.CaptionId).OrderBy (f => f.CaptionId.ToLong ()).ToList ();

				CollectionAssert.AreEqual (expected, actual);
			}
		}


		[TestMethod]
		public void LocalReferenceFieldsCacheArgumentCheck()
		{
			EntityTypeEngine ete = new EntityTypeEngine (DataInfrastructureHelper.GetEntityIds ());

			ExceptionAssert.Throw<System.ArgumentException>
			(
				() => ete.GetLocalReferenceFields (Druid.FromLong (999999))
			);
		}


		[TestMethod]
		public void LocalReferenceFieldsCacheTest()
		{
			EntityTypeEngine ete = new EntityTypeEngine (DataInfrastructureHelper.GetEntityIds ());

			foreach (var type in this.GetEntityTypes ())
			{
				var expected = this.GetLocalReferenceFields (type).OrderBy (f => f.CaptionId.ToLong ()).ToList ();
				var actual = ete.GetLocalReferenceFields (type.CaptionId).OrderBy (f => f.CaptionId.ToLong ()).ToList ();

				CollectionAssert.AreEqual (expected, actual);
			}
		}


		[TestMethod]
		public void LocalCollectionFieldsCacheArgumentCheck()
		{
			EntityTypeEngine ete = new EntityTypeEngine (DataInfrastructureHelper.GetEntityIds ());

			ExceptionAssert.Throw<System.ArgumentException>
			(
				() => ete.GetLocalCollectionFields (Druid.FromLong (999999))
			);
		}


		[TestMethod]
		public void LocalCollectionFieldsCacheTest()
		{
			EntityTypeEngine ete = new EntityTypeEngine (DataInfrastructureHelper.GetEntityIds ());

			foreach (var type in this.GetEntityTypes ())
			{
				var expected = this.GetLocalCollectionFields (type).OrderBy (f => f.CaptionId.ToLong ()).ToList ();
				var actual = ete.GetLocalCollectionFields (type.CaptionId).OrderBy (f => f.CaptionId.ToLong ()).ToList ();

				CollectionAssert.AreEqual (expected, actual);
			}
		}


		[TestMethod]
		public void ReferencingFieldsCacheArgumentCheck()
		{
			EntityTypeEngine ete = new EntityTypeEngine (DataInfrastructureHelper.GetEntityIds ());

			ExceptionAssert.Throw<System.ArgumentException>
			(
				() => ete.GetReferenceFields (Druid.FromLong (999999))
			);
		}


		[TestMethod]
		public void ReferencingFieldsFieldsCacheTest()
		{
			EntityTypeEngine ete = new EntityTypeEngine (DataInfrastructureHelper.GetEntityIds ());

			foreach (var type in this.GetEntityTypes ())
			{
				var expected = this.GetReferencingFields (type);
				var actual = ete.GetReferencingFields (type.CaptionId);

				var expectedKeys = expected.Keys.OrderBy (t => t.CaptionId.ToResourceId ()).ToList ();
				var actualKeys = actual.Keys.OrderBy (t => t.CaptionId.ToResourceId ()).ToList ();

				CollectionAssert.AreEqual (expectedKeys, actualKeys);

				foreach (var key in expectedKeys)
				{
					var expectedValues = expected[key].OrderBy (f => f.CaptionId.ToResourceId ()).ToList ();
					var actualValues = actual[key].OrderBy (f => f.CaptionId.ToResourceId ()).ToList ();

					CollectionAssert.AreEqual (expectedValues, actualValues);
				}
			}
		}

		
		private List<Druid> GetEntityIds()
		{
			return new List<Druid> ()
			{
				new Druid ("[J1A4]"), 
				new Druid ("[J1A6]"), 
				new Druid ("[J1A9]"), 
				new Druid ("[J1AE]"), 
				new Druid ("[J1AG]"), 
				new Druid ("[J1AJ]"), 
				new Druid ("[J1AN]"), 
				new Druid ("[J1AQ]"), 
				new Druid ("[J1AT]"), 
				new Druid ("[J1AV]"), 
				new Druid ("[J1A11]"),
				new Druid ("[J1A41]"),
				new Druid ("[J1A61]"),
				new Druid ("[J1A81]"),
				new Druid ("[J1AA1]"),
				new Druid ("[J1AB1]"),
				new Druid ("[J1AE1]"),
				new Druid ("[J1AJ1]"),
				new Druid ("[J1AT1]"),
				new Druid ("[J1A02]"),
				new Druid ("[J1A42]"),
				new Druid ("[J1A72]"),
			};
		}


		private List<StructuredType> GetEntityTypes()
		{
			EntityContext entityContext = new EntityContext ();

			return this.GetEntityIds ().Select (id => entityContext.GetStructuredType (id)).ToList ();
		}


		private List<StructuredType> GetBaseTypes (StructuredType type)
		{
			List<StructuredType> baseTypes = new List<StructuredType> ();
			
			StructuredType baseType = type;

			while (baseType != null)
			{
				baseTypes.Add (baseType);

				baseType = baseType.BaseType;
			}

			return baseTypes;
		}


		private StructuredType GetRootType(StructuredType type)
		{
			return this.GetBaseTypes (type).Last ();
		}


		private List<StructuredTypeField> GetFields(StructuredType type)
		{
			return type.Fields.Values.Where (f => f.Source == FieldSource.Value).ToList ();
		}


		private StructuredType GetLocalType(StructuredType type, StructuredTypeField field)
		{
			return this.GetBaseTypes (type).First (t => t.GetField (field.CaptionId.ToResourceId ()).Membership != FieldMembership.Inherited);
		}


		private List<StructuredTypeField> GetValueFields(StructuredType type)
		{
			return this.GetFields (type).Where (f => f.Relation == FieldRelation.None).ToList ();
		}


		private List<StructuredTypeField> GetReferenceFields(StructuredType type)
		{
			return this.GetFields (type).Where (f => f.Relation == FieldRelation.Reference).ToList ();
		}


		private List<StructuredTypeField> GetCollectionFields(StructuredType type)
		{
			return this.GetFields (type).Where (f => f.Relation == FieldRelation.Collection).ToList ();
		}


		private List<StructuredTypeField> GetLocalFields(StructuredType type)
		{
			return this.GetFields (type).Where (f => f.Membership != FieldMembership.Inherited).ToList ();
		}


		private List<StructuredTypeField> GetLocalValueFields(StructuredType type)
		{
			return this.GetLocalFields (type).Where (f => f.Relation == FieldRelation.None).ToList ();
		}


		private List<StructuredTypeField> GetLocalReferenceFields(StructuredType type)
		{
			return this.GetLocalFields (type).Where (f => f.Relation == FieldRelation.Reference).ToList ();
		}


		private List<StructuredTypeField> GetLocalCollectionFields(StructuredType type)
		{
			return this.GetLocalFields (type).Where (f => f.Relation == FieldRelation.Collection).ToList ();
		}


		private Dictionary<StructuredType, List<StructuredTypeField>> GetReferencingFields(StructuredType type)
		{
			var tmp = from localType in this.GetBaseTypes (type)
					  from structuredType in this.GetEntityTypes ()
					  from structuredField in structuredType.Fields.Values
					  where structuredField.Membership != FieldMembership.Inherited
					  where structuredField.Source == FieldSource.Value
					  let relation = structuredField.Relation
					  where relation == FieldRelation.Reference || relation == FieldRelation.Collection
					  where structuredField.TypeId == localType.CaptionId
					  select new { T = structuredType, F = structuredField };

			return tmp.GroupBy (item => item.T, item => item.F)
				.ToDictionary (g => g.Key, g => g.ToList ());
		}


	}


}
