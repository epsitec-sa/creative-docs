//	Copyright © 2003, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : OK/PA, 19/11/2003

namespace Epsitec.Cresus.Database
{
	/// <summary>
	/// La classe DbTypeEnum définit une énumération.
	/// </summary>
	public class DbTypeEnum : DbType, System.Collections.ICollection
	{
		public DbTypeEnum() : base (DbSimpleType.String)
		{
		}
		
		public DbTypeEnum(params string[] attributes) : base (DbSimpleType.String, attributes)
		{
		}
		
		public DbTypeEnum(System.Collections.ICollection values, params string[] attributes) : this (attributes)
		{
			this.Initialise (values);
		}
		
		public DbTypeEnum(System.Xml.XmlElement xml) : base (DbSimpleType.String)
		{
			string arg_nmlen = xml.GetAttribute ("nmlen");
			
			if (arg_nmlen.Length > 0)
			{
				this.max_name_length = System.Int32.Parse (arg_nmlen, System.Globalization.CultureInfo.InvariantCulture);
			}
		}
		
		
		internal override void SerialiseXmlAttributes(System.Text.StringBuilder buffer)
		{
			buffer.Append (@" nmlen=""");
			buffer.Append (this.max_name_length.ToString (System.Globalization.CultureInfo.InvariantCulture));
			buffer.Append (@"""");
		}
		
		
		internal void Initialise(System.Collections.ICollection values)
		{
			this.EnsureTypeIsNotInitialised ();
			
			DbEnumValue[] temp = new DbEnumValue[values.Count];
			values.CopyTo (temp, 0);
			System.Array.Sort (temp, DbEnumValue.IdComparer);
			
			if ((System.Utilities.CheckForDuplicates (temp, DbEnumValue.IdComparer, false)) ||
				(System.Utilities.CheckForDuplicates (temp, DbEnumValue.NameComparer)))
			{
				throw new System.ArgumentException ("Duplicates found");
			}
			
			this.CopyValues (temp);
		}
		
		
		public DbEnumValue					this[int index]
		{
			get
			{
				return this.values[index];
			}
		}
		
		public DbEnumValue					this[string name]
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
		
		public int							MaxNameLength
		{
			get { return this.max_name_length; }
		}
		
		
		
		public override object Clone()
		{
			DbTypeEnum type = base.Clone () as DbTypeEnum;
			
			type.CopyValues (this.values);
			type.max_name_length = this.max_name_length;
			
			return type;
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
		
		
		protected DbEnumValue[]				values;
		protected int						max_name_length = 8;
	}
}
