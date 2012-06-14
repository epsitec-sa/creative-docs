//	Copyright © 2011-2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Types;

using Epsitec.Cresus.Compta.Helpers;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.Entities
{
	public partial class ComptaPériodeEntity
	{
		public FormattedText ShortTitle
		{
			//	Retourne un résumé de la période le plus court possible.
			get
			{
				var title = Dates.GetDescription (this.DateDébut, this.DateFin);

				if (!this.Description.IsNullOrEmpty ())
				{
					title = FormattedText.Concat (title, " (", this.Description, ")");
				}

				return title;
			}
		}

		public FormattedText Résumé
		{
			get
			{
				int count = this.Journal.Count;

				if (count == 0)
				{
					return "Aucune écriture";
				}
				else if (count == 1)
				{
					return "1 écriture";
				}
				else
				{
					return string.Format ("{0} écritures", count.ToString ());
				}
			}
		}


		public int ProchainMultiId
		{
			get
			{
				if (this.Journal.Count == 0)
				{
					return 1;
				}
				else
				{
					return this.Journal.Max (x => x.MultiId) + 1;
				}
			}
		}

		public Date ProchaineDate
		{
			//	Retourne la date par défaut pour la prochaine écriture.
			get
			{
				if (this.DernièreDate.HasValue)
				{
					return this.DernièreDate.Value;
				}

				if (this.Journal.Count == 0)
				{
					return this.DateDébut;
				}
				else
				{
					return this.Journal.Last ().Date;
				}
			}
		}

		public bool ParseDate(FormattedText text, out Date? date)
		{
			//	Transforme un texte en une date valide pour la comptabilité.
			return Converters.ParseDate (text.ToSimpleText (), this.ProchaineDate, this.DateDébut, this.DateFin, out date);
		}


		public int GetJournalCount(ComptaJournalEntity journal)
		{
			//	Retourne le nombre d'écritures d'un journal.
			if (journal == null)  // tous les journaux ?
			{
				return this.Journal.Count ();
			}
			else
			{
				return this.Journal.Where (x => x.Journal == journal).Count ();
			}
		}

		public string GetJournalSummary(ComptaJournalEntity journal)
		{
			//	Retourne le résumé d'un journal d'écritures.
			IEnumerable<ComptaEcritureEntity> écritures;

			if (journal == null)  // tous les journaux ?
			{
				écritures = this.Journal;
			}
			else
			{
				écritures = this.Journal.Where (x => x.Journal == journal);
			}

			int count = écritures.Count();

			if (count == 0)
			{
				return "Aucune écriture";
			}
			else if (count == 1)
			{
				var date = écritures.First ().Date.ToString ();
				return string.Format ("1 écriture, le {0}", date);
			}
			else
			{
				var beginDate = écritures.First ().Date;
				var endDate   = écritures.Last  ().Date;

				if (beginDate == endDate)
				{
					return string.Format ("{0} écritures, le {1}", count.ToString (), beginDate.ToString ());
				}
				else
				{
					return string.Format ("{0} écritures, du {1} au {2}", count.ToString (), beginDate.ToString (), endDate.ToString ());
				}
			}
		}
	}
}
