//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Types;

using Epsitec.Cresus.Compta.Accessors;
using Epsitec.Cresus.Compta.Controllers;
using Epsitec.Cresus.Compta.Helpers;

using Epsitec.Cresus.Compta.Entities;

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
			this.duration   = TempoDataDuration.Other;
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


		public void InitDefaultDates(ComptaPériodeEntity période)
		{
			var now = Date.Today;

			if (!Dates.DateInRange (now, période.DateDébut, période.DateFin))
			{
				now = période.DateDébut;
			}

			switch (this.duration)
			{
				case TempoDataDuration.Daily:
					this.beginDate = now;
					break;

				case TempoDataDuration.Weekly:
					int dof = (int) now.DayOfWeek - 1;  // 0..6
					System.Diagnostics.Debug.Assert (dof >= 0 && dof <= 6);
					this.beginDate = Dates.AddDays (now, -dof);
					break;

				case TempoDataDuration.Monthly:
					this.beginDate = new Date (now.Year, now.Month, 1);
					break;

				case TempoDataDuration.Quarterly:
					this.beginDate = new Date (now.Year, (now.Month-1)/3+1, 1);
					break;

				case TempoDataDuration.Biannual:
					this.beginDate = new Date (now.Year, (now.Month-1)/6+1, 1);
					break;

				case TempoDataDuration.Annual:
					this.beginDate = new Date (now.Year, 1, 1);
					break;
			}

			this.InitEndDate ();
		}

		public void Next(int step)
		{
			this.beginDate = this.Next (this.beginDate, step);
			this.InitEndDate ();
		}

		private Date? Next(Date? date, int step)
		{
			if (date.HasValue)
			{
				switch (this.duration)
				{
					case TempoDataDuration.Daily:
						date = Dates.AddDays (date.Value, step);
						break;

					case TempoDataDuration.Weekly:
						date = Dates.AddDays (date.Value, step*7);
						break;

					case TempoDataDuration.Monthly:
						date = Dates.AddMonths (date.Value, step);
						break;

					case TempoDataDuration.Quarterly:
						date = Dates.AddMonths (date.Value, step*3);
						break;

					case TempoDataDuration.Biannual:
						date = Dates.AddMonths (date.Value, step*6);
						break;

					case TempoDataDuration.Annual:
						date = Dates.AddMonths (date.Value, step*12);
						break;
				}
			}

			return date;
		}

		private void InitEndDate()
		{
			if (this.beginDate.HasValue)
			{
				switch (this.duration)
				{
					case TempoDataDuration.Daily:
						this.endDate = this.beginDate;
						break;

					case TempoDataDuration.Weekly:
						this.endDate = Dates.AddDays (this.beginDate.Value, 7-1);
						break;

					case TempoDataDuration.Monthly:
						this.endDate = Dates.AddDays (Dates.AddMonths (this.beginDate.Value, 1), -1);
						break;

					case TempoDataDuration.Quarterly:
						this.endDate = Dates.AddDays (Dates.AddMonths (this.beginDate.Value, 3), -1);
						break;

					case TempoDataDuration.Biannual:
						this.endDate = Dates.AddDays (Dates.AddMonths (this.beginDate.Value, 6), -1);
						break;

					case TempoDataDuration.Annual:
						this.endDate = new Date (this.beginDate.Value.Year, 12, 31);
						break;
				}
			}
		}


		public TempoDataDuration Duration
		{
			get
			{
				return this.duration;
			}
			set
			{
				this.duration = value;
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
			return this.duration  == other.duration  &&
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
			dst.duration  = this.duration;
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


		private TempoDataDuration		duration;
		private Date?					beginDate;
		private Date?					endDate;
	}
}