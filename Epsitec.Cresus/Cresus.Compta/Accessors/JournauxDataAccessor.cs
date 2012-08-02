//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Types;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Compta.Controllers;
using Epsitec.Cresus.Compta.Entities;
using Epsitec.Cresus.Compta.Search.Data;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.Accessors
{
	/// <summary>
	/// Gère l'accès aux journaux de la comptabilité.
	/// </summary>
	public class JournauxDataAccessor : AbstractDataAccessor
	{
		public JournauxDataAccessor(AbstractController controller)
			: base (controller)
		{
			this.viewSettingsList = this.mainWindowController.GetViewSettingsList (controller.ViewSettingsKey);
			this.searchData       = this.mainWindowController.GetSettingsSearchData (controller.SearchKey);
		}


		public override void UpdateFilter()
		{
			this.UpdateAfterOptionsChanged ();
		}


		public override int AllCount
		{
			get
			{
				return this.Count;
			}
		}

		public override int Count
		{
			get
			{
				return this.compta.Journaux.Count;
			}
		}


		public override AbstractEntity GetEditionEntity(int row)
		{
			if (row < 0 || row >= this.compta.Journaux.Count)
			{
				return null;
			}
			else
			{
				return this.compta.Journaux[row];
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
				return this.compta.Journaux.IndexOf (entity as ComptaJournalEntity);
			}
		}


		public override FormattedText GetText(int row, ColumnType column, bool all = false)
		{
			var journaux = compta.Journaux;

			if (row < 0 || row >= journaux.Count)
			{
				return FormattedText.Null;
			}

			var journal = journaux[row];

			switch (column)
			{
				case ColumnType.Titre:
					return journal.Nom;

				case ColumnType.Libellé:
					return journal.Description;

				case ColumnType.Pièce:
					return JournauxDataAccessor.GetPiècesGenerator (journal);

				case ColumnType.Résumé:
					return this.compta.GetJournalRésumé (journal);

				default:
					return FormattedText.Null;
			}
		}


		public override void InsertEditionLine(int index)
		{
			var newData = new JournauxEditionLine (this.controller);

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
			this.editionLine.Add (new JournauxEditionLine (this.controller));
			this.PrepareEditionLine (0);

			this.firstEditedRow = -1;
			this.countEditedRow = 1;

			this.isCreation = true;
			this.isModification = false;
			this.justCreated = false;

			this.controller.EditorController.UpdateFieldsEditionData ();
		}

		public override int StartModificationLine(int row)
		{
			this.editionLine.Clear ();

			this.firstEditedRow = row;
			this.countEditedRow = 0;

			if (row >= 0 && row < this.compta.Journaux.Count)
			{
				var data = new JournauxEditionLine (this.controller);
				var journal = this.compta.Journaux[row];
				data.EntityToData (journal);

				this.editionLine.Add (data);
				this.countEditedRow++;
			}

			this.initialCountEditedRow = this.countEditedRow;
			this.isCreation = false;
			this.isModification = true;
			this.justCreated = false;

			this.controller.EditorController.UpdateFieldsEditionData ();

			return row;
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

			foreach (var data in this.editionLine)
			{
				var journal = this.CreateJournal ();
				data.DataToEntity (journal);

				this.compta.Journaux.Add (journal);

				if (firstRow == -1)
				{
					firstRow = this.compta.Journaux.Count-1;
				}
			}

			this.firstEditedRow = firstRow;
		}

		private void UpdateModificationData()
		{
			int row = this.firstEditedRow;

			var journal = this.compta.Journaux[row];
			this.editionLine[0].DataToEntity (journal);
		}


		public override FormattedText GetRemoveModificationLineError()
		{
			var journal = this.compta.Journaux[this.firstEditedRow];
			return this.compta.GetJournalRemoveError (journal);
		}

		public override FormattedText GetRemoveModificationLineQuestion()
		{
			var journal = this.compta.Journaux[this.firstEditedRow];
			return string.Format ("Voulez-vous supprimer le journal \"{0}\" ?", journal.Nom);
		}

		public override void RemoveModificationLine()
		{
			if (this.isModification)
			{
				for (int row = this.firstEditedRow+this.countEditedRow-1; row >= this.firstEditedRow; row--)
                {
					var journal = this.compta.Journaux[row];
					this.DeleteJournal (journal);
					this.compta.Journaux.RemoveAt (row);
                }

				if (this.firstEditedRow >= this.compta.Journaux.Count)
				{
					this.firstEditedRow = this.compta.Journaux.Count-1;
				}
			}
		}


		public override bool MoveEditionLine(int direction)
		{
			if (this.IsMoveEditionLineEnable (direction))
			{
				var t1 = this.compta.Journaux[this.firstEditedRow];
				var t2 = this.compta.Journaux[this.firstEditedRow+direction];

				this.compta.Journaux[this.firstEditedRow] = t2;
				this.compta.Journaux[this.firstEditedRow+direction] = t1;

				this.firstEditedRow += direction;

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

			return this.firstEditedRow+direction >= 0 && this.firstEditedRow+direction < this.Count;
		}


		private ComptaJournalEntity CreateJournal()
		{
			this.controller.MainWindowController.SetDirty ();

			ComptaJournalEntity journal;

			if (this.businessContext == null)
			{
				journal = new ComptaJournalEntity ();
			}
			else
			{
				journal = this.businessContext.CreateEntity<ComptaJournalEntity> ();
			}

			journal.Id = this.compta.GetJournalId ();  // assigne un identificateur unique

			return journal;
		}

		private void DeleteJournal(ComptaJournalEntity journal)
		{
			this.controller.MainWindowController.SetDirty ();

			if (this.businessContext == null)
			{
				// rien à faire
			}
			else
			{
				this.businessContext.DeleteEntity (journal);
			}
		}


		public static FormattedText GetPiècesGenerator(ComptaJournalEntity journal)
		{
			if (journal.PiècesGenerator == null)
			{
				return FormattedText.Empty;
			}
			else
			{
				return journal.PiècesGenerator.Nom;
			}
		}

		public static ComptaPiècesGeneratorEntity GetPiècesGenerator(ComptaEntity compta, FormattedText pièce)
		{
			if (pièce.IsNullOrEmpty ())
			{
				return null;
			}
			else
			{
				return compta.PiècesGenerator.Where (x => x.Nom == pièce).FirstOrDefault ();
			}
		}
	}
}