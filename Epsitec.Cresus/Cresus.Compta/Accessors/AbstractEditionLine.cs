//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
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
	/// Données éditables génériques pour une ligne de la comptabilité.
	/// </summary>
	public abstract class AbstractEditionLine
	{
		public AbstractEditionLine(AbstractController controller)
		{
			this.controller = controller;

			this.comptaEntity  = this.controller.ComptaEntity;
			this.périodeEntity = this.controller.PériodeEntity;

			this.datas = new Dictionary<ColumnType, EditionData> ();
		}


		public EditionData GetData(ColumnType columnType)
		{
			if (this.datas.ContainsKey (columnType))
			{
				return this.datas[columnType];
			}
			else
			{
				return null;
			}
		}

		public void Validate(ColumnType columnType)
		{
			//	Valide le contenu d'une colonne, en l'adaptant éventuellement.
			if (this.datas.ContainsKey (columnType))
			{
				this.datas[columnType].Validate ();
			}
		}


		protected void SetMontant(ColumnType columnType, decimal? value)
		{
			if (value.HasValue)
			{
				this.SetText (columnType, value.Value.ToString ("0.00"));
			}
			else
			{
				this.SetText (columnType, FormattedText.Empty);
			}
		}

		protected decimal? GetMontant(ColumnType columnType)
		{
			var text = this.GetText (columnType);

			if (!text.IsNullOrEmpty)
			{
				decimal d;
				if (decimal.TryParse (text.ToSimpleText (), out d))
				{
					return d;
				}
			}

			return null;
		}


		public void SetText(ColumnType columnType, FormattedText text)
		{
			if (!this.datas.ContainsKey (columnType))
			{
				this.datas.Add (columnType, new EditionData (this.controller));
			}

			this.datas[columnType].Text = text;
		}

		public FormattedText GetText(ColumnType columnType)
		{
			if (this.datas.ContainsKey (columnType))
			{
				return this.datas[columnType].Text;
			}
			else
			{
				return FormattedText.Empty;
			}
		}


		public bool HasError(ColumnType columnType)
		{
			return !this.GetError (columnType).IsNullOrEmpty;
		}

		public FormattedText GetError(ColumnType columnType)
		{
			if (this.datas.ContainsKey (columnType))
			{
				return this.datas[columnType].Error;
			}
			else
			{
				return FormattedText.Empty;
			}
		}


		public virtual void EntityToData(AbstractEntity entity)
		{
		}

		public virtual void DataToEntity(AbstractEntity entity)
		{
		}


		protected readonly AbstractController						controller;
		protected readonly ComptaEntity								comptaEntity;
		protected readonly ComptaPériodeEntity						périodeEntity;
		protected readonly Dictionary<ColumnType, EditionData>		datas;
	}
}