namespace Epsitec.Common.Widgets.Helpers
{
	/// <summary>
	/// L'interface IDragBehaviorHost doit �tre impl�ment�e par toutes les classes
	/// qui d�sirent b�n�ficier du comportement DragBehavior.
	/// </summary>
	public interface IDragBehaviorHost
	{
		Drawing.Point	DragLocation		{ get; }
		
		void OnDragBegin(Drawing.Point cursor);
		void OnDragging(DragEventArgs e);
		void OnDragEnd();
	}
}
