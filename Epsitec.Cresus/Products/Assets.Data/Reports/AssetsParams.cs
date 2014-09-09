//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Assets.Data.Reports
{
	public class AssetsParams : AbstractReportParams, System.IEquatable<AssetsParams>
	{
		public AssetsParams(string customTitle, Timestamp timestamp, Guid rootGuid, int? level)
			: base (customTitle)
		{
			this.Timestamp = timestamp;
			this.RootGuid  = rootGuid;
			this.Level     = level;
		}


		public override string					Title
		{
			get
			{
				return Res.Strings.Reports.Assets.DefaultTitle.ToString ();
			}
		}


		public static bool operator ==(AssetsParams a, AssetsParams b)
		{
			return a.Equals (b);
		}

		public static bool operator !=(AssetsParams a, AssetsParams b)
		{
			return !a.Equals (b);
		}

		public override bool Equals(object obj)
		{
			if (obj is AssetsParams)
			{
				return this.Equals ((AssetsParams) obj);
			}
			else
			{
				return false;
			}
		}

		public bool Equals(AssetsParams other)
		{
			//	Il ne faut surtout pas comparer les Guid !
			if (!(other is AssetsParams))
			{
				return false;
			}

			return this.CustomTitle == other.CustomTitle
				&& this.Timestamp   == other.Timestamp
				&& this.RootGuid    == other.RootGuid
				&& this.Level       == other.Level;
		}

		public override int GetHashCode()
		{
			return this.CustomTitle.GetHashCode ()
				^  this.Timestamp.GetHashCode ()
				^  this.RootGuid.GetHashCode ()
				^  this.Level.GetHashCode ();
		}


		public override AbstractReportParams ChangePeriod(int direction)
		{
			var timestamp = new Timestamp (this.Timestamp.Date.AddYears (direction), 0);
			return new AssetsParams (this.CustomTitle, timestamp, this.RootGuid, this.Level);
		}

		public override AbstractReportParams ChangeCustomTitle(string customTitle)
		{
			return new AssetsParams (customTitle, this.Timestamp, this.RootGuid, this.Level);
		}


		public readonly Timestamp				Timestamp;
		public readonly Guid					RootGuid;
		public readonly int?					Level;
	}
}
