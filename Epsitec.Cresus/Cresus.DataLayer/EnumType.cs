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
		
		
		public override object Clone()
		{
			EnumType def = base.Clone () as EnumType;
			
			def.values = new EnumValue[this.values.Length];
			
			for (int i = 0; i < this.values.Length; i++)
			{
				def.values[i] = this.values[i].Clone () as EnumValue;
			}
			
			return def;
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
