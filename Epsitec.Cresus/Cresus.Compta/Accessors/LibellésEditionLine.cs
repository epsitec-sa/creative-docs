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
			this.datas.Add (ColumnType.Libellé, new EditionData (this.controller, this.ValidateTitle));
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
			this.SetText (ColumnType.Permanant, libellé.Permanant ? LibellésDataAccessor.Permanant : LibellésDataAccessor.Volatile);
		}

		public override void DataToEntity(AbstractEntity entity)
		{
			var libellé = entity as ComptaLibelléEntity;

			libellé.Libellé = this.GetText (ColumnType.Libellé);

			var s1 = Converters.PreparingForSearh (this.GetText (ColumnType.Permanant));
			var s2 = Converters.PreparingForSearh (LibellésDataAccessor.Permanant);
			libellé.Permanant = (s1 == s2);
		}
	}
}