//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text.Properties
{
	/// <summary>
	/// Summary description for FontProperty.
	/// </summary>
	public class FontProperty : BaseProperty
	{
		public FontProperty()
		{
		}
		
		
		public override Properties.BaseProperty GetCombination(Properties.BaseProperty property)
		{
			Debug.Assert.IsTrue (property is Properties.FontProperty);
			
			//	TODO: gérer les propriétés cascadées où l'ancêtre modifie l'état
			//	du descendant.
			
			return property;
		}

		
		
		
		public override void UpdateContentsSignature(IO.IChecksum checksum)
		{
			checksum.UpdateValue (this.font_face);
			checksum.UpdateValue (this.font_style);
			checksum.UpdateValue (this.font_optical);
		}
		
		public override bool CompareEqualContents(object value)
		{
			return FontProperty.CompareEqualContents (this, value as FontProperty);
		}
		
		
		private static bool CompareEqualContents(FontProperty a, FontProperty b)
		{
			return a.font_face == b.font_face
				&& a.font_style == b.font_style
				&& a.font_optical == b.font_optical;
		}
		
		
		private string							font_face;
		private string							font_style;
		private string							font_optical;
		
	}
	
	public enum FontStyle : byte
	{
		Normal,
		Italic,
		Oblique,
		
		Other,
	}
	
	public enum FontWeight : byte
	{
		Normal,
		Light,
		Bold,
		
		Other,
	}
}
