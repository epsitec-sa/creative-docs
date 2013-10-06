//	Copyright � 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Samuel LOUP, Maintainer: Samuel LOUP

using Epsitec.Aider.Entities;

using Epsitec.Common.Support;
using Epsitec.Common.Types;

using Epsitec.Cresus.Bricks;

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.ActionControllers;
using Epsitec.Cresus.Core.Entities;

using System.Collections.Generic;
using System.Linq;
using Epsitec.Aider.Enumerations;
using Epsitec.Cresus.DataLayer.Loader;
using Epsitec.Data.Platform;
using Epsitec.Aider.Data.ECh;
using Epsitec.Cresus.Core.Business.UserManagement;

namespace Epsitec.Aider.Controllers.ActionControllers
{
	[ControllerSubType (61)]
	public sealed class ActionAiderPersonWarningViewController61ProcessDepartureDeceased : ActionAiderPersonWarningViewControllerInteractive
	{
		public override bool IsEnabled
		{
			get
			{
				return true;
			}
		}

		public override FormattedText GetTitle()
		{
			return Resources.FormattedText ("La personne est d�c�d�e");
		}
		
		public override ActionExecutor GetExecutor()
		{
			return ActionExecutor.Create<Date> (this.Execute);
		}

		private void Execute(Date date)
		{
			var warning = this.Entity;
			var person  = warning.Person;

			AiderPersonEntity.KillPerson (this.BusinessContext, person, date);

		//	No need to clear the warning -- killing the person removes all warnings...
		//	this.ClearWarningAndRefreshCaches ();
		}


		protected override void GetForm(ActionBrick<AiderPersonWarningEntity, SimpleBrick<AiderPersonWarningEntity>> form)
		{
			var warning = this.Entity;
			var person  = warning.Person;
			var date    = person.eCH_Person.PersonDateOfDeath ?? Date.Today;

			form
				.Title ("Marquer la personne comme d�c�d�e")
				.Text ("Attention: cette op�ration est irr�versible.")
				.Field<Date> ()
					.Title ("Date du d�c�s")
					.InitialValue (date)
				.End ()
			.End ();           
		}
	}
}
