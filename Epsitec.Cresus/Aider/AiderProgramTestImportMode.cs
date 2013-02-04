//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Aider
{
	[System.Flags]
	internal enum AiderProgramTestImportMode
	{
		Default	= 0x0000,
		EchOnly	= 0x0001,
		Subset	= 0x0002,
	}
}
