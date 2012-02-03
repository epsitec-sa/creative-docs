//	Copyright © 2011-2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Library;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.Entities
{
	public partial class ComptaPériodeEntity
	{
		public FormattedText ShortTitle
		{
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
			if (text.IsNullOrEmpty)
			{
				date = null;
				return true;
			}

			var brut = text.ToSimpleText ();
			brut = brut.Replace (".", " ");
			brut = brut.Replace (",", " ");
			brut = brut.Replace ("/", " ");
			brut = brut.Replace ("-", " ");
			brut = brut.Replace (":", " ");
			brut = brut.Replace (";", " ");
			brut = brut.Replace ("  ", " ");
			brut = brut.Replace ("  ", " ");

			var words = brut.Split (' ');
			var defaultDate = this.ProchaineDate;
			int day, month, year;

			int.TryParse (words[0], out day);

			if (words.Length <= 1 || !int.TryParse (words[1], out month))
			{
				month = defaultDate.Month;
			}

			if (words.Length <= 2 || !int.TryParse (words[2], out year))
			{
				year = defaultDate.Year;
			}

			try
			{
				date = new Date (year, month, day);
			}
			catch
			{
				date = defaultDate;
				return false;
			}

			if (date < this.DateDébut)
			{
				date = this.DateDébut;
				return false;
			}

			if (date > this.DateFin)
			{
				date = this.DateFin;
				return false;
			}

			return true;
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
