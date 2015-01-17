//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Drawing;
using Epsitec.Common.Support;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Popups;
using Epsitec.Cresus.Assets.App.Widgets;
using Epsitec.Cresus.Assets.Core.Helpers;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Views
{
	public class StateAtController 
	{
		public StateAtController(DataAccessor accessor)
		{
			this.accessor = accessor;
		}

	
		public System.DateTime? Date
		{
			get
			{
				return this.date;
			}
			set
			{
				if (this.date != value)
				{
					this.date = value;

					this.UpdateButton ();
					this.OnDateChanged ();
				}
			}
		}


		public bool Visibility
		{
			get
			{
				return this.mainButton.Visibility;
			}
			set
			{
				this.mainButton.Visibility = value;
			}
		}


		public Widget CreateUI(Widget parent)
		{
			this.mainButton = new ColoredButton
			{
				Parent        = parent,
				HoverColor    = ColorManager.HoverColor,
				AutoFocus     = false,
				PreferredSize = new Size (100, AbstractScroller.DefaultBreadth),
			};

			this.mainButton.Clicked += delegate
			{
				this.ShowPopup ();
			};

			this.UpdateButton ();

			return this.mainButton;
		}


		private void ShowPopup()
		{
			var popup = new DatePopup (this.accessor)
			{
				Date = this.date,
			};

			popup.Create (this.mainButton, leftOrRight: true);

			popup.DateChanged += delegate
			{
				this.Date = popup.Date;
			};
		}

		private void UpdateButton()
		{
			this.mainButton.Text = this.Description;
			//?this.mainButton.NormalColor = this.date.HasValue ? ColorManager.SelectionColor : Color.Empty;
		}

		private string Description
		{
			get
			{
				if (this.date.HasValue)
				{
					var date = TypeConverters.DateToString (this.date);
					return string.Format (Res.Strings.StateAtController.Date.ToString (), date);
				}
				else
				{
					return Res.Strings.StateAtController.Final.ToString ();
				}
			}
		}


		#region Events handler
		private void OnDateChanged()
		{
			this.DateChanged.Raise (this);
		}

		public event EventHandler DateChanged;
		#endregion


		private readonly DataAccessor			accessor;

		private System.DateTime?				date;
		private ColoredButton					mainButton;
	}
}
