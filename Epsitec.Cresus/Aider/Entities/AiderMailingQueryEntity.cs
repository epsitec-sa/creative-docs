//	Copyright © 2012-2014, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Samuel LOUP, Maintainer: Pierre ARNAUD

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
using Epsitec.Aider.Helpers;
using Epsitec.Cresus.Core.Metadata;
using Epsitec.Cresus.DataLayer.Loader;
using Epsitec.Aider.Override;


namespace Epsitec.Aider.Entities
{
	public partial class AiderMailingQueryEntity
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

		public static AiderMailingQueryEntity Create(BusinessContext context, string name, string commandId)
		{
			var query = context.CreateAndRegisterEntity<AiderMailingQueryEntity> ();

			query.Name        = name;
			query.Query       = AiderUserManager
									.GetCurrentDataSetUISettings (commandId)
									.SelectMany
									(
										d => d.DataSetSettings.AvailableQueries
									).Where
									(
										q => q.Name == name
									).Select
									(
										q => DataSetUISettingsEntity.XmlToByteArray (q.Save ("q"))
									)
									.FirstOrDefault ();
			query.CommandId  = commandId;
			return query;
		}


		public static void Delete(BusinessContext businessContext, AiderMailingQueryEntity query)
		{
			businessContext.DeleteEntity (query);
		}
	}
}
