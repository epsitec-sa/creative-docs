namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// L'interface IPaintFilter...
	/// </summary>
	public interface IPaintFilter
	{
		bool IsWidgetFullyDiscarded(Widget widget);
		bool IsWidgetPaintDiscarded(Widget widget);
		void EnableChildren();
		void DisableChildren();
	}
}
