//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Drawing;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Widgets;
using Epsitec.Common.Widgets.Layouts;

using Epsitec.Cresus.Core.Orchestrators;
using Epsitec.Cresus.Core.Widgets;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers
{
	public class PreviewViewController : CoreViewController
	{
		public PreviewViewController(string name, CoreData data)
			: base (name)
		{
			this.data = data;
		}

		
		public override IEnumerable<CoreController> GetSubControllers()
		{
			yield break;
		}

		public override void CreateUI(Widget container)
		{
			//	TODO: ...
		}

		
		private readonly CoreData data;
	}
}