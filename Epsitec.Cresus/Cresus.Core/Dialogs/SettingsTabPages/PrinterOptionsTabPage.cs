//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Debug;
using Epsitec.Common.Dialogs;
using Epsitec.Common.Drawing;
using Epsitec.Common.IO;
using Epsitec.Common.Printing;
using Epsitec.Common.Support;
using Epsitec.Common.Types;
using Epsitec.Common.Widgets;

using Epsitec.Common.Support.EntityEngine;
using Epsitec.Cresus.Core.Entities;

using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Epsitec.Cresus.Core.Dialogs.SettingsTabPages
{
	/// <summary>
	/// Onglet de SettingsDialog pour choisir les options d'impression pour chaque type de document.
	/// </summary>
	public class PrinterOptionsTabPage : AbstractSettingsTabPage
	{
		public PrinterOptionsTabPage(CoreApplication application)
			: base (application)
		{
			this.confirmationButtons = new List<ConfirmationButton> ();
			this.optionButtons       = new List<AbstractButton> ();
		}


		public override void AcceptChangings()
		{
		}

		public override void CreateUI(Widget parent)
		{
			int tabIndex = 0;

			var frame = new FrameBox
			{
				Parent = parent,
				Dock = DockStyle.Fill,
				Margins = new Margins (10),
			};

			var leftFrame = new FrameBox
			{
				Parent = frame,
				PreferredWidth = PrinterOptionsTabPage.panelWidth,
				Dock = DockStyle.Left,
				Margins = new Margins (0, 0, 0, 0),
			};

			this.optionsFrame = new FrameBox
			{
				Parent = frame,
				DrawFullFrame = true,
				DrawFrameState = FrameState.All,
				DrawFrameWidth = 1,
				PreferredWidth = PrinterOptionsTabPage.panelWidth,
				Dock = DockStyle.Left,
				Margins = new Margins (10, 0, 0, 0),
				Padding = new Margins (10),
			};

			//	Rempli le panneau de gauche.
#if false
			this.confirmationButtons.Clear ();

			this.entityPrintingSettings.DocumentTypeSelected = DocumentTypeDefinition.StringToType (this.GetSettings (true, "SelectedType"));

			foreach (var documentType in this.entityPrinter.DocumentTypes)
			{
				var button = new ConfirmationButton
				{
					Parent = leftFrame,
					Name = DocumentTypeDefinition.TypeToString (documentType.Type),
					Text = ConfirmationButton.FormatContent (documentType.ShortDescription, documentType.LongDescription),
					PreferredHeight = 52,
					Dock = DockStyle.Top,
					TabIndex = ++tabIndex,
				};

				button.Clicked += delegate
				{
					this.entityPrintingSettings.DocumentTypeSelected = DocumentTypeDefinition.StringToType (button.Name);
					this.SetSettings (true, "SelectedType", DocumentTypeDefinition.TypeToString (this.entityPrintingSettings.DocumentTypeSelected));
					this.UpdateWidgets ();
					this.UpdatePreview ();
				};

				this.confirmationButtons.Add (button);
			}


			//	Connection des événements.


			this.UpdateWidgets ();
#endif
		}

		private void UpdateWidgets()
		{
		}

		private void UpdateOptions(FrameBox parent)
		{
		}


		private static readonly double panelWidth = 250;

		private FrameBox								centerBox;
		private FrameBox								optionsFrame;
		private List<ConfirmationButton>				confirmationButtons;
		private List<AbstractButton>					optionButtons;
	}
}
