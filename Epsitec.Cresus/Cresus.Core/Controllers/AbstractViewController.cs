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
	public enum ViewControllerMode
	{
		Compact,
		Edition,
	}


	public abstract class AbstractViewController : CoreController
	{
		public AbstractViewController(string name, AbstractEntity entity, ViewControllerMode mode)
			: base (name)
		{
			this.entity = entity;
			this.mode = mode;
		}

		public override IEnumerable<CoreController> GetSubControllers()
		{
			yield break;
		}

		public override void CreateUI(Widget container)
		{
			this.frame = new FrameBox
			{
				Parent = container,
				Dock = DockStyle.Fill,
			};
		}



		public static AbstractViewController CreateViewController(string name, AbstractEntity entity, ViewControllerMode mode)
		{
			AbstractViewController viewController = null;

			if (entity is Entities.AbstractPersonEntity)
			{
				viewController = new PersonViewController (name, entity, mode);
			}

			return viewController;
		}

	
		protected AbstractEntity entity;
		protected ViewControllerMode mode;
		private FrameBox frame;
	}
}
