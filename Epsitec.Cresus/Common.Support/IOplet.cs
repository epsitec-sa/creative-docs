//	Copyright � 2004-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
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
