//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

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

			int schemeEnd = fullUri.IndexOf ("://");

			if (schemeEnd < 0)
			{
				throw new System.UriFormatException ("No URI scheme specified");
			}

			this.Scheme = fullUri.Substring (0, schemeEnd);

			string part1 = fullUri.Substring (schemeEnd+3);
			string part2;

			int posPart2 = part1.IndexOf ('/');

			if (posPart2 < 0)
			{
				throw new System.UriFormatException ("Missing host/path separator");
			}

			part2 = part1.Substring (posPart2+1);
			part1 = part1.Substring (0, posPart2);

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


		/// <summary>
		/// Gets or sets the URI scheme (such as <c>file</c> or <c>http</c>).
		/// </summary>
		/// <value>The URI scheme.</value>
		public string							Scheme
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
			buffer.Append ("://");

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

			buffer.Append ("/");

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


		private string path;
	}
}
