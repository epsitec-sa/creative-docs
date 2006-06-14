//	Copyright © 2004-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

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
			
			this.enumType   = enum_type;
			this.enumValues = new List<EnumValue> ();
			
			for (int i = 0; i < fields.Length; i++)
			{
				string name = fields[i].Name;
				bool   hide = (fields[i].GetCustomAttributes (typeof (HideAttribute), false).Length > 0);
				int    rank = EnumType.ConvertToInt ((System.Enum) System.Enum.Parse (this.enumType, name));

				EnumValue value = new EnumValue (rank, name);
				value.DefineHidden (hide);
				
				this.enumValues.Add (value);
			}

			this.enumValues.Sort (EnumType.RankComparer);
		}
		
		
		public IEnumerable<EnumValue>			Values
		{
			get
			{
				return this.enumValues;
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
		
		
		public static IComparer<EnumValue>		RankComparer
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
			
			foreach (EnumValue value in this.enumValues)
			{
				value.DefineCaption (Resources.MakeTextRef (id, value.Name, Tags.Caption));
				value.DefineDescription (Resources.MakeTextRef (id, value.Name, Tags.Description));
			}
		}
		
		
		public EnumValue FindValueFromRank(int rank)
		{
			for (int i = 0; i < this.enumValues.Count; i++)
			{
				if (this.enumValues[i].Rank == rank)
				{
					return this.enumValues[i];
				}
			}
			
			return null;
		}
		
		public EnumValue FindValueFromName(string name)
		{
			for (int i = 0; i < this.enumValues.Count; i++)
			{
				if (this.enumValues[i].Name == name)
				{
					return this.enumValues[i];
				}
			}
			
			return null;
		}
		
		public EnumValue FindValueFromCaption(string caption)
		{
			for (int i = 0; i < this.enumValues.Count; i++)
			{
				if (this.enumValues[i].Caption == caption)
				{
					return this.enumValues[i];
				}
			}
			
			return null;
		}
		
		
		#region IDataConstraint Members
		public virtual bool IsValidValue(object value)
		{
			try
			{
				System.Enum enum_value = (System.Enum) System.Enum.Parse (this.enumType, value.ToString ());
				
				return InvariantConverter.CheckEnumValue (this.enumType, enum_value);
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
				return enumType;
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
				if (this.enumType != null)
				{
					if (this.enumType.GetCustomAttributes (typeof (System.FlagsAttribute), false).Length > 0)
					{
						return true;
					}
				}
				
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
		
		#region RankComparerImplementation Class
		private class RankComparerImplementation : IComparer<EnumValue>
		{
			#region IComparer Members
			public int Compare(EnumValue val_x, EnumValue val_y)
			{
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
		
		private System.Type						enumType;
		private List<EnumValue>					enumValues;
		private string							caption;
		private string							description;
	}
}
