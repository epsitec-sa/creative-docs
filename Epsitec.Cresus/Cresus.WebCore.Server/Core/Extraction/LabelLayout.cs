//	Copyright © 2013-2015, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;

namespace Epsitec.Cresus.WebCore.Server.Core.Extraction
{
	/// <summary>
	/// This enumeration holds the different layout that are available to print labels, using the
	/// LabelWriter class.
	/// </summary>
	[DesignerVisible]
	public enum LabelLayout
	{
		Avery_3474,		//	70 x 37 (no top/bottom margin)
		Avery_3475,		//	70 x 36 (small top/bottom margin)

		Sheet_A5_Simple,
		Sheet_A5_SimplePP,
		Sheet_A5_SimplePPPriority,

		Sheet_A4_Simple,
		Sheet_A4_SimplePP,
		Sheet_A4_SimplePPPriority,
	}
}
