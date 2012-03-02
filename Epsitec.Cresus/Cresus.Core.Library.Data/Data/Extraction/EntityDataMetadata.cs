//	Copyright © 2011-2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;

using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Epsitec.Cresus.Core.Data.Extraction
{
	/// <summary>
	/// The <c>EntityDataMetadata</c> class is a collection of <see cref="EntityDataColum"/>
	/// instances.
	/// </summary>
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
					column.TextualIndex = index;
					column.NumericIndex = numericIndex++;
				}
				else
				{
					column.TextualIndex = index;
					column.NumericIndex = -1;
				}
			}

			System.Diagnostics.Debug.Assert (numericIndex == this.NumericColumnCount);
		}

		
		public int							TotalColumnCount
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


		/// <summary>
		/// Gets the index of the numeric field, based on a column index. Some columns
		/// do not show up in the numeric column collection, as they cannot be mapped
		/// to numbers.
		/// </summary>
		/// <param name="fieldIndex">Index of the field.</param>
		/// <returns>The index in the numeric column collection, or <c>-1</c>.</returns>
		public int GetNumericFieldIndex(int fieldIndex)
		{
			return this.columns[fieldIndex].NumericIndex;
		}


		internal void FillFromEntity(AbstractEntity entity, string[] texts, long[] nums)
		{
			foreach (var column in columns)
			{
				texts[column.TextualIndex] = column.Converter.GetText (entity);

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