//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Helpers;
using Epsitec.Cresus.Assets.Data;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Popups
{
	public class HistoryPopup : AbstractPopup
	{
		private HistoryPopup(DataAccessor accessor, BaseType baseType, Guid objectGuid, Timestamp? timestamp, ObjectField field)
		{
			this.accessor = new HistoryAccessor (accessor, baseType, objectGuid, timestamp, field);
			this.controller = new HistoryController (accessor, this.accessor);
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
			this.CreateTitle (Res.Strings.Popup.History.Title.ToString ());
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

		private event EventHandler<Timestamp> Navigate;
		#endregion


		#region Helpers
		public static void Show(Widget target, DataAccessor accessor, BaseType baseType, Guid objectGuid, Timestamp? timestamp, ObjectField field, System.Action<Timestamp> action)
		{
			//	Affiche le Popup.
			var popup = new HistoryPopup (accessor, baseType, objectGuid, timestamp, field);

			popup.Create (target, leftOrRight: true);

			popup.Navigate += delegate (object sender, Timestamp t)
			{
				action (t);
			};
		}
		#endregion


		private readonly HistoryAccessor		accessor;
		private readonly HistoryController		controller;
	}
}