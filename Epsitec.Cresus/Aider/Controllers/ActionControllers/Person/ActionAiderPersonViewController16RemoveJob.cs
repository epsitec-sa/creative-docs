//	Copyright © 2014-2019, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Samuel LOUP, Maintainer: Samuel LOUP

using Epsitec.Aider.Entities;

using Epsitec.Common.Support;
using Epsitec.Common.Types;

using Epsitec.Cresus.Bricks;

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.ActionControllers;
using Epsitec.Cresus.Core.Entities;

using System.Linq;

namespace Epsitec.Aider.Controllers.ActionControllers
{
    [ControllerSubType (16)]
    public sealed class ActionAiderPersonViewController16RemoveJob : ActionViewController<AiderPersonEntity>
    {
        public override FormattedText GetTitle()
        {
            return Resources.Text ("Supprimer un poste...");
        }

        public override ActionExecutor GetExecutor()
        {
            return ActionExecutor.Create<AiderEmployeeJobEntity> (this.Execute);
        }

        protected override void GetForm(ActionBrick<AiderPersonEntity, SimpleBrick<AiderPersonEntity>> form)
        {
            var jobs = this.Entity.Employee.EmployeeJobs;

            //  TODO: restrict jobs to *real job* entities (exluding AIDER users)

            form
                .Title ("Supprimer un poste")
                .Text ("La suppression d'un poste est une opération irréversible.")
                .Field<AiderEmployeeJobEntity> ()
                    .Title ("Poste à supprimer")
                    .WithFavorites (jobs, true)
                .End ()
            .End ();
        }

        private void Execute(AiderEmployeeJobEntity job)
        {
            if (job.IsNull ())
            {
                throw new BusinessRuleException ("Veuillez sélectionner un poste dans la liste");
            }

            job.Delete (this.BusinessContext);
        }
    }
}
