//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;
using Epsitec.Common.Types;

using Epsitec.Cresus.Compta.Controllers;
using Epsitec.Cresus.Compta.Entities;
using Epsitec.Cresus.Compta.Helpers;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.Dialogs
{
	public class DébitCréditDialog : AbstractDialog
	{
		/// <summary>
		/// Demande s'il faut passer la TVA sur le compte au débit ou au crédit.
		/// </summary>
		/// <param name="controller"></param>
		public DébitCréditDialog(AbstractController controller, ComptaCompteEntity débit, ComptaCompteEntity crédit)
			: base (controller)
		{
			this.débit  = débit;
			this.crédit = crédit;
		}


		public bool IsDébit
		{
			get;
			private set;
		}

		public bool IsCrédit
		{
			get;
			private set;
		}
		
		
		public override void Show()
		{
			//	Crée et montre la fenêtre du dialogue.
			if ( this.window == null )
			{
				this.window = new Window();
				//?this.window.Icon = this.mainWindowController.Icon;
				this.window.MakeSecondaryWindow ();
				this.window.MakeFixedSizeWindow ();
				this.window.Root.WindowStyles = WindowStyles.HasCloseButton;
				//?this.window.PreventAutoClose = true;
				this.WindowInit ("DébitCréditDialog", 320, 130, true);
				this.window.Text = "Question";
				this.window.Owner = this.parentWindow;
				this.window.Root.Padding = new Margins (10-1, 10-1, 10, 10);

				new StaticText
				{
					Parent  = this.window.Root,
					Text    = "Sur quel compte faut-il passer la TVA ?",
					Dock    = DockStyle.Top,
					Margins = new Margins (1, 1, 0, 0),
				};

				//	Informations (tout en bas).
				var info = new FrameBox
				{
					Parent              = this.window.Root,
					PreferredHeight     = 20+1+20,
					ContainerLayoutMode = ContainerLayoutMode.HorizontalFlow,
					Dock                = DockStyle.Bottom,
					Margins = new Margins (0, 0, 2, 0),
				};

				this.CreateInfo (info, this.débit);
				this.CreateInfo (info, this.crédit);

				//	Boutons de fermeture (sur les informations).
				var footer = new FrameBox
				{
					Parent              = this.window.Root,
					ContainerLayoutMode = ContainerLayoutMode.HorizontalFlow,
					PreferredHeight     = 30,
					Dock                = DockStyle.Bottom,
				};

				var débitButton = new Button
				{
					Parent        = footer,
					FormattedText = FormattedText.Concat ("Débit").ApplyFontSize (15.0),
					Dock          = DockStyle.Fill,
					Margins       = new Margins (1, 1, 0, 0),
				};

				var créditButton = new Button
				{
					Parent        = footer,
					FormattedText = FormattedText.Concat ("Crédit").ApplyFontSize (15.0),
					Dock          = DockStyle.Fill,
					Margins       = new Margins (1, 1, 0, 0),
				};

				débitButton.Clicked += delegate
				{
					this.IsDébit = true;
					this.parentWindow.MakeActive ();
					this.window.Hide ();
					this.OnClosed ();
				};

				créditButton.Clicked += delegate
				{
					this.IsCrédit = true;
					this.parentWindow.MakeActive ();
					this.window.Hide ();
					this.OnClosed ();
				};
			}

			this.IsDébit  = false;
			this.IsCrédit = false;

			this.window.ShowDialog();
		}

		private void CreateInfo(Widget parent, ComptaCompteEntity compte)
		{
			var color = Color.Empty;

			if (compte != null)
			{
				//	On met la couleur verte aux comptes de charge/produit pour inciter à les utiliser.
				//	A l'inverse, les comptes actif/passif ont la couleur rouge, pour décourager de les choisir.
				if (compte.Catégorie == CatégorieDeCompte.Charge ||
					compte.Catégorie == CatégorieDeCompte.Produit)
				{
					color = UIBuilder.CompteYesColor;
				}
				else
				{
					color = UIBuilder.CompteNoColor;
				}
			}

			var frame = new FrameBox
			{
				Parent        = parent,
				DrawFullFrame = true,
				BackColor     = color,
				Dock          = DockStyle.Fill,
				Margins = new Margins (1, 1, 0, 0),
			};

			new StaticText
			{
				Parent           = frame,
				FormattedText    = (compte == null) ? FormattedText.Empty : compte.Numéro.ApplyBold (),
				ContentAlignment = ContentAlignment.MiddleLeft,
				PreferredHeight  = 20,
				TextBreakMode    = TextBreakMode.Ellipsis | TextBreakMode.Split | TextBreakMode.SingleLine,
				Dock             = DockStyle.Top,
				Margins          = new Margins (5, 0, 0, 0),
			};

			new Separator
			{
				Parent           = frame,
				IsHorizontalLine = true,
				PreferredHeight  = 1,
				Dock             = DockStyle.Top,
			};

			new StaticText
			{
				Parent           = frame,
				FormattedText    = (compte == null) ? FormattedText.Empty : compte.Titre,
				ContentAlignment = ContentAlignment.MiddleLeft,
				PreferredHeight  = 20,
				TextBreakMode    = TextBreakMode.Ellipsis | TextBreakMode.Split | TextBreakMode.SingleLine,
				Dock             = DockStyle.Top,
				Margins          = new Margins (5, 0, 0, 0),
			};
		}


		private ComptaCompteEntity		débit;
		private ComptaCompteEntity		crédit;
	}
}
