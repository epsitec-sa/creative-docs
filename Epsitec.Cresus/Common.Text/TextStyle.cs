//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text
{
	/// <summary>
	/// La classe TextStyle définit un style de texte de haut niveau. Il s'agit
	/// d'une collection de propriétés.
	/// </summary>
	public class TextStyle : Styles.BasePropertyContainer, IContentsComparer
	{
		public TextStyle()
		{
		}
		
		public TextStyle(System.Collections.ICollection properties) : base (properties)
		{
		}
		
		
		public override void UpdateContentsSignature(Epsitec.Common.IO.IChecksum checksum)
		{
			base.UpdateContentsSignature (checksum);
		}
		
		
		#region IContentsComparer Members
		public bool CompareEqualContents(object value)
		{
			return TextStyle.CompareEqualContents (this, value as TextStyle);
		}
		#endregion
		
		public static bool CompareEqualContents(TextStyle a, TextStyle b)
		{
			//	TODO: compléter
			
			return true;
		}
	}
}
