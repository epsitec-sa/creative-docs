//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD & Marc BETTEX, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.Extensions;

using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Common.Support.Extensions
{
	/// <summary>
	/// The <c>StringExtensions</c> class contains some useful extension method for instances of
	/// <see cref="System.String"/>.
	/// </summary>
	public static class StringExtensions
	{
		public static bool IsNullOrWhiteSpace(this string text)
		{
#if DOTNET35
			return string.IsNullOrEmpty (text)
						|| text.Trim ().Length == 0;
#else
			return string.IsNullOrWhiteSpace (text);
#endif
		}
		
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
			if (value == null)
			{
				throw new System.ArgumentNullException ("value");
			}

			return new Regex ("^[a-zA-Z0-9]*$").IsMatch (value);
		}


		/// <summary>
		/// Splits <paramref name="value"/> into an array of <see cref="string"/> that contains all
		/// its substring that are separated by <paramref name="separator"/>.
		/// </summary>
		/// <param name="value">The <see cref="System.String"/> to split.</param>
		/// <param name="separator">The <see cref="System.String"/> used to separate the substrings.</param>
		/// <returns>The separated substrings.</returns>
		/// <exception cref="System.ArgumentNullException">If <paramref name="value"/> is <c>null</c>.</exception>
		/// <exception cref="System.ArgumentNullException">If <paramref name="separator"/> is <c>null</c> or empty.</exception>
		public static string[] Split(this string value, string separator)
		{
			if (value == null)
			{
				throw new System.ArgumentNullException ("value");
			}
			if (string.IsNullOrEmpty (separator))
			{
				throw new System.ArgumentException ("separator");
			}

			return value.Split (new string[] { separator }, System.StringSplitOptions.None);
		}
	}
}
