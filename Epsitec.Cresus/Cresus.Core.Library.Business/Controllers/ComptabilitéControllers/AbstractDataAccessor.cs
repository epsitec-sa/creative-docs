//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Entities;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.ComptabilitéControllers
{
	/// <summary>
	/// Gère l'accès à des données génériques de la comptabilité.
	/// </summary>
	public abstract class AbstractDataAccessor<ColumnType, Entity, Options>
		where Entity : class
		where Options : class
	{
		public AbstractDataAccessor(ComptabilitéEntity comptabilitéEntity)
		{
			this.comptabilitéEntity = comptabilitéEntity;
		}


		public Options AccessorOptions
		{
			get
			{
				return this.options;
			}
		}

		public List<Entity> SortedList
		{
			get
			{
				return this.sortedEntities;
			}
		}

		public int Count
		{
			get
			{
				return this.sortedEntities.Count;
			}
		}

		public int IndexOf(Entity entity)
		{
			return this.sortedEntities.IndexOf (entity);
		}

		public Entity ElementAt(int index)
		{
			if (index == -1)
			{
				return null;
			}
			else
			{
				return this.sortedEntities[index];
			}
		}

		public virtual int Add(Entity entity)
		{
			return 0;
		}

		public virtual void Remove(Entity entity)
		{
		}

		public virtual void UpdateSortedList()
		{
		}

		public virtual FormattedText GetText(int row, ColumnType column)
		{
			return FormattedText.Empty;
		}

		public virtual void SetText(int row, ColumnType column, FormattedText text)
		{
		}


		protected readonly ComptabilitéEntity comptabilitéEntity;
		protected Options options;
		protected List<Entity> sortedEntities;
	}
}