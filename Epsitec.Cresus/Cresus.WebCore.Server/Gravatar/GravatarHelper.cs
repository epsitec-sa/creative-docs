//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Samuel LOUP, Maintainer: Samuel LOUP

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.WebCore.Server.Gravatar
{
	public class GravatarHelper
	{
		public static string GetGravatarUrl(string email, GravatarImageSize size)
		{
			string gravatarHash = GravatarHelper.GetGravatarHash (email);
			string gravatarSize = new string (size.ToString ().Where (x => char.IsDigit (x)).ToArray ());

			return GravatarHelper.GravatarUrl + gravatarHash + "?s=" + gravatarSize + "&r=PG&d=mm";
		}

		private static string GetGravatarHash(string email)
		{
			if (string.IsNullOrWhiteSpace (email))
			{
				email = GravatarHelper.DefaultEmail;
			}

			var md5Hasher  = System.Security.Cryptography.MD5.Create ();
			var emailBytes = System.Text.Encoding.Default.GetBytes (email);
			var emailHash  = md5Hasher.ComputeHash (emailBytes);

			return string.Join ("", emailHash.Select (x => x.ToString ("x2")));
		}

		public const string GravatarUrl  = "https://www.gravatar.com/avatar/";
		public const string DefaultEmail = "meu@email.com";
		public const string BadgeSymbol  = "&#9679;";
	}
}

