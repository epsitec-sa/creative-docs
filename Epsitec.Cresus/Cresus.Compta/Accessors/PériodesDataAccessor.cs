//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Types;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Business;

using Epsitec.Cresus.Compta.Controllers;
using Epsitec.Cresus.Compta.Entities;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.Accessors
{
	/// <summary>
	/// Gère l'accès aux périodes comptables de la comptabilité.
	/// </summary>
	public class PériodesDataAccessor : AbstractDataAccessor
	{
		public PériodesDataAccessor(AbstractController controller)
			: base (controller)
		{
			this.StartCreationData ();
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
				return this.comptaEntity.Périodes.Count;
			}
		}

		public override AbstractEntity GetEditionData(int row)
		{
			if (row < 0 || row >= this.comptaEntity.Périodes.Count)
			{
				return null;
			}
			else
			{
				return this.comptaEntity.Périodes[row];
			}
		}

		public override FormattedText GetText(int row, ColumnType column, bool all = false)
		{
			var périodes = comptaEntity.Périodes;

			if (row < 0 || row >= périodes.Count)
			{
				return FormattedText.Null;
			}

			var période = périodes[row];

			switch (column)
			{
				case ColumnType.Utilise:
					return (période == this.mainWindowController.Période) ? PériodesDataAccessor.PériodeCourante : PériodesDataAccessor.AutrePériode;

				case ColumnType.DateDébut:
					return période.DateDébut.ToString ();

				case ColumnType.DateFin:
					return période.DateFin.ToString ();

				case ColumnType.Titre:
					return période.Description;

				case ColumnType.Résumé:
					return période.GetJournalSummary (null);

				default:
					return FormattedText.Null;
			}
		}


		public override int InsertionPointRow
		{
			get
			{
				var text = this.editionData[0].GetText (ColumnType.DateDébut);

				Date date;
				PériodesDataAccessor.ParseDate (text, out date);

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


		public override void InsertEditionData(int index)
		{
			var newData = new PériodesEditionLine (this.controller);

			if (index == -1)
			{
				this.editionData.Add (newData);
			}
			else
			{
				this.editionData.Insert (index, newData);
			}

			this.countEditedRow = this.editionData.Count;
		}

		public override void StartCreationData()
		{
			this.editionData.Clear ();
			this.editionData.Add (new PériodesEditionLine (this.controller));
			this.PrepareEditionLine (0);

			this.firstEditedRow = -1;
			this.countEditedRow = 1;

			this.isModification = false;
			this.justCreated = false;
		}

		public override void ResetCreationData()
		{
			if (this.justCreated)
			{
				this.PrepareEditionLine (0);

				while (this.editionData.Count > 1)
				{
					this.editionData.RemoveAt (1);
				}

				this.countEditedRow = 1;
			}
		}

		public override void StartModificationData(int row)
		{
			this.editionData.Clear ();

			this.firstEditedRow = row;
			this.countEditedRow = 0;

			if (row >= 0 && row < this.comptaEntity.Périodes.Count)
			{
				var data = new PériodesEditionLine (this.controller);
				var période = this.comptaEntity.Périodes[row];
				data.EntityToData (période);

				this.editionData.Add (data);
				this.countEditedRow++;
			}

			this.initialCountEditedRow = this.countEditedRow;
			this.isModification = true;
			this.justCreated = false;
		}

		public override void UpdateEditionData()
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

			foreach (var data in this.editionData)
			{
				var période = this.CreatePériode ();
				data.DataToEntity (période);

				int row = this.GetSortedRow (période.DateDébut);
				this.comptaEntity.Périodes.Insert (row, période);

				if (firstRow == -1)
				{
					firstRow = row;
				}
			}

			this.firstEditedRow = firstRow;
		}

		private void UpdateModificationData()
		{
			int row = this.firstEditedRow;

			var période = this.comptaEntity.Périodes[row];
			this.editionData[0].DataToEntity (période);
		}

		public override void RemoveModificationData()
		{
			if (this.isModification)
			{
				for (int row = this.firstEditedRow+this.countEditedRow-1; row >= this.firstEditedRow; row--)
                {
					var période = this.comptaEntity.Périodes[row];
					this.DeletePériode (période);
					this.comptaEntity.Périodes.RemoveAt (row);
                }

				this.SearchUpdate ();
				this.StartCreationData ();
			}
		}


		private ComptaPériodeEntity CreatePériode()
		{
			this.controller.MainWindowController.SetDirty ();

			if (this.businessContext == null)
			{
				return new ComptaPériodeEntity ();
			}
			else
			{
				return this.businessContext.CreateEntity<ComptaPériodeEntity> ();
			}
		}

		private void DeletePériode(ComptaPériodeEntity période)
		{
			this.controller.MainWindowController.SetDirty ();

			if (this.businessContext == null)
			{
				// rien à faire
			}
			else
			{
				this.businessContext.DeleteEntity (période);
			}
		}


		private bool HasCorrectOrder(int row)
		{
			var périodes = this.comptaEntity.Périodes;
			var période = périodes[row];
			return this.HasCorrectOrder (row, période.DateDébut);
		}

		private bool HasCorrectOrder(int row, Date date)
		{
			var périodes = this.comptaEntity.Périodes;

			if (row > 0 && this.Compare (row-1, date) > 0)
			{
				return false;
			}

			if (row < périodes.Count-1 && this.Compare (row+1, date) < 0)
			{
				return false;
			}

			return true;
		}

		private int Compare(int row, Date date)
		{
			var périodes = this.comptaEntity.Périodes;
			var période = périodes[row];
			return période.DateDébut.CompareTo (date);
		}

		private int GetSortedRow(Date date)
		{
			var périodes = this.comptaEntity.Périodes;

			for (int row = périodes.Count-1; row >= 0; row--)
            {
				var période = périodes[row];

				if (période.DateDébut <= date)
				{
					return row+1;
				}
            }

			return 0;
		}


		public static bool ParseDate(FormattedText text, out Date date)
		{
			//	Transforme un texte en une date valide pour la comptabilité.
			System.DateTime d;

			if (System.DateTime.TryParse (text.ToSimpleText (), System.Threading.Thread.CurrentThread.CurrentCulture, System.Globalization.DateTimeStyles.AssumeLocal | System.Globalization.DateTimeStyles.AllowWhiteSpaces, out d))
			{
				date = new Date (d);
				return true;
			}
			else
			{
				date = Date.Today;
				return false;
			}
		}


		public static readonly FormattedText	PériodeCourante = TextFormatter.FormatText ("Oui").ApplyBold ();
		public static readonly FormattedText	AutrePériode    = TextFormatter.FormatText ("Non");
	}
}