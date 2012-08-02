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
	/// Gère l'accès aux monnaies de la comptabilité.
	/// </summary>
	public class MonnaiesDataAccessor : AbstractDataAccessor
	{
		public MonnaiesDataAccessor(AbstractController controller)
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
				return this.compta.Monnaies.Count;
			}
		}


		public override AbstractEntity GetEditionEntity(int row)
		{
			if (row < 0 || row >= this.compta.Monnaies.Count)
			{
				return null;
			}
			else
			{
				return this.compta.Monnaies[row];
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
				return this.compta.Monnaies.IndexOf (entity as ComptaMonnaieEntity);
			}
		}


		public override FormattedText GetText(int row, ColumnType column, bool all = false)
		{
			var monnaies = compta.Monnaies;

			if (row < 0 || row >= monnaies.Count)
			{
				return FormattedText.Null;
			}

			var monnaie = monnaies[row];

			switch (column)
			{
				case ColumnType.Code:
					return monnaie.CodeISO;

				case ColumnType.Description:
					return monnaie.Description;

				case ColumnType.Décimales:
					return Converters.IntToString (monnaie.Décimales);

				case ColumnType.Arrondi:
					return Converters.DecimalToString (monnaie.Arrondi, monnaie.Décimales);

				case ColumnType.Cours:
					return Converters.DecimalToString (monnaie.Cours, 6);

				case ColumnType.Unité:
					return Converters.IntToString (monnaie.Unité);

				case ColumnType.CompteGain:
					return JournalDataAccessor.GetNuméro (monnaie.CompteGain);

				case ColumnType.ComptePerte:
					return JournalDataAccessor.GetNuméro (monnaie.ComptePerte);

				default:
					return FormattedText.Null;
			}
		}


		public override void InsertEditionLine(int index)
		{
			var newData = new MonnaiesEditionLine (this.controller);

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
			this.editionLine.Add (new MonnaiesEditionLine (this.controller));
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
			this.editionLine[line].SetText (ColumnType.Décimales,   Converters.IntToString (2));
			this.editionLine[line].SetText (ColumnType.Arrondi,     Converters.DecimalToString (0.01m, 2));
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

		public override int StartModificationLine(int row)
		{
			this.editionLine.Clear ();

			this.firstEditedRow = row;
			this.countEditedRow = 0;

			if (row >= 0 && row < this.compta.Monnaies.Count)
			{
				var data = new MonnaiesEditionLine (this.controller);
				var monnaie = this.compta.Monnaies[row];
				data.EntityToData (monnaie);

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
				var monnaie = this.CreateMonnaie ();
				data.DataToEntity (monnaie);

				this.compta.Monnaies.Add (monnaie);

				if (firstRow == -1)
				{
					firstRow = this.compta.Monnaies.Count-1;
				}
			}

			this.firstEditedRow = firstRow;
		}

		private void UpdateModificationData()
		{
			int row = this.firstEditedRow;

			var monnaie = this.compta.Monnaies[row];
			this.editionLine[0].DataToEntity (monnaie);
		}


		public override FormattedText GetRemoveModificationLineError()
		{
			if (this.compta.Monnaies.Count == 1)
			{
				return "Il doit exister au moins une monnaie.";
			}

			var monnaie = this.compta.Monnaies[this.firstEditedRow];
			int count = 0;

			foreach (var compte in this.compta.PlanComptable)
			{
				if (compte.Monnaie == monnaie)
				{
					count++;
				}
			}

			if (count == 0)
			{
				return FormattedText.Empty;  // ok
			}
			else
			{
				return string.Format ("Cette monnaie ne peut pas être supprimée,<br/>car elle est utilisée dans {0} compte{1}.", count.ToString (), (count>1)?"s":"");
			}
		}

		public override FormattedText GetRemoveModificationLineQuestion()
		{
			var monnaie = this.compta.Monnaies[this.firstEditedRow];
			return string.Format ("Voulez-vous supprimer la monnaie \"{0}\" ?", monnaie.CodeISO);
		}

		public override void RemoveModificationLine()
		{
			if (this.isModification)
			{
				for (int row = this.firstEditedRow+this.countEditedRow-1; row >= this.firstEditedRow; row--)
                {
					var monnaie = this.compta.Monnaies[row];
					this.DeleteMonnaie (monnaie);
					this.compta.Monnaies.RemoveAt (row);
                }

				if (this.firstEditedRow >= this.compta.Monnaies.Count)
				{
					this.firstEditedRow = this.compta.Monnaies.Count-1;
				}
			}
		}


		public override bool MoveEditionLine(int direction)
		{
			if (this.IsMoveEditionLineEnable (direction))
			{
				var t1 = this.compta.Monnaies[this.firstEditedRow];
				var t2 = this.compta.Monnaies[this.firstEditedRow+direction];

				this.compta.Monnaies[this.firstEditedRow] = t2;
				this.compta.Monnaies[this.firstEditedRow+direction] = t1;

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


		private ComptaMonnaieEntity CreateMonnaie()
		{
			this.controller.MainWindowController.SetDirty ();

			ComptaMonnaieEntity monnaie;

			if (this.businessContext == null)
			{
				monnaie = new ComptaMonnaieEntity ();
			}
			else
			{
				monnaie = this.businessContext.CreateEntity<ComptaMonnaieEntity> ();
			}

			monnaie.Cours = 1;
			monnaie.Unité = 1;

			return monnaie;
		}

		private void DeleteMonnaie(ComptaMonnaieEntity monnaie)
		{
			this.controller.MainWindowController.SetDirty ();

			if (this.businessContext == null)
			{
				// rien à faire
			}
			else
			{
				this.businessContext.DeleteEntity (monnaie);
			}
		}
	}
}