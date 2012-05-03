//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Types;
using Epsitec.Common.Drawing;

using Epsitec.Cresus.Compta.Entities;
using Epsitec.Cresus.Compta.Accessors;
using Epsitec.Cresus.Compta.Controllers;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.Helpers
{
	public static class Rectangles
	{
		public static Rectangle MoveInside(Rectangle rect, Rectangle box)
		{
			//	Pousse 'rect' pour qu'il soit inclu dans 'box'.
			//	Ceci ne fonctionne bien entendu que si 'rect' est plus petit que 'box' !
			if (rect.Left < box.Left)
			{
				rect.Offset (box.Left-rect.Left, 0);
			}

			if (rect.Bottom < box.Bottom)
			{
				rect.Offset (0, box.Bottom-rect.Bottom);
			}

			if (rect.Right > box.Right)
			{
				rect.Offset (box.Right-rect.Right, 0);
			}

			if (rect.Top > box.Top)
			{
				rect.Offset (0, box.Top-rect.Top);
			}

			return rect;
		}
	}
}
