//	Copyright � 2004-2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Support
{
	/// <summary>
	/// L'interface IOplet d�finit les m�thodes pour impl�menter le UNDO/REDO
	/// avec une granulosit� plus fine que l'action.
	/// </summary>
	public interface IOplet : System.IDisposable
	{
		IOplet Undo();
		IOplet Redo();
		
		bool	IsFence		{ get; }
//		void Dispose();
		
	}
}
