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
	/// Gère l'accès aux taux de TVA de la comptabilité.
	/// </summary>
	public class TauxTVADataAccessor : AbstractDataAccessor
	{
		public TauxTVADataAccessor(AbstractController controller)
			: base (controller)
		{
			this.searchData = this.mainWindowController.GetSettingsSearchData ("Présentation.TauxTVA.Search");
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
				return this.compta.TauxTVA.Count;
			}
		}


		public override AbstractEntity GetEditionEntity(int row)
		{
			if (row < 0 || row >= this.compta.TauxTVA.Count)
			{
				return null;
			}
			else
			{
				return this.compta.TauxTVA[row];
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
				return this.compta.TauxTVA.IndexOf (entity as ComptaTauxTVAEntity);
			}
		}


		public override FormattedText GetText(int row, ColumnType column, bool all = false)
		{
			var tauxTVA = compta.TauxTVA;

			if (row < 0 || row >= tauxTVA.Count)
			{
				return FormattedText.Null;
			}

			var taux = tauxTVA[row];

			switch (column)
			{
				case ColumnType.Nom:
					return taux.Nom;

				case ColumnType.DateDébut:
					return taux.DateDébut.ToString ();

				case ColumnType.DateFin:
					return taux.DateFin.ToString ();

				case ColumnType.Taux:
					return Converters.PercentToString (taux.Taux);

				case ColumnType.ParDéfaut:
					return taux.ParDéfaut ? "Oui" : "";

				default:
					return FormattedText.Null;
			}
		}


		public override void InsertEditionLine(int index)
		{
			var newData = new TauxTVAEditionLine (this.controller);

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
			this.editionLine.Add (new TauxTVAEditionLine (this.controller));
			this.PrepareEditionLine (0);

			this.firstEditedRow = -1;
			this.countEditedRow = 1;

			this.isCreation = true;
			this.isModification = false;
			this.justCreated = false;

			this.controller.EditorController.UpdateFieldsEditionData ();
		}

		public override void StartModificationLine(int row)
		{
			this.editionLine.Clear ();

			this.firstEditedRow = row;
			this.countEditedRow = 0;

			if (row >= 0 && row < this.compta.TauxTVA.Count)
			{
				var data = new TauxTVAEditionLine (this.controller);
				var tauxTVA = this.compta.TauxTVA[row];
				data.EntityToData (tauxTVA);

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
				var tauxTVA = this.CreateTauxTVA ();
				data.DataToEntity (tauxTVA);

				this.compta.TauxTVA.Add (tauxTVA);

				if (firstRow == -1)
				{
					firstRow = this.compta.TauxTVA.Count-1;
				}
			}

			this.firstEditedRow = firstRow;
		}

		private void UpdateModificationData()
		{
			int row = this.firstEditedRow;

			var tauxTVA = this.compta.TauxTVA[row];
			this.editionLine[0].DataToEntity (tauxTVA);
		}


		public override FormattedText GetRemoveModificationLineError()
		{
			var tauxTVA = this.compta.TauxTVA[this.firstEditedRow];
			int count = this.compta.ListesTVA.Where (x => x.Taux.Contains (tauxTVA)).Count ();

			if (count == 0)
			{
				return FormattedText.Empty;  // ok
			}
			else
			{
				return string.Format ("Ce taux de TVA ne peut pas être supprimé,<br/>car il est utilisé dans {0} liste{1} de taux.", count.ToString (), (count>1)?"s":"");
			}
		}

		public override FormattedText GetRemoveModificationLineQuestion()
		{
			var tauxTVA = this.compta.TauxTVA[this.firstEditedRow];
			return string.Format ("Voulez-vous supprimer le taux de TVA \"{0}\" ?", tauxTVA.Nom);
		}

		public override void RemoveModificationLine()
		{
			if (this.isModification)
			{
				for (int row = this.firstEditedRow+this.countEditedRow-1; row >= this.firstEditedRow; row--)
                {
					var tauxTVA = this.compta.TauxTVA[row];
					this.DeleteTauxTVA (tauxTVA);
					this.compta.TauxTVA.RemoveAt (row);
                }

				if (this.firstEditedRow >= this.compta.TauxTVA.Count)
				{
					this.firstEditedRow = this.compta.TauxTVA.Count-1;
				}
			}
		}


		public override bool MoveEditionLine(int direction)
		{
			if (this.IsMoveEditionLineEnable (direction))
			{
				var t1 = this.compta.TauxTVA[this.firstEditedRow];
				var t2 = this.compta.TauxTVA[this.firstEditedRow+direction];

				this.compta.TauxTVA[this.firstEditedRow] = t2;
				this.compta.TauxTVA[this.firstEditedRow+direction] = t1;

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


		private ComptaTauxTVAEntity CreateTauxTVA()
		{
			this.controller.MainWindowController.SetDirty ();

			ComptaTauxTVAEntity tauxTVA;

			if (this.businessContext == null)
			{
				tauxTVA = new ComptaTauxTVAEntity ();
			}
			else
			{
				tauxTVA = this.businessContext.CreateEntity<ComptaTauxTVAEntity> ();
			}

			return tauxTVA;
		}

		private void DeleteTauxTVA(ComptaTauxTVAEntity tauxTVA)
		{
			this.controller.MainWindowController.SetDirty ();

			if (this.businessContext == null)
			{
				// rien à faire
			}
			else
			{
				this.businessContext.DeleteEntity (tauxTVA);
			}
		}
	}
}