//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
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
	public class UrisViewController : EntityViewController
	{
		public UrisViewController(string name)
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
			var person = this.Entity as Entities.AbstractPersonEntity;
			System.Diagnostics.Debug.Assert (person != null);

			// TODO: Il faudra créer ici un autre Tile permettant d'éditer !
			this.CreateSummaryTile (person, ViewControllerMode.None, "Data.Uri", "Mails", "[ <i>Ici prendra place l'édition des adresses mail</i> ]");

			this.AdjustLastTile ();
		}
	}
}
