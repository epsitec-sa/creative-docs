//	Copyright © 2012-2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Samuel LOUP, Maintainer: Samuel LOUP

using Epsitec.Common.Support;
using Epsitec.Common.Types;

using Epsitec.Aider.Controllers.ActionControllers;
using Epsitec.Aider.Controllers.EditionControllers;
using Epsitec.Aider.Entities;
using Epsitec.Aider.Override;


using Epsitec.Cresus.Core.Business.UserManagement;
using Epsitec.Cresus.Core.Bricks;
using Epsitec.Cresus.Core.Controllers.SummaryControllers;
using Epsitec.Cresus.Core.Library;

using System.Linq;
using Epsitec.Cresus.Bricks;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Aider.Controllers.SetControllers;

namespace Epsitec.Aider.Controllers.SummaryControllers
{
	[ControllerSubType (5)]
	public sealed class SummaryAiderOfficeManagementViewController5Members : SummaryViewController<AiderOfficeManagementEntity>
	{
		protected override void CreateBricks(BrickWall<AiderOfficeManagementEntity> wall)
		{
			SummaryAiderOfficeManagementViewController5Members.CreateBricksParishMembers (wall);
			SummaryAiderOfficeManagementViewController5Members.CreateBricksDerogations (wall);	
		}

		public static void CreateBricksParishMembers(BrickWall<AiderOfficeManagementEntity> wall)
		{
			wall.AddBrick (p => p.ParishGroup)
				.Icon ("Data.AiderGroup.People")
				.Title ("Membres de la paroisse")
				.Text ("Voir tous les paroissiens")
				.Attribute (BrickMode.DefaultToSetSubView)
				.WithSpecialController (typeof (SetAiderGroupViewController0GroupParticipant));
		}

		private static void CreateBricksDerogations(BrickWall<AiderOfficeManagementEntity> wall)
		{
			wall.AddBrick (p => p.ParishGroup.Subgroups.Single (s => s.GroupDef.Classification == Enumerations.GroupClassification.DerogationIn))
				.Icon ("Data.AiderGroup.People")
				.Title ("Dérogations entrantes")
				.Text (p => p.GetParticipantsSummary ())
				.Attribute (BrickMode.DefaultToSetSubView)
				.EnableActionButton<ActionAiderGroupViewController9GenerateOfficialReport> ()
				.WithSpecialController (typeof (SetAiderGroupViewController2DerogationsContact));

			wall.AddBrick (p => p.ParishGroup.Subgroups.Single (s => s.GroupDef.Classification == Enumerations.GroupClassification.DerogationOut))
				.Icon ("Data.AiderGroup.People")
				.Title ("Dérogations sortantes")
				.Text (p => p.GetParticipantsSummary ())
				.Attribute (BrickMode.DefaultToSetSubView)
				.EnableActionButton<ActionAiderGroupViewController9GenerateOfficialReport> ()
				.WithSpecialController (typeof (SetAiderGroupViewController2DerogationsContact));
		}
	}
}
