//	Copyright © 2005-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Widgets.Helpers
{
	/// <summary>
	/// L'interface IClientInfo renseigne sur les dimensions internes (dites "client")
	/// d'un widget.
	/// </summary>
	public interface IClientInfo
	{
		Drawing.Rectangle Bounds
		{
			get;
		}
		Drawing.Size Size
		{
			get;
		}
	}
}
