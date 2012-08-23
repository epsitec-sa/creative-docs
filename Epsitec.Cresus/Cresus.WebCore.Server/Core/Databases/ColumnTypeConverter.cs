using Epsitec.Common.Types;

using System;
using System.Globalization;


namespace Epsitec.Cresus.WebCore.Server.Core.Databases
{


	internal static class ColumnTypeConverter
	{


		public static object ServerToClient(ColumnType type, string value)
		{
			if (string.IsNullOrEmpty (value))
			{
				return value;
			}

			switch (type)
			{
				case ColumnType.Boolean:
					return bool.Parse (value);

				case ColumnType.Date:
					var date = DateTime.Parse (value).Date;
					var format = "dd/MM/yyyy";
					var culture = CultureInfo.InvariantCulture;
					return date.ToString (format, culture);

				case ColumnType.Integer:
					return InvariantConverter.ParseLong (value);

				case ColumnType.Number:
					return InvariantConverter.ParseDecimal (value);

				case ColumnType.String:
					return value;

				default:
					throw new NotImplementedException ();
			}
		}


		public static string ClientToServer(ColumnType type, object value)
		{
			if (value == null)
			{
				return null;
			}

			if (value is string && (string) value == "" && type != ColumnType.String)
			{
				return null;
			}

			switch (type)
			{
				case ColumnType.Boolean:
				case ColumnType.Integer:
				case ColumnType.Number:
					return InvariantConverter.ToString (value);

				case ColumnType.Date:
				case ColumnType.String:
					return (string) value;

				default:
					throw new NotImplementedException ();
			}
		}


	}


}
