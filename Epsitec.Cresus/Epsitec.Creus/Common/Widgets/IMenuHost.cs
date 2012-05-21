//	Copyright © 2005-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// L'interface IMenuHost permet à un widget de spécifier où un sous-menu
	/// doit s'afficher à l'écran.
	/// </summary>
	public interface IMenuHost
	{
		void GetMenuDisposition(Widget parentWidget, ref Drawing.Size size, out Drawing.Point location, out Animation animation);
	}
}
