﻿//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Types;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Business;

using Epsitec.Cresus.Compta.Controllers;
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
		public JournalDataAccessor(AbstractController controller)
			: base (controller)
		{
			this.options    = this.mainWindowController.GetSettingsOptions<JournalOptions> ("Présentation.Journal.Options", this.comptaEntity);
			this.searchData = this.mainWindowController.GetSettingsSearchData<SearchData> ("Présentation.Journal.Search");
			this.filterData = this.mainWindowController.GetSettingsSearchData<SearchData> ("Présentation.Journal.Filter");

			this.UpdateAfterOptionsChanged ();
			this.StartCreationData ();
		}


		public override void FilterUpdate()
		{
			this.UpdateAfterOptionsChanged ();
		}

		public override void UpdateAfterOptionsChanged()
		{
			if (this.IsAllJournaux)
			{
				this.journalAll = this.périodeEntity.Journal;
			}
			else
			{
				var j = (this.options as JournalOptions).Journal;
				this.journalAll = this.périodeEntity.Journal.Where (x => x.Journal == j).ToList ();
			}

			if (this.filterData == null || this.filterData.IsEmpty)
			{
				this.journal = this.journalAll;
			}
			else
			{
				this.journal = new List<ComptaEcritureEntity> ();
				this.journal.Clear ();

				int count = this.journalAll.Count;
				for (int row = 0; row < count; row++)
				{
					int founds = this.FilterLine (row);

					if (founds != 0 && (this.filterData.OrMode || founds == this.filterData.TabsData.Count))
					{
						this.journal.Add (this.journalAll[row]);
					}
				}
			}

			this.soldesJournalManager.Initialize (this.journal);
		}


		public override int AllCount
		{
			get
			{
				return this.journalAll.Count;
			}
		}

		public override int Count
		{
			get
			{
				return this.journal.Count;
			}
		}

		public override AbstractEntity GetEditionData(int row)
		{
			if (row < 0 || row >= this.journal.Count)
			{
				return null;
			}
			else
			{
				return this.journal[row];
			}
		}

		public override FormattedText GetText(int row, ColumnType column, bool all = false)
		{
			var journal = all ? this.journalAll : this.journal;

			if (row < 0 || row >= journal.Count)
			{
				return FormattedText.Null;
			}

			var écriture = journal[row];

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

				case ColumnType.Journal:
					return écriture.Journal.Name;

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

			var écriture1 = this.journal[row];
			var écriture2 = this.journal[row+1];

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
			var newData = new JournalEditionData (this.controller);

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
			this.editionData.Add (new JournalEditionData (this.controller));
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
			this.editionData[line].SetText (ColumnType.Date,  this.périodeEntity.ProchaineDate.ToString ());
			this.editionData[line].SetText (ColumnType.Pièce, this.comptaEntity.ProchainePièce);
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
					var data = new JournalEditionData (this.controller);
					var écriture = this.journal[this.firstEditedRow+i];
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
			this.controller.MainWindowController.SetDirty ();
		}

		private void UpdateCreationData()
		{
			int firstRow = -1;

			int multiId = 0;
			if (this.editionData.Count > 1)
			{
				multiId = this.périodeEntity.ProchainMultiId;
			}

			foreach (var data in this.editionData)
			{
				var écriture = this.CreateEcriture ();
				data.DataToEntity (écriture);
				écriture.MultiId = multiId;

				int row = this.GetSortedRow (écriture.Date);
				this.journal.Insert (row, écriture);

				if (!this.IsAllJournaux)
				{
					int globalRow = this.GetSortedRow (écriture.Date, global: true);
					this.périodeEntity.Journal.Insert (globalRow, écriture);
				}

				if (firstRow == -1)
				{
					firstRow = row;
				}
			}

			Date date;
			this.ParseDate (this.editionData[0].GetText (ColumnType.Date), out date);
			this.périodeEntity.DernièreDate = date;
			this.editionData[0].SetText (ColumnType.Date, date.ToString ());

			this.comptaEntity.DernièrePièce = this.editionData[0].GetText (ColumnType.Pièce);
			this.editionData[0].SetText (ColumnType.Pièce, this.comptaEntity.ProchainePièce);

			this.firstEditedRow = firstRow;
		}

		private void UpdateModificationData()
		{
			int row = this.firstEditedRow;
			var initialEcriture = this.journal[row];

			//	On passe dans l'espace global.
			row = this.périodeEntity.Journal.IndexOf (initialEcriture);
			int globalFirstEditerRow = row;

			int multiId = this.périodeEntity.Journal[row].MultiId;

			foreach (var data in this.editionData)
			{
				if (row >= globalFirstEditerRow+this.initialCountEditedRow)
				{
					//	Crée une écriture manquante.
					var écriture = this.CreateEcriture ();
					data.DataToEntity (écriture);
					écriture.MultiId = multiId;
					this.périodeEntity.Journal.Insert (row, écriture);
				}
				else
				{
					//	Met à jour une écriture existante.
					var écriture = this.périodeEntity.Journal[row];
					data.DataToEntity (écriture);
				}

				row++;
			}

			//	Supprime les écritures surnuméraires.
			int countToDelete  = this.initialCountEditedRow - this.editionData.Count;
			while (countToDelete > 0)
            {
				var écriture = this.périodeEntity.Journal[row];
				this.DeleteEcriture (écriture);
				this.périodeEntity.Journal.RemoveAt (row);

				countToDelete--;
            }

			this.countEditedRow = this.editionData.Count;

			//	Vérifie si les écritures modifiées doivent changer de place.
			if (!this.HasCorrectOrder (globalFirstEditerRow, global: true) ||
				!this.HasCorrectOrder (globalFirstEditerRow+this.countEditedRow-1, global: true))
			{
				var temp = new List<ComptaEcritureEntity> ();

				for (int i = 0; i < this.countEditedRow; i++)
                {
					var écriture = this.périodeEntity.Journal[globalFirstEditerRow];
                    temp.Add (écriture);
					this.périodeEntity.Journal.RemoveAt (globalFirstEditerRow);
                }

				int newRow = this.GetSortedRow (temp[0].Date, global: true);

				for (int i = 0; i < this.countEditedRow; i++)
                {
					var écriture = temp[i];
					this.périodeEntity.Journal.Insert (newRow+i, écriture);
                }

				globalFirstEditerRow = newRow;
			}

			//	On revient dans l'espace spécifique.
			this.UpdateAfterOptionsChanged ();
			this.firstEditedRow = this.journal.IndexOf (initialEcriture);
		}

		public override void RemoveModificationData()
		{
			if (this.isModification)
			{
				for (int row = this.firstEditedRow+this.countEditedRow-1; row >= this.firstEditedRow; row--)
                {
					var écriture = this.journal[row];
					this.DeleteEcriture (écriture);
					this.journal.RemoveAt (row);

					this.périodeEntity.Journal.Remove (écriture);
                }

				this.SearchUpdate ();
				this.StartCreationData ();
			}
		}


		private ComptaEcritureEntity CreateEcriture()
		{
			this.controller.MainWindowController.SetDirty ();

			ComptaEcritureEntity écriture;

			if (this.businessContext == null)
			{
				écriture = new ComptaEcritureEntity ();
			}
			else
			{
				écriture = this.businessContext.CreateEntity<ComptaEcritureEntity> ();
			}

			//	Utilise le journal choisi dans les options.
			//	Si on est en mode "tous les journaux", on laisse null, car le bon sera mis plus tard.
			var journal = (this.options as JournalOptions).Journal;
			if (journal != null)  // dans un journal spécifique ?
			{
				écriture.Journal = journal;
			}

			return écriture;
		}

		private void DeleteEcriture(ComptaEcritureEntity  écriture)
		{
			this.controller.MainWindowController.SetDirty ();

			if (this.businessContext == null)
			{
				// rien à faire
			}
			else
			{
				this.businessContext.DeleteEntity (écriture);
			}
		}


		private bool HasCorrectOrder(int row, bool global = false)
		{
			var journal = this.GetJournal (global);
			var écriture = journal[row];
			return this.HasCorrectOrder (row, écriture.Date, global);
		}

		private bool HasCorrectOrder(int row, Date date, bool global = false)
		{
			var journal = this.GetJournal (global);

			if (row > 0 && this.Compare (row-1, date, global) > 0)
			{
				return false;
			}

			if (row < journal.Count-1 && this.Compare (row+1, date, global) < 0)
			{
				return false;
			}

			return true;
		}

		private int Compare(int row, Date date, bool global)
		{
			var journal = this.GetJournal (global);
			var écriture = journal[row];
			return écriture.Date.CompareTo (date);
		}

		private int GetSortedRow(Date date, bool global = false)
		{
			var journal = this.GetJournal (global);

#if false
			for (int row = count-1; row >= 0; row--)
            {
				var écriture = journal[row];
            	
				if (écriture.Date <= date)
				{
					return row+1;
				}
            }

			return 0;
#else
			//	Effectue une recherche ultra-rapide, par bissection.
			int count = journal.Count;

			if (count == 0)
			{
				return 0;
			}

			int inf = 0;        // borne inférieure
			int mid = 0;        // borne médiane
			int sup = count-1;  // borne supérieure

			ComptaEcritureEntity écriture;

			//	Recherche grossière dans un premier temps.
			while (inf < sup-1)
			{
				mid = (inf+sup)/2;
				écriture = journal[mid];

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
			while (mid > 0 && journal[mid].Date > date)
			{
				mid--;
			}

			while (mid < count && journal[mid].Date <= date)
			{
				mid++;
			}

			return mid;
#endif
		}

		private IList<ComptaEcritureEntity> GetJournal(bool global)
		{
			if (global)
			{
				return this.périodeEntity.Journal;
			}
			else
			{
				return this.journal;
			}
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

			var écriture = this.journal[row];

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
				écriture = this.journal[--row];

				if (écriture.MultiId != multiId)
				{
					row++;
					break;
				}

				firstRow = row;
			}

			countRow = 0;
			while (row < this.Count)
			{
				écriture = this.journal[row++];

				if (écriture.MultiId != multiId)
				{
					break;
				}

				countRow++;
			}
		}


		public static FormattedText GetNuméro(ComptaCompteEntity compte)
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

		public static ComptaCompteEntity GetCompte(ComptaEntity compta, FormattedText numéro)
		{
			numéro = PlanComptableDataAccessor.GetCompteNuméro (numéro);

			if (numéro.IsNullOrEmpty || numéro == JournalDataAccessor.multi)
			{
				return null;
			}
			else
			{
				return compta.PlanComptable.Where (x => x.Numéro == numéro).FirstOrDefault ();
			}
		}

		public static ComptaJournalEntity GetJournal(ComptaEntity compta, FormattedText name)
		{
			if (name.IsNullOrEmpty)
			{
				return null;
			}
			else
			{
				return compta.Journaux.Where (x => x.Name == name).FirstOrDefault ();
			}
		}


		public bool ParseDate(FormattedText text, out Date date)
		{
			//	Transforme un texte en une date valide pour la comptabilité.
			System.DateTime d;

			if (System.DateTime.TryParse (text.ToSimpleText (), System.Threading.Thread.CurrentThread.CurrentCulture, System.Globalization.DateTimeStyles.AssumeLocal | System.Globalization.DateTimeStyles.AllowWhiteSpaces, out d))
			{
				date = new Date (d);

				if (date < this.périodeEntity.DateDébut)
				{
					date = this.périodeEntity.DateDébut;
					return false;
				}

				if (date > this.périodeEntity.DateFin)
				{
					date = this.périodeEntity.DateFin;
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


		private bool IsAllJournaux
		{
			get
			{
				return (this.options as JournalOptions).Journal == null;
			}
		}


		public static readonly FormattedText		multi = "...";

		private IList<ComptaEcritureEntity>			journalAll;
		private IList<ComptaEcritureEntity>			journal;
	}
}