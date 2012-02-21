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
	/// Données éditables pour un générateur de numéros de pièces de la comptabilité.
	/// </summary>
	public class PiècesEditionLine : AbstractEditionLine
	{
		public PiècesEditionLine(AbstractController controller)
			: base (controller)
		{
			this.datas.Add (ColumnType.Nom,         new EditionData (this.controller, this.ValidateNom));
			this.datas.Add (ColumnType.Préfixe,     new EditionData (this.controller));
			this.datas.Add (ColumnType.Numéro,      new EditionData (this.controller, this.ValidateNuméro));
			this.datas.Add (ColumnType.Postfixe,    new EditionData (this.controller));
			this.datas.Add (ColumnType.SépMilliers, new EditionData (this.controller));
			this.datas.Add (ColumnType.Digits,      new EditionData (this.controller, this.ValidateDigits));
			this.datas.Add (ColumnType.Incrément,   new EditionData (this.controller, this.ValidateIncrément));
		}


		#region Validators
		private void ValidateNom(EditionData data)
		{
			Validators.ValidateText (data, "Il manque le nom de l'utilisateur");
		}

		private void ValidateNuméro(EditionData data)
		{
			data.ClearError ();

			if (data.HasText)
			{
				int n;
				if (int.TryParse (data.Text.ToSimpleText (), out n))
				{
					if (n >= 1 && n <= 1000000000)
					{
						return;
					}
				}

				data.Error = "Vous devez donner un numéro compris entre 1 et 1'000'000'000";
			}
			else
			{
				data.Error = "Il manque le numéro";
			}
		}

		private void ValidateDigits(EditionData data)
		{
			data.ClearError ();

			if (data.HasText)
			{
				int n;
				if (int.TryParse (data.Text.ToSimpleText (), out n))
				{
					if (n >= 1 && n <= 9)
					{
						return;
					}
				}

				data.Error = "Vous devez donner un nombre de chiffres compris entre 1 et 9";
			}
		}

		private void ValidateIncrément(EditionData data)
		{
			data.ClearError ();

			if (data.HasText)
			{
				int n;
				if (int.TryParse (data.Text.ToSimpleText (), out n))
				{
					if (n >= 1 && n <= 1000000)
					{
						return;
					}
				}

				data.Error = "Vous devez donner un incrément compris entre 1 et 1'000'000";
			}
			else
			{
				data.Error = "Il manque l'incrément";
			}
		}
		#endregion


		public override void EntityToData(AbstractEntity entity)
		{
			var pièce = entity as ComptaPièceEntity;

			this.SetText (ColumnType.Nom,         pièce.Nom);
			this.SetText (ColumnType.Préfixe,     pièce.Préfixe);
			this.SetText (ColumnType.Numéro,      Converters.IntToString (pièce.Numéro));
			this.SetText (ColumnType.Postfixe,    pièce.Postfixe);
			this.SetText (ColumnType.SépMilliers, pièce.SépMilliers);
			this.SetText (ColumnType.Digits,      (pièce.Digits == 0) ? FormattedText.Empty : Converters.IntToString (pièce.Digits));
			this.SetText (ColumnType.Incrément,   Converters.IntToString (pièce.Incrément));
		}

		public override void DataToEntity(AbstractEntity entity)
		{
			var pièce = entity as ComptaPièceEntity;

			pièce.Nom         = this.GetText (ColumnType.Nom);
			pièce.Préfixe     = this.GetText (ColumnType.Préfixe);
			pièce.Numéro      = Converters.ParseInt (this.GetText (ColumnType.Numéro)).GetValueOrDefault (1);
			pièce.Postfixe    = this.GetText (ColumnType.Postfixe);
			pièce.SépMilliers = this.GetText (ColumnType.SépMilliers);
			pièce.Digits      = Converters.ParseInt (this.GetText (ColumnType.Digits)).GetValueOrDefault (0);
			pièce.Incrément   = Converters.ParseInt (this.GetText (ColumnType.Incrément)).GetValueOrDefault (1);
		}
	}
}