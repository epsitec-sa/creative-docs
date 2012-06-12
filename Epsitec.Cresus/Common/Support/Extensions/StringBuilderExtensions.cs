//	Copyright © 2011-2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Common.Support.Extensions
{
	/// <summary>
	/// The <c>StringBuilderExtensions</c> class provides extension methods for the
	/// <see cref="System.Text.StringBuilder"/> class.
	/// </summary>
	public static class StringBuilderExtensions
	{
		/// <summary>
		/// Appends the items of the collection while joining them with the separator.
		/// </summary>
		/// <param name="buffer">The buffer.</param>
		/// <param name="separator">The separator.</param>
		/// <param name="collection">The collection.</param>
		public static void AppendJoin(this System.Text.StringBuilder buffer, string separator, IEnumerable<string> collection)
		{
			bool first = true;

			foreach (var item in collection)
			{
				if (first)
				{
					first = false;
				}
				else
				{
					buffer.Append (separator);
				}

				buffer.Append (item);
			}
		}

		/// <summary>
		/// Checks if the buffer ends with the specified value.
		/// </summary>
		/// <param name="buffer">The buffer.</param>
		/// <param name="value">The value.</param>
		/// <returns><c>true</c> if the buffer ends with the specified value; otherwise, <c>false</c>.</returns>
		public static bool EndsWith(this System.Text.StringBuilder buffer, string value)
		{
			if (buffer == null)
			{
				return false;
			}
			if (string.IsNullOrEmpty (value))
			{
				return true;
			}

			int n = buffer.Length;

			if (value.Length > n)
			{
				return false;
			}

			for (int i = value.Length; i > 0; )
			{
				char c1 = value[--i];
				char c2 = buffer[--n];

				if (c1 != c2)
				{
					return false;
				}
			}

			return true;
		}

		/// <summary>
		/// Gets the last character in the buffer, after simplification.
		/// </summary>
		/// <param name="buffer">The buffer.</param>
		/// <returns>The last character or <c>0</c> if the simplified string is empty.</returns>
		public static char LastCharacterOfSimpleText(this System.Text.StringBuilder buffer)
		{
			string text = buffer.ToString ();
			return text.LastCharacterOfSimpleText ();
		}
	}
}
