//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Types
{
	using IComparer = System.Collections.IComparer;
	using FieldInfo = System.Reflection.FieldInfo;
	using BindingFlags = System.Reflection.BindingFlags;
	
	/// <summary>
	/// La classe EnumType décrit divers une énumération native.
	/// </summary>
	public class EnumType : IEnum, IDataConstraint
	{
		public EnumType(System.Type enum_type)
		{
			FieldInfo[] fields = enum_type.GetFields (BindingFlags.Public | BindingFlags.DeclaredOnly | BindingFlags.Static);
			
			this.enum_type   = enum_type;
			this.enum_values = new EnumValue[fields.Length];
			
			for (int i = 0; i < fields.Length; i++)
			{
				string name = fields[i].Name;
				bool   hide = (fields[i].GetCustomAttributes (typeof (HideAttribute), false).Length > 0);
				int    rank = (int) System.Enum.Parse (this.enum_type, name);
				
				this.enum_values[i] = new EnumValue (rank, name);
				this.enum_values[i].DefineHidden (hide);
			}
			
			System.Array.Sort (this.enum_values, EnumType.RankComparer);
		}
		
		
		public EnumValue[]						Values
		{
			get
			{
				return this.enum_values;
			}
		}
		
		public EnumValue						this[string name]
		{
			get
			{
				return this.FindValueFromName (name);
			}
		}
		
		public EnumValue						this[int rank]
		{
			get
			{
				return this.FindValueFromRank (rank);
			}
		}
		
		public EnumValue						this[System.Enum rank]
		{
			get
			{
				return this.FindValueFromRank (System.Convert.ToInt32 (rank));
			}
		}
		
		
		public static IComparer					RankComparer
		{
			get
			{
				return new RankComparerClass ();
			}
		}
		
		
		public void DefineCaption(string caption)
		{
			this.caption = caption;
		}
			
		public void DefineDescription(string description)
		{
			this.description = description;
		}
		
		public void DefineCaptionsFromResources(string id)
		{
			this.DefineCaption (Resources.MakeTextRef (id));
			
			foreach (EnumValue value in this.enum_values)
			{
				value.DefineCaption (Resources.MakeTextRef (id, value.Name));
			}
		}
		
		
		public EnumValue FindValueFromRank(int rank)
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
		
		public EnumValue FindValueFromName(string name)
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
		
		public EnumValue FindValueFromCaption(string caption)
		{
			for (int i = 0; i < this.enum_values.Length; i++)
			{
				if (this.enum_values[i].Caption == caption)
				{
					return this.enum_values[i];
				}
			}
			
			return null;
		}
		
		
		#region IDataConstraint Members
		public virtual bool CheckConstraint(object value)
		{
			try
			{
				System.Enum enum_value = (System.Enum) System.Enum.Parse (this.enum_type, value.ToString ());
				
				return Converter.CheckEnumValue (this.enum_type, enum_value);
			}
			catch
			{
			}
			
			return false;
		}
		#endregion
		
		#region INamedType Members
		public virtual System.Type				SystemType
		{
			get
			{
				return enum_type;
			}
		}
		#endregion
		
		#region IEnum Members
		public virtual bool						IsCustomizable
		{
			get
			{
				return false;
			}
		}
		
		public virtual bool						IsFlags
		{
			get
			{
				if (this.enum_type != null)
				{
					if (this.enum_type.GetCustomAttributes (typeof (System.FlagsAttribute), false).Length > 0)
					{
						return true;
					}
				}
				
				return false;
			}
		}
		
		IEnumValue[]							IEnum.Values
		{
			get
			{
				return this.Values;
			}
		}
		
		IEnumValue								IEnum.this[string name]
		{
			get
			{
				return this[name];
			}
		}
		
		IEnumValue								IEnum.this[int rank]
		{
			get
			{
				return this[rank];
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
				return this.caption;
			}
		}

		public virtual string					Description
		{
			get
			{
				return this.description;
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
			
			
			public void DefineCaption(string caption)
			{
				this.caption = caption;
			}
			
			public void DefineDescription(string description)
			{
				this.description = description;
			}
			
			public void DefineHidden(bool hide)
			{
				this.hidden = hide;
			}
			
			
			#region IEnumValue Members
			public int							Rank
			{
				get
				{
					return this.rank;
				}
			}
			
			public bool							IsHidden
			{
				get
				{
					return this.hidden;
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
			private bool						hidden;
			private string						name;
			private string						caption;
			private string						description;
		}
		
		
		private class RankComparerClass : System.Collections.IComparer
		{
			#region IComparer Members
			public int Compare(object x, object y)
			{
				IEnumValue val_x = x as IEnumValue;
				IEnumValue val_y = y as IEnumValue;

				if (val_x == val_y)
				{
					return 0;
				}
				
				if (val_x == null)
				{
					return -1;
				}
				if (val_y == null)
				{
					return 1;
				}
				
				int rx = val_x.Rank;
				int ry = val_y.Rank;
				
				return rx - ry;
			}
			#endregion
		}

		
		private System.Type						enum_type;
		private EnumValue[]						enum_values;
		private string							caption;
		private string							description;
	}
}
