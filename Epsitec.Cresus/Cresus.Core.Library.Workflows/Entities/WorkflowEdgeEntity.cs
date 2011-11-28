//	Copyright © 2010-2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Data;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Entities
{
	public partial class WorkflowEdgeEntity : IEntityInitializer, ILabelGetter
	{
		public void InitializeDefaultValues()
		{
			this.Code = (string) ItemCodeGenerator.NewCode ();
		}

		public WorkflowNodeEntity GetContinuationOrDefault(WorkflowNodeEntity defaultNode)
		{
			return this.Continuation.IsNull () ? defaultNode : this.Continuation;
		}

		#region ILabelGetter Members

		public FormattedText GetLabel(LabelDetailLevel detailLevel)
		{
			var labelSource = TextFormatter.FormatText (this.Labels);

			if (labelSource.Length == 0)
			{
				labelSource = TextFormatter.FormatText (this.Name);
			}

			var labels = labelSource.Split ("\n", System.StringSplitOptions.RemoveEmptyEntries);

			int n = labels.Length;

			if (n == 0)
			{
				return FormattedText.Empty;
			}

			if (detailLevel == LabelDetailLevel.Default)
			{
				return labels[0];
			}

			if (n > 1)
			{
				labels = labels.OrderBy (x => x.Length).ToArray ();
			}

			switch (detailLevel)
			{
				case LabelDetailLevel.Compact:
					return labels[0];

				case LabelDetailLevel.Detailed:
					return labels[n-1];

				case LabelDetailLevel.Normal:
					return labels[n > 1 ? 1 : 0];

				case LabelDetailLevel.Default:
					//	Already handled above
				default:
					throw new System.NotSupportedException (string.Format ("{0} not supported", detailLevel.GetQualifiedName ()));
			}
		}

		#endregion

	}
}
