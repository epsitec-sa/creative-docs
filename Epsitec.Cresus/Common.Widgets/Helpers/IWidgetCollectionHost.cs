namespace Epsitec.Common.Widgets.Helpers
{
	public interface IWidgetCollectionHost
	{
		void NotifyInsertion(Widget widget);
		void NotifyRemoval(Widget widget);
	}
}
