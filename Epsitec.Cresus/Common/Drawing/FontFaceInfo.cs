//	Copyright © 2003-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Drawing
{
	public sealed class FontFaceInfo
	{
		internal FontFaceInfo(string name)
		{
			this.name  = name;
			this.fonts = new List<Font> ();
		}
		
		internal void Add(Font font)
		{
			System.Diagnostics.Debug.Assert (font.FaceName == this.name);

			if (this.fonts.Contains (font))
			{
				System.Diagnostics.Debug.Assert (font.FaceInfo == this);
			}
			else
			{
				System.Diagnostics.Debug.Assert (font.FaceInfo == null);

				foreach (Font item in this.fonts)
				{
					if (item.StyleName == font.StyleName)
					{
						System.Diagnostics.Debug.WriteLine (string.Format ("Duplicate font, FaceName={0}, StyleName={1}", font.FaceName, font.StyleName), "Epsitec.Common.Drawing.FontFaceInfo");
						break;
					}
				}

				this.fonts.Add (font);
				font.DefineFaceInfo (this);
			}
		}


		public string							Name
		{
			get
			{
				return this.name;
			}
		}

		public string[]							StyleNames
		{
			get
			{
				List<string> styles = new List<string> ();

				foreach (Font font in this.fonts)
				{
					string name = font.StyleName;

					if (string.IsNullOrEmpty (name))
					{
						name = "Regular";
					}

					if (styles.Contains (name) == false)
					{
						styles.Add (name);
					}
				}

				styles.Sort ();
				
				return styles.ToArray ();
			}
		}

		public int								Count
		{
			get
			{
				return this.fonts.Count;
			}
		}

		public bool								IsLatin
		{
			get
			{
				if (this.fonts.Count > 0)
				{
					Font font = this.fonts[0];
					return font.OpenTypeFont.FontIdentity.IsSymbolFont ? false : true;
				}
				
				return false;
			}
		}


		public Font GetFont(bool bold, bool italic)
		{
#if false
			bool regular = !bold && !italic;
			
			foreach (Font font in this.fonts)
			{
				bool isBoldFont   = font.IsStyleBold;
				bool isItalicFont = font.IsStyleItalic;
				
				if ((bold == isBoldFont) &&
					(italic == isItalicFont))
				{
					if ((!regular) ||
						(font.IsStyleRegular))
					{
						return font;
					}
				}
			}
#else
			//	[DR]
			//	J'ai ajouté ceci pour trouver correctement la bonne police "Segoe UI" parmi
			//	les nombreuses variantes et éviter le bug des polices italiques qui
			//	s'affichaient en bold-italique (en fait, l'ancien code trouvait du "Black Italic").
			//	Note: Ces quelques lignes auraient pu s'écrire plus élégamment avec link, mais
			//	il ne semble pas disponible ici !

			foreach (var font in this.fonts)
			{
				if (font.MatchForStyle (bold, italic))
				{
					return font;
				}
			}
#endif
			
			Font syntheticFont = this.GetSyntheticFont (bold, italic);

			if (syntheticFont != null)
			{
				this.Add (syntheticFont);
			}
			
			return syntheticFont;
		}

		public Font[] GetFonts()
		{
			return this.fonts.ToArray ();
		}


		private Font GetSyntheticFont(bool bold, bool italic)
		{
			string style = null;

			if (bold)
			{
				style = italic ? "Bold Italic" : "Bold";
			}
			else if (italic)
			{
				style = "Italic";
			}
			else
			{
				return null;
			}

			return Font.GetFont (this.name, style);
		}

		private string name;
		private List<Font> fonts;
	}
}
