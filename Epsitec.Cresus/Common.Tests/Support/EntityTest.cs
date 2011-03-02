//	Copyright © 2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;

using NUnit.Framework;

using System.Collections.Generic;

[assembly: EntityClass ("[70052]", typeof (Epsitec.Common.Tests.Support.EntityTest.MyEnumTypeEntity))]
[assembly: EntityClass ("[70062]", typeof (Epsitec.Common.Tests.Support.EntityTest.MyEnumValueEntity))]
[assembly: EntityClass ("[7013]", typeof (Epsitec.Common.Tests.Support.EntityTest.MyTestInterfaceUserEntity))]
[assembly: EntityClass ("[7007]", typeof (Epsitec.Common.Tests.Support.EntityTest.MyResourceStringEntity))]
[assembly: EntityClass ("[700G]", typeof (Epsitec.Common.Tests.Support.EntityTest.MyResourceCommandEntity))]
[assembly: EntityClass ("[700Q]", typeof (Epsitec.Common.Tests.Support.EntityTest.MyShortcutEntity))]

namespace Epsitec.Common.Tests.Support
{
	[TestFixture]
	public class EntityTest
	{
		[Test]
		public void CheckBasicAccess1()
		{
			MyEnumTypeEntity entity = new MyEnumTypeEntity ();

			Assert.AreEqual (null, entity.GetField<string> (Epsitec.Common.Support.Res.Fields.ResourceBase.Comment.ToString ()));
			Assert.AreEqual (EntityDataState.Unchanged, entity.GetEntityDataState ());
			Assert.IsFalse (entity.ContainsDataVersion (EntityDataVersion.Original));
			Assert.IsFalse (entity.ContainsDataVersion (EntityDataVersion.Modified));

			entity.SetField (Epsitec.Common.Support.Res.Fields.ResourceBase.Comment.ToString (), null, "Abc");

			Assert.AreEqual ("Abc", entity.GetField<string> (Epsitec.Common.Support.Res.Fields.ResourceBase.Comment.ToString ()));
			Assert.AreEqual (EntityDataState.Modified, entity.GetEntityDataState ());
			Assert.IsFalse (entity.ContainsDataVersion (EntityDataVersion.Original));
			Assert.IsTrue (entity.ContainsDataVersion (EntityDataVersion.Modified));
			
			using (entity.DefineOriginalValues ())
			{
				entity.SetField (Epsitec.Common.Support.Res.Fields.ResourceBase.Comment.ToString (), null, "Xyz");
				Assert.AreEqual ("Xyz", entity.GetField<string> (Epsitec.Common.Support.Res.Fields.ResourceBase.Comment.ToString ()));
			}

			Assert.AreEqual ("Abc", entity.GetField<string> (Epsitec.Common.Support.Res.Fields.ResourceBase.Comment.ToString ()));
			Assert.AreEqual (EntityDataState.Modified, entity.GetEntityDataState ());
			Assert.IsTrue (entity.ContainsDataVersion (EntityDataVersion.Original));
			Assert.IsTrue (entity.ContainsDataVersion (EntityDataVersion.Modified));
		}

