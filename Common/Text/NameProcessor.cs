//	Copyright © 2012-2017, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Marc BETTEX

using System.Collections.Generic;
using System.Linq;
using System.Text;
using Epsitec.Common.Support;

namespace Epsitec.Common.Text
{
    public static class NameProcessor
    {
        public static bool IsConsonant(char c)
        {
            c = char.ToUpperInvariant(Epsitec.Common.Types.Converters.TextConverter.StripAccent(c));

            switch (c)
            {
                case 'A':
                case 'E':
                case 'I':
                case 'O':
                case 'U':
                case 'Y':
                    return false;
                default:
                    return c >= 'A' && c <= 'Z';
            }
        }

        public static bool IsVowel(char c)
        {
            c = char.ToUpperInvariant(Epsitec.Common.Types.Converters.TextConverter.StripAccent(c));

            switch (c)
            {
                case 'A':
                case 'E':
                case 'I':
                case 'O':
                case 'U':
                case 'Y':
                    return true;
                default:
                    return false;
            }
        }

        public static string GetAbbreviatedFirstName(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return name;
            }

            // For the abbreviation, we will keep only the first part of the name, i.e. everything
            // that comes before the first whitespace in the name.

            int? whitespaceIndex = null;

            for (int i = 0; i < name.Length; i++)
            {
                if (char.IsWhiteSpace(name[i]))
                {
                    whitespaceIndex = i;
                    break;
                }
            }

            if (whitespaceIndex.HasValue)
            {
                name = name.Substring(0, whitespaceIndex.Value);
            }

            // Now we want to abbreviate each token and keep the token separators. A token is
            // defined as a sequence of letters (with accents). They are separated by separators,
            // which are everything else (usually '-').

            var result = new StringBuilder();
            var token = new StringBuilder();

            // Here we store the first symbol of the current token, so that when we reach its end, we
            // can make the abbreviation.
            char? firstTokenSymbol = null;

            // Here we store the fact whether the token has one or more symbols, so that we know
            // whether or not we must put a dot in the abbreviation.
            bool singleLetterToken = true;

            for (int i = 0; i < name.Length; i++)
            {
                var c = name[i];

                bool isPartOfToken = NameProcessor.IsPartOfToken(c);
                bool isLastSymbol = (i == name.Length - 1);

                // Process the current symbol as part of the token if it is.
                if (isPartOfToken)
                {
                    if (firstTokenSymbol.HasValue)
                    {
                        singleLetterToken = false;
                    }
                    else
                    {
                        firstTokenSymbol = c;
                        singleLetterToken = true;
                    }
                    token.Append(c);
                }

                // Finish the token if the current symbol is not part of a token or if it is the
                // last symbol in the name.
                if (!isPartOfToken || isLastSymbol)
                {
                    if (firstTokenSymbol.HasValue)
                    {
                        if (singleLetterToken)
                        {
                            result.Append(firstTokenSymbol.Value);
                        }
                        else
                        {
                            result.Append(NameProcessor.ShortenFirstName(token.ToString()));
                            result.Append('.');
                        }

                        firstTokenSymbol = null;
                        token.Length = 0;
                    }
                }

                // Adds the current symbol, if it is a token separator.
                if (!isPartOfToken)
                {
                    result.Append(c);
                }
            }

            return result.ToString();
        }

        private static string ShortenFirstName(string name)
        {
            if (name.Length > 2)
            {
                var prefix = name.Substring(0, 3);
                switch (prefix.ToUpperInvariant())
                {
                    case "SCH":
                    case "CHR":
                    case "SHR":
                    case "SHL":
                        return prefix;
                }
            }
            if (name.Length > 1)
            {
                var letter1 = name[0];
                var letter2 = char.ToUpperInvariant(name[1]);

                if (letter2 == 'H')
                {
                    return name.Substring(0, 2);
                }

                if (NameProcessor.IsConsonant(letter1))
                {
                    switch (letter2)
                    {
                        case 'R':
                        case 'L':
                            return name.Substring(0, 2);
                    }
                }
            }
            return name.Substring(0, 1);
        }

