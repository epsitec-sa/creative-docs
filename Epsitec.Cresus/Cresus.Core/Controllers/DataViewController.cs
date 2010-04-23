//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using System.Text;
using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;

namespace Epsitec.Cresus.Core.Controllers
{
	/// <summary>
	/// Ce contrôleur représente une bande verticale dans laquelle on empile des tuiles AbstractViewController.
	/// </summary>
	public class DataViewController : CoreViewController
	{
		public DataViewController(string name, AbstractEntity entity)
			: base (name)
		{
			this.entity = entity;

			this.viewController = EntityViewController.CreateViewController ("DataViewController", entity, ViewControllerMode.Compact);
			System.Diagnostics.Debug.Assert (this.viewController != null);
		}

		public override IEnumerable<CoreController> GetSubControllers()
		{
			yield return this.viewController;
		}

		public override void CreateUI(Widget container)
		{
			this.viewController.CreateUI (container);
		}


		private AbstractEntity entity;
		private EntityViewController viewController;
	}
}
