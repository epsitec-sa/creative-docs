//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : OK/PA, 26/04/2004

namespace Epsitec.Common.UI.Data
{
	/// <summary>
	/// La classe Record décrit un ensemble de champs utilisés pour échanger des
	/// données entre une application et son interface via mapper/binder/...
	/// </summary>
	public class Record : Types.AbstractDataCollection, Types.IDataFolder, Types.IDataGraph
	{
		public Record()
		{
			this.graph = new Types.DataGraph (this);
		}
		
		
		public void Add(Field field)
		{
			this.list.Add (field);
			this.ClearCachedItemArray ();
		}
		
		public void AddRange(System.Collections.ICollection fields)
		{
			foreach (object field in fields)
			{
				if (!(field is Field))
				{
					throw new System.ArgumentException ("Collection contains invalid field.", "fields");
				}
			}
			
			this.list.AddRange (fields);
			this.ClearCachedItemArray ();
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
		Types.IDataCollection Types.IDataGraph.Select(string query)
		{
			return this.graph.Select (query);
		}
		
		Types.IDataFolder						Types.IDataGraph.Root
		{
			get
			{
				return this;
			}
		}
		
		Types.IDataItem Types.IDataGraph.Navigate(string path)
		{
			return this.graph.Navigate (path);
		}
		#endregion
		
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
