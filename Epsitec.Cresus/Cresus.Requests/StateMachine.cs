//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Cresus.Requests
{
	/// <summary>
	/// La classe StateMachine permet de vérifier que les transitions d'état sont
	/// conformes à ce qui est prévu par la machine d'état.
	/// </summary>
	public class StateMachine
	{
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
