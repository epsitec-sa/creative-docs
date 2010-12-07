//	Copyright � 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.WorkflowDesigner.Objects
{
	public enum DraggingMode
	{
		None,
		MoveObject,
		ChangeWidth,
		MoveLinkDst,
		MoveLinkCustomize,
		MoveCommentAttach,
		MoveInfoLine,
	}
}
