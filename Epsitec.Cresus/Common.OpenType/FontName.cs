//	Copyright © 2005-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.OpenType
{
	/// <summary>
	/// The <c>FontName</c> structure stores the face and style name as a
	/// single object.
	/// </summary>
	public struct FontName : System.IComparable<FontName>, System.IEquatable<FontName>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="T:FontName"/> structure.
		/// </summary>
		/// <param name="face">The font face.</param>
		/// <param name="style">The font style.</param>
		public FontName(string face, string style)
		{
			this.face  = face;
			this.style = style;
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
		public string FullName
		{
			get
			{
				if (this.style == "")
				{
					return this.face;
				}
				else
				{
					return string.Format("{0} {1}", this.face, this.style);
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
			return this.face.GetHashCode () ^ this.style.GetHashCode ();
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
			if (this.face == other.face)
			{
				return this.style.CompareTo (other.style);
			}
			else
			{
				return this.face.CompareTo (other.face);
			}
		}

		#endregion

		#region IEquatable<FontName> Members

		public bool Equals(FontName other)
		{
			return this.face == other.face
				&& this.style == other.style;
		}

		#endregion
		
		private string							face;
		private string							style;
	}
}
