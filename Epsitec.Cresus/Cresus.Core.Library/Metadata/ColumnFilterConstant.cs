//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Common.Support.Extensions;

using Epsitec.Cresus.DataLayer.Context;

using System.Linq.Expressions;
using System.Xml.Linq;

namespace Epsitec.Cresus.Core.Metadata
{
	/// <summary>
	/// The <c>ColumnFilterConstant</c> structure represents a constant used in a column
	/// filter. The value is persisted through <see cref="ToString"/> and <see cref="Parse"/>
	/// methods.
	/// </summary>
	public struct ColumnFilterConstant : IFilter, System.IEquatable<ColumnFilterConstant>
	{
		private ColumnFilterConstant(ColumnFilterConstantType type, object value)
		{
			this.type = type;
			this.value = value;
		}


		public ColumnFilterConstantType			Type
		{
			get
			{
				return this.type;
			}
		}

		public System.Type						SystemType
		{
			get
			{
				if ((this.value != null) &&
					(this.type == ColumnFilterConstantType.Enumeration))
				{
					return this.value.GetType ();
				}
				else
				{
					return ColumnFilterConstant.ToSystemType (this.type);
				}
			}
		}

		public object							Value
		{
			get
			{
				return this.value;
			}
		}

		public bool								IsNull
		{
			get
			{
				return (this.type != ColumnFilterConstantType.Undefined) && this.value == null;
			}
		}
		
		public bool								IsDefined
		{
			get
			{
				return this.type != ColumnFilterConstantType.Undefined;
			}
		}

		#region IFilter Members

		public bool								IsValid
		{
			get
			{
				return this.IsNull == false && this.IsDefined;
			}
		}
		
		public Expression GetExpression(Expression parameter)
		{
			return Expression.Constant (this.value, this.SystemType);
		}

		#endregion

		public override string ToString()
		{
			string text;
			string code = ColumnFilterConstant.ToTypeCode (this.type);

			if (this.value == null)
			{
				// The Invariant Converter does not handle properly null values. It doesn't know how
				// to convert them. So we do it by ourselves.

				text = "";
			}
			else if (this.type == ColumnFilterConstantType.Enumeration)
			{
				text = EnumType.ToCompactString ((System.Enum)this.value);
			}
			else if (this.type == ColumnFilterConstantType.DateTime)
			{
				// Here we don't use the InvariantConverter because it serializes only the UTC value
				// of the DateTime and does not store the data about its time zone. So if we had a
				// constant that is a local DateTime, we would loose part of its information. That's
				// why we use a custom serialization here.

				text = ((System.DateTime) this.value).ToString ("o");
			}
			else
			{
				text = InvariantConverter.ToString (this.value);
			}

			return string.Concat (code, text);
		}

		#region IEquatable<ColumnFilterConstant> Members

		public bool Equals(ColumnFilterConstant other)
		{
			if (other.type == this.type)
			{
				if (this.type == ColumnFilterConstantType.String)
				{
					return (string) other.value == (string) this.value;
				}
				else if (this.type == ColumnFilterConstantType.EntityKey)
				{
					return (EntityKey) other.value == (EntityKey) this.value;
				}
				else
				{
					return other.value == this.value;
				}
			}

			return false;
		}

		#endregion


		public static ColumnFilterConstant From(int? value)
		{
			return new ColumnFilterConstant (ColumnFilterConstantType.Integer, value);
		}

		public static ColumnFilterConstant From(long? value)
		{
			return new ColumnFilterConstant (ColumnFilterConstantType.Long, value);
		}

		public static ColumnFilterConstant From(bool? value)
		{
			return new ColumnFilterConstant (ColumnFilterConstantType.Boolean, value);
		}

		public static ColumnFilterConstant From(decimal? value)
		{
			return new ColumnFilterConstant (ColumnFilterConstantType.Decimal, value);
		}

		public static ColumnFilterConstant From(string value)
		{
			return new ColumnFilterConstant (ColumnFilterConstantType.String, value);
		}

		public static ColumnFilterConstant From(System.Enum value)
		{
			return new ColumnFilterConstant (ColumnFilterConstantType.Enumeration, value);
		}

		public static ColumnFilterConstant From(EntityKey? value)
		{
			return new ColumnFilterConstant (ColumnFilterConstantType.EntityKey, value);
		}

		public static ColumnFilterConstant From(System.DateTime? value)
		{
			return new ColumnFilterConstant (ColumnFilterConstantType.DateTime, value);
		}

		public static ColumnFilterConstant From(Date? value)
		{
			return new ColumnFilterConstant (ColumnFilterConstantType.Date, value);
		}

		public static ColumnFilterConstant From(Time? value)
		{
			return new ColumnFilterConstant (ColumnFilterConstantType.Time, value);
		}


		/// <summary>
		/// Creates a constant from an <c>enum</c> value.
		/// </summary>
		/// <typeparam name="T">The type of the <c>enum</c>.</typeparam>
		/// <param name="value">The value.</param>
		/// <returns>The filter constant.</returns>
		public static ColumnFilterConstant FromEnum<T>(T value)
			where T : struct
		{
			return ColumnFilterConstant.From ((System.Enum) (object) value);
		}


		/// <summary>
		/// Parses the specified value back into a filter constant. This is the
		/// opposite of <see cref="ToString"/>.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns>The filter constant.</returns>
		public static ColumnFilterConstant Parse(string value)
		{
			if (string.IsNullOrEmpty (value))
			{
				throw new System.ArgumentException ("Null or empty value");
			}

			var type = ColumnFilterConstant.FromTypeCode (value.Substring (0, 1));
			var obj  = ColumnFilterConstant.FromValue (type, value.Substring (1));

			return new ColumnFilterConstant (type, obj);
		}

