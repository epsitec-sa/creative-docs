//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : OK/PA, 27/04/2004

namespace Epsitec.Common.Types
{
	/// <summary>
	/// La classe EnumType décrit divers types numériques natifs.
	/// </summary>
	public class EnumType : IEnum
	{
		public EnumType(System.Type enum_type)
		{
			string[] names = System.Enum.GetNames (enum_type);
			
			System.Array.Sort (names);
			
			this.enum_type   = enum_type;
			this.enum_values = new EnumValue[names.Length];
			
			for (int i = 0; i < names.Length; i++)
			{
				string name = names[i];
				int    rank = (int) System.Enum.Parse (this.enum_type, name);
				
				this.enum_values[i] = new EnumValue (rank, name);
			}
		}
		
		
		
		#region INamedType Members
		public System.Type						SystemType
		{
			get
			{
				return enum_type;
			}
		}
		#endregion
		
		#region IEnum Members
		public IEnumValue[]						Values
		{
			get
			{
				return this.enum_values;
			}
		}
		public IEnumValue						this[string name]
		{
			get
			{
				for (int i = 0; i < this.enum_values.Length; i++)
				{
					if (this.enum_values[i].Name == name)
					{
						return this.enum_values[i];
					}
				}
				
				return null;
			}
		}
		
		public IEnumValue						this[int rank]
		{
			get
			{
				for (int i = 0; i < this.enum_values.Length; i++)
				{
					if (this.enum_values[i].Rank == rank)
					{
						return this.enum_values[i];
					}
				}
				
				return null;
			}
		}
		#endregion
		
		#region INameCaption Members
		public virtual string					Name
		{
			get
			{
				return "Enumeration";
			}
		}

		public virtual string					Caption
		{
			get
			{
				return null;
			}
		}

		public virtual string					Description
		{
			get
			{
				return null;
			}
		}
		#endregion
		
		
		public class EnumValue : IEnumValue
		{
			public EnumValue(int rank, string name) : this (rank, name, null, null)
			{
			}
			
			public EnumValue(int rank, string name, string caption) : this (rank, name, caption, null)
			{
			}
			
			public EnumValue(int rank, string name, string caption, string description)
			{
				this.rank        = rank;
				this.name        = name;
				this.caption     = caption;
				this.description = description;
			}
			
			
			#region IEnumValue Members
			public int							Rank
			{
				get
				{
					return this.rank;
				}
			}
			#endregion

			#region INameCaption Members
			public string						Name
			{
				get
				{
					return this.name;
				}
			}

			public string						Caption
			{
				get
				{
					return this.caption;
				}
			}

			public string						Description
			{
				get
				{
					return this.description;
				}
			}
			#endregion
			
			private int							rank;
			private string						name;
			private string						caption;
			private string						description;
		}

		
		private System.Type						enum_type;
		private EnumValue[]						enum_values;
	}
}
