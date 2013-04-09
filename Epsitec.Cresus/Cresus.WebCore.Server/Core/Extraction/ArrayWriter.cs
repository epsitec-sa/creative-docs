using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Core.Data;
using Epsitec.Cresus.Core.Metadata;

using Epsitec.Cresus.WebCore.Server.Core.PropertyAccessor;

using System;

using System.Collections.Generic;

using System.IO;

using System.Linq;

using Epsitec.Common.Support;


namespace Epsitec.Cresus.WebCore.Server.Core.Extraction
{


	internal class ArrayWriter : EntityWriter
	{


		public ArrayWriter(PropertyAccessorCache properties, DataSetMetadata metadata, DataSetAccessor accessor, ArrayFormat format)
			: base (metadata, accessor)
		{
			this.properties = properties;
			this.format = format;
		}


		public override string Filename
		{
			get
			{
				var rawName = this.Metadata.Command.Caption.DefaultLabel;
				var modName = StringUtils.RemoveDiacritics (rawName.ToLowerInvariant ());
				var extension = this.format.Extension;

				return string.Format ("extraction_{0}.{1}", modName, extension);
			}
		}


		public override Stream GetStream()
		{
			var columns = this.Metadata.EntityTableMetadata.Columns;

			var headers = this.GetHeaders (columns);
			var rows = this.GetRows (columns);

			var stream = new MemoryStream ();

			this.format.Write (stream, headers, rows);

			stream.Position = 0;

			return stream;
		}


		private IList<string> GetHeaders(IEnumerable<EntityColumnMetadata> columns)
		{
			return columns
				.Select (c => c.GetColumnTitle ().ToSimpleText ())
				.ToList ();
		}


		private IList<IList<string>> GetRows(IList<EntityColumnMetadata> columns)
		{
			var columnAccessors = this.GetColumnAccessors (columns);

			return this.Accessor
				.GetAllItems ()
				.Select (e => this.GetCells (columnAccessors, e))
				.ToList ();
		}


		private IList<AbstractPropertyAccessor> GetColumnAccessors(IList<EntityColumnMetadata> columns)
		{
			return columns
				.Select (c => this.properties.Get (c.Expression))
				.ToList ();
		}


		private IList<string> GetCells(IList<AbstractPropertyAccessor> columnAccessors, AbstractEntity entity)
		{
			return columnAccessors
				.Select (ca => this.GetCell (ca, entity))
				.ToList ();
		}


		private string GetCell(AbstractPropertyAccessor columnAccessors, AbstractEntity entity)
		{
			var value = columnAccessors.GetValue (entity);

			if (value != null && !(value is string))
			{
				throw new InvalidOperationException ();
			}

			return (string) value;
		}


		private readonly PropertyAccessorCache properties;


		private readonly ArrayFormat format;


	}


}
