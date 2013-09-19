//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Cresus.Assets.Core.Business;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Assets.App.Converters
{
	public sealed class TimelineCellConverter
	{
		public Widgets.TimelineCell Convert(TimelineEventCell item)
		{
			return new Widgets.TimelineCell (item.Date, Widgets.TimelineCellGlyph.FilledCircle);
		}
	}
}
