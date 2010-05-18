//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.EntityEngine;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers
{
	public abstract class TileController
	{
		public abstract EntityViewController CreateSubViewController(Orchestrators.DataViewOrchestrator orchestrator);
	}

	public class EntityTileController<T> : TileController where T : AbstractEntity
	{
		public T Entity
		{
			get;
			set;
		}

		public ViewControllerMode ChildrenMode
		{
			get;
			set;
		}

		public override EntityViewController CreateSubViewController(Orchestrators.DataViewOrchestrator orchestrator)
		{
			return EntityViewController.CreateEntityViewController ("ViewController", this.Entity, this.ChildrenMode, orchestrator);
		}
	}
}
