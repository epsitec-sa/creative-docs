//	Copyright © 2005-2012, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Common.Widgets.Helpers
{
	/// <summary>
	/// L'interface <c>IClientInfo</c> renseigne sur les dimensions internes (dites "client")
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

		double Height
		{
			get;
		}

		double Width
		{
			get;
		}
	}
}
