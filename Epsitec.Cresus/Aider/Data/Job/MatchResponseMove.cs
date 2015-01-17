//	Copyright © 2014, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Samuel LOUP, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Aider.Data.Job
{
	internal class MatchResponseMove : MatchResponse
	{
		public string Ustat
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

		public bool HasSameNewAddress(MatchResponseMove other)
		{
			return (this.NewStreet == other.NewStreet)
					&& (this.NewTown   == other.NewTown)
					&& (this.NewZip    == other.NewZip)
					&& (this.NewNumber == other.NewNumber);
		}
	}
}

