//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

namespace Epsitec.Cresus.Core.Controllers.SummaryControllers
{
	public class SummaryNaturalPersonViewController : SummaryAbstractPersonViewController
	{
		public SummaryNaturalPersonViewController(string name, Entities.NaturalPersonEntity entity)
			: base (name, entity)
		{
		}

		protected override void CreatePersonUI(UIBuilder builder)
		{
			var group = builder.CreateGroupingTile ("Data.NaturalPerson", "Personne physique", false);

			var accessor = new EntitiesAccessors.NaturalPersonAccessor (null, this.Entity, false);
			builder.CreateSummaryTile (group, accessor, false, ViewControllerMode.Edition);
		}
	}
}
