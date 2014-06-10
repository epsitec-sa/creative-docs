//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Epsitec.Cresus.Assets.Server.Export
{
	/// <summary>
	/// Paramètres pour PdfExport.
	/// </summary>
	public class PdfExportProfile : AbstractExportProfile
	{
		public PdfExportProfile(bool landscape, bool evenOddGrey, int leftMargin, int rightMargin, int topMargin, int bottomMargin)
		{
			this.Landscape    = landscape;
			this.EvenOddGrey  = evenOddGrey;
			this.LeftMargin   = leftMargin;
			this.RightMargin  = rightMargin;
			this.TopMargin    = topMargin;
			this.BottomMargin = bottomMargin;
		}

		public static PdfExportProfile Default = new PdfExportProfile (true, false, 10, 10, 10, 10);

		public readonly bool					Landscape;
		public readonly bool					EvenOddGrey;
		public readonly int						LeftMargin;
		public readonly int						RightMargin;
		public readonly int						TopMargin;
		public readonly int						BottomMargin;
	}
}