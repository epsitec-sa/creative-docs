//	Copyright � 2004-2007, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Widgets.Behaviors
{
	/// <summary>
	/// L'interface IDragBehaviorHost doit �tre impl�ment�e par toutes les classes
	/// qui d�sirent b�n�ficier du comportement DragBehavior.
	/// </summary>
	public interface IDragBehaviorHost
	{
		Drawing.Point	DragLocation		{ get; }
		
		bool OnDragBegin(Drawing.Point cursor);
		void OnDragging(DragEventArgs e);
		void OnDragEnd();
	}
}