		[Test]
		public void CheckBasicAccess2()
		{
			MyEnumTypeEntity entity = new MyEnumTypeEntity ();

			Assert.AreEqual (null, entity.GetField<string> (Epsitec.Common.Support.Res.Fields.ResourceBase.Comment.ToString ()));
			Assert.AreEqual (EntityDataState.Unchanged, entity.GetEntityDataState ());
			Assert.IsFalse (entity.ContainsDataVersion (EntityDataVersion.Original));
			Assert.IsFalse (entity.ContainsDataVersion (EntityDataVersion.Modified));

			using (entity.DefineOriginalValues ())
			{
				entity.SetField (Epsitec.Common.Support.Res.Fields.ResourceBase.Comment.ToString (), null, "Abc");
			}

			Assert.AreEqual ("Abc", entity.GetField<string> (Epsitec.Common.Support.Res.Fields.ResourceBase.Comment.ToString ()));
			Assert.AreEqual (EntityDataState.Unchanged, entity.GetEntityDataState ());
			Assert.IsTrue (entity.ContainsDataVersion (EntityDataVersion.Original));
			Assert.IsFalse (entity.ContainsDataVersion (EntityDataVersion.Modified));

			entity.SetField (Epsitec.Common.Support.Res.Fields.ResourceBase.Comment.ToString (), "Abc", "Xyz");
			
			Assert.AreEqual ("Xyz", entity.GetField<string> (Epsitec.Common.Support.Res.Fields.ResourceBase.Comment.ToString ()));
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
			
			list1 = entity.GetFieldCollection<MyEnumValueEntity> (Epsitec.Common.Support.Res.Fields.ResourceEnumType.Values.ToString ());

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
			list2 = entity.GetFieldCollection<MyEnumValueEntity> (Epsitec.Common.Support.Res.Fields.ResourceEnumType.Values.ToString ());

			Assert.AreEqual (2, list1.Count);
			Assert.AreEqual (2, list2.Count);
			Assert.AreEqual (EntityDataState.Modified, entity.GetEntityDataState ());
			Assert.IsTrue (entity.ContainsDataVersion (EntityDataVersion.Original));
			Assert.IsTrue (entity.ContainsDataVersion (EntityDataVersion.Modified));
			Assert.IsTrue (list1 != list2);

			list2.Add (new MyEnumValueEntity ());
			Assert.AreEqual (3, list1.Count);
			Assert.AreEqual (3, list2.Count);
			
			using (entity.DefineOriginalValues ())
			{
				list1.Add (new MyEnumValueEntity ());
			}
			
			Assert.AreEqual (3, list1.Count);
			Assert.AreEqual (3, list2.Count);

			System.Collections.IList list3;

			entity = new MyEnumTypeEntity ();

			((IEntityCollection) entity.InternalGetFieldCollection (Epsitec.Common.Support.Res.Fields.ResourceEnumType.Values.ToString ())).CopyOnWrite ();

			list3 = entity.InternalGetFieldCollection (Epsitec.Common.Support.Res.Fields.ResourceEnumType.Values.ToString ());
			list2 = list3 as IList<MyEnumValueEntity>;
			list1 = entity.GetFieldCollection<MyEnumValueEntity> (Epsitec.Common.Support.Res.Fields.ResourceEnumType.Values.ToString ());

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

			IList<MyEnumValueEntity> list0;
			IList<MyEnumValueEntity> list1;
			IList<MyEnumValueEntity> list2;

			list1 = entity.GetFieldCollection<MyEnumValueEntity> (Epsitec.Common.Support.Res.Fields.ResourceEnumType.Values.ToString ());
			list0 = entity.InternalGetValue (Epsitec.Common.Support.Res.Fields.ResourceEnumType.Values.ToString ()) as IList<MyEnumValueEntity>;

			Assert.IsNotNull (list0);
			Assert.IsNotNull (list1);

			Assert.AreNotEqual (list0.GetType (), list1.GetType ());
			Assert.AreEqual (list1, list0);			//	list1 is a proxy to list0, but with same contents
			
			using (entity.DefineOriginalValues ())
			{
				list1.Add (new MyEnumValueEntity ());
			}

			list1.Add (new MyEnumValueEntity ());
			list2 = entity.GetFieldCollection<MyEnumValueEntity> (Epsitec.Common.Support.Res.Fields.ResourceEnumType.Values.ToString ());

			Assert.AreEqual (1, list0.Count);
			Assert.AreEqual (2, list1.Count);
			Assert.AreEqual (2, list2.Count);

			//	We may no longer modify the original list here :
			
			list0.Add (new MyEnumValueEntity ());
		}

