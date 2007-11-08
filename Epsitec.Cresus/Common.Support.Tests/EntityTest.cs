using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;

using NUnit.Framework;

using System.Collections.Generic;

[assembly: Entity ("[70052]", typeof (Epsitec.Common.Support.EntityTest.MyEnumTypeEntity))]
[assembly: Entity ("[70062]", typeof (Epsitec.Common.Support.EntityTest.MyEnumValueEntity))]
[assembly: Entity ("[7013]", typeof (Epsitec.Common.Support.EntityTest.MyTestInterfaceUserEntity))]
[assembly: Entity ("[7007]", typeof (Epsitec.Common.Support.EntityTest.MyResourceStringEntity))]

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
			Assert.AreEqual (EntityDataState.Unchanged, entity.GetEntityDataState ());
			Assert.IsFalse (entity.ContainsDataVersion (EntityDataVersion.Original));
			Assert.IsFalse (entity.ContainsDataVersion (EntityDataVersion.Modified));

			entity.SetField (Res.Fields.ResourceBase.Comment.ToString (), null, "Abc");

			Assert.AreEqual ("Abc", entity.GetField<string> (Res.Fields.ResourceBase.Comment.ToString ()));
			Assert.AreEqual (EntityDataState.Modified, entity.GetEntityDataState ());
			Assert.IsFalse (entity.ContainsDataVersion (EntityDataVersion.Original));
			Assert.IsTrue (entity.ContainsDataVersion (EntityDataVersion.Modified));
			
			using (entity.DefineOriginalValues ())
			{
				entity.SetField (Res.Fields.ResourceBase.Comment.ToString (), null, "Xyz");
				Assert.AreEqual ("Xyz", entity.GetField<string> (Res.Fields.ResourceBase.Comment.ToString ()));
			}

			Assert.AreEqual ("Abc", entity.GetField<string> (Res.Fields.ResourceBase.Comment.ToString ()));
			Assert.AreEqual (EntityDataState.Modified, entity.GetEntityDataState ());
			Assert.IsTrue (entity.ContainsDataVersion (EntityDataVersion.Original));
			Assert.IsTrue (entity.ContainsDataVersion (EntityDataVersion.Modified));
		}

		[Test]
		public void CheckBasicAccess2()
		{
			MyEnumTypeEntity entity = new MyEnumTypeEntity ();

			Assert.AreEqual (null, entity.GetField<string> (Res.Fields.ResourceBase.Comment.ToString ()));
			Assert.AreEqual (EntityDataState.Unchanged, entity.GetEntityDataState ());
			Assert.IsFalse (entity.ContainsDataVersion (EntityDataVersion.Original));
			Assert.IsFalse (entity.ContainsDataVersion (EntityDataVersion.Modified));

			using (entity.DefineOriginalValues ())
			{
				entity.SetField (Res.Fields.ResourceBase.Comment.ToString (), null, "Abc");
			}

			Assert.AreEqual ("Abc", entity.GetField<string> (Res.Fields.ResourceBase.Comment.ToString ()));
			Assert.AreEqual (EntityDataState.Unchanged, entity.GetEntityDataState ());
			Assert.IsTrue (entity.ContainsDataVersion (EntityDataVersion.Original));
			Assert.IsFalse (entity.ContainsDataVersion (EntityDataVersion.Modified));

			entity.SetField (Res.Fields.ResourceBase.Comment.ToString (), "Abc", "Xyz");
			
			Assert.AreEqual ("Xyz", entity.GetField<string> (Res.Fields.ResourceBase.Comment.ToString ()));
			Assert.AreEqual (EntityDataState.Modified, entity.GetEntityDataState ());
			Assert.IsTrue (entity.ContainsDataVersion (EntityDataVersion.Original));
			Assert.IsTrue (entity.ContainsDataVersion (EntityDataVersion.Modified));
		}

		[Test]
		public void CheckBasicAccess3()
		{
			MyEnumTypeEntity entity = new MyEnumTypeEntity ();

			Assert.AreEqual (EntityDataState.Unchanged, entity.GetEntityDataState ());
			Assert.IsFalse (entity.ContainsDataVersion (EntityDataVersion.Original));
			Assert.IsFalse (entity.ContainsDataVersion (EntityDataVersion.Modified));

			IList<MyEnumValueEntity> list1;
			IList<MyEnumValueEntity> list2;
			
			list1 = entity.GetFieldCollection<MyEnumValueEntity> (Res.Fields.ResourceEnumType.Values.ToString ());

			Assert.IsNotNull (list1);

			Assert.AreEqual (EntityDataState.Unchanged, entity.GetEntityDataState ());
			Assert.IsTrue (entity.ContainsDataVersion (EntityDataVersion.Original));
			Assert.IsFalse (entity.ContainsDataVersion (EntityDataVersion.Modified));

			using (entity.DefineOriginalValues ())
			{
				list1.Add (new MyEnumValueEntity ());
			}

			Assert.AreEqual (1, list1.Count);
			Assert.AreEqual (EntityDataState.Unchanged, entity.GetEntityDataState ());
			Assert.IsTrue (entity.ContainsDataVersion (EntityDataVersion.Original));
			Assert.IsFalse (entity.ContainsDataVersion (EntityDataVersion.Modified));

			list1.Add (new MyEnumValueEntity ());
			list2 = entity.GetFieldCollection<MyEnumValueEntity> (Res.Fields.ResourceEnumType.Values.ToString ());

			Assert.AreEqual (1, list1.Count);
			Assert.AreEqual (2, list2.Count);
			Assert.AreEqual (EntityDataState.Modified, entity.GetEntityDataState ());
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

		[Test]
		public void CheckCreation()
		{
			AbstractEntity entity;
			EntityContext context = EntityContext.Current;

			entity = context.CreateEmptyEntity (Res.Types.ResourceEnumType.CaptionId);

			Assert.IsNotNull (entity);
			Assert.AreEqual (typeof (MyEnumTypeEntity), entity.GetType ());

			entity = context.CreateEmptyEntity<MyEnumTypeEntity> ();

			Assert.IsNotNull (entity);
			Assert.AreEqual (typeof (MyEnumTypeEntity), entity.GetType ());

			entity = context.CreateEmptyEntity<MyTestInterfaceUserEntity> ();

			Assert.IsNotNull (entity);
			Assert.AreEqual (typeof (MyTestInterfaceUserEntity), entity.GetType ());

			entity = context.CreateEntity<MyTestInterfaceUserEntity> ();

			Assert.IsNotNull (entity);
			Assert.IsNotNull (entity.GetField<MyResourceStringEntity> (Res.Fields.TestInterface.Resource.ToString ()));

			Assert.AreEqual (typeof (MyTestInterfaceUserEntity), entity.GetType ());
		}


		#region Fake EnumType Entity

		internal class MyEnumTypeEntity : AbstractEntity
		{
			public override Druid GetEntityStructuredTypeId()
			{
				return Res.Types.ResourceEnumType.CaptionId;
			}
		}

		#endregion

		#region Fake EnumValue Entity

		internal class MyEnumValueEntity : AbstractEntity
		{
			public override Druid GetEntityStructuredTypeId()
			{
				return Res.Types.EnumValue.CaptionId;
			}
		}

		#endregion

		#region Fake TestInterfaceUser Entity

		internal class MyTestInterfaceUserEntity : AbstractEntity
		{
			public override Druid GetEntityStructuredTypeId()
			{
				return Res.Types.TestInterfaceUser.CaptionId;
			}
		}

		#endregion

		#region Fake ResourceString Entity

		internal class MyResourceStringEntity : AbstractEntity
		{
			public override Druid GetEntityStructuredTypeId()
			{
				return Res.Types.ResourceString.CaptionId;
			}
		}

		#endregion
	}
}
