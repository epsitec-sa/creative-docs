//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.UI.Data
{
	/// <summary>
	/// La classe Record décrit un ensemble de champs utilisés pour échanger des
	/// données entre une application et son interface via mapper/binder/...
	/// </summary>
	public class Record : Types.AbstractDataCollection, Types.IDataFolder, Types.IDataGraph, Support.Data.IChangedSource
	{
		public Record()
		{
			this.graph = new Types.DataGraph (this);
		}
		
		
		public void Add(Field field)
		{
			this.list.Add (field);
			this.OnChanged ();
		}
		
		public void AddRange(System.Collections.ICollection fields)
		{
			if (fields.Count > 0)
			{
				foreach (object field in fields)
				{
					if (!(field is Field))
					{
						throw new System.ArgumentException ("Collection contains invalid field.", "fields");
					}
				}
				
				this.list.AddRange (fields);
				this.OnChanged ();
			}
		}
		
		public void AddGraph(Types.IDataGraph graph)
		{
			if ((graph == null) ||
				(graph.Root == null))
			{
				return;
			}
			
			int count = graph.Root.Count;
			
			if (count == 0)
			{
				return;
			}
			
			Types.IDataItem[]            items = new Types.IDataItem[count];
			System.Collections.ArrayList list  = new System.Collections.ArrayList ();
			
			graph.Root.CopyTo (items, 0);
			
			for (int i = 0; i < count; i++)
			{
				if (items[i].Classes == Types.DataItemClasses.Value)
				{
					list.Add (Field.CreateFromValue (items[i] as Types.IDataValue));
				}
			}
			
			this.AddRange (list);
		}
		
		public void Clear()
		{
			this.list.Clear ();
			this.OnChanged ();
		}
		
		
		public new Field						this[string name]
		{
			get
			{
				return base[name] as Field;
			}
		}

		public new Field						this[int index]
		{
			get
			{
				return base[index] as Field;
			}
		}
		
		
		#region IDataItem Members
		public Types.DataItemClasses			Classes
		{
			get
			{
				return Types.DataItemClasses.Folder;
			}
		}
		#endregion
		
		#region INameCaption Members
		public string							Description
		{
			get
			{
				return null;
			}
		}
		
		public string							Caption
		{
			get
			{
				return null;
			}
		}
		
		public string							Name
		{
			get
			{
				return null;
			}
		}
		#endregion
		
		#region IDataGraph Members
		public Types.IDataCollection Select(string query)
		{
			return this.graph.Select (query);
		}
		
		public Types.IDataFolder						Root
		{
			get
			{
				return this;
			}
		}
		
		public Types.IDataItem Navigate(string path)
		{
			return this.graph.Navigate (path);
		}
		#endregion
		
		#region IChangedSource Members
		public event Support.EventHandler		Changed;
		#endregion
		
		protected virtual void OnChanged()
		{
			this.ClearCachedItemArray ();
			
			if (this.Changed != null)
			{
				this.Changed (this);
			}
		}
		
		
		protected override Types.IDataItem[] GetCachedItemArray()
		{
			return this.fields;
		}
		
		protected override void ClearCachedItemArray()
		{
			base.ClearCachedItemArray ();
			
			this.fields = null;
		}
		
		protected override void UpdateCachedItemArray()
		{
			base.UpdateCachedItemArray ();
			
			this.fields = new Field[this.list.Count];
			this.list.CopyTo (this.fields);
		}

		
		private Field[]							fields;
		private Types.DataGraph					graph;
	}
}
