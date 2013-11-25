//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Helpers;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Popups
{
	public class HistoryPopup : AbstractPopup
	{
		public HistoryPopup(DataAccessor accessor, BaseType baseType, Guid objectGuid, Timestamp? timestamp, ObjectField field)
		{
			this.accessor = new HistoryAccessor (accessor, baseType, objectGuid, timestamp, field);
			this.controller = new HistoryController (this.accessor);
		}


		protected override Size DialogSize
		{
			get
			{
				return this.controller.GetSize (this.GetParent ());
			}
		}

		public override void CreateUI()
		{
			this.CreateTitle ("Amortissements");
			this.CreateCloseButton ();

			var frame = new FrameBox
			{
				Parent = this.mainFrameBox,
				Dock   = DockStyle.Fill,
			};

			this.controller.CreateUI (frame);

			this.controller.Navigate += delegate (object sender, Timestamp timestamp)
			{
				this.ClosePopup ();
				this.OnNavigate (timestamp);
			};
		}


		#region Events handler
		private void OnNavigate(Timestamp timestamp)
		{
			this.Navigate.Raise (this, timestamp);
		}

		public event EventHandler<Timestamp> Navigate;
		#endregion


		private readonly HistoryAccessor		accessor;
		private readonly HistoryController		controller;
	}
}