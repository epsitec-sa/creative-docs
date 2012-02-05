﻿//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Types;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Entities;

using Epsitec.Cresus.Compta.Controllers;
using Epsitec.Cresus.Compta.Entities;

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
			this.datas.Add (ColumnType.DateDébut, new EditionData (this.controller, this.ValidateDate));
			this.datas.Add (ColumnType.DateFin,   new EditionData (this.controller, this.ValidateDate));
		}


		#region Validators
		private void ValidateDate(EditionData data)
		{
			data.ClearError ();

			if (data.HasText)
			{
				Date date;
				if (!PériodesDataAccessor.ParseDate (data.Text, out date))
				{
					data.Error = "La date est incorrecte";
				}
			}
			else
			{
				data.Error = "Il manque la date";
			}
		}
		#endregion


		public override void EntityToData(AbstractEntity entity)
		{
			var période = entity as ComptaPériodeEntity;

			bool sel = (this.controller.MainWindowController.Période == entity);

			this.SetText (ColumnType.Utilise,   sel ? PériodesDataAccessor.PériodeCourante : PériodesDataAccessor.AutrePériode);
			this.SetText (ColumnType.DateDébut, période.DateDébut.ToString ());
			this.SetText (ColumnType.DateFin,   période.DateFin.ToString ());
			this.SetText (ColumnType.Titre,     période.Description);
		}

		public override void DataToEntity(AbstractEntity entity)
		{
			var période = entity as ComptaPériodeEntity;

			if (this.GetText (ColumnType.Utilise).ToSimpleText ().ToLower () == PériodesDataAccessor.PériodeCourante.ToSimpleText ().ToLower ())
			{
				this.controller.MainWindowController.Période = période;
			}

			Date date;

			if (PériodesDataAccessor.ParseDate (this.GetText (ColumnType.DateDébut), out date))
			{
				période.DateDébut = date;
			}

			if (PériodesDataAccessor.ParseDate (this.GetText (ColumnType.DateFin), out date))
			{
				période.DateFin = date;
			}

			période.Description = this.GetText (ColumnType.Titre);
		}
	}
}