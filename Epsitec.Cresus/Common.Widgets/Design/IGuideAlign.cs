namespace Epsitec.Common.Widgets.Design
{
	/// <summary>
	/// L'interface IGuideAlign est utilisée par la classe SmartGuide pour
	/// déterminer l'alignement des widgets.
	/// </summary>
	public interface IGuideAlign
	{
		Drawing.Margins GetInnerMargins(Widget widget);
		Drawing.Margins GetAlignMargins(Widget widget_a, Widget widget_b);
	}
}
