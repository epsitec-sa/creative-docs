//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : en chantier/PA

using Epsitec.Common.Widgets;

namespace Epsitec.Common.Designer.Behaviors
{
	/// <summary>
	/// L'interface IGuideAlignHint est utilisée par la classe SmartGuideBehavior pour
	/// déterminer l'alignement des widgets.
	/// </summary>
	public interface IGuideAlignHint
	{
		Drawing.Margins GetInnerMargins(Widget widget);
		Drawing.Margins GetAlignMargins(Widget widget_a, Widget widget_b);
	}
}
