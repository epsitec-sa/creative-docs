//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Types;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Core.Entities;

using Epsitec.Cresus.Compta.Controllers;
using Epsitec.Cresus.Compta.Entities;
using Epsitec.Cresus.Compta.Helpers;

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

			this.compta  = this.controller.ComptaEntity;
			this.période = this.controller.PériodeEntity;

			this.dataDict = new Dictionary<ColumnType, EditionData> ();
		}


		public EditionData GetData(ColumnType columnType)
		{
			if (this.dataDict.ContainsKey (columnType))
			{
				return this.dataDict[columnType];
			}
			else
			{
				return null;
			}
		}

		public void Validate(ColumnType columnType)
		{
			//	Valide le contenu d'une colonne, en l'adaptant éventuellement.
			if (this.dataDict.ContainsKey (columnType))
			{
				this.dataDict[columnType].Validate ();
			}
		}


		public void Clear()
		{
			foreach (var data in this.dataDict.Values)
			{
				data.Text = null;
			}
		}


		protected void SetMontant(ColumnType columnType, decimal? value)
		{
			this.SetText (columnType, Converters.MontantToString (value));
		}

		protected decimal? GetMontant(ColumnType columnType)
		{
			return Converters.ParseMontant (this.GetText (columnType));
		}


		public void SetText(ColumnType columnType, FormattedText text)
		{
			if (!this.dataDict.ContainsKey (columnType))
			{
				this.dataDict.Add (columnType, new EditionData (this.controller));
			}

			this.dataDict[columnType].Text = text;
		}

		public FormattedText GetText(ColumnType columnType)
		{
			if (this.dataDict.ContainsKey (columnType))
			{
				return this.dataDict[columnType].Text;
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
			if (this.dataDict.ContainsKey (columnType))
			{
				return this.dataDict[columnType].Error;
			}
			else
			{
				return FormattedText.Empty;
			}
		}


		public virtual void EntityToData(AbstractEntity entity)
		{
		}

		public virtual void EntityToData(AbstractData data)
		{
		}

		public virtual void DataToEntity(AbstractEntity entity)
		{
		}

		public virtual void DataToEntity(AbstractData data)
		{
		}


		protected readonly AbstractController						controller;
		protected readonly ComptaEntity								compta;
		protected readonly ComptaPériodeEntity						période;
		protected readonly Dictionary<ColumnType, EditionData>		dataDict;
	}
}