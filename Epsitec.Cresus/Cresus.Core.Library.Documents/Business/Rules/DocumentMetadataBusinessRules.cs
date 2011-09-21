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
			//var generatorPool   = Logic.Current.GetComponent<RefIdGeneratorPool> ();

			//var generator = generatorPool.GetGenerator<DocumentMetadataEntity> ();
			//var nextId    = generator.GetNextId ();

			//entity.IdA                   = string.Format ("{0:000000}", nextId);
			entity.DocumentTitle         = "";
			entity.Description           = "";
			entity.FileName              = null;
			entity.FileUri               = null;
			entity.FileMimeType          = null;
		}

		public override void ApplyBindRule(DocumentMetadataEntity entity)
		{
			var businessContext = Logic.Current.GetComponent<IBusinessContext> ();

			businessContext.Register (entity.BusinessDocument);
		}

		public override void ApplyUpdateRule(DocumentMetadataEntity entity)
		{
		}
	}
}