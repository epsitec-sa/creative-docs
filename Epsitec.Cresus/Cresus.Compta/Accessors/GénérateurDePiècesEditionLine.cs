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
	public class GénérateurDePiècesEditionLine : AbstractEditionLine
	{
		public GénérateurDePiècesEditionLine(AbstractController controller)
			: base (controller)
		{
			this.datas.Add (ColumnType.Nom,         new EditionData (this.controller, this.ValidateNom));
			this.datas.Add (ColumnType.Préfixe,     new EditionData (this.controller));
			this.datas.Add (ColumnType.Numéro,      new EditionData (this.controller, this.ValidateNuméro));
			this.datas.Add (ColumnType.Suffixe,    new EditionData (this.controller));
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
			var generator = entity as ComptaGénérateurDePiècesEntity;

			this.SetText (ColumnType.Nom,         generator.Nom);
			this.SetText (ColumnType.Préfixe,     generator.Préfixe);
			this.SetText (ColumnType.Numéro,      Converters.IntToString (generator.Numéro));
			this.SetText (ColumnType.Suffixe,     generator.Suffixe);
			this.SetText (ColumnType.SépMilliers, generator.SépMilliers);
			this.SetText (ColumnType.Digits,      (generator.Digits == 0) ? FormattedText.Empty : Converters.IntToString (generator.Digits));
			this.SetText (ColumnType.Incrément,   Converters.IntToString (generator.Incrément));
		}

		public override void DataToEntity(AbstractEntity entity)
		{
			var generator = entity as ComptaGénérateurDePiècesEntity;

			generator.Nom         = this.GetText (ColumnType.Nom);
			generator.Préfixe     = this.GetText (ColumnType.Préfixe);
			generator.Numéro      = Converters.ParseInt (this.GetText (ColumnType.Numéro)).GetValueOrDefault (1);
			generator.Suffixe     = this.GetText (ColumnType.Suffixe);
			generator.SépMilliers = this.GetText (ColumnType.SépMilliers);
			generator.Digits      = Converters.ParseInt (this.GetText (ColumnType.Digits)).GetValueOrDefault (0);
			generator.Incrément   = Converters.ParseInt (this.GetText (ColumnType.Incrément)).GetValueOrDefault (1);
		}
	}
}