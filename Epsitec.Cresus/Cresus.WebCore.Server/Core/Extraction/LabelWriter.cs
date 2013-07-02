//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Pierre ARNAUD

using Epsitec.Common.Pdf.Engine;
using Epsitec.Common.Pdf.Labels;

using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Data;
using Epsitec.Cresus.Core.Metadata;
using Epsitec.Cresus.Core.Labels;

using System.Collections.Generic;
using System.IO;
using System.Linq;


namespace Epsitec.Cresus.WebCore.Server.Core.Extraction
{
	internal sealed class LabelWriter : EntityWriter
	{
		public LabelWriter (DataSetMetadata metadata, DataSetAccessor accessor, LabelTextFactory textFactory, LabelLayout layout)
			: base (metadata, accessor)
		{
			this.textFactory = textFactory;
			this.layout      = layout;
		}


		protected override string GetExtension()
		{
			return "pdf";
		}


		protected override void WriteStream(Stream stream)
		{
			var labelTexts = this.GetLabelTexts ().ToList ();
			var labels     = this.GetLabels ();

			labels.GeneratePdf (stream, labelTexts.Count, i => labelTexts[i]);
		}


		private IEnumerable<FormattedText> GetLabelTexts()
		{
			return this.Accessor.GetAllItems ()
				.Select (e => this.textFactory.GetLabelText (e));
		}


		private LabelGenerator GetLabels()
		{
			var exportPdfInfo   = this.layout.GetExportPdfInfo ();
			var labelPageLayout = this.layout.GetLabelPageLayout ();
			var labelRenderer   = this.layout.GetLabelRenderer ();

			return new LabelGenerator (exportPdfInfo, labelPageLayout, labelRenderer);
		}


		private readonly LabelTextFactory		textFactory;
		private readonly LabelLayout			layout;
	}
}
