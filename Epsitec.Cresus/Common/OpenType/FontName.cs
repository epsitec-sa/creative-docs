//	Copyright © 2005-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.OpenType
{
	/// <summary>
	/// The <c>FontName</c> structure stores the face and style name as a
	/// single object.
	/// </summary>
	[System.Serializable]
	public struct FontName : System.IComparable<FontName>, System.IEquatable<FontName>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="FontName"/> structure.
		/// </summary>
		/// <param name="face">The font face.</param>
		/// <param name="style">The font style.</param>
		public FontName(string face, string style)
		{
			this.face  = face;
			this.style = style;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="FontName"/> structure.
		/// </summary>
		/// <param name="fontIdentity">The font identity.</param>
		public FontName(FontIdentity fontIdentity)
		{
			this.face  = fontIdentity.InvariantFaceName;
			this.style = fontIdentity.InvariantStyleName;
		}

		/// <summary>
		/// Gets the name of the font face.
		/// </summary>
		/// <value>The name of the font face.</value>
		public string							FaceName
		{
			get
			{
				return this.face;
			}
		}

		/// <summary>
		/// Gets the name of the font style.
		/// </summary>
		/// <value>The name of the font style.</value>
		public string							StyleName
		{
			get
			{
				return this.style;
			}
		}

		/// <summary>
		/// Gets the full name of the font (face and style).
		/// </summary>
		/// <value>The full name of the font (face and style).</value>
		public string							FullName
		{
			get
			{
				if (string.IsNullOrEmpty (this.style))
				{
					return this.face;
				}
				else
				{
					return string.Concat (this.face, " ", this.style);
				}
			}
		}

		/// <summary>
		/// Returns the hash code for this instance.
		/// </summary>
		/// <returns>
		/// A 32-bit signed integer that is the hash code for this instance.
		/// </returns>
		public override int GetHashCode()
		{
			return this.FullName.GetHashCode ();
		}

		/// <summary>
		/// Indicates whether this instance and a specified object are equal.
		/// </summary>
		/// <param name="obj">Another object to compare to.</param>
		/// <returns>
		/// <c>true</c> if obj and this instance are the same type and represent
		/// the same value; otherwise, <c>false</c>.
		/// </returns>
		public override bool Equals(object obj)
		{
			FontName that = (FontName) obj;

			return this.Equals (that);
		}

		/// <summary>
		/// Gets the full font name, based on the concatenation of the font face
		/// name and font style name.
		/// </summary>
		/// <param name="face">The font face name.</param>
		/// <param name="style">The font style name.</param>
		/// <returns>The full font name.</returns>
		public static string GetFullName(string face, string style)
		{
			if (string.IsNullOrEmpty (style))
			{
				return face;
			}
			else
			{
				return string.Concat (face, " ", style);
			}
		}

		/// <summary>
		/// Gets the full hash of the specified full font name. This will sort
		/// all elements in alphabetic order and remove any <c>"Regular"</c>,
		/// <c>"Normal"</c> or <c>"Roman"</c> from the name; duplicates are
		/// also removed.
		/// </summary>
		/// <param name="fullName">The full name.</param>
		/// <returns>The full name hash.</returns>
		public static string GetFullHash(string fullName)
		{
			if (string.IsNullOrEmpty (fullName))
			{
				return null;
			}
			
			List<string> names = new List<string> ();

			string   clean = fullName.Replace ("(", "").Replace (")", "");
			string[] split = clean.Split (' ');

			foreach (string element in split)
			{
				switch (element)
				{
					case "Regular":
						break;
					case "Normal":
						break;
					case "Roman":
						break;

					default:
						if (!names.Contains (element))
						{
							names.Add (element);
						}
						break;
				}
			}

			names.Sort ();

			System.Text.StringBuilder buffer = new System.Text.StringBuilder ();

			foreach (string element in names)
			{
				if (buffer.Length > 0)
				{
					buffer.Append (" ");
				}

				buffer.Append (element);
			}

			return buffer.ToString ();
		}

		#region IComparable Members
		
		public int CompareTo(object obj)
		{
			FontName that = (FontName) obj;
			
			return this.CompareTo (that);
		}
		
		#endregion

		#region IComparable<FontName> Members

		public int CompareTo(FontName other)
		{
			return this.FullName.CompareTo (other.FullName);
		}

		#endregion

		#region IEquatable<FontName> Members

		public bool Equals(FontName other)
		{
			return this.FullName == other.FullName;
		}

		#endregion
		
		private string							face;
		private string							style;
	}
}
