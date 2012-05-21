//	Copyright © 2005-2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;

namespace Epsitec.Common.Widgets
{

	[System.Flags]
	[DesignerVisible]
	public enum FrameEdges : uint
	{
		[Hidden]
		None	= 0,
		
		Left	= 0x00000001,
		Right	= 0x00000002,
		Top		= 0x00000004,
		Bottom	= 0x00000008,
		
		[Hidden]
		All		= 0x0000000F,
	}
}
