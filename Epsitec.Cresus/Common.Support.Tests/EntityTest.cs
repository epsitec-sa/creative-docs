using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;

using NUnit.Framework;

using System.Collections.Generic;

[assembly: EntityClass ("[70052]", typeof (Epsitec.Common.Support.EntityTest.MyEnumTypeEntity))]
[assembly: EntityClass ("[70062]", typeof (Epsitec.Common.Support.EntityTest.MyEnumValueEntity))]
[assembly: EntityClass ("[7013]", typeof (Epsitec.Common.Support.EntityTest.MyTestInterfaceUserEntity))]
[assembly: EntityClass ("[7007]", typeof (Epsitec.Common.Support.EntityTest.MyResourceStringEntity))]

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

			System.Collections.IList list3;

			entity = new MyEnumTypeEntity ();
			list3 = entity.InternalGetFieldCollection (Res.Fields.ResourceEnumType.Values.ToString ());
			list2 = list3 as IList<MyEnumValueEntity>;
			list1 = entity.GetFieldCollection<MyEnumValueEntity> (Res.Fields.ResourceEnumType.Values.ToString ());

			Assert.IsNotNull (list3);
			Assert.IsNotNull (list2);
			Assert.IsNotNull (list1);
			
			Assert.AreEqual (list1, list2);
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

		[Test]
		public void CheckIStructuredData()
		{
			EntityContext context = EntityContext.Current;

			MyTestInterfaceUserEntity entity = context.CreateEntity<MyTestInterfaceUserEntity> ();

			Assert.IsNotNull (entity);
			Assert.IsNotNull (entity.GetField<MyResourceStringEntity> (Res.Fields.TestInterface.Resource.ToString ()));
			Assert.AreEqual (0, entity.getNameCounter);
			Assert.AreEqual (0, entity.setNameCounter);

			IStructuredData data = entity;

			Assert.AreEqual (UndefinedValue.Value, entity.InternalGetValue ("[700J2]"));
			Assert.AreEqual (null, entity.GetField<string> ("[700J2]"));
			Assert.AreEqual (null, data.GetValue ("[700J2]"));
			Assert.AreEqual (1, entity.getNameCounter);
			Assert.AreEqual (0, entity.setNameCounter);

			data.SetValue ("[700J2]", "Foo");

			Assert.AreEqual (2, entity.getNameCounter);
			Assert.AreEqual (1, entity.setNameCounter);
			Assert.AreEqual ("Foo", data.GetValue ("[700J2]"));
			Assert.AreEqual (3, entity.getNameCounter);
			Assert.AreEqual (1, entity.setNameCounter);
		}

		[Test]
		public void CheckProxy()
		{
			AbstractEntity entity;
			AbstractEntity proxyData;

			EntityContext context = EntityContext.Current;

			proxyData = context.CreateEmptyEntity<MyResourceStringEntity> ();
			entity = context.CreateEmptyEntity<MyTestInterfaceUserEntity> ();

			TestProxy proxy = new TestProxy (proxyData);

			Assert.IsNull (entity.GetField<MyResourceStringEntity> (Res.Fields.TestInterface.Resource.ToString ()));

			Assert.AreEqual (0, proxy.ReadCounter);
			Assert.AreEqual (0, proxy.WriteCounter);
			
			entity.InternalSetValue (Res.Fields.TestInterface.Resource.ToString (), proxy);

			Assert.AreEqual (0, proxy.ReadCounter);
			Assert.AreEqual (1, proxy.WriteCounter);

			Assert.AreEqual (proxyData, entity.GetField<MyResourceStringEntity> (Res.Fields.TestInterface.Resource.ToString ()));
			Assert.AreEqual (proxyData, entity.GetField<MyResourceStringEntity> (Res.Fields.TestInterface.Resource.ToString ()));
			
			Assert.AreEqual (2, proxy.ReadCounter);
			Assert.AreEqual (1, proxy.WriteCounter);
		}

		class TestProxy : IEntityProxy
		{
			public TestProxy(object data)
			{
				this.data = data;
			}

			public int ReadCounter
			{
				get
				{
					return this.readCounter;
				}
			}

			public int WriteCounter
			{
				get
				{
					return this.writeCounter;
				}
			}

			#region IEntityProxy Members

			public object GetReadEntityValue(IValueStore store, string id)
			{
				this.readCounter++;
				return this.data;
			}

			public object GetWriteEntityValue(IValueStore store, string id)
			{
				this.writeCounter++;
				return this;
			}

			#endregion

			private readonly object data;
			private int readCounter;
			private int writeCounter;
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
			[EntityField ("[700J2]")]
			public string Name
			{
				get
				{
					this.getNameCounter++;
					return this.GetField<string> ("[700J2]");
				}
				set
				{
					this.setNameCounter++;
					this.SetField<string> ("[700J2]", this.Name, value);
				}
			}

			public override Druid GetEntityStructuredTypeId()
			{
				return Res.Types.TestInterfaceUser.CaptionId;
			}

			public int getNameCounter;
			public int setNameCounter;
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
