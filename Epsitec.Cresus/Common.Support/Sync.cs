//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Support
{
	/// <summary>
	/// La classe Sync permet de simplifier les attentes sur de multiples
	/// événements, tout en traitant aussi l'événement global AbortEvent.
	/// </summary>
	public sealed class Sync
	{
		public static int Wait(params System.Threading.WaitHandle[] wait)
		{
			int n = wait.Length;
			
			System.Threading.WaitHandle[] handles = new System.Threading.WaitHandle[n + 1];
			
			wait.CopyTo (handles, 0);
			
			handles[n] = Common.Support.Globals.AbortEvent;
			
			int handle_index = System.Threading.WaitHandle.WaitAny (handles);
			
			//	Gère le cas particulier décrit dans la documentation où l'index peut être
			//	incorrect dans certains cas :
			
			if (handle_index >= 128)
			{
				handle_index -= 128;
			}
			
			//	Tout événement autre que celui lié à la queue provoque l'interruption
			//	du processus :
			
			if (handle_index == n)
			{
				throw new System.Threading.ThreadInterruptedException ("Globals.AbortEvent set");
			}
			
			return handle_index;
		}
	}
}
