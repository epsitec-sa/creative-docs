//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Marc BETTEX

using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Support.Extensions;

using System;

using System.Collections.Generic;


namespace Epsitec.Cresus.DataLayer.Expressions
{
	
	
	/// <summary>
	/// The <c>EntityField</c> class represents a field of an <see cref="AbstractEntity"/> in an
	/// <see cref="Expression"/>.
	/// </summary>
	public abstract class EntityField : Value
	{
		
		
		/// <summary>
		/// Creates a new <c>EntityField</c>.
		/// </summary>
		/// <param name="entity">The entity that will be referenced by this instance</param>
		protected EntityField(AbstractEntity entity)
		{
			entity.ThrowIfNull ("entity");

			this.entity = entity;
		}


		/// <summary>
		/// The entity referenced by this instance.
		/// </summary>
		public AbstractEntity Entity
		{
			get
			{
				return this.entity;
			}
		}


		internal override void AddEntities(HashSet<AbstractEntity> entities)
		{
			entities.Add (this.entity);
		}

		
		private readonly AbstractEntity entity;


	}


}