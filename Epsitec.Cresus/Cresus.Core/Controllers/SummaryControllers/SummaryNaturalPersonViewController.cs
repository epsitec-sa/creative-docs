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

namespace Epsitec.Cresus.Core.Controllers.SummaryControllers
{
	public class SummaryNaturalPersonViewController : AbstractSummaryPersonViewController
	{
		public SummaryNaturalPersonViewController(string name, AbstractEntity entity, ViewControllerMode mode)
			: base (name, entity, mode)
		{
		}

		public override IEnumerable<CoreController> GetSubControllers()
		{
			yield break;
		}

		public override void CreateUI(Widget container)
		{
			this.container = container;

			System.Diagnostics.Debug.Assert (this.Entity != null);
			var person = this.Entity as Entities.NaturalPersonEntity;
			System.Diagnostics.Debug.Assert (person != null);

			this.CreateUITiles (person);
		}


		protected override void CreatePersonUI()
		{
			var group = EntityViewController.CreateGroupingTile (this.Container, "Data.NaturalPerson", "Personne physique", false);

			var accessor = new EntitiesAccessors.NaturalPersonAccessor (null, this.Entity, false);
			this.CreateSummaryTile (group, accessor, false, ViewControllerMode.PersonEdition);
		}
	}
}
