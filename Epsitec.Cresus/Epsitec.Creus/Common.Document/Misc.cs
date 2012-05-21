using Epsitec.Common.Drawing;
using Epsitec.Common.Support;
using Epsitec.Common.Text;
using Epsitec.Common.Widgets;

namespace Epsitec.Common.Document
{
	/// <summary>
	/// La classe Misc contient quelques routines générales.
	/// </summary>
	public class Misc
	{
		[System.Flags] public enum StringSearch
		{
			IgnoreMaj    = 0x00000001,	// m = M
			IgnoreAccent = 0x00000002,	// é = e
			WholeWord    = 0x00000004,	// mot entier
			EndToStart   = 0x00000008,	// sens inverse (en arrière)
		}


		static public void AutoFocus(Widget widget, bool state)
		{
			//	Modifie le mode AutoFocus d'un widget et de tous ses enfants.
			foreach (Widget child in widget.FindAllChildren())
			{
				child.AutoFocus = state;
			}
		}


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
				string  dec = Types.InvariantConverter.ExtractDecimal(ref text);
					
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


		static public Path GetHatchPath(Rectangle rect, double distance, Point reference)
		{
			//	Retourne des hachures à 45 degrés remplissant sans déborder un rectangle.
			//	Une hachure passe toujours par le point de référence.
			Path path = new Path();

			//	Déplace le point de référence sur le bord gauche du rectangle.
			reference.Y += rect.Left - reference.X;
			reference.X = rect.Left;
			double d = reference.Y - rect.Bottom;

			double v = System.Math.Ceiling(rect.Width/distance) * distance;
			v -= d % distance;

			for (double y=rect.Bottom-v; y<rect.Top; y+=distance)
			{
				double x1 = rect.Left;
				double y1 = y;
				double x2 = rect.Right;
				double y2 = y+rect.Width;

				if (y1 < rect.Bottom)
				{
					x1 += rect.Bottom-y1;
					y1 = rect.Bottom;
				}

				if (y2 > rect.Top)
				{
					x2 -= y2-rect.Top;
					y2 = rect.Top;
				}

				if (x1 < x2)
				{
					path.MoveTo(x1, y1);
					path.LineTo(x2, y2);
				}
			}

			return path;
		}

		
		static public string GetColorNiceName(RichColor color)
		{
			//	Donne le nom d'une couleur d'après ses composantes.
			System.Text.StringBuilder builder = new System.Text.StringBuilder();

			if ( color.ColorSpace == ColorSpace.Cmyk )
			{
				builder.Append(Res.Strings.Color.Cmyk);
				builder.Append(":");
			}

			if ( color.ColorSpace == ColorSpace.Gray )
			{
				builder.Append(Res.Strings.Color.Gray);
				builder.Append(":");
			}

			if ( color.ColorSpace == ColorSpace.Cmyk && color.C == 1.0 && color.M == 1.0 && color.Y == 1.0 && color.K == 1.0 )
			{
				builder.Append(Res.Strings.Color.RegistrationBlack);
			}
			else if ( color.R == color.G && color.R == color.B )  // gris ?
			{
				double br = color.Basic.GetBrightness();

				     if ( br == 0.0 )  builder.Append(Res.Strings.Color.Black);
				else if ( br == 1.0 )  builder.Append(Res.Strings.Color.White);
				else if ( br <  0.3 )  builder.Append(Res.Strings.Color.DarkGray);
				else if ( br >  0.7 )  builder.Append(Res.Strings.Color.LightGray);
				else                   builder.Append(Res.Strings.Color.Gray);
			}
			else
			{
				double h,s,v;
				color.Basic.GetHsv(out h, out s, out v);

				     if ( h < 30+60*0 )  builder.Append(Res.Strings.Color.Red);
				else if ( h < 30+60*1 )  builder.Append(Res.Strings.Color.Yellow);
				else if ( h < 30+60*2 )  builder.Append(Res.Strings.Color.Green);
				else if ( h < 30+60*3 )  builder.Append(Res.Strings.Color.Cyan);
				else if ( h < 30+60*4 )  builder.Append(Res.Strings.Color.Blue);
				else if ( h < 30+60*5 )  builder.Append(Res.Strings.Color.Magenta);
				else                     builder.Append(Res.Strings.Color.Red);
			}

			if ( color.A < 1.0 )  // partiellement transparent ?
			{
				builder.Append(" (");
				builder.Append(Res.Strings.Color.Alpha);
				builder.Append("=");
				builder.Append((color.A*255).ToString("F0"));
				builder.Append(")");
			}

			return builder.ToString();
		}


