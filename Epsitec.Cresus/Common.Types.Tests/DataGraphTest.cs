using NUnit.Framework;

namespace Epsitec.Common.Types
{
	[TestFixture] public class DataGraphTest
	{
		[Test] public void CheckNavigate()
		{
			DataGraph graph = new DataGraph ();
			Folder    root  = new Folder ("/");
			
			string[] names = { "a", "b", "c/x/one", "c/x/two", "c/y/three", "test", "long item name" };
			
			for (int i = 0; i < names.Length; i++)
			{
				root.CreateValue (names[i]);
			}
			
			graph.DefineRoot (root);
			
			Assert.AreEqual (5, root.Count);
			Assert.AreEqual ("a", root[0].Name);
			Assert.AreEqual ("b", root[1].Name);
			Assert.AreEqual ("c", root[2].Name);
			Assert.AreEqual ("test", root[3].Name);
			Assert.AreEqual ("long item name", root[4].Name);
			
			Assert.AreEqual ("x", graph.Navigate ("c.x").Name);
			Assert.AreEqual ("x", graph.Navigate ("c.[0]").Name);
			Assert.AreEqual ("y", graph.Navigate ("c.[1]").Name);
			Assert.AreEqual ("one", graph.Navigate ("c.[0].[0]").Name);
			Assert.AreEqual ("two", graph.Navigate ("c.[0].[1]").Name);
			Assert.AreEqual ("three", graph.Navigate ("c.[1].three").Name);
		}
		
		[Test] public void CheckQuery()
		{
			DataGraph graph = new DataGraph ();
			Folder    root  = new Folder ("/");
			
			string[] names = { "a", "b", "c/x/one", "c/x/two", "c/y/three", "test", "long item name" };
			
			for (int i = 0; i < names.Length; i++)
			{
				root.CreateValue (names[i]);
			}
			
			graph.DefineRoot (root);
			int index;
			
			string[] result_1 = { "a", "b", "c", "x", "one", "two", "y", "three", "test", "long item name" };
			string[] result_2 = { "a", "b", "c", "test", "long item name" };
			string[] result_3 = { "x", "y" };
			string[] result_4 = { "x", "one", "two", "y", "three" };
			string[] result_5 = { "x" };
			string[] result_6 = { "one", "two" };
			string[] result_7 = { "a", "x", "one", "two", "three" };
			
			index = 0;
			foreach (IDataItem item in graph.Select ("*"))
			{
				Assert.AreEqual (result_1[index++], item.Name);
			}
			
			index = 0;
			foreach (IDataItem item in graph.Select ("*.*"))
			{
				Assert.AreEqual (result_1[index++], item.Name);
			}
			
			index = 0;
			foreach (IDataItem item in graph.Select ("?"))
			{
				Assert.AreEqual (result_2[index++], item.Name);
			}
			
			index = 0;
			foreach (IDataItem item in graph.Select ("?.?"))
			{
				Assert.AreEqual (result_3[index++], item.Name);
			}
			
			index = 0;
			foreach (IDataItem item in graph.Select ("?.?.*"))
			{
				Assert.AreEqual (result_4[index++], item.Name);
			}
			
			index = 0;
			foreach (IDataItem item in graph.Select ("c.?.*"))
			{
				Assert.AreEqual (result_4[index++], item.Name);
			}
			
			index = 0;
			foreach (IDataItem item in graph.Select ("*.x"))
			{
				Assert.AreEqual (result_5[index++], item.Name);
			}
			
			index = 0;
			foreach (IDataItem item in graph.Select ("*.x.?.*"))
			{
				Assert.AreEqual (result_6[index++], item.Name);
			}
			
			index = 0;
			foreach (IDataItem item in graph.Select ("*.[0].*"))
			{
				Assert.AreEqual (result_7[index++], item.Name);
			}
		}
		
		[Test] public void CheckClone()
		{
			Folder root_1 = new Folder ("/");
			
			string[] names = { "a", "b", "c/x/one", "c/x/two", "c/y/three", "test", "long item name" };
			
			for (int i = 0; i < names.Length; i++)
			{
				root_1.CreateValue (names[i]);
			}
			
			Folder root_2 = root_1.Clone ();
			
			DataGraphTest.CompareFolders (root_1, root_2);
			
			Assert.IsTrue (DataGraph.Equal (root_1, root_2));
		}
		
		[Test] public void CheckEqual()
		{
			Folder root_1 = new Folder ("/");
			Folder root_2 = new Folder ("/");
			Folder root_3 = new Folder ("/");
			
			string[] names_1 = { "a", "b", "c/x/one", "c/x/two", "c/y/three", "test", "long item name" };
			string[] names_2 = { "a", "b", "c/x/one", "c/x/two", "c/y/four", "test", "long item name" };
			string[] names_3 = { "a", "b", "c/x/one", "c/x/two", "c/z", "test", "long item name" };
			
			for (int i = 0; i < names_1.Length; i++)
			{
				root_1.CreateValue (names_1[i]);
			}
			for (int i = 0; i < names_2.Length; i++)
			{
				root_2.CreateValue (names_2[i]);
			}
			for (int i = 0; i < names_3.Length; i++)
			{
				root_3.CreateValue (names_3[i]);
			}
			
			Assert.IsFalse (DataGraph.Equal (root_1, root_2));
			Assert.IsFalse (DataGraph.Equal (root_1, root_3));
		}		
		
		[Test] public void CheckCopyValues()
		{
			Folder root_1 = new Folder ("/");
			Folder root_2 = new Folder ("/");
			Folder root_3 = new Folder ("/");
			
			Value v1 = new Value ("a");
			Value v2 = new Value ("b");
			Value v3 = new Value ("c");
			
			v1.WriteValue (1);	root_1.Add (v1.Clone () as IDataItem);
			v1.WriteValue (2);	root_2.Add (v1.Clone () as IDataItem);
			v1.WriteValue (5);	root_3.Add (v1.Clone () as IDataItem);
			
			v2.WriteValue ("X"); //root_1.Add (v2.Clone () as IDataItem);
			v2.WriteValue ("Y"); root_2.Add (v2.Clone () as IDataItem);
			v2.WriteValue ("Z"); root_3.Add (v2.Clone () as IDataItem);
			
			v3.WriteValue (1.0); root_1.Add (v3.Clone () as IDataItem);
			v3.WriteValue (2.0); //root_2.Add (v3.Clone () as IDataItem);
			v3.WriteValue (5.0); root_3.Add (v3.Clone () as IDataItem);
			
			Assert.IsFalse (DataGraph.Equal (root_1, root_2));
			Assert.IsFalse (DataGraph.Equal (root_1, root_3));
			Assert.IsFalse (DataGraph.Equal (root_2, root_3));
			
			Assert.AreEqual (1, DataGraph.CopyValues (root_1, root_2));
			Assert.AreEqual (2, DataGraph.CopyValues (root_1, root_3));
			Assert.AreEqual (1, DataGraph.CopyValues (root_2, root_3));
		}		
		
		
		
		private static void CompareFolders(IDataFolder f1, IDataFolder f2)
		{
			//	Vérifie que les dossiers sont identiques... récursivement.
			
			Assert.AreEqual (f1.Name, f2.Name);
			Assert.AreEqual (f1.CaptionId, f2.CaptionId);
			Assert.AreEqual (f1.Count, f2.Count);
			
			int n = f1.Count;
			
			for (int i = 0; i < n; i++)
			{
				Assert.AreEqual (f1[i].Name, f2[i].Name);
				Assert.AreEqual (f1[i].GetType (), f2[i].GetType ());
				
				if (f1[i] is Folder)
				{
					DataGraphTest.CompareFolders (f1[i] as Folder, f2[i] as Folder);
				}
			}
		}
		
		
		private class Folder : AbstractDataCollection, IDataFolder
		{
			public Folder(string name)
			{
				this.name = name;
			}
			
			
			public void CreateValue(string path)
			{
				string[] elems = path.Split ('/');
				Folder   root  = this;
				
				for (int i = 0; i < elems.Length; i++)
				{
					string name = elems[i];
					
					if (i == elems.Length-1)
					{
						root.Add (new Value (name));
					}
					else
					{
						Folder folder = null;
						
						for (int j = 0; j < root.Count; j++)
						{
							if (root[j].Name == name)
							{
								folder = root[j] as Folder;
								break;
							}
						}
						
						if (folder == null)
						{
							folder = new Folder (name);
							root.Add (folder);
						}
						
						root = folder;
					}
				}
			}
			
			public Folder Clone()
			{
				System.ICloneable cloneable = this;
				return cloneable.Clone () as Folder;
			}
			
			
			protected override void ClearCachedItemArray()
			{
				base.ClearCachedItemArray ();
				this.items = null;
			}
			
			protected override void UpdateCachedItemArray()
			{
				base.UpdateCachedItemArray ();
				
				this.items = new IDataItem[this.Count];
				this.CopyTo (this.items, 0);
			}
			
			protected override IDataItem[] GetCachedItemArray()
			{
				return this.items;
			}

			
			#region IDataItem Members
			public DataItemClasses				Classes
			{
				get
				{
					return DataItemClasses.Folder;
				}
			}
			#endregion
			
			#region INameCaption Members
			public Support.Druid				CaptionId
			{
				get
				{
					return Support.Druid.Empty;
				}
			}

			public string						Name
			{
				get
				{
					return this.name;
				}
			}

			#endregion
			
			protected override object CloneNewObject()
			{
				return new Folder (this.name);
			}
			
			
			private string						name;
			private IDataItem[]					items;
		}

		private class Value : IDataValue
		{
			public Value(string name)
			{
				this.name = name;
			}
			
			
			#region IDataValue Members
			public INamedType					DataType
			{
				get
				{
					return null;
				}
			}
			
			public IDataConstraint				DataConstraint
			{
				get
				{
					return null;
				}
			}
			
			public bool							IsValueValid
			{
				get
				{
					return true;
				}
			}
			
			public object ReadValue()
			{
				return this.value;
			}
			
			public void WriteValue(object value)
			{
				this.value = value;
			}
			
			public void NotifyInvalidData()
			{
			}
			
			
			public event Support.EventHandler	Changed;
			#endregion

			#region IDataItem Members
			public DataItemClasses				Classes
			{
				get
				{
					return DataItemClasses.Value;
				}
			}
			#endregion

			#region INameCaption Members
			public Support.Druid				CaptionId
			{
				get
				{
					return Support.Druid.Empty;
				}
			}

			public string						Name
			{
				get
				{
					return this.name;
				}
			}
			#endregion
			
			#region ICloneable Members
			public object Clone()
			{
				Value that = new Value (this.name);
				
				that.value = this.value;
				
				return that;
			}
			#endregion
			
			protected void OnChanged()
			{
				if (this.Changed != null)
				{
					this.Changed (this);
				}
			}
			
			
			private string						name;
			private object						value;
		}
	}
}
