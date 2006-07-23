//	Copyright © 2003-2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Cresus.Database.Collections
{
	/// <summary>
	/// La classe DbColumns encapsule une collection d'instances de type DbColumn.
	/// </summary>
	public class DbColumns : AbstractList
	{
		public DbColumns()
		{
		}
		
		public DbColumns(System.Xml.XmlElement xml)
		{
			this.ProcessXmlDefinition (xml);
		}
		
		
		public virtual DbColumn				this[int index]
		{
			get
			{
				return this.List[index] as DbColumn;
			}
		}
		
		public virtual DbColumn				this[string column_name]
		{
			get
			{
				int index = this.IndexOf (column_name);
				
				if (index >= 0)
				{
					return this[index];
				}
				
				return null;
			}
		}
		
		public virtual DbColumn				this[string column_name, DbColumnClass column_class]
		{
			get
			{
				int index = this.IndexOf (column_name);
				
				while (index >= 0)
				{
					if (this[index].ColumnClass == column_class)
					{
						return this[index];
					}
					
					index = this.IndexOf (column_name, index+1);
				}
				
				return null;
			}
		}
		
		
		public virtual void Add(DbColumn column)
		{
			this.InternalAdd (column);
		}

		public virtual void AddRange(DbColumn[] columns)
		{
			this.InternalAddRange (columns);
		}
		
		public virtual void Remove(DbColumn column)
		{
			this.InternalRemove (column);
			this.OnChanged ();
		}
		
		
		public virtual bool Contains(DbColumn column)
		{
			return this.List.Contains (column);
		}
		
		public virtual int IndexOf(DbColumn column)
		{
			return this.List.IndexOf (column);
		}
		
		public override int IndexOf(string column_name)
		{
			return this.IndexOf (column_name, 0);
		}
		
		public virtual int IndexOf(string column_name, int start)
		{
			if (start >= 0)
			{
				for (int i = start; i < this.List.Count; i++)
				{
					DbColumn column = this.List[i] as DbColumn;
					
					if (column.Name == column_name)
					{
						return i;
					}
				}
			}
			
			return -1;
		}
		
		
		public static DbColumns CreateCollection(string xml)
		{
			System.Xml.XmlDocument doc = new System.Xml.XmlDocument ();
			doc.LoadXml (xml);
			return DbColumns.CreateCollection (doc.DocumentElement);
		}
		
		public static DbColumns CreateCollection(System.Xml.XmlElement xml)
		{
			return (xml.Name == "null") ? null : new DbColumns (xml);
		}
		
		
		public static string SerializeToXml(DbColumns columns, string id)
		{
			System.Text.StringBuilder buffer = new System.Text.StringBuilder ();
			DbColumns.SerializeToXml (buffer, columns, id);
			return buffer.ToString ();
		}
		
		public static void SerializeToXml(System.Text.StringBuilder buffer, DbColumns columns, string id)
		{
			if (columns == null)
			{
				buffer.Append (@"<null id=""");
				buffer.Append (Common.Support.Utilities.TextToXml (id));
				buffer.Append (@"""/>");
			}
			else
			{
				columns.SerializeXmlDefinition (buffer, id);
			}
		}
		
		
		protected void SerializeXmlDefinition(System.Text.StringBuilder buffer, string id)
		{
			buffer.Append (@"<cols id=""");
			buffer.Append (Common.Support.Utilities.TextToXml (id));
			buffer.Append (@""">");
			
			for (int i = 0; i < this.list.Count; i++)
			{
				DbColumn.SerializeToXml (buffer, this[i], true);
			}
			
			buffer.Append (@"</cols>");
		}
		
		protected void ProcessXmlDefinition(System.Xml.XmlElement xml)
		{
			if (xml.Name != "cols")
			{
				throw new System.FormatException (string.Format ("Expected root element named <cols>, but found <{0}>.", xml.Name));
			}
			
			for (int i = 0; i < xml.ChildNodes.Count; i++)
			{
				this.Add (DbColumn.CreateColumn (xml.ChildNodes[i] as System.Xml.XmlElement));
			}
		}
	}
}
