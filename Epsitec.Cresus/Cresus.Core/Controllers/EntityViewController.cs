//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types.Converters;
using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Widgets;

using Epsitec.Cresus.Core.Controllers.EditionControllers;
using Epsitec.Cresus.Core.Controllers.CreationControllers;
using Epsitec.Cresus.Core.Controllers.SummaryControllers;
using Epsitec.Cresus.Core.Orchestrators.Navigation;
using Epsitec.Cresus.Core.Widgets;
using Epsitec.Cresus.Core.Widgets.Tiles;

using Epsitec.Cresus.DataLayer;
using Epsitec.Cresus.DataLayer.Context;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers
{
	public abstract class EntityViewController : CoreViewController
	{
		protected EntityViewController(string name)
			: base (name)
		{
		}


		public TileContainer TileContainer
		{
			get;
			protected set;
		}

		public override IEnumerable<CoreController> GetSubControllers()
		{
			yield break;
		}

		public abstract AbstractEntity GetEntity();

		protected virtual EntityStatus GetEditionStatus()
		{
			return EntityStatus.None;
		}

		protected abstract void CreateUI();

		public T NotifyChildItemCreated<T>(T entity)
			where T : AbstractEntity
		{
			return entity;
		}
		
		public void NotifyChildItemDeleted<T>(T entity)
			where T : AbstractEntity
		{
		}

		
		protected void ReopenSubView(params NavigationPathElement[] elements)
		{
			var orchestrator = this.Orchestrator;
			var navigator    = this.Navigator;
			var history      = navigator.History;
			var path         = navigator.GetLeafNavigationPath ();

			path.AddRange (elements);

			using (history.SuspendRecording ())
			{
				history.NavigateInPlace (path);
			}
		}
	}
}