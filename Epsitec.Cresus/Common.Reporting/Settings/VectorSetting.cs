//	Copyright © 2008, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;

using System.Collections.Generic;

namespace Epsitec.Common.Reporting.Settings
{
	/// <summary>
	/// The <c>VectorSetting</c> class defines how a vector will be
	/// mapped to a row of values.
	/// </summary>
	public class VectorSetting
	{
		public VectorSetting()
		{
			this.values = new List<VectorValueSetting> ();
		}


		public InclusionMode DefaultValueInclusionMode
		{
			get;
			set;
		}

		public IList<VectorValueSetting> Values
		{
			get
			{
				return this.values;
			}
		}


		public bool CheckInclusion(string id)
		{
			InclusionMode mode = this.GetInclusionMode (id);
			
			switch (this.DefaultValueInclusionMode)
			{
				case InclusionMode.None:
				case InclusionMode.Exclude:
					
					//	Exclude all values which are not explicitely specified
					//	in the inclusion list:

					return mode == InclusionMode.Include;

				case InclusionMode.Include:
					
					//	Include all values which are not explicitely specified
					//	in the exclusion list:
					
					return mode != InclusionMode.Exclude;

			}

			throw new System.NotSupportedException (string.Format ("DefaultValueInclusionMode set to {0}", this.DefaultValueInclusionMode));
		}


		private InclusionMode GetInclusionMode(string id)
		{
			InclusionMode mode = InclusionMode.None;

			foreach (var item in this.values)
			{
				if (item.Id == id)
				{
					mode = item.InclusionMode;
				}
			}
			
			return mode;
		}


		private readonly List<VectorValueSetting> values;
	}
}
