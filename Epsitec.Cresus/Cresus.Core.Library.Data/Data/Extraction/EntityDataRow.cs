//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Data.Extraction
{
	public sealed class EntityDataRow
	{
		public EntityDataRow(EntityDataMetadata metadata, AbstractEntity entity)
		{
			this.entity     = entity;
			this.textFields = new string[metadata.ColumnCount];
			this.numFields  = new long[metadata.NumericColumnCount];

			if (entity != null)
			{
				metadata.FillFromEntity (entity, this.textFields, this.numFields);
			}
		}


		public AbstractEntity Entity
		{
			get
			{
				return this.entity;
			}
		}


		public string GetTextField(int index)
		{
			return this.textFields[index];
		}

		public string[] GetTextFields(params int[] indexes)
		{
			return indexes.Select (x => this.textFields[x]).ToArray ();
		}

		public long GetNumericField(int index)
		{
			return this.numFields[index];
		}

		public long[] GetNumericFields(params int[] indexes)
		{
			return indexes.Select (x => this.numFields[x]).ToArray ();
		}

		private readonly AbstractEntity			entity;
		private readonly string[]				textFields;
		private readonly long[]					numFields;
	}

	public class EntityDataRowComparer : IComparer<EntityDataRow>
	{

		#region IComparer<EntityDataRow> Members

		public int Compare(EntityDataRow x, EntityDataRow y)
		{
			return string.CompareOrdinal (x.GetTextField (0), y.GetTextField (0));
		}

		#endregion

		public static readonly EntityDataRowComparer Instance = new EntityDataRowComparer ();
	}
}