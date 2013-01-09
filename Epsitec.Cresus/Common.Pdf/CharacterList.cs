using Epsitec.Common.Widgets;
using Epsitec.Common.OpenType;
using Epsitec.Common.Text;

namespace Epsitec.Common.Pdf
{
	/// <summary>
	/// La classe CharacterList enregistre les informations sur chaque caractère.
	/// </summary>
	public class CharacterList
	{
		//	TODO: résoudre le cas où Drawing.Font est synthétique (IsSynthetic = true) -- pour réaliser des
		//	obliques par exemple ("Tahoma Oblique" n'existe pas et est construit artificiellement).
		
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

		public Drawing.Rectangle Bounds
		{
			get
			{
				double xMin, xMax, yMin, yMax;
				this.openTypeFont.GetGlyphBounds(this.glyph, 1.0, out xMin, out xMax, out yMin, out yMax);
				return new Drawing.Rectangle(xMin, yMin, xMax-xMin, yMax-yMin);
			}
		}

		public OpenType.Font OpenTypeFont
		{
			get { return this.openTypeFont; }
		}

		public override bool Equals(object obj)
		{
			//	Compare deux caractères. Ils doivent utiliser des fontes ayant une apparence identique
			//	pour être considérés comme identiques.
			CharacterList o = obj as CharacterList;

			return ( this.unicode == o.unicode &&
					 this.glyph   == o.glyph   &&
					 OpenType.Font.HaveEqualTypography(this.openTypeFont, o.openTypeFont) );
		}

		public override int GetHashCode() 
		{
			return this.unicode ^ this.openTypeFont.GetTypographyHashCode();
		}


		protected int						unicode;
		protected int[]						unicodes;
		protected ushort					glyph;
		protected OpenType.Font				openTypeFont;
	}
}
