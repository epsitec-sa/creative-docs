//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Support
{
	/// <summary>
	/// L'interface IOplet définit les méthodes pour implémenter le UNDO/REDO
	/// avec une granulosité plus fine que l'action.
	/// </summary>
	public interface IOplet
	{
		IOplet Undo();
		IOplet Redo();
		
		void Dispose();
		
		bool	IsFence		{ get; }
	}
}
