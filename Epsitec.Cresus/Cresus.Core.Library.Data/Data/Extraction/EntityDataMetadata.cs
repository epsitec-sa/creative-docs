//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;

using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Epsitec.Cresus.Core.Data.Extraction
{
	public sealed class EntityDataMetadata
	{
		internal EntityDataMetadata(IEnumerable<EntityDataColumn> columns)
		{
			this.columns = columns.ToArray ();
			
			this.numericColumnCount = this.columns.Count (x => x.IsNumeric);

			int numericIndex = 0;

			for (int index = 0; index < this.columns.Length; index++)
			{
				var column = this.columns[index];

				if (column.IsNumeric)
				{
					column.Index        = index;
					column.NumericIndex = -1;
				}
				else
				{
					column.Index        = index;
					column.NumericIndex = numericIndex++;
				}
			}

			System.Diagnostics.Debug.Assert (numericIndex == this.NumericColumnCount);
		}

		
		public int							ColumnCount
		{
			get
			{
				return this.columns.Length;
			}
		}

		public int							NumericColumnCount
		{
			get
			{
				return this.numericColumnCount;
			}
		}

		public int GetNumericFieldIndex(int fieldIndex)
		{
			return this.columns[fieldIndex].NumericIndex;
		}


		internal void FillFromEntity(AbstractEntity entity, string[] texts, long[] nums)
		{
			foreach (var column in columns)
			{
				texts[column.Index] = column.Converter.GetText (entity);

				var numIndex = column.NumericIndex;

				if (numIndex >= 0)
				{
					nums[numIndex] = column.Converter.GetNumber (entity);
				}
			}
		}

		private readonly EntityDataColumn[] columns;
		private readonly int				numericColumnCount;
	}
}