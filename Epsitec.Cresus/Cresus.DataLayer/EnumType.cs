//	Copyright © 2003, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : en chantier/PA

namespace Epsitec.Cresus.DataLayer
{
	/// <summary>
	/// La classe EnumType définit une énumération.
	/// </summary>
	public class EnumType : DataType, System.Collections.ICollection
	{
		public EnumType()
		{
		}
		
		public EnumType(params string[] attributes) : base (attributes)
		{
		}
		
		
		public override object Clone()
		{
			EnumType def = base.Clone () as EnumType;
			
			if (this.values != null)
			{
				def.CopyValues (this.values);
			}
			
			return def;
		}
		
		
		public void DefineValues(System.Collections.ICollection values)
		{
			if (this.values != null)
			{
				throw new System.InvalidOperationException ("Cannot redefine values");
			}
			
			EnumValue[] temp = new EnumValue[values.Count];
			values.CopyTo (temp, 0);
			System.Array.Sort (temp, EnumValue.IdComparer);
			
			if ((System.Utilities.CheckForDuplicates (temp, EnumValue.IdComparer, false)) ||
				(System.Utilities.CheckForDuplicates (temp, EnumValue.NameComparer)))
			{
				throw new System.ArgumentException ("Duplicates found");
			}
			
			this.CopyValues (temp);
		}
		
		protected void CopyValues(EnumValue[] values)
		{
			this.values = new EnumValue[values.Length];
			
			for (int i = 0; i < values.Length; i++)
			{
				this.values[i] = values[i].Clone () as EnumValue;
			}
		}
		
		
		public EnumValue					this[int index]
		{
			get
			{
				return this.values[index];
			}
		}
		
		public EnumValue					this[string name]
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
		
		
		protected EnumValue[]				values;
	}
}
