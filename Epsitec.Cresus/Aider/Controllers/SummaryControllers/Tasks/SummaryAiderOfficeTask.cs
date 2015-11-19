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
			wall.AddBrick (x => x.Process)
						.Title ("Processus")
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
			var task    = this.Entity;

			switch (task.Kind)
			{
				case Enumerations.OfficeTaskKind.CheckParticipation:
					if (this.Entity.GetSourceEntity<AiderGroupParticipantEntity> (this.DataContext).IsNull ())
					{
						wall.AddBrick ()
						.Title ("Terminer la tâche")
						.Text ("Une erreur est survenue dans le processus")
						.EnableActionButton<ActionAiderOfficeTaskViewController2Cancel> ();
					}
					else
					{
						wall.AddBrick (x => x.GetSourceEntity<AiderGroupParticipantEntity> (this.DataContext))
							.Title ("Participation à vérifier")
							.Text (x => x.GetSummaryWithHierarchicalGroupName ())
							.Attribute (BrickMode.DefaultToCreationOrEditionSubView);

						wall.AddBrick ()
							.Title ("Choisir une action:")
							.Text ("")
							.EnableActionButton<ActionAiderOfficeTaskViewController11KeepParticipation> ()
							.EnableActionButton<ActionAiderOfficeTaskViewController10RemoveParticipation> ();
					}

					break;
				case Enumerations.OfficeTaskKind.EnterNewAddress:
					if (this.Entity.GetSourceEntity<AiderContactEntity> (this.DataContext).IsNull ())
					{
						wall.AddBrick ()
							.Title ("Terminer la tâche")
							.Text ("Une erreur est survenue dans le processus")
							.EnableActionButton<ActionAiderOfficeTaskViewController2Cancel> ();
					}
					else
					{
						wall.AddBrick (x => x.GetSourceEntity<AiderContactEntity> (this.DataContext))
							.Title ("Nouvelle adresse")
							.Text (x => "Actuelle: \n" + x.GetAddress ().GetSummary ())
							.WithSpecialController (typeof (EditionAiderContactViewController1Address));

						wall.AddBrick ()
							.Title ("Terminer la tâche")
							.Text ("")
							.EnableActionButton<ActionAiderOfficeTaskViewController12AddressChanged> ();
					}

					break;
			}
		}
	}
}
