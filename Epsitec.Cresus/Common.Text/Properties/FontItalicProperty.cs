//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text.Properties
{
	/// <summary>
	/// La classe FontItalicProperty indique qu'il faut inverser l'italique de
	/// la fonte (regular -> italic, italic -> regular).
	/// </summary>
	public class FontItalicProperty : Property
	{
		public FontItalicProperty()
		{
		}
		
		
		public override WellKnownType			WellKnownType
		{
			get
			{
				return WellKnownType.FontItalic;
			}
		}
		
		public override PropertyType			PropertyType
		{
			get
			{
				return PropertyType.Style;
			}
		}
		
		public override CombinationMode			CombinationMode
		{
			get
			{
				return CombinationMode.Invalid;
			}
		}
		
		
		public override Property EmptyClone()
		{
			return new FontItalicProperty ();
		}
		
		public override void SerializeToText(System.Text.StringBuilder buffer)
		{
		}
		
		public override void DeserializeFromText(TextContext context, string text, int pos, int length)
		{
		}
		
		public override Property GetCombination(Property property)
		{
			throw new System.InvalidOperationException ("Cannot combine FontItalic properties.");
		}
		
		
		public override void UpdateContentsSignature(IO.IChecksum checksum)
		{
		}
		
		public override bool CompareEqualContents(object value)
		{
			return FontItalicProperty.CompareEqualContents (this, value as FontItalicProperty);
		}
		
		
		private static bool CompareEqualContents(FontItalicProperty a, FontItalicProperty b)
		{
			return true;
		}
	}
}
