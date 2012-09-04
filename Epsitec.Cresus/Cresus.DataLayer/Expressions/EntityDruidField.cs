//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Marc BETTEX

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Support.Extensions;

using Epsitec.Cresus.Database;

using Epsitec.Cresus.DataLayer.Loader;

using System.Collections.Generic;


namespace Epsitec.Cresus.DataLayer.Expressions
{
	
	
	/// <summary>
	/// The <c>EntityDruidField</c> class represents a field of an <see cref="AbstractEntity"/> in an
	/// <see cref="DataExpression"/> that is referenced by a Druid.
	/// </summary>
	public abstract class EntityDruidField : EntityField
	{
		
		
		/// <summary>
		/// Creates a new <c>EntityDruidField</c>.
		/// </summary>
		/// <param name="entity">The entity that will be referenced by this instance</param>
		/// <param name="fieldId">The id of the field that will be referenced by this instance</param>
		protected EntityDruidField(AbstractEntity entity, Druid fieldId)
			: base (entity)
		{
			fieldId.ThrowIf (id => id.IsEmpty, "fieldId is empty");

			this.fieldId = fieldId;
		}


		/// <summary>
		/// The id of the field referenced by this instance.
		/// </summary>
		public Druid FieldId
		{
			get
			{
				return this.fieldId;
			}
		}


		internal override SqlField CreateSqlField(SqlFieldBuilder builder)
		{
			return builder.BuildEntityField (this.Entity, this.FieldId);
		}


		private readonly Druid fieldId;


	}


}