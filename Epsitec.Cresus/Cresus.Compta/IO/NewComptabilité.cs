//	Copyright © 2011-2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Types;
using Epsitec.Common.Support;

using Epsitec.Cresus.Compta.Entities;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.IO
{
	/// <summary>
	/// Cette classe s'occupe de créer une nouvelle comptabilité de toutes pièces.
	/// </summary>
	public class NewComptabilité
	{
		public void NewEmpty(ComptabilitéEntity comptabilité)
		{
			var now = Date.Today;
			var beginDate = new Date(now.Year,  1,  1);  // 1 janvier
			var endDate   = new Date(now.Year, 12, 31);  // 31 décembre

			comptabilité.Name          = "vide";
			comptabilité.Description   = null;
			comptabilité.BeginDate     = beginDate;
			comptabilité.EndDate       = endDate;
			comptabilité.DernièreDate  = beginDate;
			comptabilité.DernièrePièce = "0";

			comptabilité.Journal.Clear ();
			comptabilité.PlanComptable.Clear ();
		}

		public void NewModel(ComptabilitéEntity comptabilité)
		{
			// TODO...
			this.NewEmpty (comptabilité);
		}
	}
}
