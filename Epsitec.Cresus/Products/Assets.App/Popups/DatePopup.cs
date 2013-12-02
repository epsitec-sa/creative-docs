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

		public override void CreateUI()
		{
			this.CreateTitle ("Choix d'une date");

			this.CreateDateUI ();
			this.CreateCloseButton ();
		}

		private void CreateDateUI()
		{
			this.dateFrame = this.CreateFrame (DatePopup.margins, DatePopup.margins, DateController.ControllerWidth, DateController.ControllerHeight);

			this.dateController = new DateController (this.accessor)
			{
				Date            = this.Date,
				DateLabelWidth  = 0,
				DateDescription = null,
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


		private static readonly int margins      = 20;
		private static readonly int dialogWidth  = DateController.ControllerWidth  + DatePopup.margins*2;
		private static readonly int dialogHeight = AbstractPopup.TitleHeight + DateController.ControllerHeight + DatePopup.margins*2;

		private readonly DataAccessor			accessor;

		private FrameBox						dateFrame;
		private DateController					dateController;
	}
}