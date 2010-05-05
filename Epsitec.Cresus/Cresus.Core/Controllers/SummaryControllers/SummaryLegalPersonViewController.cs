//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

namespace Epsitec.Cresus.Core.Controllers.SummaryControllers
{
	public class SummaryLegalPersonViewController : SummaryAbstractPersonViewController
	{
		public SummaryLegalPersonViewController(string name, Entities.LegalPersonEntity entity, ViewControllerMode mode)
			: base (name, entity, mode)
		{
		}

		protected override void CreatePersonUI()
		{
			var group = EntityViewController.CreateGroupingTile (this.Container, "Data.LegalPerson", "Personne morale", false);

			var accessor = new EntitiesAccessors.LegalPersonAccessor (null, this.Entity, false);
			this.CreateSummaryTile (group, accessor, false, ViewControllerMode.PersonEdition);
		}
	}
}
