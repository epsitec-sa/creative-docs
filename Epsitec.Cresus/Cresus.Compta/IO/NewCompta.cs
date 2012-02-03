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
			compta.DernièrePièce = null;

			compta.PlanComptable.Clear ();
			compta.Périodes.Clear ();
			compta.Journaux.Clear ();
		}

		public void NewEmpty(ComptaEntity compta)
		{
			compta.Name          = "vide";
			compta.Description   = null;
			compta.DernièrePièce = "0";

			compta.PlanComptable.Clear ();
			compta.Périodes.Clear ();
			compta.Journaux.Clear ();

			compta.Périodes.Add (this.CreatePériode ());
			compta.Journaux.Add (this.CreateJournal ());
		}

		private ComptaPériodeEntity CreatePériode()
		{
			//	Crée une première période.
			var now = Date.Today;
			var beginDate = new Date (now.Year,  1,  1);  // du 1 janvier
			var endDate   = new Date (now.Year, 12, 31);  // au 31 décembre

			var période = new ComptaPériodeEntity ();
			période.DateDébut    = beginDate;
			période.DateFin      = endDate;
			période.DernièreDate = beginDate;

			return période;
		}

		private ComptaJournalEntity CreateJournal()
		{
			//	Crée un journal principal.
			var journal = new ComptaJournalEntity ();
			journal.Name = "Principal";

			return journal;
		}

		public void NewModel(ComptaEntity compta)
		{
			// TODO...
			this.NewEmpty (compta);
		}
	}
}
