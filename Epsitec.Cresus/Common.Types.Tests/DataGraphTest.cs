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
		
		
		
		private class Folder : AbstractDataCollection, IDataFolder
		{
			public Folder(string name)
			{
				this.name = name;
			}
			
			
			public void Add(IDataItem item)
			{
				this.list.Add (item);
				this.ClearCachedItemArray ();
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
			
			protected override void ClearCachedItemArray()
			{
				base.ClearCachedItemArray ();
				this.items = null;
			}
			
			protected override void UpdateCachedItemArray()
			{
				base.UpdateCachedItemArray ();
				
				this.items = new IDataItem[this.list.Count];
				this.list.CopyTo (this.items);
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
			public string						Description
			{
				get
				{
					return null;
				}
			}

			public string						Caption
			{
				get
				{
					return null;
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
			
			public object ReadValue()
			{
				return null;
			}
			
			public void WriteValue(object value)
			{
			}
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
			public string						Description
			{
				get
				{
					return null;
				}
			}

			public string						Caption
			{
				get
				{
					return null;
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
			
			private string						name;
		}
	}
}
