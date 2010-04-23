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
	public class DataViewController : CoreController
	{
		public DataViewController(string name, AbstractEntity entity)
			: base (name)
		{
			this.entity = entity;
		}

		public override IEnumerable<CoreController> GetSubControllers()
		{
			yield return this.viewController;
		}

		public override void CreateUI(Widget container)
		{
			System.Diagnostics.Debug.Assert (this.entity != null);

			string name = string.Concat (this.Name, ".DataViewController");
			this.viewController = AbstractViewController.CreateViewController (name, entity, ViewControllerMode.Compact);

			if (this.viewController != null)
			{
				FrameBox frame = new FrameBox
				{
					Parent = container,
					Dock = DockStyle.Fill,
//-					Margins = new Margins (0, 0, 0, (index<entity.Count-1) ? -1:0),
//-					Dock = DockStyle.Top,
				};

				viewController.CreateUI (frame);
			}
		}


		private AbstractEntity entity;
		private AbstractViewController viewController;
	}
}
