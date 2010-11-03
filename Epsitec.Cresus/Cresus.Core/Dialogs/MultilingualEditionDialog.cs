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
	class MultilingualEditionDialog : AbstractDialog
	{
		public MultilingualEditionDialog(AbstractTextField textField, MultilingualText multilingualText)
		{
			this.IsApplicationWindow = true;  // pour avoir les boutons Minimize/Maximize/Close !

			this.textField = textField;
			this.multilingualText = multilingualText;

			this.textFields = new List<AbstractTextField> ();
		}


		protected override Window CreateWindow()
		{
			Window window = new Window ();

			this.SetupWindow  (window);
			this.SetupWidgets (window);

			window.AdjustWindowSize ();

			return window;
		}

		private void SetupWindow(Window window)
		{
			this.OwnerWindow = this.textField.Window;

			window.Icon = this.textField.Window.Icon;
			window.Text = "Edition multilingue";
			window.ClientSize = new Size (this.IsMulti ? 500 : 400, this.RequiredHeight);
			window.Root.WindowStyles = WindowStyles.DefaultDocumentWindow;  // pour avoir les boutons Minimize/Maximize/Close !

			window.WindowCloseClicked += delegate
			{
				this.OnDialogClosed ();
				this.CloseDialog ();
			};
		}

		private double RequiredHeight
		{
			get
			{
				if (this.IsMulti)
				{
					return MultilingualEditionDialog.fixHeight + MultilingualEditionDialog.GetLanguageIds.Count () * (MultilingualEditionDialog.labelHeight+MultilingualEditionDialog.multiHeight);
				}
				else
				{
					return MultilingualEditionDialog.fixHeight + MultilingualEditionDialog.GetLanguageIds.Count () * (MultilingualEditionDialog.labelHeight+22);
				}
			}
		}

		private void SetupWidgets(Window window)
		{
			AbstractTextField focusedTextField = null;
			int tabIndex = 1;

			var frame = new FrameBox
			{
				Parent = window.Root,
				Dock = DockStyle.Fill,
				TabIndex = tabIndex++,
			};

			frame.SizeChanged += delegate
			{
				this.AdjustGeometry (10 + frame.ActualHeight + 40);
			};

			var leftFrame = new FrameBox
			{
				Parent = frame,
				Dock = DockStyle.Left,
				Padding = new Margins (10),
				TabIndex = tabIndex++,
			};

			new Separator
			{
				Parent = frame,
				PreferredWidth = 1,
				Dock = DockStyle.Left,
			};

			var rightFrame = new FrameBox
			{
				Parent = frame,
				Dock = DockStyle.Fill,
				Padding = new Margins (10, 10, 10, 0),
				TabIndex = tabIndex++,
			};

			//	Rempli le panneau de gauche.
			var icon = new StaticText
			{
				Parent = leftFrame,
				Text = @"<img src=""manifest:Epsitec.Common.Widgets.Images.Cmd.MultilingualEdition.icon"" dx=""64"" dy=""64""/>",
				PreferredSize = new Size (64, 64),
				Dock = DockStyle.Top,
				Margins = new Margins (0, 0, 10, 0),
			};

			//	Rempli le panneau principal de droite.
			this.textFields.Clear ();

			foreach (var id in MultilingualEditionDialog.GetLanguageIds)
			{
				var desc = string.Format ("{0} {1} :", MultilingualEditionDialog.GetIcon (id), MultilingualEditionDialog.GetDescription (id));
				var text = this.multilingualText.GetText (id);

				if (MultilingualEditionDialog.IsCurrentLanguage (id))
				{
					desc = string.Concat ("<b>", desc, "</b>");
				}

				var label = new StaticText
				{
					Parent = rightFrame,
					Text = desc,
					Dock = DockStyle.Top,
					Margins = new Margins (0, 0, 0, UIBuilder.MarginUnderLabel),
				};

				AbstractTextField textField;

				if (this.IsMulti)
				{
					textField = new TextFieldMulti
					{
						Parent = rightFrame,
						FormattedText = text.GetValueOrDefault (),
						PreferredHeight = MultilingualEditionDialog.multiHeight,
						Dock = DockStyle.Top,
						Margins = new Margins (0, 0, 0, UIBuilder.MarginUnderTextField),
						TabIndex = tabIndex++,
					};
				}
				else
				{
					textField = new TextField
					{
						Parent = rightFrame,
						FormattedText = text.GetValueOrDefault (),
						Dock = DockStyle.Top,
						Margins = new Margins (0, 0, 0, UIBuilder.MarginUnderTextField),
						TabIndex = tabIndex++,
					};
				}

				textField.TextChanged += delegate
				{
					this.isDirty = true;
					this.UpdateButtons ();
				};

				this.textFields.Add (textField);

				if (MultilingualEditionDialog.IsCurrentLanguage (id))
				{
					focusedTextField = textField;
				}
			}

			//	Rempli le pied de page.
			var footer = new FrameBox
			{
				Parent = window.Root,
				PreferredHeight = 20,
				Dock = DockStyle.Bottom,
				Padding = new Margins (10),
				TabIndex = tabIndex++,
			};

			this.cancelButton = new Button ()
			{
				Parent = footer,
				Text = "Annuler",
				ButtonStyle = Common.Widgets.ButtonStyle.DefaultCancel,
				Dock = DockStyle.Right,
				Margins = new Margins (10, 0, 0, 0),
				TabIndex = 101,
			};

			this.acceptButton = new Button ()
			{
				Parent = footer,
				Text = "D'accord",
				ButtonStyle = Common.Widgets.ButtonStyle.DefaultAccept,
				Dock = DockStyle.Right,
				Margins = new Margins (64+10+10, 0, 0, 0),
				TabIndex = 100,
			};

			this.acceptButton.Clicked += delegate
			{
				this.SaveTexts ();

				this.Result = DialogResult.Accept;
				this.CloseDialog ();
			};

			this.cancelButton.Clicked += delegate
			{
				this.Result = DialogResult.Cancel;
				this.CloseDialog ();
			};

			new Separator
			{
				Parent = window.Root,
				PreferredHeight = 1,
				Dock = DockStyle.Bottom,
			};

			this.UpdateButtons ();

			//	Initialise le focus.
			if (focusedTextField == null)
			{
				focusedTextField = this.textFields[0];
			}

			focusedTextField.SelectAll ();
			focusedTextField.Focus ();
		}


		private void UpdateButtons()
		{
			this.acceptButton.Enable = this.isDirty;
		}

		private void AdjustGeometry(double height)
		{
			if (this.IsMulti)
			{
				double h = ((height - MultilingualEditionDialog.fixHeight) / MultilingualEditionDialog.GetLanguageIds.Count ()) - MultilingualEditionDialog.labelHeight;
				h = System.Math.Floor (h);
				h = System.Math.Max (h, 10+14*3);  // 3 lignes au minimum

				foreach (var textField in this.textFields)
				{
					textField.PreferredHeight = h;
				}
			}
		}

		private void SaveTexts()
		{
			int index = 0;

			foreach (var id in MultilingualEditionDialog.GetLanguageIds)
			{
				this.multilingualText.SetText (id, this.textFields[index++].FormattedText);
			}
		}

		protected override void OnDialogClosed()
		{
			base.OnDialogClosed ();
		}


		private bool IsMulti
		{
			get
			{
				return this.textField is TextFieldMulti ||
					   this.textField is TextFieldMultiEx;
			}
		}


		private static bool IsCurrentLanguage (string languageId)
		{
			if (languageId == UI.Settings.CultureForData.LanguageId)
			{
				return true;
			}

			if (MultilingualText.DefaultLanguageId == languageId && !UI.Settings.CultureForData.HasLanguageId)
			{
				return true;
			}

			return false;
		}

		private static string GetIcon(string languageId)
		{
			switch (languageId)
			{
				case "*":
				case "fr":
					return @"<img src=""manifest:Epsitec.Common.Widgets.Images.Flags.FlagFR.icon""/>";

				case "de":
					return @"<img src=""manifest:Epsitec.Common.Widgets.Images.Flags.FlagDE.icon""/>";

				case "us":
					return @"<img src=""manifest:Epsitec.Common.Widgets.Images.Flags.FlagUS.icon""/>";

				case "it":
					return @"<img src=""manifest:Epsitec.Common.Widgets.Images.Flags.FlagIT.icon""/>";
			}

			return languageId;
		}

		private static string GetDescription(string languageId)
		{
			switch (languageId)
			{
				case "*":
					return "Français (par défaut)";

				case "fr":
					return "Français";

				case "de":
					return "Allemand";

				case "us":
					return "Anglais";

				case "it":
					return "Italien";
			}

			return languageId;
		}

		private static IEnumerable<string> GetLanguageIds
		{
			//	Retourne la liste des langues éditables dans le dialogue.
			get
			{
				yield return "*";
				yield return "de";
				yield return "us";
				yield return "it";
			}
		}


		private static readonly double					multiHeight = 10+14*4;  // hauteur pour 4 lignes
		private static readonly double					labelHeight = 18;
		private static readonly double					fixHeight = 10+10+10+22+10;

		private readonly AbstractTextField				textField;
		private readonly MultilingualText				multilingualText;
		private readonly List<AbstractTextField>		textFields;

		private bool									isDirty;
		private Button									acceptButton;
		private Button									cancelButton;
	}
}
