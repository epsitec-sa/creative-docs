//	Copyright � 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Support
{
	/// <summary>
	/// La classe Sync permet de simplifier les attentes sur de multiples
	/// �v�nements, tout en traitant aussi l'�v�nement global AbortEvent.
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
			
			//	G�re le cas particulier d�crit dans la documentation o� l'index peut �tre
			//	incorrect dans certains cas :
			
			if (handle_index >= 128)
			{
				handle_index -= 128;
			}
			
			//	Tout �v�nement autre que celui li� � la queue provoque l'interruption
			//	du processus :
			
			if (handle_index == n)
			{
				throw new System.Threading.ThreadInterruptedException ("Globals.AbortEvent set");
			}
			
			return handle_index;
		}
	}
}
