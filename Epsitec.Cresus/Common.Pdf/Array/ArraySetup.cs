//	Copyright © 2004-2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Epsitec.Common.Drawing;
using Epsitec.Common.Types;

namespace Epsitec.Common.Pdf.Array
{
	public class ArraySetup
	{
		public ArraySetup()
		{
			//	Rappel: L'unité est le dixième de millimètre.
			this.PageMargins          = new Margins (200.0, 100.0, 100.0, 100.0);
			this.CellMargins          = new Margins (10.0);
			this.FontFace             = "Arial";
			this.FontStyle            = "Regular";
			this.FontSize             = 30.0;
			this.BorderThickness      = 1.0;
			this.HeaderMargins        = new Margins (0.0, 0.0, 0.0, 50.0);
			this.FooterMargins        = new Margins (0.0, 0.0, 50.0, 0.0);
			this.LabelBackgroundColor = Color.FromBrightness (0.9);
			this.EvenBackgroundColor  = Color.Empty;
			this.OddBackgroundColor   = Color.Empty;
		}

		public Margins PageMargins
		{
			//	Marges globales de la page.
			set;
			get;
		}

		public Margins CellMargins
		{
			//	Marges dans une cellule
			set;
			get;
		}

		public string FontFace
		{
			//	Nom de la police de caractères.
			set;
			get;
		}

		public string FontStyle
		{
			//	Style de la police de caractères, généralement "Regular".
			set;
			get;
		}

		public double FontSize
		{
			//	Taille de la police de caractères (en dixièmes de millimètres).
			set;
			get;
		}

		public FormattedText HeaderText
		{
			//	Texte imprimé au début de la première page.
			set;
			get;
		}

		public Margins HeaderMargins
		{
			set;
			get;
		}

		public FormattedText FooterText
		{
			//	Texte imprimé à la fin de la dernière page.
			set;
			get;
		}

		public Margins FooterMargins
		{
			set;
			get;
		}

		public double BorderThickness
		{
			//	Epaisseur des traits d'encadrement du tableau.
			//	Mettre 0 pour supprimer ces traits.
			set;
			get;
		}

		public Color LabelBackgroundColor
		{
			//	Couleur de fond pour la ligne des labels du tableau.
			set;
			get;
		}

		public Color EvenBackgroundColor
		{
			//	Couleur de fond pour les lignes paires du tableau.
			set;
			get;
		}

		public Color OddBackgroundColor
		{
			//	Couleur de fond pour les lignes impaires du tableau.
			set;
			get;
		}
	}
}
