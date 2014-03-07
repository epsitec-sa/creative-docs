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
using Epsitec.Common.Pdf.LetterDocument;


namespace Epsitec.Cresus.WebCore.Server.Core.Extraction
{

	internal sealed class LetterDocumentWriter : EntityWriter
	{
		public LetterDocumentWriter(DataSetMetadata metadata, DataSetAccessor accessor, LabelLayout layout)
			: base (metadata, accessor)
		{	
			this.columns = this.Accessor.DataSetMetadata.EntityTableMetadata.Columns.Select (
								c => new Column (new EntityColumnMetadata (c.Expression, c.Title),c.Id)).ToList ();
			this.layout      = layout;
		}


		protected override string GetExtension()
		{
			return "test.pdf";//must be changed: filename not correctly implemented
		}

		protected override void WriteStream(Stream stream)
		{
			
			var rows			= this.GetRows ();
			var template		= rows[0][0];
			var topReference	= rows[0][1];
			var title			= rows[0][2];
			var firstname		= rows[0][3];
			var lastname		= rows[0][4];
			var address			= rows[0][5];
			var p0				= rows[0][6];
			var p1				= rows[0][7];
			var p2				= rows[0][8];

			var setup			= new LetterDocumentSetup ();

			var report			= this.GetReport (setup);

			var addressBuilder = new System.Text.StringBuilder ();
			addressBuilder.Append (rows[0][2]);
			addressBuilder.Append ("<br/>");
			addressBuilder.Append (rows[0][3]);
			addressBuilder.Append (" ");
			addressBuilder.Append (rows[0][4]);
			addressBuilder.Append ("<br/>");
			addressBuilder.Append (rows[0][5]);

			var contentTemplateBuilder = new System.Text.StringBuilder ();

			contentTemplateBuilder.Append (string.Format (template, p0, p1, p2));

			report.GeneratePdf (stream, topReference, addressBuilder.ToString (), contentTemplateBuilder.ToString ());
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

		private LetterDocument GetReport(LetterDocumentSetup setup)
		{
			var exportPdfInfo   = this.layout.GetExportPdfInfo ();
			var labelPageLayout = this.layout.GetLabelPageLayout ();
			var labelRenderer   = this.layout.GetLabelRenderer ();

			return new LetterDocument (exportPdfInfo, setup);
		}

		private readonly IEnumerable<Column>	columns;
		private readonly PropertyAccessorCache	properties;
		private readonly LabelLayout			layout;
	}
}
