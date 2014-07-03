//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Helpers;
using Epsitec.Cresus.Assets.App.Views;
using Epsitec.Cresus.Assets.App.Widgets;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Popups.StackedControllers
{
	public abstract class AbstractStackedController
	{
		public AbstractStackedController(DataAccessor accessor, StackedControllerDescription description)
		{
			this.accessor    = accessor;
			this.description = description;
		}


		public virtual bool						HasError
		{
			get
			{
				return false;
			}
		}


		public abstract int						RequiredHeight
		{
			//	Retourne la hauteur requise pour le contrôleur.
			get;
		}

		public virtual int						RequiredControllerWidth
		{
			//	Retourne la largeur de la partie droite requise pour le contrôleur.
			get
			{
				return this.description.Width + 4;
			}
		}
		
		public virtual int						RequiredLabelsWidth
		{
			//	Retourne la largeur de la partie gauche requise pour le contrôleur.
			get
			{
				return this.description.Label.GetTextWidth ();
			}
		}


		public StackedControllerDescription		Description
		{
			get
			{
				return this.description;
			}
		}


		public virtual void CreateUI(Widget parent, int labelWidth, ref int tabIndex, StackedControllerDescription description)
		{
		}

		public virtual void SetFocus()
		{
		}


		protected void CreateLabel(Widget parent, int labelWidth, StackedControllerDescription description)
		{
			new StaticText
			{
				Parent           = parent,
				Text             = description.Label,
				ContentAlignment = ContentAlignment.TopRight,
				TextBreakMode    = TextBreakMode.Ellipsis | TextBreakMode.Split | TextBreakMode.SingleLine,
				Dock             = DockStyle.Left,
				PreferredWidth   = labelWidth,
				PreferredHeight  = AbstractFieldController.lineHeight - 1,
				Margins          = new Margins (0, 10, 3, 0),
			};
		}

		protected FrameBox CreateControllerFrame(Widget parent)
		{
			return new FrameBox
			{
				Parent    = parent,
				Dock      = DockStyle.Fill,
				BackColor = ColorManager.WindowBackgroundColor,
			};
		}


		#region Events handler
		protected void OnValueChanged(StackedControllerDescription description)
		{
			this.ValueChanged.Raise (this, description);
		}

		public event EventHandler<StackedControllerDescription> ValueChanged;
		#endregion


		protected readonly DataAccessor					accessor;
		protected readonly StackedControllerDescription	description;
	}
}