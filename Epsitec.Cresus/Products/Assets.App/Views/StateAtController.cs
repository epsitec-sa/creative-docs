//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Drawing;
using Epsitec.Common.Support;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Popups;
using Epsitec.Cresus.Assets.App.Widgets;
using Epsitec.Cresus.Assets.Server.Helpers;

namespace Epsitec.Cresus.Assets.App.Views
{
	public class StateAtController 
	{
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


		public void CreateUI(Widget parent)
		{
			this.mainButton = new ColoredButton
			{
				Parent        = parent,
				NormalColor   = Color.Empty,
				HoverColor    = ColorManager.HoverColor,
				AutoFocus     = false,
				Anchor        = AnchorStyles.BottomLeft,
				PreferredSize = new Size (100, AbstractScroller.DefaultBreadth),
			};

			this.mainButton.Clicked += delegate
			{
				this.ShowPopup ();
			};

			this.UpdateButton ();
		}


		private void ShowPopup()
		{
			var popup = new DatePopup
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
		}

		private string Description
		{
			get
			{
				if (this.date.HasValue)
				{
					var date = TypeConverters.DateToString (this.date);
					return "Etat au " + date;
				}
				else
				{
					return "Etat final";
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


		private System.DateTime?				date;
		private ColoredButton					mainButton;
	}
}
