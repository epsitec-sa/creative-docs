using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;

using NUnit.Framework;

using System.Collections.Generic;

namespace Epsitec.Common.Support
{
	[TestFixture]
	public class EntityTest
	{
		[Test]
		public void CheckBasicAccess1()
		{
			MyEnumTypeEntity entity = new MyEnumTypeEntity ();

			Assert.AreEqual (null, entity.GetField<string> (Res.Fields.ResourceBase.Comment.ToString ()));
			Assert.AreEqual (EntityDataState.Unchanged, entity.DataState);
			Assert.IsFalse (entity.ContainsDataVersion (EntityDataVersion.Original));
			Assert.IsFalse (entity.ContainsDataVersion (EntityDataVersion.Modified));

			entity.SetField (Res.Fields.ResourceBase.Comment.ToString (), null, "Abc");

			Assert.AreEqual ("Abc", entity.GetField<string> (Res.Fields.ResourceBase.Comment.ToString ()));
			Assert.AreEqual (EntityDataState.Modified, entity.DataState);
			Assert.IsFalse (entity.ContainsDataVersion (EntityDataVersion.Original));
			Assert.IsTrue (entity.ContainsDataVersion (EntityDataVersion.Modified));
		}

		[Test]
		public void CheckBasicAccess2()
		{
			MyEnumTypeEntity entity = new MyEnumTypeEntity ();

			Assert.AreEqual (null, entity.GetField<string> (Res.Fields.ResourceBase.Comment.ToString ()));
			Assert.AreEqual (EntityDataState.Unchanged, entity.DataState);
			Assert.IsFalse (entity.ContainsDataVersion (EntityDataVersion.Original));
			Assert.IsFalse (entity.ContainsDataVersion (EntityDataVersion.Modified));

			using (entity.DefineOriginalValues ())
			{
				entity.SetField (Res.Fields.ResourceBase.Comment.ToString (), null, "Abc");
			}

			Assert.AreEqual ("Abc", entity.GetField<string> (Res.Fields.ResourceBase.Comment.ToString ()));
			Assert.AreEqual (EntityDataState.Unchanged, entity.DataState);
			Assert.IsTrue (entity.ContainsDataVersion (EntityDataVersion.Original));
			Assert.IsFalse (entity.ContainsDataVersion (EntityDataVersion.Modified));

			entity.SetField (Res.Fields.ResourceBase.Comment.ToString (), "Abc", "Xyz");
			
			Assert.AreEqual ("Xyz", entity.GetField<string> (Res.Fields.ResourceBase.Comment.ToString ()));
			Assert.AreEqual (EntityDataState.Modified, entity.DataState);
			Assert.IsTrue (entity.ContainsDataVersion (EntityDataVersion.Original));
			Assert.IsTrue (entity.ContainsDataVersion (EntityDataVersion.Modified));
		}

		[Test]
		public void CheckBasicAccess3()
		{
			MyEnumTypeEntity entity = new MyEnumTypeEntity ();

			Assert.AreEqual (EntityDataState.Unchanged, entity.DataState);
			Assert.IsFalse (entity.ContainsDataVersion (EntityDataVersion.Original));
			Assert.IsFalse (entity.ContainsDataVersion (EntityDataVersion.Modified));

			IList<MyEnumValueEntity> list1;
			IList<MyEnumValueEntity> list2;
			
			list1 = entity.GetFieldCollection<MyEnumValueEntity> (Res.Fields.ResourceEnumType.Values.ToString ());

			Assert.IsNotNull (list1);

			Assert.AreEqual (EntityDataState.Unchanged, entity.DataState);
			Assert.IsTrue (entity.ContainsDataVersion (EntityDataVersion.Original));
			Assert.IsFalse (entity.ContainsDataVersion (EntityDataVersion.Modified));

			using (entity.DefineOriginalValues ())
			{
				list1.Add (new MyEnumValueEntity ());
			}

			Assert.AreEqual (1, list1.Count);
			Assert.AreEqual (EntityDataState.Unchanged, entity.DataState);
			Assert.IsTrue (entity.ContainsDataVersion (EntityDataVersion.Original));
			Assert.IsFalse (entity.ContainsDataVersion (EntityDataVersion.Modified));

			list1.Add (new MyEnumValueEntity ());
			list2 = entity.GetFieldCollection<MyEnumValueEntity> (Res.Fields.ResourceEnumType.Values.ToString ());

			Assert.AreEqual (1, list1.Count);
			Assert.AreEqual (2, list2.Count);
			Assert.AreEqual (EntityDataState.Modified, entity.DataState);
			Assert.IsTrue (entity.ContainsDataVersion (EntityDataVersion.Original));
			Assert.IsTrue (entity.ContainsDataVersion (EntityDataVersion.Modified));
			Assert.IsTrue (list1 != list2);

			list2.Add (new MyEnumValueEntity ());
			Assert.AreEqual (1, list1.Count);
			Assert.AreEqual (3, list2.Count);
			
			using (entity.DefineOriginalValues ())
			{
				list1.Add (new MyEnumValueEntity ());
			}
			
			Assert.AreEqual (2, list1.Count);
			Assert.AreEqual (3, list2.Count);
		}

		[Test]
		[ExpectedException (typeof (System.InvalidOperationException))]
		public void CheckBasicAccessEx1()
		{
			MyEnumTypeEntity entity = new MyEnumTypeEntity ();

			IList<MyEnumValueEntity> list1;
			IList<MyEnumValueEntity> list2;

			list1 = entity.GetFieldCollection<MyEnumValueEntity> (Res.Fields.ResourceEnumType.Values.ToString ());
			
			using (entity.DefineOriginalValues ())
			{
				list1.Add (new MyEnumValueEntity ());
			}

			list1.Add (new MyEnumValueEntity ());
			list2 = entity.GetFieldCollection<MyEnumValueEntity> (Res.Fields.ResourceEnumType.Values.ToString ());

			Assert.AreEqual (1, list1.Count);
			Assert.AreEqual (2, list2.Count);

			//	We may no longer modify the original list here :
			
			list1.Add (new MyEnumValueEntity ());
		}


		#region Fake EnumType Entity

		private class MyEnumTypeEntity : AbstractEntity
		{
			public override Druid GetStructuredTypeId()
			{
				return Res.Types.ResourceEnumType.CaptionId;
			}
		}

		#endregion

		#region Fake EnumValue Entity

		private class MyEnumValueEntity : AbstractEntity
		{
			public override Druid GetStructuredTypeId()
			{
				return Res.Types.EnumValue.CaptionId;
			}
		}

		#endregion
	}
}
