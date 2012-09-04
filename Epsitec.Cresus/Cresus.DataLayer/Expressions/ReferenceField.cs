//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Marc BETTEX

using Epsitec.Common.Support;
using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Database;

using Epsitec.Cresus.DataLayer.Loader;

using System;


namespace Epsitec.Cresus.DataLayer.Expressions
{


	/// <summary>
	/// The <c>ReferenceField</c> class represents a field of an <see cref="AbstractEntity"/> in an
	/// <see cref="DataExpression"/> that targets a reference field of the entity.
	/// </summary>
	public sealed class ReferenceField : EntityDruidField
	{


		public ReferenceField(AbstractEntity entity, Druid fieldId)
			: base (entity, fieldId)
		{		
		}


		internal override void CheckField(FieldChecker checker)
		{
			checker.CheckReferenceField (this.Entity, this.FieldId);
		}


		public static ReferenceField Create<T, V>(T entity, System.Linq.Expressions.Expression<Func<T, V>> expression)
			where T : AbstractEntity, new ()
		{
			return new ReferenceField (entity, EntityInfo.GetFieldDruid (expression));
		}
	
	
	}


}
