//	Copyright © 2014, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Samuel LOUP, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Aider.Data.Job
{
	internal class MatchResponseFix : MatchResponse
	{
		public string Pstat
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
	}
}

