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
	public class EnumType : AbstractType, IEnumType
	{
		public EnumType(System.Type enumType)
			: base (string.Concat ("Enumeration", " ", enumType.Name))
		{
			this.CreateEnumValues (enumType);
		}

		public EnumType(Caption caption)
			: this (null, caption)
		{
		}

		public EnumType(System.Type enumType, Caption caption)
			: base (caption)
		{
			if ((enumType == null) ||
				(enumType == typeof (NotAnEnum)))
			{
				this.CreateEnumValues (caption);
			}
			else
			{
				this.CreateEnumValues (enumType);
			}
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
		
		public EnumValue						this[System.Enum value]
		{
			get
			{
				return this.FindValueFromEnumValue (value);
			}
		}
		
		
		public static IComparer<EnumValue>		RankComparer
		{
			get
			{
				return new RankComparerImplementation ();
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

		public EnumValue FindValueFromEnumValue(System.Enum value)
		{
			for (int i = 0; i < this.enumValues.Count; i++)
			{
				if (System.Enum.Equals (this.enumValues[i].Value, value))
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

		public EnumValue FindValueFromCaptionId(Support.Druid captionId)
		{
			for (int i = 0; i < this.enumValues.Count; i++)
			{
				if (this.enumValues[i].CaptionId == captionId)
				{
					return this.enumValues[i];
				}
			}
			
			return null;
		}


		#region IDataConstraint Members
		public override bool IsValidValue(object value)
		{
			try
			{
				System.Enum enumValue = (System.Enum) System.Enum.Parse (this.enumType, value.ToString ());
				
				return InvariantConverter.CheckEnumValue (this.enumType, enumValue);
			}
			catch (System.ArgumentException)
			{
			}
			
			return false;
		}
		#endregion
		
		#region ISystemType Members
		public override System.Type				SystemType
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
		
		#region RankComparerImplementation Class
		private class RankComparerImplementation : IComparer<EnumValue>
		{
			#region IComparer Members
			public int Compare(EnumValue valX, EnumValue valY)
			{
				if (valX == valY)
				{
					return 0;
				}
				
				if (valX == null)
				{
					return -1;
				}
				if (valY == null)
				{
					return 1;
				}
				
				int rx = valX.Rank;
				int ry = valY.Rank;

				if (rx < ry)
				{
					return -1;
				}
				if (rx > ry)
				{
					return 1;
				}

				return string.CompareOrdinal (valX.Name, valY.Name);
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

		/// <summary>
		/// Gets the default <c>EnumType</c> for some enumeration type.
		/// </summary>
		/// <param name="systemType">The enumeration type.</param>
		/// <returns>The <c>EnumType</c> instance for the specified enumeration type.</returns>
		public static EnumType GetDefault(System.Type systemType)
		{
			if (systemType == null)
			{
				throw new System.ArgumentNullException ();
			}
			if (systemType.IsEnum == false)
			{
				throw new System.ArgumentException (string.Format ("Type {0} is not an enum", systemType.Name));
			}
			
			lock (EnumType.exclusion)
			{
				EnumType enumType;

				if (EnumType.cache.TryGetValue (systemType, out enumType))
				{
					return enumType;
				}
				
				enumType = new EnumType (systemType);

				EnumType.cache[systemType] = enumType;
				
				return enumType;
			}
		}

		private void CreateEnumValues(System.Type enumType)
		{
			System.Diagnostics.Debug.Assert (enumType != null);
			System.Diagnostics.Debug.Assert (enumType != typeof (NotAnEnum));
			
			FieldInfo[] fields = enumType.GetFields (BindingFlags.Public | BindingFlags.DeclaredOnly | BindingFlags.Static);

			this.enumType   = enumType;
			this.enumValues = new Collections.EnumValueCollection ();

			for (int i = 0; i < fields.Length; i++)
			{
				object[] hiddenAttributes;
				object[] rankAttributes;

				hiddenAttributes = fields[i].GetCustomAttributes (typeof (HiddenAttribute), false);
				rankAttributes   = fields[i].GetCustomAttributes (typeof (RankAttribute), false);

				string name = fields[i].Name;
				bool hide = hiddenAttributes.Length == 1;
				int rank;

				System.Enum value = (System.Enum) System.Enum.Parse (this.enumType, name);

				if (rankAttributes.Length == 1)
				{
					RankAttribute rankAttribute = rankAttributes[0] as RankAttribute;
					rank = rankAttribute.Rank;
				}
				else
				{
					rank = EnumType.ConvertToInt (value);
				}

				this.enumValues.Add (new EnumValue (value, rank, hide, name));
			}

			this.enumValues.Sort (EnumType.RankComparer);
			this.enumValues.Lock ();
		}

		private void CreateEnumValues(Caption caption)
		{
			this.enumType   = typeof (NotAnEnum);
			this.enumValues = new Collections.EnumValueCollection ();

			//	TODO: fill enum values from caption definition
			
			this.enumValues.Lock ();

			AbstractType.SetSystemType (caption, this.enumType);
		}

		private static object exclusion = new object ();
		private static Dictionary<System.Type, EnumType> cache = new Dictionary<System.Type, EnumType> ();
		
		private System.Type						enumType;
		private Collections.EnumValueCollection	enumValues;
	}
}
