//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : OK/PA, 01/04/2004

namespace Epsitec.Common.Support
{
	/// <summary>
	/// La classe AbstractOplet implémente le canevas de base pour un
	/// "oplet" (petite opération) utilisée par OpletQueue pour réaliser
	/// le UNDO/REDO.
	/// </summary>
	public abstract class AbstractOplet : IOplet
	{
		protected AbstractOplet()
		{
		}
		
		public abstract IOplet Undo();
		public abstract IOplet Redo();
		
		public virtual void Dispose()
		{
		}
		
		public bool								IsFence
		{
			get
			{
				return false;
			}
		}
	}
}
