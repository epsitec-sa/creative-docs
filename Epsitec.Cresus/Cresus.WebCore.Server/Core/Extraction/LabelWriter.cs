using Epsitec.Common.Pdf.Engine;
using Epsitec.Common.Pdf.Labels;

using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Data;
using Epsitec.Cresus.Core.Metadata;
using Epsitec.Cresus.Core.Labels;

using System;

using System.Collections.Generic;

using System.IO;

using System.Linq;


namespace Epsitec.Cresus.WebCore.Server.Core.Extraction
{


	internal sealed class LabelWriter : EntityWriter
	{


		public LabelWriter
		(
			DataSetMetadata metadata,
			DataSetAccessor accessor,
			LabelTextFactory textFactory,
			LabelLayout layout

		)
			: base (metadata, accessor)
		{
			this.textFactory = textFactory;
			this.layout = layout;
		}


		protected override string GetExtension()
		{
			return "pdf";
		}


		protected override void WriteStream(Stream stream)
		{
			var labelTexts = this.GetLabelTexts ();
			var labels = this.GetLabels ();

			labels.GeneratePdf (stream, labelTexts.Count, i => labelTexts[i]);
		}


		private List<FormattedText> GetLabelTexts()
		{
			return this.Accessor.GetAllItems ()
				.Select (e => this.textFactory.GetLabelText (e))
				.ToList ();
		}


		private Labels GetLabels()
		{
			var exportPdfInfo = this.layout.GetExportPdfInfo ();
			var labelsSetup = this.layout.GetLabelsSetup ();

			return new Labels (exportPdfInfo, labelsSetup);
		}


		private readonly LabelTextFactory textFactory;


		private readonly LabelLayout layout;


	}


}
