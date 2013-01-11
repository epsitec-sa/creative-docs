using Epsitec.Common.Drawing;
using Epsitec.Common.OpenType;

namespace Epsitec.Common.Pdf.Engine
{
	/// <summary>
	/// La classe FontList enregistre les informations sur chaque police.
	/// </summary>
	public class FontList
	{
		public FontList(OpenType.Font font, int id)
		{
			this.openTypeFont = font;
			this.drawingFont = Drawing.Font.GetFont(font);
			this.id = id;
		}

		public void AddCharacter(CharacterList cl)
		{
			if ( this.characters == null )
			{
				this.characters = new System.Collections.ArrayList();
			}

			this.characters.Add(cl);
		}

		public int CharacterCount
		{
			get
			{
				if ( this.characters == null )  return 0;
				return this.characters.Count;
			}
		}

		public CharacterList GetCharacter(int index)
		{
			if ( this.characters == null )  return null;
			return this.characters[index] as CharacterList;
		}

		public Rectangle CharacterBBox
		{
			get
			{
				Rectangle bbox = Rectangle.Empty;
				foreach ( CharacterList cl in this.characters )
				{
					if ( cl.Unicodes == null )
					{
						bbox.MergeWith(this.drawingFont.GetCharBounds(cl.Unicode));
					}
					else
					{
						for ( int i=0 ; i<cl.Unicodes.Length ; i++ )
						{
							bbox.MergeWith(this.drawingFont.GetCharBounds(cl.Unicodes[i]));
						}
					}
				}
				return bbox;
			}
		}

		public int GetUnicodeIndex(int unicode)
		{
			//	Retourne l'index d'un caractère dans la fonte.
			for ( int i=0 ; i<this.characters.Count ; i++ )
			{
				CharacterList cl = this.characters[i] as CharacterList;
				if ( cl.Unicode == unicode )  return i;
			}
			return -1;
		}

		public int GetGlyphIndex(ushort glyph)
		{
			//	Retourne l'index d'un caractère dans la fonte.
			for ( int i=0 ; i<this.characters.Count ; i++ )
			{
				CharacterList cl = this.characters[i] as CharacterList;
				if ( cl.Glyph == glyph )  return i;
			}
			return -1;
		}


		public Drawing.Font DrawingFont
		{
			get { return this.drawingFont; }
		}

		public OpenType.Font OpenTypeFont
		{
			get { return this.openTypeFont; }
		}

		public int Id
		{
			get { return this.id; }
		}


		public override bool Equals(object obj)
		{
			//	Compare si deux fontes ont des apparences identiques, même si les objets
			//	ne sont pas physiquement les mêmes.
			FontList o = obj as FontList;

			return OpenType.Font.HaveEqualTypography(this.openTypeFont, o.openTypeFont);
		}

		public override int GetHashCode() 
		{
			return this.openTypeFont.GetTypographyHashCode();
		}


		public static void CreateFonts(FontHash fonts, CharacterHash characters)
		{
			//	Crée les fontes d'après les caractères.
			int id = 0;
			foreach ( System.Collections.DictionaryEntry dict in characters )
			{
				CharacterList cl = dict.Key as CharacterList;
				FontList nfl = new FontList(cl.OpenTypeFont, id);
				FontList cfl = fonts[nfl] as FontList;

				if ( cfl == null )
				{
					fonts.Add(nfl, nfl);
					cfl = nfl;
					id ++;
				}

				cfl.AddCharacter(cl);
			}

#if false
			System.Console.WriteLine("Fonts and characters founds:");
			foreach ( System.Collections.DictionaryEntry dict in fonts )
			{
				FontList fl = dict.Key as FontList;

				for ( int i=0 ; i<fl.CharacterCount ; i++ )
				{
					CharacterList cl = fl.GetCharacter(i);
					string n = cl.OpenTypeFont.FontIdentity.FullName;
					string c = cl.Unicode.ToString();
					System.Console.WriteLine(string.Format(System.Globalization.CultureInfo.InvariantCulture, "  >  font={0} char={1}", n, c));
				}
			}
#endif
		}

		public static FontList Search(FontHash fonts, Drawing.Font drawingFont)
		{
			//	Cherche une fonte ayant une apparence identique.
			foreach ( System.Collections.DictionaryEntry dict in fonts )
			{
				FontList fl = dict.Key as FontList;
				if (OpenType.Font.HaveEqualTypography(fl.OpenTypeFont, drawingFont.OpenTypeFont))
				{
					return fl;
				}
			}
			return null;
		}


		protected OpenType.Font					openTypeFont;
		protected Drawing.Font					drawingFont;
		protected int							id;
		protected System.Collections.ArrayList	characters;
	}
}