        private static bool IsPartOfToken(char c)
        {
            var converted = StringUtils.RemoveDiacritics(c.ToString());

            for (int i = 0; i < converted.Length; i++)
            {
                if (!char.IsLetter(converted[i]))
                {
                    return false;
                }
            }

            return true;
        }

        public static string GetShortenedLastName(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return name;
            }

            // We keep only the part of the name before the first space. The only exception is that
            // we also keep the particules that are separated from the main name by a space. So we
            // consider as particules all names that have at most 5 characters that are at the start
            // of the name. Note that we can have more than one particule.
            // I chose 5 as the length of the particule because I looked that on Wikipedia and they
            // have examples of particules in severy languages. The longest ones where Dell', Dall'
            // and Della (in Italian).
            // This simple algorithm will of course produce false positives, because it won't
            // shorten some names that could be. But it will probably never shorten an name that
            // shouldn't.

            // Here we store the size of the shortened version of the name.
            var shortSize = 0;

            // Here we store the size of the current token, that we use to check if it is  particule
            // or not.
            var tokenSize = 0;

            for (int i = 0; i < name.Length; i++)
            {
                if (name[i] == ' ')
                {
                    // This is a separator, so we must terminate the token.

                    if (tokenSize <= 5)
                    {
                        // The current token is a particule. So we reset the particule size.
                        tokenSize = 0;
                        shortSize++;
                    }
                    else
                    {
                        // The current token is not a particule, so we can return.
                        break;
                    }
                }
                else
                {
                    // Regular symbol, so we increment the sizes.
                    shortSize++;
                    tokenSize++;
                }
            }

            return name.Substring(0, shortSize);
        }

        public static List<string> FilterLastNamePseudoDuplicates(IEnumerable<string> lastNames)
        {
            // Pseudo duplicates are names that contains another name. Like when we have a family
            // where the wife has kept its maiden name and appended the name of her husband to it.
            // For instance the husband is called "Albert Dupond" and the wife is called "Ginette
            // Dupond-Dupuis" or "Ginette Dupuis-Dupond".

            var originalNames = lastNames.Distinct().ToList();

            // Order the names by size, so that we know that a name can only be included in names
            // that are after it in the list.

            var normalizedNames = originalNames
                .OrderBy(n => n.Length)
                .Select(x => new { Original = x, Normalized = NameProcessor.NormalizeName(x) })
                .ToList();

            for (int i = 0; i < normalizedNames.Count; i++)
            {
                var shorter = normalizedNames[i].Normalized;
                var length = shorter.Length;

                for (int j = i + 1; j < normalizedNames.Count; j++)
                {
                    var longer = normalizedNames[j].Normalized;

                    if (longer == shorter)
                    {
                        // Exact duplicate based on lower-case accent-stripped name ("André" =
                        // "Andre", "von Siebenthal" = "Von Siebenthal").
                    }
                    else
                    {
                        int pos = longer.IndexOf(shorter);

                        //	nothing in common
                        if (pos < 0)
                        {
                            continue;
                        }

                        int end = pos + shorter.Length;

                        if (
                            ((end < longer.Length) && (longer[end] != ' '))
                            || ((pos > 0) && longer[pos - 1] != ' ')
                        )
                        {
                            // The longer word does not start or end with the short name, nor is it
                            // part of the longer name...

                            continue;
                        }
                    }

                    originalNames.Remove(normalizedNames[j].Original);
                    normalizedNames.RemoveAt(j);
                }
            }

            return originalNames;
        }

        private static string NormalizeName(string name)
        {
            return Epsitec
                .Common.Types.Converters.TextConverter.ConvertToLowerAndStripAccents(name)
                .Replace('-', ' ');
        }
    }
}
