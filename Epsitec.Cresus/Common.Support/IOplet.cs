//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : OK/PA, 01/04/2004

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
