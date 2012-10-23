//	Copyright © 2010-2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Cresus.Core.Entities;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Business.Rules
{
	[BusinessRule]
	internal class DocumentMetadataBusinessRules : GenericBusinessRule<DocumentMetadataEntity>
	{
		public override void ApplySetupRule(DocumentMetadataEntity entity)
		{
			entity.DocumentTitle = "";
			entity.Description   = "";
			entity.FileName      = null;
			entity.FileUri       = null;
			entity.FileMimeType  = null;
		}

		public override void ApplyBindRule(DocumentMetadataEntity entity)
		{
			var businessContext = Logic.Current.GetComponent<BusinessContext> ();

			businessContext.Register (entity.BusinessDocument);
		}

		public override void ApplyUpdateRule(DocumentMetadataEntity entity)
		{
		}
	}
}