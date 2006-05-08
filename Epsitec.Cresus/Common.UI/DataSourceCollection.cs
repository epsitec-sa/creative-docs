//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;
using Epsitec.Common.Types;

namespace Epsitec.Common.UI
{
	/// <summary>
	/// The <c>DataSourceCollection</c> class provides the list of named objects
	/// to which a user interface can be data bound.
	/// </summary>
	public class DataSourceCollection : IStructuredTree, IStructuredData
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="T:DataSourceCollection"/> class.
		/// </summary>
		public DataSourceCollection()
		{
		}

		/// <summary>
		/// Adds a named data source.
		/// </summary>
		/// <param name="name">The name of the datasource.</param>
		/// <param name="source">The datasource.</param>
		public void AddDataSource(string name, IStructuredData source)
		{
			if (this.Contains (name))
			{
				throw new System.InvalidOperationException (string.Format ("Cannot insert duplicate data source with name '{0}'", name));
			}
			
			this.items.Add (new ItemRecord (name, source));
		}

		/// <summary>
		/// Determines whether the collection contains the named datasource.
		/// </summary>
		/// <param name="name">The name of the datasource to get.</param>
		/// <returns>
		/// 	<c>true</c> if the collection contains the specified named datasource; otherwise, <c>false</c>.
		/// </returns>
		public bool Contains(string name)
		{
			ItemRecord record = this.GetItemRecord (name);

			return (record.IsEmpty) ? false : true;
		}

		/// <summary>
		/// Gets the data source for the specified name.
		/// </summary>
		/// <param name="name">The name of the data source.</param>
		/// <returns>The data source or <c>null</c> if it is not known.</returns>
		public IStructuredData GetDataSource(string name)
		{
			ItemRecord record = this.GetItemRecord (name);

			if (record.IsEmpty)
			{
				return null;
			}
			else
			{
				return record.Data;
			}
		}

		/// <summary>
		/// Fills the serialization context ExternalMap property.
		/// </summary>
		/// <param name="context">The serialization context.</param>
		public void FillSerializationContext(Types.Serialization.Context context)
		{
			foreach (ItemRecord record in this.items)
			{
				context.ExternalMap.Record (record.Name, record.Data);
			}
		}

		#region IStructuredTree Members

		/// <summary>
		/// Gets the field names.
		/// </summary>
		/// <returns>Array of field names.</returns>
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

		/// <summary>
		/// Gets the field paths of the fields found at the specified path.
		/// </summary>
		/// <param name="path">The path to search.</param>
		/// <returns>Array of field paths.</returns>
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

		/// <summary>
		/// Attaches the listener for the specified path.
		/// </summary>
		/// <param name="path">The path.</param>
		/// <param name="handler">The handler.</param>
		public void AttachListener(string path, Epsitec.Common.Support.EventHandler<DependencyPropertyChangedEventArgs> handler)
		{
			string name = StructuredTree.GetRootName (path);
			
			ItemRecord record = this.GetItemRecord (name);
			IStructuredData data = record.Data;

			if (data == null)
			{
				throw new System.ArgumentException (string.Format ("Path '{0}' cannot be resolved", path));
			}
			
			data.AttachListener (StructuredTree.GetSubPath (path, 1), handler);
		}

		/// <summary>
		/// Detaches the listener for the specified path.
		/// </summary>
		/// <param name="path">The path.</param>
		/// <param name="handler">The handler.</param>
		public void DetachListener(string path, Epsitec.Common.Support.EventHandler<DependencyPropertyChangedEventArgs> handler)
		{
			string name = StructuredTree.GetRootName (path);

			ItemRecord record = this.GetItemRecord (name);
			IStructuredData data = record.Data;

			if (data == null)
			{
				throw new System.ArgumentException (string.Format ("Path '{0}' cannot be resolved", path));
			}

			data.DetachListener (StructuredTree.GetSubPath (path, 1), handler);
		}

		/// <summary>
		/// Gets the value for the specified path.
		/// </summary>
		/// <param name="path">The path.</param>
		/// <returns>The value for the specified path.</returns>
		public object GetValue(string path)
		{
			string name = StructuredTree.GetRootName (path);

			ItemRecord record = this.GetItemRecord (name);
			IStructuredData data = record.Data;

			if (data == null)
			{
				throw new System.ArgumentException (string.Format ("Path '{0}' cannot be resolved", path));
			}

			return data.GetValue (StructuredTree.GetSubPath (path, 1));
		}

		/// <summary>
		/// Sets the value for the specified path.
		/// </summary>
		/// <param name="path">The path.</param>
		/// <param name="value">The value.</param>
		public void SetValue(string path, object value)
		{
			string name = StructuredTree.GetRootName (path);

			ItemRecord record = this.GetItemRecord (name);
			IStructuredData data = record.Data;

			if (data == null)
			{
				throw new System.ArgumentException (string.Format ("Path '{0}' cannot be resolved", path));
			}

			data.SetValue (StructuredTree.GetSubPath (path, 1), value);
		}

		#endregion

		/// <summary>
		/// Gets the item record for the specified name.
		/// </summary>
		/// <param name="name">The name of the item record to get.</param>
		/// <returns>The item record for the name.</returns>
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

		/// <summary>
		/// The <c>ItemRecord</c> structure stores a name and a structured data.
		/// </summary>
		protected struct ItemRecord
		{
			/// <summary>
			/// Initializes a new instance of the <see cref="T:ItemRecord"/> structure.
			/// </summary>
			/// <param name="name">The name of the item.</param>
			/// <param name="data">The data of the item.</param>
			public ItemRecord(string name, IStructuredData data)
			{
				this.data = data;
				this.name = name;
				this.caption = name;
			}

			/// <summary>
			/// Gets the datasource.
			/// </summary>
			/// <value>The datasource.</value>
			public IStructuredData Data
			{
				get
				{
					return this.data;
				}
			}

			/// <summary>
			/// Gets the name of the datasource.
			/// </summary>
			/// <value>The name of the datasource.</value>
			public string Name
			{
				get
				{
					return this.name;
				}
			}

			/// <summary>
			/// Gets or sets the caption of the datasource.
			/// </summary>
			/// <value>The caption of the datasource.</value>
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

			/// <summary>
			/// Gets a value indicating whether this record is empty.
			/// </summary>
			/// <value><c>true</c> if this record is empty; otherwise, <c>false</c>.</value>
			public bool IsEmpty
			{
				get
				{
					return this.data == null;
				}
			}
			
			/// <summary>
			/// Empty item record.
			/// </summary>
			public static readonly ItemRecord Empty = new ItemRecord ();

			private IStructuredData data;
			private string name;
			private string caption;
		}
		
		#endregion

		List<ItemRecord> items = new List<ItemRecord> ();
	}
}
