//	Copyright � 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : OK/PA, 01/04/2004

namespace Epsitec.Common.Support
{
	/// <summary>
	/// L'interface IOplet d�finit les m�thodes pour impl�menter le UNDO/REDO
	/// avec une granulosit� plus fine que l'action.
	/// </summary>
	public interface IOplet
	{
		IOplet Undo();
		IOplet Redo();
		
		void Dispose();
		
		bool	IsFence		{ get; }
	}
}
