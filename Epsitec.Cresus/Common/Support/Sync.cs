//	Copyright © 2004-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
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
			//	Retourne l'index de l'événement qui a reçu le signal. En cas de timeout,
			//	retourne -1; en cas d'avortement, retourne 'n'.
			
			int n = wait.Length;
			
			System.Threading.WaitHandle[] handles = new System.Threading.WaitHandle[n + 1];
			
			wait.CopyTo (handles, 0);
			
			handles[n] = Common.Support.Globals.AbortEvent;
			
			int handleIndex = System.Threading.WaitHandle.WaitAny (handles);
			
			//	Gère le cas particulier décrit dans la documentation où l'index peut être
			//	incorrect dans certains cas :
			
			if (handleIndex >= 128)
			{
				handleIndex -= 128;
			}
			
			return handleIndex;
		}
	}
}
