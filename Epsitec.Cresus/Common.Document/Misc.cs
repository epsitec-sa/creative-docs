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
		static public void ConvertStringToDouble(out double value, out Text.Properties.SizeUnits units, string text, double min, double max, double defaultValue)
		{
			//	Conversion d'une chaîne en nombre réel.
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

		static public double ConvertStringToDouble(string text, double min, double max, double defaultValue)
		{
			//	Conversion d'une chaîne en nombre réel.
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

		static public string ConvertDoubleToString(double value, Text.Properties.SizeUnits units, double fracDigits)
		{
			//	Conversion d'un nombre réel en chaîne.
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

		static public string ConvertDoubleToString(double value, double fracDigits)
		{
			//	Conversion d'un nombre réel en chaîne.
			if ( fracDigits > 0 )
			{
				return value.ToString(string.Format("F{0}", fracDigits));
			}
			else
			{
				return ((int)value).ToString(System.Globalization.CultureInfo.InvariantCulture);
			}
		}


		static public System.Collections.ArrayList MergeFontList(System.Collections.ArrayList inList, System.Collections.ArrayList quickFaceNames, bool quickOnly, string selectedFaceName, out int quickCount)
		{
			//	Crée une liste qui contient les fontes rapides au début.
			//	En mode quickOnly (liste courte), la police selectedFaceName apparaît même si elle
			//	ne fait pas partie des polices rapides.
			//	inList: tous les OpenType.FontIdentity connus
			//	quickFaceNames: strings des FaceNames fréquement utilisés, dans n'importe quel ordre
			//	outList (liste retournée): OpenType.FontIdentity
			System.Collections.ArrayList outList = new System.Collections.ArrayList();
			System.Collections.ArrayList begin   = new System.Collections.ArrayList();

			if ( quickFaceNames.Count == 0 )  quickOnly = false;

			//	Copie la liste en enlevant toutes les fontes rapides.
			foreach ( Common.OpenType.FontIdentity id in inList )
			{
				if ( quickFaceNames.Contains(id.InvariantFaceName) )  // fonte fréquement utilisée ?
				{
					begin.Add(id);  // begin <- fontes fréquentes dans le même ordre que inList

					if ( selectedFaceName == id.InvariantFaceName )
					{
						selectedFaceName = null;
					}
				}

				if ( !quickOnly || id.InvariantFaceName == selectedFaceName )
				{
					outList.Add(id);  // outList <- fontes normales
				}
			}

			//	Remet les fontes rapides au début.
			int ii = 0;
			foreach ( Common.OpenType.FontIdentity id in begin )
			{
				outList.Insert(ii++, id);
			}

			quickCount = begin.Count;  // quickCount <- nombre de fontes fréquement utilisées
			return outList;
		}

		static public System.Collections.ArrayList GetFontList(bool enableSymbols)
		{
			//	Donne la liste de tous les OpenType.FontIdentity des fontes connues.
			//	Cette liste est déjà triée par ordre alphabétique.
			if ( Misc.fontListWithSymbols == null )  // cache à créer ?
			{
				Misc.fontListWithSymbols    = new System.Collections.ArrayList();
				Misc.fontListWithoutSymbols = new System.Collections.ArrayList();

				foreach( string face in TextContext.GetAvailableFontFaces() )
				{
					OpenType.FontIdentity id = Misc.DefaultFontIdentityStyle(face);
					if ( id != null )
					{
						Misc.fontListWithSymbols.Add(id);  // Misc.fontListWithSymbols <- caractères + symboles

						if ( !id.IsSymbolFont )
						{
							Misc.fontListWithoutSymbols.Add(id);  // Misc.fontListWithoutSymbols <- seulement les caractères
						}
					}
				}
			}

			return enableSymbols ? Misc.fontListWithSymbols : Misc.fontListWithoutSymbols;
		}

		static public void AddFontList(TextFieldCombo combo, bool enableSymbols)
		{
			//	Ajoute la liste des fontes dans la liste d'un TextFieldCombo.
			//	TODO: à supprimer prochainement...
			if ( Misc.fontListCombo == null )
			{
				Misc.fontListCombo = new System.Collections.ArrayList();

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

					Misc.fontListCombo.Add((symbol ? "S" : "N") + face);
				}
			}

			foreach ( string s in Misc.fontListCombo )
			{
				if ( !enableSymbols && s[0] == 'S' )  continue;
				combo.Items.Add(s.Substring(1));
			}
		}

		static public string DefaultFontStyle(string face)
		{
			//	Cherche le FontStyle par défaut pour un FontFace donné.
			OpenType.FontIdentity id = Misc.DefaultFontIdentityStyle(face);
			if ( id == null )  return "";
			return id.InvariantStyleName;
		}

		static public OpenType.FontIdentity DefaultFontIdentityStyle(string face)
		{
			//	Cherche le FontStyle par défaut pour un FontFace donné.
			OpenType.FontIdentity[] list = TextContext.GetAvailableFontIdentities(face);

			foreach ( OpenType.FontIdentity id in list )
			{
				if ( id.InvariantStyleName == "Regular" ||
					 id.InvariantStyleName == "Normal"  )
				{
					return id;
				}
			}

			foreach ( OpenType.FontIdentity id in list )
			{
				if ( id.FontWeight == OpenType.FontWeight.Normal &&
					 id.FontStyle  == OpenType.FontStyle.Normal  )
				{
					return id;
				}
			}

			foreach ( OpenType.FontIdentity id in list )
			{
				if ( id.FontWeight == OpenType.FontWeight.Normal )
				{
					return id;
				}
			}

			foreach ( OpenType.FontIdentity id in list )
			{
				if ( id.FontStyle == OpenType.FontStyle.Normal )
				{
					return id;
				}
			}

			foreach ( OpenType.FontIdentity id in list )
			{
				return id;
			}

			return null;
		}

		static public string SimplifyFontStyle(string style)
		{
			//	Simplifie un style ("Regular !Bold" devient "Bold" par exemple).
			string simple = OpenType.FontCollection.GetStyleHash(style);
			if ( simple != "" )  return simple;
			return style;
		}

		static public Font GetFont(string fontName)
		{
			//	Donne une fonte d'après son nom.
			Font font = Font.GetFont(fontName, "Regular");

			if ( font == null )
			{
				font = Font.GetFontFallback(fontName);
			}
			
			return font;
		}

		static public string GetUnicodeName(int code, string fontFace, string fontStyle)
		{
			//	Retourne le nom d'un caractère Unicode ou du glyph d'une fonte de symboles.
			Common.OpenType.Font font = TextContext.GetFont(fontFace, fontStyle);
			if ( font != null && font.FontIdentity.IsSymbolFont )
			{
				int glyph = font.GetGlyphIndex(code);
				return font.FontIdentity.GetGlyphName(glyph);
			}

			return Misc.GetUnicodeName(code);
		}

		static public string GetUnicodeName(int code)
		{
			//	Retourne le nom d'un caractère Unicode.
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
				//	Première lettre en majuscule, le reste en minuscules.
				text = string.Format("{0}{1}", text.Substring(0, 1).ToUpper(), text.Substring(1, text.Length-1).ToLower());
			}

			return text;
		}


		static public string[] DefaultFeatures()
		{
			//	Donne la liste des "features" communs à toutes les fontes.
			string[] list = new string[2];
			int i=0;
			list[i++] = "liga";
			list[i++] = "dlig";
			return list;
		}

		static public string GetFeatureText(string feature)
		{
			//	Donne le texte descriptif pour une "feature" OpenType.
			string text = Res.Strings.GetString("Text", string.Format("Features_{0}", feature));
			if ( text != null )  return text;

			return string.Format(Res.Strings.Text.FeaturesUnknow, feature.ToUpper());
		}

		static public bool IsInsideList(string[] list, string text)
		{
			//	Retourne true si text est dans list.
			if ( list == null )  return false;

			for ( int i=0 ; i<list.Length ; i++ )
			{
				if ( list[i] == text )  return true;
			}
			return false;
		}


		static public string Bold(string text)
		{
			//	Retourne le texte en gras.
			return string.Format("<b>{0}</b>", text);
		}

		static public string Italic(string text)
		{
			//	Retourne le texte en italique.
			return string.Format("<i>{0}</i>", text);
		}


		static public string Image(string icon)
		{
			//	Retourne le texte pour mettre une image dans un texte.
			return string.Format(@"<img src=""{0}""/>", Misc.Icon(icon));
		}

		static public string Icon(string icon)
		{
			//	Retourne le nom complet d'une icône.
			if ( icon == "FontBold"       ||
				 icon == "FontItalic"     ||
				 icon == "FontUnderlined" ||
				 icon == "FontOverlined"  )
			{
				string language = System.Globalization.CultureInfo.CurrentCulture.TwoLetterISOLanguageName;
				if ( language != "fr" && language != "en" && language != "de" )
				{
					language = "en";
				}
				return string.Format("manifest:Epsitec.App.DocumentEditor.Images.{0}-{1}.icon", icon, language);
			}
			else
			{
				return string.Format("manifest:Epsitec.App.DocumentEditor.Images.{0}.icon", icon);
			}
		}

		static public string GetShortCut(CommandState cs)
		{
			//	Retourne le nom des touches associées à une commande.
			if ( cs == null || cs.HasShortcuts == false )  return null;

			return cs.PreferredShortcut.ToString();
		}

		static public string GetTextWithShortcut(CommandState cs)
		{
			//	Donne le nom d'une commande, avec le raccourci clavier éventuel entre parenthèses.
			string shortcut = Misc.GetShortCut(cs);

			if ( shortcut == null )
			{
				return cs.LongCaption;
			}
			else
			{
				return string.Format("{0} ({1})", cs.LongCaption, shortcut);
			}
		}


		static public string FullName(string filename, bool dirtySerialize)
		{
			//	Donne le nom complet du fichier.
			//	Si le nom n'existe pas, donne "sans titre".
			//	Si le fichier doit être sérialisé, donne le nom en gras.
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

		static public string ExtractName(string filename, bool dirtySerialize)
		{
			//	Extrait le nom de fichier, en ignorant les noms de dossiers et l'extension.
			//	Si le nom n'existe pas, donne "sans titre".
			//	Si le fichier doit être sérialisé, donne le nom en gras.
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

		static public string ExtractName(string filename)
		{
			//	Extrait le nom de fichier, en ignorant les noms de dossiers et l'extension.
			//	"c:\rep\abc.txt" devient "abc".
			return System.IO.Path.GetFileNameWithoutExtension(filename);
		}

		static public bool IsExtension(string filename, string ext)
		{
			//	Indique si un fichier utilise une extension donnée.
			return filename.ToLower().EndsWith(ext);
		}

		static public string CopyName(string name)
		{
			//	Retourne la copie d'un nom.
			//	"Bidon"              ->  "Copie de Bidon"
			//	"Copie de Bidon"     ->  "Copie (2) de Bidon"
			//	"Copie (2) de Bidon" ->  "Copie (3) de Bidon"
			return Misc.CopyName(name, Res.Strings.Misc.Copy, Res.Strings.Misc.CopyOf);
		}

		static public string CopyName(string name, string copy, string of)
		{
			//	Retourne la copie d'un nom.
			//	copy = "Copie" ou "Copy"
			//	of = "de" ou "of"
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

		static public void Swap(ref bool a, ref bool b)
		{
			//	Permute deux variables.
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


		protected static System.Collections.ArrayList		fontListWithSymbols;
		protected static System.Collections.ArrayList		fontListWithoutSymbols;
		protected static System.Collections.ArrayList		fontListCombo;
	}
}
