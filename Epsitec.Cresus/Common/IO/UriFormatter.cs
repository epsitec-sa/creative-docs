//	Copyright © 2012-2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;

using System.Collections.Generic;
using System.Linq;

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
				return UriFormatter.ToFormattedText (uri.ToString (), target);
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
			if (string.IsNullOrEmpty (uri))
			{
				return FormattedText.Empty;
			}

			var cleanUri = new UriBuilder (UriBuilder.FixScheme (uri));
			var display  = UriFormatter.GetDisplayText (cleanUri);

			if (string.IsNullOrEmpty (target))
			{
				return new FormattedText (string.Format (@"<a href=""{0}"">{1}</a>", cleanUri.ToString (), display));
			}
			else
			{
				return new FormattedText (string.Format (@"<a href=""{0}"" target=""{1}"">{2}</a>", cleanUri.ToString (), target, display));
			}
		}
		
		private static FormattedText GetDisplayText(UriBuilder cleanUri)
		{
			var display  = new System.Text.StringBuilder ();
			var hostName = UriBuilder.DecodePunyCode (cleanUri.Host);
			var path     = string.IsNullOrEmpty (cleanUri.Path) ? "" : "/" + cleanUri.Path;

			switch (cleanUri.Scheme)
			{
				case "http":
				case "https":
					display.Append (hostName);
					display.Append (path);
					break;

				case "mailto":
					display.Append (cleanUri.UserName);
					display.Append ("@");
					display.Append (hostName);
					break;
				
				default:
					display.Append (cleanUri.Scheme);
					display.Append (cleanUri.SchemeSuffix);
					display.Append (hostName);
					display.Append (path);
					break;
			}
			
			return FormattedText.FromSimpleText (display.ToString ());
		}
	}
}
