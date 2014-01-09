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
		public string Ustat
		{
			get;
			set;
		}

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

		public string CorrectedZip
		{
			get;
			set;
		}

		public string CorrectedZipAddOn
		{
			get;
			set;
		}

		public string CorrectedTown
		{
			get;
			set;
		}

		public string CorrectedStreet
		{
			get;
			set;
		}

		public string CorrectedNumber
		{
			get;
			set;
		}

		public string NewZip
		{
			get;
			set;
		}

		public string NewZipAddOn
		{
			get;
			set;
		}

		public string NewTown
		{
			get;
			set;
		}

		public string NewStreet
		{
			get;
			set;
		}

		public string NewNumber
		{
			get;
			set;
		}

		public string NewPostBox
		{
			get;
			set;
		}

		public AiderContactEntity GetContact(BusinessContext businessContext)
		{
			return businessContext.DataContext.GetPersistedEntity (this.ContactId) as AiderContactEntity;
		}

		public bool IsSameNewAddress(MatchResponse toCompare)
		{
			if (this.NewStreet == toCompare.NewStreet && this.NewTown == toCompare.NewTown && this.NewZip == toCompare.NewZip && this.NewNumber == toCompare.NewNumber)
				return true;
			else
				return false;
		}
	}
}

