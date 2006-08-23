using Epsitec.Common.Widgets;
using Epsitec.Common.OpenType;
using Epsitec.Common.Text;

namespace Epsitec.Common.Document.PDF
{
	/// <summary>
	/// La classe CharacterList enregistre les informations sur chaque caractère.
	/// </summary>
	public class CharacterList
	{
		public CharacterList(TextLayout.OneCharStructure oneChar)
		{
			this.unicode      = oneChar.Character;
			this.unicodes     = null;
			this.openTypeFont = oneChar.Font.OpenTypeFont;
			this.glyph        = this.openTypeFont.GetGlyphIndex (this.unicode);
		}

		public CharacterList(ushort glyph, int unicode, OpenType.Font font)
		{
			this.unicode      = unicode;
			this.unicodes     = null;
			this.glyph        = glyph;
			this.openTypeFont = font;
		}

		public CharacterList(ushort glyph, int[] unicodes, OpenType.Font font)
		{
			this.unicode      = 0;
			this.unicodes     = unicodes;
			this.glyph        = glyph;
			this.openTypeFont = font;
		}

		public int Unicode
		{
			get { return this.unicode; }
		}

		public int[] Unicodes
		{
			get { return this.unicodes; }
		}

		public ushort Glyph
		{
			get { return this.glyph; }
		}

		public double Width
		{
			get
			{
				return this.openTypeFont.GetGlyphWidth (this.glyph, 1.0);
			}
		}

		public OpenType.Font OpenTypeFont
		{
			get { return this.openTypeFont; }
		}

		public override bool Equals(object obj)
		{
			CharacterList o = obj as CharacterList;

			return ( this.unicode      == o.unicode      &&
					 this.glyph        == o.glyph        &&
					 this.openTypeFont == o.openTypeFont );
		}

		public override int GetHashCode() 
		{
			return this.unicode ^ this.openTypeFont.GetHashCode();
		}


		protected int						unicode;
		protected int[]						unicodes;
		protected ushort					glyph;
		protected OpenType.Font				openTypeFont;
	}
}
