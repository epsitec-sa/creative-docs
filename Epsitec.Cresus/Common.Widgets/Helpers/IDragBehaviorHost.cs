namespace Epsitec.Common.Widgets.Helpers
{
	/// <summary>
	/// L'interface IDragBehaviorHost doit être implémentée par toutes les classes
	/// qui désirent bénéficier du comportement DragBehavior.
	/// </summary>
	public interface IDragBehaviorHost
	{
		Drawing.Point	DragLocation		{ get; }
		
		void OnDragBegin(Drawing.Point cursor);
		void OnDragging(DragEventArgs e);
		void OnDragEnd();
	}
}
