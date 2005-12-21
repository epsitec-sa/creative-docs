//	Copyright © 2004-2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Support
{
	/// <summary>
	/// L'interface IOplet définit les méthodes pour implémenter le UNDO/REDO
	/// avec une granulosité plus fine que l'action.
	/// </summary>
	public interface IOplet : System.IDisposable
	{
		IOplet Undo();
		IOplet Redo();
		
		bool	IsFence		{ get; }
//		void Dispose();
		
	}
}
