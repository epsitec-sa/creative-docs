//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Types
{
	/// <summary>
	/// La classe DataGraph fournit une implémentation de base de
	/// l'interface IDataGraph
	/// </summary>
	public class DataGraph : IDataGraph
	{
		public DataGraph()
		{
		}
		
		public DataGraph(IDataFolder root) : this ()
		{
			this.DefineRoot (root);
		}
		
		
		public void DefineRoot(IDataFolder folder)
		{
			this.root = folder;
		}
		
		
		#region IDataGraph Members
		public IDataFolder						Root
		{
			get
			{
				return this.root;
			}
		}

		public IDataItem Navigate(string path)
		{
			return DataGraph.Navigate (path, this.Root);
		}
		
		public IDataCollection Select(string query)
		{
			return DataGraph.Select (query, this.Root);
		}
		#endregion
		
		public static IDataItem Navigate(string path, IDataFolder start)
		{
			string[] names = System.Utilities.Split (path, '.');
			
			IDataFolder root;
			IDataItem   item = start;
			
			for (int i = 0; i < names.Length; i++)
			{
				string name = names[i];
				
				if ((item.Classes & DataItemClasses.Folder) == 0)
				{
					throw new System.ArgumentOutOfRangeException ("path", path, string.Format ("Path element {0} does not refer to a folder.", i-1));
				}
				
				root = item as IDataFolder;
				
				System.Diagnostics.Debug.Assert (root != null);
				
				if ((name.Length > 2) &&
					(name[0] == '[') &&
					(name[name.Length-1] == ']'))
				{
					int index = System.Int32.Parse (name.Substring (1, name.Length-2), System.Globalization.CultureInfo.InvariantCulture);
					int count = root.Count;
					
					if ((index >= 0) &&
						(index < count))
					{
						item = root[index];
					}
					else
					{
						throw new System.ArgumentOutOfRangeException ("path", path, string.Format ("Path element {0} (index {1}) is not valid.", i, index));
					}
				}
				else
				{
					item = root[name];
					
					if (item == null)
					{
						throw new System.ArgumentOutOfRangeException ("path", path, string.Format ("Path element {0} ({1}) is not valid.", i, name));
					}
				}
				
				System.Diagnostics.Debug.Assert (item != null);
			}
			
			return item;
		}
		
		public static IDataCollection Select(string query, IDataFolder start)
		{
			QueryResult result = new QueryResult ();
			string[]    names  = System.Utilities.Split (query, '.');
			
			result.Select (names, start);
			
			return result;
		}
		
		
		#region QueryResult Class
		protected class QueryResult : AbstractDataCollection
		{
			public QueryResult()
			{
			}
			
			
			public void Select(string[] query, IDataFolder root)
			{
				this.Select (query, 0, root);
				this.ClearCachedItemArray ();
			}
			
			
			protected void Select(string[] query, int offset, IDataFolder root)
			{
				int n = query.Length;
				
				if ((offset >= n) ||
					(root == null))
				{
					return;
				}
				
				string  name = query[offset];
				
				int i = offset;
				
				while (++i < n)
				{
					if (query[i] != "*")
					{
						break;
					}
				}
				
				bool is_leaf = (i == n);
				
				if (name == "*")
				{
					//	Considérons tout d'abord le cas où "*" ne correspond à aucun élément :
					
					this.Select (query, offset+1, root);
					
					//	Ensuite, considérons le cas où "*" correspond à l'élément en cours, sans
					//	puis avec descente récursive.
					
					foreach (IDataItem item in root)
					{
						if (is_leaf)
						{
							if (this.list.Contains (item) == false)
							{
								this.list.Add (item);
							}
						}
						
						if ((item.Classes & DataItemClasses.Folder) != 0)
						{
							this.Select (query, offset+1, item as IDataFolder);		//	sans descente récursive
							this.Select (query, offset, item as IDataFolder);		//	avec descente récursive
						}
					}
					
					return;
				}
				
				if (name == "?")
				{
					foreach (IDataItem item in root)
					{
						if (is_leaf)
						{
							if (this.list.Contains (item) == false)
							{
								this.list.Add (item);
							}
						}
						
						if ((item.Classes & DataItemClasses.Folder) != 0)
						{
							this.Select (query, offset+1, item as IDataFolder);
						}
					}
					
					return;
				}
				
				if ((name.Length > 2) &&
					(name[0] == '[') &&
					(name[name.Length-1] == ']'))
				{
					int index = System.Int32.Parse (name.Substring (1, name.Length-2), System.Globalization.CultureInfo.InvariantCulture);
					int count = root.Count;
						
					if ((index >= 0) &&
						(index < count))
					{
						IDataItem item = root[index];
						
						if (is_leaf)
						{
							if (this.list.Contains (item) == false)
							{
								this.list.Add (item);
							}
						}
						
						if ((item.Classes & DataItemClasses.Folder) != 0)
						{
							this.Select (query, offset+1, item as IDataFolder);
						}
					}
				}
				else
				{
					IDataItem item = root[name];
					
					if (item != null)
					{
						if (is_leaf)
						{
							if (this.list.Contains (item) == false)
							{
								this.list.Add (item);
							}
						}
						
						if ((item.Classes & DataItemClasses.Folder) != 0)
						{
							this.Select (query, offset+1, item as IDataFolder);
						}
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
			
			
			private IDataItem[]					items;
		}
		#endregion
		
		private IDataFolder						root;
	}
}
