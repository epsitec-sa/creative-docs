//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Marc BETTEX

using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Types.Collections;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Common.Support
{
	/// <summary>
	/// The <see cref="EscapeSplit"/> class provides methods to join several <see cref="System.String"/>
	/// together with a separator and split them back afterwards.
	/// </summary>
	/// <remarks>
	/// What makes these method special is that their result remains consistent even if some of the
	/// <see cref="System.String"/> joined together contain the separator char. In order to achieve
	/// this, the methods ask for a separator char and an escape char. The separator is used to
	/// separate the input <see cref="System.String"/> and the escape char is used to escape each
	/// occurrence of the separator char and the escape char within the input
	/// <see cref="System.String"/>. In addition a prefix is added to the result of the join to make
	/// the difference between an empty input sequence and an input sequence with a single empty
	/// element.
	/// </remarks>
	public static class StringPacker
	{
		/// <summary>
		/// Groups all the given <see cref="System.String"/> into a single <see cref="System.String"/>,
		/// which can later be expanded with the <see cref="StringPacker.Unpack"/> method.
		/// <param name="strings">The <see cref="System.String"/> to join.</param>
		/// <param name="separatorChar">The <see cref="char"/> used to separate the <see cref="System.String"/>.</param>
		/// <param name="escapeChar">The <see cref="char"/> used to escape itself and the separator.</param>
		/// <returns>A single <see cref="System.String"/> that contains all the input ones.</returns>
		public static string Pack(IEnumerable<string> strings, char separatorChar = ';', char escapeChar = '\\')
		{
			strings.ThrowIfNull ("strings");
			separatorChar.ThrowIf (c => c == escapeChar, "Escape and separator char are the same.");

			var stringsCopy = strings.ToList ();

			if (stringsCopy.Any ())
			{
				return separatorChar + StringPacker.Join (stringsCopy, separatorChar, escapeChar);
			}
			else
			{
				//	Encode the empty collection:

				return escapeChar.ToString ();
			}
		}

		/// <summary>
		/// Expands the given <see cref="System.String"/> which is the result of the
		/// <see cref="StringPacker.Pack"/> method into the original <see cref="System.String"/>
		/// sequence.
		/// <param name="data">The result of the <see cref="StringPacker.Pack"/> function.</param>
		/// <param name="separatorChar">The <see cref="char"/> used to separate the <see cref="System.String"/>.</param>
		/// <param name="escapeChar">The <see cref="char"/> used to escape itself and the separator.</param>
		/// <returns>The sequence of <see cref="System.String"/>.</returns>
		public static IEnumerable<string> Unpack(string data, char separatorChar = ';', char escapeChar = '\\')
		{
			data.ThrowIfNullOrEmpty ("data");
			data.ThrowIf (s => s[0] != separatorChar && s[0] != escapeChar, "Invalid data");
			separatorChar.ThrowIf (c => c == escapeChar, "Escape and separator char are the same.");

			char prefix = data[0];

			//	Prefix can either be the escape charater (the collection is empty) or the separator
			//	(the collection contains at least one item) :

			if (prefix == escapeChar)
			{
				return EmptyEnumerable<string>.Instance;
			}
			
			if (prefix == separatorChar)
			{
				return StringPacker.Split (data.Substring (1), separatorChar, escapeChar)
					.Select (s => StringPacker.Unescape (s, separatorChar, escapeChar));
			}

			throw new System.ArgumentException ();
		}

		/// <summary>
		/// Groups all the given <see cref="System.String"/> into a single <see cref="byte[]"/>,
		/// which can later be expanded with the <see cref="StringPacker.UnpackFromBytes"/> method.
		/// <param name="strings">The <see cref="System.String"/> to join.</param>
		/// <param name="separatorChar">The <see cref="char"/> used to separate the <see cref="System.String"/>.</param>
		/// <param name="escapeChar">The <see cref="char"/> used to escape itself and the separator.</param>
		/// <returns>A single <see cref="byte[]"/> that contains all the input strings.</returns>
		public static byte[] PackToBytes(IEnumerable<string> strings, char separatorChar = ';', char escapeChar = '\\')
		{
			return System.Text.Encoding.UTF8.GetBytes (StringPacker.Pack (strings, separatorChar, escapeChar));
		}

		/// <summary>
		/// Expands the given <see cref="byte[]"/> which is the result of the
		/// <see cref="StringPacker.PackToBytes"/> method into the original <see cref="System.String"/>
		/// sequence.
		/// <param name="data">The result of the <see cref="StringPacker.Pack"/> function.</param>
		/// <param name="separatorChar">The <see cref="char"/> used to separate the <see cref="string"/>.</param>
		/// <param name="escapeChar">The <see cref="char"/> used to escape itself and the separator.</param>
		/// <returns>The sequence of <see cref="string"/>.</returns>
		public static IEnumerable<string> UnpackFromBytes(byte[] data, char separatorChar = ';', char escapeChar = '\\')
		{
			return StringPacker.Unpack (System.Text.Encoding.UTF8.GetString (data), separatorChar, escapeChar);
		}

		/// <summary>
		/// Joins all the given <see cref="string"/> into a single <see cref="string"/>,
		/// 
		/// </summary>
		/// <param name="strings">The <see cref="System.String"/> to join.</param>
		/// <param name="separatorChar">The <see cref="char"/> used to separate the <see cref="System.String"/>.</param>
		/// <param name="escapeChar">The <see cref="char"/> used to escape itself and the separator.</param>
		/// <returns>A single <see cref="System.String"/> that contains all the input ones.</returns>
		private static string Join(IEnumerable<string> strings, char separatorChar, char escapeChar)
		{
			var processedStrings = strings.Select (s => StringPacker.Escape (s, separatorChar, escapeChar)).ToArray ();

			return string.Join ("" + separatorChar, processedStrings);
		}

		/// <summary>
		/// Splits the given <see cref="System.String"/> into a sequence of
		/// <see cref="System.String"/>.
		/// </summary>
		/// <remarks>
		/// This method must be called with the result of the <see cref="StringPacker.Pack"/> function
		/// with the same separator and escape char.
		/// </remarks>
		/// <param name="data">The result of the <see cref="StringPacker.Pack"/> function.</param>
		/// <param name="separatorChar">The <see cref="char"/> used to separate the <see cref="System.String"/>.</param>
		/// <param name="escapeChar">The <see cref="char"/> used to escape itself and the separator.</param>
		/// <returns>The sequence of <see cref="System.String"/>.</returns>
		private static IEnumerable<string> Split(string data, char separatorChar, char escapeChar)
		{
			List<int> separatorIndexes = new List<int> ();

			for (int i = 0; i < data.Length; i++)
			{
				char currentChar = data[i];

				if (currentChar == separatorChar)
				{
					separatorIndexes.Add (i);
				}
				else if (currentChar == escapeChar)
				{
					if (i + 1 >= data.Length)
					{
						throw new System.ArgumentException ();
					}

					char nextChar = data[i + 1];

					if (nextChar != separatorChar && nextChar != escapeChar)
					{
						throw new System.ArgumentException ();
					}

					i++;
				}
			}

			int segmentStart = 0;

			foreach (int separatorIndex in separatorIndexes)
			{
				yield return data.Substring (segmentStart, separatorIndex - segmentStart);

				segmentStart = separatorIndex + 1;
			}

			yield return data.Substring (segmentStart);
		}

		/// <summary>
		/// Escapes the occurrences of <paramref name="separatorChar"/> and <paramref name="escapeChar"/>
		/// within <paramref name="s"/> with <paramref name="escapeChar"/>.
		/// </summary>
		/// <param name="s">The <see cref="System.String"/> to escape.</param>
		/// <param name="separatorChar">The <see cref="char"/> to escape.</param>
		/// <param name="escapeChar">The <see cref="cbar"/> used to escape.</param>
		/// <returns>The escaped <see cref="System.String"/>.</returns>
		private static string Escape(string s, char separatorChar, char escapeChar)
		{
			if (string.IsNullOrEmpty (s))
			{
				return s;
			}
			else
			{
				return s
					.Replace (escapeChar.ToString (), escapeChar.ToString () + escapeChar.ToString ())
					.Replace (separatorChar.ToString (), escapeChar.ToString () + separatorChar.ToString ());
			}
		}

		/// <summary>
		/// Unescapes a <see cref="System.String"/> escaped with the <see cref="StringPacker.Escape"/>
		/// function.
		/// </summary>
		/// <param name="s">The <see cref="System.String"/> to unescape.</param>
		/// <param name="separatorChar">The <see cref="char"/> to escape.</param>
		/// <param name="escapeChar">The <see cref="cbar"/> used to escape.</param>
		/// <returns>The unescaped <see cref="System.String"/>.</returns>
		private static string Unescape(string s, char separatorChar, char escapeChar)
		{
			if (string.IsNullOrEmpty (s))
			{
				return s;
			}
			else
			{
				return s
					.Replace (escapeChar.ToString () + separatorChar.ToString (), separatorChar.ToString ())
					.Replace (escapeChar.ToString () + escapeChar.ToString (), escapeChar.ToString ());
			}
		}
	}
}