		public static ColumnFilterConstant Parse(XAttribute attribute)
		{
			if (attribute == null)
			{
				throw new System.ArgumentException ("Null or empty value");
			}

			return ColumnFilterConstant.Parse (attribute.Value);
		}
		
		private static object FromValue(ColumnFilterConstantType type, string value)
		{
			if (string.IsNullOrEmpty (value))
			{
				return null;
			}

			switch (type)
			{
				case ColumnFilterConstantType.Integer:
					return InvariantConverter.ConvertFromString<int> (value);
				case ColumnFilterConstantType.Long:
					return InvariantConverter.ConvertFromString<long> (value);
				case ColumnFilterConstantType.Boolean:
					return InvariantConverter.ConvertFromString<bool> (value);
				case ColumnFilterConstantType.Decimal:
					return InvariantConverter.ConvertFromString<decimal> (value);
				case ColumnFilterConstantType.DateTime:
					return System.DateTime.Parse (value, null, System.Globalization.DateTimeStyles.RoundtripKind);
				case ColumnFilterConstantType.Date:
					return InvariantConverter.ConvertFromString<Date> (value);
				case ColumnFilterConstantType.Time:
					return InvariantConverter.ConvertFromString<Time> (value);
				case ColumnFilterConstantType.String:
					return value;
				case ColumnFilterConstantType.EntityKey:
					return EntityKey.Parse (value);
				case ColumnFilterConstantType.Enumeration:
					return EnumType.FromCompactString (value);
				case ColumnFilterConstantType.Undefined:
					return null;
				default:
					throw new System.NotSupportedException (string.Format ("{0} not supported", type.GetQualifiedName ()));
			}
		}

		private static string ToTypeCode(ColumnFilterConstantType type)
		{
			switch (type)
			{
				case ColumnFilterConstantType.Integer:
					return Strings.IntegerType;
				case ColumnFilterConstantType.Long:
					return Strings.LongType;
				case ColumnFilterConstantType.Boolean:
					return Strings.BooleanType;
				case ColumnFilterConstantType.Decimal:
					return Strings.DecimalType;
				case ColumnFilterConstantType.DateTime:
					return Strings.DateTimeType;
				case ColumnFilterConstantType.Date:
					return Strings.DateType;
				case ColumnFilterConstantType.Time:
					return Strings.TimeType;
				case ColumnFilterConstantType.String:
					return Strings.StringType;
				case ColumnFilterConstantType.Enumeration:
					return Strings.EnumerationType;
				case ColumnFilterConstantType.EntityKey:
					return Strings.EntityKeyType;
				case ColumnFilterConstantType.Undefined:
					return Strings.UndefinedType;
				default:
					throw new System.NotSupportedException (string.Format ("{0} not supported", type.GetQualifiedName ()));
			}
		}

		private static System.Type ToSystemType(ColumnFilterConstantType type)
		{
			switch (type)
			{
				case ColumnFilterConstantType.Integer:
					return typeof (int);
				case ColumnFilterConstantType.Long:
					return typeof (long);
				case ColumnFilterConstantType.Boolean:
					return typeof (bool);
				case ColumnFilterConstantType.Decimal:
					return typeof (decimal);
				case ColumnFilterConstantType.DateTime:
					return typeof (System.DateTime);
				case ColumnFilterConstantType.Date:
					return typeof (Date);
				case ColumnFilterConstantType.Time:
					return typeof (Time);
				case ColumnFilterConstantType.String:
					return typeof (string);
				case ColumnFilterConstantType.Undefined:
					return typeof (void);
				case ColumnFilterConstantType.Enumeration:
					return typeof (System.Enum);
				case ColumnFilterConstantType.EntityKey:
					return typeof (EntityKey);
				default:
					throw new System.NotSupportedException (string.Format ("{0} not supported", type.GetQualifiedName ()));
			}
		}

		private static ColumnFilterConstantType FromTypeCode(string code)
		{
			switch (code)
			{
				case Strings.IntegerType:
					return ColumnFilterConstantType.Integer;
				case Strings.LongType:
					return ColumnFilterConstantType.Long;
				case Strings.BooleanType:
					return ColumnFilterConstantType.Boolean;
				case Strings.DecimalType:
					return ColumnFilterConstantType.Decimal;
				case Strings.DateTimeType:
					return ColumnFilterConstantType.DateTime;
				case Strings.DateType:
					return ColumnFilterConstantType.Date;
				case Strings.TimeType:
					return ColumnFilterConstantType.Time;
				case Strings.StringType:
					return ColumnFilterConstantType.String;
				case Strings.EnumerationType:
					return ColumnFilterConstantType.Enumeration;
				case Strings.EntityKeyType:
					return ColumnFilterConstantType.EntityKey;
				case Strings.UndefinedType:
					return ColumnFilterConstantType.Undefined;
				default:
					throw new System.NotSupportedException (string.Format ("{0} not supported", code));
			}
		}

		
		private static class Strings
		{
			public const string UndefinedType   = "U";
			public const string IntegerType     = "I";
			public const string LongType		= "L";
			public const string BooleanType		= "B";
			public const string DecimalType     = "N";
			public const string DateTimeType    = "D";
			public const string DateType        = "d";
			public const string TimeType        = "t";
			public const string StringType      = "S";
			public const string EnumerationType = "e";
			public const string EntityKeyType   = "k";
		}


		private readonly ColumnFilterConstantType type;
		private readonly object					  value;
	}
}
