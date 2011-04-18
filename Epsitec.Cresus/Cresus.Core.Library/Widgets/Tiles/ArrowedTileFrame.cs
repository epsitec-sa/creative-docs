//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Widgets;

namespace Epsitec.Cresus.Core.Widgets.Tiles
{
	/// <summary>
	/// The <c>ArrowedTileFrame</c> class is used as a container for the
	/// <see cref="Epsitec.Cresus.Core.Controllers.ListController&lt;T&gt;"/>.
	/// </summary>
	public class ArrowedTileFrame : ArrowedTile
	{
		public ArrowedTileFrame(Direction arrowDirection)
			: base (arrowDirection)
		{
		}
	}
}
