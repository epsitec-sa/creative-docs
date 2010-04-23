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
		public EntityViewController(string name)
			: base (name)
		{
		}

		public AbstractEntity Entity
		{
			get;
			set;
		}

		public ViewControllerMode Mode
		{
			get;
			set;
		}

		public Orchestrators.DataViewOrchestrator Orchestrator
		{
			get;
			set;
		}

		public static EntityViewController CreateViewController(string name, AbstractEntity entity, ViewControllerMode mode, Orchestrators.DataViewOrchestrator orchestrator)
		{
			EntityViewController controller = EntityViewController.ResolveViewController (name, entity);

			if (controller == null)
			{
				return null;
			}

			controller.Entity = entity;
			controller.Mode = mode;
			controller.Orchestrator = orchestrator;

			return controller;
		}
		
		private static EntityViewController ResolveViewController(string name, AbstractEntity entity)
		{
			if (entity is Entities.AbstractPersonEntity)
			{
				return new PersonViewController (name);
			}

			return null;
		}
	}
}
