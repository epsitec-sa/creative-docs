//	Copyright � 2004-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Cresus.Requests
{
	/// <summary>
	/// La classe StateMachine permet de v�rifier que les transitions d'�tat sont
	/// conformes � ce qui est pr�vu par la machine d'�tat.
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
