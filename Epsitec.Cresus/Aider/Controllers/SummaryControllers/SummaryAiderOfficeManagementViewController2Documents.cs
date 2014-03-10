//	Copyright © 2012-2014, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
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
	[ControllerSubType (2)]
	public sealed class SummaryAiderOfficeManagementViewController2Documents : SummaryViewController<AiderOfficeManagementEntity>
	{
		protected override void CreateBricks(BrickWall<AiderOfficeManagementEntity> wall)
		{

			/*wall.AddBrick (p => p.OfficeReports)
								.Attribute (BrickMode.DefaultToSummarySubView)
								.Attribute (BrickMode.AutoGroup)
								.Attribute (BrickMode.HideAddButton)
								.Attribute (BrickMode.HideRemoveButton)
								.Template ()
									.Title ("Documents")
									.Text (x => x.ReportName
												+ "<a href='/proxy/reporting/letter/"  // TODO: adapter les 2 arguments ici !!!
												+ Res.Commands.Base.PrintOfficeReports.CommandId + "/" 
												+ this.DataContext.GetPersistedId (x).Substring(3).Replace(':','-') + "' " +
												+ "target='_blank'>Consulter</a>")									
								.End ();*/
			
			
		}
	}
}
