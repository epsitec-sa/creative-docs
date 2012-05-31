//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Marc BETTEX

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.DataLayer.Loader;

using System;


namespace Epsitec.Cresus.DataLayer.Expressions
{


	/// <summary>
	/// The <c>ValueField</c> class represents a field of an <see cref="AbstractEntity"/> in an
	/// <see cref="Expression"/> that targets a value field of the entity.
	/// </summary>
	public sealed class ValueField : EntityDruidField
	{


		public ValueField(AbstractEntity entity, Druid fieldId)
			: base (entity, fieldId)
		{
		}


		internal override void CheckField(FieldChecker checker)
		{
			checker.CheckValueField (this.Entity, this.FieldId);
		}


		public static ValueField Create<T, V>(T entity, System.Linq.Expressions.Expression<Func<T, V>> expression)
			where T : AbstractEntity, new ()
		{
			return new ValueField (entity, EntityInfo.GetFieldDruid (expression));
		}
	
	
	}


}
