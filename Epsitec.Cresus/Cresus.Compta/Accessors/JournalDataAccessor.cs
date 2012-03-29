//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Types;
using Epsitec.Common.Drawing;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus;

using Epsitec.Cresus.Compta.Controllers;
using Epsitec.Cresus.Compta.Entities;
using Epsitec.Cresus.Compta.Widgets;
using Epsitec.Cresus.Compta.Helpers;
using Epsitec.Cresus.Compta.Search.Data;
using Epsitec.Cresus.Compta.Options.Data;

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
			this.options    = this.mainWindowController.GetSettingsOptions<JournalOptions> ("Présentation.Journal.Options", this.compta);
			this.searchData = this.mainWindowController.GetSettingsSearchData ("Présentation.Journal.Search");
			this.filterData = this.mainWindowController.GetSettingsSearchData ("Présentation.Journal.Filter");

			this.UpdateAfterOptionsChanged ();
		}


		public override void UpdateFilter()
		{
			this.UpdateAfterOptionsChanged ();
		}

		public override void UpdateAfterOptionsChanged()
		{
			if (this.IsAllJournaux)
			{
				this.journalAll = this.période.Journal;
			}
			else
			{
				int id = (this.options as JournalOptions).JournalId;
				this.journalAll = this.période.Journal.Where (x => x.Journal.Id == id).ToList ();
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
					if (this.FilterLine (row))
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


		public override int GetIndexOf(AbstractEntity entity)
		{
			return this.journal.IndexOf (entity as ComptaEcritureEntity);
		}


		public override AbstractEntity GetEditionEntity(int row)
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

		public override int GetEditionIndex(AbstractEntity entity)
		{
			if (entity == null)
			{
				return -1;
			}
			else
			{
				return this.journal.IndexOf (entity as ComptaEcritureEntity);
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
					if (écriture.MultiId != 0 && !écriture.TotalAutomatique)
					{
						return FormattedText.Empty;
					}
					else
					{
						return Converters.DateToString (écriture.Date);
					}

				case ColumnType.Débit:
					return JournalDataAccessor.GetNuméro (écriture.Débit);

				case ColumnType.Crédit:
					return JournalDataAccessor.GetNuméro (écriture.Crédit);

				case ColumnType.Pièce:
					return écriture.Pièce;

				case ColumnType.Libellé:
					return this.GetLibellé (écriture);

				case ColumnType.Montant:
					var montantTTC = Core.TextFormatter.FormatText (Converters.MontantToString (écriture.Montant));
					if (écriture.TotalAutomatique)
					{
						montantTTC = montantTTC.ApplyBold ();
					}
					return montantTTC;

				case ColumnType.MontantTTC:
					if (écriture.Type == (int) TypeEcriture.BaseTVA)
					{
						return Converters.MontantToString (écriture.Montant + écriture.MontantComplément);
					}
					else
					{
						return FormattedText.Empty;
					}

				case ColumnType.CodeTVA:
					return JournalEditionLine.GetCodeTVADescription (écriture.CodeTVA);

				case ColumnType.TauxTVA:
					return Converters.PercentToString (écriture.TauxTVA);

				case ColumnType.CompteTVA:
					return JournalEditionLine.GetCodeTVACompte (écriture.CodeTVA);

				case ColumnType.Journal:
					return écriture.Journal.Nom;

				case ColumnType.Type:
					return écriture.ShortType;

				case ColumnType.OrigineTVA:
					return écriture.OrigineTVA;

				default:
					return FormattedText.Null;
			}
		}

		private FormattedText GetLibellé(ComptaEcritureEntity écriture)
		{
			//	Retourne le libellé à afficher dans le tableau.
			if (écriture.Type == (int) TypeEcriture.CodeTVA)
			{
				//	Une ligne 'CodeTVA' suit toujours une ligne 'BaseTVA'. Son champ Libellé est toujours
				//	identique au champ libellé de la ligne précédente 'BaseTVA'. Mais dans le journal, ce
				//	n'est pas ce qu'on désire afficher. On calcule donc un texte mieux adapté.

				return StringArray.SpecialContentRightAlignment + écriture.ShortLibelléTVA;
				//?return new string (' ', 10) + écriture.LibelléTVA;
				//?return FormattedText.Concat (UIBuilder.leftIndentText, écriture.LibelléTVA);
				//?return "□   " + écriture.LibelléTVA;
			}
			else
			{
				return écriture.Libellé;
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


		public override ColumnType ColumnForInsertionPoint
		{
			get
			{
				return ColumnType.Date;
			}
		}

		public override int InsertionPointRow
		{
			get
			{
				var text = this.editionLine[0].GetText (ColumnType.Date);

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


		public override void InsertEditionLine(int index)
		{
			var newData = new JournalEditionLine (this.controller);

			if (index == -1)
			{
				this.editionLine.Add (newData);
			}
			else
			{
				this.editionLine.Insert (index, newData);
			}

			base.InsertEditionLine (index);
		}

		public override void StartCreationLine()
		{
			this.editionLine.Clear ();
			this.editionLine.Add (new JournalEditionLine (this.controller));
			this.PrepareEditionLine (0);

			this.firstEditedRow = -1;
			this.countEditedRow = 1;

			this.isCreation = true;
			this.isModification = false;
			this.justCreated = false;

			this.controller.EditorController.UpdateFieldsEditionData ();
		}

		public override void ResetCreationLine()
		{
			if (this.justCreated)
			{
				this.PrepareEditionLine (0);

				//	Il ne doit pas rester de "..." dans les comptes au débit/crédit. Sinon, ils
				//	sèment le "pétchi", car le contrôleur du journal essaie tout de suite de créer
				//	les lignes correspondantes.
				if (this.editionLine[0].GetText (ColumnType.Débit) == JournalDataAccessor.multi)
				{
					this.editionLine[0].SetText (ColumnType.Débit, FormattedText.Empty);
				}

				if (this.editionLine[0].GetText (ColumnType.Crédit) == JournalDataAccessor.multi)
				{
					this.editionLine[0].SetText (ColumnType.Crédit, FormattedText.Empty);
				}

				this.editionLine[0].SetText (ColumnType.TotalAutomatique, "0");

				while (this.editionLine.Count > 1)
				{
					this.editionLine.RemoveAt (1);
				}

				this.countEditedRow = 1;
			}
		}

		protected override void PrepareEditionLine(int line)
		{
			this.editionLine[line].SetText (ColumnType.Date,       Converters.DateToString (this.période.ProchaineDate));
			this.editionLine[line].SetText (ColumnType.Pièce,      this.mainWindowController.PiècesGenerator.GetProchainePièce (this.GetDefaultJournal));
			this.editionLine[line].SetText (ColumnType.MontantTTC, FormattedText.Empty);
			this.editionLine[line].SetText (ColumnType.Montant,    Converters.MontantToString (0));
			this.editionLine[line].SetText (ColumnType.Type,       Converters.TypeEcritureToString (TypeEcriture.Nouveau));

			base.PrepareEditionLine (line);
		}

		public override void StartModificationLine(int row)
		{
			this.editionLine.Clear ();

			int firstRow, countRow;
			this.ExploreMulti (row, out firstRow, out countRow);

			if (firstRow != -1 && countRow > 0)
			{
				this.firstEditedRow = firstRow;
				this.countEditedRow = countRow;

				bool isMulti = this.countEditedRow > 1;

				for (int i = 0; i < this.countEditedRow; i++)
				{
					var data = new JournalEditionLine (this.controller);
					var écriture = this.journal[this.firstEditedRow+i];
					data.EntityToData (écriture);

					this.editionLine.Add (data);
				}

				//	Crée éventuellement la ligne vide supplémentaire, permettant à l'utilisateur d'ajouter une
				//	ligne à son écriture multiple sans utiliser le bouton "+".
				if (isMulti && this.controller.SettingsList.GetBool (SettingsType.EcritureProposeVide))
				{
					this.CreateEmptyLine (-1);
				}
			}

			this.initialCountEditedRow = this.countEditedRow - this.CountEmptyRow;
			this.isCreation = false;
			this.isModification = true;
			this.justCreated = false;

			this.controller.EditorController.UpdateFieldsEditionData ();
		}

		public void CreateEmptyLine(int index)
		{
			//	Crée une ligne vide à l'avant-dernière position.
			var écriture = new ComptaEcritureEntity ()
			{
				Type = (int) TypeEcriture.Vide,
			};

			var data = new JournalEditionLine (this.controller);
			data.EntityToData (écriture);

			if (this.controller.SettingsList.GetBool (SettingsType.EcriturePlusieursPièces))
			{
				data.SetText (ColumnType.Pièce, this.mainWindowController.PiècesGenerator.GetProchainePièce (this.GetDefaultJournal, this.editionLine.Count-1));
			}
			else
			{
				foreach (var line in this.editionLine)
				{
					var auto = line.GetData (ColumnType.TotalAutomatique);
					if (auto != null && auto.Text == "1")
					{
						data.SetText (ColumnType.Pièce, line.GetData (ColumnType.Pièce).Text);
					}
				}
			}

			//	Par défaut, on insère à l'avant-dernière ligne.
			if (index == -1)
			{
				index = this.editionLine.Count-1;
			}

			//	N'insère jamais une ligne vide entre BaseTVA et CodeTVA.
			if (index < this.editionLine.Count)
			{
				var type = Converters.StringToTypeEcriture (this.editionLine[index].GetText (ColumnType.Type));
				if (type == TypeEcriture.CodeTVA)
				{
					index++;
				}
			}

			this.editionLine.Insert (index, data);  // insère à l'avant-dernière position

			this.countEditedRow++;
		}

		public override void UpdateEditionLine()
		{
			if (this.isModification)
			{
				this.UpdateModificationData ();
				this.justCreated = true;
				this.isCreation = true;
				this.isModification = false;
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
			if (this.editionLine.Count > 1)
			{
				multiId = this.période.ProchainMultiId;
			}

			ComptaJournalEntity journalUtilisé = null;
			var pièces = new List<FormattedText> ();

			foreach (var data in this.editionLine)
			{
				if ((data as JournalEditionLine).IsEmptyLine)
				{
					continue;
				}

				this.compta.AddLibellé (this.période, data.GetText (ColumnType.Libellé));

				var écriture = this.CreateEcriture ();
				data.DataToEntity (écriture);
				écriture.MultiId = multiId;

				int row = this.GetSortedRow (écriture.Date);
				this.journal.Insert (row, écriture);

				if (!this.IsAllJournaux)
				{
					int globalRow = this.GetSortedRow (écriture.Date, global: true);
					this.période.Journal.Insert (globalRow, écriture);
				}

				if (firstRow == -1)
				{
					firstRow = row;
				}

				journalUtilisé = écriture.Journal;

				if (!écriture.Pièce.IsNullOrEmpty && !pièces.Contains (écriture.Pièce))
				{
					pièces.Add (écriture.Pièce);
				}
			}

			this.mainWindowController.PiècesGenerator.Burn (journalUtilisé, pièces);

			Date date;
			this.ParseDate (this.editionLine[0].GetText (ColumnType.Date), out date);
			this.période.DernièreDate = date;
			this.editionLine[0].SetText (ColumnType.Date, Converters.DateToString (date));

			this.editionLine[0].SetText (ColumnType.Pièce, this.mainWindowController.PiècesGenerator.GetProchainePièce (this.GetDefaultJournal));

			this.firstEditedRow = firstRow;
		}

		private void UpdateModificationData()
		{
			int row = this.firstEditedRow;
			var initialEcriture = this.journal[row];

			//	On passe dans l'espace global.
			row = this.période.Journal.IndexOf (initialEcriture);
			int globalFirstEditerRow = row;

			int multiId = this.période.Journal[row].MultiId;

			ComptaJournalEntity journalUtilisé = null;
			var pièces = new List<FormattedText> ();

			foreach (var data in this.editionLine)
			{
				if ((data as JournalEditionLine).IsEmptyLine)
				{
					continue;
				}

				this.compta.AddLibellé (this.période, data.GetText (ColumnType.Libellé));

				ComptaEcritureEntity écriture;

				if (row >= globalFirstEditerRow+this.initialCountEditedRow)
				{
					//	Crée une écriture manquante.
					écriture = this.CreateEcriture ();
					data.DataToEntity (écriture);
					écriture.MultiId = multiId;
					this.période.Journal.Insert (row, écriture);
				}
				else
				{
					//	Met à jour une écriture existante.
					écriture = this.période.Journal[row];
					data.DataToEntity (écriture);
				}

				journalUtilisé = écriture.Journal;

				if (!écriture.Pièce.IsNullOrEmpty && !pièces.Contains (écriture.Pièce))
				{
					pièces.Add (écriture.Pièce);
				}

				row++;
			}

			//	Supprime les écritures surnuméraires.
			int countToDelete  = this.initialCountEditedRow - (this.editionLine.Count - this.CountEmptyRow);
			while (countToDelete > 0)
            {
				var écriture = this.période.Journal[row];
				this.DeleteEcriture (écriture);
				this.période.Journal.RemoveAt (row);

				countToDelete--;
            }

			this.countEditedRow = this.editionLine.Count;

			//	Vérifie si les écritures modifiées doivent changer de place.
			if (!this.HasCorrectOrder (globalFirstEditerRow, global: true) ||
				!this.HasCorrectOrder (globalFirstEditerRow+this.countEditedRow-1, global: true))
			{
				var temp = new List<ComptaEcritureEntity> ();

				for (int i = 0; i < this.countEditedRow; i++)
                {
					var écriture = this.période.Journal[globalFirstEditerRow];
                    temp.Add (écriture);
					this.période.Journal.RemoveAt (globalFirstEditerRow);
                }

				int newRow = this.GetSortedRow (temp[0].Date, global: true);

				for (int i = 0; i < this.countEditedRow; i++)
                {
					var écriture = temp[i];
					this.période.Journal.Insert (newRow+i, écriture);
                }

				globalFirstEditerRow = newRow;
			}

			//	On revient dans l'espace spécifique.
			this.UpdateAfterOptionsChanged ();
			this.firstEditedRow = this.journal.IndexOf (initialEcriture);

			this.mainWindowController.PiècesGenerator.Burn (journalUtilisé, pièces);
		}

		public override int CountEmptyRow
		{
			//	Retourne le nombre de lignes vides.
			get
			{
				int count = 0;

				foreach (var x in this.editionLine)
				{
					if ((x as JournalEditionLine).IsEmptyLine)
					{
						count++;
					}
				}

				return count;
			}
		}


		public override FormattedText GetRemoveModificationLineQuestion()
		{
			if (this.CountEditedRowWithoutEmpty <= 1)
			{
				return "Voulez-vous supprimer l'écriture sélectionnée ?";
			}
			else
			{
				return string.Format ("Voulez-vous supprimer les {0} lignes de l'écriture sélectionnée ?", this.countEditedRow.ToString ());
			}
		}

		public override void RemoveModificationLine()
		{
			if (this.isModification)
			{
				for (int row = this.firstEditedRow+this.countEditedRow-1; row >= this.firstEditedRow; row--)
                {
					var écriture = this.journal[row];
					this.DeleteEcriture (écriture);
					this.journal.RemoveAt (row);

					this.période.Journal.Remove (écriture);
                }

				int firstRow = this.firstEditedRow;

				if (firstRow >= this.journal.Count)
				{
					firstRow = this.journal.Count-1;
				}

				this.ExploreMulti (firstRow, out this.firstEditedRow, out this.countEditedRow);
			}
		}


		public override bool MoveEditionLine(int direction)
		{
			if (this.IsMoveEditionLineEnable (direction))
			{
				var deleted = new List<ComptaEcritureEntity> ();

				for (int i = 0; i < this.countEditedRow; i++)
				{
					deleted.Add (this.journal[this.firstEditedRow]);
					this.journal.RemoveAt (this.firstEditedRow);
				}

				int row = (direction > 0) ? this.firstEditedRow : this.firstEditedRow-1;
				int firstRow, countRow;
				this.ExploreMulti(row, out firstRow, out countRow);

				this.firstEditedRow = (direction > 0) ? firstRow+countRow : firstRow;
				row = this.firstEditedRow;

				foreach (var écriture in deleted)
				{
					this.journal.Insert (row++, écriture);
				}

				return true;
			}
			else
			{
				return false;
			}
		}

		public override bool IsMoveEditionLineEnable(int direction)
		{
			if (this.firstEditedRow == -1)
			{
				return false;
			}

			int firstRow, countRow;
			this.ExploreMulti (this.firstEditedRow, out firstRow, out countRow);

			if (direction < 0)  // monte ?
			{
				if (firstRow+direction < 0)
				{
					return false;
				}

				if (this.journal[firstRow].Date != this.journal[firstRow-1].Date)
				{
					return false;
				}
			}

			if (direction > 0)  // descend ?
			{
				if (firstRow+countRow >= this.Count)
				{
					return false;
				}

				if (this.journal[firstRow].Date != this.journal[firstRow+countRow].Date)
				{
					return false;
				}
			}

			return true;
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
			écriture.Journal = this.GetDefaultJournal;

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

		private ComptaJournalEntity GetDefaultJournal
		{
			get
			{
				int id = (this.options as JournalOptions).JournalId;

				if (id == 0)  // tous les journaux ?
				{
					return null;
				}
				else
				{
					return this.compta.Journaux.Where (x => x.Id == id).FirstOrDefault ();
				}
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
			return JournalDataAccessor.GetSortedRow (this.GetJournal (global), date);
		}

		public static int GetSortedRow(IList<ComptaEcritureEntity> journal, Date date)
		{
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
				return this.période.Journal;
			}
			else
			{
				return this.journal;
			}
		}

		private void ExploreMulti(int row, out int firstRow, out int countRow)
		{
			JournalDataAccessor.ExploreMulti (this.journal, row, out firstRow, out countRow);
		}

		public static void ExploreMulti(IList<ComptaEcritureEntity> journal, int row, out int firstRow, out int countRow)
		{
			//	A partir d'une ligne quelconque d'une écriture multiple, retourne la première et le nombre (1..n).
			firstRow = row;
			countRow = 1;

			if (row < 0 || row >= journal.Count)  // garde-fou
			{
				return;
			}

			var écriture = journal[row];

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
				écriture = journal[--row];

				if (écriture.MultiId != multiId)
				{
					row++;
					break;
				}

				firstRow = row;
			}

			countRow = 0;
			while (row < journal.Count)
			{
				écriture = journal[row++];

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
				return compta.Journaux.Where (x => x.Nom == name).FirstOrDefault ();
			}
		}


		public bool ParseDate(FormattedText text, out Date date)
		{
			//	Transforme un texte en une date valide pour la comptabilité.
			System.DateTime d;

			if (System.DateTime.TryParse (text.ToSimpleText (), System.Threading.Thread.CurrentThread.CurrentCulture, System.Globalization.DateTimeStyles.AssumeLocal | System.Globalization.DateTimeStyles.AllowWhiteSpaces, out d))
			{
				date = new Date (d);

				if (date < this.période.DateDébut)
				{
					date = this.période.DateDébut;
					return false;
				}

				if (date > this.période.DateFin)
				{
					date = this.période.DateFin;
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
				return (this.options as JournalOptions).JournalId == 0;
			}
		}


		public static readonly FormattedText		multi = "...";

		private IList<ComptaEcritureEntity>			journalAll;
		private IList<ComptaEcritureEntity>			journal;
	}
}