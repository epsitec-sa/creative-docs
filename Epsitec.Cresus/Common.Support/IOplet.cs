//	Copyright © 2004-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
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
