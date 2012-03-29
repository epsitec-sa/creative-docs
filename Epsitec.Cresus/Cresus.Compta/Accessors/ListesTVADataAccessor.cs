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
	/// Gère l'accès aux listes de taux de TVA de la comptabilité.
	/// </summary>
	public class ListesTVADataAccessor : AbstractDataAccessor
	{
		public ListesTVADataAccessor(AbstractController controller)
			: base (controller)
		{
			this.searchData = this.mainWindowController.GetSettingsSearchData ("Présentation.ListesTVA.Search");
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
				return this.compta.ListesTVA.Count;
			}
		}


		public override AbstractEntity GetEditionEntity(int row)
		{
			if (row < 0 || row >= this.compta.ListesTVA.Count)
			{
				return null;
			}
			else
			{
				return this.compta.ListesTVA[row];
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
				return this.compta.ListesTVA.IndexOf (entity as ComptaListeTVAEntity);
			}
		}


		public override FormattedText GetText(int row, ColumnType column, bool all = false)
		{
			var listesTVA = compta.ListesTVA;

			if (row < 0 || row >= listesTVA.Count)
			{
				return FormattedText.Null;
			}

			var listeTVA = listesTVA[row];

			switch (column)
			{
				case ColumnType.Nom:
					return listeTVA.Nom;

				case ColumnType.Taux:
					return listeTVA.SummaryTaux;

				default:
					return FormattedText.Null;
			}
		}


		public override void InsertEditionLine(int index)
		{
			var newData = new ListesTVAEditionLine (this.controller);

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
			this.editionLine.Add (new ListesTVAEditionLine (this.controller));
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
			var array = this.editionLine[line].GetArray (ColumnType.Taux);

			array.SetBool    (0, ColumnType.ParDéfaut, true);
			array.SetDate    (0, ColumnType.Date, new Date (Date.Today.Year, 1, 1));
			array.SetPercent (0, ColumnType.Taux, 0.05m);

			base.PrepareEditionLine (line);
		}

		public override void StartModificationLine(int row)
		{
			this.editionLine.Clear ();

			this.firstEditedRow = row;
			this.countEditedRow = 0;

			if (row >= 0 && row < this.compta.ListesTVA.Count)
			{
				var data = new ListesTVAEditionLine (this.controller);
				var listeTVA = this.compta.ListesTVA[row];
				data.EntityToData (listeTVA);

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
				var listeTVA = this.CreateListeTVA ();
				data.DataToEntity (listeTVA);

				this.compta.ListesTVA.Add (listeTVA);

				if (firstRow == -1)
				{
					firstRow = this.compta.ListesTVA.Count-1;
				}
			}

			this.firstEditedRow = firstRow;
		}

		private void UpdateModificationData()
		{
			int row = this.firstEditedRow;

			var listeTVA = this.compta.ListesTVA[row];
			this.editionLine[0].DataToEntity (listeTVA);
		}


		public override FormattedText GetRemoveModificationLineError()
		{
			var liste = this.compta.ListesTVA[this.firstEditedRow];
			int count = this.compta.CodesTVA.Where (x => x.ListeTaux == liste).Count ();

			if (count == 0)
			{
				return FormattedText.Empty;  // ok
			}
			else
			{
				return string.Format ("Cette liste de taux de TVA ne peut pas être supprimée,<br/>car elle est utilisée dans {0} code{1} TVA.", count.ToString (), (count>1)?"s":"");
			}
		}

		public override FormattedText GetRemoveModificationLineQuestion()
		{
			var liste = this.compta.ListesTVA[this.firstEditedRow];
			return string.Format ("Voulez-vous supprimer la liste de taux de TVA \"{0}\" ?", liste.Nom);
		}

		public override void RemoveModificationLine()
		{
			if (this.isModification)
			{
				for (int row = this.firstEditedRow+this.countEditedRow-1; row >= this.firstEditedRow; row--)
                {
					var listeTVA = this.compta.ListesTVA[row];
					this.DeleteListeTVA (listeTVA);
					this.compta.ListesTVA.RemoveAt (row);
                }

				if (this.firstEditedRow >= this.compta.ListesTVA.Count)
				{
					this.firstEditedRow = this.compta.ListesTVA.Count-1;
				}
			}
		}


		public override bool MoveEditionLine(int direction)
		{
			if (this.IsMoveEditionLineEnable (direction))
			{
				var t1 = this.compta.ListesTVA[this.firstEditedRow];
				var t2 = this.compta.ListesTVA[this.firstEditedRow+direction];

				this.compta.ListesTVA[this.firstEditedRow] = t2;
				this.compta.ListesTVA[this.firstEditedRow+direction] = t1;

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


		private ComptaListeTVAEntity CreateListeTVA()
		{
			this.controller.MainWindowController.SetDirty ();

			ComptaListeTVAEntity listeTVA;

			if (this.businessContext == null)
			{
				listeTVA = new ComptaListeTVAEntity ();
			}
			else
			{
				listeTVA = this.businessContext.CreateEntity<ComptaListeTVAEntity> ();
			}

			return listeTVA;
		}

		private void DeleteListeTVA(ComptaListeTVAEntity listeTVA)
		{
			this.controller.MainWindowController.SetDirty ();

			if (this.businessContext == null)
			{
				// rien à faire
			}
			else
			{
				this.businessContext.DeleteEntity (listeTVA);
			}
		}
	}
}