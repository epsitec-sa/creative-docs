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
			this.PageMargins = new Margins (200.0, 100.0, 100.0, 100.0);
			this.CellMargins = new Margins (20.0);
			this.FontFace    = "Arial";
			this.FontStyle   = "Regular";
			this.FontSize    = 30.0;
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

		public FormattedText Header
		{
			set;
			get;
		}

		public FormattedText Footer
		{
			set;
			get;
		}
	}
}
