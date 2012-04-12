//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Types;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Compta.Controllers;
using Epsitec.Cresus.Compta.Entities;
using Epsitec.Cresus.Compta.Search.Data;
using Epsitec.Cresus.Compta.Helpers;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.Accessors
{
	/// <summary>
	/// Gère l'accès aux générateurs de numéros de pièces de la comptabilité.
	/// </summary>
	public class TauxChangeDataAccessor : AbstractDataAccessor
	{
		public TauxChangeDataAccessor(AbstractController controller)
			: base (controller)
		{
			this.searchData = this.mainWindowController.GetSettingsSearchData ("Présentation.TauxChange.Search");
		}


		public override void UpdateFilter()
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
				return this.compta.TauxChange.Count;
			}
		}


		public override AbstractEntity GetEditionEntity(int row)
		{
			if (row < 0 || row >= this.compta.TauxChange.Count)
			{
				return null;
			}
			else
			{
				return this.compta.TauxChange[row];
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
				return this.compta.TauxChange.IndexOf (entity as ComptaTauxChangeEntity);
			}
		}


		public override FormattedText GetText(int row, ColumnType column, bool all = false)
		{
			var listeTaux = compta.TauxChange;

			if (row < 0 || row >= listeTaux.Count)
			{
				return FormattedText.Null;
			}

			var taux = listeTaux[row];

			switch (column)
			{
				case ColumnType.Code:
					return taux.CodeISO;

				case ColumnType.Description:
					return taux.Description;

				case ColumnType.Cours:
					return Converters.DecimalToString (taux.Cours, 6);

				case ColumnType.Unité:
					return Converters.IntToString (taux.Unité);

				case ColumnType.CompteGain:
					return JournalDataAccessor.GetNuméro (taux.CompteGain);

				case ColumnType.ComptePerte:
					return JournalDataAccessor.GetNuméro (taux.ComptePerte);

				default:
					return FormattedText.Null;
			}
		}


		public override void InsertEditionLine(int index)
		{
			var newData = new TauxChangeEditionLine (this.controller);

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
			this.editionLine.Add (new TauxChangeEditionLine (this.controller));
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
			this.editionLine[line].SetText (ColumnType.Cours,       Converters.DecimalToString (1, 6));
			this.editionLine[line].SetText (ColumnType.Unité,       Converters.IntToString (1));
			this.editionLine[line].SetText (ColumnType.CompteGain,  this.GetCompte ("Gains de change"));
			this.editionLine[line].SetText (ColumnType.ComptePerte, this.GetCompte ("Pertes de change"));

			base.PrepareEditionLine (line);
		}

		private FormattedText GetCompte(string titre)
		{
			var compte = this.compta.PlanComptable.Where (x => x.Titre == titre).FirstOrDefault ();

			if (compte == null)
			{
				return FormattedText.Empty;
			}
			else
			{
				return compte.Numéro;
			}
		}

		public override void StartModificationLine(int row)
		{
			this.editionLine.Clear ();

			this.firstEditedRow = row;
			this.countEditedRow = 0;

			if (row >= 0 && row < this.compta.TauxChange.Count)
			{
				var data = new TauxChangeEditionLine (this.controller);
				var pièce = this.compta.TauxChange[row];
				data.EntityToData (pièce);

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
				var pièce = this.CreateTauxChange ();
				data.DataToEntity (pièce);

				this.compta.TauxChange.Add (pièce);

				if (firstRow == -1)
				{
					firstRow = this.compta.TauxChange.Count-1;
				}
			}

			this.firstEditedRow = firstRow;
		}

		private void UpdateModificationData()
		{
			int row = this.firstEditedRow;

			var pièce = this.compta.TauxChange[row];
			this.editionLine[0].DataToEntity (pièce);
		}


#if false
		public override FormattedText GetRemoveModificationLineError()
		{
			var taux = this.compta.TauxChange[this.firstEditedRow];
			return this.mainWindowController.TauxChange.GetRemoveError (taux);
		}
#endif

		public override FormattedText GetRemoveModificationLineQuestion()
		{
			var taux = this.compta.TauxChange[this.firstEditedRow];
			return string.Format ("Voulez-vous supprimer la monnaie \"{0}\" ?", taux.CodeISO);
		}

		public override void RemoveModificationLine()
		{
			if (this.isModification)
			{
				for (int row = this.firstEditedRow+this.countEditedRow-1; row >= this.firstEditedRow; row--)
                {
					var pièce = this.compta.TauxChange[row];
					this.DeleteTauxChange (pièce);
					this.compta.TauxChange.RemoveAt (row);
                }

				if (this.firstEditedRow >= this.compta.TauxChange.Count)
				{
					this.firstEditedRow = this.compta.TauxChange.Count-1;
				}
			}
		}


		public override bool MoveEditionLine(int direction)
		{
			if (this.IsMoveEditionLineEnable (direction))
			{
				var t1 = this.compta.TauxChange[this.firstEditedRow];
				var t2 = this.compta.TauxChange[this.firstEditedRow+direction];

				this.compta.TauxChange[this.firstEditedRow] = t2;
				this.compta.TauxChange[this.firstEditedRow+direction] = t1;

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


		private ComptaTauxChangeEntity CreateTauxChange()
		{
			this.controller.MainWindowController.SetDirty ();

			ComptaTauxChangeEntity taux;

			if (this.businessContext == null)
			{
				taux = new ComptaTauxChangeEntity ();
			}
			else
			{
				taux = this.businessContext.CreateEntity<ComptaTauxChangeEntity> ();
			}

			taux.Cours = 1;
			taux.Unité = 1;

			return taux;
		}

		private void DeleteTauxChange(ComptaTauxChangeEntity taux)
		{
			this.controller.MainWindowController.SetDirty ();

			if (this.businessContext == null)
			{
				// rien à faire
			}
			else
			{
				this.businessContext.DeleteEntity (taux);
			}
		}
	}
}