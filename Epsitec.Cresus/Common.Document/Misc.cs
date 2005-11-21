using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Common.Text;

namespace Epsitec.Common.Document
{
	/// <summary>
	/// La classe Misc contient quelques routines générales.
	/// </summary>
	public class Misc
	{
		// Conversion d'une chaîne en nombre réel.
		static public void ConvertStringToDouble(out double value, out Text.Properties.SizeUnits units, string text, double min, double max, double defaultValue)
		{
			if ( text == "" )
			{
				value = 0;
				units = Text.Properties.SizeUnits.None;
			}
			else if ( text.EndsWith("%") )
			{
				text = text.Substring(0, text.Length-1);
				value = Misc.ConvertStringToDouble(text, min, max, defaultValue);
				units = Text.Properties.SizeUnits.Percent;
			}
			else
			{
				value = Misc.ConvertStringToDouble(text, min, max, defaultValue);
				units = Text.Properties.SizeUnits.Points;
			}
		}

		// Conversion d'une chaîne en nombre réel.
		static public double ConvertStringToDouble(string text, double min, double max, double defaultValue)
		{
			double value = defaultValue;
				
			if ( text != "" )
			{
				string  dec = Types.Converter.ExtractDecimal(ref text);
					
				try
				{
					value = double.Parse(dec, System.Globalization.CultureInfo.CurrentUICulture);
				}
				catch
				{
					return defaultValue;
				}
			}

			value = System.Math.Max(value, min);
			value = System.Math.Min(value, max);
			return value;
		}

		// Conversion d'un nombre réel en chaîne.
		static public string ConvertDoubleToString(double value, Text.Properties.SizeUnits units, double fracDigits)
		{
			if ( units == Text.Properties.SizeUnits.Percent )
			{
				return Misc.ConvertDoubleToString(value, fracDigits) + "%";
			}

			if ( units == Text.Properties.SizeUnits.Points )
			{
				return Misc.ConvertDoubleToString(value, fracDigits);
			}

			return "";
		}

		// Conversion d'un nombre réel en chaîne.
		static public string ConvertDoubleToString(double value, double fracDigits)
		{
			if ( fracDigits > 0 )
			{
				return value.ToString(string.Format("F{0}", fracDigits));
			}
			else
			{
				return ((int)value).ToString(System.Globalization.CultureInfo.InvariantCulture);
			}
		}


		// Ajoute la liste des fontes dans la liste d'un TextFieldCombo.
		static public void AddFontList(TextFieldCombo combo, bool enableSymbols)
		{
#if false
			foreach( string face in TextContext.GetAvailableFontFaces() )
			{
				bool add = true;
				if ( !enableSymbols )
				{
					OpenType.FontIdentity[] ids = TextContext.GetAvailableFontIdentities(face);
					foreach ( OpenType.FontIdentity id in ids )
					{
						if ( id.IsSymbolFont )  add = false;
					}
				}

				if ( add )
				{
					combo.Items.Add(face);
				}
			}
#else
			if ( Misc.fontList == null )
			{
				Misc.fontList = new System.Collections.ArrayList();

				foreach( string face in TextContext.GetAvailableFontFaces() )
				{
					bool symbol = false;
					OpenType.FontIdentity[] ids = TextContext.GetAvailableFontIdentities(face);
					foreach ( OpenType.FontIdentity id in ids )
					{
						if ( id.IsSymbolFont )
						{
							symbol = true;
							break;
						}
					}

					Misc.fontList.Add((symbol ? "S" : "N") + face);
				}
			}

			foreach ( string s in Misc.fontList )
			{
				if ( !enableSymbols && s[0] == 'S' )  continue;
				combo.Items.Add(s.Substring(1));
			}
#endif
		}

