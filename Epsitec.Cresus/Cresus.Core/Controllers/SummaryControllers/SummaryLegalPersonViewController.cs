//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

namespace Epsitec.Cresus.Core.Controllers.SummaryControllers
{
	public class SummaryLegalPersonViewController : SummaryAbstractPersonViewController<Entities.LegalPersonEntity>
	{
		public SummaryLegalPersonViewController(string name, Entities.LegalPersonEntity entity)
			: base (name, entity)
		{
		}

		protected override void CreatePersonUI(UIBuilder builder)
		{
			var group = builder.CreateGroupingTile ("Data.LegalPerson", "Personne morale", false);

			var accessor = new EntitiesAccessors.LegalPersonAccessor (null, this.Entity, false)
			{
				ViewControllerMode = ViewControllerMode.Edition
			};

			builder.CreateSummaryTile (group, accessor, this);
		}
	}
}
