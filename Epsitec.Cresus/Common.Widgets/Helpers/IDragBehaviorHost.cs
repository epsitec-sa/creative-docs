namespace Epsitec.Common.Widgets.Helpers
{
	/// <summary>
	/// Summary description for IDragBehaviorHost.
	/// </summary>
	public interface IDragBehaviorHost
	{
		Drawing.Point	DragLocation		{ get; }
		
		void OnDragBegin();
		void OnDragging(DragEventArgs e);
		void OnDragEnd();
	}
}
