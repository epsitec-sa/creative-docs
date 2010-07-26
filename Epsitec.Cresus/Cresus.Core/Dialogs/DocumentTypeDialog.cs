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

			this.settings = CoreApplication.ExtractSettings (this.SettingsPrefix);
		}


		protected override Window CreateWindow()
		{
			this.window = new Window ();

			this.SetupWindow ();
			this.SetupWidgets ();
			this.SetupEvents ();
			this.UpdateWidgets ();
			this.UpdatePreview ();

			this.window.AdjustWindowSize ();

			return this.window;
		}

		private void SetupWindow()
		{
			this.OwnerWindow = this.application.Window;
			this.window.Icon = this.application.Window.Icon;
			this.window.Text = "Choix du type de document";
			this.window.MakeFloatingWindow ();

			bool showOptions = this.GetSettings (this.GetSettingsKey ("ShowOptions")) != "No";
			bool showPreview = this.GetSettings (this.GetSettingsKey ("ShowPreview")) != "No";

			this.UpdateWindowSize (showOptions, showPreview);
		}

		private void UpdateWindowSize(bool showOptions, bool showPreview)
		{
			double width = 300;
			double height = (int) (width*297/210);  // place une une page A4 verticale

			int columns = 1;

			if (showOptions)
			{
				columns++;
			}

			if (showPreview)
			{
				columns++;
			}

			this.window.ClientSize = new Size (10 + (10+width)*columns + 10, 10 + height + 40);
			this.window.AdjustWindowSize ();
		}

		private void SetupWidgets()
		{
			var frame = new FrameBox
			{
				Parent = this.window.Root,
				Anchor = AnchorStyles.All,
				ContainerLayoutMode = Common.Widgets.ContainerLayoutMode.HorizontalFlow,
				Margins = new Margins (10, 10, 10, 40),
			};

			//	Crée les 3 panneaux côte-à-côte.
			bool showOptions = this.GetSettings (this.GetSettingsKey ("ShowOptions")) != "No";
			bool showPreview = this.GetSettings (this.GetSettingsKey ("ShowPreview")) != "No";

			var leftFrame = new FrameBox
			{
				Parent = frame,
				Dock = DockStyle.Fill,
				Margins = new Margins (0, 0, 0, 0),
			};

			this.optionsFrame = new FrameBox
			{
				Parent = frame,
				DrawFullFrame = true,
				DrawFrameState = FrameState.All,
				DrawFrameWidth = 1,
				Visibility = showOptions,
				Dock = DockStyle.Fill,
				Margins = new Margins (10, 0, 0, 0),
				Padding = new Margins (10),
			};

			this.previewFrame = new Widgets.PreviewEntity
			{
				Parent = frame,
				Visibility = showPreview,
				Dock = DockStyle.Fill,
				Margins = new Margins (10, 0, 0, 0),
			};

			//	Rempli le panneau de gauche.
			this.confirmationButtons.Clear ();
			int tabIndex = 0;

			this.entityPrinter.DocumentTypeSelected = this.GetSettings (this.GetSettingsKey ("SelectedType"));

			foreach (var documentType in this.entityPrinter.DocumentTypes)
			{
				var button = new ConfirmationButton
				{
					Parent = leftFrame,
					Name = documentType.Name,
					Text = ConfirmationButton.FormatContent (documentType.ShortDescription, documentType.LongDescription),
					PreferredHeight = 52,
					Dock = DockStyle.Top,
					TabIndex = ++tabIndex,
				};

				button.Clicked += delegate
				{
					this.entityPrinter.DocumentTypeSelected = button.Name;
					this.SetSettings (this.GetSettingsKey ("SelectedType"), this.entityPrinter.DocumentTypeSelected);
					this.UpdateWidgets ();
					this.UpdatePreview ();
				};

				this.confirmationButtons.Add (button);
			}

			//	Rempli le pied de page.
			var footer = new FrameBox
			{
				Parent = this.window.Root,
				PreferredHeight = 20,
				Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom,
				Margins = new Margins (10, 10, 0, 10),
			};

			this.showOptionsCheckButton = new CheckButton ()
			{
				Parent = footer,
				Text = "Montrer les options",
				PreferredWidth = 130,
				ActiveState = showOptions ? ActiveState.Yes : ActiveState.No,
				Dock = DockStyle.Left,
				TabIndex = 3,
			};

			this.previewCheckButton = new CheckButton ()
			{
				Parent = footer,
				Text = "Montrer l'aperçu",
				PreferredWidth = 110,
				ActiveState = showPreview ? ActiveState.Yes : ActiveState.No,
				Dock = DockStyle.Left,
				TabIndex = 4,
			};

			this.cancelButton = new Button ()
			{
				Parent = footer,
				Text = "Annuler",
				Dock = DockStyle.Right,
				Margins = new Margins (10, 0, 0, 0),
				TabIndex = 2,
			};

			this.acceptButton = new Button ()
			{
				Parent = footer,
				Text = this.isPreview ? "Aperçu" : "Imprimer",
				Dock = DockStyle.Right,
				TabIndex = 1,
			};

			this.pagesInfo = new StaticText ()
			{
				Parent = footer,
				ContentAlignment = Common.Drawing.ContentAlignment.MiddleRight,
				PreferredWidth = 100,
				Dock = DockStyle.Right,
				Margins = new Margins (0, 20, 0, 0),
			};

			this.UpdateWidgets ();
			this.UpdatePreview ();
		}

		private void SetupEvents()
		{
			this.showOptionsCheckButton.ActiveStateChanged += delegate
			{
				this.optionsFrame.Visibility = this.showOptionsCheckButton.ActiveState == ActiveState.Yes;
				this.UpdateWindowSize (this.optionsFrame.Visibility, this.previewFrame.Visibility);
				this.SetSettings (this.GetSettingsKey ("ShowOptions"), this.optionsFrame.Visibility ? "Yes" : "No");
			};

			this.previewCheckButton.ActiveStateChanged += delegate
			{
				this.previewFrame.Visibility = this.previewCheckButton.ActiveState == ActiveState.Yes;
				this.pagesInfo.Visibility = this.previewCheckButton.ActiveState == ActiveState.Yes;
				this.UpdateWindowSize (this.optionsFrame.Visibility, this.previewFrame.Visibility);
				this.SetSettings (this.GetSettingsKey ("ShowPreview"), this.previewFrame.Visibility ? "Yes" : "No");
			};

			this.acceptButton.Clicked += delegate
			{
				CoreApplication.MergeSettings (this.SettingsPrefix, this.settings);

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
							ActiveState state = option.DefautState ? ActiveState.Yes : ActiveState.No;

							string settings = this.GetSettings (this.GetSettingsKey (option.Name));
							if (!string.IsNullOrEmpty (settings))
							{
								if (settings == "No")
								{
									state = ActiveState.No;
								}

								if (settings == "Yes")
								{
									state = ActiveState.Yes;
								}
							}

							var check = new CheckButton
							{
								Parent = this.optionsFrame,
								Name = option.Name,
								Text = option.Description,
								ActiveState = state,
								Dock = DockStyle.Top,
								AutoToggle = false,
								TabIndex = ++tabIndex,
							};

							check.Clicked += delegate
							{
								if (check.ActiveState == ActiveState.No)
								{
									check.ActiveState = ActiveState.Yes;
									this.SetSettings (this.GetSettingsKey (check.Name), "Yes");

								}
								else
								{
									check.ActiveState = ActiveState.No;
									this.SetSettings (this.GetSettingsKey (check.Name), "No");
								}

								this.UpdateSelectedOptions ();
								this.UpdatePreview ();
							};

							this.optionButtons.Add (check);
						}
						else
						{
							ActiveState state = option.DefautState ? ActiveState.Yes : ActiveState.No;

							string settings = this.GetSettings (this.GetSettingsKey (option.RadioName));
							if (!string.IsNullOrEmpty (settings))
							{
								state = (settings == option.Name) ? ActiveState.Yes : ActiveState.No;
							}

							var radio = new RadioButton
							{
								Parent = this.optionsFrame,
								Name = option.Name,
								Group = option.RadioName,
								Text = option.Description,
								ActiveState = state,
								Dock = DockStyle.Top,
								AutoToggle = false,
								TabIndex = ++tabIndex,
							};

							radio.Clicked += delegate
							{
								this.SetSettings (this.GetSettingsKey (radio.Group), radio.Name);
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
				this.previewFrame.BuildSections (this.entityPrinter);
				this.previewFrame.Invalidate ();  // pour forcer le dessin

				this.pagesInfo.Text = string.Format ("{0} page{1}", this.entityPrinter.PageCount.ToString (), (this.entityPrinter.PageCount<=1)?"":"s");
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


		private string GetSettings(string key)
		{
			if (this.settings.ContainsKey (key))
			{
				return this.settings[key];
			}

			return null;
		}

		private void SetSettings(string key, string value)
		{
			this.settings[key] = value;
		}

		private string GetSettingsKey(string key)
		{
			return string.Concat (this.SettingsPrefix, ".", key);
		}

		private string SettingsPrefix
		{
			get
			{
				if (this.entities != null && this.entities.Count () != 0)
				{
					var entity = this.entities.First ();
					var type = entity.GetType ();
					var names = type.ToString ().Split ('.');
					var name = names[names.Length-1];

					return string.Concat ("DocumentTypeDialog.", name);
				}
				else
				{
					return "DocumentTypeDialog";
				}
			}
		}


		private readonly CoreApplication				application;
		private readonly IEnumerable<AbstractEntity>	entities;
		private readonly Printers.AbstractEntityPrinter	entityPrinter;
		private readonly bool							isPreview;

		private Window									window;
		private List<ConfirmationButton>				confirmationButtons;
		private List<AbstractButton>					optionButtons;
		private FrameBox								optionsFrame;
		private Widgets.PreviewEntity					previewFrame;
		private CheckButton								showOptionsCheckButton;
		private CheckButton								previewCheckButton;
		private StaticText								pagesInfo;
		private Button									acceptButton;
		private Button									cancelButton;
		private Dictionary<string, string>				settings;
	}
}
