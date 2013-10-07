﻿//	Copyright © 2010-2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Epsitec.Common.Types.Converters;

namespace Epsitec.Common.IO
{
	/// <summary>
	/// The <c>UriBuilder</c> class encodes a full URI (very similar to what is done by
	/// <see cref="System.UriBuilder"/>, but without its limitations and bugs, as of
	/// .NET 4.0).
	/// </summary>
	public class UriBuilder : System.IEquatable<UriBuilder>
	{
		public UriBuilder()
		{
		}

		public UriBuilder(string fullUri)
		{
			if (string.IsNullOrEmpty (fullUri))
			{
				return;
			}

			var schemeName = UriBuilder.GetSchemeName (fullUri);

			if (schemeName.Length == 0)
			{
				throw new System.UriFormatException ("No URI scheme specified");
			}

			this.Scheme       = schemeName;
			this.SchemeSuffix = UriBuilder.GetSchemeSuffix (schemeName, fullUri);

			string part1 = fullUri.Substring (this.Scheme.Length + this.SchemeSuffix.Length);
			string part2 = "";

			int posPart2 = part1.IndexOf ('/');

			if (posPart2 >= 0)
			{
				part2 = part1.Substring (posPart2+1);
				part1 = part1.Substring (0, posPart2);
			}
			
			int posHost = part1.IndexOf ('@');

			if (posHost >= 0)
			{
				string part1User = part1.Substring (0, posHost);
				string part1Host = part1.Substring (posHost+1);

				this.SetupUserNameAndPassword (part1User);
				this.SetupHostAndPortNumber (part1Host);
			}
			else
			{
				this.SetupHostAndPortNumber (part1);
			}

			this.SetupPathAndQueryAndFragment (part2);
		}

		static UriBuilder()
		{
			string pattern;

			//	Taken from Phil Haack's blog, based on RFC 2821/2822.
			//	http://haacked.com/archive/2007/08/21/i-knew-how-to-validate-an-email-address-until-i.aspx

			pattern = @"^(?!\.)(""([^""\r\\]|\\[""\r\\])*""|" +
				/**/  @"([-a-z0-9!#$%&'*+\/=?^_`{|}~]|(?<!\.)\.)*)(?<!\.)" +
				/**/  @"@[a-z0-9][\w\.-]*[a-z0-9]\.[a-z][a-z\.]*[a-z]$";

			UriBuilder.mailValidationRegex = new Regex (pattern, RegexOptions.IgnoreCase | RegexOptions.Compiled);

			//	Common Regular Expressions (MSDN) for URL validation
			//	http://msdn.microsoft.com/en-us/library/ff650303.aspx

			pattern = @"^(ht|f)tp(s?)\:\/\/[0-9a-zA-Z]([-.\w]*[0-9a-zA-Z])*(:(0-9)*)*(\/?)([a-zA-Z0-9\-\.\?\,\'\/\\\+&%\$#_]*)?$";

			UriBuilder.urlValidationRegex = new Regex (pattern, RegexOptions.Compiled);

			//	Regular expression for Fully Qualified Domain Name (i.e. a name with a domain name)
			//	and maximum length names (63 chars)
			//	http://stackoverflow.com/questions/106179/regular-expression-to-match-hostname-or-ip-address

			pattern = @"^(([a-zA-Z0-9]|[a-zA-Z0-9][a-zA-Z0-9\-]{0,61}[a-zA-Z0-9])\.)+([A-Za-z][A-Za-z](|[A-Za-z0-9\-]{0,60}[A-Za-z0-9]))$";

			UriBuilder.hostFqdnValidationRegex = new Regex (pattern, RegexOptions.Compiled);
			
			//	Regular expression for IPV4 host names (such as 192.168.1.255)
			//	http://stackoverflow.com/questions/106179/regular-expression-to-match-hostname-or-ip-address

			pattern = @"^(([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])\.){3}([0-9]|[1-9][0-9]|1[0-9‌​]{2}|2[0-4][0-9]|25[0-5])$";

			UriBuilder.hostIpV4ValidationRegex = new Regex (pattern, RegexOptions.Compiled);
		}


		/// <summary>
		/// Gets or sets the URI scheme (such as <c>file</c> or <c>http</c>).
		/// </summary>
		/// <value>The URI scheme.</value>
		public string							Scheme
		{
			get;
			set;
		}

		public string							SchemeSuffix
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the port number. Use zero if no port number is specified.
		/// </summary>
		/// <value>The port number, between 0 and 65535.</value>
		public int								PortNumber
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the URI user name.
		/// </summary>
		/// <value>The URI user name.</value>
		public string							UserName
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the URI password.
		/// </summary>
		/// <value>The URI password.</value>
		public string							Password
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the URI host.
		/// </summary>
		/// <value>The URI host.</value>
		public string							Host
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the URI path. Backslashes will be replaced with forward slashes.
		/// </summary>
		/// <value>The URI path.</value>
		public string							Path
		{
			get
			{
				return this.path;
			}
			set
			{
				if (value == null)
				{
					this.path = null;
				}
				else
				{
					this.path = value.Replace ('\\', '/');
				}
			}
		}