		[Test]
		public void CheckCollections1()
		{
			EntityContext context = EntityContext.Current;
			
			MyResourceCommandEntity cmd1 = context.CreateEmptyEntity<MyResourceCommandEntity> ();
			MyResourceCommandEntity cmd2 = context.CreateEmptyEntity<MyResourceCommandEntity> ();
			MyResourceCommandEntity cmd3 = context.CreateEmptyEntity<MyResourceCommandEntity> ();

			Assert.IsNotNull (cmd1.Shortcuts);
			Assert.IsNotNull (cmd2.InternalGetFieldCollection ("[700O]"));
			Assert.IsNotNull (cmd3.GetFieldCollection<AbstractEntity> ("[700O]"));

			((IEntityCollection) cmd1.Shortcuts).CopyOnWrite ();
			((IEntityCollection) cmd2.InternalGetFieldCollection ("[700O]")).CopyOnWrite ();
			((IEntityCollection) cmd3.GetFieldCollection<AbstractEntity> ("[700O]")).CopyOnWrite ();

			Assert.AreEqual (typeof (EntityCollection<MyShortcutEntity>), cmd1.Shortcuts.GetType ());
			Assert.AreEqual (typeof (EntityCollection<MyShortcutEntity>), cmd2.InternalGetFieldCollection ("[700O]").GetType ());
			Assert.AreEqual (typeof (EntityCollection<AbstractEntity>),   cmd3.GetFieldCollection<AbstractEntity> ("[700O]").GetType ());

			Assert.AreEqual (typeof (EntityCollection<MyShortcutEntity>), cmd1.Shortcuts.GetType ());
			Assert.AreEqual (typeof (EntityCollection<MyShortcutEntity>), cmd2.Shortcuts.GetType ());
			Assert.AreEqual (typeof (EntityCollectionProxy<MyShortcutEntity>), cmd3.Shortcuts.GetType ());

			Assert.AreEqual (typeof (EntityCollection<MyShortcutEntity>), cmd1.InternalGetFieldCollection ("[700O]").GetType ());
			Assert.AreEqual (typeof (EntityCollection<MyShortcutEntity>), cmd2.InternalGetFieldCollection ("[700O]").GetType ());
			Assert.AreEqual (typeof (EntityCollection<AbstractEntity>), cmd3.InternalGetFieldCollection ("[700O]").GetType ());

			Assert.AreEqual (typeof (EntityCollectionProxy<AbstractEntity>), cmd1.GetFieldCollection<AbstractEntity> ("[700O]").GetType ());
			Assert.AreEqual (typeof (EntityCollectionProxy<AbstractEntity>), cmd2.GetFieldCollection<AbstractEntity> ("[700O]").GetType ());
			Assert.AreEqual (typeof (EntityCollection<AbstractEntity>), cmd3.GetFieldCollection<AbstractEntity> ("[700O]").GetType ());

			IList<MyShortcutEntity> list1 = cmd1.Shortcuts;
			IList<MyShortcutEntity> list2 = cmd2.Shortcuts;
			IList<MyShortcutEntity> list3 = cmd3.Shortcuts;

			Assert.AreEqual (list1, cmd1.Shortcuts);
			Assert.AreEqual (list2, cmd2.Shortcuts);
			Assert.AreEqual (list3, cmd3.Shortcuts);		//	every proxy is different, but the contents are the same
			
			list1.Add (context.CreateEmptyEntity<MyShortcutEntity> ());
			list2.Add (context.CreateEmptyEntity<MyShortcutEntity> ());
			list3.Add (context.CreateEmptyEntity<MyShortcutEntity> ());

			Assert.AreEqual (list1, cmd1.Shortcuts);
			Assert.AreEqual (list2, cmd2.Shortcuts);
			Assert.AreEqual (list3, cmd3.Shortcuts);		//	every proxy is different, but the contents are the same
		}

