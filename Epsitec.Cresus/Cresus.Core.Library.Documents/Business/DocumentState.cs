//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;

namespace Epsitec.Cresus.Core.Business
{
	[DesignerVisible]
	[System.Flags]

	public enum DocumentState
	{
		None					= 0,

		Draft					= 1,
		Valid					= 2,


		ValueMask				= 0x000000ff,

		IsReferenced			= 0x00000100,
		IsHidden				= 0x00000200,
		IsFrozen				= 0x00000400,
	}
}
