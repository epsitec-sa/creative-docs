//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Widgets.Tiles
{
	public sealed class TileEntityDisplaySettings
	{
		public TileEntityDisplaySettings()
		{
			this.fields = new Dictionary<Druid, List<TileUserFieldDisplaySettings>> ();
		}


		public void Add(Druid field, TileUserFieldDisplaySettings settings)
		{
			List<TileUserFieldDisplaySettings> list;

			if (this.fields.TryGetValue (field, out list) == false)
			{
				list = new List<TileUserFieldDisplaySettings> ();
				this.fields[field] = list;
			}

			list.Add (settings);
		}

		public bool Remove(Druid field, TileUserFieldDisplaySettings settings)
		{
			List<TileUserFieldDisplaySettings> list;

			if (this.fields.TryGetValue (field, out list))
			{
				if (list.Remove (settings))
				{
					if (list.Count == 0)
					{
						this.fields.Remove (field);
					}

					return true;
				}
			}

			return false;
		}

		public int RemoveAll(Druid field, System.Predicate<TileUserFieldDisplaySettings> match)
		{
			List<TileUserFieldDisplaySettings> list;
			int count = 0;

			if (this.fields.TryGetValue (field, out list))
			{
				count = list.RemoveAll (match);

				if (list.Count == 0)
				{
					this.fields.Remove (field);
				}
			}

			return count;
		}




		private readonly Dictionary<Druid, List<TileUserFieldDisplaySettings>> fields;
	}
}
