namespace Epsitec.Common.Widgets.Design
{
	public interface IGuideAlign
	{
		Drawing.Margins GetInnerMargins(Widget widget);
		Drawing.Margins GetAlignMargins(Widget widget_a, Widget widget_b);
	}
}
