//	Copyright © 2015, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Samuel LOUP, Maintainer: Samuel LOUP

using Epsitec.Aider.Entities;
using Epsitec.Cresus.Bricks;
using Epsitec.Cresus.Core.Bricks;
using Epsitec.Cresus.Core.Controllers.SummaryControllers;
using Epsitec.Aider.Controllers.ActionControllers;

namespace Epsitec.Aider.Controllers.SummaryControllers
{
	public sealed class SummaryAiderOfficeTask : SummaryViewController<AiderOfficeTaskEntity>
	{
		protected override void CreateBricks(BrickWall<AiderOfficeTaskEntity> wall)
		{
			var task    = this.Entity;
			var process = task.Process;
			
			if (!task.IsDone)
			{
				switch (process.Type)
				{
					case Enumerations.OfficeProcessType.PersonsOutputProcess:
						this.CreateBrickForPersonsExitProcess (wall);
						break;
				}
			}
		}

		private void CreateBrickForPersonsExitProcess(BrickWall<AiderOfficeTaskEntity> wall)
		{
			var task    = this.Entity;

			wall.AddBrick ()
				.Title ("Veuillez choisir une action")
				.EnableActionButton<ActionAiderOfficeTaskViewController10RemoveParticipation> ();


			switch (task.Kind)
			{
				case Enumerations.OfficeTaskKind.CheckParticipation:
					wall.AddBrick (x => x.GetSourceEntity<AiderGroupParticipantEntity> (this.DataContext))
						.Title ("Participation à vérifier")
						.Text (x => x.GetSummaryWithHierarchicalGroupName ())
						.Attribute (BrickMode.DefaultToCreationOrEditionSubView);
					break;
			}
		}
	}
}
