//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Drawing;

namespace Epsitec.Common.Pdf.Labels
{
	/// <summary>
	/// The <c>LabelRendererSheetA4</c> class renders an A4-sheet to be used as
	/// a label, with the P.P. franking information, when requested.
	/// </summary>
	public class LabelRendererSheetA4 : LabelRendererSheetAx<LabelRendererSheetA4>
	{
		public LabelRendererSheetA4()
			: base (new Point (0, 1480))
		{
		}
	}
}

