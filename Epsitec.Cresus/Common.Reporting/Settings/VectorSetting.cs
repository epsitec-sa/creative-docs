//	Copyright © 2008, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Collections;

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
			this.values = new ObservableList<VectorValueSetting> ();
			this.values.CollectionChanged += this.ClearCache;
			this.modes = new Dictionary<string, InclusionMode> ();
			this.inclusions = new List<string> ();
		}


		public InclusionMode DefaultInclusionMode
		{
			get
			{
				if (this.defaultInclusionMode == InclusionMode.None)
				{
					this.RegenerateCache ();
				}

				return this.defaultInclusionMode;
			}
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
			InclusionMode def  = this.DefaultInclusionMode;
			InclusionMode mode = this.GetInclusionMode (id);
			
			switch (def)
			{
				case InclusionMode.Exclude:
					
					//	Exclude all values which are not explicitely specified
					//	in the inclusion list:

					return mode == InclusionMode.Include;

				case InclusionMode.Include:
					
					//	Include all values which are not explicitely specified
					//	in the exclusion list:
					
					return mode != InclusionMode.Exclude;

			}

			throw new System.NotSupportedException (string.Format ("DefaultInclusionMode set to {0}", this.DefaultInclusionMode));
		}

		public List<string> CreateList(IEnumerable<string> vectorIds)
		{
			List<string> ids = new List<string> ();
			
			if (this.DefaultInclusionMode == InclusionMode.Include)
			{
				foreach (string id in vectorIds)
				{
					switch (this.GetInclusionMode (id))
					{
						case InclusionMode.None:
							ids.Add (id);
							break;

						case InclusionMode.Include:
						case InclusionMode.Exclude:
							break;
					}
				}

				int index = 0;

				foreach (string id in this.inclusions)
				{
					if (id == "*")
					{
						index = ids.Count;
					}
					else
					{
						ids.Insert (index++, id);
					}
				}
			}
			else
			{
				ids.AddRange (this.inclusions);
			}
			
			return ids;
		}


		private void ClearCache(object sender, CollectionChangedEventArgs e)
		{
			this.defaultInclusionMode = InclusionMode.None;
			this.modes.Clear ();
			this.inclusions.Clear ();
		}

		private InclusionMode GetInclusionMode(string id)
		{
			System.Diagnostics.Debug.Assert (this.defaultInclusionMode != InclusionMode.None);

			InclusionMode mode;

			if (this.modes.TryGetValue (id, out mode))
			{
				return mode;
			}
			else
			{
				return InclusionMode.None;
			}
		}

		private void RegenerateCache()
		{
			System.Diagnostics.Debug.Assert (this.modes.Count == 0);
			System.Diagnostics.Debug.Assert (this.inclusions.Count == 0);

			this.defaultInclusionMode = InclusionMode.Exclude;

			List<string> ids = new List<string> ();

			foreach (var item in this.values)
			{
				string id = item.Id;

				if (id == "*")
				{
					if (item.InclusionMode == InclusionMode.Include)
					{
						this.defaultInclusionMode = InclusionMode.Include;
					}
					else
					{
						this.defaultInclusionMode = InclusionMode.Exclude;
					}
				}
				
				if (this.modes.ContainsKey (id))
				{
					//	Don't add the item a second time into the inclusion list.
				}
				else
				{
					ids.Add (id);
				}

				switch (item.InclusionMode)
				{
					case InclusionMode.None:
						this.modes.Remove (id);
						break;
					
					case InclusionMode.Include:
						this.modes[id] = InclusionMode.Include;
						break;
					
					case InclusionMode.Exclude:
						this.modes[id] = InclusionMode.Exclude;
						break;
				}
			}

			foreach (string id in ids)
			{
				InclusionMode mode;

				if ((this.modes.TryGetValue (id, out mode)) &&
					(mode == InclusionMode.Include))
				{
					this.inclusions.Add (id);
				}
			}
		}


		private readonly ObservableList<VectorValueSetting> values;
		private readonly Dictionary<string, InclusionMode> modes;
		private readonly List<string> inclusions;
		
		private InclusionMode defaultInclusionMode;
	}
}
