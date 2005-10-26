//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text.Properties
{
	/// <summary>
	/// La classe OpenTypeProperty donne accès aux glyphes supplémentaires d'une
	/// fonte OpenType (variantes, etc.)
	/// </summary>
	public class OpenTypeProperty : Property
	{
		public OpenTypeProperty()
		{
		}
		
		public OpenTypeProperty(string font_face, string font_style, int glyph_index)
		{
			this.font_face   = font_face;
			this.font_style  = font_style;
			this.glyph_index = glyph_index;
		}
		
		
		public override WellKnownType			WellKnownType
		{
			get
			{
				return WellKnownType.OpenType;
			}
		}
		
		public override PropertyType			PropertyType
		{
			get
			{
				return PropertyType.LocalSetting;
			}
		}
		
		public override PropertyAffinity		PropertyAffinity
		{
			get
			{
				return PropertyAffinity.Symbol;
			}
		}

		public override CombinationMode			CombinationMode
		{
			get
			{
				return CombinationMode.Invalid;
			}
		}
		
		public override bool					RequiresSpecialCodeProcessing
		{
			get
			{
				return true;
			}
		}
		
		
		public string							FontFace
		{
			get
			{
				return this.font_face;
			}
		}
		
		public string							FontStyle
		{
			get
			{
				return this.font_style;
			}
		}
		
		public int								GlyphIndex
		{
			get
			{
				return this.glyph_index;
			}
		}
		
		
		public override int GetGlyphForSpecialCode(ulong code)
		{
			return this.glyph_index;
		}
		
		
		public override Property EmptyClone()
		{
			return new OpenTypeProperty ();
		}
		
		public override void SerializeToText(System.Text.StringBuilder buffer)
		{
			SerializerSupport.Join (buffer,
				/**/				SerializerSupport.SerializeString (this.font_face),
				/**/				SerializerSupport.SerializeString (this.font_style),
				/**/				SerializerSupport.SerializeInt (this.glyph_index));
		}

		public override void DeserializeFromText(TextContext context, string text, int pos, int length)
		{
			string[] args = SerializerSupport.Split (text, pos, length);
			
			Debug.Assert.IsTrue (args.Length == 3);
			
			string font_face   = SerializerSupport.DeserializeString (args[0]);
			string font_style  = SerializerSupport.DeserializeString (args[1]);
			int    glyph_index = SerializerSupport.DeserializeInt (args[2]);
			
			this.font_face   = font_face;
			this.font_style  = font_style;
			this.glyph_index = glyph_index;
		}
		
		public override Property GetCombination(Property property)
		{
			throw new System.InvalidOperationException ("Cannot combine OpenType properties.");
		}

		
		public override void UpdateContentsSignature(IO.IChecksum checksum)
		{
			checksum.UpdateValue (this.font_face);
			checksum.UpdateValue (this.font_style);
			checksum.UpdateValue (this.glyph_index);
		}
		
		public override bool CompareEqualContents(object value)
		{
			return OpenTypeProperty.CompareEqualContents (this, value as OpenTypeProperty);
		}
		
		
		private static bool CompareEqualContents(OpenTypeProperty a, OpenTypeProperty b)
		{
			return a.font_face == b.font_face
				&& a.font_style == b.font_style
				&& a.glyph_index == b.glyph_index;
		}
		
		
		
		private string							font_face;
		private string							font_style;
		private int								glyph_index;
	}
}
