//	Copyright © 2003-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Cresus.Database
{
	using IEnumType  = Common.Types.IEnumType;
	using IEnumValue = Common.Types.IEnumValue;
	
	/// <summary>
	/// La classe DbTypeEnum définit une énumération.
	/// </summary>
	public class DbTypeEnum : DbType, System.Collections.ICollection, IEnumType
	{
		public DbTypeEnum() : base (DbSimpleType.String)
		{
		}
		
		public DbTypeEnum(params string[] attributes) : base (DbSimpleType.String, attributes)
		{
		}
		
		public DbTypeEnum(System.Collections.ICollection values, params string[] attributes) : this (attributes)
		{
			this.DefineValues (values);
		}
		
		
		public DbEnumValue						this[int rank]
		{
			get
			{
				for (int i = 0; i < this.values.Length; i++)
				{
					if (this.values[i].Rank == rank)
					{
						return this.values[i];
					}
				}
				
				return null;
			}
		}
		
		public DbEnumValue						this[string name]
		{
			get
			{
				for (int i = 0; i < this.values.Length; i++)
				{
					if (this.values[i].Name == name)
					{
						return this.values[i];
					}
				}
				
				return null;
			}
		}
		
		public IEnumerable<DbEnumValue>			Values
		{
			get
			{
				return this.values;
			}
		}
		
		
		public int								MaxNameLength
		{
			get { return this.max_name_length; }
		}
		
		
		#region IEnum Members
		bool									IEnumType.IsCustomizable
		{
			get
			{
				return false;
			}
		}
		
		bool									IEnumType.IsDefinedAsFlags
		{
			get
			{
				return false;
			}
		}
		
		IEnumerable<IEnumValue>					IEnumType.Values
		{
			get
			{
				foreach (IEnumValue item in this.Values)
				{
					yield return item;
				}
			}
		}
		
		IEnumValue								IEnumType.this[int rank]
		{
			get
			{
				return this[rank];
			}
		}
		
		IEnumValue								IEnumType.this[string name]
		{
			get
			{
				return this[name];
			}
		}
		#endregion
		
		public System.Type						SystemType
		{
			get
			{
				return typeof (string);
			}
		}
		
		
		internal override void SerializeXmlAttributes(System.Text.StringBuilder buffer, bool full)
		{
			buffer.Append (@" nmlen=""");
			buffer.Append (this.max_name_length.ToString (System.Globalization.CultureInfo.InvariantCulture));
			buffer.Append (@"""");
			
			base.SerializeXmlAttributes (buffer, full);
		}
		
		internal override void SerializeXmlElements(System.Text.StringBuilder buffer, bool full)
		{
			base.SerializeXmlElements (buffer, full);
			
			if (this.values != null)
			{
				for (int i = 0; i < this.values.Length; i++)
				{
					DbEnumValue.SerializeToXml (buffer, this.values[i], full);
				}
			}
		}
		
		internal override void DeserializeXmlAttributes(System.Xml.XmlElement xml)
		{
			base.DeserializeXmlAttributes (xml);
			
			string arg_nmlen = xml.GetAttribute ("nmlen");
			
			if (arg_nmlen.Length > 0)
			{
				this.max_name_length = System.Int32.Parse (arg_nmlen, System.Globalization.CultureInfo.InvariantCulture);
			}
		}

		internal override void DeserializeXmlElements(System.Xml.XmlNodeList nodes, ref int index)
		{
			base.DeserializeXmlElements (nodes, ref index);
			
			System.Collections.ArrayList list = new System.Collections.ArrayList ();
			
			while ((index < nodes.Count) && (nodes[index].Name == "enumval"))
			{
				System.Xml.XmlElement node = nodes[index++] as System.Xml.XmlElement;
				list.Add (DbEnumValue.CreateEnumValue (node));
			}
			
			this.values = new DbEnumValue[list.Count];
			list.CopyTo (this.values);
		}
		
		
		internal void DefineValues(System.Collections.ICollection values)
		{
			DbEnumValue[] temp = new DbEnumValue[values.Count];
			values.CopyTo (temp, 0);
			System.Array.Sort (temp, DbEnumValue.RankComparer);
			
			if (System.Utilities.CheckForDuplicates (temp, DbEnumValue.NameComparer))
			{
				throw new System.ArgumentException ("Duplicates found.", "values");
			}
			
			this.CopyValues (temp);
		}
		
		
		protected override object CloneNewObject()
		{
			return new DbTypeEnum ();
		}
		
		protected override object CloneCopyToNewObject(object o)
		{
			DbTypeEnum that = o as DbTypeEnum;
			
			base.CloneCopyToNewObject (that);
			
			that.CopyValues (this.values);
			that.max_name_length = this.max_name_length;
			
			return that;
		}
		
		
		protected void CopyValues(DbEnumValue[] values)
		{
			if (values == null)
			{
				this.values = null;
			}
			else
			{
				this.values = new DbEnumValue[values.Length];
				
				for (int i = 0; i < values.Length; i++)
				{
					this.values[i] = values[i].Clone () as DbEnumValue;
				}
			}
		}
		
		
		protected override void EnsureTypeIsNotInitialised()
		{
			if (this.values != null)
			{
				throw new System.InvalidOperationException ("Cannot reinitialise type");
			}
		}
		
		
		#region ICollection Members
		public bool IsSynchronized
		{
			get
			{
				return this.values == null ? false : this.values.IsSynchronized;
			}
		}

		public int Count
		{
			get
			{
				return this.values == null ? 0 : this.values.Length;
			}
		}

		public void CopyTo(System.Array array, int index)
		{
			if (this.values != null)
			{
				this.values.CopyTo (array, index);
			}
		}

		public object SyncRoot
		{
			get
			{
				return this.values.SyncRoot;
			}
		}
		#endregion
		
		#region IEnumerable Members
		public System.Collections.IEnumerator GetEnumerator()
		{
			return this.values.GetEnumerator ();
		}

		#endregion
		
		
		protected DbEnumValue[]					values;
		private int								max_name_length = 8;
	}
}
