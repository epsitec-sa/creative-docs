//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;

namespace Epsitec.Cresus.Core.Widgets.Tiles
{
	/// <summary>
	/// THe <c>TileEditionMode</c> enumeration defines how tiles should be edited.
	/// </summary>
	[DesignerVisible]
	[System.Flags]
	public enum TileEditionMode
	{
		Undefined				= 0,

		ReadWrite				= 0x01,
		ReadOnly				= 0x02,
	}
}
