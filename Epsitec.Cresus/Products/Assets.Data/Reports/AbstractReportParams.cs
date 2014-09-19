//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Assets.Data.Reports
{
	public abstract class AbstractReportParams : IGuid, System.IEquatable<AbstractReportParams>
	{
		public AbstractReportParams(string customTitle)
		{
			this.customTitle = customTitle;
			this.guid        = Guid.NewGuid ();
		}


		#region IGuid Members
		public Guid Guid
		{
			get
			{
				return this.guid;
			}
		}
		#endregion


		public string							CustomTitle
		{
			get
			{
				return this.customTitle;
			}
		}

		public abstract string					Title
		{
			get;
		}

		public virtual bool						HasParams
		{
			get
			{
				return true;
			}
		}


		public static bool operator ==(AbstractReportParams a, AbstractReportParams b)
		{
			if (!(a is AbstractReportParams) || !(b is AbstractReportParams))
			{
				return false;
			}

			return a.Equals (b);
		}

		public static bool operator !=(AbstractReportParams a, AbstractReportParams b)
		{
			if (!(a is AbstractReportParams) || !(b is AbstractReportParams))
			{
				return true;
			}

			return !a.Equals (b);
		}

		public virtual bool Equals(AbstractReportParams other)
		{
			//	Il ne faut surtout pas comparer les Guid !
			if (!(other is AbstractReportParams))
			{
				return false;
			}

			return this.CustomTitle == other.CustomTitle;
		}

		public override int GetHashCode()
		{
			return this.CustomTitle.GetHashCode ();
		}


		public virtual AbstractReportParams ChangePeriod(int direction)
		{
			return null;
		}

		public virtual AbstractReportParams ChangeCustomTitle(string customTitle)
		{
			return null;
		}


		private readonly Guid					guid;
		private readonly string					customTitle;
	}
}
