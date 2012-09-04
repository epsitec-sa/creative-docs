//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Common.Support.Extensions;

using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Epsitec.Cresus.Core.Metadata
{
	/// <summary>
	/// The <c>IFilter</c> interface gives access to the underlying expression, which
	/// describes the predicate for the filter.
	/// </summary>
	public interface IFilter
	{
		bool IsValid
		{
			get;
		}

		/// <summary>
		/// Gets the expression for this filter, applied to the specified parameter.
		/// The expression is always represented by a predicate returning a boolean.
		/// </summary>
		/// <param name="parameter">The parameter (which may be the body of a lambda).</param>
		/// <returns>The expression for this filter.</returns>
		Expression GetExpression(Expression parameter);
	}
}
