//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Cresus.Core.Widgets.Tiles;

using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Epsitec.Cresus.Core.Controllers.DataAccessors.DynamicFactories
{
	internal abstract class DynamicFactory
	{
		public abstract object CreateUI(EditionTile tile, UIBuilder builder);
	}
}
