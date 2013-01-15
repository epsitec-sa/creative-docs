//	Copyright © 2004-2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Epsitec.Common.Drawing;
using Epsitec.Common.Types;
using Epsitec.Common.Pdf.Common;

namespace Epsitec.Common.Pdf.TextDocument
{
	public class TextDocumentSetup : CommonSetup
	{
		public TextDocumentSetup()
			: base ()
		{
			//	Rappel: L'unité est le dixième de millimètre.
			this.PageMargins          = new Margins (200.0);
			this.HeaderMargins        = new Margins (0.0, 0.0, 0.0, 50.0);
			this.FooterMargins        = new Margins (0.0, 0.0, 50.0, 0.0);
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
	}
}