		/// <summary>
		/// Gets or sets the fragment, which is the part after the "#" in the URI.
		/// </summary>
		/// <value>The fragment.</value>
		public string							Fragment
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the query, which is the part after the "?" in the URI.
		/// </summary>
		/// <value>The query.</value>
		public string							Query
		{
			get;
			set;
		}


		/// <summary>
		/// Determines whether the URI is a valid address, according to the mailto: protocol.
		/// </summary>
		/// <param name="fullUri">The full URI.</param>
		/// <returns>
		///   <c>true</c> if the specified full URI is a valid mailto: address; otherwise, <c>false</c>.
		/// </returns>
		public static bool IsValidMailTo(string fullUri)
		{
			if (string.IsNullOrEmpty (fullUri))
			{
				return false;
			}

			string prefix = "mailto:";

			if (fullUri.StartsWith (prefix))
			{
				var email = fullUri.Substring (prefix.Length);

				if (UriBuilder.mailValidationRegex.IsMatch (email))
				{
					return true;
				}
			}
			
			return false;
		}

		public static bool IsValidUrl(string fullUri)
		{
			if (string.IsNullOrEmpty (fullUri))
			{
				return false;
			}
			
			if (UriBuilder.urlValidationRegex.IsMatch (fullUri))
			{
				return true;
			}

			return false;
		}

		public static bool IsValidFullyQualifiedDomainNameUrl(string fullUri)
		{
			if (UriBuilder.IsValidUrl (fullUri))
			{
				var uri  = new UriBuilder (fullUri);
				var host = uri.Host;

				if ((UriBuilder.hostFqdnValidationRegex.IsMatch (host)) ||
					(UriBuilder.hostIpV4ValidationRegex.IsMatch (host)))
				{
					return true;
				}
			}

			return false;
		}

		public static string EncodePunyCode(string unicodeHostName)
		{
			//	Examples:
			//	
			//	- παράδειγμα.δοκιμή --> xn--hxajbheg2az3al.xn--jxalpdlp
			//	- bücher.com ---------> xn--bcher-kva.com
			//	- crésus.ch ----------> xn--crsus-csa.ch
			//
			//	See also http://en.wikipedia.org/wiki/Top-level_domain_name

			var mapping = new System.Globalization.IdnMapping ();
			return mapping.GetAscii (unicodeHostName);
		}

		public static string DecodePunyCode(string asciiHostName)
		{
			var mapping = new System.Globalization.IdnMapping ();
			return mapping.GetUnicode (asciiHostName);
		}

		public static string GetSchemeName(string fullUri)
		{
			if (string.IsNullOrEmpty (fullUri))
			{
				return "";
			}
			else
			{
				int schemeEnd = fullUri.IndexOf (':');
				
				if (schemeEnd < 0)
				{
					return "";
				}
				else
				{
					return fullUri.Substring (0, schemeEnd);
				}
			}
		}

		private static string GetSchemeSuffix(string schemeName, string fullUri)
		{
			int schemeNameLength = schemeName.Length;

			if (schemeNameLength == 0)
			{
				return "";
			}
			else
			{
				return fullUri.IndexOf ("://", schemeNameLength) == schemeNameLength ? "://" : ":";
			}
		}

		public static string FixScheme(string fullUri)
		{
			if (string.IsNullOrWhiteSpace (fullUri))
			{
				return null;
			}

			string officialSuffix = null;
			string officialName;
			string partialUri;
			
			var schemeName = UriBuilder.GetSchemeName (fullUri);

			if ((schemeName.Length == 0) ||
				(schemeName.StartsWith ("www")))
			{
				if ((!fullUri.StartsWith ("www")) &&
					(fullUri.Contains ("@")))
				{
					officialName   = "mailto";
					officialSuffix = ":";
				}
				else
				{
					officialName   = "http";
					officialSuffix = "://";
				}

				partialUri = fullUri;
			}
			else
			{
				var schemeSuffix = UriBuilder.GetSchemeSuffix (schemeName, fullUri);

				switch (schemeName)
				{
					case "http":
					case "https":
					case "ftp":
					case "file":
						officialName   = schemeName;
						officialSuffix = "://";
						break;
					
					default:
						officialName   = schemeName;
						officialSuffix = officialSuffix ?? schemeSuffix;
						break;
				}
				
				if (officialSuffix == schemeSuffix)
				{
					return fullUri;
				}

				partialUri = fullUri.Substring (schemeName.Length + schemeSuffix.Length);
			}

			return officialName + officialSuffix + partialUri;
		}


		public static string ConvertToAlphaNumericQueryArgument(string arg)
		{
			if (string.IsNullOrWhiteSpace (arg))
			{
				return "";
			}

			var buffer = new System.Text.StringBuilder ();

			bool escaped = true;

			foreach (var c in TextConverter.StripAccents (arg))
			{
				if (((c >= 'a') && (c <= 'z')) ||
					((c >= 'A') && (c <= 'Z')) ||
					((c >= '0') && (c <= '9')) ||
					((c == '-') || (c == ',') || (c == '.')))
				{
					buffer.Append (c);
					escaped = false;
				}
				else
				{
					if (escaped == false)
					{
						buffer.Append ('+');
						escaped = true;
					}
				}
			}

			return buffer.ToString ().TrimEnd ('+');
		}


