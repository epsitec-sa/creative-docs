//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Types;
using Epsitec.Common.Widgets;

using Epsitec.Cresus.Compta.Accessors;
using Epsitec.Cresus.Compta.Controllers;
using Epsitec.Cresus.Compta.Search.Data;
using Epsitec.Cresus.Compta.Options.Data;
using Epsitec.Cresus.Compta.Helpers;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.Memory.Data
{
	public class NavigatorEngine
	{
		public NavigatorEngine()
		{
			this.history = new List<NavigatorData> ();
			this.Clear ();
		}

		public void Clear()
		{
			this.history.Clear ();
			this.index = -1;
		}


		public bool PrevEnable
		{
			get
			{
				return this.index > 0;
			}
		}

		public bool NextEnable
		{
			get
			{
				return this.index < this.history.Count-1;
			}
		}


		public void Update(AbstractDataAccessor accessor, Command command, FormattedText description)
		{
			if (this.index != -1 && this.history[this.index].Command == command)
			{
				this.history[this.index] = this.CreateNavigatorData (accessor, command, description);
			}
		}

		public void Put(AbstractDataAccessor accessor, Command command, FormattedText description)
		{
			var data = this.CreateNavigatorData (accessor, command, description);
			this.history.Insert (++this.index, data);

			int overflow = this.history.Count-this.index-1;
			for (int i = 0; i < overflow; i++)
			{
				this.history.RemoveAt (this.index+1);
			}
		}

		private NavigatorData CreateNavigatorData(AbstractDataAccessor accessor, Command command, FormattedText description)
		{
			SearchData      search  = null;
			SearchData      filter  = null;
			AbstractOptions options = null;

			if (accessor != null && accessor.SearchData != null)
			{
				search  = accessor.SearchData.CopyFrom ();
			}

			if (accessor != null && accessor.FilterData != null)
			{
				filter  = accessor.FilterData.CopyFrom ();
			}

			if (accessor != null && accessor.Options != null)
			{
				options = accessor.Options.CopyFrom ();
			}

			return new NavigatorData (command, description, search, filter, options);
		}


		public Command Back
		{
			get
			{
				return this.history[--this.index].Command;
			}
		}

		public Command Forward
		{
			get
			{
				return this.history[++this.index].Command;
			}
		}

		public void Restore(AbstractDataAccessor accessor)
		{
			var data = this.history[this.index];

			if (accessor != null && accessor.SearchData != null)
			{
				data.Search.CopyTo (accessor.SearchData);
			}

			if (accessor != null && accessor.FilterData != null)
			{
				data.Filter.CopyTo (accessor.FilterData);
			}

			if (accessor != null && accessor.Options != null)
			{
				data.Options.CopyTo (accessor.Options);
			}
		}


		private readonly List<NavigatorData>	history;
		private int								index;
	}
}