//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Views;
using Epsitec.Cresus.Assets.Server.NaiveEngine;

namespace Epsitec.Cresus.Assets.App.Popups
{
	public class HistoryPopup : AbstractPopup
	{
		public HistoryPopup(DataAccessor accessor, Guid objectGuid, Timestamp? timestamp, int field)
		{
			this.controller = new HistoryController (accessor, objectGuid, timestamp, field);
		}


		protected override Size DialogSize
		{
			get
			{
				//	On calcule une hauteur adaptée au contenu, mais qui ne dépasse
				//	évidement pas la hauteur de la fenêtre principale.
				var parent = this.GetParent ();

				double h = parent.ActualHeight
						 - HistoryController.HeaderHeight
						 - AbstractScroller.DefaultBreadth;

				//	Utilise au maximum les 3/4 de la hauteur.
				int max = (int) (h*0.75) / HistoryController.RowHeight;

				int rows = System.Math.Min (this.controller.RowsCount, max);
					rows = System.Math.Max (rows, 3);

				int dx = HistoryController.DateColumnWidth
					   + HistoryController.ValueColumnWidth
					   + (int) AbstractScroller.DefaultBreadth;

				int dy = HistoryController.TitleHeight
					   + HistoryController.HeaderHeight
					   + rows * HistoryController.RowHeight
					   + (int) AbstractScroller.DefaultBreadth;

				return new Size (dx, dy);
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


		private readonly HistoryController		controller;
	}
}