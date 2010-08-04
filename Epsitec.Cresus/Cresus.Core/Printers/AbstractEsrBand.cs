//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Debug;
using Epsitec.Common.Dialogs;
using Epsitec.Common.Drawing;
using Epsitec.Common.IO;
using Epsitec.Common.Printing;
using Epsitec.Common.Support;
using Epsitec.Common.Types;
using Epsitec.Common.Widgets;

using Epsitec.Common.Support.EntityEngine;
using Epsitec.Cresus.Core.Entities;

using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Epsitec.Cresus.Core.Printers
{
	/// <summary>
	/// Classe générique permettant de dessiner un BVR orange ou un BV rose.
	/// </summary>
	public abstract class AbstractEsrBand : AbstractBand
	{
		public AbstractEsrBand()
			: base ()
		{
			this.PaintEsrSimulator = true;
			this.PaintSpecimen = true;
		}


		public static Size DefautlSize
		{
			get
			{
				return new Size (210, 106);
			}
		}


		/// <summary>
		/// true  -> Dessine un faux BVR orangé ou BV rose sur du papier vierge. A n'utiliser que pour l'aperçu avant impression.
		/// false -> Ne dessine que les informations réelles sur du papier avec un BVR/BV préimprimé.
		/// </summary>
		public bool PaintEsrSimulator
		{
			get;
			set;
		}

		/// <summary>
		/// Indique s'il faut dessiner un gros "SPECIMEN".
		/// </summary>
		public bool PaintSpecimen
		{
			get;
			set;
		}

		/// <summary>
		/// Indique s'il s'agit d'un faux BV avec un montant "XXXXX XX".
		/// </summary>
		public bool NotForUse
		{
			get;
			set;
		}

		/// <summary>
		/// Texte multi-lignes "versé par".
		/// </summary>
		public string From
		{
			get;
			set;
		}

		/// <summary>
		/// Texte multi-lignes "versement pour".
		/// </summary>
		public string To
		{
			get;
			set;
		}

		/// <summary>
		/// Texte pour la zone de communication (seulement pour les BV, pas les BVR).
		/// </summary>
		public string Communication
		{
			get;
			set;
		}

		/// <summary>
		/// Numéro de CCP.
		/// Par exemple "01-69444-3".
		/// </summary>
		public string EsrCustomerNumber
		{
			get;
			set;
		}

		/// <summary>
		/// Ligne de codage imprimée en OCR-B, avec ou sans les espaces.
		/// Par exemple "96 13070 01000 02173 50356 73892".
		/// </summary>
		public string EsrReferenceNumber
		{
			get;
			set;
		}

		/// <summary>
		/// Montant imprimé. Un montant de 0.0M permet au client d'inscrire lui-même le montant.
		/// </summary>
		public decimal Price
		{
			get;
			set;
		}


		public override double RequiredHeight(double width)
		{
			return AbstractEsrBand.DefautlSize.Height;
		}


		protected virtual Color LightPinkColor(bool isPreview)
		{
			return Color.Empty;
		}

		protected virtual Color DarkPinkColor(bool isPreview)
		{
			return Color.Empty;
		}


		/// <summary>
		/// Effectue la justification verticale pour découper le texte en sections.
		/// </summary>
		/// <param name="width">Largeur pour toutes les sections</param>
		/// <param name="initialHeight">Hauteur de la première section</param>
		/// <param name="middleheight">Hauteur des sections suivantes</param>
		/// <param name="finalHeight">Hauteur de la dernière section</param>
		/// <returns>Retourne false s'il n'a pas été possible de mettre tout le contenu</returns>
		public override bool BuildSections(double width, double initialHeight, double middleheight, double finalHeight)
		{
			return true;
		}

		public override int SectionCount
		{
			get
			{
				return 1;
			}
		}

		/// <summary>
		/// Retourne la hauteur que l'objet occupe dans une section.
		/// </summary>
		/// <param name="section"></param>
		/// <returns></returns>
		public override double GetSectionHeight(int section)
		{
			if (section == 0)
			{
				return AbstractEsrBand.DefautlSize.Height;
			}

			return 0;
		}

		/// <summary>
		/// Dessine une section de l'objet à une position donnée.
		/// </summary>
		/// <param name="port">Port graphique</param>
		/// <param name="section">Rang de la section à dessiner</param>
		/// <param name="topLeft">Coin supérieur gauche</param>
		/// <returns>Retourne false si le contenu est trop grand et n'a pas pu être dessiné</returns>
		public override bool Paint(IPaintPort port, bool isPreview, int section, Point topLeft)
		{
			if (section != 0)
			{
				return true;
			}

			if (this.PaintEsrSimulator)
			{
				this.PaintFix (port, isPreview, topLeft);
			}
			else
			{
				this.PaintSpecimenPattern (port);
			}

			this.PaintContent(port, topLeft);

			return true;
		}


		protected virtual void PaintFix(IPaintPort port, bool isPreview, Point topLeft)
		{
			//	Dessine tous éléments fixes, pour simuler un BV sur un fond blanc.

			//	Dessine les fonds.
			port.Color = this.LightPinkColor (isPreview);
			port.PaintSurface (Path.FromRectangle (topLeft.X, topLeft.Y-106, 210, 106));

			port.Color = Color.FromBrightness (1);
			port.PaintSurface (Path.FromRectangle (topLeft.X+60, topLeft.Y-106, 150, 25));

			this.PaintSpecimenPattern (port);

			//	Dessine les grandes lignes noires de séparation.
			port.LineWidth = 0.15;
			port.Color = Color.FromBrightness (0.7);
			AbstractEsrBand.PaintCutLine (port, new Point (topLeft.X+0, topLeft.Y-0), new Point (topLeft.X+210, topLeft.Y-0));
			AbstractEsrBand.PaintCutLine (port, new Point (topLeft.X+60, topLeft.Y-5), new Point (topLeft.X+60, topLeft.Y-106));

			port.Color = Color.FromBrightness (0);
			port.PaintOutline (Path.FromLine (topLeft.X, topLeft.Y-5, topLeft.X+210, topLeft.Y-5));
			port.PaintOutline (Path.FromLine (topLeft.X+60, topLeft.Y, topLeft.X+60, topLeft.Y-5));
			port.PaintOutline (Path.FromLine (topLeft.X+121, topLeft.Y-5, topLeft.X+121, topLeft.Y-81));
			port.PaintOutline (Path.FromLine (topLeft.X+177, topLeft.Y-5, topLeft.X+177, topLeft.Y-30));
			port.PaintOutline (Path.FromLine (topLeft.X+121, topLeft.Y-30, topLeft.X+210, topLeft.Y-30));

			//	Dessine les 'L' en bas à gauche et à droite.
			port.Color = Color.FromBrightness (0);
			port.LineWidth = 0.3;
			port.PaintOutline (Path.FromLine (topLeft.X+63, topLeft.Y-79, topLeft.X+63, topLeft.Y-81));
			port.PaintOutline (Path.FromLine (topLeft.X+63, topLeft.Y-81, topLeft.X+68, topLeft.Y-81));
			port.PaintOutline (Path.FromLine (topLeft.X+204, topLeft.Y-79, topLeft.X+204, topLeft.Y-81));
			port.PaintOutline (Path.FromLine (topLeft.X+204, topLeft.Y-81, topLeft.X+199, topLeft.Y-81));

			//	Dessine les cercles.
			port.Color = this.DarkPinkColor (isPreview);
			port.LineWidth = 0.1;
			AbstractEsrBand.PaintDashedCircle (port, new Point (topLeft.X+194, topLeft.Y-17), 9);
			AbstractEsrBand.PaintDashedCircle (port, new Point (topLeft.X+17, topLeft.Y-95), 9);

			//	Dessine les textes.
			port.Color = Color.FromBrightness (0);
			port.PaintText (topLeft.X+2, topLeft.Y-4, "Empfangsschein / Récépissé / Ricevuta", AbstractEsrBand.fixFontBold, 3.0);
			port.PaintText (topLeft.X+64, topLeft.Y-4, "Einzahlung Giro", AbstractEsrBand.fixFontBold, 3.0);
			port.PaintText (topLeft.X+114, topLeft.Y-4, "Versement Virement", AbstractEsrBand.fixFontBold, 3.0);
			port.PaintText (topLeft.X+173, topLeft.Y-4, "Versamento Girata", AbstractEsrBand.fixFontBold, 3.0);
			port.PaintText (topLeft.X+2, topLeft.Y-48.5, "CHF", AbstractEsrBand.fixFontRegular, 3.0);
			port.PaintText (topLeft.X+63, topLeft.Y-48.5, "CHF", AbstractEsrBand.fixFontRegular, 3.0);

			port.Color = this.DarkPinkColor (isPreview);
			port.PaintText (topLeft.X+2, topLeft.Y-7, "Einzahlung für / Versement pour / Versamento per", AbstractEsrBand.fixFontRegular, 2.0);
			port.PaintText (topLeft.X+63, topLeft.Y-7, "Einzahlung für / Versement pour / Versamento per", AbstractEsrBand.fixFontRegular, 2.0);
			port.PaintText (topLeft.X+2, topLeft.Y-45, "Konto / Compte / Conto", AbstractEsrBand.fixFontRegular, 2.0);
			port.PaintText (topLeft.X+63, topLeft.Y-45, "Konto / Compte / Conto", AbstractEsrBand.fixFontRegular, 2.0);
			port.PaintText (topLeft.X+2, topLeft.Y-59, "Einbezahlt von / Versé par / Versato da", AbstractEsrBand.fixFontRegular, 2.0);
			port.PaintText (topLeft.X+124, topLeft.Y-45, "Einbezahlt von / Versé par / Versato da", AbstractEsrBand.fixFontRegular, 2.0);

			port.Color = Color.FromBrightness (0);
			port.PaintText (topLeft.X+32, topLeft.Y-90.0, "Die Annahmestelle", AbstractEsrBand.fixFontRegular, 2.0);
			port.PaintText (topLeft.X+32, topLeft.Y-92.5, "L'office de dépôt", AbstractEsrBand.fixFontRegular, 2.0);
			port.PaintText (topLeft.X+32, topLeft.Y-95.0, "L'ufficio d'accettazione", AbstractEsrBand.fixFontRegular, 2.0);

			//	Dessine la zone de découpe.
			port.Color = this.DarkPinkColor (isPreview);
			port.PaintText (topLeft.X+60, topLeft.Y+2, "Vor der Einzahlung abzutrennen / A détacher avant le versement / Da staccare prima del vertamento", AbstractEsrBand.fixFontRegular, 2.0);

			AbstractEsrBand.PaintTriangle (port, new Point (topLeft.X+42+4.5*0, topLeft.Y+2));
			AbstractEsrBand.PaintTriangle (port, new Point (topLeft.X+42+4.5*1, topLeft.Y+2));
			AbstractEsrBand.PaintTriangle (port, new Point (topLeft.X+42+4.5*2, topLeft.Y+2));
			AbstractEsrBand.PaintTriangle (port, new Point (topLeft.X+42+4.5*3, topLeft.Y+2));

			AbstractEsrBand.PaintTriangle (port, new Point (topLeft.X+154+4.5*0, topLeft.Y+2));
			AbstractEsrBand.PaintTriangle (port, new Point (topLeft.X+154+4.5*1, topLeft.Y+2));
			AbstractEsrBand.PaintTriangle (port, new Point (topLeft.X+154+4.5*2, topLeft.Y+2));
			AbstractEsrBand.PaintTriangle (port, new Point (topLeft.X+154+4.5*3, topLeft.Y+2));
		}

		protected virtual void PaintContent(IPaintPort port, Point topLeft)
		{
			//	Dessine tous les textes variables.
		}


		private void PaintSpecimenPattern(IPaintPort port)
		{
			//	Dessine un très gros "SPECIMEN" au travers du BV.
			if (this.PaintSpecimen)
			{
				var font = Font.GetFont("Arial", "Bold");

				var initial = port.Transform;

				port.Transform = port.Transform.MultiplyByPostfix (Transform.CreateRotationDegTransform (20));

				port.Color = Color.FromBrightness (this.PaintEsrSimulator ? 1.0 : 0.95);
				port.PaintText (12, 0, "SPECIMEN", AbstractEsrBand.fixFontBold, 40);

				port.Transform = initial;
			}
		}


		private static void PaintCutLine(IPaintPort port, Point p1, Point p2)
		{
			var path = new DashedPath ();
			path.MoveTo (p1);
			path.LineTo (p2);
			path.AddDash (2.0, 1.2);

			using (Path dashed = path.GenerateDashedPath ())
			{
				port.PaintOutline (dashed);
			}
		}

		private static void PaintDashedCircle(IPaintPort port, Point center, double radius)
		{
			var path = new DashedPath ();
			path.AppendCircle (center, radius);
			path.AddDash (0.2, 0.6);

			using (Path dashed = path.GenerateDashedPath ())
			{
				port.PaintOutline (dashed);
			}
		}

		private static void PaintTriangle(IPaintPort port, Point pos)
		{
			//	Dessine un petit triangle 'v' dont on donne la pointe.
			Path path = new Path ();
			path.MoveTo (pos);
			path.LineTo (new Point (pos.X-2, pos.Y+2));
			path.LineTo (new Point (pos.X+2, pos.Y+2));
			path.Close ();

			port.PaintSurface (path);
		}

		private static string PriceToStringRef(decimal price)
		{
			//	Extrait le codage d'un prix.
			int franc = decimal.ToInt32 (price);
			int cent = decimal.ToInt32 (price * 100) - decimal.ToInt32 (price) * 100;
			return string.Concat (franc.ToString ("D8"), cent.ToString ("D2"));
		}

		protected static void PaintText(IPaintPort port, Rectangle bounds, ContentAlignment alignment, Font font, double fontSize, string text)
		{
			if (string.IsNullOrEmpty (text))
			{
				return;
			}

			var textLayout = new TextLayout ()
			{
				Text = text,
				Alignment = alignment,
				LayoutSize = bounds.Size,
				DefaultFont = font,
				DefaultFontSize = fontSize,
				DefaultColor = port.Color,
			};

			textLayout.Paint (bounds.BottomLeft, port);
		}


		protected string FormatedEsrReferenceNumber
		{
			//	Retourne le numéro au format "96 13070 01000 02173 50356 73892".
			get
			{
				string compact = this.CompactEsrReferenceNumber;

				if (string.IsNullOrEmpty (compact))
				{
					return null;
				}

				if (compact.Length == 27)
				{
					string s1 = compact.Substring (0, 2);
					string s2 = compact.Substring (2, 5);
					string s3 = compact.Substring (7, 5);
					string s4 = compact.Substring (12, 5);
					string s5 = compact.Substring (17, 5);
					string s6 = compact.Substring (22, 5);

					return string.Concat (s1, " ", s2, " ", s3, " ", s4, " ", s5, " ", s6);
				}
				else
				{
					return this.EsrReferenceNumber;
				}
			}
		}

		protected string CompactEsrReferenceNumber
		{
			//	Retourne le numéro au format "961307001000021735035673892".
			get
			{
				if (!string.IsNullOrEmpty (this.EsrReferenceNumber))
				{
					return this.EsrReferenceNumber.Replace (" ", "");
				}

				return null;
			}
		}

		protected string FullEsrReferenceNumber
		{
			//	Retourne le numéro complet au format "0100000106201>100000001668190000043332147+ 010619511>"
			get
			{
				string compact = this.CompactEsrReferenceNumber;

				if (string.IsNullOrEmpty (compact))
				{
					return null;
				}

				string ccp = this.EsrCustomerNumber;

				if (ccp == null)
				{
					ccp = "";
				}
				else
				{
					ccp = ccp.Replace (" ", "");
				}

				if (string.IsNullOrEmpty (ccp))
				{
					ccp = new string ('X', 9);
				}
				else
				{
					string[] ccps = ccp.Split ('-');
					if (ccps.Length == 3)
					{
						// "10-88556-5" devient "100885565":
						ccp = ccps[0] + new string ('0', 6-System.Math.Min (6, ccps[1].Length)) + ccps[1] + ccps[2];
					}
				}

				string fixe = this.CompactEsrReferenceNumber;
				if (string.IsNullOrEmpty (fixe))
				{
					fixe = new string ('X', 27);
				}
				else
				{
					fixe = fixe + new string ('X', System.Math.Max (0, 27-fixe.Length));
				}

				string montant;
				if (this.NotForUse)
				{
					montant = string.Concat ("01", new string ('X', 10+1));
				}
				else
				{
					string p = string.Concat ("01", AbstractEsrBand.PriceToStringRef (this.Price));
					char c = EsrHelper.ComputeControlKey (p);

					montant = string.Concat(p, c);
				}

				return string.Format ("{0}>{1}+ {2}>", montant, fixe, ccp);
			}
		}


		protected static readonly Font	fixFontRegular	= Font.GetFont ("Arial", "Regular");
		protected static readonly Font	fixFontBold		= Font.GetFont ("Arial", "Bold");
		protected static readonly Font	ocrFont			= Font.GetFont ("OCR-B", "Bold");
	}
}
