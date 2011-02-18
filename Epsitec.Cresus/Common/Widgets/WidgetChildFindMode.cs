//	Copyright © 2004-2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Common.Widgets
{
	[System.Flags]
	public enum WidgetChildFindMode
	{
		All					= 0,
		SkipHidden			= 0x00000001,
		SkipDisabled		= 0x00000002,
		SkipTransparent		= 0x00000004,
		SkipEmbedded		= 0x00000008,
		SkipNonContainer	= 0x00000010,
		SkipMask			= 0x000000ff,

		Deep				= 0x00010000
	}
}
