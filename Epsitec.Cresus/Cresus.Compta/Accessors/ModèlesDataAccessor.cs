//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Types;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Business;

using Epsitec.Cresus.Compta.Controllers;
using Epsitec.Cresus.Compta.Entities;
using Epsitec.Cresus.Compta.Helpers;

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
			this.searchData = this.mainWindowController.GetSettingsSearchData<SearchData> ("Présentation.Modèles.Search");

			this.StartCreationLine ();
		}


		public override void FilterUpdate()
		{
			this.UpdateAfterOptionsChanged ();
		}

		public override void UpdateAfterOptionsChanged()
		{
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
				return this.comptaEntity.Modèles.Count;
			}
		}

		public override AbstractEntity GetEditionEntity(int row)
		{
			if (row < 0 || row >= this.comptaEntity.Modèles.Count)
			{
				return null;
			}
			else
			{
				return this.comptaEntity.Modèles[row];
			}
		}

		public override FormattedText GetText(int row, ColumnType column, bool all = false)
		{
			var modèles = comptaEntity.Modèles;

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
					if (modèle.Montant.HasValue)
					{
						return TextFormatter.FormatText (modèle.Montant.Value.ToString ("0.00"));
					}
					else
					{
						return FormattedText.Empty;
					}

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

			this.countEditedRow = this.editionLine.Count;
		}

		public override void StartCreationLine()
		{
			this.editionLine.Clear ();
			this.editionLine.Add (new ModèlesEditionLine (this.controller));
			this.PrepareEditionLine (0);

			this.firstEditedRow = -1;
			this.countEditedRow = 1;

			this.isModification = false;
			this.justCreated = false;
		}

		protected override void PrepareEditionLine(int line)
		{
			this.editionLine[line].SetText (ColumnType.Raccourci, Converters.RaccourciToString (RaccourciModèle.None));
		}

		public override void StartModificationLine(int row)
		{
			this.editionLine.Clear ();

			this.firstEditedRow = row;
			this.countEditedRow = 0;

			if (row >= 0 && row < this.comptaEntity.Modèles.Count)
			{
				var data = new ModèlesEditionLine (this.controller);
				var modèle = this.comptaEntity.Modèles[row];
				data.EntityToData (modèle);

				this.editionLine.Add (data);
				this.countEditedRow++;
			}

			this.initialCountEditedRow = this.countEditedRow;
			this.isModification = true;
			this.justCreated = false;
		}

		public override void UpdateEditionLine()
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

			foreach (var data in this.editionLine)
			{
				var modèle = this.CreateModèle ();
				data.DataToEntity (modèle);

				this.comptaEntity.Modèles.Add (modèle);

				if (firstRow == -1)
				{
					firstRow = this.comptaEntity.Modèles.Count-1;
				}
			}

			this.firstEditedRow = firstRow;
		}

		private void UpdateModificationData()
		{
			int row = this.firstEditedRow;

			var modèle = this.comptaEntity.Modèles[row];
			this.editionLine[0].DataToEntity (modèle);
		}

		public override void RemoveModificationLine()
		{
			if (this.isModification)
			{
				for (int row = this.firstEditedRow+this.countEditedRow-1; row >= this.firstEditedRow; row--)
                {
					var modèle = this.comptaEntity.Modèles[row];
					this.DeleteModèle (modèle);
					this.comptaEntity.Modèles.RemoveAt (row);
                }

				this.SearchUpdate ();
				this.StartCreationLine ();
			}
		}


		public override bool MoveEditionLine(int direction)
		{
			if (this.IsMoveEditionLineEnable (direction))
			{
				var t1 = this.comptaEntity.Modèles[this.firstEditedRow];
				var t2 = this.comptaEntity.Modèles[this.firstEditedRow+direction];

				this.comptaEntity.Modèles[this.firstEditedRow] = t2;
				this.comptaEntity.Modèles[this.firstEditedRow+direction] = t1;

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
			numéro = PlanComptableDataAccessor.GetCompteNuméro (numéro);

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