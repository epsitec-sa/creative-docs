//	Copyright © 2004-2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System;
using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Drawing;
using Epsitec.Common.Pdf.Common;

namespace Epsitec.Common.Pdf.Stickers
{
	public class StickersSetup : CommonSetup
	{
		// PageMargins.Left  StickerGap.Width      PageMargins.Right
		//     |<--->|            >|--|<                |<--->| v
		//     +----------------------------------------------+--
		//     | Page                                   .     | |  PageMargins.Top
		//     |     +-------------+  +-------------+   .     |--
		//     |     | Sticker     |  |             |   .     | |
		//     |     |  +-------+  |  |  +-------+  |   .     | |
		//     |     |  | Text  |  |  |  |       |  |   .     | |  StickerSize.Height
		//     |     |  |       |  |  |  |       |  |   .     | |
		//     |     |  +-------+  |  |  +-------+  |   .     | |
		//     |     |             |  |             |   .     | |
		//     |     +-------------+  +-------------+   .     |--
		//     |                                        .     | |  StickerGap.Height
		//     |     +-------------+  +-------------+   .     |--
		//     |     |             |  |             |   .     | |  StickerMargins.Top  
		//     |     |  +-------+  |  |  +-------+  |   .     |--
		//     |     |  |       |  |  |  |       |  |   .     |
		//     |     |  |       |  |  |  |       |  |   .     |
		//     |     |  +-------+  |  |  +-------+  |   .     |--
		//     |     |             |  |             |   .     | |  StickerMargins.Bottom
		//     |     +-------------+  +-------------+   .     |--
		//     |                                        .     | |  reste
		//     |  . . . . . . . . . . . . . . . . . . . . . . |--
		//     |                                        .     | |  PageMargins.Bottom
		//     +----------------------------------------------+--
		//           |<----------->|            >|--|<->|  reste
		//           StickerSize.Width    StickerMargins.Right

		public StickersSetup()
			: base ()
		{
			//	Rappel: L'unité est le dixième de millimètre.
			this.PageMargins   = new Margins (100.0);
			this.StickerSize    = new Size (620.0, 400.0);
			this.StickerMargins = new Margins (50.0);
			this.StickerGap     = new Size (10.0, 10.0);
			this.PaintFrame    = false;
		}

		public Size StickerSize
		{
			//	Dimensions d'une étiquette.
			set;
			get;
		}

		public Margins StickerMargins
		{
			//	Marges à l'intérieur d'une étiquette, pour le texte.
			set;
			get;
		}

		public Size StickerGap
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
