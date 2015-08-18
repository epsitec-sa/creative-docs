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
using Epsitec.Cresus.WebCore.Server.Processors;
using Epsitec.Cresus.Core.Business;


namespace Epsitec.Cresus.WebCore.Server.Core.Extraction
{

	internal sealed class ReportWriter : EntityWriter
	{
		public ReportWriter(DataSetMetadata metadata, DataSetAccessor accessor, BusinessContext context, dynamic parameters, IReportingProcessor processor)
			: base (metadata, accessor)
		{
			this.processor  = processor;
			this.context    = context;
			this.parameters = parameters;
			this.accessor   = accessor;
			
		}

		protected override string GetExtension()
		{
			return "pdf";
		}

		protected override void WriteStream(Stream stream)
		{
			processor.CreateReports (stream, this.context, accessor.GetAllItems (), parameters);
		}

		private IReportingProcessor processor;
		private BusinessContext context;
		private dynamic parameters;
		private DataSetAccessor accessor;
	}
}
