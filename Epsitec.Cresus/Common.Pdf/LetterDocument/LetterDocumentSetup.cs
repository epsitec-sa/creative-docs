//	Copyright © 2012-2014, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Samuel LOUP

using Epsitec.Common.Drawing;
using Epsitec.Common.Pdf.TextDocument;
using Epsitec.Common.Types;

namespace Epsitec.Common.Pdf.LetterDocument
{
	public class LetterDocumentSetup : TextDocumentSetup
	{
		public LetterDocumentSetup()
		{
			//	Rappel: L'unité est le dixième de millimètre.
			this.PageMargins = new Margins (300.0, 250.0, 0, 0);
		}

		public Margins							SenderAddressMargins
		{
			set;
			get;
		}

		public Margins							RecipientAddressMargins
		{
			set;
			get;
		}
	}
}
