/*
This file is part of CreativeDocs.

Copyright © 2003-2024, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland

CreativeDocs is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
any later version.

CreativeDocs is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/


using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Text;
using Epsitec.Common.Types;

namespace Epsitec.Common.Support.Extensions
{
    /// <summary>
    /// The <c>StringExtensions</c> class contains some useful extension method for instances of
    /// <see cref="System.String"/>.
    /// </summary>
    public static class StringExtensions
    {
        public static string JoinNonEmpty(string separator, params string[] values)
        {
            separator.ThrowIfNull("separator");
            values.ThrowIfNull("values");

            var buffer = new System.Text.StringBuilder();

            foreach (var value in values)
            {
                if (string.IsNullOrEmpty(value))
                {
                    continue;
                }

                if (buffer.Length > 0)
                {
                    buffer.Append(separator);
                }

                buffer.Append(value);
            }

            return buffer.ToString();
        }

        public static string SubstringStart(this string text, int length)
        {
            if (text == null)
            {
                return null;
            }
            else
            {
                return text.Substring(0, length);
            }
        }

        public static string SubstringEnd(this string text, int length)
        {
            if (text == null)
            {
                return null;
            }
            else
            {
                int count = text.Length;
                int take = System.Math.Min(count, length);
                int skip = count - take;

                return text.Substring(skip, take);
            }
        }

        public static bool IsNullOrWhiteSpace(this string text)
        {
            return string.IsNullOrWhiteSpace(text);
        }

        public static bool StartsWith(this string text, char character)
        {
            if ((text != null) && (text.Length > 0))
            {
                return text[0] == character;
            }
            else
            {
                return false;
            }
        }

        public static string StripSuffix(this string text, string suffix)
        {
            if (string.IsNullOrEmpty(suffix))
            {
                return text;
            }

            if ((string.IsNullOrEmpty(text)) || (text.EndsWith(suffix) == false))
            {
                throw new System.ArgumentException(
                    string.Format("Suffix {0} not found in {1}", suffix, text),
                    "suffix"
                );
            }

            return text.Substring(0, text.Length - suffix.Length);
        }

        public static string Replace(
            this string text,
            string pattern,
            string replacement,
            System.StringComparison comparison
        )
        {
            if (comparison == System.StringComparison.Ordinal)
            {
                return text.Replace(pattern, replacement);
            }

            int pos = 0;

            while (true)
            {
                int hit = text.IndexOf(pattern, pos, comparison);

                if (hit < 0)
                {
                    return text;
                }

                string before = text.Substring(0, hit);
                string after = text.Substring(hit + pattern.Length);

                text = string.Concat(before, replacement, after);

                pos = hit + replacement.Length;
            }
        }

        public static int IndexOfAny(this string text, params string[] patterns)
        {
            var indexes = patterns.Select(x => text.IndexOf(x)).Where(x => x > -1);

            if (indexes.IsEmpty())
            {
                return -1;
            }

            return indexes.Min();
        }

        /// <summary>
        /// Determines whether the specified string contains any of the
        /// words, using space, non-breaking space, tab and newlines as
        /// delimiters.
        /// </summary>
        /// <param name="value">The string.</param>
        /// <param name="words">The words to look for.</param>
        /// <returns>
        ///   <c>true</c> if the specified string contains any of the words; otherwise, <c>false</c>.
        /// </returns>
        public static bool ContainsAnyWords(this string value, params string[] words)
        {
            return value.ContainsAnyWords(StringExtensions.spaceSeparators, words);
        }

        /// <summary>
        /// Determines whether the specified string contains any of the
        /// words, using the given separator characters.
        /// </summary>
        /// <param name="value">The string.</param>
        /// <param name="separators">The separators.</param>
        /// <param name="words">The words to look for.</param>
        /// <returns>
        ///   <c>true</c> if the specified string contains any of the words; otherwise, <c>false</c>.
        /// </returns>
        public static bool ContainsAnyWords(
            this string value,
            char[] separators,
            params string[] words
        )
        {
            var sourceTokens = value
                .ToLowerInvariant()
                .Split(separators, System.StringSplitOptions.RemoveEmptyEntries);
            var searchTokens = words.Select(x => x.ToLowerInvariant().Split(separators)).ToArray();

            for (int i = 0; i < sourceTokens.Length; i++)
            {
                foreach (var search in searchTokens)
                {
                    int j = -1;

                    while (++j < search.Length)
                    {
                        if (sourceTokens[i + j] != search[j])
                        {
                            goto skip;
                        }
                    }

                    return true;

                    skip:
                    continue;
                }
            }

            return false;
        }

        /// <summary>
        /// Truncates the specified string.
        /// </summary>
        /// <param name="value">The string.</param>
        /// <param name="maximumLength">The maximum length.</param>
        /// <returns>The possibly truncated string.</returns>
        public static string Truncate(this string value, int maximumLength)
        {
            if (string.IsNullOrEmpty(value))
            {
                return value;
            }
            else
            {
                if (value.Length <= maximumLength)
                {
                    return value;
                }
                else if (maximumLength <= 0)
                {
                    return "";
                }
                else
                {
                    return value.Substring(0, maximumLength);
                }
            }
        }

        /// <summary>
        /// Truncates the specified string and adds an ellipsis if needed.
        /// </summary>
        /// <param name="value">The string.</param>
        /// <param name="maximumLength">The maximum length.</param>
        /// <param name="ellipsis">The ellipsis (by default, uses the single character <c>…</c>).</param>
        /// <returns>The possibly truncated string.</returns>
        public static string TruncateAndAddEllipsis(
            this string value,
            int maximumLength,
            string ellipsis = "\u2026"
        )
        {
            if (string.IsNullOrEmpty(value))
            {
                return value;
            }
            else
            {
                if (value.Length <= maximumLength)
                {
                    return value;
                }
                else
                {
                    if (ellipsis.Length > maximumLength)
                    {
                        throw new System.ArgumentException(
                            "Ellipsis is longer than the maximum length"
                        );
                    }

                    return string.Concat(
                        value.Substring(0, maximumLength - ellipsis.Length),
                        ellipsis
                    );
                }
            }
        }

        /// <summary>
        /// Determines whether the string contains the search text at the specified position.
        /// </summary>
        /// <param name="text">The string.</param>
        /// <param name="search">The search text.</param>
        /// <param name="pos">The position where the search is done.</param>
        /// <returns>
        ///   <c>true</c> if the string contains the search text at the specified position; otherwise, <c>false</c>.
        /// </returns>
        public static bool ContainsAtPosition(this string text, string search, int pos)
        {
            text.ThrowIfNull("text");
            search.ThrowIfNull("search");

            if ((pos < 0) || (pos > text.Length))
            {
                return false;
            }

            int i = 0;

            while (i < search.Length)
            {
                if (pos >= text.Length)
                {
                    return false;
                }

                if (text[pos++] != search[i++])
                {
                    return false;
                }
            }

            return true;
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
                throw new System.ArgumentNullException("value");
            }

            return StringExtensions.alphaNumRegex.IsMatch(value);
        }

        /// <summary>
        /// Checks that <paramref name="value"/> is an alpha <see cref="System.String"/>,
        /// i.e. that it is empty or that it contains only lower case letters and upper case letters.
        /// </summary>
        /// <param name="value">The <see cref="System.String"/> to check.</param>
        /// <returns><c>true</c> if <paramref name="value"/> is alpha <c>false</c> if it isn't.</returns>
        /// <exception cref="System.ArgumentNullException">If <paramref name="value"/> is <c>null</c>.</exception>
        public static bool IsAlpha(this string value)
        {
            if (value == null)
            {
                throw new System.ArgumentNullException("value");
            }

            return StringExtensions.alphaRegex.IsMatch(value);
        }

        /// <summary>
        /// Checks that <paramref name="value"/> is a numeric <see cref="System.String"/>,
        /// i.e. that it is empty or that it contains only digits.
        /// </summary>
        /// <param name="value">The <see cref="System.String"/> to check.</param>
        /// <returns><c>true</c> if <paramref name="value"/> is numeric <c>false</c> if it isn't.</returns>
        /// <exception cref="System.ArgumentNullException">If <paramref name="value"/> is <c>null</c>.</exception>
        public static bool IsNumeric(this string value)
        {
            if (value == null)
            {
                throw new System.ArgumentNullException("value");
            }

            for (int i = 0; i < value.Length; i++)
            {
                if (!char.IsDigit(value[i]))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Determines whether the specified value is an integer.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        ///   <c>true</c> if the specified value is an integer; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsInteger(this string value)
        {
            if (value == null)
            {
                throw new System.ArgumentNullException("value");
            }

            bool valid = false;

            for (int i = 0; i < value.Length; i++)
            {
                char c = value[i];

                if ((c < '0') || (c > '9'))
                {
                    if (i == 0)
                    {
                        if ((c == '+') || (c == '-') || (c == (char)Unicode.Code.MinusSign))
                        {
                            continue;
                        }
                    }

                    return false;
                }

                valid = true;
            }

            return valid;
        }

        /// <summary>
        /// Determines whether the specified value is a decimal.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        ///   <c>true</c> if the specified value is a decimal; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsDecimal(this string value)
        {
            if (value == null)
            {
                throw new System.ArgumentNullException("value");
            }

            bool valid = false;
            bool dot = false;

            for (int i = 0; i < value.Length; i++)
            {
                char c = value[i];

                if ((c < '0') || (c > '9'))
                {
                    if (i == 0)
                    {
                        if ((c == '+') || (c == '-') || (c == (char)Unicode.Code.MinusSign))
                        {
                            continue;
                        }
                    }
                    if ((c == '.') || (c == ','))
                    {
                        if (dot)
                        {
                            return false;
                        }
                        dot = true;
                        continue;
                    }

                    return false;
                }

                valid = true;
            }

            return valid;
        }

        public static System.Tuple<string, string> SplitAfter(
            this string value,
            System.Predicate<char> predicate
        )
        {
            int pos = 0;
            int len = value == null ? 0 : value.Length;

            while (pos < len)
            {
                if (predicate(value[pos]))
                {
                    pos++;
                }
                else
                {
                    break;
                }
            }

            return value.SplitAfter(pos);
        }

        public static System.Tuple<string, string> SplitAfter(this string value, int pos)
        {
            if (value == null)
            {
                return new System.Tuple<string, string>(null, null);
            }
            else if (pos == value.Length)
            {
                return new System.Tuple<string, string>(value, "");
            }
            else if (pos == 0)
            {
                return new System.Tuple<string, string>("", value);
            }
            else
            {
                return new System.Tuple<string, string>(
                    value.Substring(0, pos),
                    value.Substring(pos)
                );
            }
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
                throw new System.ArgumentNullException("value");
            }
            if (string.IsNullOrEmpty(separator))
            {
                throw new System.ArgumentException("separator");
            }

            return value.Split(new string[] { separator }, System.StringSplitOptions.None);
        }

        /// <summary>
        /// Gets the first token found in the text; the text gets split at every
        /// occurrence of the separator.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="separator">The separator.</param>
        /// <returns>The first token found in the text. If the text is <c>null</c>, returns an empty string.</returns>
        public static string FirstToken(this string text, string separator)
        {
            if ((string.IsNullOrEmpty(text)) || (string.IsNullOrEmpty(separator)))
            {
                return "";
            }

            int pos = text.IndexOf(separator);

            if (pos < 0)
            {
                return text;
            }
            else
            {
                return text.Substring(0, pos);
            }
        }

        /// <summary>
        /// Removes the first token in the text; the text gets split at every
        /// occurrence of the separator.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="separator">The separator.</param>
        /// <returns>The text without the first token. If the text is <c>null</c>, returns an empty string.</returns>
        public static string RemoveFirstToken(this string text, string separator)
        {
            if ((string.IsNullOrEmpty(text)) || (string.IsNullOrEmpty(separator)))
            {
                return text ?? "";
            }

            int pos = text.IndexOf(separator);

            if (pos < 0)
            {
                return "";
            }
            else
            {
                return text.Substring(pos + separator.Length);
            }
        }

        public static string Join(this IEnumerable<string> strings, string separator)
        {
            strings.ThrowIfNull("strings");
            separator.ThrowIfNull("separator");

            return string.Join(separator, strings.ToArray());
        }

        public static string JoinNonEmpty(this IEnumerable<string> strings, string separator)
        {
            strings.ThrowIfNull("strings");
            separator.ThrowIfNull("separator");

            return StringExtensions.JoinNonEmpty(separator, strings.ToArray());
        }

        public static int CountOccurences(this string text, string substring)
        {
            substring.ThrowIfNullOrEmpty("substring");

            int nbOccurences = 0;

            for (int i = 0; i + substring.Length <= text.Length; i++)
            {
                bool isOccurence = true;

                for (int j = 0; j < substring.Length && isOccurence; j++)
                {
                    isOccurence = text[i + j] == substring[j];
                }

                if (isOccurence)
                {
                    nbOccurences++;
                }
            }

            return nbOccurences;
        }

        public static char LastCharacter(this string text)
        {
            int n = text.Length - 1;
            return n < 0 ? (char)0 : text[n];
        }

        public static char FirstCharacter(this string text)
        {
            int n = text.Length;
            return n < 1 ? (char)0 : text[0];
        }

        public static char LastCharacterOfSimpleText(this string text)
        {
            return FormattedText.Unescape(text).LastCharacter();
        }

        public static char FirstCharacterOfSimpleText(this string text)
        {
            return FormattedText.Unescape(text).FirstCharacter();
        }

        public static bool IsPunctuationMark(this char c)
        {
            // Exclut le caractère '/', pour permettre de numéroter une facture "1000 / 45 / bg" (par exemple).

            switch (c)
            {
                case ',':
                case ';':
                case '.':
                case ':':
                case '!':
                case '?':
                    return true;

                default:
                    return false;
            }
        }

        public static bool IsAllUpperCase(this string text)
        {
            text.ThrowIfNull("text");

            for (int i = 0; i < text.Length; i++)
            {
                if (char.IsLower(text, i))
                {
                    return false;
                }
            }

            return true;
        }

        public static string TrimSpacesAndDashes(this string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return text;
            }

            int posDash = text.IndexOf('-');
            int posSpace = text.IndexOf(' ');

            if ((posDash < 0) && (posSpace < 0))
            {
                return text;
            }

            var buffer = new System.Text.StringBuilder();
            char last = '*';

            foreach (char c in text)
            {
                if (c == ' ')
                {
                    if (buffer.Length == 0)
                    {
                        continue;
                    }

                    if ((last == '-') || (last == ' '))
                    {
                        continue;
                    }
                }
                else if (c == '-')
                {
                    if (last == '-')
                    {
                        continue;
                    }
                    if (last == ' ')
                    {
                        buffer.Length = buffer.Length - 1;
                    }
                    if (buffer.Length == 0)
                    {
                        continue;
                    }
                }

                buffer.Append(c);
                last = c;
            }

            if ((last == ' ') || (last == '-'))
            {
                buffer.Length = buffer.Length - 1;
            }

            return buffer.ToString();
        }

        public static string CapitalizeFirstLetter(this string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return value;
            }

            if (char.IsUpper(value[0]))
            {
                return value;
            }

            var chars = value.ToArray();

            chars[0] = char.ToUpper(chars[0]);

            return new string(chars);
        }

        static StringExtensions()
        {
            StringExtensions.alphaRegex = new Regex("^[a-zA-Z]*$", RegexOptions.Compiled);
            StringExtensions.alphaNumRegex = new Regex("^[a-zA-Z0-9]*$", RegexOptions.Compiled);
            StringExtensions.spaceSeparators = new char[] { ' ', '\t', '\n', '\r', '\u00A0' };
        }

        private static readonly Regex alphaNumRegex;
        private static readonly Regex alphaRegex;
        private static readonly char[] spaceSeparators;
    }
}
