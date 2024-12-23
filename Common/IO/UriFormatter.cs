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


using Epsitec.Common.Types;

namespace Epsitec.Common.IO
{
    public static class UriFormatter
    {
        /// <summary>
        /// Converts the URI into a formatted text containing a link to the specified URI.
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <param name="target">The optional target for the &lt;a&gt; element.</param>
        /// <returns>The formatted text.</returns>
        public static FormattedText ToFormattedText(UriBuilder uri, string target = null)
        {
            if (uri == null)
            {
                return FormattedText.Empty;
            }
            else
            {
                return UriFormatter.ToFormattedText(uri.ToString(), target);
            }
        }

        /// <summary>
        /// Converts the URI into a formatted text containing a link to the specified URI.
        /// The displayed URI might be somewhat simplified.
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <param name="target">The optional target for the &lt;a&gt; element.</param>
        /// <returns>The formatted text.</returns>
        public static FormattedText ToFormattedText(string uri, string target = null)
        {
            uri = UriBuilder.FixScheme(uri);

            if (uri == null)
            {
                return FormattedText.Empty;
            }

            var cleanUri = new UriBuilder(uri);
            var display = UriFormatter.GetDisplayText(cleanUri);

            if (string.IsNullOrEmpty(target))
            {
                return new FormattedText(
                    string.Format(@"<a href=""{0}"">{1}</a>", cleanUri.ToString(), display)
                );
            }
            else
            {
                return new FormattedText(
                    string.Format(
                        @"<a href=""{0}"" target=""{1}"">{2}</a>",
                        cleanUri.ToString(),
                        target,
                        display
                    )
                );
            }
        }

        private static FormattedText GetDisplayText(UriBuilder cleanUri)
        {
            var display = new System.Text.StringBuilder();
            var hostName = UriBuilder.DecodePunyCode(cleanUri.Host);
            var path = string.IsNullOrEmpty(cleanUri.Path) ? "" : "/" + cleanUri.Path;

            switch (cleanUri.Scheme)
            {
                case "http":
                case "https":
                    display.Append(hostName);
                    display.Append(path);
                    break;

                case "mailto":
                    display.Append(cleanUri.UserName);
                    display.Append("@");
                    display.Append(hostName);
                    break;

                default:
                    display.Append(cleanUri.Scheme);
                    display.Append(cleanUri.SchemeSuffix);
                    display.Append(hostName);
                    display.Append(path);
                    break;
            }

            return FormattedText.FromSimpleText(display.ToString());
        }
    }
}
