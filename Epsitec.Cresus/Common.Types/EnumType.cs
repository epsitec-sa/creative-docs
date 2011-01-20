//	Copyright © 2004-2011, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

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
		/// <summary>
		/// Initializes a new instance of the <see cref="EnumType"/> class.
		/// </summary>
		public EnumType()
			: this (null, new Caption ())
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="EnumType"/> class.
		/// </summary>
		/// <param name="enumType">Type of the native enum to use as model.</param>
		public EnumType(System.Type enumType)
			: base (string.Concat ("Enumeration", " ", enumType.Name))
		{
			this.CreateEnumValues (enumType);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="EnumType"/> class.
		/// </summary>
		/// <param name="enumType">Type of the enum to use as model.</param>
		/// <param name="caption">The caption to use as model.</param>
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


		/// <summary>
		/// Gets the type code for the type.
		/// </summary>
		/// <value>The type code.</value>
		public override TypeCode TypeCode
		{
			get
			{
				return TypeCode.Enum;
			}
		}



		/// <summary>
		/// Enumerates through the <see cref="EnumValue"/> values, sorted first by
		/// rank, then by name.
		/// </summary>
		/// <value>The sorted enumeration values.</value>
		public IEnumerable<EnumValue>			Values
		{
			get
			{
				return this.EnumValues;
			}
		}

		/// <summary>
		/// Gets the first <see cref="EnumValue"/> with the specified name.
		/// </summary>
		/// <value>
		/// The <see cref="EnumValue"/> or <c>null</c> if no match could
		/// be found.
		/// </value>
		public EnumValue						this[string name]
		{
			get
			{
				return this.FindValueFromName (name);
			}
		}

		/// <summary>
		/// Gets the first <see cref="EnumValue"/> with the specified rank.
		/// </summary>
		/// <value>
		/// The <see cref="EnumValue"/> or <c>null</c> if no match could
		/// be found.
		/// </value>
		public EnumValue						this[int rank]
		{
			get
			{
				return this.FindValueFromRank (rank);
			}
		}

		/// <summary>
		/// Gets the <see cref="EnumValue"/> with the specified value.
		/// </summary>
		/// <value>The <see cref="EnumValue"/> or <c>null</c> if no match could
		/// be found.</value>
		public EnumValue						this[System.Enum value]
		{
			get
			{
				return this.FindValueFromEnumValue (value);
			}
		}

		/// <summary>
		/// Gets the <see cref="EnumValue"/> with the specified DRUID.
		/// </summary>
		/// <value>The <see cref="EnumValue"/> or <c>null</c> if no match could
		/// be found.</value>
		public EnumValue						this[Support.Druid druid]
		{
			get
			{
				return this.FindValueFromDruid (druid);
			}
		}

		/// <summary>
		/// Gets a value indicating whether this enumeration is based on a native CLR enum.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this enumeration is based on a native CLR enum; otherwise, <c>false</c>.
		/// </value>
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

		public override object					DefaultValue
		{
			get
			{
				object value = base.DefaultValue;
				return value ?? this.ConvertToEnum (0);
			}
		}

		/// <summary>
		/// Gets the collection of <see cref="EnumValue"/> values, which can
		/// be modified if <c>MakeEditable</c> was called.
		/// </summary>
		/// <value>The collection of <see cref="EnumValue"/> values.</value>
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

		/// <summary>
		/// Gets a comparer which can be used to sort <see cref="EnumValue"/> values
		/// by their rank.
		/// </summary>
		/// <value>The rank comparer.</value>
		public static IComparer<EnumValue>		RankComparer
		{
			get
			{
				return new RankComparerImplementation ();
			}
		}

		/// <summary>
		/// Makes the collection of <see cref="EnumValue"/> values editable.
		/// </summary>
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

		/// <summary>
		/// Converts an integer value to the <c>enum</c> value of the underlying
		/// type.
		/// </summary>
		/// <param name="value">The integer value.</param>
		/// <returns>The <c>enum</c> value of the underlying type or: <c>NotAnEnum.Instance</c>
		/// if the conversion is not possible, <c>UnresolvedEnum.Instance</c> if the <c>enum</c>
		/// cannot be resolved because of a missing assembly.</returns>
		public System.Enum ConvertToEnum(int value)
		{
			if (this.enumType == typeof (NotAnEnum))
			{
				return NotAnEnum.Instance;
			}
			else if (this.enumType == typeof (UnresolvedEnum))
			{
				return UnresolvedEnum.Instance;
			}
			else
			{
				return (System.Enum) System.Enum.ToObject (this.enumType, value);
			}
		}

		#region IDataConstraint Members
		public override bool IsValidValue(object value)
		{
			if (this.IsNullValue (value))
			{
				return this.IsNullable;
			}

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
				return this.enumType;
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

		/// <summary>
		/// Converts an <c>enum</c> value to an integer value.
		/// </summary>
		/// <param name="value">The <c>enum</c> value.</param>
		/// <returns>The integer value for the specified <c>enum</c> value.</returns>
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
		/// Converts a integer which encodes flags into a collection of matching
		/// enum values.
		/// </summary>
		/// <param name="type">The enum type.</param>
		/// <param name="flags">The flags.</param>
		/// <returns>The enum values (hidden values are excluded).</returns>
		public static IEnumerable<IEnumValue> ConvertEnumValuesFromFlags(IEnumType type, int flags)
		{
			foreach (IEnumValue enumValue in type.Values)
			{
				if (enumValue.IsHidden)
				{
					continue;
				}
				
				int value = EnumType.ConvertToInt (enumValue.Value);

				if ((value & flags) == value)
				{
					yield return enumValue;
				}
			}
		}

		/// <summary>
		/// Converts the collection of enum values back to an interger representation
		/// of the flags.
		/// </summary>
		/// <param name="values">The enum values.</param>
		/// <returns>The flags.</returns>
		public static int ConvertEnumValuesToFlags(IEnumerable<IEnumValue> values)
		{
			int flags = 0;

			foreach (IEnumValue value in values)
			{
				int flag = EnumType.ConvertToInt (value.Value);
				flags |= flag;
			}

			return flags;
		}

		/// <summary>
		/// Converts the collection of enum values to a string. This is similar to
		/// calling <see cref="System.Enum.ToString()"/> on the equivalent <c>enum</c>.
		/// </summary>
		/// <param name="values">The enum values.</param>
		/// <returns>The string representation.</returns>
		public static string ConvertToString(IEnumerable<IEnumValue> values)
		{
			System.Text.StringBuilder buffer = new System.Text.StringBuilder ();

			foreach (IEnumValue value in values)
			{
				if (buffer.Length > 0)
				{
					buffer.Append (", ");
				}
				
				buffer.Append (value.Name);
			}
			
			return buffer.Length == 0 ? null : buffer.ToString ();
		}

		/// <summary>
		/// Converts the string to a collection of enum values. This is similar to
		/// calling <see cref="System.Enum.Parse(System.Type, string)"/>.
		/// </summary>
		/// <param name="type">The enum type.</param>
		/// <param name="value">The value.</param>
		/// <returns>The collection of enum values.</returns>
		public static IEnumerable<IEnumValue> ConvertFromString(IEnumType type, string value)
		{
			if (string.IsNullOrEmpty (value))
			{
				yield break;
			}

			string[] tokens = value.Split (',');

			foreach (string token in tokens)
			{
				yield return type[token.Trim ()];
			}
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

		/// <summary>
		/// Gets all enum values for a given enumeration type.
		/// </summary>
		/// <returns>The enumeration of all enumeration values.</returns>
		public static IEnumerable<T> GetAllEnumValues<T>()
		{
			System.Type enumType = typeof (T);

			FieldInfo[] fields = enumType.GetFields (BindingFlags.Public | BindingFlags.DeclaredOnly | BindingFlags.Static);

			for (int i = 0; i < fields.Length; i++)
			{
				string name = fields[i].Name;
				yield return (T) System.Enum.Parse (enumType, name);
			}
		}

		public static bool GetIsNative(DependencyObject obj)
		{
			return (bool) obj.GetValue (EnumType.IsNativeProperty);
		}

		public static void SetIsNative(DependencyObject obj, bool value)
		{
			if (value)
			{
				obj.SetValue (EnumType.IsNativeProperty, value);
			}
			else
			{
				obj.ClearValue (EnumType.IsNativeProperty);
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

			this.enumType = enumType;

			if (caption != null)
			{
				this.DefineCaption (caption);
			}

			if (enumType != typeof (UnresolvedEnum))
			{
				FieldInfo[] fields = enumType.GetFields (BindingFlags.Public | BindingFlags.DeclaredOnly | BindingFlags.Static);

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

		/// <summary>
		/// When the <c>EnumType</c> is created based on a native enumeration and a
		/// <c>Caption</c> object, all definitions from the caption are just stored
		/// in the <c>pendingEnumValues</c> collection; <c>EnumValues</c> is populated
		/// with automatically generated values taken from the <c>enum</c> itself.
		/// <c>FinishCreation</c> merges this with the additional information found
		/// in the <c>Caption</c> object.
		/// </summary>
		private void FinishCreation()
		{
			if (this.pendingEnumValues != null)
			{
				foreach (EnumValue pendingValue in this.pendingEnumValues)
				{
					string name = pendingValue.Name;
					Support.Druid druid = pendingValue.CaptionId;

					EnumValue internalValue = string.IsNullOrEmpty (name) ? this[druid] : this[name];

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
		public static DependencyProperty IsNativeProperty = DependencyProperty.RegisterAttached ("IsNative", typeof (bool), typeof (EnumType), new DependencyPropertyMetadata (false));
		
		private static object exclusion = new object ();
		private static Dictionary<System.Type, EnumType> cache = new Dictionary<System.Type, EnumType> ();
		
		private System.Type						enumType;
		private Collections.EnumValueCollection enumValues;
		private Collections.EnumValueCollection pendingEnumValues;
	}
}
