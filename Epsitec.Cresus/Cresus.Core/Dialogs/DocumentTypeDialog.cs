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
using Epsitec.Cresus.Core.Printers;

using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Epsitec.Cresus.Core.Dialogs
{

	class DocumentTypeDialog : AbstractDialog
	{
		public DocumentTypeDialog(CoreApplication application, Printers.AbstractEntityPrinter entityPrinter, IEnumerable<AbstractEntity> entities, bool isPreview)
		{
			this.application   = application;
			this.entityPrinter = entityPrinter;
			this.entities      = entities;
			this.isPreview     = isPreview;

			this.confirmationButtons = new List<ConfirmationButton> ();
			this.optionButtons = new List<AbstractButton> ();
		}


		protected override Window CreateWindow()
		{
			Window window = new Window ();

			this.SetupWindow  (window);
			this.SetupWidgets (window);
			this.SetupEvents  (window);
			this.UpdateWidgets ();
			this.UpdatePreview ();

			window.AdjustWindowSize ();

			return window;
		}

		private void SetupWindow(Window window)
		{
			double width = 300;
			double height = (int) (width*297/210);  // place une une page A4 verticale

			this.OwnerWindow = this.application.Window;
			window.Icon = this.application.Window.Icon;
			window.Text = "Choix du type de document";
			window.ClientSize = new Size (10+(width+5)*3+10, 10+height+40);
			window.MakeFloatingWindow ();
		}

		private void SetupWidgets(Window window)
		{
			var frame = new FrameBox
			{
				Parent = window.Root,
				Anchor = AnchorStyles.All,
				ContainerLayoutMode = Common.Widgets.ContainerLayoutMode.HorizontalFlow,
				Margins = new Margins (10, 10, 10, 40),
			};

			var left = new FrameBox
			{
				Parent = frame,
				Dock = DockStyle.Fill,
				Margins = new Margins (0, 5, 0, 0),
			};

			this.optionsFrame = new FrameBox
			{
				Parent = frame,
				DrawFullFrame = true,
				DrawFrameState = FrameState.All,
				DrawFrameWidth = 1,
				Dock = DockStyle.Fill,
				Margins = new Margins (5, 5, 0, 0),
				Padding = new Margins (10),
			};

			this.preview = new Widgets.PreviewEntity
			{
				Parent = frame,
				Dock = DockStyle.Fill,
				Margins = new Margins (5, 0, 0, 0),
			};

			this.confirmationButtons.Clear ();
			int tabIndex = 0;

			foreach (var documentType in this.entityPrinter.DocumentTypes)
			{
				var button = new ConfirmationButton
				{
					Parent = left,
					Name = documentType.Name,
					Text = ConfirmationButton.FormatContent (documentType.ShortDescription, documentType.LongDescription),
					PreferredHeight = 52,
					Dock = DockStyle.Top,
					TabIndex = ++tabIndex,
				};

				button.Clicked += delegate
				{
					this.entityPrinter.DocumentTypeSelected = button.Name;
					this.UpdateWidgets ();
					this.UpdatePreview ();
				};

				this.confirmationButtons.Add (button);
			}

			var footer = new FrameBox
			{
				Parent = window.Root,
				PreferredHeight = 20,
				Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom,
				Margins = new Margins (10, 10, 0, 10),
			};

			this.cancelButton = new Button ()
			{
				Parent = footer,
				Text = "Annuler",
				Dock = DockStyle.Right,
				Margins = new Margins (10, 0, 0, 0),
				TabIndex = 1,
			};

			this.acceptButton = new Button ()
			{
				Parent = footer,
				Text = this.isPreview ? "Aperçu" : "Imprimer",
				Dock = DockStyle.Right,
				TabIndex = 1,
			};
		}

		private void SetupEvents(Window window)
		{
			this.acceptButton.Clicked += delegate
			{
				this.Result = DialogResult.Accept;
				this.CloseDialog ();
			};

			this.cancelButton.Clicked += delegate
			{
				this.Result = DialogResult.Cancel;
				this.CloseDialog ();
			};
		}

		private void UpdateWidgets()
		{
			IAdorner adorner = Common.Widgets.Adorners.Factory.Active;

			foreach (var button in this.confirmationButtons)
			{
				if (button.Name == this.entityPrinter.DocumentTypeSelected)
				{
					button.ButtonStyle = ButtonStyle.ActivableIcon;
					button.SetSelected (true);
					button.ActiveState = ActiveState.Yes;
					button.BackColor = adorner.ColorCaption;
				}
				else
				{
					button.ButtonStyle = ButtonStyle.Confirmation;
					button.SetSelected (false);
					button.ActiveState = ActiveState.No;
					button.BackColor = Color.Empty;
				}
			}

			this.acceptButton.Enable = !string.IsNullOrEmpty (this.entityPrinter.DocumentTypeSelected);

			this.UpdateOptions ();
		}

		private void UpdateOptions()
		{
			this.optionsFrame.Children.Clear ();
			this.optionButtons.Clear ();

			var documentType = this.GetDocumentType (this.entityPrinter.DocumentTypeSelected);
			if (documentType != null)
			{
				if (documentType.DocumentOptions.Count == 0)
				{
					var title = new StaticText
					{
						Parent = this.optionsFrame,
						Text = "<font size=\"30\" color=\"#ffffff\"><i>Aucune option</i></font>",
						ContentAlignment = Common.Drawing.ContentAlignment.MiddleCenter,
						Dock = DockStyle.Fill,
					};
				}
				else
				{
					var title = new StaticText
					{
						Parent = this.optionsFrame,
						Text = "<font size=\"16\">Options disponibles</font>",
						ContentAlignment = Common.Drawing.ContentAlignment.TopLeft,
						PreferredHeight = 30,
						Dock = DockStyle.Top,
					};
				}

				int tabIndex = 0;

				foreach (var option in documentType.DocumentOptions)
				{
					if (option.IsTitle)
					{
						var title = new StaticText
						{
							Parent = this.optionsFrame,
							Text = option.Title,
							Dock = DockStyle.Top,
							Margins = new Margins (0, 0, 10, 5),
						};
					}
					else if (option.IsMargin)
					{
						var margin = new FrameBox
						{
							Parent = this.optionsFrame,
							PreferredHeight = option.Height,
							Dock = DockStyle.Top,
						};
					}
					else
					{
						if (string.IsNullOrEmpty (option.RadioName))
						{
							var check = new CheckButton
							{
								Parent = this.optionsFrame,
								Name = option.Name,
								Text = option.Description,
								ActiveState = option.DefautState ? ActiveState.Yes : ActiveState.No,
								Dock = DockStyle.Top,
								AutoToggle = false,
								TabIndex = ++tabIndex,
							};

							check.Clicked += delegate
							{
								if (check.ActiveState == ActiveState.No)
								{
									check.ActiveState = ActiveState.Yes;

								}
								else
								{
									check.ActiveState = ActiveState.No;
								}

								this.UpdateSelectedOptions ();
								this.UpdatePreview ();
							};

							this.optionButtons.Add (check);
						}
						else
						{
							var radio = new RadioButton
							{
								Parent = this.optionsFrame,
								Name = option.Name,
								Group = option.RadioName,
								Text = option.Description,
								ActiveState = option.DefautState ? ActiveState.Yes : ActiveState.No,
								Dock = DockStyle.Top,
								AutoToggle = false,
								TabIndex = ++tabIndex,
							};

							radio.Clicked += delegate
							{
								this.SetRadio (radio.Name);
								this.UpdatePreview ();
							};

							this.optionButtons.Add (radio);
						}
					}
				}
			}

			this.UpdateSelectedOptions ();
		}

		private void SetRadio(string name)
		{
			var documentType = this.GetDocumentType (this.entityPrinter.DocumentTypeSelected);

			string radioName = null;
			var documentOption = this.GetDocumentOption (documentType, name);
			if (documentOption != null)
			{
				radioName = documentOption.RadioName;
			}

			foreach (var button in this.optionButtons)
			{
				var option = this.GetDocumentOption (documentType, button.Name);

				if (option.RadioName == radioName)
				{
					if (option.Name == name)
					{
						button.ActiveState = ActiveState.Yes;
					}
					else
					{
						button.ActiveState = ActiveState.No;
					}
				}
			}

			this.UpdateSelectedOptions ();
		}

		private void UpdateSelectedOptions()
		{
			this.entityPrinter.DocumentOptionsSelected.Clear ();

			foreach (var button in this.optionButtons)
			{
				if (button.ActiveState == ActiveState.Yes)
				{
					this.entityPrinter.DocumentOptionsSelected.Add (button.Name);
				}
			}
		}


		private void UpdatePreview()
		{
			if (!string.IsNullOrEmpty (this.entityPrinter.DocumentTypeSelected))
			{
				this.entityPrinter.Clear ();
				this.preview.BuildSections (this.entityPrinter);
				this.preview.Invalidate ();  // pour forcer le dessin
			}
		}


		private DocumentOption GetDocumentOption(DocumentType documentType, string name)
		{
			foreach (var documentOption in documentType.DocumentOptions)
			{
				if (documentOption.Name == name)
				{
					return documentOption;
				}
			}

			return null;
		}

		private DocumentType GetDocumentType(string name)
		{
			foreach (var documentType in this.entityPrinter.DocumentTypes)
			{
				if (documentType.Name == name)
				{
					return documentType;
				}
			}

			return null;
		}


		private readonly CoreApplication application;
		private readonly IEnumerable<AbstractEntity> entities;
		private readonly Printers.AbstractEntityPrinter entityPrinter;
		private readonly bool isPreview;

		private List<ConfirmationButton> confirmationButtons;
		private List<AbstractButton> optionButtons;
		private FrameBox optionsFrame;
		private Widgets.PreviewEntity preview;
		private Button acceptButton;
		private Button cancelButton;
	}
}
