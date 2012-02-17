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


		public int Count
		{
			get
			{
				return this.history.Count;
			}
		}

		public int Index
		{
			get
			{
				return this.index;
			}
		}

		public NavigatorData GetNavigatorData(int index)
		{
			return this.history[index];
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


		public void Update(AbstractController controller, Command command)
		{
			if (this.index != -1 && this.history[this.index].Command == command)
			{
				this.history[this.index] = this.CreateNavigatorData (controller, command);
			}
		}

		public void Put(AbstractController controller, Command command)
		{
			var data = this.CreateNavigatorData (controller, command);
			this.history.Insert (++this.index, data);

			int overflow = this.history.Count-this.index-1;
			for (int i = 0; i < overflow; i++)
			{
				this.history.RemoveAt (this.index+1);
			}
		}

		private NavigatorData CreateNavigatorData(AbstractController controller, Command command)
		{
			SearchData      search  = null;
			SearchData      filter  = null;
			AbstractOptions options = null;

			if (controller.DataAccessor != null && controller.DataAccessor.SearchData != null)
			{
				search = controller.DataAccessor.SearchData.CopyFrom ();
			}

			if (controller.DataAccessor != null && controller.DataAccessor.FilterData != null)
			{
				filter = controller.DataAccessor.FilterData.CopyFrom ();
			}

			if (controller.DataAccessor != null && controller.DataAccessor.Options != null)
			{
				options = controller.DataAccessor.Options.NavigatorCopyFrom ();
			}

			return new NavigatorData (command, controller.MixTitle, controller.MemoryList.Selected, search, filter, options);
		}


		public Command Any(int index)
		{
			this.index = index;
			return this.history[this.index].Command;
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

		public void Restore(AbstractController controller)
		{
			var data = this.history[this.index];

			controller.MemoryList.Selected = data.Memory;

			if (controller.DataAccessor != null && controller.DataAccessor.SearchData != null)
			{
				data.Search.CopyTo (controller.DataAccessor.SearchData);
			}

			if (controller.DataAccessor != null && controller.DataAccessor.FilterData != null)
			{
				data.Filter.CopyTo (controller.DataAccessor.FilterData);
			}

			if (controller.DataAccessor != null && controller.DataAccessor.Options != null)
			{
				data.Options.NavigatorCopyTo (controller.DataAccessor.Options);
			}
		}


		private readonly List<NavigatorData>	history;
		private int								index;
	}
}