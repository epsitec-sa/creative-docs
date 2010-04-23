﻿//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using System.Text;
using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;

namespace Epsitec.Cresus.Core.Controllers
{
	public class MailContactViewController : EntityViewController
	{
		public MailContactViewController(string name)
			: base (name)
		{
		}

		public override IEnumerable<CoreController> GetSubControllers()
		{
			yield break;
		}

		public override void CreateUI(Widget container)
		{
			this.container = container;

			System.Diagnostics.Debug.Assert (this.Entity != null);
			var mailContact = this.Entity as Entities.MailContactEntity;
			System.Diagnostics.Debug.Assert (mailContact != null);

			// TODO: Il faudra créer ici un autre Tile permettant d'éditer !
			this.CreateSummaryTile (mailContact, ViewControllerMode.None, "Data.Mail", EntitySummary.GetMailTitle (mailContact), "[ <i>Ici prendra place l'édition de l'adresse</i> ]");

			this.AdjustLastTile ();
		}
	}
}
