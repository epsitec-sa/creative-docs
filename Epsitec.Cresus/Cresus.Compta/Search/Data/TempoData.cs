//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Types;

using Epsitec.Cresus.Compta.Accessors;
using Epsitec.Cresus.Compta.Controllers;
using Epsitec.Cresus.Compta.Helpers;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.Search.Data
{
	public class TempoData : ISettingsData
	{
		public TempoData()
		{
			this.Clear ();
		}


		public void Clear()
		{
			//	Vide les données et prépare une unique ligne.
			this.period    = TempoDataPeriod.Daily;
			this.beginDate = null;
			this.endDate   = null;

			this.Enable = true;
		}


		public bool Enable
		{
			get;
			set;
		}

		public bool IsEmpty
		{
			//	Indique si les données sont totalement vides.
			get
			{
				if (this.Enable == false)
				{
					return true;
				}

				return !this.IsDefined;
			}
		}

		private bool IsDefined
		{
			//	Indique si les données sont définies.
			get
			{
				return this.beginDate.HasValue || this.endDate.HasValue;
			}
		}


		public void Next(int step)
		{
			this.beginDate = this.Next (this.beginDate, step);
			this.endDate   = this.Next (this.endDate,   step);
		}

		private Date? Next(Date? date, int step)
		{
			if (date.HasValue)
			{
				if (this.period == TempoDataPeriod.Daily)
				{
					date = Dates.AddDays (date.Value, step);
				}

				if (this.period == TempoDataPeriod.Weekly)
				{
					date = Dates.AddDays (date.Value, step*7);
				}

				if (this.period == TempoDataPeriod.Monthly)
				{
					date = Dates.AddMonths (date.Value, step);
				}

				if (this.period == TempoDataPeriod.Quarterly)
				{
					date = Dates.AddMonths (date.Value, step*3);
				}

				if (this.period == TempoDataPeriod.Biannual)
				{
					date = Dates.AddMonths (date.Value, step*6);
				}

				if (this.period == TempoDataPeriod.Annual)
				{
					date = Dates.AddMonths (date.Value, step*12);
				}
			}

			return date;
		}


		public TempoDataPeriod Period
		{
			get
			{
				return this.period;
			}
			set
			{
				this.period = value;
			}
		}

		public Date? BeginDate
		{
			get
			{
				return this.beginDate;
			}
			set
			{
				this.beginDate = value;
			}
		}

		public Date? EndDate
		{
			get
			{
				return this.endDate;
			}
			set
			{
				this.endDate = value;
			}
		}


		public bool CompareTo(TempoData other)
		{
			return this.period    == other.period    &&
				   this.beginDate == other.beginDate &&
				   this.endDate   == other.endDate;
		}

		public TempoData CopyFrom()
		{
			var data = new TempoData ();
			this.CopyTo (data);
			return data;
		}

		public void CopyTo(TempoData dst)
		{
			dst.period    = this.period;
			dst.beginDate = this.beginDate;
			dst.endDate   = this.endDate;
		}


		public FormattedText GetSummary(List<ColumnMapper> columnMappers)
		{
			if (this.IsEmpty)
			{
				return FormattedText.Empty;
			}
			else
			{
				var builder = new System.Text.StringBuilder ();

				if (this.beginDate.HasValue && this.endDate.HasValue)
				{
					builder.Append ("Du ");
					builder.Append (Converters.DateToString (this.beginDate));
					builder.Append (" au ");
					builder.Append (Converters.DateToString (this.endDate));
				}
				else if (this.beginDate.HasValue)
				{
					builder.Append ("Du ");
					builder.Append (Converters.DateToString (this.beginDate));
				}
				else if (this.endDate.HasValue)
				{
					builder.Append ("Au ");
					builder.Append (Converters.DateToString (this.endDate));
				}

				return builder.ToString ();
			}
		}


		private TempoDataPeriod		period;
		private Date?				beginDate;
		private Date?				endDate;
	}
}