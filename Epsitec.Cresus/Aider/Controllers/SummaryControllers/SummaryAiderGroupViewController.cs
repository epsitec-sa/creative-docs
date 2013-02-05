//	Copyright � 2011-2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Marc BETTEX

using Epsitec.Aider.Entities;

using Epsitec.Cresus.Bricks;
using Epsitec.Cresus.Core.Bricks;
using Epsitec.Cresus.Core.Controllers.SummaryControllers;

namespace Epsitec.Aider.Controllers.SummaryControllers
{
	public sealed class SummaryAiderGroupViewController : SummaryViewController<AiderGroupEntity>
	{
		protected override void CreateBricks(BrickWall<AiderGroupEntity> wall)
		{
			wall.AddBrick ();

			wall.AddBrick (x => x.Subgroups)
				.Attribute (BrickMode.DefaultToSummarySubView)
				.Attribute (BrickMode.AutoGroup)
				// These two attributes have been added because this functionnality is not yet fully
				// implemented and we dont' want the users to mess up this data.
				.Attribute (BrickMode.HideAddButton)
				.Attribute (BrickMode.HideRemoveButton)
				.Template ()
					.Title ("Sous groupes")
				.End ();

			wall.AddBrick ()
				.Icon ("Data.AiderGroup.People")
				.Title (p => p.GetParticipantsTitle ())
				.Text (p => p.GetParticipantsSummary ())
				.Attribute (BrickMode.DefaultToSetSubView)
				.Attribute (BrickMode.SpecialController0);

			wall.AddBrick (x => x.Comment)
				.Attribute (BrickMode.AutoCreateNullEntity);
		}
	}
}