		[Test]
		public void CheckCollections2()
		{
			EntityContext context = EntityContext.Current;

			MyResourceCommandEntity cmd1 = context.CreateEmptyEntity<MyResourceCommandEntity> ();
			MyResourceCommandEntity cmd2 = context.CreateEmptyEntity<MyResourceCommandEntity> ();
			MyResourceCommandEntity cmd3 = context.CreateEmptyEntity<MyResourceCommandEntity> ();

			Assert.IsNotNull (cmd1.Shortcuts);
			Assert.IsNotNull (cmd2.InternalGetFieldCollection ("[700O]"));
			Assert.IsNotNull (cmd3.GetFieldCollection<AbstractEntity> ("[700O]"));

			//	All collections in copy-on-write mode

			Assert.AreEqual (typeof (EntityCollectionProxy<MyShortcutEntity>), cmd1.Shortcuts.GetType ());
			Assert.AreEqual (typeof (EntityCollectionProxy<MyShortcutEntity>), cmd2.InternalGetFieldCollection ("[700O]").GetType ());
			Assert.AreEqual (typeof (EntityCollectionProxy<AbstractEntity>), cmd3.GetFieldCollection<AbstractEntity> ("[700O]").GetType ());

			Assert.AreEqual (typeof (EntityCollectionProxy<MyShortcutEntity>), cmd1.Shortcuts.GetType ());
			Assert.AreEqual (typeof (EntityCollectionProxy<MyShortcutEntity>), cmd2.Shortcuts.GetType ());
			Assert.AreEqual (typeof (EntityCollectionProxy<MyShortcutEntity>), cmd3.Shortcuts.GetType ());

			Assert.AreEqual (typeof (EntityCollectionProxy<MyShortcutEntity>), cmd1.InternalGetFieldCollection ("[700O]").GetType ());
			Assert.AreEqual (typeof (EntityCollectionProxy<MyShortcutEntity>), cmd2.InternalGetFieldCollection ("[700O]").GetType ());
			Assert.AreEqual (typeof (EntityCollectionProxy<AbstractEntity>), cmd3.InternalGetFieldCollection ("[700O]").GetType ());

			Assert.AreEqual (typeof (EntityCollectionProxy<AbstractEntity>), cmd1.GetFieldCollection<AbstractEntity> ("[700O]").GetType ());
			Assert.AreEqual (typeof (EntityCollectionProxy<AbstractEntity>), cmd2.GetFieldCollection<AbstractEntity> ("[700O]").GetType ());
			Assert.AreEqual (typeof (EntityCollectionProxy<AbstractEntity>), cmd3.GetFieldCollection<AbstractEntity> ("[700O]").GetType ());

			IList<MyShortcutEntity> list1 = cmd1.Shortcuts;
			IList<MyShortcutEntity> list2 = cmd2.Shortcuts;
			IList<MyShortcutEntity> list3 = cmd3.Shortcuts;

			Assert.AreEqual (list1, cmd1.Shortcuts);		//	every proxy is different, but the contents are the same
			Assert.AreEqual (list2, cmd2.Shortcuts);
			Assert.AreEqual (list3, cmd3.Shortcuts);

			list1.Add (context.CreateEmptyEntity<MyShortcutEntity> ());
			list2.Add (context.CreateEmptyEntity<MyShortcutEntity> ());
			list3.Add (context.CreateEmptyEntity<MyShortcutEntity> ());

			//	Now, copy-on-write has been executed on all collections and proxies are
			//	no longer required :

			Assert.AreEqual (typeof (EntityCollection<MyShortcutEntity>), cmd1.Shortcuts.GetType ());
			Assert.AreEqual (typeof (EntityCollection<MyShortcutEntity>), cmd2.InternalGetFieldCollection ("[700O]").GetType ());
			Assert.AreEqual (typeof (EntityCollection<AbstractEntity>), cmd3.GetFieldCollection<AbstractEntity> ("[700O]").GetType ());

			Assert.AreEqual (typeof (EntityCollection<MyShortcutEntity>), cmd1.Shortcuts.GetType ());
			Assert.AreEqual (typeof (EntityCollection<MyShortcutEntity>), cmd2.Shortcuts.GetType ());
			Assert.AreEqual (typeof (EntityCollectionProxy<MyShortcutEntity>), cmd3.Shortcuts.GetType ());

			Assert.AreEqual (typeof (EntityCollection<MyShortcutEntity>), cmd1.InternalGetFieldCollection ("[700O]").GetType ());
			Assert.AreEqual (typeof (EntityCollection<MyShortcutEntity>), cmd2.InternalGetFieldCollection ("[700O]").GetType ());
			Assert.AreEqual (typeof (EntityCollection<AbstractEntity>), cmd3.InternalGetFieldCollection ("[700O]").GetType ());

			Assert.AreEqual (typeof (EntityCollectionProxy<AbstractEntity>), cmd1.GetFieldCollection<AbstractEntity> ("[700O]").GetType ());
			Assert.AreEqual (typeof (EntityCollectionProxy<AbstractEntity>), cmd2.GetFieldCollection<AbstractEntity> ("[700O]").GetType ());
			Assert.AreEqual (typeof (EntityCollection<AbstractEntity>), cmd3.GetFieldCollection<AbstractEntity> ("[700O]").GetType ());
		}


