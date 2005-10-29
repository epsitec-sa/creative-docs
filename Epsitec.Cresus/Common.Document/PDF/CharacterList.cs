using Epsitec.Common.Drawing;
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
			this.unicode      = (int) oneChar.Character;
			this.unicodes     = null;
			this.glyph        = 0;
			this.openTypeFont = oneChar.Font.OpenTypeFont;
			this.bold         = oneChar.Bold;
			this.italic       = oneChar.Italic;
			this.isGlyph      = false;
		}

		public CharacterList(ushort glyph, int unicode, OpenType.Font font)
		{
			this.unicode      = unicode;
			this.unicodes     = null;
			this.glyph        = glyph;
			this.openTypeFont = font;
			this.bold         = false;
			this.italic       = false;
			this.isGlyph      = true;
		}

		public CharacterList(ushort glyph, int[] unicodes, OpenType.Font font)
		{
			this.unicode      = 0;
			this.unicodes     = unicodes;
			this.glyph        = glyph;
			this.openTypeFont = font;
			this.bold         = false;
			this.italic       = false;
			this.isGlyph      = true;
		}

		public void Dispose()
		{
			this.openTypeFont = null;
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

		public int Code
		{
			get
			{
				if ( this.isGlyph )
				{
					return this.glyph;
				}
				else
				{
					return this.unicode;
				}
			}
		}

		public double Width
		{
			get
			{
				if ( this.isGlyph )
				{
					return this.openTypeFont.GetGlyphWidth(this.glyph, 1.0);
				}
				else
				{
					Drawing.Font df = Drawing.Font.GetFont(this.openTypeFont);
					return df.GetCharAdvance(this.unicode);
				}
			}
		}

		public OpenType.Font OpenTypeFont
		{
			get { return this.openTypeFont; }
		}

		public bool Bold
		{
			get { return this.bold; }
		}

		public bool Italic
		{
			get { return this.italic; }
		}

		public bool IsGlyph
		{
			get { return this.isGlyph; }
		}


		public override bool Equals(object obj)
		{
			CharacterList o = obj as CharacterList;

			return ( this.unicode      == o.unicode      &&
					 this.glyph        == o.glyph        &&
					 this.openTypeFont == o.openTypeFont &&
					 this.bold         == o.bold         &&
					 this.italic       == o.italic       &&
					 this.isGlyph      == o.isGlyph      );
		}

		public override int GetHashCode() 
		{
			return this.unicode.GetHashCode() ^ this.openTypeFont.GetHashCode();
		}


		protected int						unicode;
		protected int[]						unicodes;
		protected ushort					glyph;
		protected OpenType.Font				openTypeFont;
		protected bool						bold;
		protected bool						italic;
		protected bool						isGlyph;
	}
}
