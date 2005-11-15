//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// L'interface IMenuHost permet à un widget de spécifier où un sous-menu
	/// doit s'afficher à l'écran.
	/// </summary>
	public interface IMenuHost
	{
		void GetMenuDisposition(Widget parent_widget, Drawing.Size size, out Drawing.Point location, out Animation animation);
	}
}
