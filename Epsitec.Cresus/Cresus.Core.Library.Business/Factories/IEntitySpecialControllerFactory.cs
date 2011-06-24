//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Widgets;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Factories
{
	public interface IEntitySpecialControllerFactory
	{
		bool CanRepresent(AbstractEntity entity, int mode);
		IEntitySpecialController Create(TileContainer container, AbstractEntity entity, int mode);
	}
}
