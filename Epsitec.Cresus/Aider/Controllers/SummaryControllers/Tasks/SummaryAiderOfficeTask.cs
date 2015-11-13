//	Copyright © 2015, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Samuel LOUP, Maintainer: Samuel LOUP

using Epsitec.Aider.Entities;
using Epsitec.Cresus.Bricks;
using Epsitec.Cresus.Core.Bricks;
using Epsitec.Cresus.Core.Controllers.SummaryControllers;
using Epsitec.Aider.Controllers.ActionControllers;
using Epsitec.Aider.Override;

namespace Epsitec.Aider.Controllers.SummaryControllers
{
	public sealed class SummaryAiderOfficeTask : SummaryViewController<AiderOfficeTaskEntity>
	{
		protected override void CreateBricks(BrickWall<AiderOfficeTaskEntity> wall)
		{
			var task      = this.Entity;
			var process   = task.Process;
			var userCanDo = AiderUserManager.Current.AuthenticatedUser.CanDoTaskInOffice (this.Entity.Office);
			wall.AddBrick (x => x.Process)
						.Title ("Processus")
						.Text (x => x.GetSummary ())
						.Attribute (BrickMode.DefaultToNoSubView);

			if (!task.IsDone && userCanDo)
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

			switch (task.Kind)
			{
				case Enumerations.OfficeTaskKind.CheckParticipation:
					wall.AddBrick (x => x.GetSourceEntity<AiderGroupParticipantEntity> (this.DataContext))
						.Title ("Participation à vérifier")
						.Text (x => x.GetSummaryWithHierarchicalGroupName ())
						.Attribute (BrickMode.DefaultToCreationOrEditionSubView);

					wall.AddBrick ()
						.Title ("Choisir une action:")
						.EnableActionButton<ActionAiderOfficeTaskViewController11KeepParticipation> ()
						.EnableActionButton<ActionAiderOfficeTaskViewController10RemoveParticipation> ();
					break;
				case Enumerations.OfficeTaskKind.EnterNewAddress:
					wall.AddBrick (x => x.GetSourceEntity<AiderAddressEntity> (this.DataContext))
						.Title ("Nouvelle adresse")
						.Text (x => "Actuelle: \n" + x.GetSummary ())
						.Attribute (BrickMode.DefaultToCreationOrEditionSubView);

					wall.AddBrick ()
						.Title ("Terminer la tâche")
						.EnableActionButton<ActionAiderOfficeTaskViewController0Done> ();
					break;
			}
		}
	}
}
