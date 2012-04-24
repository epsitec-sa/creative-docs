using Epsitec.Common.Types;

using System;

using System.Globalization;

using System.Linq;

using System.Text;


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


		public static string RemoveDiacritics(string text)
		{
			// NOTE This code is strongly inspired by the code found at these two places :
			// - http://stackoverflow.com/questions/249087
			// - http://blogs.msdn.com/b/michkap/archive/2007/05/14/2629747.aspx
			
			// What happens here is that first separate the chars from their diacritic. That's what
			// the formD transformation does. Then we discard all diacritics to build the result,
			// and finally we recompose some chars together. That's what the formC transform does.
			// However, I don't really understand why we must do so.

			if (text == null)
			{
				return text;
			}

			var formD = text.Normalize (NormalizationForm.FormD);

			var stripped = new StringBuilder (formD.Length);

			for (int i = 0; i < formD.Length; i++)
			{
				var c = formD[i];

				var charCategory = CharUnicodeInfo.GetUnicodeCategory (c);

				if (charCategory != UnicodeCategory.NonSpacingMark)
				{
					stripped.Append (c);
				}
			}

			var formC = stripped.ToString ().Normalize (NormalizationForm.FormC);

			return formC;
		}


		public static string GetDigits(string text)
		{
			var stringBuilder = new StringBuilder ();

			foreach (var c in text)
			{
				if (char.IsDigit (c))
				{
					stringBuilder.Append (c);
				}
			}

			return stringBuilder.ToString ();
		}


	}


}
