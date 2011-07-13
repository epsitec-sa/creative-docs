//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;

using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Epsitec.Cresus.Core.Data.Extraction
{
	public class EntityDataColumn
	{
		public EntityDataColumn(LambdaExpression lambda, EntityDataColumnConverter converter)
		{
			this.lambda = lambda;
			this.converter = converter;
			this.name = TextFormatter.FormatText (EntityInfo.GetFieldCaption (lambda));
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

		public int								Index
		{
			get
			{
				return this.index;
			}
			set
			{
				this.index = value;
			}
		}
		
		public int								NumericIndex
		{
			get
			{
				return this.numericIndex;
			}
			set
			{
				this.numericIndex = value;
			}
		}


		private readonly LambdaExpression		lambda;
		private readonly EntityDataColumnConverter	converter;
		private readonly FormattedText			name;
		private int								index;
		private int								numericIndex;
	}
}
