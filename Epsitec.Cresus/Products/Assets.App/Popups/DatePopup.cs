//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Helpers;
using Epsitec.Cresus.Assets.App.Views;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Popups
{
	/// <summary>
	/// Popup permettant de choisir une date, à l'aide du composant complet DateController.
	/// </summary>
	public class DatePopup : AbstractPopup
	{
		public DatePopup(DataAccessor accessor)
		{
			this.accessor = accessor;
		}


		public System.DateTime?					Date;

		protected override Size					DialogSize
		{
			get
			{
				return new Size (DatePopup.dialogWidth, DatePopup.dialogHeight);
			}
		}

		protected override void CreateUI()
		{
			this.CreateTitle (Res.Strings.Popup.Date.Title.ToString ());

			this.CreateDateUI ();
			this.CreateCloseButton ();
		}

		private void CreateDateUI()
		{
			this.dateFrame = this.CreateFrame (DatePopup.margins, DatePopup.margins, DateController.controllerWidth, DateController.controllerHeight);

			this.dateController = new DateController (this.accessor)
			{
				DateRangeCategory = DateRangeCategory.Mandat,
				Date              = this.Date,
				DateLabelWidth    = 0,
				DateDescription   = null,
			};

			this.dateController.CreateUI (this.dateFrame);

			this.dateController.DateChanged += delegate
			{
				this.Date = this.dateController.Date;
				this.OnDateChanged (this.dateController.Date);
			};
		}


		#region Events handler
		private void OnDateChanged(System.DateTime? dateTime)
		{
			this.DateChanged.Raise (this, dateTime);
		}

		public event EventHandler<System.DateTime?> DateChanged;
		#endregion


		private const int margins      = 20;
		private const int dialogWidth  = DateController.controllerWidth  + DatePopup.margins*2;
		private const int dialogHeight = AbstractPopup.titleHeight + DateController.controllerHeight + DatePopup.margins*2;

		private readonly DataAccessor			accessor;

		private FrameBox						dateFrame;
		private DateController					dateController;
	}
}