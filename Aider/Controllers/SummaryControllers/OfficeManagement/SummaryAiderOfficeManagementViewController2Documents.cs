//	Copyright © 2012-2019, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Samuel LOUP, Maintainer: Pierre ARNAUD

using Epsitec.Aider.Entities;
using Epsitec.Aider.Enumerations;

using Epsitec.Cresus.Bricks;
using Epsitec.Cresus.Core.Bricks;

using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.SummaryControllers;
using Epsitec.Cresus.Core.Factories;

namespace Epsitec.Aider.Controllers.SummaryControllers
{
	[ControllerSubType (2)]
	public sealed class SummaryAiderOfficeManagementViewController2Documents : SummaryViewController<AiderOfficeManagementEntity>
	{
        public SummaryAiderOfficeManagementViewController2Documents()
        {
            //  Expect exactly two arguments:
            //  - type for filtering
            //  - year for filtering

            var args = EntityViewControllerFactory.Default.ControllerSubTypeId.Arg.Split (' ');

            if ((args.Length > 0) && System.Enum.TryParse (args[0], out EventType eventType))
            {
                this.filter = eventType;
            }

            if ((args.Length > 1) && int.TryParse (args[1], System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture, out var year))
            {
                this.year = year;
            }
        }

        protected override void CreateBricks(BrickWall<AiderOfficeManagementEntity> wall)
		{
			wall.AddBrick (p => p.GetDocuments (this.filter, this.year))
				.Attribute (BrickMode.DefaultToSummarySubView)
				.Attribute (BrickMode.AutoGroup)
				.Attribute (BrickMode.HideAddButton)
				.Attribute (BrickMode.HideRemoveButton)
                .Template ()
					.Title ("Documents")
					.Text (x => x.GetSummary ())								
				.End ();
		}

        private readonly EventType filter;
        private readonly int? year;
	}
}