		static public string FaceInvariantToInvariant(string face, string style)
		{
			//	Conversion d'un nom de famille en filtrant en plus d'éventuels
			//	"Black" qui traineraient derrière un "Arial Black".
#if false
			if (face == null || style == null)
			{
				return null;
			}

			OpenType.FontName fontName = new OpenType.FontName (face, style);
			OpenType.FontIdentity id = OpenType.FontCollection.Default[fontName];

			if (id == null)
			{
				return null;
			}

			return id.InvariantFaceName;
#else
			return face;
#endif
		}

		static public string FaceInvariantToLocale(string face, string style)
		{
			//	Conversion d'un nom de famille dans la culture locale, en
			//	filtrant en plus d'éventuels "Black" qui traineraient derrière
			//	un "Arial Black".
			if (face == null || style == null)
			{
				return null;
			}

			OpenType.FontName fontName = new OpenType.FontName (face, style);
			OpenType.FontIdentity id = OpenType.FontCollection.Default[fontName];

			if (id == null)
			{
				return null;
			}

			return id.LocaleFaceName;
		}

		static public string StyleInvariantToLocale(string face, string style)
		{
			//	Conversion d'un nom de style dans la culture locale.
			//	Par exemple, 'Bold' devient 'Gras'.
			if (face == null || style == null)
			{
				return null;
			}

			OpenType.FontName fontName = new OpenType.FontName(face, style);
			OpenType.FontIdentity id = OpenType.FontCollection.Default[fontName];

			if (id == null)
			{
				return null;
			}

			return id.LocaleStyleName;
		}

		static public System.Collections.ArrayList MergeFontList(System.Collections.ArrayList inList, System.Collections.ArrayList quickFaceNames, bool quickOnly, string selectedFaceName, out int quickCount)
		{
			//	Crée une liste qui contient les polices rapides au début.
			//	En mode quickOnly (liste courte), la police selectedFaceName apparaît même si elle
			//	ne fait pas partie des polices rapides.
			//	inList: tous les OpenType.FontIdentity connus
			//	quickFaceNames: strings des FaceNames fréquement utilisés, dans n'importe quel ordre
			//	outList (liste retournée): OpenType.FontIdentity
			System.Collections.ArrayList outList = new System.Collections.ArrayList();
			System.Collections.ArrayList begin   = new System.Collections.ArrayList();

			if ( quickFaceNames.Count == 0 )  quickOnly = false;

			//	Copie la liste en enlevant toutes les polices rapides.
			foreach ( Common.OpenType.FontIdentity id in inList )
			{
				if ( quickFaceNames.Contains(id.InvariantFaceName) )  // police fréquement utilisée ?
				{
					begin.Add(id);  // begin <- polices fréquentes dans le même ordre que inList

					if ( selectedFaceName == id.InvariantFaceName )
					{
						selectedFaceName = null;
					}
				}

				if ( !quickOnly || id.InvariantFaceName == selectedFaceName )
				{
					outList.Add(id);  // outList <- polices normales
				}
			}

			//	Remet les polices rapides au début.
			int ii = 0;
			foreach ( Common.OpenType.FontIdentity id in begin )
			{
				outList.Insert(ii++, id);
			}

			quickCount = begin.Count;  // quickCount <- nombre de polices fréquement utilisées
			return outList;
		}

		static public void ClearFontList()
		{
			//	Met à zéro les listes des polices connues. Cela forcera à les refaire.
			Misc.fontListWithSymbols    = null;
			Misc.fontListWithoutSymbols = null;
		}