		[Test]
		public void CheckCreation()
		{
			AbstractEntity entity;
			EntityContext context = EntityContext.Current;

			entity = context.CreateEmptyEntity (Epsitec.Common.Support.Res.Types.ResourceEnumType.CaptionId);

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
			//Assert.IsNotNull (entity.GetField<MyResourceStringEntity> (Epsitec.Common.Support.Res.Fields.TestInterface.Resource.ToString ()));

			Assert.AreEqual (typeof (MyTestInterfaceUserEntity), entity.GetType ());
		}

		[Test]
		public void CheckEventHandling()
		{
			MyEnumTypeEntity entity = EntityContext.Current.CreateEmptyEntity<MyEnumTypeEntity> ();
			IStructuredData data = entity;

			Assert.IsNotNull (entity);
			Assert.IsNotNull (data);

			string fieldId = Epsitec.Common.Support.Res.Fields.ResourceBase.Comment.ToString ();
			List<string> events = new List<string> ();

			data.AttachListener (fieldId,
				delegate (object sender, DependencyPropertyChangedEventArgs e)
				{
					Assert.AreEqual (entity, sender);
					events.Add (string.Format ("Field modified from {0} to {1}", e.OldValue ?? "<null>", e.NewValue ?? "<null>"));
				});
			
			data.AttachListener ("*",
				delegate (object sender, DependencyPropertyChangedEventArgs e)
				{
					Assert.AreEqual (entity, sender);
					events.Add (string.Format ("Content modified from {0} to {1}", e.OldValue ?? "<null>", e.NewValue ?? "<null>"));
				});
			
			entity.SetField (fieldId, null, "Abc");
			entity.SetField (fieldId, "Abc", "Xyz");

			string[] results = new string[]
			{
				"Field modified from <null> to Abc",
				"Content modified from <null> to Abc",
				"Field modified from Abc to Xyz",
				"Content modified from Abc to Xyz",
			};

			int index = 0;

			events.ForEach (
				delegate (string e)
				{
					Assert.AreEqual (results[index++], e);
				});
		}

