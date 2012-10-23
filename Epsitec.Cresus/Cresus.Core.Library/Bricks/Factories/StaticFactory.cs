//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Widgets;

using Epsitec.Cresus.Core.Business;

using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

namespace Epsitec.Cresus.Core.Bricks.Factories
{
	public abstract class StaticFactory
	{
		protected StaticFactory(BusinessContext businessContext, ExpandoObject settings)
		{
			this.businessContext = businessContext;
			this.settings        = settings;
		}

		public abstract void CreateUI(FrameBox container, UIBuilder builder);

		protected readonly BusinessContext businessContext;
		protected readonly dynamic settings;
	}
}
