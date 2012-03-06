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
	/// Gère l'accès aux codes TVA de la comptabilité.
	/// </summary>
	public class CodesTVADataAccessor : AbstractDataAccessor
	{
		public CodesTVADataAccessor(AbstractController controller)
			: base (controller)
		{
			this.searchData = this.mainWindowController.GetSettingsSearchData ("Présentation.CodesTVA.Search");
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
				return this.compta.CodesTVA.Count;
			}
		}


		public override AbstractEntity GetEditionEntity(int row)
		{
			if (row < 0 || row >= this.compta.CodesTVA.Count)
			{
				return null;
			}
			else
			{
				return this.compta.CodesTVA[row];
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
				return this.compta.CodesTVA.IndexOf (entity as ComptaCodeTVAEntity);
			}
		}


		public override FormattedText GetText(int row, ColumnType column, bool all = false)
		{
			var codesTVA = compta.CodesTVA;

			if (row < 0 || row >= codesTVA.Count)
			{
				return FormattedText.Null;
			}

			var codeTVA = codesTVA[row];

			switch (column)
			{
				case ColumnType.Code:
					return codeTVA.Code;

				case ColumnType.Titre:
					return codeTVA.Description;

				case ColumnType.Taux:
					return codeTVA.LastTauxNom;

				case ColumnType.Compte:
					return JournalDataAccessor.GetNuméro (codeTVA.Compte);

				case ColumnType.Chiffre:
					return Converters.IntToString (codeTVA.Chiffre);

				case ColumnType.MontantFictif:
					return Converters.MontantToString (codeTVA.MontantFictif);

				default:
					return FormattedText.Null;
			}
		}


		public override void InsertEditionLine(int index)
		{
			var newData = new CodesTVAEditionLine (this.controller);

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
			this.editionLine.Add (new CodesTVAEditionLine (this.controller));
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
			this.editionLine[line].SetText (ColumnType.Format,    Converters.IntToString (1));
			this.editionLine[line].SetText (ColumnType.Numéro,    Converters.IntToString (1));
			this.editionLine[line].SetText (ColumnType.Incrément, Converters.IntToString (1));
		}

		public override void StartModificationLine(int row)
		{
			this.editionLine.Clear ();

			this.firstEditedRow = row;
			this.countEditedRow = 0;

			if (row >= 0 && row < this.compta.CodesTVA.Count)
			{
				var data = new CodesTVAEditionLine (this.controller);
				var codeTVA = this.compta.CodesTVA[row];
				data.EntityToData (codeTVA);

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
				var codeTVA = this.CreateCodesTVA ();
				data.DataToEntity (codeTVA);

				this.compta.CodesTVA.Add (codeTVA);

				if (firstRow == -1)
				{
					firstRow = this.compta.CodesTVA.Count-1;
				}
			}

			this.firstEditedRow = firstRow;
		}

		private void UpdateModificationData()
		{
			int row = this.firstEditedRow;

			var codeTVA = this.compta.CodesTVA[row];
			this.editionLine[0].DataToEntity (codeTVA);
		}


		public override FormattedText GetRemoveModificationLineError()
		{
			return FormattedText.Empty;
		}

		public override FormattedText GetRemoveModificationLineQuestion()
		{
			var codeTVA = this.compta.CodesTVA[this.firstEditedRow];
			return string.Format ("Voulez-vous supprimer le code TVA \"{0}\" ?", codeTVA.Code);
		}

		public override void RemoveModificationLine()
		{
			if (this.isModification)
			{
				for (int row = this.firstEditedRow+this.countEditedRow-1; row >= this.firstEditedRow; row--)
                {
					var codeTVA = this.compta.CodesTVA[row];
					this.DeleteCodesTVA (codeTVA);
					this.compta.CodesTVA.RemoveAt (row);
                }

				if (this.firstEditedRow >= this.compta.CodesTVA.Count)
				{
					this.firstEditedRow = this.compta.CodesTVA.Count-1;
				}
			}
		}


		public override bool MoveEditionLine(int direction)
		{
			if (this.IsMoveEditionLineEnable (direction))
			{
				var t1 = this.compta.CodesTVA[this.firstEditedRow];
				var t2 = this.compta.CodesTVA[this.firstEditedRow+direction];

				this.compta.CodesTVA[this.firstEditedRow] = t2;
				this.compta.CodesTVA[this.firstEditedRow+direction] = t1;

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


		private ComptaCodeTVAEntity CreateCodesTVA()
		{
			this.controller.MainWindowController.SetDirty ();

			ComptaCodeTVAEntity codeTVA;

			if (this.businessContext == null)
			{
				codeTVA = new ComptaCodeTVAEntity ();
			}
			else
			{
				codeTVA = this.businessContext.CreateEntity<ComptaCodeTVAEntity> ();
			}

			return codeTVA;
		}

		private void DeleteCodesTVA(ComptaCodeTVAEntity codeTVA)
		{
			this.controller.MainWindowController.SetDirty ();

			if (this.businessContext == null)
			{
				// rien à faire
			}
			else
			{
				this.businessContext.DeleteEntity (codeTVA);
			}
		}
	}
}