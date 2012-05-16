//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Types;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Compta.Controllers;
using Epsitec.Cresus.Compta.Entities;
using Epsitec.Cresus.Compta.Helpers;
using Epsitec.Cresus.Compta.Search.Data;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.Accessors
{
	/// <summary>
	/// Gère l'accès aux écritures modèles de la comptabilité.
	/// </summary>
	public class ModèlesDataAccessor : AbstractDataAccessor
	{
		public ModèlesDataAccessor(AbstractController controller)
			: base (controller)
		{
			this.viewSettingsList = this.mainWindowController.GetViewSettingsList ("Présentation.Journal.ViewSettings");

			this.searchData = this.mainWindowController.GetSettingsSearchData ("Présentation.Modèles.Search");
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
				return this.compta.Modèles.Count;
			}
		}


		public override AbstractEntity GetEditionEntity(int row)
		{
			if (row < 0 || row >= this.compta.Modèles.Count)
			{
				return null;
			}
			else
			{
				return this.compta.Modèles[row];
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
				return this.compta.Modèles.IndexOf (entity as ComptaModèleEntity);
			}
		}


		public override FormattedText GetText(int row, ColumnType column, bool all = false)
		{
			var modèles = compta.Modèles;

			if (row < 0 || row >= modèles.Count)
			{
				return FormattedText.Null;
			}

			var modèle = modèles[row];

			switch (column)
			{
				case ColumnType.Code:
					return modèle.Code;

				case ColumnType.Raccourci:
					return modèle.Raccourci;

				case ColumnType.Débit:
					return ModèlesDataAccessor.GetNuméro (modèle.Débit);

				case ColumnType.Crédit:
					return ModèlesDataAccessor.GetNuméro (modèle.Crédit);

				case ColumnType.Pièce:
					return modèle.Pièce;

				case ColumnType.Libellé:
					return modèle.Libellé;

				case ColumnType.Montant:
					return Converters.MontantToString (modèle.Montant, this.compta.Monnaies[0]);

				default:
					return FormattedText.Null;
			}
		}


		public override void InsertEditionLine(int index)
		{
			var newData = new ModèlesEditionLine (this.controller);

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
			this.editionLine.Add (new ModèlesEditionLine (this.controller));
			this.PrepareEditionLine (0);

			this.firstEditedRow = -1;
			this.countEditedRow = 1;

			this.isCreation = true;
			this.isModification = false;
			this.justCreated = false;

			this.controller.EditorController.UpdateFieldsEditionData ();
		}

		protected override void PrepareEditionLine(int line)
		{
			this.editionLine[line].SetText (ColumnType.Raccourci, Converters.RaccourciToString (RaccourciModèle.None));

			base.PrepareEditionLine (line);
		}

		public override void StartModificationLine(int row)
		{
			this.editionLine.Clear ();

			this.firstEditedRow = row;
			this.countEditedRow = 0;

			if (row >= 0 && row < this.compta.Modèles.Count)
			{
				var data = new ModèlesEditionLine (this.controller);
				var modèle = this.compta.Modèles[row];
				data.EntityToData (modèle);

				this.editionLine.Add (data);
				this.countEditedRow++;
			}

			this.initialCountEditedRow = this.countEditedRow;
			this.isCreation = false;
			this.isModification = true;
			this.justCreated = false;

			this.controller.EditorController.UpdateFieldsEditionData ();
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
				var modèle = this.CreateModèle ();
				data.DataToEntity (modèle);

				this.compta.Modèles.Add (modèle);

				if (firstRow == -1)
				{
					firstRow = this.compta.Modèles.Count-1;
				}
			}

			this.firstEditedRow = firstRow;
		}

		private void UpdateModificationData()
		{
			int row = this.firstEditedRow;

			var modèle = this.compta.Modèles[row];
			this.editionLine[0].DataToEntity (modèle);
		}


		public override FormattedText GetRemoveModificationLineQuestion()
		{
			var modèle = this.compta.Modèles[this.firstEditedRow];
			return string.Format ("Voulez-vous supprimer l'écriture modèle  \"{0}\" ?", modèle.Libellé);
		}

		public override void RemoveModificationLine()
		{
			if (this.isModification)
			{
				for (int row = this.firstEditedRow+this.countEditedRow-1; row >= this.firstEditedRow; row--)
                {
					var modèle = this.compta.Modèles[row];
					this.DeleteModèle (modèle);
					this.compta.Modèles.RemoveAt (row);
                }

				if (this.firstEditedRow >= this.compta.Modèles.Count)
				{
					this.firstEditedRow = this.compta.Modèles.Count-1;
				}
			}
		}


		public override bool MoveEditionLine(int direction)
		{
			if (this.IsMoveEditionLineEnable (direction))
			{
				var t1 = this.compta.Modèles[this.firstEditedRow];
				var t2 = this.compta.Modèles[this.firstEditedRow+direction];

				this.compta.Modèles[this.firstEditedRow] = t2;
				this.compta.Modèles[this.firstEditedRow+direction] = t1;

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


		private ComptaModèleEntity CreateModèle()
		{
			this.controller.MainWindowController.SetDirty ();

			if (this.businessContext == null)
			{
				return new ComptaModèleEntity ();
			}
			else
			{
				return this.businessContext.CreateEntity<ComptaModèleEntity> ();
			}
		}

		private void DeleteModèle(ComptaModèleEntity modèle)
		{
			this.controller.MainWindowController.SetDirty ();

			if (this.businessContext == null)
			{
				// rien à faire
			}
			else
			{
				this.businessContext.DeleteEntity (modèle);
			}
		}


		public static FormattedText GetNuméro(ComptaCompteEntity compte)
		{
			if (compte == null || compte.Numéro.IsNullOrEmpty)
			{
				return FormattedText.Empty;
			}
			else
			{
				return compte.Numéro;
			}
		}

		public static ComptaCompteEntity GetCompte(ComptaEntity compta, FormattedText numéro)
		{
			if (numéro.IsNullOrEmpty)
			{
				return null;
			}
			else
			{
				return compta.PlanComptable.Where (x => x.Numéro == numéro).FirstOrDefault ();
			}
		}

	}
}