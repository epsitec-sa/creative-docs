﻿//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
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
	public class TemporalData : ISettingsData
	{
		public TemporalData()
		{
			this.Clear ();
		}


		public void Clear()
		{
			//	Vide les données et prépare une unique ligne.
			this.duration   = TemporalDataDuration.Other;
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


		public void SetDate(Date? date)
		{
			//	Initialise les dates début/fin selon la durée en cours.
			if (!date.HasValue)
			{
				date = Date.Today;
			}

			switch (this.duration)
			{
				case TemporalDataDuration.Daily:
					this.beginDate = date.Value;
					break;

				case TemporalDataDuration.Weekly:
					int dow = (int) date.Value.DayOfWeek - 1;  // 0..6
					System.Diagnostics.Debug.Assert (dow >= 0 && dow <= 6);
					this.beginDate = Dates.AddDays (date.Value, -dow);
					break;

				case TemporalDataDuration.Monthly:
					this.beginDate = new Date (date.Value.Year, date.Value.Month, 1);
					break;

				case TemporalDataDuration.Quarterly:
					this.beginDate = new Date (date.Value.Year, ((date.Value.Month-1)/3)*3+1, 1);
					break;

				case TemporalDataDuration.Biannual:
					this.beginDate = new Date (date.Value.Year, ((date.Value.Month-1)/6)*6+1, 1);
					break;

				case TemporalDataDuration.Annual:
					this.beginDate = new Date (date.Value.Year, 1, 1);
					break;
			}

			this.InitializeEndDate ();
		}

		private void InitializeEndDate()
		{
			if (this.beginDate.HasValue)
			{
				switch (this.duration)
				{
					case TemporalDataDuration.Daily:
						this.endDate = this.beginDate;
						break;

					case TemporalDataDuration.Weekly:
						this.endDate = Dates.AddDays (this.beginDate.Value, 7-1);
						break;

					case TemporalDataDuration.Monthly:
						this.endDate = Dates.AddDays (Dates.AddMonths (this.beginDate.Value, 1), -1);
						break;

					case TemporalDataDuration.Quarterly:
						this.endDate = Dates.AddDays (Dates.AddMonths (this.beginDate.Value, 3), -1);
						break;

					case TemporalDataDuration.Biannual:
						this.endDate = Dates.AddDays (Dates.AddMonths (this.beginDate.Value, 6), -1);
						break;

					case TemporalDataDuration.Annual:
						this.endDate = new Date (this.beginDate.Value.Year, 12, 31);
						break;
				}
			}
		}


		public void MergeDates(ref Date? beginDate, ref Date? endDate)
		{
			//	Fusionne les dates du filtre normal avec celles du filtre temporel, pour obtenir
			//	le "et" logique.
			if (!this.IsEmpty)
			{
				beginDate = Dates.Max (beginDate, this.beginDate);
				endDate   = Dates.Min (endDate,   this.endDate);
			}
		}


		public TemporalDataDuration Duration
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

		public bool AnyMode
		{
			get;
			set;
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


		public bool CompareTo(TemporalData other)
		{
			return this.duration  == other.duration  &&
				   this.beginDate == other.beginDate &&
				   this.endDate   == other.endDate;
		}

		public TemporalData CopyFrom()
		{
			var data = new TemporalData ();
			this.CopyTo (data);
			return data;
		}

		public void CopyTo(TemporalData dst)
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
				return Dates.GetDescription (this.beginDate, this.endDate);
			}
		}


		public bool Match(Date? date)
		{
			if (date.HasValue)
			{
				return Dates.DateInRange (date, this.beginDate, this.endDate);
			}
			else
			{
				return true;
			}
		}


		private TemporalDataDuration		duration;
		private Date?						beginDate;
		private Date?						endDate;
	}
}