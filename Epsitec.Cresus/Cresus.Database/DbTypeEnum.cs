//	Copyright © 2003, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : OK/PA, 19/11/2003

namespace Epsitec.Cresus.Database
{
	/// <summary>
	/// La classe DbTypeEnum définit une énumération.
	/// </summary>
	public class DbTypeEnum : DbType, System.Collections.ICollection
	{
		public DbTypeEnum(params string[] attributes) : base (attributes)
		{
		}
		
		public DbTypeEnum(System.Collections.ICollection values, params string[] attributes) : this (attributes)
		{
			this.Initialise (values);
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
		
		
		
		public override object Clone()
		{
			DbTypeEnum def = base.Clone () as DbTypeEnum;
			
			def.CopyValues (this.values);
			
			return def;
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
			base.EnsureTypeIsNotInitialised ();
			
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
				return this.values.IsSynchronized;
			}
		}

		public int Count
		{
			get
			{
				return this.values.Length;
			}
		}

		public void CopyTo(System.Array array, int index)
		{
			this.values.CopyTo (array, index);
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
	}
}
