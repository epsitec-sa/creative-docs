using System.Collections.Generic;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Common.Support;

namespace Epsitec.Common.Designer
{
	/// <summary>
	/// La classe Misc contient quelques routines générales.
	/// </summary>
	public class Misc
	{
		static public string CompactModuleAndName(string module, string name, bool tags = true)
		{
			//	Retourne le texte description pour un nom d'un module, sous la forme "module.name".
			//	Le nom du module est affiché en estompé (si tags = true), pour le distinguer du "name".
			if (tags)
			{
				return string.Concat ("<font color=\"#777777\">", module, ".</font>", name);
			}
			else
			{
				return string.Concat (module, ".", name);
			}
		}


		static public bool IsValidGuid(string guid)
		{
			//	Retourne true si la chaîne correspond à un Guid valide.
			//	Le Guid doit contenir 32 chiffres et 4 tirets (xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx).
			if (string.IsNullOrEmpty(guid))
			{
				return false;
			}

			return Misc.guidRegex.IsMatch(guid);
		}


		static public int DruidsPathLevel(string druidsPath)
		{
			//	Retourne le nombre de niveaux dans un chemin de Druids.
			//	Par exemple, "[102].[103]" retourne 2.
			return druidsPath.Split('.').Length;
		}

		static public string DruidsPathPart(string druidsPath, int count)
		{
			//	Retourne une partie de chemin de Druids.
			//	Par exemple, druidsPath = "[102].[103].[104].[105]" et count = 2 retourne "[102].[103]".
			int index = 0;

			while (count > 0)
			{
				int i = druidsPath.IndexOf('.', index+1);
				if (i == -1)
				{
					index = druidsPath.Length;
					break;
				}
				else
				{
					index = i;
				}

				count--;
			}

			return druidsPath.Substring(0, index);
		}


		static public Path GetCrossPath(Rectangle column, Rectangle row)
		{
			//	Retourne le chemin d'une croix. Le chemin peut également prendre la forme
			//	d'un "T" ou d'un "L" dans n'importe quelle orientation.
			Path path = new Path();

			path.MoveTo(column.BottomLeft);
			path.LineTo(column.Left, row.Bottom);
			path.LineTo(row.Left, row.Bottom);
			path.LineTo(row.Left, row.Top);
			path.LineTo(column.Left, row.Top);
			path.LineTo(column.TopLeft);
			path.LineTo(column.TopRight);
			path.LineTo(column.Right, row.Top);
			path.LineTo(row.Right, row.Top);
			path.LineTo(row.Right, row.Bottom);
			path.LineTo(column.Right, row.Bottom);
			path.LineTo(column.BottomRight);
			path.Close();

			return path;
		}

