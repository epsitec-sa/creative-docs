//	Copyright � 2004-2008, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Support
{
	/// <summary>
	/// La classe AbstractOplet impl�mente le canevas de base pour un
	/// "oplet" (petite op�ration) utilis�e par OpletQueue pour r�aliser
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
