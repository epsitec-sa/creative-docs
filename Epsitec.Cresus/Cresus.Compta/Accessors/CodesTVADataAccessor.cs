//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Types;
using Epsitec.Common.Drawing;
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
			var result = FormattedText.Empty;

			switch (column)
			{
				case ColumnType.Désactivé:
					string icon = codeTVA.Désactivé ? "Button.CheckNo" : "Button.CheckYes";
					return string.Format (@"<img src=""{0}"" voff=""-5""/>", UIBuilder.GetResourceIconUri (icon));

				case ColumnType.Code:
					result = codeTVA.Code;
					break;

				case ColumnType.Titre:
					result = codeTVA.Description;
					break;

				case ColumnType.Taux:
					result = (codeTVA.ListeTaux == null) ? FormattedText.Empty : codeTVA.ListeTaux.Nom;
					break;

				case ColumnType.Compte:
					result = JournalDataAccessor.GetNuméro (codeTVA.Compte);
					break;

				case ColumnType.Chiffre:
					result = Converters.IntToString (codeTVA.Chiffre);
					break;

				case ColumnType.MontantFictif:
					result = Converters.MontantToString (codeTVA.MontantFictif, this.compta.Monnaies[0]);
					break;
			}

			if (codeTVA.Désactivé)
			{
				result = result.ApplyItalic ().ApplyFontColor (Color.FromBrightness (0.5));
			}

			return result;
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
			this.editionLine[line].SetText (ColumnType.Désactivé, "1");  // met la coche 'Code activé' (logique inversée)

			base.PrepareEditionLine (line);
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
			var codeTVA = this.compta.CodesTVA[this.firstEditedRow];
			int count = 0;

			foreach (var période in this.compta.Périodes)
			{
				count += période.Journal.Where (x => x.CodeTVA == codeTVA).Count ();
			}

			if (count == 0)
			{
				return FormattedText.Empty;  // ok
			}
			else
			{
				return string.Format ("Ce code TVA ne peut pas être supprimé,<br/>car il est utilisé dans {0} écriture{1}.", count.ToString (), (count>1)?"s":"");
			}
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