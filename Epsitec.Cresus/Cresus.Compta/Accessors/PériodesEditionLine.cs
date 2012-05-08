//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Types;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Compta.Controllers;
using Epsitec.Cresus.Compta.Entities;
using Epsitec.Cresus.Compta.Helpers;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.Accessors
{
	/// <summary>
	/// Données éditables pour une période comptable de la comptabilité.
	/// </summary>
	public class PériodesEditionLine : AbstractEditionLine
	{
		public PériodesEditionLine(AbstractController controller)
			: base (controller)
		{
			this.dataDict.Add (ColumnType.DateDébut, new EditionData (this.controller, this.ValidateDate));
			this.dataDict.Add (ColumnType.DateFin,   new EditionData (this.controller, this.ValidateDate));
			this.dataDict.Add (ColumnType.Titre,     new EditionData (this.controller));
			this.dataDict.Add (ColumnType.Pièce,     new EditionData (this.controller, this.ValidatePièce));
			this.dataDict.Add (ColumnType.Utilise,   new EditionData (this.controller));
		}


		#region Validators
		private void ValidateDate(EditionData data)
		{
			Validators.ValidateDate (data, emptyAccepted: false);
		}

		private void ValidatePièce(EditionData data)
		{
			data.ClearError ();

			if (data.HasText)
			{
				var pièce = PériodesDataAccessor.GetPiècesGenerator (this.compta, data.Text);
				if (pièce == null)
				{
					data.Error = "Ce générateur de numéros de pièces n'existe pas";
				}
			}
		}
		#endregion


		public override void EntityToData(AbstractEntity entity)
		{
			var période = entity as ComptaPériodeEntity;

			bool sel = (this.controller.MainWindowController.Période == entity);

			this.SetText (ColumnType.Utilise,   sel ? "1":"0");
			this.SetText (ColumnType.DateDébut, période.DateDébut.ToString ());
			this.SetText (ColumnType.DateFin,   période.DateFin.ToString ());
			this.SetText (ColumnType.Titre,     période.Description);
			this.SetText (ColumnType.Pièce, PériodesDataAccessor.GetPiècesGenerator (période));
		}

		public override void DataToEntity(AbstractEntity entity)
		{
			var période = entity as ComptaPériodeEntity;

			if (this.GetText (ColumnType.Utilise) == "1")
			{
				this.controller.MainWindowController.Période = période;
			}

			Date? date;

			date = Converters.ParseDate (this.GetText (ColumnType.DateDébut));
			if (date.HasValue)
			{
				période.DateDébut = date.Value;
			}

			date = Converters.ParseDate (this.GetText (ColumnType.DateFin));
			if (date.HasValue)
			{
				période.DateFin = date.Value;
			}

			période.Description = this.GetText (ColumnType.Titre);
			période.PiècesGenerator = PériodesDataAccessor.GetPiècesGenerator (this.compta, this.GetText (ColumnType.Pièce));
		}
	}
}