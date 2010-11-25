using System.Text.RegularExpressions;


namespace Epsitec.Common.Support.Extensions
{


	/// <summary>
	/// The <c>StringExtensions</c> class contains some useful extension method for instances of
	/// <see cref="System.String"/>.
	/// </summary>
	public static class StringExtensions
	{


		/// <summary>
		/// Checks that <paramref name="value"/> is an alpha numeric <see cref="System.String"/>,
		/// i.e. that it is empty or that it contains only lower case letters, upper case letters
		/// or numbers.
		/// </summary>
		/// <param name="value">The <see cref="System.String"/> to check.</param>
		/// <returns><c>true</c> if <paramref name="value"/> is alpha numeric, <c>false</c> if it isn't.</returns>
		/// <exception cref="System.ArgumentNullException">If <paramref name="value"/> is <c>null</c>.</exception>
		public static bool IsAlphaNumeric(this string value)
		{
			value.ThrowIfNull ("value");
			
			return new Regex ("^[a-zA-Z0-9]*$").IsMatch (value);
		}


		/// <summary>
		/// Splits <paramref name="value"/> into a <see cref="System.String[]"/> that contains all
		/// its substring that are separated by <paramref name="separator"/>. This is equivalent as
		/// The <see cref="System.String.Split"/> method.
		/// </summary>
		/// <param name="value">The <see cref="System.String"/> to split.</param>
		/// <param name="separator">The <see cref="System.String"/> used to separate the substrings.</param>
		/// <returns>The separated substrings.</returns>
		/// <exception cref="System.ArgumentNullException">If <paramref name="value"/> is <c>null</c>.</exception>
		/// <exception cref="System.ArgumentNullException">If <paramref name="separator"/> is <c>null</c> or empty.</exception>
		public static string[] Split(this string value, string separator)
		{
			value.ThrowIfNull ("value");
			separator.ThrowIfNullOrEmpty ("separator");

			return value.Split (new string[] { separator }, System.StringSplitOptions.None);
		}


	}


}
