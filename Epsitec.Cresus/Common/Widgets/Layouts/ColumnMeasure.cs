//	Copyright © 2006-2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Widgets.Layouts
{
	public sealed class ColumnMeasure : LayoutMeasure
	{
		public ColumnMeasure()
			: this (0)
		{
		}

		public ColumnMeasure(int passId)
			: base (passId)
		{
		}
	}
}