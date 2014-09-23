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


		#region IEquatable<AbstractReportParams> Members
		public virtual bool Equals(AbstractReportParams other)
		{
			//	Il ne faut surtout pas comparer les Guid !
			return !object.ReferenceEquals (other, null)
				&& this.CustomTitle == other.CustomTitle;
		}
		#endregion

		public override sealed bool Equals(object obj)
		{
			return this.Equals (obj as AbstractReportParams);
		}

		public override int GetHashCode()
		{
			return this.CustomTitle.GetHashCode ();
		}


		public static bool operator ==(AbstractReportParams a, AbstractReportParams b)
		{
			return object.Equals (a, b);
		}

		public static bool operator !=(AbstractReportParams a, AbstractReportParams b)
		{
			return !(a == b);
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
