//	Copyright © 2004-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

[assembly: Epsitec.Common.Types.DependencyClass (typeof (Epsitec.Common.Types.EnumType))]

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
		public EnumType()
			: this (null, new Caption ())
		{
		}
		
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
			: base ()
		{
			if ((enumType == null) ||
				(enumType == typeof (NotAnEnum)))
			{
				if (caption != null)
				{
					this.DefineCaption (caption);
				}

				this.CreateEnumValues ();
			}
			else
			{
				this.CreateEnumValues (enumType, caption);
			}
		}

		public EnumType(Support.Druid druid)
			: this (null, druid)
		{
		}

		public EnumType(System.Type enumType, Support.Druid druid)
			: base (druid)
		{
			if ((enumType == null) ||
				(enumType == typeof (NotAnEnum)))
			{
				this.CreateEnumValues ();
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
				return this.EnumValues;
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
		
		public EnumValue						this[Support.Druid druid]
		{
			get
			{
				return this.FindValueFromDruid (druid);
			}
		}

		public bool								IsNativeEnum
		{
			get
			{
				if ((this.enumType == typeof (NotAnEnum)) ||
					(this.enumType == null))
				{
					return false;
				}
				else
				{
					return true;
				}
			}
		}

		public Collections.EnumValueCollection	EnumValues
		{
			get
			{
				if (this.enumValues == null)
				{
					this.enumValues = new Collections.EnumValueCollection ();
				}
				
				return this.enumValues;
			}
		}

		public static IComparer<EnumValue>		RankComparer
		{
			get
			{
				return new RankComparerImplementation ();
			}
		}

		public void MakeEditable()
		{
			this.EnumValues.Unlock ();
		}
		
		public EnumValue FindValueFromRank(int rank)
		{
			for (int i = 0; i < this.EnumValues.Count; i++)
			{
				if (this.EnumValues[i].Rank == rank)
				{
					return this.EnumValues[i];
				}
			}

			return null;
		}

		public EnumValue FindValueFromDruid(Support.Druid druid)
		{
			for (int i = 0; i < this.EnumValues.Count; i++)
			{
				if (this.EnumValues[i].CaptionId == druid)
				{
					return this.EnumValues[i];
				}
			}

			return null;
		}

		public EnumValue FindValueFromEnumValue(System.Enum value)
		{
			if (this.enumType == value.GetType ())
			{
				for (int i = 0; i < this.EnumValues.Count; i++)
				{
					if (System.Enum.Equals (this.EnumValues[i].Value, value))
					{
						return this.EnumValues[i];
					}
				}
			}

			return null;
		}

		public EnumValue FindValueFromName(string name)
		{
			for (int i = 0; i < this.EnumValues.Count; i++)
			{
				if (this.EnumValues[i].Name == name)
				{
					return this.EnumValues[i];
				}
			}
			
			return null;
		}

		public EnumValue FindValueFromCaptionId(Support.Druid captionId)
		{
			for (int i = 0; i < this.EnumValues.Count; i++)
			{
				if (this.EnumValues[i].CaptionId == captionId)
				{
					return this.EnumValues[i];
				}
			}
			
			return null;
		}

		public System.Enum ConvertToEnum(int value)
		{
			if (this.enumType == typeof (NotAnEnum))
			{
				return NotAnEnum.Instance;
			}
			else
			{
				return (System.Enum) System.Enum.ToObject (this.enumType, value);
			}
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

		protected override void OnCaptionDefined()
		{
			base.OnCaptionDefined ();

			Caption caption = this.Caption;
			
			Collections.EnumValueCollection internalValues = this.EnumValues;
			Collections.EnumValueCollection externalValues = EnumType.GetEnumValues (caption);

			System.Diagnostics.Debug.Assert (internalValues != null);
			System.Diagnostics.Debug.Assert (externalValues != null);
			
			foreach (EnumValue externalValue in externalValues)
			{
				EnumValue internalValue = null;
				
				string        name  = externalValue.Name;
				Support.Druid druid = externalValue.CaptionId;

				if (string.IsNullOrEmpty (name))
				{
					if (druid.IsValid)
					{
						internalValue = this[druid];
					}
				}
				else
				{
					internalValue = this[name];
				}

				if (internalValue == null)
				{
					if (this.pendingEnumValues == null)
					{
						this.pendingEnumValues = new Collections.EnumValueCollection ();
					}
					
					this.pendingEnumValues.Add (externalValue);
				}
				else
				{
					EnumValue.CopyProperties (externalValue, internalValue);
				}
			}

			EnumType.SetEnumValues (caption, internalValues);
		}
		
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
				
				//	TODO: map System.Type to Caption (using an attribute ?)
				
				enumType = new EnumType (systemType);

				EnumType.cache[systemType] = enumType;
				
				return enumType;
			}
		}

		private void CreateEnumValues(System.Type enumType)
		{
			this.CreateEnumValues (enumType, null);
		}
		
		private void CreateEnumValues(System.Type enumType, Caption caption)
		{
			System.Diagnostics.Debug.Assert (enumType != null);
			System.Diagnostics.Debug.Assert (enumType != typeof (NotAnEnum));
			
			FieldInfo[] fields = enumType.GetFields (BindingFlags.Public | BindingFlags.DeclaredOnly | BindingFlags.Static);

			this.enumType = enumType;

			if (caption != null)
			{
				this.DefineCaption (caption);
			}
			
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

				this.EnumValues.Add (new EnumValue (value, rank, hide, name));
			}

			this.FinishCreation ();
		}

		private void CreateEnumValues()
		{
			Caption caption = this.Caption;
			
			this.enumType = typeof (NotAnEnum);

			//	TODO: fill enum values from caption definition

			this.FinishCreation ();

			AbstractType.SetSystemType (caption, this.enumType);
		}

		private void FinishCreation()
		{
			if (this.pendingEnumValues != null)
			{
				foreach (EnumValue pendingValue in this.pendingEnumValues)
				{
					string name = pendingValue.Name;

					EnumValue internalValue = this[name];

					if (internalValue == null)
					{
						this.EnumValues.Add (pendingValue);
					}
					else
					{
						EnumValue.CopyProperties (pendingValue, internalValue);
					}
				}

				this.pendingEnumValues = null;
			}

			this.EnumValues.Sort (EnumType.RankComparer);
			this.EnumValues.Lock ();
		}

		private static Collections.EnumValueCollection GetEnumValues(Caption caption)
		{
			return (Collections.EnumValueCollection) caption.GetValue (EnumType.EnumValuesProperty);
		}

		private static void SetEnumValues(Caption caption, Collections.EnumValueCollection value)
		{
			if (value == null)
			{
				caption.ClearValue (EnumType.EnumValuesProperty);
			}
			else
			{
				caption.SetValue (EnumType.EnumValuesProperty, value);
			}
		}

		private static object GetEnumValuesValue(DependencyObject obj)
		{
			if (obj.ContainsLocalValue (EnumType.EnumValuesProperty))
			{
				return obj.GetLocalValue (EnumType.EnumValuesProperty);
			}
			else
			{
				Collections.EnumValueCollection collection = new Collections.EnumValueCollection ();

				obj.SetLocalValue (EnumType.EnumValuesProperty, collection);
				
				return collection;
			}
		}

		public static DependencyProperty EnumValuesProperty = DependencyProperty.RegisterAttached ("EnumValues", typeof (Collections.EnumValueCollection), typeof (EnumType), new DependencyPropertyMetadata (EnumType.GetEnumValuesValue));
		
		private static object exclusion = new object ();
		private static Dictionary<System.Type, EnumType> cache = new Dictionary<System.Type, EnumType> ();
		
		private System.Type						enumType;
		private Collections.EnumValueCollection enumValues;
		private Collections.EnumValueCollection pendingEnumValues;
	}
}
