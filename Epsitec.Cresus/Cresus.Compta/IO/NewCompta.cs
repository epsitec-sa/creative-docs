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
	public class NewCompta
	{
		public void NewNull(ComptaEntity compta)
		{
			compta.Name          = "vide";
			compta.Description   = null;
			compta.BeginDate     = null;
			compta.EndDate       = null;
			compta.DernièreDate  = null;
			compta.DernièrePièce = null;

			compta.Journal.Clear ();
			compta.PlanComptable.Clear ();
			compta.Journaux.Clear ();
		}

		public void NewEmpty(ComptaEntity compta)
		{
			var now = Date.Today;
			var beginDate = new Date (now.Year, 1, 1);  // 1 janvier
			var endDate   = new Date (now.Year, 12, 31);  // 31 décembre

			compta.Name          = "vide";
			compta.Description   = null;
			compta.BeginDate     = beginDate;
			compta.EndDate       = endDate;
			compta.DernièreDate  = beginDate;
			compta.DernièrePièce = "0";

			compta.Journal.Clear ();
			compta.PlanComptable.Clear ();
			compta.Journaux.Clear ();

			//	Crée un journal principal.
			var journal = new ComptaJournalEntity ();
			journal.Name = "Principal";

			compta.Journaux.Add (journal);
		}

		public void NewModel(ComptaEntity compta)
		{
			// TODO...
			this.NewEmpty (compta);
		}
	}
}
