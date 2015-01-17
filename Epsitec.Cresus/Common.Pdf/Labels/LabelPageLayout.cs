//	Copyright © 2004-2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;
using Epsitec.Common.Pdf.Common;

namespace Epsitec.Common.Pdf.Labels
{
	public class LabelPageLayout : CommonSetup
	{
		// PageMargins.Left    LabelGap.Width      PageMargins.Right
		//     |<--->|            >|--|<                |<--->| v
		//     +----------------------------------------------+--
		//     | Page                                   .     | |  PageMargins.Top
		//     |     +-------------+  +-------------+   .     |--
		//     |     | Label       |  |             |   .     | |
		//     |     |  +-------+  |  |  +-------+  |   .     | |
		//     |     |  | Text  |  |  |  |       |  |   .     | |  LabelSize.Height
		//     |     |  |       |  |  |  |       |  |   .     | |
		//     |     |  +-------+  |  |  +-------+  |   .     | |
		//     |     |             |  |             |   .     | |
		//     |     +-------------+  +-------------+   .     |--
		//     |                                        .     | |  LabelGap.Height
		//     |     +-------------+  +-------------+   .     |--
		//     |     |             |  |             |   .     | |  LabelMargins.Top  
		//     |     |  +-------+  |  |  +-------+  |   .     |--
		//     |     |  |       |  |  |  |       |  |   .     |
		//     |     |  |       |  |  |  |       |  |   .     |
		//     |     |  +-------+  |  |  +-------+  |   .     |--
		//     |     |             |  |             |   .     | |  LabelMargins.Bottom
		//     |     +-------------+  +-------------+   .     |--
		//     |                                        .     | |  reste
		//     |  . . . . . . . . . . . . . . . . . . . . . . |--
		//     |                                        .     | |  PageMargins.Bottom
		//     +----------------------------------------------+--
		//           |<----------->|            >|--|<->|  reste
		//           LabelSize.Width      LabelMargins.Right

		public LabelPageLayout()
			: base ()
		{
			//	Rappel: L'unité est le dixième de millimètre.
			this.PageMargins  = new Margins (100.0);
			this.LabelSize    = new Size (620.0, 400.0);
			this.LabelMargins = new Margins (50.0);
			this.LabelGap     = new Size (10.0, 10.0);
		}

		
		public Size								LabelSize
		{
			//	Dimensions d'une étiquette.
			set;
			get;
		}

		public Margins							LabelMargins
		{
			//	Marges à l'intérieur d'une étiquette, pour le texte.
			set;
			get;
		}

		public Size								LabelGap
		{
			//	Espace vide entre les étiquettes.
			set;
			get;
		}

		public bool								ShouldPaintFrame
		{
			//	Indique s'il faut imprimer un cadre rectangulaire gris pour délimiter chaque étiquette.
			//	Utile lors de la phase de mise en place.
			set;
			get;
		}
	}
}
