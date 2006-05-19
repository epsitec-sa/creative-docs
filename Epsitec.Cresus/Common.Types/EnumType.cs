//	Copyright © 2004-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Types
{
	using IComparer    = System.Collections.IComparer;
	using FieldInfo    = System.Reflection.FieldInfo;
	using BindingFlags = System.Reflection.BindingFlags;
	
	/// <summary>
	/// La classe EnumType décrit des valeurs de type System.Enum.
	/// </summary>
	public class EnumType : IEnumType, IDataConstraint
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
				int    rank = EnumType.ConvertToInt ((System.Enum) System.Enum.Parse (this.enum_type, name));
				
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
				return new RankComparerImplementation ();
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
		
		
		public void DefineTextsFromResources(string id)
		{
			this.DefineCaption (Resources.MakeTextRef (id, Tags.Caption));
			this.DefineDescription (Resources.MakeTextRef (id, Tags.Description));
			
			foreach (EnumValue value in this.enum_values)
			{
				value.DefineCaption (Resources.MakeTextRef (id, value.Name, Tags.Caption));
				value.DefineDescription (Resources.MakeTextRef (id, value.Name, Tags.Description));
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
		public virtual bool ValidateValue(object value)
		{
			try
			{
				System.Enum enum_value = (System.Enum) System.Enum.Parse (this.enum_type, value.ToString ());
				
				return Converter.CheckEnumValue (this.enum_type, enum_value);
			}
			catch (System.ArgumentException)
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
		
		#region IEnumType Members
		public virtual bool						IsCustomizable
		{
			get
			{
				return false;
			}
		}
		
		public virtual bool						IsDefinedAsFlags
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
		
		IEnumValue[]							IEnumType.Values
		{
			get
			{
				return this.Values;
			}
		}
		
		IEnumValue								IEnumType.this[string name]
		{
			get
			{
				return this[name];
			}
		}
		
		IEnumValue								IEnumType.this[int rank]
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
		
		#region EnumValue Class
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
		#endregion
		
		#region RankComparerImplementation Class
		private class RankComparerImplementation : System.Collections.IComparer
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
		#endregion

		public static int ConvertToInt(System.Enum value)
		{
			//	TODO: optimize this code
			
			//	I guess we could do the same with this very simple IL, provided the value is
			//	represented using 32-bit (or less) :
			//
			//		ldarg.0
			//		ret
			//
			//	For 64-bit, maybe :
			//
			//		ldarg.0
			//		conv.i4
			//		ret
			
			System.Type enumType = value.GetType ();
			string text = System.Enum.Format (enumType, value, "d");
			
			long number = System.Int64.Parse (text, System.Globalization.CultureInfo.InvariantCulture);
			
			if ((number < int.MinValue) ||
				(number > int.MaxValue))
			{
				throw new System.InvalidOperationException (string.Format ("Value {0} cannot be mapped to int", number));
			}
			
			return (int) number;
		}
		
		private System.Type						enum_type;
		private EnumValue[]						enum_values;
		private string							caption;
		private string							description;
	}
}
