//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;

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
			var pool = Logic.Current.Data.RefIdGeneratorPool;
			var generator = pool.GetGenerator<AffairEntity> ();
			var nextId    = generator.GetNextId ();

			entity.IdA                   = string.Format ("{0:000000}", nextId);
			entity.DocumentTitle         = "";
			entity.Description           = "";
			entity.FileName              = null;
			entity.FileUri               = null;
			entity.FileMimeType          = null;
			entity.CreationDate          = System.DateTime.Now;
			entity.LastModificationDate  = entity.CreationDate;
		}

		public override void ApplyBindRule(DocumentMetadataEntity entity)
		{
			Logic.Current.BusinessContext.Register (entity.BusinessDocument);
		}

		public override void ApplyUpdateRule(DocumentMetadataEntity entity)
		{
			entity.LastModificationDate  = System.DateTime.Now;
		}
	}
}
