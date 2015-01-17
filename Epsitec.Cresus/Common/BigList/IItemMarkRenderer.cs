//	Copyright � 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Drawing;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Common.BigList
{
	public interface IItemMarkRenderer
	{
		void Render(ItemListMark mark, ItemListMarkOffset offset, Graphics graphics, Rectangle bounds);
	}
}
