//	Copyright � 2005-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Widgets.Helpers
{
	/// <summary>
	/// L'interface IToolTipHost permet � un widget de d�finir plusieurs r�gions
	/// sensibles pour les tool-tips.
	/// </summary>
	public interface IToolTipHost
	{
		object GetToolTipCaption(Drawing.Point pos);
	}
}
