//	Copyright © 2014, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Samuel LOUP, Maintainer: Pierre ARNAUD

using Epsitec.Aider.Entities;

using Epsitec.Cresus.Core.Business;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Aider.Data.Job
{
	internal class MatchResponse
	{
		public string Pstat
		{
			get;
			set;
		}

		public string ContactId
		{
			get;
			set;
		}

		public string Zip
		{
			get;
			set;
		}

		public string ZipAddOn
		{
			get;
			set;
		}

		public string Town
		{
			get;
			set;
		}

		public string Street
		{
			get;
			set;
		}

		public string Number
		{
			get;
			set;
		}

		public AiderContactEntity GetContact(BusinessContext businessContext)
		{
			return businessContext.DataContext.GetPersistedEntity (this.ContactId) as AiderContactEntity;
		}
	}
}

