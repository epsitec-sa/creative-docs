//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Widgets;
using Epsitec.Cresus.Assets.App.Helpers;
using Epsitec.Common.Support;

namespace Epsitec.Cresus.Assets.App.Popups
{
	/// <summary>
	/// Ce contrôleur gère la ligne de recherche présente en bas de certains popups.
	/// </summary>
	public abstract class AbstractFilterController
	{
		public string							Filter
		{
			//	Retourne le texte de recherche donné par l'utilisateur.
			get
			{
				if (this.filterField == null)
				{
					return null;
				}
				else
				{
					return this.filterField.Text;
				}
			}
		}


		public void CreateUI(Widget parent)
		{
			//	Crée la partie inférieure permettant la saisie d'un filtre.
			bool hasOptions = this.CreateOptionsUI (parent);

			var footer = new FrameBox
			{
				Parent          = parent,
				PreferredHeight = AbstractFilterController.filterMargins + AbstractFilterController.filterHeight + AbstractFilterController.filterMargins,
				Dock            = DockStyle.Bottom,
				Padding         = new Margins (AbstractFilterController.filterMargins),
				BackColor       = ColorManager.WindowBackgroundColor,
			};

			var text = Res.Strings.Popup.FilterController.Label.ToString ();

			new StaticText
			{
				Parent           = footer,
				Text             = text,
				ContentAlignment = ContentAlignment.MiddleRight,
				PreferredWidth   = 10 + text.GetTextWidth (),
				Margins          = new Margins (0, 10, 0, 0),
				Dock             = DockStyle.Left,
			};

			this.filterField = new TextField
			{
				Parent           = footer,
				Dock             = DockStyle.Fill,
			};

			if (hasOptions)
			{
				this.optionsButton = new IconButton
				{
					Parent        = footer,
					AutoFocus     = false,
					Dock          = DockStyle.Right,
					PreferredSize = new Size (AbstractFilterController.filterHeight, AbstractFilterController.filterHeight),
					Margins       = new Margins (10, 0, 0, 0),
				};
			}

			var clearButton = new IconButton
			{
				Parent        = footer,
				IconUri       = Misc.GetResourceIconUri ("Field.Delete"),
				AutoFocus     = false,
				Dock          = DockStyle.Right,
				PreferredSize = new Size (AbstractFilterController.filterHeight, AbstractFilterController.filterHeight),
				Margins       = new Margins (2, 0, 0, 0),
				Enable        = false,
			};

			this.UpdateOptions ();

			//	Connexions des événements.
			this.filterField.TextChanged += delegate
			{
				this.OnFilterChanged ();
				clearButton.Enable = !string.IsNullOrEmpty (this.filterField.Text);
			};

			clearButton.Clicked += delegate
			{
				this.filterField.Text = null;
			};

			if (this.optionsButton != null)
			{
				this.optionsButton.Clicked += delegate
				{
					this.OptionsVisibility = !this.OptionsVisibility;
				};
			}

			this.filterField.Focus ();
		}

		protected virtual bool CreateOptionsUI(Widget parent)
		{
			//	Crée l'éventuelle interface pour les options.
			return false;
		}

		protected virtual void UpdateOptions()
		{
			//	Met à jour les options.
		}

		protected virtual bool					OptionsVisibility
		{
			get
			{
				return false;
			}
			set
			{
			}
		}


		protected void UpdateOptionsButton()
		{
			if (this.OptionsVisibility)
			{
				this.optionsButton.IconUri  = Misc.GetResourceIconUri ("Triangle.Down");
			}
			else
			{
				this.optionsButton.IconUri  = Misc.GetResourceIconUri ("Triangle.Up");
			}
		}


		#region Events handler
		protected void OnFilterChanged()
		{
			this.FilterChanged.Raise (this);
		}

		public event EventHandler FilterChanged;
		#endregion


		public const int height = AbstractFilterController.filterMargins*2 + AbstractFilterController.filterHeight;

		protected const int filterMargins = 5;
		protected const int filterHeight  = 20;

		protected TextField								filterField;
		protected IconButton							optionsButton;
	}
}