		static public Path GetCornerPath(Rectangle area)
		{
			//	Retourne le chemin contenant les 4 gros 'coins' d'une zone.
			double length = System.Math.Min(area.Width, area.Height)*0.4;
			double thickness = length*0.4;
			length = System.Math.Floor(length+0.5);
			thickness = System.Math.Floor(thickness+0.5);
			thickness = System.Math.Min(thickness, 3);

			Path path = new Path();

			path.MoveTo(area.BottomLeft);
			path.LineTo(area.Left, area.Bottom+length);
			path.LineTo(area.Left+thickness, area.Bottom+length);
			path.LineTo(area.Left+thickness, area.Bottom+thickness);
			path.LineTo(area.Left+length, area.Bottom+thickness);
			path.LineTo(area.Left+length, area.Bottom);
			path.Close();

			path.MoveTo(area.BottomRight);
			path.LineTo(area.Right, area.Bottom+length);
			path.LineTo(area.Right-thickness, area.Bottom+length);
			path.LineTo(area.Right-thickness, area.Bottom+thickness);
			path.LineTo(area.Right-length, area.Bottom+thickness);
			path.LineTo(area.Right-length, area.Bottom);
			path.Close();

			path.MoveTo(area.TopLeft);
			path.LineTo(area.Left, area.Top-length);
			path.LineTo(area.Left+thickness, area.Top-length);
			path.LineTo(area.Left+thickness, area.Top-thickness);
			path.LineTo(area.Left+length, area.Top-thickness);
			path.LineTo(area.Left+length, area.Top);
			path.Close();

			path.MoveTo(area.TopRight);
			path.LineTo(area.Right, area.Top-length);
			path.LineTo(area.Right-thickness, area.Top-length);
			path.LineTo(area.Right-thickness, area.Top-thickness);
			path.LineTo(area.Right-length, area.Top-thickness);
			path.LineTo(area.Right-length, area.Top);
			path.Close();

			return path;
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

		static public void DrawPathDash(Graphics graphics, Path path, double width, double dash, double gap, bool round, Color color)
		{
			graphics.PaintDashedOutline (path, width, dash, gap, round ? CapStyle.Round : CapStyle.Square, color);
		}

		static public void AddSpring(Graphics graphics, Point p1, Point p2, double thickness, int loops)
		{
			//	Dessine un ressort horizontal ou vertical.
			double step = Point.Distance(p1, p2)/loops;

			if (p1.Y == p2.Y)  // ressort horizontal ?
			{
				p1.X = System.Math.Min(p1.X, p2.X);
				for (int i=0; i<loops; i++)
				{
					graphics.AddLine(p1.X, p1.Y, p1.X+step*0.25, p1.Y+thickness);
					graphics.AddLine(p1.X+step*0.25, p1.Y+thickness, p1.X+step*0.75, p1.Y-thickness);
					graphics.AddLine(p1.X+step*0.75, p1.Y-thickness, p1.X+step, p1.Y);
					p1.X += step;
				}
			}
			else if (p1.X == p2.X)  // ressort vertical ?
			{
				p1.Y = System.Math.Min(p1.Y, p2.Y);
				for (int i=0; i<loops; i++)
				{
					graphics.AddLine(p1.X, p1.Y, p1.X+thickness, p1.Y+step*0.25);
					graphics.AddLine(p1.X+thickness, p1.Y+step*0.25, p1.X-thickness, p1.Y+step*0.75);
					graphics.AddLine(p1.X-thickness, p1.Y+step*0.75, p1.X, p1.Y+step);
					p1.Y += step;
				}
			}
			else
			{
				throw new System.Exception("This geometry is not implemented.");
			}
		}

		static public void AlignForLine(Graphics graphics, ref Rectangle r)
		{
			//	Aligne un rectangle pour permettre un joli Graphics.AddLine.
			if (graphics != null)
			{
				graphics.Align(ref r);
				r.Offset(0.5, 0.5);
			}
		}

		static public void AlignForLine(Graphics graphics, ref Point p)
		{
			//	Aligne un point pour permettre un joli Graphics.AddLine.
			if (graphics != null)
			{
				graphics.Align(ref p);
				p.X += 0.5;
				p.Y += 0.5;
			}
		}


		static public int IndexOfString(string[] list, string searched)
		{
			//	Cherche l'index d'une chaîne dans une liste de chaînes.
			for (int i=0; i<list.Length; i++)
			{
				if (list[i] == searched)  return i;
			}
			return -1;
		}


		static public string SourceText(CultureMapSource source)
		{
			//	Retourne le texte descriptif en fonction de la source d'une ressource.
			switch (source)
			{
				case CultureMapSource.ReferenceModule:
					return "inchangé";

				case CultureMapSource.DynamicMerge:
					return "modifié";

				case CultureMapSource.PatchModule:
					return "local";

				default:
					return null;
			}
		}

		static public Color SourceColor(CultureMapSource source)
		{
			//	Retourne la couleur de fond à utiliser en fonction de la source d'une ressource.
			switch (source)
			{
				case CultureMapSource.ReferenceModule:
					return Color.FromAlphaRgb(0.1, 0.0, 0.0, 0.0);  // noir très transparent (gris clair)

				case CultureMapSource.DynamicMerge:
					return Color.FromAlphaRgb(0.2, 1.0, 0.7, 0.0);  // orange très transparent

				default:
					return Color.Empty;
			}
		}

		static public Color AlphaColor(Color c, double a)
		{
			//	Retourne la même couleur avec la valeur alpha spécifiée.
			return Color.FromAlphaRgb(a, c.R, c.G, c.B);
		}

		static public Color FactorColor(Color c, double factor)
		{
			//	Retroune le même couleur en multipliant son canal alpha par le facteur.
			return Color.FromAlphaRgb(c.A*factor, c.R, c.G, c.B);
		}

		static public Color WarningColor
		{
			//	Retourne la couleur à utiliser pour un texte d'avertissement dans une fenêtre.
			get
			{
				IAdorner adorner = Common.Widgets.Adorners.Factory.Active;
				Color w = adorner.ColorWindow;
				if (w.GetBrightness() < 0.5)
				{
					return Color.FromRgb(1, 0.85, 0.35);  // jaune-orange
				}
				else
				{
					//?return Color.FromRgb(0.5, 0, 0);  // brun
					return Color.FromRgb(0, 0.2, 0.5);  // bleu foncé
				}
			}
		}


		//	Liste des cultures gérées par Designer, par ordre d'importance.
		static public string[] Cultures = { "fr", "en", "de", "it", "es", "pt" };

		static public string CultureBaseName(System.Globalization.CultureInfo culture)
		{
			//	Retourne le nom de base d'une culture, par exemple "fr".
			return culture.TwoLetterISOLanguageName;
		}

		static public string CultureLongName(System.Globalization.CultureInfo culture)
		{
			//	Retourne le nom long d'une culture, par exemple "Italiano (Italian, IT)".
			//?return string.Format("{0} ({1}, {2})", Misc.CultureName(culture), culture.DisplayName, Misc.CultureShortName(culture));
			return string.Format("{0} {1}", Misc.CultureName(culture), Misc.CultureShortName(culture));
		}

		static public string CultureShortName(System.Globalization.CultureInfo culture)
		{
			//	Retourne le nom court ISO (2 lettres) d'une culture, par exemple "[fr]".
			return string.Format("[{0}]", culture.TwoLetterISOLanguageName);
		}

		static public string CultureName(System.Globalization.CultureInfo culture)
		{
			//	Retourne le nom standard (Français, Deutsch, English, etc.) d'une culture.
			return Misc.ProperName(culture.NativeName);
		}

		static public string CultureName(string twoLettersCode)
		{
			//	Retourne le nom standard (Français, Deutsch, English, etc.) d'une culture.
			System.Globalization.CultureInfo culture = Support.Resources.FindCultureInfo(twoLettersCode);
			return (culture == null) ? Misc.Italic("(indéfini)") : Misc.ProperName(culture.NativeName);
		}


		static public string ProperName(string text)
		{
			//	Retourne le texte avec une majuscule au début.
			if (text.Length <= 1)
			{
				return text.ToUpper();
			}
			else
			{
				return string.Concat(text.Substring(0, 1).ToUpper(), text.Substring(1));
			}
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


		static public bool IsValidLabel(ref string label, bool isEntityOrForm)
		{
			//	Vérifie si un nom de label est correct.
			if (isEntityOrForm)
			{
				return Misc.IsValidName(ref label, isEntityOrForm);
			}
			else
			{
				string[] list = label.Split('.');
				System.Text.StringBuilder builder = new System.Text.StringBuilder(label.Length);
				for (int i=0; i<list.Length; i++)
				{
					if (!Misc.IsValidName(ref list[i], isEntityOrForm))
					{
						return false;
					}

					if (i > 0)
					{
						builder.Append('.');
					}
					builder.Append(list[i]);
				}

				label = builder.ToString();
				return true;
			}
		}

		static protected bool IsValidName(ref string name, bool isEntityOrForm)
		{
			//	Vérifie si un nom commence par une lettre puis est suivi de lettres ou de chiffres.
			//	Le nom retourné commence par une majuscule suivie de minuscules.
			if (name.Length == 0)
			{
				return false;
			}

			System.Text.StringBuilder builder = new System.Text.StringBuilder(name.Length);
			bool first = true;
			foreach (char c in name)
			{
				if (first)
				{
					if (Misc.IsUpperLetter(c))
					{
						builder.Append(c);
						first = false;
					}
					else if (Misc.IsLowerLetter(c))
					{
						builder.Append(System.Char.ToUpper(c));
						first = false;
					}
					else
					{
						return false;
					}
				}
				else
				{
					if (c >= '0' && c <= '9')
					{
						builder.Append(c);
					}
					else if (Misc.IsUpperLetter(c))
					{
						builder.Append(c);
					}
					else if (Misc.IsLowerLetter(c))
					{
						builder.Append(c);
					}
					else if (c == '_' && isEntityOrForm)
					{
						builder.Append(c);
					}
					else
					{
						return false;
					}
				}
			}

			name = builder.ToString();
			return true;
		}

		static protected bool IsLowerLetter(char c)
		{
			//	Indique si un caractère est une lettre.
			//	TODO: traiter tous les accents unicode ?
			switch (c)
			{
				case 'á':
				case 'à':
				case 'â':
				case 'ä':
				case 'ã':

				case 'ç':

				case 'é':
				case 'è':
				case 'ê':
				case 'ë':

				case 'í':
				case 'ì':
				case 'î':
				case 'ï':

				case 'ñ':

				case 'ó':
				case 'ò':
				case 'ô':
				case 'ö':
				case 'õ':

				case 'ú':
				case 'ù':
				case 'û':
				case 'ü':
					return true;
			}

			return (c >= 'a' && c <= 'z');
		}

		static protected bool IsUpperLetter(char c)
		{
			//	Indique si un caractère est une lettre.
			//	TODO: traiter tous les accents unicode ?
			switch (c)
			{
				case 'Á':
				case 'À':
				case 'Â':
				case 'Ä':
				case 'Ã':

				case 'Ç':

				case 'É':
				case 'È':
				case 'Ê':
				case 'Ë':

				case 'Í':
				case 'Ì':
				case 'Î':
				case 'Ï':

				case 'Ñ':

				case 'Ó':
				case 'Ò':
				case 'Ô':
				case 'Ö':
				case 'Õ':

				case 'Ú':
				case 'Ù':
				case 'Û':
				case 'Ü':
					return true;
			}

			return (c >= 'A' && c <= 'Z');
		}


		static public void ComboMenuToList(TextFieldCombo combo, List<string> list)
		{
			//	Copie le contenu du combo-menu dans une liste.
			list.Clear();

			foreach (string s in combo.Items)
			{
				list.Add(s);
			}
		}

		static public void ComboMenuFromList(TextFieldCombo combo, List<string> list)
		{
			//	Copie une liste dans le combo-menu.
			combo.Items.Clear();

			foreach (string s in list)
			{
				combo.Items.Add(s);
			}
		}

		static public void ComboMenuAdd(TextFieldCombo combo)
		{
			//	Ajoute le texte actuel dans le combo-menu.
			Misc.ComboMenuAdd(combo, combo.Text);
		}

		static public void ComboMenuAdd(TextFieldCombo combo, string text)
		{
			//	Ajoute un texte dans le combo-menu.
			if ( combo.Items.Contains(text) )
			{
				combo.Items.Remove(text);
			}
			combo.Items.Insert(0, text);
		}


		static public Command CreateStructuredCommandWithName(string commandName)
		{
			return null;
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


		static public string RemoveTags(string text)
		{
			//	Supprime tous les tags xml <...> dans un texte.
			while (true)
			{
				int start = text.IndexOf("<");
				if (start == -1)
				{
					break;
				}

				int end = text.IndexOf(">", start);
				if (end == -1)
				{
					break;
				}

				text = text.Remove(start, end-start+1);
			}

			return text;
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


		static public string GetMenuIconCheckState(bool state)
		{
			//	Retourne l'icône à utiliser dans un menu, pour un item booléen de type check.
			return state ? Misc.Icon("ActiveYes") : Misc.Icon("ActiveNo");
		}

		static public string GetMenuIconRadioState(bool state)
		{
			//	Retourne l'icône à utiliser dans un menu, pour un item booléen de type radio.
			return state ? Misc.Icon("RadioYes") : Misc.Icon("RadioNo");
		}

		static public void GetIconNames(string fullName, out string moduleName, out string shortName)
		{
			//	Fractionne un nom du type "manifest:Epsitec.Common.Designer.Images.xxx.icon".
			if (string.IsNullOrEmpty(fullName))
			{
				moduleName = null;
				shortName = null;
			}
			else
			{
				string infix  = ".Images.";
				string suffix = ".icon";

				int infixPos  = fullName.IndexOf(infix);
				int prefixLen = fullName.IndexOf(':') + 1;

				if (infixPos > 0)
				{
					//	"manifest:Epsitec.Common.Designer.Images.Xyz.Abc.icon" produit les
					//	résultats suivants :
					//
					//	moduleName = "Epsitec.Common.Designer"
					//	shortName  = "Xyz.Abc"

					moduleName = fullName.Substring(prefixLen, infixPos - prefixLen);
					shortName  = fullName.Substring(infixPos + infix.Length);

					if (shortName.EndsWith(suffix))
					{
						shortName = shortName.Substring(0, shortName.Length - suffix.Length);
					}
				}
				else
				{
					//	TODO: faire mieux !
					string[] parts = fullName.Split('.');
					moduleName = parts[parts.Length-4];
					shortName = parts[parts.Length-2];
				}
			}
		}

		static public string ImageFull(string fullName)
		{
			//	Retourne le texte pour mettre une image dans un texte.
			return string.Format(@"<img src=""{0}""/>", fullName);
		}

		static public string ImageFull(string fullName, double verticalOffset)
		{
			//	Retourne le texte pour mettre une image dans un texte.
			return string.Format(@"<img src=""{0}"" voff=""{1}""/>", fullName, verticalOffset.ToString(System.Globalization.CultureInfo.InvariantCulture));
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
			return string.Format("manifest:Epsitec.Common.Designer.Images.{0}.icon", icon);
		}

		static public string IconDyn(string name, string parameter)
		{
			//	Retourne le nom complet d'une icône dynamique.
			return string.Format("dyn:{0}/{1}", name, parameter);
		}


		static public string GetShortcut(Command command)
		{
			//	Retourne le nom des touches associées à une commande.
			if (command == null || command.HasShortcuts == false)
				return null;

			return command.PreferredShortcut.ToString();
		}

		static public string GetTextWithShortcut(Command command)
		{
			//	Donne le nom d'une commande, avec le raccourci clavier éventuel entre parenthèses.
			string shortcut = Misc.GetShortcut(command);

			if (shortcut == null)
			{
				return command.Description;
			}
			else
			{
				return string.Format("{0} ({1})", command.Description, shortcut);
			}
		}


		static public string DescriptionName(string moduleName, bool dirtySerialize, bool isPatch)
		{
			//	Retourne le nom d'un module et son état.
			var builder = new System.Text.StringBuilder();

			if (string.IsNullOrEmpty (moduleName))
			{
				builder.Append (Res.Strings.Misc.NoTitle);
			}
			else
			{
				builder.Append (Misc.ExtractName (moduleName, shortName: false));
			}

			if (isPatch || dirtySerialize)
			{
				builder.Append (" (");

				if (dirtySerialize)
				{
					builder.Append ("modifié");
				}

				if (isPatch)
				{
					if (dirtySerialize)
					{
						builder.Append (", ");
					}

					builder.Append ("patch");
				}

				builder.Append (")");
			}

			return builder.ToString ();
		}

		static public string ExtractName(string moduleName, bool dirtySerialize, bool isPatch)
		{
			//	Extrait le nom de module.
			//	Si le nom n'existe pas, donne "sans titre".
			//	Si le module doit être sérialisé, donne le nom en gras.
			//	Si le module provient d'un patch, donne le nom en italique.
			string name = string.IsNullOrEmpty (moduleName) ? Res.Strings.Misc.NoTitle : Misc.ExtractName (moduleName, shortName: true);

			if (isPatch)
			{
				name = Misc.Italic (name);
			}

			if (dirtySerialize)
			{
				name = Misc.Bold (name);
			}

			return name;
		}

		static public string ExtractName(string moduleName, bool shortName)
		{
			//	Extrait le nom de module.
			//	Si shortName = true, "Cresus.Core.Business" devient "Business".
			if (shortName)
			{
				int index = moduleName.LastIndexOf ('.');

				if (index != -1)
				{
					moduleName = moduleName.Substring (index+1);
				}
			}

			return TextLayout.ConvertToTaggedText(moduleName);
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


		static private readonly System.Text.RegularExpressions.Regex guidRegex = new System.Text.RegularExpressions.Regex("^[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}$", System.Text.RegularExpressions.RegexOptions.IgnoreCase | System.Text.RegularExpressions.RegexOptions.Compiled);
	}
}
