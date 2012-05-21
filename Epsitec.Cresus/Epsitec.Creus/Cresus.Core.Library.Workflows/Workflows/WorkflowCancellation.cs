//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Cresus.Core.Workflows
{
	/// <summary>
	/// The <c>WorkflowCancellation</c> enumeration specifies what should be canceled
	/// in the current workflow.
	/// </summary>
	public enum WorkflowCancellation
	{
		Action			= 0,
		Transition		= 1,
		Routine			= 2,
		Thread			= 3,
		Workflow		= 4,
	}
}
