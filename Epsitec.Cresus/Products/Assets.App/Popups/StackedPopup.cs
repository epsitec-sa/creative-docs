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
	public abstract class StackedPopup : AbstractPopup
	{
		public StackedPopup(DataAccessor accessor)
		{
			this.accessor = accessor;

			this.descriptions = new List<StackedControllerDescription> ();
			this.controllers = new List<AbstractStackedController> ();
		}


		protected void SetDescriptions(List<StackedControllerDescription> descriptions)
		{
			this.descriptions.Clear ();
			this.descriptions.AddRange (descriptions);

			this.CreateControllers ();
		}

		protected AbstractStackedController GetController(int rank)
		{
			return this.controllers[rank];
		}


		protected override Size DialogSize
		{
			get
			{
				int w = StackedPopup.margin*2 + this.LabelsWidth + this.ControllersWidth;
				int h = StackedPopup.margin*2 + this.Height + 50;

				return new Size (w, h);
			}
		}


		public override void CreateUI()
		{
			this.CreateControllersUI (this.mainFrameBox);
			this.CreateButtons ();
		}

		private void CreateControllers()
		{
			this.controllers.Clear ();

			foreach (var description in this.descriptions)
			{
				var controller = StackedControllerDescription.CreateController (this.accessor, description);
				this.controllers.Add (controller);
			}
		}

		private void CreateControllersUI(Widget parent)
		{
			var frame = new FrameBox
			{
				Parent  = parent,
				Anchor  = AnchorStyles.All,
				Margins = new Margins (StackedPopup.margin),
			};

			int labelsWidth = this.LabelsWidth;
			int tabIndex = 0;

			for (int i=0; i<this.descriptions.Count; i++)
			{
				var description = this.descriptions[i];
				var controller = this.controllers[i];

				controller.CreateUI (frame, labelsWidth, ++tabIndex, description);

				controller.ValueChanged += delegate (object sender, StackedControllerDescription d)
				{
					this.OnValueChanged (d);
				};
			}
		}


		private int Height
		{
			get
			{
				int value = 0;

				foreach (var description in this.descriptions)
				{
					value += description.RequiredHeight + description.BottomMargin;
				}

				return value;
			}
		}

		private int ControllersWidth
		{
			get
			{
				int value = 0;

				foreach (var description in this.descriptions)
				{
					value = System.Math.Max (value, description.RequiredControllerWidth);
				}

				return value;
			}
		}

		private int LabelsWidth
		{
			get
			{
				int value = 0;

				foreach (var description in this.descriptions)
				{
					value = System.Math.Max (value, description.RequiredLabelsWidth);
				}

				return value;
			}
		}


		private void CreateButtons()
		{
			var footer = this.CreateFooter ();

			this.okButton     = this.CreateFooterButton (footer, DockStyle.Left,  "ok",     "D'accord");
			this.cancelButton = this.CreateFooterButton (footer, DockStyle.Right, "cancel", "Annuler");
		}


		private void UpdateButtons()
		{
		}


		#region Events handler
		protected void OnValueChanged(StackedControllerDescription description)
		{
			this.ValueChanged.Raise (this, description);
		}

		public event EventHandler<StackedControllerDescription> ValueChanged;
		#endregion


		private const int margin = 20;

		private readonly DataAccessor			accessor;
		private readonly List<StackedControllerDescription> descriptions;
		private readonly List<AbstractStackedController> controllers;

		private Button							okButton;
		private Button							cancelButton;
	}
}