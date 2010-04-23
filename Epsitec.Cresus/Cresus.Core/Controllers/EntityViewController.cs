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
	public abstract class EntityViewController : CoreViewController
	{
		public EntityViewController(string name, AbstractEntity entity, ViewControllerMode mode)
			: base (name)
		{
			this.entity = entity;
			this.mode = mode;
		}

		public static EntityViewController CreateViewController(string name, AbstractEntity entity, ViewControllerMode mode)
		{
			if (entity is Entities.AbstractPersonEntity)
			{
				return new PersonViewController (name, entity, mode);
			}

			return null;
		}

	
		protected readonly AbstractEntity entity;
		protected readonly ViewControllerMode mode;
		private FrameBox frame;
	}
}
