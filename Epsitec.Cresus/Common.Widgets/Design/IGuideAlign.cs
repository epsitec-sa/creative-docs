namespace Epsitec.Common.Widgets.Design
{
	public interface IGuideAlign
	{
		Drawing.Margins GetInnerMargins(System.Type type);
		Drawing.Margins GetAlignMargins(System.Type type_a, System.Type type_b);
	}
}
