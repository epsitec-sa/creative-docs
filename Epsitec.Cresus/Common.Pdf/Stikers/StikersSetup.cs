//	Copyright © 2004-2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System;
using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Drawing;
using Epsitec.Common.Pdf.Common;

namespace Epsitec.Common.Pdf.Stikers
{
	public class StikersSetup : CommonSetup
	{
		// PageMargins.Left  StikerGap.Width       PageMargins.Right
		//     |<--->|            >|--|<                |<--->| v
		//     +----------------------------------------------+--
		//     | Page                                   .     | |  PageMargins.Top
		//     |     +-------------+  +-------------+   .     |--
		//     |     | Stiker      |  |             |   .     | |
		//     |     |  +-------+  |  |  +-------+  |   .     | |
		//     |     |  | Text  |  |  |  |       |  |   .     | |  StikerSize.Height
		//     |     |  |       |  |  |  |       |  |   .     | |
		//     |     |  +-------+  |  |  +-------+  |   .     | |
		//     |     |             |  |             |   .     | |
		//     |     +-------------+  +-------------+   .     |--
		//     |                                        .     | |  StikerGap.Height
		//     |     +-------------+  +-------------+   .     |--
		//     |     |             |  |             |   .     | |  StikerMargins.Top  
		//     |     |  +-------+  |  |  +-------+  |   .     |--
		//     |     |  |       |  |  |  |       |  |   .     |
		//     |     |  |       |  |  |  |       |  |   .     |
		//     |     |  +-------+  |  |  +-------+  |   .     |--
		//     |     |             |  |             |   .     | |  StikerMargins.Bottom
		//     |     +-------------+  +-------------+   .     |--
		//     |                                        .     | |  reste
		//     |  . . . . . . . . . . . . . . . . . . . . . . |--
		//     |                                        .     | |  PageMargins.Bottom
		//     +----------------------------------------------+--
		//           |<----------->|            >|--|<->|  reste
		//           StikerSize.Width     StikerMargins.Right

		public StikersSetup()
			: base ()
		{
			//	Rappel: L'unité est le dixième de millimètre.
			this.PageMargins   = new Margins (100.0);
			this.StikerSize    = new Size (620.0, 400.0);
			this.StikerMargins = new Margins (50.0);
			this.StikerGap     = new Size (10.0, 10.0);
			this.PaintFrame    = false;
		}

		public Size StikerSize
		{
			//	Dimensions d'une étiquette.
			set;
			get;
		}

		public Margins StikerMargins
		{
			//	Marges à l'intérieur d'une étiquette, pour le texte.
			set;
			get;
		}

		public Size StikerGap
		{
			//	Espace vide entre les étiquettes.
			set;
			get;
		}

		public bool PaintFrame
		{
			//	Indique s'il faut imprimer un cadre rectangulaire gris pour délimiter chaque étiquette.
			//	Utile lors de la phase de mise en place.
			set;
			get;
		}
	}
}
