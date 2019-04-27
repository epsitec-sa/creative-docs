//	Copyright © 2014-2019, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Aider.Entities;

using Epsitec.Common.Support;
using Epsitec.Common.Types;

using Epsitec.Cresus.Bricks;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.ActionControllers;

using System.Linq;

namespace Epsitec.Aider.Controllers.ActionControllers
{
  [ControllerSubType(3)]
  public sealed class ActionAiderEmployeeViewController03RemoveJob : TemplateActionViewController<AiderEmployeeEntity, AiderEmployeeJobEntity>
  {
    public override FormattedText GetTitle()
    {
      return Resources.Text("Supprimer un poste...");
    }

    public override ActionExecutor GetExecutor()
    {
      return ActionExecutor.Create(this.Execute);
    }

    protected override void GetForm(ActionBrick<AiderEmployeeEntity, SimpleBrick<AiderEmployeeEntity>> form)
    {
      form
          .Title("Supprimer un poste")
          .Text("Faut-il vraiment supprimer ce poste ?")
          .End();
    }

    private void Execute()
    {
      var context = this.BusinessContext;
      var job = this.AdditionalEntity;

      job.Delete(context);
    }
  }
}
