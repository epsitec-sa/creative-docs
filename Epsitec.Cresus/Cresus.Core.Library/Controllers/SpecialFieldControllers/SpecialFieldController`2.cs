using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Widgets;

using Epsitec.Cresus.Core.Business;

using System;
using System.Linq.Expressions;

namespace Epsitec.Cresus.Core.Controllers.SpecialFieldControllers
{
	public abstract class SpecialFieldController<TEntity, TField> : SpecialFieldController
		where TEntity : AbstractEntity, new ()
	{
		protected SpecialFieldController(BusinessContext businessContext, TEntity entity, Expression<Func<TEntity, TField>> lambda)
			: base (businessContext)
		{
			this.entity = entity;
			this.lambda = lambda;
		}

		/// <summary>
		/// Gets the value of the field, by executing the lambda against the entity or by getting
		/// the value that was specified.
		/// </summary>
		/// <exception cref="NotSupportedException">If this instance has been created without a value.</exception>
		protected TField GetFieldValue()
		{
			return this.lambda.Compile ().Invoke (this.entity);
		}

		public override sealed Widget GetDesktopField()
		{
			// This override is temporary until the management of special fields controllers is
			// implemented in the desktop version of Cresus.Core.

			throw new NotImplementedException ();
		}

		protected static Expression<Func<TEntity, TField>> CreateLambda()
		{
			// We can't directly throw the exception because then the compiler won't let us convert
			// the lambda to an expression tree. Therefore we wrap the exception throwing within an
			// helper function.

			return e => SpecialFieldController<TEntity, TField>.ThrowIfCalled ();
		}

		private static TField ThrowIfCalled()
		{
			throw new NotSupportedException ();
		}


		protected static Expression<Func<TEntity, TField>> CreateLambda(object value)
		{
			return e => (TField) value;
		}

		private readonly Expression<Func<TEntity, TField>> lambda;

		private readonly TEntity entity;
	}
}
