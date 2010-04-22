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
	public enum ViewControlerMode
	{
		Compact,
		Edition,
	}


	public abstract class AbstractViewControler : CoreController
	{
		public AbstractViewControler(string name)
			: base (name)
		{
		}

		public void SetEntity(AbstractEntity entity, ViewControlerMode mode)
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



		public static AbstractViewControler CreateViewControler(string name, AbstractEntity entity, ViewControlerMode mode)
		{
			AbstractViewControler viewControler = null;

			if (entity is Entities.AbstractPersonEntity)
			{
				viewControler = new PersonViewControler (name);
				viewControler.SetEntity (entity, mode);
			}

			return viewControler;
		}

	
		protected AbstractEntity entity;
		protected ViewControlerMode mode;
		private FrameBox frame;
	}
}
