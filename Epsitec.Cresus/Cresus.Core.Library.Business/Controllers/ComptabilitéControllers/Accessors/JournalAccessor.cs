//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Types;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Business.Finance;
using Epsitec.Cresus.Core.Business.Finance.Comptabilité;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.ComptabilitéControllers
{
	/// <summary>
	/// Gère l'accès aux données du journal des écritures de la comptabilité.
	/// </summary>
	public class JournalAccessor : AbstractDataAccessor<JournalColumn, ComptabilitéEcritureEntity>
	{
		public JournalAccessor(ComptabilitéEntity comptabilitéEntity)
			: base (comptabilitéEntity)
		{
			this.UpdateSortedList ();
		}


		public override int Add(ComptabilitéEcritureEntity écriture)
		{
			this.comptabilitéEntity.Journal.Add (écriture);
			this.UpdateSortedList ();
			return this.sortedEntities.IndexOf (écriture);
		}

		public override void Remove(ComptabilitéEcritureEntity écriture)
		{
			this.comptabilitéEntity.Journal.Remove (écriture);
			this.UpdateSortedList ();
		}

		public override void UpdateSortedList()
		{
			this.sortedEntities = this.comptabilitéEntity.Journal.OrderBy (x => x.Date).ToList ();
		}


		public override FormattedText GetText(int row, JournalColumn column)
		{
			if (row < 0 || row >= this.Count)
			{
				return FormattedText.Null;
			}

			var écriture = this.sortedEntities[row];

			switch (column)
			{
				case JournalColumn.Date:
					return écriture.Date.ToString ();

				case JournalColumn.Débit:
					return JournalAccessor.GetNuméro (écriture.Débit);

				case JournalColumn.Crédit:
					return JournalAccessor.GetNuméro (écriture.Crédit);

				case JournalColumn.Pièce:
					return écriture.Pièce;

				case JournalColumn.Libellé:
					return écriture.Libellé;

				case JournalColumn.Montant:
					return écriture.Montant.ToString ("0.00");

				default:
					return FormattedText.Null;
			}
		}

		public override void SetText(int row, JournalColumn column, FormattedText text)
		{
			if (row < 0 || row >= this.Count)
			{
				return;
			}

			var écriture = this.sortedEntities[row];

			switch (column)
			{
				case JournalColumn.Date:
					Date? d;
					if (this.comptabilitéEntity.ParseDate (text, out d) && d.HasValue)
					{
						écriture.Date = d.Value;
					}
					break;

				case JournalColumn.Débit:
					écriture.Débit = JournalAccessor.GetCompte (this.comptabilitéEntity, text);
					break;

				case JournalColumn.Crédit:
					écriture.Crédit = JournalAccessor.GetCompte (this.comptabilitéEntity, text);
					break;

				case JournalColumn.Pièce:
					écriture.Pièce = text;
					break;

				case JournalColumn.Libellé:
					écriture.Libellé = text;
					break;

				case JournalColumn.Montant:
					decimal m;
					if (decimal.TryParse (text.ToSimpleText (), out m))
					{
						écriture.Montant = m;
					}
					break;
			}
		}


		private static FormattedText GetNuméro(ComptabilitéCompteEntity compte)
		{
			if (compte == null)
			{
				return JournalAccessor.multi;
			}
			else
			{
				return compte.Numéro;
			}
		}

		private static ComptabilitéCompteEntity GetCompte(ComptabilitéEntity comptabilité, FormattedText numéro)
		{
			numéro = PlanComptableAccessor.GetCompteNuméro (numéro);

			if (numéro.IsNullOrEmpty || numéro == JournalAccessor.multi)
			{
				return null;
			}
			else
			{
				return comptabilité.PlanComptable.Where (x => x.Numéro == numéro).FirstOrDefault ();
			}
		}


		public bool ParseDate(FormattedText text, out Date date)
		{
			//	Transforme un texte en une date valide pour la comptabilité.
			System.DateTime d;

			if (System.DateTime.TryParse (text.ToSimpleText (), System.Threading.Thread.CurrentThread.CurrentCulture, System.Globalization.DateTimeStyles.AssumeLocal | System.Globalization.DateTimeStyles.AllowWhiteSpaces, out d))
			{
				date = new Date (d);

				if (this.comptabilitéEntity.BeginDate.HasValue && date < this.comptabilitéEntity.BeginDate.Value)
				{
					date = this.comptabilitéEntity.BeginDate.Value;
					return false;
				}

				if (this.comptabilitéEntity.EndDate.HasValue && date > this.comptabilitéEntity.EndDate.Value)
				{
					date = this.comptabilitéEntity.EndDate.Value;
					return false;
				}

				return true;
			}
			else
			{
				date = Date.Today;
				return false;
			}
		}


		public static readonly FormattedText		multi = "...";
	}
}