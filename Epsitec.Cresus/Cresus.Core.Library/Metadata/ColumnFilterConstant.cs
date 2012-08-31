//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Common.Support.Extensions;

namespace Epsitec.Cresus.Core.Metadata
{
	public struct ColumnFilterConstant : System.IEquatable<ColumnFilterConstant>
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

		public object							Value
		{
			get
			{
				return this.value;
			}
		}

		
		public override string ToString()
		{
			string text;
			string code = ColumnFilterConstant.ToTypeCode (this.type);
			InvariantConverter.Convert (this.value, out text);

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
				else
				{
					return other.value == this.value;
				}
			}

			return false;
		}

		#endregion


		public static ColumnFilterConstant From(int value)
		{
			return new ColumnFilterConstant (ColumnFilterConstantType.Integer, value);
		}

		public static ColumnFilterConstant From(decimal value)
		{
			return new ColumnFilterConstant (ColumnFilterConstantType.Decimal, value);
		}

		public static ColumnFilterConstant From(string value)
		{
			return new ColumnFilterConstant (ColumnFilterConstantType.String, value);
		}

		//	TODO: add other From methods, as needed

		
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

		
		private static object FromValue(ColumnFilterConstantType type, string value)
		{
			switch (type)
			{
				case ColumnFilterConstantType.Integer:
					return InvariantConverter.ConvertFromString<int> (value);
				case ColumnFilterConstantType.Decimal:
					return InvariantConverter.ConvertFromString<decimal> (value);
				case ColumnFilterConstantType.DateTime:
					return InvariantConverter.ConvertFromString<System.DateTime> (value);
				case ColumnFilterConstantType.Date:
					return InvariantConverter.ConvertFromString<Date> (value);
				case ColumnFilterConstantType.Time:
					return InvariantConverter.ConvertFromString<Time> (value);
				case ColumnFilterConstantType.String:
					return value;
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
				case ColumnFilterConstantType.Undefined:
					return Strings.UndefinedType;
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
				case Strings.UndefinedType:
					return ColumnFilterConstantType.Undefined;
				default:
					throw new System.NotSupportedException (string.Format ("{0} not supported", code));
			}
		}

		
		private static class Strings
		{
			public const string IntegerType = "I";
			public const string DecimalType = "N";
			public const string DateTimeType = "D";
			public const string DateType = "d";
			public const string TimeType = "t";
			public const string StringType = "S";
			public const string UndefinedType = "U";
		}


		private readonly ColumnFilterConstantType type;
		private readonly object value;
	}
}
