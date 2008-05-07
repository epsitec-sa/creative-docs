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
	public class VectorSetting : System.Collections.IEnumerable
	{
		public VectorSetting()
		{
			this.values = new ObservableList<VectorValueSetting> ();
			this.values.CollectionChanged += (sender, e) => this.ClearCache ();
			this.lookup = new Dictionary<string, VectorValueSetting> ();
			this.inclusions = new List<string> ();
		}


		/// <summary>
		/// Gets the default inclusion mode, based on the <see cref="InclusionMode"/>
		/// defined for the value with the <c>"*"</c> id (if any).
		/// </summary>
		/// <value>The default inclusion mode.</value>
		public InclusionMode DefaultInclusionMode
		{
			get
			{
				this.RegenerateCacheIfNeeded ();

				return this.defaultInclusionMode;
			}
		}

		/// <summary>
		/// Gets the collection of value settings. Add <see cref="VectorValueSetting"/>
		/// instances to this list. The last definition wins in case of redefinitions.
		/// </summary>
		/// <value>The value settings.</value>
		public IList<VectorValueSetting> Values
		{
			get
			{
				return this.values;
			}
		}


		/// <summary>
		/// Adds the specified value setting.
		/// </summary>
		/// <param name="value">The value setting.</param>
		public void Add(VectorValueSetting value)
		{
			this.values.Add (value);
		}

		/// <summary>
		/// Checks whether the specified value should be included in the output
		/// vector.
		/// </summary>
		/// <param name="id">The value id.</param>
		/// <returns><c>true</c> if the specified value should be included in the
		/// output vector.</returns>
		public bool CheckInclusion(string id)
		{
			InclusionMode def  = this.DefaultInclusionMode;
			InclusionMode mode = this.LookUpInclusionMode (id);
			
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

		/// <summary>
		/// Creates the output list of value ids, based on the provided list
		/// and on the expected ids, as defined by the <c>Values</c> collection.
		/// </summary>
		/// <param name="valueIds">The value ids.</param>
		/// <returns>The output list of value ids.</returns>
		public List<string> CreateList(IEnumerable<string> valueIds)
		{
			List<string> ids = new List<string> ();
			
			if (this.DefaultInclusionMode == InclusionMode.Include)
			{
				foreach (string id in valueIds)
				{
					switch (this.LookUpInclusionMode (id))
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

		/// <summary>
		/// Gets the title for the specified value id (i.e. the column title).
		/// The title is represented using formatted text.
		/// </summary>
		/// <param name="valueId">The value id.</param>
		/// <value>The title, as formatted text.</value>
		public FormattedText GetValueTitle(string valueId)
		{
			this.RegenerateCacheIfNeeded ();

			VectorValueSetting value;

			if (this.lookup.TryGetValue (valueId, out value))
			{
				return value.Title;
			}
			else
			{
				return FormattedText.Null;
			}
		}

		#region IEnumerable Members

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return this.values.GetEnumerator ();
		}

		#endregion


		/// <summary>
		/// Clears the cache.
		/// </summary>
		private void ClearCache()
		{
			this.defaultInclusionMode = InclusionMode.None;
			this.lookup.Clear ();
			this.inclusions.Clear ();
		}

		/// <summary>
		/// Regenerates the cached information, if needed.
		/// See method <see cref="RegenerateCache"/> for the details.
		/// </summary>
		private void RegenerateCacheIfNeeded()
		{
			if (this.defaultInclusionMode == InclusionMode.None)
			{
				this.RegenerateCache ();
			}
		}

		/// <summary>
		/// Regenerates the cached information: the lookup dictionary is used
		/// to find value settings based on their id and the inclusions list is
		/// a simplified version of the values list (only inclusions are kept).
		/// </summary>
		private void RegenerateCache()
		{
			System.Diagnostics.Debug.Assert (this.lookup.Count == 0);
			System.Diagnostics.Debug.Assert (this.inclusions.Count == 0);

			this.defaultInclusionMode = InclusionMode.Exclude;

			List<string> ids = new List<string> ();

			foreach (var item in this.values)
			{
				string id = item.Id;

				//	The "*" value setting defines the default inclusion mode
				//	for this vector setting:
				
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

				if (this.lookup.ContainsKey (id))
				{
					//	Don't add the item a second time into the inclusion list.
				}
				else
				{
					ids.Add (id);
				}

				switch (item.InclusionMode)
				{
					case InclusionMode.None:	this.lookup.Remove (id); break;
					case InclusionMode.Include:	this.lookup[id] = item;  break;
					case InclusionMode.Exclude:	this.lookup[id] = item;  break;
				}
			}

			//	Produce the simplified inclusions list, which lists only the
			//	inclusions, and at most once.
			
			foreach (string id in ids)
			{
				VectorValueSetting value;

				if ((this.lookup.TryGetValue (id, out value)) &&
					(value.InclusionMode == InclusionMode.Include))
				{
					this.inclusions.Add (id);
				}
			}
		}

		/// <summary>
		/// Looks up the inclusion mode for the specified value id.
		/// </summary>
		/// <param name="id">The value id.</param>
		/// <returns>The inclusion mode or <c>InclusionMode.None</c> if there
		/// is no explicit setting for this value id.</returns>
		private InclusionMode LookUpInclusionMode(string id)
		{
			System.Diagnostics.Debug.Assert (this.defaultInclusionMode != InclusionMode.None);

			VectorValueSetting value;

			if (this.lookup.TryGetValue (id, out value))
			{
				return value.InclusionMode;
			}
			else
			{
				return InclusionMode.None;
			}
		}


		private readonly ObservableList<VectorValueSetting> values;
		private readonly Dictionary<string, VectorValueSetting> lookup;
		private readonly List<string> inclusions;
		
		private InclusionMode defaultInclusionMode;
	}
}
