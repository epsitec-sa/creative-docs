//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text.Properties
{
	/// <summary>
	/// La classe FontBoldProperty indique qu'il faut inverser la graisse de la
	/// fonte (regular -> bold, bold -> regular).
	/// </summary>
	public class FontBoldProperty : Property
	{
		public FontBoldProperty()
		{
		}
		
		
		public override WellKnownType			WellKnownType
		{
			get
			{
				return WellKnownType.FontBold;
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
			return new FontBoldProperty ();
		}
		
		public override void SerializeToText(System.Text.StringBuilder buffer)
		{
		}
		
		public override void DeserializeFromText(TextContext context, string text, int pos, int length)
		{
		}
		
		public override Property GetCombination(Property property)
		{
			throw new System.InvalidOperationException ("Cannot combine FontBold properties.");
		}
		
		
		public override void UpdateContentsSignature(IO.IChecksum checksum)
		{
		}
		
		public override bool CompareEqualContents(object value)
		{
			return FontBoldProperty.CompareEqualContents (this, value as FontBoldProperty);
		}
		
		
		private static bool CompareEqualContents(FontBoldProperty a, FontBoldProperty b)
		{
			return true;
		}
	}
}
