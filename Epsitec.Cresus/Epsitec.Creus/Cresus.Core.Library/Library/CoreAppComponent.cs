//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Library
{
	public abstract class CoreAppComponent : CoreComponent<CoreApp, CoreAppComponent>
	{
		protected CoreAppComponent(CoreApp app)
			: base (app)
		{
		}
	}
}
