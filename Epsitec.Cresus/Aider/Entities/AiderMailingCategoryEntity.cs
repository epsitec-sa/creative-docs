//	Copyright © 2012-2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Marc BETTEX

using Epsitec.Aider.Enumerations;

using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Text;
using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Entities;

using Epsitec.Cresus.DataLayer.Context;

using System;
using System.Collections.Generic;
using System.Linq;


namespace Epsitec.Aider.Entities
{
	public partial class AiderMailingCategoryEntity
	{
		public override FormattedText GetCompactSummary()
		{
			return TextFormatter.FormatText
			(
				this.Name
			);
		}


		public override FormattedText GetSummary()
		{
			return TextFormatter.FormatText (this.Name);
		}

		public void RefreshCache()
		{
		
			
		}

		public static AiderMailingCategoryEntity Create(BusinessContext context, string name)
		{
			var mailingCategory = context.CreateAndRegisterEntity<AiderMailingCategoryEntity> ();
			mailingCategory.Name = name;
			return mailingCategory;
		}


		public static void Delete(BusinessContext businessContext, AiderMailingCategoryEntity mailingCategory)
		{
			businessContext.DeleteEntity (mailingCategory);			
		}
	}
}
