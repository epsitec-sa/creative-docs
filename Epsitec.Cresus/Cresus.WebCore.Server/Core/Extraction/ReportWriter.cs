//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Samuel LOUP, Maintainer: Samuel LOUP

using Epsitec.Common.Pdf.Labels;
using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Data;
using Epsitec.Cresus.Core.Labels;
using Epsitec.Cresus.Core.Metadata;

using System.Collections.Generic;
using System.IO;
using System.Linq;
using Epsitec.Common.Pdf.TextDocument;
using Epsitec.Cresus.WebCore.Server.Core.PropertyAccessor;
using Epsitec.Cresus.WebCore.Server.Core.Databases;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Cresus.WebCore.Server.Core.IO;


namespace Epsitec.Cresus.WebCore.Server.Core.Extraction
{

	internal sealed class ReportWriter : EntityWriter
	{
		public ReportWriter(DataSetMetadata metadata, DataSetAccessor accessor, LabelLayout layout)
			: base (metadata, accessor)
		{	
			this.columns = this.Accessor.DataSetMetadata.EntityTableMetadata.Columns.Select (
								c => new Column (new EntityColumnMetadata (c.Expression, c.Title),c.Id)).ToList ();
			this.layout      = layout;
		}


		protected override string GetExtension()
		{
			return "pdf";
		}

		protected override void WriteStream(Stream stream)
		{
			
			var rows		= this.GetRows ();
			var content		= rows[0];

			var setup		= new TextDocumentSetup
							{
								HeaderText = rows[0][0]
							};

			var report		= this.GetReport (setup);


			report.GeneratePdf (stream, rows[0][2]);
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
				.Select (c => AbstractPropertyAccessor.Create(c.LambdaExpression,c.ColumnId))
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

		private TextDocument GetReport(TextDocumentSetup setup)
		{
			var exportPdfInfo   = this.layout.GetExportPdfInfo ();
			var labelPageLayout = this.layout.GetLabelPageLayout ();
			var labelRenderer   = this.layout.GetLabelRenderer ();

			return new TextDocument (exportPdfInfo, setup);
		}

		private readonly IEnumerable<Column>	columns;
		private readonly PropertyAccessorCache	properties;
		private readonly LabelLayout			layout;
	}
}
