//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Common.IO
{
	public static class UriFormatter
	{
		public static FormattedText ToFormattedText(UriBuilder uri)
		{
			if (uri == null)
			{
				return FormattedText.Empty;
			}
			else
			{
				return UriFormatter.ToFormattedText (uri.ToString ());
			}
		}

		public static FormattedText ToFormattedText(string uri)
		{
			if (string.IsNullOrEmpty (uri))
			{
				return FormattedText.Empty;
			}

			var cleanUri = new UriBuilder (UriBuilder.FixScheme (uri));

			return new FormattedText (@"<a href=""" + cleanUri.ToString () + @""">" + FormattedText.FromSimpleText (uri) + "</a>");
			
		}
	}
}