		static public System.Collections.ArrayList GetFontList(bool enableSymbols)
		{
			//	Donne la liste de tous les OpenType.FontIdentity des polices connues.
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

		static public bool IsExistingFont(OpenType.FontName fontName)
		{
			//	Indique si une police existe, c'est-à-dire si elle est installée.
			if (fontName.StyleName == "")
			{
				OpenType.FontIdentity did = Misc.DefaultFontIdentityStyle(fontName.FaceName);
				if (did != null)
				{
					fontName = new OpenType.FontName(did.InvariantFaceName, did.InvariantStyleName);
				}
			}

			OpenType.FontIdentity id = OpenType.FontCollection.Default[fontName];
			return (id != null);
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

		static public bool IsExistingFontStyle(string face, string style)
		{
			//	Cherche si un FontStyle existe pour un FontFace donné.
			OpenType.FontIdentity[] list = TextContext.GetAvailableFontIdentities(face);

			foreach ( OpenType.FontIdentity id in list )
			{
				if ( id.InvariantStyleName == style )  return true;
			}

			return false;
		}

		static public Font GetFont(string fontName)
		{
			//	Donne une police d'après son nom.
			Font font = Font.GetFont(fontName, "Regular");

			if ( font == null )
			{
				font = Font.GetFontFallback(fontName);
			}
			
			return font;
		}

		static public string GetUnicodeName(int code, string fontFace, string fontStyle)
		{
			//	Retourne le nom d'un caractère Unicode ou du glyph d'une police de symboles.
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

			string text = TextBreak.GetUnicodeName (code);

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
			//	Donne la liste des "features" communs à toutes les polices.
			string[] list = new string[2];
			int i=0;
			list[i++] = "liga";
			list[i++] = "dlig";
			return list;
		}

		static public string GetFeatureText(string feature)
		{
			//	Donne le texte descriptif pour une "feature" OpenType.
			string text = Res.Strings.GetString ("Text", string.Format ("Features_{0}", feature));
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


		static public int IndexOf(string text, string value, int startIndex, StringSearch mode)
		{
			int count;
			if ( (mode&StringSearch.EndToStart) != 0 )  // en arrière ?
			{
				count = startIndex+1;
			}
			else	// en avant ?
			{
				count = text.Length-startIndex;
			}

			return Misc.IndexOf(text, value, startIndex, count, mode);
		}

		static public int IndexOf(string text, string value, int startIndex, int count, StringSearch mode)
		{
			//	Cherche l'index de 'value' dans 'text' (un peu comme string.IndexOf), mais avec quelques
			//	options supplémentaires.
			//	Lorsqu'on recule (StringSearch.EndToStart), 'startIndex' est à la fin (sur le premier
			//	caractère cherché) et 'count' est positif (mais compte de droite à gauche).
			//	Cette façon absurde de procéder est celle de string.LastIndexOf !
			//	TODO: optimiser la vitesse !
			if ( (mode&StringSearch.EndToStart) != 0 )  // en arrière ?
			{
				startIndex = System.Math.Min(startIndex, text.Length);
				count = System.Math.Min(count, startIndex+1);
			}
			else	// en avant ?
			{
				startIndex = System.Math.Max(startIndex, 0);
				count = System.Math.Min(count, text.Length-startIndex);
			}

			if ( count <= 0 || text.Length < value.Length )  return -1;

			if ( (mode&StringSearch.IgnoreMaj) != 0 )  // maj = min ?
			{
				text = text.ToLower();
				value = value.ToLower();
			}

			if ( (mode&StringSearch.IgnoreAccent) != 0 )  // é = e ?
			{
				text = StringUtils.RemoveDiacritics(text);
				value = StringUtils.RemoveDiacritics(value);
			}

			if ( (mode&StringSearch.WholeWord) != 0 )  // mot entier ?
			{
				if ( (mode&StringSearch.EndToStart) != 0 )  // en arrière ?
				{
					int begin = startIndex-count+1;
					while ( true )
					{
						startIndex = text.LastIndexOf(value, startIndex, count);
						if ( startIndex == -1 )  return -1;
						if ( Misc.IsWholeWord(text, startIndex, value.Length) )  return startIndex;
						startIndex --;
						count = startIndex-begin+1;
						if ( startIndex < 0 )  return -1;
					}
				}
				else	// en avant ?
				{
					int length = startIndex+count;
					while ( true )
					{
						startIndex = text.IndexOf(value, startIndex, count);
						if ( startIndex == -1 )  return -1;
						if ( Misc.IsWholeWord(text, startIndex, value.Length) )  return startIndex;
						startIndex ++;
						count = length-startIndex;
						if ( count <= 0 )  return -1;
					}
				}
			}
			else
			{
				if ( (mode&StringSearch.EndToStart) != 0 )  // en arrière ?
				{
					return text.LastIndexOf(value, startIndex, count);
				}
				else	// en avant ?
				{
					return text.IndexOf(value, startIndex, count);
				}
			}
		}

		static protected bool IsWholeWord(string text, int index, int count)
		{
			//	Vérifie si un mot et précédé et suivi d'un caractère séparateur de mots.
			if ( index > 0 )
			{
				char c1 = text[index-1];
				char c2 = text[index];
				if ( !Text.Unicode.IsWordStart(c2, c1) )  return false;
			}

			if ( index+count < text.Length )
			{
				char c1 = text[index+count-1];
				char c2 = text[index+count];
				if ( !Text.Unicode.IsWordEnd(c2, c1) )  return false;
			}

			return true;
		}


		static public string Resume(string text)
		{
			//	Retourne une version résumée à environ 20 caractères au maximum.
			return Misc.Resume(text, 20);
		}
		
		static public string Resume(string text, int max)
		{
			//	Retourne une version résumée à environ 'max' caractères au maximum.
			System.Diagnostics.Debug.Assert(max > 2);
			if ( text.Length > max )
			{
				return string.Concat(text.Substring(0, max-2), "...");
			}
			else
			{
				return text;
			}
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

		static public string FontSize(string text, double size)
		{
			//	Retourne le texte dans une autre taille.
			return string.Format("<font size=\"{1}%\">{0}</font>", text, (size*100).ToString(System.Globalization.CultureInfo.InvariantCulture));
		}


		static public string Image(string icon)
		{
			//	Retourne le texte pour mettre une image dans un texte.
			return string.Format(@"<img src=""{0}""/>", Misc.Icon(icon));
		}

		static public string Image(string icon, double verticalOffset)
		{
			//	Retourne le texte pour mettre une image dans un texte.
			return string.Format(@"<img src=""{0}"" voff=""{1}""/>", Misc.Icon(icon), verticalOffset.ToString(System.Globalization.CultureInfo.InvariantCulture));
		}

		static public string ImageDyn(string name, string parameter)
		{
			//	Retourne le texte pour mettre une image dynamique dans un texte.
			return string.Format(@"<img src=""{0}""/>", Misc.IconDyn(name, parameter));
		}

		static public string ImageDyn(string name, string parameter, double verticalOffset)
		{
			//	Retourne le texte pour mettre une image dynamique dans un texte.
			return string.Format(@"<img src=""{0}"" voff=""{1}""/>", Misc.IconDyn(name, parameter), verticalOffset.ToString(System.Globalization.CultureInfo.InvariantCulture));
		}

		static public Size IconPreferredSize(string iconSize)
		{
			//	Retourne la taille préférée pour une icône. Si la taille réelle de l'icône n'est
			//	pas exactement identique, ce n'est pas important. Drawing.Canvas cherche au mieux.
			if ( iconSize == "Small" )  return new Size(14, 14);
			if ( iconSize == "Large" )  return new Size(31, 31);
			return new Size(20, 20);
		}

		static public string Icon(string icon)
		{
			//	Retourne le nom complet d'une icône.
			if (string.IsNullOrEmpty(icon))
			{
				return null;
			}
			
			System.Diagnostics.Debug.Assert(icon.StartsWith("manifest:") == false);
			System.Diagnostics.Debug.Assert(icon.EndsWith(".icon") == false);

			return string.Format ("manifest:Epsitec.Common.DocumentEditor.Images.{0}.icon", icon);
		}

		static public string IconDyn(string name, string parameter)
		{
			//	Retourne le nom complet d'une icône dynamique.
			return string.Format("dyn:{0}/{1}", name, parameter);
		}

		static public string GetShortcut(Command command)
		{
			//	Retourne le nom des touches associées à une commande.
			if ( command == null || command.HasShortcuts == false )  return null;

			return command.PreferredShortcut.ToString();
		}

		static public Command CreateStructuredCommandWithName(string commandName)
		{
			return null;
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
				name += TextLayout.ConvertToTaggedText(filename);
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
			return TextLayout.ConvertToTaggedText(System.IO.Path.GetFileNameWithoutExtension(filename));
		}

		static public bool IsExtension(string filename, string ext)
		{
			//	Indique si un fichier utilise une extension donnée.
			string fileExt = System.IO.Path.GetExtension(filename);
			return string.Equals(fileExt, ext, System.StringComparison.OrdinalIgnoreCase);
		}

		static public bool IsTextStyleName(string name)
		{
			//	Indique s'il s'agit d'un nom de style de paragraphe ou de caractère.
			return name.StartsWith("P.") || name.StartsWith("C.");
		}

		static public string UserTextStyleName(string name)
		{
			//	Retourne le nom d'un style de paragraphe ou de caractère pour l'utilisateur.
			if ( Misc.IsTextStyleName(name) )
			{
				return name.Substring(2);
			}
			return name;
		}

		static public string CopyName(string name)
		{
			//	Retourne la copie d'un nom.
			//	"Bidon"              ->  "Copie de Bidon"
			//	"Copie de Bidon"     ->  "Copie (2) de Bidon"
			//	"Copie (2) de Bidon" ->  "Copie (3) de Bidon"
			if ( Misc.IsTextStyleName(name) )
			{
				string start = name.Substring(0, 2);
				string end   = name.Substring(2);
				return start + Misc.CopyName(end, Res.Strings.Misc.Copy, Res.Strings.Misc.CopyOf);
			}
			else
			{
				return Misc.CopyName(name, Res.Strings.Misc.Copy, Res.Strings.Misc.CopyOf);
			}
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
				return string.Concat(copy, " (", num.ToString(System.Globalization.CultureInfo.InvariantCulture), name.Substring(i));
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
