//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Assets.Server.NodeGetters
{
	public class DateCumulValue : AbstractCumulValue
	{
		public DateCumulValue(System.DateTime? value)
			: base ()
		{
			this.minValue = value;
			this.maxValue = value;
		}

		private DateCumulValue(System.DateTime? minValue, System.DateTime? maxValue)
			: base ()
		{
			this.minValue = minValue;
			this.maxValue = maxValue;
		}


		public System.DateTime?					MinValue
		{
			get
			{
				return this.minValue;
			}
		}

		public System.DateTime?					MaxValue
		{
			get
			{
				return this.maxValue;
			}
		}

		public bool								IsRange
		{
			get
			{
				return this.minValue.HasValue &&
					   this.maxValue.HasValue &&
					   this.minValue.Value != this.maxValue.Value;
			}
		}

		public override bool					IsExist
		{
			get
			{
				return this.minValue.HasValue;
			}
		}

		public override AbstractCumulValue Merge(AbstractCumulValue a)
		{
			var aa = a as DateCumulValue;

			if (this.IsExist && aa.IsExist)
			{
				if (!this.IsRange && !aa.IsRange && this.MinValue == aa.MinValue)
				{
					return this;
				}
				else
				{
					var min = DateCumulValue.Min (this.minValue.Value, aa.minValue.Value);
					var max = DateCumulValue.Max (this.maxValue.Value, aa.maxValue.Value);
					return new DateCumulValue (min, max);
				}
			}
			else if (this.IsExist)
			{
				return this;
			}
			else if (aa.IsExist)
			{
				return aa;
			}

			return new DateCumulValue (null, null);
		}


		private static System.DateTime Min(System.DateTime d1, System.DateTime d2)
		{
			if (d1 < d2)
			{
				return d1;
			}
			else
			{
				return d2;
			}
		}

		private static System.DateTime Max(System.DateTime d1, System.DateTime d2)
		{
			if (d1 > d2)
			{
				return d1;
			}
			else
			{
				return d2;
			}
		}


		private readonly System.DateTime?		minValue;
		private readonly System.DateTime?		maxValue;
	}
}
