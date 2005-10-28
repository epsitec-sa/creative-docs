using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;

namespace Epsitec.Common.Document.PDF
{
	/// <summary>
	/// La classe CharacterList enregistre les informations sur chaque caractère.
	/// </summary>
	public class CharacterList
	{
		public CharacterList(TextLayout.OneCharStructure oneChar)
		{
			this.unicode = (int) oneChar.Character;
			this.font    = oneChar.Font;
			this.bold    = oneChar.Bold;
			this.italic  = oneChar.Italic;
			this.isGlyph = false;
		}

		public CharacterList(ushort glyph, Drawing.Font font)
		{
			this.unicode = (int) glyph;
			this.font    = font;
			this.bold    = false;
			this.italic  = false;
			this.isGlyph = true;
		}

		public void Dispose()
		{
			this.font = null;
		}

		public int Unicode
		{
			get { return this.unicode; }
		}

		public Drawing.Font Font
		{
			get { return this.font; }
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

			return ( this.unicode == o.unicode &&
					 this.font    == o.font    &&
					 this.bold    == o.bold    &&
					 this.italic  == o.italic  );
		}

		public override int GetHashCode() 
		{
			return this.unicode.GetHashCode() ^ this.font.GetHashCode();
		}


		protected int						unicode;
		protected Drawing.Font				font;
		protected bool						bold;
		protected bool						italic;
		protected bool						isGlyph;
	}
}
