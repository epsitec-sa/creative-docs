//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Support.EntityEngine;

using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Epsitec.Cresus.Core.Metadata
{
	/// <summary>
	/// The <c>LambdaFilter&lt;TEntity&gt;</c> class is used to turn a lambda expression into
	/// an <see cref="IFilter"/> compatible filter, operating on entities of type
	/// <typeparamref name="TEntity"/>.
	/// </summary>
	/// <typeparam name="TEntity">The type of the entity.</typeparam>
	public class LambdaFilter<TEntity> : LambdaFilter
		where TEntity : AbstractEntity
	{
		public LambdaFilter(Expression<System.Func<TEntity, bool>> lambda)
			: base (lambda)
		{
		}
	}
}
