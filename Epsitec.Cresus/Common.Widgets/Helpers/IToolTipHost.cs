//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Widgets.Helpers
{
	/// <summary>
	/// L'interface IToolTipHost permet à un widget de définir plusieurs régions
	/// sensibles pour les tool-tips.
	/// </summary>
	public interface IToolTipHost
	{
		string GetToolTipCaption(Drawing.Point pos);
	}
}
