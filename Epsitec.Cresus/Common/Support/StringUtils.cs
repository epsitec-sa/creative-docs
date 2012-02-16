using System;

using System.Linq;
using Epsitec.Common.Types;

namespace Epsitec.Common.Support
{


	public static class StringUtils
	{


		public static string Join(string separator, params string[] strings)
		{
			return string.Join (separator, strings.Where (s => !string.IsNullOrEmpty (s)));
		}


		public static string Join(string separator, params object[] objects)
		{
			var strings = objects
				.Where (o => o != null)
				.Select (o => o.ToString ())
				.Where (s => !string.IsNullOrEmpty (s));

			return string.Join (separator, strings);
		}


		public static int? ParseNullableInt(string text)
		{
			int value;

			if (int.TryParse (text, out value))
			{
				return value;
			}
			else
			{
				return null;
			}
		}


		public static DateTime? ParseNullableDateTime(string text)
		{
			DateTime value;

			if (DateTime.TryParse (text, out value))
			{
				return value;
			}
			else
			{
				return null;
			}
		}


		public static Date? ParseNullableDate(string text)
		{
			DateTime? value = StringUtils.ParseNullableDateTime (text);

			if (value.HasValue)
			{
				return new Date (value.Value);
			}
			else
			{
				return null;
			}
		}


	}


}
