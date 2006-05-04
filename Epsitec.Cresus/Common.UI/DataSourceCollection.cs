//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;
using Epsitec.Common.Types;

namespace Epsitec.Common.UI
{
	public class DataSourceCollection : IStructuredTree, IStructuredData
	{
		public DataSourceCollection()
		{
		}

		public void AddDataSource(string name, IStructuredData source)
		{
			if (this.Contains (name))
			{
				throw new System.InvalidOperationException (string.Format ("Cannot insert duplicate data source with name '{0}'", name));
			}
			
			this.items.Add (new ItemRecord (name, source));
		}

		public bool Contains(string name)
		{
			ItemRecord record = this.GetItemRecord (name);

			return (record.Data == null) ? false : true;
		}

		#region IStructuredTree Members

		public string[] GetFieldNames()
		{
			string[] names = new string[this.items.Count];

			for (int i = 0; i < this.items.Count; i++)
			{
				names[i] = this.items[i].Name;
			}

			System.Array.Sort (names);
			
			return names;
		}

		public string[] GetFieldPaths(string path)
		{
			string rootName = StructuredTree.GetRootName (path);
			string nextPath = StructuredTree.GetSubPath (path, 1);
			
			IStructuredTree root = this.GetItemRecord (rootName).Data as IStructuredTree;

			if (root == null)
			{
				return null;
			}
			string[] paths;
			
			if (nextPath.Length == 0)
			{
				paths = root.GetFieldNames ();
			}
			else
			{
				paths = root.GetFieldPaths (nextPath);
			}

			for (int i = 0; i < paths.Length; i++)
			{
				paths[i] = StructuredTree.CreatePath (rootName, paths[i]);
			}

			return paths;
		}

		#endregion

		#region IStructuredData Members

		public void AttachListener(string path, Epsitec.Common.Support.EventHandler<DependencyPropertyChangedEventArgs> handler)
		{
			throw new System.Exception ("The method or operation is not implemented.");
		}

		public void DetachListener(string path, Epsitec.Common.Support.EventHandler<DependencyPropertyChangedEventArgs> handler)
		{
			throw new System.Exception ("The method or operation is not implemented.");
		}

		public object GetValue(string path)
		{
			throw new System.Exception ("The method or operation is not implemented.");
		}

		public void SetValue(string path, object value)
		{
			throw new System.Exception ("The method or operation is not implemented.");
		}

		#endregion

		protected ItemRecord GetItemRecord(string name)
		{
			foreach (ItemRecord record in this.items)
			{
				if (record.Name == name)
				{
					return record;
				}
			}

			return ItemRecord.Empty;
		}

		#region ItemRecord Structure

		protected struct ItemRecord
		{
			public ItemRecord(string name, IStructuredData data)
			{
				this.data = data;
				this.name = name;
				this.caption = name;
			}
			
			public IStructuredData Data
			{
				get
				{
					return this.data;
				}
			}

			public string Name
			{
				get
				{
					return this.name;
				}
			}

			public string Caption
			{
				get
				{
					return this.caption;
				}
				set
				{
					this.caption = value;
				}
			}
			
			public static readonly ItemRecord Empty = new ItemRecord ();

			private IStructuredData data;
			private string name;
			private string caption;
		}
		
		#endregion

		List<ItemRecord> items = new List<ItemRecord> ();
	}
}
