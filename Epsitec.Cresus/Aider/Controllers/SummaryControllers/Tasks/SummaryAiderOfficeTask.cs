//	Copyright © 2015, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Samuel LOUP, Maintainer: Samuel LOUP

using Epsitec.Aider.Entities;
using Epsitec.Cresus.Bricks;
using Epsitec.Cresus.Core.Bricks;
using Epsitec.Cresus.Core.Controllers.SummaryControllers;

namespace Epsitec.Aider.Controllers.SummaryControllers
{
	public sealed class SummaryAiderOfficeTask : SummaryViewController<AiderOfficeTaskEntity>
	{
		protected override void CreateBricks(BrickWall<AiderOfficeTaskEntity> wall)
		{
			var task    = this.Entity;
			var process = task.Process;

			switch (process.Type)
			{
				case Enumerations.OfficeProcessType.PersonsOutputProcess:
					this.CreateBricksPersonExitProcess (wall);
					break;
			}

		}

		private void CreateBricksPersonExitProcess(BrickWall<AiderOfficeTaskEntity> wall)
		{
			wall.AddBrick (x => x.Process)
							.Title ("Processus")
							.Text (x => x.GetSummary ())
							.Attribute (BrickMode.DefaultToNoSubView);

			wall.AddBrick (x => x.GetExitProcessPerson ())
							.Title ("Personne concernée")
							.Text (x => x.GetSummary ())
							.Attribute (BrickMode.DefaultToSummarySubView);


			wall.AddBrick (x => x.GetExitProcessParticipations ())
					.Attribute (BrickMode.HideAddButton)
					.Attribute (BrickMode.HideRemoveButton)
					.Attribute (BrickMode.AutoGroup)
					.Attribute (BrickMode.DefaultToNoSubView)
					.Template ()
						.Title ("Participations concernée")
						.Text (x => x.GetCompactSummary ())
					.End ();
		}
	}
}
