//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Drawing;
using Epsitec.Cresus.Assets.Server.NaiveEngine;

namespace Epsitec.Cresus.Assets.App.Popups
{
	public class HistoryPopup : AbstractPopup
	{
		public HistoryPopup(DataAccessor accessor, Guid objectGuid, Timestamp? timestamp, int field)
		{
			this.accessor = new HistoryAccessor (accessor, objectGuid, timestamp, field);
			this.controller = new HistoryController (this.accessor);
		}


		protected override Size DialogSize
		{
			get
			{
				return this.controller.GetSize (this.GetParent ());
			}
		}

		protected override void CreateUI()
		{
			var frame = this.CreateFullFrame ();

			this.controller.CreateUI (frame);
			this.CreateCloseButton ();

			this.controller.Navigate += delegate (object sender, Timestamp timestamp)
			{
				this.ClosePopup ();
				this.OnNavigate (timestamp);
			};
		}


		#region Events handler
		private void OnNavigate(Timestamp timestamp)
		{
			if (this.Navigate != null)
			{
				this.Navigate (this, timestamp);
			}
		}

		public delegate void NavigateEventHandler(object sender, Timestamp timestamp);
		public event NavigateEventHandler Navigate;
		#endregion


		private readonly HistoryAccessor		accessor;
		private readonly HistoryController		controller;
	}
}