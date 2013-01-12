//	Copyright © 2004-2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Epsitec.Common.Drawing;
using Epsitec.Common.Types;
using Epsitec.Common.Pdf.Common;

namespace Epsitec.Common.Pdf.Array
{
	public class ArraySetup : CommonSetup
	{
		public ArraySetup() : base()
		{
			//	Rappel: L'unité est le dixième de millimètre.
			this.PageMargins          = new Margins (200.0, 100.0, 100.0, 100.0);
			this.CellMargins          = new Margins (10.0);
			this.BorderThickness      = 1.0;
			this.HeaderMargins        = new Margins (0.0, 0.0, 0.0, 50.0);
			this.FooterMargins        = new Margins (0.0, 0.0, 50.0, 0.0);
			this.LabelBackgroundColor = Color.FromBrightness (0.9);  // gris très clair
			this.EvenBackgroundColor  = Color.Empty;
			this.OddBackgroundColor   = Color.Empty;
		}

		public Margins CellMargins
		{
			//	Marges dans une cellule
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
			//	Marges autour du texte de header.
			//	On utilise généralement uniquement Margins.Bottom.
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
			//	Marges autour du texte de footer.
			//	On utilise généralement uniquement Margins.Top.
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
