//	Copyright © 2004-2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Cresus.Requests
{
	
	
	/// <summary>
	/// The <c>StateMachine</c> class checks that the state transitions for a
	/// given state machine are valid.
	/// </summary>
	internal static class StateMachine
	{
		
		
		/// <summary>
		/// Checks that the transition from one state to another is allowed.
		/// </summary>
		/// <param name="before">The state before transition.</param>
		/// <param name="after">The state after transition.</param>
		/// <returns><c>true</c> if the transition is allowed; otherwise, <c>false</c>.</returns>
		public static bool Check(ExecutionState before, ExecutionState after)
		{
			if (before == after)
			{
				return true;
			}
			
			switch (before)
			{
				case ExecutionState.Pending:			 return (after == ExecutionState.ExecutedByClient) || (after == ExecutionState.Conflicting);
				case ExecutionState.ConflictingOnServer: return (after == ExecutionState.Conflicting);
				case ExecutionState.Conflicting:		 return (after == ExecutionState.ConflictResolved);
				case ExecutionState.ConflictResolved:	 return (after == ExecutionState.ExecutedByClient) || (after == ExecutionState.Conflicting);
				case ExecutionState.ExecutedByClient:	 return (after == ExecutionState.SentToServer);
				case ExecutionState.SentToServer:		 return (after == ExecutionState.ExecutedByServer) || (after == ExecutionState.ConflictingOnServer);
				case ExecutionState.ExecutedByServer:	 break;
			}
			
			return false;
		}


	}


}
