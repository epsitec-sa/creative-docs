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
			this.dataDict.Add (ColumnType.Nom,       new EditionData (this.controller, this.ValidateNom));
			this.dataDict.Add (ColumnType.Préfixe,   new EditionData (this.controller));
			this.dataDict.Add (ColumnType.Suffixe,   new EditionData (this.controller));
			this.dataDict.Add (ColumnType.Format,    new EditionData (this.controller, this.ValidateFormat));
			this.dataDict.Add (ColumnType.Numéro,    new EditionData (this.controller, this.ValidateNuméro));
			this.dataDict.Add (ColumnType.Incrément, new EditionData (this.controller, this.ValidateIncrément));
		}


		#region Validators
		private void ValidateNom(EditionData data)
		{
			data.ClearError ();

			if (data.HasText)
			{
				var generator = this.compta.PiècesGenerator.Where (x => x.Nom == data.Text).FirstOrDefault ();
				if (generator == null)
				{
					return;
				}

				var himself = (this.controller.DataAccessor.JustCreated || this.controller.EditorController.Duplicate) ? null : this.controller.DataAccessor.GetEditionEntity (this.controller.DataAccessor.FirstEditedRow) as ComptaPiècesGeneratorEntity;
				if (himself != null && himself.Nom == data.Text)
				{
					return;
				}

				data.Error = "Ce nom de générateur existe déjà";
			}
			else
			{
				data.Error = "Il manque le nom du générateur de numéros de pièces";
			}
		}

		private void ValidateFormat(EditionData data)
		{
			data.ClearError ();

			if (data.HasText)
			{
				var n = Converters.ParseInt (data.Text);
				if (n.HasValue)
				{
					if (n.Value < 0 || n.Value > 9)
					{
						data.Error = "Vous devez donner un nombre minimum de chiffres compris entre 1 et 9";
					}
				}
				else
				{
					if (!data.Text.ToSimpleText ().Contains ('#'))
					{
						data.Error = "Le format doit contenir au moins un '#', pour indiquer la position du numéro<br/>Exemples: \"#\", \"A##/d\", \"##-###.P\"";
					}
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
			this.SetText (ColumnType.Préfixe,   generator.Préfixe);
			this.SetText (ColumnType.Suffixe,   generator.Suffixe);
			this.SetText (ColumnType.Format,    generator.Format);
			this.SetText (ColumnType.Numéro,    Converters.IntToString (generator.Numéro));
			this.SetText (ColumnType.Incrément, Converters.IntToString (generator.Incrément));
		}

		public override void DataToEntity(AbstractEntity entity)
		{
			var generator = entity as ComptaPiècesGeneratorEntity;

			generator.Nom       = this.GetText (ColumnType.Nom);
			generator.Préfixe   = this.GetText (ColumnType.Préfixe);
			generator.Suffixe   = this.GetText (ColumnType.Suffixe);
			generator.Format    = this.GetText (ColumnType.Format).ToSimpleText ();
			generator.Numéro    = Converters.ParseInt (this.GetText (ColumnType.Numéro)).GetValueOrDefault (1);
			generator.Incrément = Converters.ParseInt (this.GetText (ColumnType.Incrément)).GetValueOrDefault (1);
		}
	}
}