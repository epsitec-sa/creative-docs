//	Copyright � 2005-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// L'interface IMenuHost permet � un widget de sp�cifier o� un sous-menu
	/// doit s'afficher � l'�cran.
	/// </summary>
	public interface IMenuHost
	{
		void GetMenuDisposition(Widget parent_widget, ref Drawing.Size size, out Drawing.Point location, out Animation animation);
	}
}
