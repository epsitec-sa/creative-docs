//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;

namespace Epsitec.Cresus.Core.Widgets.Tiles
{
	/// <summary>
	/// The <c>TileVisibilityMode</c> enumeration defines how tiles should be displayed.
	/// </summary>
	[DesignerVisible]
	[System.Flags]
	public enum TileVisibilityMode
	{
		Undefined				= 0,

		Visible					= 0x01,
		Hidden					= 0x02,
		NeverVisible			= 0x04,
	}
}