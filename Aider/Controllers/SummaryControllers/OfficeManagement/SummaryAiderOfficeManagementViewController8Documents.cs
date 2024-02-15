//	Copyright © 2019, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Aider.Entities;
using Epsitec.Aider.Enumerations;

using Epsitec.Cresus.Bricks;

using Epsitec.Cresus.Core.Bricks;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.SummaryControllers;
using Epsitec.Cresus.Core.Factories;

using System.Linq;

namespace Epsitec.Aider.Controllers.SummaryControllers
{
    [ControllerSubType (8)]
    public sealed class SummaryAiderOfficeManagementViewController8Documents : SummaryViewController<AiderOfficeManagementEntity>
    {
        public SummaryAiderOfficeManagementViewController8Documents()
        {
            this.filter = (EventType)System.Enum.Parse (typeof (EventType), EntityViewControllerFactory.Default.ControllerSubTypeId.Arg);
        }

        protected override void CreateBricks(BrickWall<AiderOfficeManagementEntity> wall)
        {
            var docs  = this.Entity.GetEventDocuments ()[this.filter];
            var years = from doc in docs
                        group doc by doc.Year into g
                        select new
                        {
                            Value = g.Key,
                            Count = g.Count ()
                        };

            foreach (var year in years.Where (x => x.Value > 2010))
            {
                var arg = string.Format (System.Globalization.CultureInfo.InvariantCulture, "{0} {1}", this.filter, year.Value);

                wall.AddBrick ()
                    .Icon ("Data.ArticleAccountingDefinition")
                    .Title (p => string.Format ("Documents {0}", year.Value))
                    .Text (p => AiderOfficeManagementEntity.GetDocumentsSummary (year.Count))
                    .Attribute (BrickMode.DefaultToSummarySubView)
                    .WithSpecialController (typeof (SummaryAiderOfficeManagementViewController2Documents), arg);
            }
        }

        private readonly EventType filter;
    }
}
