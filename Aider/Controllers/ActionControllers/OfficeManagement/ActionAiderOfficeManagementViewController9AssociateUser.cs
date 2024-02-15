//	Copyright © 2019, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Aider.Entities;
using Epsitec.Aider.Enumerations;

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
    [ControllerSubType (9)]
    public sealed class ActionAiderOfficeManagementViewController9AssociateUser : ActionViewController<AiderOfficeManagementEntity>
    {
        public override FormattedText GetTitle()
        {
            return Resources.Text ("Associer un utilisateur");
        }

        public override ActionExecutor GetExecutor()
        {
            return ActionExecutor.Create<AiderUserEntity,
                                         EmployeeJobFunction,
                                         EmployeeEmployer> (this.Execute);
        }

        protected override void GetForm(ActionBrick<AiderOfficeManagementEntity, SimpleBrick<AiderOfficeManagementEntity>> form)
        {
            form
                .Title ("Associer un utilisateur")
                .Field<AiderUserEntity> ()
                    .Title ("Utilisateur à associer à cette gestion")
                .End ()
                .Field<EmployeeJobFunction> ()
                    .Title ("Fonction")
                .End ()
                .Field<EmployeeEmployer> ()
                    .Title ("Employeur")
                    .InitialValue (EmployeeEmployer.CS)
                .End ()
            .End ();
        }
        private void Execute(AiderUserEntity user,
                             EmployeeJobFunction jobFunction,
                             EmployeeEmployer employer)
        {
            var person = user.Contact.Person;

            if (person.IsNull ())
            {
                Logic.BusinessRuleException ("L'utilisateur sélectionné n'est pas lié correctement à une personne.");
            }
            if (person.Employee.IsNull ())
            {
                //  TODO: create employee first, then add job
                throw new System.NotImplementedException ();
            }

            switch (jobFunction)
            {
                case EmployeeJobFunction.GestionnaireAIDER:
                case EmployeeJobFunction.UtilisateurAIDER:
                case EmployeeJobFunction.SuppléantAIDER:
                    break;

                default:
                    Logic.BusinessRuleException ("La fonction choisie n'est pas compatible; sélectionnez la fonction liée à AIDER que l'utilisateur assure pour cette gestion.");
                    break;
            }

            var employee = person.Employee;
            var job = this.BusinessContext.CreateAndRegisterEntity<AiderEmployeeJobEntity> ();

            job.Employee = employee;
            job.EmployeeJobFunction = jobFunction;
            job.Employer = employer;
            job.Office = this.Entity;
        }
    }
}
