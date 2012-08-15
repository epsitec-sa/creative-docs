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
		/// <param name="lambda">The lambda expression (as an expression, not as compiled code).</param>
		/// <param name="converter">The converter.</param>
		/// <param name="sortOrder">The sort order.</param>
		internal EntityDataColumn(LambdaExpression lambda, EntityDataColumnConverter converter, SortOrder sortOrder)
		{
			this.lambda    = lambda;
			this.converter = converter;
			this.sortOrder = sortOrder;
			this.name      = TextFormatter.FormatText (EntityInfo.GetFieldCaption (lambda));
		}



		public EntityDataColumnConverter		Converter
		{
			get
			{
				return this.converter;
			}
		}

		public EntityDataType					DataType
		{
			get
			{
				return this.converter.DataType;
			}
		}

		public bool								IsNumeric
		{
			get
			{
				return this.converter.IsNumeric;
			}
		}

		public SortOrder						SortOrder
		{
			get
			{
				return this.sortOrder;
			}
		}

		
		public int								TextualIndex
		{
			get
			{
				return this.textualIndex;
			}
			internal set
			{
				this.textualIndex = value;
			}
		}
		
		public int								NumericIndex
		{
			get
			{
				return this.numericIndex;
			}
			internal set
			{
				this.numericIndex = value;
			}
		}

		public LambdaExpression					Lambda
		{
			get
			{
				return this.lambda;
			}
		}


		private readonly LambdaExpression			lambda;
		private readonly EntityDataColumnConverter	converter;
		private readonly FormattedText				name;
		private readonly SortOrder					sortOrder;
		
		private int								textualIndex;
		private int								numericIndex;
	}
}
