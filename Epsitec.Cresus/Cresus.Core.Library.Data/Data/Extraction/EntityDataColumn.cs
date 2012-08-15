//	Copyright © 2011-2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;

using Epsitec.Cresus.DataLayer.Expressions;

using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Epsitec.Cresus.Core.Data.Extraction
{
	/// <summary>
	/// The <c>EntityDataColumn</c> class defines a column (i.e. a field transformed to simple
	/// data) of an entity.
	/// </summary>
	public sealed class EntityDataColumn
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="EntityDataColumn"/> class. This should
		/// not be called directly. Use <see cref="EntityDataMetadataRecorder.Column"/> instead.
		/// </summary>
		/// <param name="expression">The lambda expression (as an expression, not as compiled code).</param>
		/// <param name="sortOrder">The sort order.</param>
		/// <param name="name">The name associated with the column.</param>
		internal EntityDataColumn(LambdaExpression expression, SortOrder sortOrder, FormattedText name)
		{
			this.expression = expression;
			this.sortOrder  = sortOrder;
			this.name       = name.IsNotNull () ? name : TextFormatter.FormatText (EntityInfo.GetFieldCaption (expression));
		}



		public SortOrder						SortOrder
		{
			get
			{
				return this.sortOrder;
			}
		}

		
		public LambdaExpression					Expression
		{
			get
			{
				return this.expression;
			}
		}


		private readonly LambdaExpression		expression;
		private readonly FormattedText			name;
		private readonly SortOrder				sortOrder;
	}
}
