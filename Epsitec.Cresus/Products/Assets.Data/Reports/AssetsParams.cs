//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Assets.Data.Reports
{
	public class AssetsParams : AbstractReportParams
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


		public override bool Equals(AbstractReportParams other)
		{
			//	Il ne faut surtout pas comparer les Guid !
			if (!base.Equals (other))
			{
				return false;
			}

			var o = other as AssetsParams;

			return !object.ReferenceEquals (other, null)
				&& this.Timestamp == o.Timestamp
				&& this.RootGuid  == o.RootGuid
				&& this.Level     == o.Level;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode ()
				^  this.CustomTitle.GetHashCode ()
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
