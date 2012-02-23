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
	/// Données éditables pour un libellé usuel de la comptabilité.
	/// </summary>
	public class LibellésEditionLine : AbstractEditionLine
	{
		public LibellésEditionLine(AbstractController controller)
			: base (controller)
		{
			this.datas.Add (ColumnType.Libellé,   new EditionData (this.controller, this.ValidateTitle));
			this.datas.Add (ColumnType.Permanent, new EditionData (this.controller));
		}


		#region Validators
		private void ValidateTitle(EditionData data)
		{
			Validators.ValidateText (data, "Il manque le libellé");
		}
		#endregion


		public override void EntityToData(AbstractEntity entity)
		{
			var libellé = entity as ComptaLibelléEntity;

			this.SetText (ColumnType.Libellé,   libellé.Libellé);
			this.SetText (ColumnType.Permanent, libellé.Permanent ? "1":"0");
		}

		public override void DataToEntity(AbstractEntity entity)
		{
			var libellé = entity as ComptaLibelléEntity;

			libellé.Libellé   = this.GetText (ColumnType.Libellé);
			libellé.Permanent = this.GetText (ColumnType.Permanent) == "1";
		}
	}
}