//	Copyright © 2007-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;

using Epsitec.Cresus.Database;
using Epsitec.Cresus.DataLayer;

using System.Collections.Generic;


namespace Epsitec.Cresus.DataLayer.Context
{
	
	
	/// <summary>
	/// The <c>EntityDataMapping</c> class maps entity instances with their data
	/// row(s) in the <see cref="System.Data.DataSet"/> associated with the live
	/// <see cref="DataContext"/>.
	/// </summary>
	internal sealed class EntityDataMapping
	{
		
		
		/// <summary>
		/// Initializes a new instance of the <see cref="EntityDataMapping"/> class.
		/// </summary>
		/// <param name="entity">The entity.</param>
		/// <param name="leafEntityId">The entity id.</param>
		/// <param name="rootEntityId">The root entity id.</param>
		public EntityDataMapping(AbstractEntity entity, Druid leafEntityId, Druid rootEntityId)
			: this (entity, leafEntityId, rootEntityId, null)
		{
		}


		public EntityDataMapping(AbstractEntity entity, Druid leafEntityId, Druid rootEntityId, DbKey? rowKey)
		{
			this.entity = entity;

			this.leafEntityId = leafEntityId;
			this.rootEntityId = rootEntityId;

			this.rowKey = rowKey;
		}

		
		/// <summary>
		/// Gets the associated entity.
		/// </summary>
		/// <value>The entity.</value>
		public AbstractEntity Entity
		{
			get
			{
				return this.entity;
			}
		}

		
		/// <summary>
		/// Gets the associated entity id.
		/// </summary>
		/// <value>The entity id.</value>
		public Druid LeafEntityId
		{
			get
			{
				return this.leafEntityId;
			}
		}


		/// <summary>
		/// Gets the associated root entity id.
		/// </summary>
		/// <value>The root entity id.</value>
		public Druid RootEntityId
		{
			get
			{
				return this.rootEntityId;
			}
		}


		/// <summary>
		/// Gets or sets the serialization generation.
		/// </summary>
		/// <value>The serialization generation.</value>
		public long SerialGeneration
		{
			get;
			set;
		}


		/// <summary>
		/// Gets or sets the row key for the associated entity.
		/// </summary>
		/// <exception cref="System.InvalidOperationException">Thrown when a row
		/// key is modified after it has become definitive.</exception>
		/// <value>The row key.</value>
		public DbKey RowKey
		{
			get
			{
				return this.rowKey ?? DbKey.Empty;
			}
			set
			{
				if (this.rowKey != null)
				{
					throw new System.InvalidOperationException ("RowKey cannot be further modified");
				}
				
				this.rowKey = value;
			}
		}


		private readonly AbstractEntity entity;


		private readonly Druid leafEntityId;


		private readonly Druid rootEntityId;
		

		private DbKey? rowKey;
	}
}
