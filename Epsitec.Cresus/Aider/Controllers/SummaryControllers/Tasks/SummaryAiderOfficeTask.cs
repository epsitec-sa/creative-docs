//	Copyright © 2015, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Samuel LOUP, Maintainer: Samuel LOUP

using Epsitec.Aider.Entities;
using Epsitec.Cresus.Bricks;
using Epsitec.Cresus.Core.Bricks;
using Epsitec.Cresus.Core.Controllers.SummaryControllers;
using Epsitec.Aider.Controllers.ActionControllers;
using Epsitec.Aider.Override;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Aider.Controllers.EditionControllers;

namespace Epsitec.Aider.Controllers.SummaryControllers
{
	public sealed class SummaryAiderOfficeTask : SummaryViewController<AiderOfficeTaskEntity>
	{
		protected override void CreateBricks(BrickWall<AiderOfficeTaskEntity> wall)
		{
			var task      = this.Entity;
			var process   = task.Process;
			var userCanDo = AiderUserManager.Current.AuthenticatedUser.CanDoTaskInOffice (this.Entity.Office);
			wall.AddBrick ()
						.Title ("Tâche")
						.Text (x => x.GetSummary ())
						.Attribute (BrickMode.DefaultToNoSubView);

			if (!task.IsDone && userCanDo)
			{
				switch (process.Type)
				{
					case Enumerations.OfficeProcessType.PersonsParishChangeProcess:
					case Enumerations.OfficeProcessType.PersonsOutputProcess:
						this.CreateBrickForPersonsExitProcess (wall);
						break;
				}
			}
		}

		private void CreateBrickForPersonsExitProcess(BrickWall<AiderOfficeTaskEntity> wall)
		{
			var task     = this.Entity;

			switch (task.Kind)
			{
				case Enumerations.OfficeTaskKind.CheckParticipation:
					if (this.Entity.GetSourceEntity<AiderGroupParticipantEntity> (this.DataContext).IsNull ())
					{
						wall.AddBrick ()
							.Title ("Tâche annulée")
							.Text ("Cette participation n'est plus à vérifier")
							.EnableActionButton<ActionAiderOfficeTaskViewController2Cancel> ();
					}
					else
					{
						wall.AddBrick (x => x.GetSourceEntity<AiderGroupParticipantEntity> (this.DataContext))
							.Title ("Participation à vérifier")
							.Text (task.GetTaskInfo ())
							.Attribute (BrickMode.DefaultToCreationOrEditionSubView);

						wall.AddBrick ()
							.Title ("Choisir une action:")
							.Text (task.GetTaskHelp ())
							.EnableActionButton<ActionAiderOfficeTaskViewController11KeepParticipation> ()
							.EnableActionButton<ActionAiderOfficeTaskViewController10RemoveParticipation> ();
					}

					break;
				case Enumerations.OfficeTaskKind.EnterNewAddress:
					if (this.Entity.GetSourceEntity<AiderContactEntity> (this.DataContext).IsNull ())
					{
						wall.AddBrick ()
							.Title ("Tâche annulée")
							.Text ("Le contact n'est plus disponible pour cette personne")
							.EnableActionButton<ActionAiderOfficeTaskViewController2Cancel> ();
					}
					else
					{
						wall.AddBrick (x => x.GetSourceEntity<AiderContactEntity> (this.DataContext))
							.Title ("Saisir la nouvelle adresse")
							.Text (task.GetTaskInfo ())
							.WithSpecialController (typeof (EditionAiderContactViewController1Address));

						wall.AddBrick ()
							.Title ("Choisir une action:")
							.Text (task.GetTaskHelp ())
							.EnableActionButton<ActionAiderOfficeTaskViewController12AddressChanged> ()
							.EnableActionButton<ActionAiderOfficeTaskViewController2Cancel> ();
					}

					break;
			}
		}
	}
}
