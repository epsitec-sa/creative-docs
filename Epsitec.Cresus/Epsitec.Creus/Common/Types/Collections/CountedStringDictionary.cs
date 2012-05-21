//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Types.Collections
{
	/// <summary>
	/// The <c>CountedStringDictionary</c> can be used to count how many identical strings
	/// were added to the dictionary.
	/// </summary>
	public sealed class CountedStringDictionary : Dictionary<string, int>
	{
		public CountedStringDictionary()
			: this (separator: "@", format: "00")
		{
		}

		public CountedStringDictionary(string separator, string format)
		{
			this.format = string.Concat ("{0}", separator, "{1:", format, "}");
		}

		/// <summary>
		/// Gets the count of strings identical to the specified name, which were added
		/// by the <see cref="AddUnique"/> method.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <returns>The number of times <see cref="AddUnique"/> was called with this name.</returns>
		public int GetUniqueAddCount(string name)
		{
			int value;

			if (this.TryGetValue (name, out value))
			{
				return value;
			}
			else
			{
				return 0;
			}
		}


		/// <summary>
		/// Adds a string to the dictionary and produces a unique name. If the string was already
		/// in the dictionary, the unique string will be suffixed with a counter.
		/// </summary>
		/// <param name="name">The string.</param>
		/// <param name="format">The format (by default, uses the format string <c>{0}@{1:00}</c>).</param>
		/// <returns>A unique name based on the given string.</returns>
		public string AddUnique(string name, string format = null)
		{
			int value = this.GetUniqueAddCount (name);
			
			this[name] = value + 1;

			if (value == 0)
			{
				return name;
			}
			else
			{
				return string.Format (System.Globalization.CultureInfo.InvariantCulture, format ?? this.format, name, value);
			}
		}

		private readonly string format;
	}
}
