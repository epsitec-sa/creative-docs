//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Drawing;

namespace Epsitec.Common.Pdf.Labels
{
	/// <summary>
	/// The <c>LabelRendererSheetA5</c> class renders an A5-sheet to be used as
	/// a label, with the P.P. franking information, when requested.
	/// </summary>
	public class LabelRendererSheetA5 : LabelRendererSheetAx<LabelRendererSheetA5>
	{
		public LabelRendererSheetA5()
			: base (Point.Zero)
		{
		}
	}
}