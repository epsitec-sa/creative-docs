//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Core.Data;
using Epsitec.Cresus.Core.Metadata;

using Epsitec.Cresus.WebCore.Server.Core.Databases;
using Epsitec.Cresus.WebCore.Server.Core.IO;
using Epsitec.Cresus.WebCore.Server.Core.PropertyAccessor;

using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Epsitec.Cresus.WebCore.Server.Core.Extraction
{
	/// <summary>
	/// This class is used to serialize a sequence of entities into an array where each entity has
	/// one row in the array, and where the columns are customizable. Moreover, the format in which
	/// the file is written is customizable by using an ArrayFormat.
	/// </summary>
	internal sealed class ArrayWriter : EntityWriter
	{
		internal ArrayWriter(PropertyAccessorCache properties, DataSetMetadata metadata, IEnumerable<Column> columns, DataSetAccessor accessor, ArrayFormat format)
			: base (metadata, accessor)
		{
			this.columns = columns.ToList ();
			this.properties = properties;
			this.format = format;
		}


		protected override string GetExtension()
		{
			return this.format.Extension;
		}

		protected override void WriteStream(Stream stream)
		{
			var headers = this.GetHeaders ();
			var rows    = this.GetRows ();

			if (this.RemoveDuplicates)
			{
				rows = rows.Distinct (ListEqualityComparer.GetComparer<string> ()).ToList ();
			}

			this.format.Write (stream, headers, rows);
		}




		private IList<string> GetHeaders()
		{
			return this.columns
				.Select (c => c.MetaData.GetColumnTitle ().ToSimpleText ())
				.ToList ();
		}

		private IList<IList<string>> GetRows()
		{
			var columnAccessors = this.GetColumnAccessors ();
			var items = this.Accessor.GetAllItems ();

			var dataContext = this.Accessor.IsolatedDataContext;
			var expressions = this.columns.Select (c => c.LambdaExpression);
			dataContext.LoadRelatedData (items, expressions);

			return items
				.Select (e => this.GetCells (columnAccessors, e))
				.ToList ();
		}

		private IList<AbstractPropertyAccessor> GetColumnAccessors()
		{
			return this.columns
				.Select (c => this.properties.Get (c.LambdaExpression))
				.ToList ();
		}

		private IList<string> GetCells(IList<AbstractPropertyAccessor> columnAccessors, AbstractEntity entity)
		{
			return columnAccessors
				.Select (c => this.GetCell (c, entity))
				.ToList ();
		}

		private string GetCell(AbstractPropertyAccessor columnAccessor, AbstractEntity entity)
		{
			var value = columnAccessor.GetValue (entity);

			var fieldType = columnAccessor.FieldType;
			if (fieldType == FieldType.EntityReference || fieldType == FieldType.EntityCollection)
			{
				throw new System.NotImplementedException ("Unsupported field type");
			}

			return FieldIO.ConvertToString (value, fieldType);
		}


		private readonly IEnumerable<Column>	columns;
		private readonly PropertyAccessorCache	properties;
		private readonly ArrayFormat			format;
	}
}
