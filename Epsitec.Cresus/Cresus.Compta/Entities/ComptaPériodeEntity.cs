//	Copyright © 2011-2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Library;

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
			//	Par exemple:
			//	2012					-> une année entière
			//	2012 — 2013				-> deux années entières
			//	03.2012					-> un mois entier
			//	01 — 03.2012			-> quelques mois entiers
			//	10.01.2012 — 25.04.2012	-> une période quelconque
			get
			{
				FormattedText title;

				if (this.DateDébut.Year  == this.DateFin.Year &&
					this.DateDébut.Day   == 1  &&
					this.DateDébut.Month == 1  &&
					this.DateFin.Day     == 31 &&
					this.DateFin.Month   == 12 )  // pile une année entière ?
				{
					title = this.DateDébut.Year.ToString ();
				}
				else if (this.DateDébut.Day   == 1  &&
						 this.DateDébut.Month == 1  &&
						 this.DateFin.Day     == 31 &&
						 this.DateFin.Month   == 12)  // pile plusieurs années entières ?
				{
					title = this.DateDébut.Year.ToString () + " — " + this.DateFin.Year.ToString ();
				}
				else if (this.DateDébut.Year  == this.DateFin.Year &&
					     this.DateDébut.Month == this.DateFin.Month &&
						 this.DateDébut.Day   == 1  &&
						 Dates.IsLastDayOfMonth (this.DateFin))  // pile un mois entier ?
				{
					title = this.DateDébut.Month.ToString ("00") + "." + this.DateDébut.Year.ToString ();
				}
				else if (this.DateDébut.Year  == this.DateFin.Year &&
						 this.DateDébut.Day   == 1  &&
						 Dates.IsLastDayOfMonth (this.DateFin))  // pile quelques mois entiers ?
				{
					title = this.DateDébut.Month.ToString ("00") + " — " + this.DateFin.Month.ToString ("00") + "." + this.DateDébut.Year.ToString ();
				}
				else
				{
					title = this.DateDébut.ToString () + " — " + this.DateFin.ToString ();
				}

				if (!this.Description.IsNullOrEmpty)
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
