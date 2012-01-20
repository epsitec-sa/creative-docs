//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Types;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Business;

using Epsitec.Cresus.Compta.Entities;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.Accessors
{
	/// <summary>
	/// Gère l'accès aux données du journal des écritures de la comptabilité.
	/// </summary>
	public class JournalDataAccessor : AbstractDataAccessor
	{
		public JournalDataAccessor(BusinessContext businessContext, ComptabilitéEntity comptabilitéEntity)
			: base (businessContext, comptabilitéEntity)
		{
			this.StartCreationData ();
		}


		public override int Count
		{
			get
			{
				return this.comptabilitéEntity.Journal.Count;
			}
		}

		public override AbstractEntity GetEditionData(int row)
		{
			if (row < 0 || row >= this.comptabilitéEntity.Journal.Count)
			{
				return null;
			}
			else
			{
				return this.comptabilitéEntity.Journal[row];
			}
		}

		public override FormattedText GetText(int row, ColumnType column)
		{
			if (row < 0 || row >= this.Count)
			{
				return FormattedText.Null;
			}

			var écriture = this.comptabilitéEntity.Journal[row];

			switch (column)
			{
				case ColumnType.Date:
					return écriture.Date.ToString ();

				case ColumnType.Débit:
					return JournalDataAccessor.GetNuméro (écriture.Débit);

				case ColumnType.Crédit:
					return JournalDataAccessor.GetNuméro (écriture.Crédit);

				case ColumnType.Pièce:
					return écriture.Pièce;

				case ColumnType.Libellé:
					return écriture.Libellé;

				case ColumnType.Montant:
					var montant = TextFormatter.FormatText (écriture.Montant.ToString ("0.00"));

					if (écriture.TotalAutomatique)
					{
						montant = montant.ApplyBold ();
					}

					return montant;

				default:
					return FormattedText.Null;
			}
		}

		public override bool HasBottomSeparator(int row)
		{
			if (row < 0 || row >= this.Count-1)
			{
				return false;
			}

			var écriture1 = this.comptabilitéEntity.Journal[row];
			var écriture2 = this.comptabilitéEntity.Journal[row+1];

			return écriture1.MultiId != écriture2.MultiId;
		}


		public override int InsertionPointRow
		{
			get
			{
				var text = this.editionData[0].GetText (ColumnType.Date);

				Date date;
				this.ParseDate (text, out date);

				if (!this.justCreated && this.firstEditedRow != -1 && this.countEditedRow != 0)
				{
					if (this.HasCorrectOrder (this.firstEditedRow, date))
					{
						return -1;
					}
				}

				return this.GetSortedRow (date);
			}
		}


		public override void InsertEditionData(int index)
		{
			var newData = new JournalEditionData (this.comptabilitéEntity);

			if (index == -1)
			{
				this.editionData.Add (newData);
			}
			else
			{
				this.editionData.Insert (index, newData);
			}

			this.countEditedRow = this.editionData.Count;
		}

		public override void StartCreationData()
		{
			this.editionData.Clear ();
			this.editionData.Add (new JournalEditionData (this.comptabilitéEntity));
			this.PrepareEditionLine (0);

			this.firstEditedRow = -1;
			this.countEditedRow = 1;

			this.isModification = false;
			this.justCreated = false;
		}

		public override void ResetCreationData()
		{
			if (this.justCreated)
			{
				this.PrepareEditionLine (0);

				while (this.editionData.Count > 1)
				{
					this.editionData.RemoveAt (1);
				}

				this.countEditedRow = 1;
			}
		}

		protected override void PrepareEditionLine(int line)
		{
			this.editionData[line].SetText (ColumnType.Date,    this.comptabilitéEntity.ProchaineDate.ToString ());
			this.editionData[line].SetText (ColumnType.Pièce,   this.comptabilitéEntity.ProchainePièce);
			this.editionData[line].SetText (ColumnType.Montant, "0.00");
		}

		public override void StartModificationData(int row)
		{
			this.editionData.Clear ();

			int firstRow, countRow;
			this.ExploreMulti (row, out firstRow, out countRow);

			if (firstRow != -1 && countRow > 0)
			{
				this.firstEditedRow = firstRow;
				this.countEditedRow = countRow;

				for (int i = 0; i < this.countEditedRow; i++)
                {
					var data = new JournalEditionData (this.comptabilitéEntity);
					var écriture = this.comptabilitéEntity.Journal[this.firstEditedRow+i];
					data.EntityToData (écriture);

					this.editionData.Add (data);
                }
			}

			this.initialCountEditedRow = this.countEditedRow;
			this.isModification = true;
			this.justCreated = false;
		}

		public override void UpdateEditionData()
		{
			if (this.isModification)
			{
				this.UpdateModificationData ();
				this.justCreated = false;
			}
			else
			{
				this.UpdateCreationData ();
				this.justCreated = true;
			}

			this.SearchUpdate ();
		}

		private void UpdateCreationData()
		{
			int firstRow = -1;

			int multiId = 0;
			if (this.editionData.Count > 1)
			{
				multiId = this.comptabilitéEntity.ProchainMultiId;
			}

			foreach (var data in this.editionData)
			{
				var écriture = this.CreateEcriture ();
				data.DataToEntity (écriture);
				écriture.MultiId = multiId;

				int row = this.GetSortedRow (écriture.Date);
				this.comptabilitéEntity.Journal.Insert (row, écriture);

				if (firstRow == -1)
				{
					firstRow = row;
				}
			}

			Date date;
			this.ParseDate (this.editionData[0].GetText (ColumnType.Date), out date);
			this.comptabilitéEntity.DernièreDate = date;
			this.editionData[0].SetText (ColumnType.Date, date.ToString ());

			this.comptabilitéEntity.DernièrePièce = this.editionData[0].GetText (ColumnType.Pièce);
			this.editionData[0].SetText (ColumnType.Pièce, this.comptabilitéEntity.ProchainePièce);

			this.firstEditedRow = firstRow;
		}

		private void UpdateModificationData()
		{
			int row = this.firstEditedRow;
			int multiId = this.comptabilitéEntity.Journal[row].MultiId;

			foreach (var data in this.editionData)
			{
				if (row >= this.firstEditedRow+this.initialCountEditedRow)
				{
					//	Crée une écriture manquante.
					var écriture = this.CreateEcriture ();
					data.DataToEntity (écriture);
					écriture.MultiId = multiId;
					this.comptabilitéEntity.Journal.Insert (row, écriture);
				}
				else
				{
					//	Met à jour une écriture existante.
					var écriture = this.comptabilitéEntity.Journal[row];
					data.DataToEntity (écriture);
				}

				row++;
			}

			//	Supprime les écritures surnuméraires.
			int countToDelete  = this.initialCountEditedRow - this.editionData.Count;
			while (countToDelete > 0)
            {
				var écriture = this.comptabilitéEntity.Journal[row];
				this.DeleteEcriture (écriture);
				this.comptabilitéEntity.Journal.RemoveAt (row);

				countToDelete--;
            }

			this.countEditedRow = this.editionData.Count;

			//	Vérifie si les écritures modifiées doivent changer de place.
			if (!this.HasCorrectOrder (this.firstEditedRow) ||
				!this.HasCorrectOrder (this.firstEditedRow+this.countEditedRow-1))
			{
				var temp = new List<ComptabilitéEcritureEntity> ();

				for (int i = 0; i < this.countEditedRow; i++)
                {
					var écriture = this.comptabilitéEntity.Journal[this.firstEditedRow];
                    temp.Add (écriture);
					this.comptabilitéEntity.Journal.RemoveAt (this.firstEditedRow);
                }

				int newRow = this.GetSortedRow (temp[0].Date);

				for (int i = 0; i < this.countEditedRow; i++)
                {
					var écriture = temp[i];
					this.comptabilitéEntity.Journal.Insert (newRow+i, écriture);
                }

				this.firstEditedRow = newRow;
			}
		}

		public override void RemoveModificationData()
		{
			if (this.isModification)
			{
				for (int row = this.firstEditedRow+this.countEditedRow-1; row >= this.firstEditedRow; row--)
                {
					var écriture = this.comptabilitéEntity.Journal[row];
					this.DeleteEcriture (écriture);
					this.comptabilitéEntity.Journal.RemoveAt (row);
                }

				this.SearchUpdate ();
				this.StartCreationData ();
			}
		}


		private ComptabilitéEcritureEntity CreateEcriture()
		{
			if (this.businessContext == null)
			{
				return new ComptabilitéEcritureEntity ();
			}
			else
			{
				return this.businessContext.CreateEntity<ComptabilitéEcritureEntity> ();
			}
		}

		private void DeleteEcriture(ComptabilitéEcritureEntity  écriture)
		{
			if (this.businessContext == null)
			{
				// rien à faire
			}
			else
			{
				this.businessContext.DeleteEntity (écriture);
			}
		}


		private bool HasCorrectOrder(int row)
		{
			var écriture = this.comptabilitéEntity.Journal[row];
			return this.HasCorrectOrder (row, écriture.Date);
		}

		private bool HasCorrectOrder(int row, Date date)
		{
			if (row > 0 && this.Compare (row-1, date) > 0)
			{
				return false;
			}

			if (row > 0 && row < this.comptabilitéEntity.Journal.Count-1 && this.Compare (row+1, date) < 0)
			{
				return false;
			}

			return true;
		}

		private int Compare(int row, Date date)
		{
			var écriture = this.comptabilitéEntity.Journal[row];
			return écriture.Date.CompareTo (date);
		}

		private int GetSortedRow(Date date)
		{
#if false
			for (int row = count-1; row >= 0; row--)
            {
				var écriture = this.comptabilitéEntity.Journal[row];
            	
				if (écriture.Date <= date)
				{
					return row+1;
				}
            }

			return 0;
#else
			//	Effectue une recherche ultra-rapide, par bissection.
			int count = this.comptabilitéEntity.Journal.Count;

			if (count == 0)
			{
				return 0;
			}

			int inf = 0;        // borne inférieure
			int mid = 0;        // borne médiane
			int sup = count-1;  // borne supérieure

			ComptabilitéEcritureEntity écriture;

			//	Recherche grossière dans un premier temps.
			while (inf < sup-1)
			{
				mid = (inf+sup)/2;
				écriture = this.comptabilitéEntity.Journal[mid];

				if (écriture.Date == date)
				{
					break;
				}
				else if (écriture.Date < date)
				{
					inf = mid;
				}
				else
				{
					sup = mid;
				}
			}

			//	Affine la recherche dans un deuxième temps.
			while (mid > 0 && this.comptabilitéEntity.Journal[mid].Date > date)
			{
				mid--;
			}

			while (mid < count && this.comptabilitéEntity.Journal[mid].Date <= date)
			{
				mid++;
			}

			return mid;
#endif
		}

		private void ExploreMulti(int row, out int firstRow, out int countRow)
		{
			//	A partir d'une ligne quelconque d'une écriture multiple, retourne la première et le nombre (1..n).
			firstRow = row;
			countRow = 1;

			if (row == -1)
			{
				return;
			}

			var écriture = this.comptabilitéEntity.Journal[row];

			if (écriture == null)
			{
				return;
			}

			int multiId = écriture.MultiId;

			if (multiId == 0)  // pas une écriture multiple ?
			{
				return;
			}

			while (row > 0)
			{
				écriture = this.comptabilitéEntity.Journal[--row];

				if (écriture.MultiId != multiId)
				{
					firstRow = row+1;
					break;
				}
			}

			row++;
			countRow = 0;
			while (row < this.Count)
			{
				écriture = this.comptabilitéEntity.Journal[row++];

				if (écriture.MultiId != multiId)
				{
					break;
				}

				countRow++;
			}
		}


		public static FormattedText GetNuméro(ComptabilitéCompteEntity compte)
		{
			if (compte == null || compte.Numéro.IsNullOrEmpty)
			{
				return JournalDataAccessor.multi;
			}
			else
			{
				return compte.Numéro;
			}
		}

		public static ComptabilitéCompteEntity GetCompte(ComptabilitéEntity comptabilité, FormattedText numéro)
		{
			numéro = PlanComptableDataAccessor.GetCompteNuméro (numéro);

			if (numéro.IsNullOrEmpty || numéro == JournalDataAccessor.multi)
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