//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text.Properties
{
	/// <summary>
	/// La classe OpenTypeProperty donne accès aux glyphes supplémentaires d'une
	/// fonte OpenType (variantes, etc.)
	/// </summary>
	public class OpenTypeProperty : BaseProperty
	{
		public OpenTypeProperty()
		{
		}
		
		public OpenTypeProperty(string font_name, int glyph_index)
		{
			this.font_name   = font_name;
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
		
		
		public string							FontName
		{
			get
			{
				return this.font_name;
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
		
		
		public override void SerializeToText(System.Text.StringBuilder buffer)
		{
			SerializerSupport.Join (buffer,
				/**/				SerializerSupport.SerializeString (this.font_name),
				/**/				SerializerSupport.SerializeInt (this.glyph_index));
		}

		public override void DeserializeFromText(Context context, string text, int pos, int length)
		{
			string[] args = SerializerSupport.Split (text, pos, length);
			
			Debug.Assert.IsTrue (args.Length == 2);
			
			string font_name   = SerializerSupport.DeserializeString (args[0]);
			int    glyph_index = SerializerSupport.DeserializeInt (args[1]);
			
			this.font_name   = font_name;
			this.glyph_index = glyph_index;
		}
		
		public override Properties.BaseProperty GetCombination(Properties.BaseProperty property)
		{
			throw new System.InvalidOperationException ("Cannot combine OpenType properties.");
		}

		
		public override void UpdateContentsSignature(IO.IChecksum checksum)
		{
			checksum.UpdateValue (this.font_name);
			checksum.UpdateValue (this.glyph_index);
		}
		
		public override bool CompareEqualContents(object value)
		{
			return OpenTypeProperty.CompareEqualContents (this, value as OpenTypeProperty);
		}
		
		
		private static bool CompareEqualContents(OpenTypeProperty a, OpenTypeProperty b)
		{
			return a.font_name == b.font_name
				&& a.glyph_index == b.glyph_index;
		}
		
		
		
		private string							font_name;
		private int								glyph_index;
	}
}
