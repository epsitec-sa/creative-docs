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
		public PdfExportProfile(bool landscape, bool evenOddGrey)
		{
			this.Landscape   = landscape;
			this.EvenOddGrey = evenOddGrey;
		}

		public static PdfExportProfile Default = new PdfExportProfile (true, false);

		public readonly bool					Landscape;
		public readonly bool					EvenOddGrey;
	}
}