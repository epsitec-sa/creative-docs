//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;

namespace Epsitec.Cresus.Core.Workflows
{
	[DesignerVisible]
	[System.Flags]
	public enum WorkflowNodeAttributes
	{
		Auto    = 0x00000001,
		Public  = 0x00000002,
		Foreign = 0x00000004,
	}
}
