namespace Epsitec.Common.Widgets.Design
{
	/// <summary>
	/// L'interface IGuideAlign est utilis�e par la classe SmartGuide pour
	/// d�terminer l'alignement des widgets.
	/// </summary>
	public interface IGuideAlign
	{
		Drawing.Margins GetInnerMargins(Widget widget);
		Drawing.Margins GetAlignMargins(Widget widget_a, Widget widget_b);
	}
}
