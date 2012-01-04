//	Copyright © 2010-2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;

namespace Epsitec.Cresus.Core.Workflows
{
	/// <summary>
	/// The <c>WorkflowState</c> enumeration defines the states in which a workflow can be.
	/// </summary>
	
	[DesignerVisible]
	[System.Flags]
	
	public enum WorkflowState
	{
		None			= 0,

		/// <summary>
		/// Workflow is pending for execution.
		/// </summary>
		Pending			= 1,

		/// <summary>
		/// Workflow is active and executing.
		/// </summary>
		Active			= 2,

		/// <summary>
		/// Workflow has finished executing.
		/// </summary>
		Done			= 3,

		/// <summary>
		/// Workflow was cancelled.
		/// </summary>
		Cancelled		= 4,

		/// <summary>
		/// Workflow timed out.
		/// </summary>
		TimedOut		= 5,

		/// <summary>
		/// Workflow has finished executing and it is again pending for execution.
		/// </summary>
		RestartPending	= 6,


		/// <summary>
		/// Flag: workflow is restricted to a user specified by a user code.
		/// </summary>
		IsRestricted	= 0x000100,

		/// <summary>
		/// Flag: workflow is currently frozen.
		/// </summary>
		IsFrozen		= 0x000200,
		
		
		ValueMask		= 0x0000ff,
		FlagsMask		= ~ValueMask,
	}
}
