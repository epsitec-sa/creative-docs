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
		public EntityDataRow(EntityDataMetadata metadata)
		{
			this.textFields = new string[metadata.ColumnCount];
			this.numFields  = new long[metadata.NumericColumnCount];
		}


		public void Fill(EntityDataMetadata metadata, AbstractEntity entity)
		{
			metadata.FillFromEntity (entity, this.textFields, this.numFields);
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


		private readonly string[]		textFields;
		private readonly long[]			numFields;
	}
}