		// Cherche le FontStyle par défaut pour un FontFace donné.
		static public string DefaultFontStyle(string face)
		{
			OpenType.FontIdentity[] list = TextContext.GetAvailableFontIdentities(face);

			foreach ( OpenType.FontIdentity id in list )
			{
				if ( id.FontWeight == OpenType.FontWeight.Normal &&
					 id.FontStyle  == OpenType.FontStyle.Normal  )
				{
					return id.InvariantStyleName;
				}
			}

			foreach ( OpenType.FontIdentity id in list )
			{
				if ( id.FontWeight == OpenType.FontWeight.Normal )
				{
					return id.InvariantStyleName;
				}
			}

			foreach ( OpenType.FontIdentity id in list )
			{
				if ( id.FontStyle == OpenType.FontStyle.Normal )
				{
					return id.InvariantStyleName;
				}
			}

			foreach ( OpenType.FontIdentity id in list )
			{
				return id.InvariantStyleName;
			}

			return "";
		}

		// Simplifie un style ("Regular !Bold" devient "Bold" par exemple).
		static public string SimplifyFontStyle(string style)
		{
			string simple = OpenType.FontCollection.GetStyleHash(style);
			if ( simple != "" )  return simple;
			return style;
		}

		// Donne une fonte d'après son nom.
		static public Font GetFont(string fontName)
		{
			Font font = Font.GetFont(fontName, "Regular");

			if ( font == null )
			{
				font = Font.GetFontFallback(fontName);
			}
			
			return font;
		}

		// Retourne le nom d'un caractère Unicode ou du glyph d'une fonte de symboles.
		static public string GetUnicodeName(int code, string fontFace, string fontStyle)
		{
			Common.OpenType.Font font = TextContext.GetFont(fontFace, fontStyle);
			if ( font != null && font.FontIdentity.IsSymbolFont )
			{
				int glyph = font.GetGlyphIndex(code);
				return font.FontIdentity.GetGlyphName(glyph);
			}

			return Misc.GetUnicodeName(code);
		}

		// Retourne le nom d'un caractère Unicode.
		static public string GetUnicodeName(int code)
		{
			if ( code == 0 )  return "";

			string text = TextBreak.GetUnicodeName(code);

			bool minus = false;
			for( int i=0 ; i<text.Length ; i++ )
			{
				if ( text[i] >= 'a' && text[i] <= 'z' )
				{
					minus = true;  // contient une lettre minuscule
					break;
				}
			}

			if ( !minus )  // aucune minuscule dans le texte ?
			{
				// Première lettre en majuscule, le reste en minuscules.
				text = string.Format("{0}{1}", text.Substring(0, 1).ToUpper(), text.Substring(1, text.Length-1).ToLower());
			}

			return text;
		}


		// Donne la liste des "features" communs à toutes les fontes.
		static public string[] DefaultFeatures()
		{
			string[] list = new string[2];
			int i=0;
			list[i++] = "liga";
			list[i++] = "dlig";
			return list;
		}

		// Donne le texte descriptif pour une "feature" OpenType.
		static public string GetFeatureText(string feature)
		{
			string text = Res.Strings.GetString("Text", string.Format("Features_{0}", feature));
			if ( text != null )  return text;

			return string.Format(Res.Strings.Text.FeaturesUnknow, feature.ToUpper());
		}

		// Retourne true si text est dans list.
		static public bool IsInsideList(string[] list, string text)
		{
			if ( list == null )  return false;

			for ( int i=0 ; i<list.Length ; i++ )
			{
				if ( list[i] == text )  return true;
			}
			return false;
		}


		// Retourne le texte en gras.
		static public string Bold(string text)
		{
			return string.Format("<b>{0}</b>", text);
		}

		// Retourne le texte en italique.
		static public string Italic(string text)
		{
			return string.Format("<i>{0}</i>", text);
		}

		// Retourne le texte pour mettre une image dans un texte.
		static public string Image(string icon)
		{
			return string.Format(@"<img src=""{0}""/>", Misc.Icon(icon));
		}

		// Retourne le nom complet d'une icône.
		static public string Icon(string icon)
		{
			return string.Format("manifest:Epsitec.App.DocumentEditor.Images.{0}.icon", icon);
		}