		[Test]
		public void CheckIStructuredData()
		{
			EntityContext context = EntityContext.Current;

			MyTestInterfaceUserEntity entity = context.CreateEntity<MyTestInterfaceUserEntity> ();

			Assert.IsNotNull (entity);
			//Assert.IsNotNull (entity.GetField<MyResourceStringEntity> (Epsitec.Common.Support.Res.Fields.TestInterface.Resource.ToString ()));
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

			System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch ();
			watch.Start ();
			const int n = 100*1000;

			for (int i = 0; i < n; i++)
			{
				data.SetValue ("[700J2]", i.ToString ());
			}

			watch.Stop ();
			System.Console.Out.WriteLine ("SetValue : {1} us ({0} calls)", n, watch.ElapsedMilliseconds * 100 * 1000 / n / 100.0M);

			watch.Reset ();
			watch.Start ();
			
			for (int i = 0; i < n; i++)
			{
				entity.SetField<string> ("[700J2]", i.ToString ());
			}

			watch.Stop ();
			System.Console.Out.WriteLine ("SetField<string> : {1} us ({0} calls)", n, watch.ElapsedMilliseconds * 100 * 1000 / n / 100.0M);
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

			//Assert.IsNull (entity.GetField<MyResourceStringEntity> (Epsitec.Common.Support.Res.Fields.TestInterface.Resource.ToString ()));

			Assert.AreEqual (0, proxy.ReadCounter);
			Assert.AreEqual (0, proxy.WriteCounter);
			Assert.AreEqual (0, proxy.DiscardCounter);
			
			//entity.InternalSetValue (Epsitec.Common.Support.Res.Fields.TestInterface.Resource.ToString (), proxy);

			Assert.AreEqual (0, proxy.ReadCounter);
			Assert.AreEqual (1, proxy.WriteCounter);
			Assert.AreEqual (0, proxy.DiscardCounter);

			//Assert.AreEqual (proxyData, entity.GetField<MyResourceStringEntity> (Epsitec.Common.Support.Res.Fields.TestInterface.Resource.ToString ()));
			//Assert.AreEqual (proxyData, entity.GetField<MyResourceStringEntity> (Epsitec.Common.Support.Res.Fields.TestInterface.Resource.ToString ()));
			
			Assert.AreEqual (2, proxy.ReadCounter);
			Assert.AreEqual (1, proxy.WriteCounter);
			Assert.AreEqual (0, proxy.DiscardCounter);

			//entity.InternalSetValue (Epsitec.Common.Support.Res.Fields.TestInterface.Resource.ToString (), proxy);
			
			Assert.AreEqual (2, proxy.ReadCounter);
			Assert.AreEqual (2, proxy.WriteCounter);
			Assert.AreEqual (1, proxy.DiscardCounter);
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

			public int DiscardCounter
			{
				get
				{
					return this.discardCounter;
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

			public bool DiscardWriteEntityValue(IValueStore store, string id, ref object value)
			{
				this.discardCounter++;
				return false;
			}

			public object PromoteToRealInstance()
			{
				return this.data;
			}

			#endregion

			private readonly object data;
			private int readCounter;
			private int writeCounter;
			private int discardCounter;
		}


		#region Fake EnumType Entity

		internal class MyEnumTypeEntity : AbstractEntity
		{
			public override Druid GetEntityStructuredTypeId()
			{
				return Epsitec.Common.Support.Res.Types.ResourceEnumType.CaptionId;
			}

			public override string GetEntityStructuredTypeKey()
			{
				return this.GetEntityStructuredTypeId ().ToString ();
			}
		}

		#endregion

		#region Fake EnumValue Entity

		internal class MyEnumValueEntity : AbstractEntity
		{
			public override Druid GetEntityStructuredTypeId()
			{
				return Epsitec.Common.Support.Res.Types.EnumValue.CaptionId;
			}

			public override string GetEntityStructuredTypeKey()
			{
				return this.GetEntityStructuredTypeId ().ToString ();
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
				//return Epsitec.Common.Support.Res.Types.TestInterfaceUser.CaptionId;
				throw new System.NotImplementedException ();
			}

			public override string GetEntityStructuredTypeKey()
			{
				return this.GetEntityStructuredTypeId ().ToString ();
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
				return Epsitec.Common.Support.Res.Types.ResourceString.CaptionId;
			}

			public override string GetEntityStructuredTypeKey()
			{
				return this.GetEntityStructuredTypeId ().ToString ();
			}
		}

		#endregion

		#region Fake ResourceCommand Entity

		internal class MyResourceCommandEntity : AbstractEntity
		{
			public override Druid GetEntityStructuredTypeId()
			{
				return Epsitec.Common.Support.Res.Types.ResourceCommand.CaptionId;
			}

			public override string GetEntityStructuredTypeKey()
			{
				return this.GetEntityStructuredTypeId ().ToString ();
			}
			
			public IList<MyShortcutEntity> Shortcuts
			{
				get
				{
					return this.GetFieldCollection<MyShortcutEntity> ("[700O]");
				}
			}
		}

		#endregion

		#region Fake Shortcut Entity

		internal class MyShortcutEntity : AbstractEntity
		{
			public override Druid GetEntityStructuredTypeId()
			{
				return Epsitec.Common.Support.Res.Types.Shortcut.CaptionId;
			}
			
			public override string GetEntityStructuredTypeKey()
			{
				return this.GetEntityStructuredTypeId ().ToString ();
			}
		}

		#endregion
	}
}
