//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

namespace Epsitec.Cresus.Core.Controllers.SummaryControllers
{
	public class SummaryNaturalPersonViewController : SummaryAbstractPersonViewController
	{
		public SummaryNaturalPersonViewController(string name, Entities.NaturalPersonEntity entity, ViewControllerMode mode)
			: base (name, entity, mode)
		{
		}

		protected override void CreatePersonUI()
		{
			var group = EntityViewController.CreateGroupingTile (this.Container, "Data.NaturalPerson", "Personne physique", false);

			var accessor = new EntitiesAccessors.NaturalPersonAccessor (null, this.Entity, false);
			this.CreateSummaryTile (group, accessor, false, ViewControllerMode.PersonEdition);
		}
	}
}
