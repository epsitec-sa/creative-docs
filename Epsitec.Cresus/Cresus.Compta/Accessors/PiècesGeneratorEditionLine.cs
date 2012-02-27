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
	public class PiècesGeneratorEditionLine : AbstractEditionLine
	{
		public PiècesGeneratorEditionLine(AbstractController controller)
			: base (controller)
		{
			this.datas.Add (ColumnType.Nom,         new EditionData (this.controller, this.ValidateNom));
			this.datas.Add (ColumnType.Format,      new EditionData (this.controller, this.ValidateFormat));
			this.datas.Add (ColumnType.Numéro,      new EditionData (this.controller, this.ValidateNuméro));
			this.datas.Add (ColumnType.Incrément,   new EditionData (this.controller, this.ValidateIncrément));
		}


		#region Validators
		private void ValidateNom(EditionData data)
		{
			Validators.ValidateText (data, "Il manque le nom du générateur de numéros de pièces");
		}

		private void ValidateFormat(EditionData data)
		{
			data.ClearError ();

			if (data.HasText)
			{
				if (!data.Text.ToSimpleText ().Contains ('#'))
				{
					data.Error = "Le format doit contenir au moins un '#', pour indiquer la position du numéro<br/>Exemples: \"#\", \"A##/d\", \"##-###.P\"";
				}
			}
			else
			{
				data.Error = "Il manque le format";
			}
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
			var generator = entity as ComptaPiècesGeneratorEntity;

			this.SetText (ColumnType.Nom,       generator.Nom);
			this.SetText (ColumnType.Format,    generator.Format);
			this.SetText (ColumnType.Numéro,    Converters.IntToString (generator.Numéro));
			this.SetText (ColumnType.Incrément, Converters.IntToString (generator.Incrément));
		}

		public override void DataToEntity(AbstractEntity entity)
		{
			var generator = entity as ComptaPiècesGeneratorEntity;

			generator.Nom       = this.GetText (ColumnType.Nom);
			generator.Format    = this.GetText (ColumnType.Format).ToSimpleText ();
			generator.Numéro    = Converters.ParseInt (this.GetText (ColumnType.Numéro)).GetValueOrDefault (1);
			generator.Incrément = Converters.ParseInt (this.GetText (ColumnType.Incrément)).GetValueOrDefault (1);
		}
	}
}