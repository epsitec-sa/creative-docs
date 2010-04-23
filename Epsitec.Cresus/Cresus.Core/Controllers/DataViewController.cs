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
		public DataViewController(string name)
			: base (name)
		{
			this.controllers = new List<CoreController> ();

		}

		public void SetEntity(List<AbstractEntity> entities)
		{
			this.entities = entities;
		}

		public override IEnumerable<CoreController> GetSubControllers()
		{
			foreach (CoreController controller in this.controllers)
			{
				yield return controller;
			}
		}

		public override void CreateUI(Widget container)
		{
			System.Diagnostics.Debug.Assert (this.entities != null);

			int index = 0;
			foreach (Common.Support.EntityEngine.AbstractEntity entity in this.entities)
			{
				string name = string.Concat (this.Name, ".DataViewController", index.ToString(System.Globalization.CultureInfo.InvariantCulture));
				AbstractViewController viewController = AbstractViewController.CreateViewController (name, entity, ViewControllerMode.Compact);

				if (viewController != null)
				{
					FrameBox frame = new FrameBox
					{
						Parent = container,
						Margins = new Margins (0, 0, 0, (index<entities.Count-1) ? -1:0),
						Dock = DockStyle.Top,
					};

					viewController.CreateUI (frame);
					this.controllers.Add (viewController);

					index++;
				}
			}
		}


		private List<CoreController> controllers;
		private List<AbstractEntity> entities;
	}
}
