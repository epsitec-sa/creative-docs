//	Copyright © 2004-2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Cresus.Requests
{
	
	
	/// <summary>
	/// The <c>OrchestratorState</c> enumeration describes the possible states of
	/// the <see cref="Orchestrator"/> class.
	/// </summary>
	public enum OrchestratorState
	{
		
		
		/// <summary>
		/// Orchestrator ready, waiting for something to do.
		/// </summary>
		Ready				= 0,
		
		
		/// <summary>
		/// Orchestrator ready, waiting for a conflict to be resolved.
		/// </summary>
		Conflicting			= 1,
		
		
		/// <summary>
		/// Orchestrator processing requestes.
		/// </summary>
		Processing			= 2,
	
	
	}


}