		// Retourne le nom complet d'une icône selon la langue.
		static public string IconL(string icon)
		{
			string language = System.Globalization.CultureInfo.CurrentCulture.TwoLetterISOLanguageName;
			if ( language != "fr" && language != "en" && language != "de" )
			{
				language = "en";
			}
			return string.Format("manifest:Epsitec.App.DocumentEditor.Images.{0}-{1}.icon", icon, language);
		}

		// Donne le nom complet du fichier.
		// Si le nom n'existe pas, donne "sans titre".
		// Si le fichier doit être sérialisé, donne le nom en gras.
		static public string FullName(string filename, bool dirtySerialize)
		{
			string name = "";
			if ( dirtySerialize )  name += "<b>";

			if ( filename == "" )
			{
				name += Res.Strings.Misc.NoTitle;
			}
			else
			{
				name += filename;
			}

			if ( dirtySerialize )  name += "</b>";
			return name;
		}

		// Extrait le nom de fichier, en ignorant les noms de dossiers et l'extension.
		// Si le nom n'existe pas, donne "sans titre".
		// Si le fichier doit être sérialisé, donne le nom en gras.
		static public string ExtractName(string filename, bool dirtySerialize)
		{
			string name = "";
			if ( dirtySerialize )  name += "<b>";

			if ( filename == "" )
			{
				name += Res.Strings.Misc.NoTitle;
			}
			else
			{
				name += ExtractName(filename);
			}

			if ( dirtySerialize )  name += "</b>";
			return name;
		}

		// Extrait le nom de fichier, en ignorant les noms de dossiers et l'extension.
		// "c:\rep\abc.txt" devient "abc".
		static public string ExtractName(string filename)
		{
			return System.IO.Path.GetFileNameWithoutExtension(filename);
		}

		// Indique si un fichier utilise une extension donnée.
		static public bool IsExtension(string filename, string ext)
		{
			return filename.ToLower().EndsWith(ext);
		}

		// Retourne la copie d'un nom.
		// "Bidon"              ->  "Copie de Bidon"
		// "Copie de Bidon"     ->  "Copie (2) de Bidon"
		// "Copie (2) de Bidon" ->  "Copie (3) de Bidon"
		static public string CopyName(string name)
		{
			return Misc.CopyName(name, Res.Strings.Misc.Copy, Res.Strings.Misc.CopyOf);
		}

		// Retourne la copie d'un nom.
		// copy = "Copie" ou "Copy"
		// of = "de" ou "of"
		static public string CopyName(string name, string copy, string of)
		{
			if ( name == "" )
			{
				return copy;
			}

			if ( name.StartsWith(string.Concat(copy, " ", of, " ")) )
			{
				return string.Concat(copy, " (2) ", of, " ", name.Substring(copy.Length+of.Length+2));
			}

			if ( name.StartsWith(string.Concat(copy, " (")) )
			{
				int num = 0;
				int i = copy.Length+2;
				while ( name[i] >= '0' && name[i] <= '9' )
				{
					num *= 10;
					num += name[i++]-'0';
				}
				num ++;
				return string.Concat(copy, " (", num.ToString(), name.Substring(i));
			}

			return string.Concat(copy, " ", of, " ", name);
		}

		// Permute deux variables.
		static public void Swap(ref bool a, ref bool b)
		{
			bool t = a;
			a = b;
			b = t;
		}

		static public void Swap(ref int a, ref int b)
		{
			int t = a;
			a = b;
			b = t;
		}

		static public void Swap(ref double a, ref double b)
		{
			double t = a;
			a = b;
			b = t;
		}

		static public void Swap(ref Point a, ref Point b)
		{
			Point t = a;
			a = b;
			b = t;
		}

		static public void Swap(ref Size a, ref Size b)
		{
			Size t = a;
			a = b;
			b = t;
		}


		protected static System.Collections.ArrayList		fontList;
	}
}
