using Epsitec.Common.Drawing;

namespace Epsitec.Common.Document.PDF
{
	/// <summary>
	/// La classe FontList enregistre les informations sur chaque police.
	/// </summary>
	public class FontList
	{
		public FontList(Drawing.Font font, int id)
		{
			this.font = font;
			this.id   = id;
		}

		public void Dispose()
		{
			this.font = null;
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
					bbox.MergeWith(this.font.GetCharBounds(cl.Unicode));
				}
				return bbox;
			}
		}

		// Retourne l'index d'un caractère dans la fonte.
		public int GetUnicodeIndex(int unicode)
		{
			for ( int i=0 ; i<this.characters.Count ; i++ )
			{
				CharacterList cl = this.characters[i] as CharacterList;
				if ( cl.Unicode == unicode )  return i;
			}
			return -1;
		}


		public Drawing.Font Font
		{
			get { return this.font; }
		}

		public int Id
		{
			get { return this.id; }
		}


		public override bool Equals(object obj)
		{
			FontList o = obj as FontList;

			return ( this.font == o.font );
		}

		public override int GetHashCode() 
		{
			return this.font.GetHashCode();
		}


		// Crée les fontes d'après les caractères.
		public static void CreateFonts(System.Collections.Hashtable fonts, System.Collections.Hashtable characters)
		{
			int id = 0;
			foreach ( System.Collections.DictionaryEntry dict in characters )
			{
				CharacterList cl = dict.Key as CharacterList;
				FontList nfl = new FontList(cl.Font, id);
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
					string f = cl.Font.FaceName;
					string b = cl.Bold ? "+Bold" : "";
					string j = cl.Italic ? "+Italic" : "";
					string c = cl.Unicode.ToString();
					System.Console.WriteLine(string.Format("  >  font={0}{1}{2} char={3}", f, b,j, c));
				}
			}
#endif
		}

		public static FontList Search(System.Collections.Hashtable fonts, Font drawingFont)
		{
			foreach ( System.Collections.DictionaryEntry dict in fonts )
			{
				FontList fl = dict.Key as FontList;
				if ( fl.font == drawingFont )  return fl;
			}
			return null;
		}


		protected Drawing.Font					font;
		protected int							id;
		protected System.Collections.ArrayList	characters;
	}
}
