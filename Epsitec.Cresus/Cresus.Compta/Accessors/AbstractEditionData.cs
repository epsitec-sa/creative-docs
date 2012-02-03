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
	/// Données éditables génériques pour la comptabilité.
	/// </summary>
	public abstract class AbstractEditionData
	{
		public AbstractEditionData(AbstractController controller)
		{
			this.controller = controller;
			this.comptaEntity = this.controller.ComptaEntity;
			this.périodeEntity = this.controller.PériodeEntity;

			this.datas = new Dictionary<ColumnType, FormattedText> ();
			this.errors = new Dictionary<ColumnType, FormattedText> ();
		}


		public virtual void Validate(ColumnType columnType)
		{
			//	Valide le contenu d'une colonne, en adaptant éventuellement son contenu.
		}

		public void SetText(ColumnType columnType, FormattedText text)
		{
			this.datas[columnType] = text;
		}

		public FormattedText GetText(ColumnType columnType)
		{
			if (this.datas.ContainsKey (columnType))
			{
				return this.datas[columnType];
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
			if (this.errors.ContainsKey (columnType))
			{
				return this.errors[columnType];
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
		protected readonly Dictionary<ColumnType, FormattedText>	datas;
		protected readonly Dictionary<ColumnType, FormattedText>	errors;
	}
}