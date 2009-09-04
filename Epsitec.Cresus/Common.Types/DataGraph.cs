//	Copyright © 2004-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
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
			string[] names = Support.Utilities.Split (path, '.');
			
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
			QueryResultCollection result = new QueryResultCollection ();
			string[]    names  = Support.Utilities.Split (query, '.');
			
			result.Select (names, start);
			
			return result;
		}
		
		
		public static bool Equal(IDataItem i1, IDataItem i2)
		{
			if (i1 == i2)
			{
				return true;
			}
			
			if ((i1 == null) ||
				(i2 == null))
			{
				return false;
			}
			
			if (i1.Name != i2.Name)
			{
				return false;
			}
			
			bool ok = false;

			IDataFolder f1 = i1 as IDataFolder;
			IDataFolder f2 = i2 as IDataFolder;
			
			if ((f1 != null) &&
				(f2 != null))
			{
				if (f1.Count != f2.Count)
				{
					return false;
				}
				
				int n = f1.Count;
				
				for (int i = 0; i < n; i++)
				{
					if (DataGraph.Equal (f1[i], f2[i]) == false)
					{
						return false;
					}
				}
				
				ok = true;
				
				//	Les noeuds pourraient aussi être des valeurs (IDataValue), alors on
				//	ne s'arrête pas immédiatement en cas de succès, mais on vérifie que
				//	les valeurs correspondent aussi, s'il y en a.
			}

			IDataValue v1 = i1 as IDataValue;
			IDataValue v2 = i2 as IDataValue;
			
			if ((v1 != null) &&
				(v2 != null))
			{
				object o1 = v1.ReadValue ();
				object o2 = v2.ReadValue ();
				
				return Comparer.Equal (o1, o2);
			}
			
			if (ok)
			{
				return true;
			}
			
			throw new System.InvalidOperationException (string.Format ("Cannot compare items of type {0} and {1}.", i1.GetType ().Name, i2.GetType ().Name));
		}
		
		public static int CopyValues(IDataItem src, IDataItem dst)
		{
			//	Copie les valeurs de la source à la destination. Cela ne va pas créer
			//	des noeuds et/ou feuilles supplémentaires dans la destination, mais
			//	uniquement copier les données qui existaient déjà dans la destination.
			
			//	Retourne le nombre valeurs copiées.
			
			int count = 0;
			
			DataGraph.CopyValues (src, dst, ref count);
			
			return count;
		}
		
		
		private static void CopyValues(IDataItem src, IDataItem dst, ref int count)
		{
			if ((src == null) ||
				(dst == null) ||
				(src == dst) ||
				(src.Name != dst.Name))
			{
				return;
			}

			IDataFolder f1 = src as IDataFolder;
			IDataFolder f2 = dst as IDataFolder;
			
			if ((f1 != null) &&
				(f2 != null))
			{
				int n = f1.Count;
				
				for (int i = 0; i < n; i++)
				{
					DataGraph.CopyValues (f1[i], f2[f1[i].Name], ref count);
				}
			}

			IDataValue v1 = src as IDataValue;
			IDataValue v2 = dst as IDataValue;
			
			if ((v1 != null) &&
				(v2 != null))
			{
				object o1 = v1.ReadValue ();
				object o2 = v2.ReadValue ();
				
				if (Comparer.Equal (o1, o2) == false)
				{
					v2.WriteValue (o1);
					count++;
				}
			}
		}
		
		
		#region QueryResult Class
		protected class QueryResultCollection : AbstractDataCollection
		{
			public QueryResultCollection()
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
				
				bool isLeaf = (i == n);
				
				if (name == "*")
				{
					//	Considérons tout d'abord le cas où "*" ne correspond à aucun élément :
					
					this.Select (query, offset+1, root);
					
					//	Ensuite, considérons le cas où "*" correspond à l'élément en cours, sans
					//	puis avec descente récursive.
					
					foreach (IDataItem item in root)
					{
						if (isLeaf)
						{
							if (this.Contains (item) == false)
							{
								this.Add (item);
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
						if (isLeaf)
						{
							if (this.Contains (item) == false)
							{
								this.Add (item);
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
						
						if (isLeaf)
						{
							if (this.Contains (item) == false)
							{
								this.Add (item);
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
						if (isLeaf)
						{
							if (this.Contains (item) == false)
							{
								this.Add (item);
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
				this.items = new IDataItem[this.Count];
				this.CopyTo (this.items, 0);
			}
			
			protected override IDataItem[] GetCachedItemArray()
			{
				return this.items;
			}
			
			
			protected override object CloneNewObject()
			{
				return new QueryResultCollection ();
			}
			
			
			private IDataItem[]					items;
		}
		#endregion
		
		private IDataFolder						root;
	}
}
