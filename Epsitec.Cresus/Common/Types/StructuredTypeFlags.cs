//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Common.Types
{
	[DesignerVisible]
	[System.Flags]
	public enum StructuredTypeFlags
	{
		None = 0,

		GenerateSchema		= 0x00000001,
		GenerateRepository	= 0x00000002,
	}
}
