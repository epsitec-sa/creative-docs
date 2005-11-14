//	Copyright © 2004-2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Widgets.Behaviors
{
	/// <summary>
	/// L'interface IDragBehaviorHost doit être implémentée par toutes les classes
	/// qui désirent bénéficier du comportement DragBehavior.
	/// </summary>
	public interface IDragBehaviorHost
	{
		Drawing.Point	DragLocation		{ get; }
		
		bool OnDragBegin(Drawing.Point cursor);
		void OnDragging(DragEventArgs e);
		void OnDragEnd();
	}
}
