//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.OpenType
{
	/// <summary>
	/// La classe FontName représente le nom d'une fonte (famille et style).
	/// </summary>
	public sealed class FontName : System.IComparable
	{
		public FontName(string face, string style)
		{
			this.face  = face;
			this.style = style;
		}
		
		
		public string							FaceName
		{
			get
			{
				return this.face;
			}
		}
		
		public string							StyleName
		{
			get
			{
				return this.style;
			}
		}
		
		
		public override int GetHashCode()
		{
			return this.face.GetHashCode () ^ this.style.GetHashCode ();
		}
		
		public override bool Equals(object obj)
		{
			FontName that = obj as FontName;
			
			if (that == null) return false;
			if (that == this) return true;
			
			return this.face == that.face
				&& this.style == that.style;
		}

		
		#region IComparable Members
		public int CompareTo(object obj)
		{
			FontName that = obj as FontName;
			
			if (that == null) return 1;
			
			if (this.face == that.face)
			{
				return this.style.CompareTo (that.style);
			}
			else
			{
				return this.face.CompareTo (that.face);
			}
		}
		#endregion
		
		private string							face;
		private string							style;
	}
}
