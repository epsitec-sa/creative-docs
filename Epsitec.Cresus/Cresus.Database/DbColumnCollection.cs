//	Copyright © 2003-2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : OK/PA, 07/10/2003

namespace Epsitec.Cresus.Database
{
	/// <summary>
	/// La classe DbColumnCollection encapsule une collection d'instances de type DbColumn.
	/// </summary>
	public class DbColumnCollection : InternalCollectionList
	{
		public DbColumnCollection()
		{
		}
		
		public DbColumnCollection(System.Xml.XmlElement xml)
		{
			this.ProcessXmlDefinition (xml);
		}
		
		
		public virtual void Add(DbColumn column)
		{
			this.List.Add (column);
			this.OnChanged ();
		}

		public virtual void AddRange(DbColumn[] columns)
		{
			if (columns == null)
			{
				return;
			}
			
			this.List.AddRange (columns);
			this.OnChanged ();
		}
		
		public virtual void Remove(DbColumn column)
		{
			this.List.Remove (column);
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
			for (int i = 0; i < this.List.Count; i++)
			{
				DbColumn column = this.List[i] as DbColumn;
				
				if (column.Name == column_name)
				{
					return i;
				}
			}
			
			return -1;
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
		
		
		public static DbColumnCollection NewColumn(string xml)
		{
			System.Xml.XmlDocument doc = new System.Xml.XmlDocument ();
			doc.LoadXml (xml);
			return DbColumnCollection.NewColumnCollection (doc.DocumentElement);
		}
		
		public static DbColumnCollection NewColumnCollection(System.Xml.XmlElement xml)
		{
			return (xml.Name == "null") ? null : new DbColumnCollection (xml);
		}
		
		
		public static string SerialiseToXml(DbColumnCollection columns, string id)
		{
			System.Text.StringBuilder buffer = new System.Text.StringBuilder ();
			DbColumnCollection.SerialiseToXml (buffer, columns, id);
			return buffer.ToString ();
		}
		
		public static void SerialiseToXml(System.Text.StringBuilder buffer, DbColumnCollection columns, string id)
		{
			if (columns == null)
			{
				buffer.Append (@"<null id=""");
				buffer.Append (System.Utilities.TextToXml (id));
				buffer.Append (@"""/>");
			}
			else
			{
				columns.SerialiseXmlDefinition (buffer, id);
			}
		}
		
		
		protected void SerialiseXmlDefinition(System.Text.StringBuilder buffer, string id)
		{
			buffer.Append (@"<cols id=""");
			buffer.Append (System.Utilities.TextToXml (id));
			buffer.Append (@""">");
			
			for (int i = 0; i < this.list.Count; i++)
			{
				DbColumn.SerialiseToXml (buffer, this[i], true);
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
				this.Add (DbColumn.NewColumn (xml.ChildNodes[i] as System.Xml.XmlElement));
			}
		}
	}
}