		#region IEquatable<UriBuilder> Members

		public bool Equals(UriBuilder other)
		{
			if (other == null)
			{
				return false;
			}
			
			if ((this.Scheme == other.Scheme) && 
				(this.Path == other.Path))
			{
				return this.ToString () == other.ToString ();
			}

			return false;
		}

		#endregion
		
		public override string ToString()
		{
			var buffer = new System.Text.StringBuilder ();

			if (string.IsNullOrEmpty (this.Scheme))
			{
				throw new System.UriFormatException ("No URI scheme available");
			}

			buffer.Append (this.Scheme);
			buffer.Append (this.SchemeSuffix ?? "://");

			string separator = "";

			if (!string.IsNullOrEmpty (this.UserName))
			{
				buffer.Append (System.Uri.EscapeUriString (this.UserName));
				separator = "@";
			}
			if (!string.IsNullOrEmpty (this.Password))
			{
				buffer.Append (":");
				buffer.Append (System.Uri.EscapeUriString (this.Password));
				separator = "@";
			}

			buffer.Append (separator);

			if (!string.IsNullOrEmpty (this.Host))
			{
				buffer.Append (System.Uri.EscapeUriString (this.Host));
			}
			if (this.PortNumber != 0)
			{
				buffer.Append (":");
				buffer.Append (this.PortNumber.ToString (System.Globalization.CultureInfo.InvariantCulture));
			}

			if ((!string.IsNullOrEmpty (this.Path)) ||
				(!string.IsNullOrEmpty (this.Query)) ||
				(!string.IsNullOrEmpty (this.Fragment)))
			{
				buffer.Append ("/");
			}

			if (!string.IsNullOrEmpty (this.Path))
			{
				buffer.Append (System.Uri.EscapeUriString (this.Path));
			}

			if (!string.IsNullOrEmpty (this.Query))
			{
				buffer.Append ("?");
				buffer.Append (System.Uri.EscapeUriString (this.Query));
			}

			if (!string.IsNullOrEmpty (this.Fragment))
			{
				buffer.Append ("#");
				buffer.Append (System.Uri.EscapeUriString (this.Fragment));
			}

			return buffer.ToString ();
		}

		public override bool Equals(object obj)
		{
			return this.Equals (obj as UriBuilder);
		}

		public override int GetHashCode()
		{
			return ((this.Scheme == null) ? 0 : this.Scheme.GetHashCode ())
				 ^ ((this.Path == null)   ? 0 : this.Path.GetHashCode ());
		}


		
		private void SetupPathAndQueryAndFragment(string value)
		{
			int posQuery    = value.IndexOf ('?');
			int posFragment = value.IndexOf ('#');

			if (posQuery < 0)
			{
				if (posFragment < 0)
				{
					this.Path = System.Uri.UnescapeDataString (value);
				}
				else
				{
					this.Path     = System.Uri.UnescapeDataString (value.Substring (0, posFragment));
					this.Fragment = System.Uri.UnescapeDataString (value.Substring (posFragment+1));
				}
			}
			else
			{
				if (posFragment < 0)
				{
					this.Path  = System.Uri.UnescapeDataString (value.Substring (0, posQuery));
					this.Query = System.Uri.UnescapeDataString (value.Substring (posQuery+1));
				}
				else if (posQuery < posFragment)
				{
					this.Path     = System.Uri.UnescapeDataString (value.Substring (0, posQuery));
					this.Query    = System.Uri.UnescapeDataString (value.Substring (posQuery+1, posFragment-posQuery-1));
					this.Fragment = System.Uri.UnescapeDataString (value.Substring (posFragment+1));
				}
				else
				{
					throw new System.UriFormatException ("Query (?) must precede fragment (#)");
				}
			}
		}

		private void SetupUserNameAndPassword(string value)
		{
			int pos = value.IndexOf (':');

			if (pos < 0)
			{
				this.UserName = System.Uri.UnescapeDataString (value);
			}
			else
			{
				this.UserName = System.Uri.UnescapeDataString (value.Substring (0, pos));
				this.Password = System.Uri.UnescapeDataString (value.Substring (pos+1));
			}
		}

		private void SetupHostAndPortNumber(string value)
		{
			int pos = value.IndexOf (':');

			if (pos < 0)
			{
				this.Host = System.Uri.UnescapeDataString (value);
			}
			else
			{
				this.Host       = System.Uri.UnescapeDataString (value.Substring (0, pos));
				this.PortNumber = int.Parse (value.Substring (pos+1), System.Globalization.CultureInfo.InvariantCulture);
			}
		}


		private string							path;


		private static readonly Regex			mailValidationRegex;
		private static readonly Regex			urlValidationRegex;
		private static readonly Regex			hostFqdnValidationRegex;
		private static readonly Regex			hostIpV4ValidationRegex;
	}
}