//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Dialogs;
using Epsitec.Common.Drawing;
using Epsitec.Common.Types;
using Epsitec.Common.Widgets;

using Epsitec.Cresus.Core.Library;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Dialogs
{
	using StaticGlyph = Widgets.StaticGlyph;

	public class MultilingualEditionDialog : AbstractDialog
	{
		public MultilingualEditionDialog(AbstractTextField textField, MultilingualText multilingualText)
		{
			this.IsApplicationWindow = true;  // pour avoir les boutons Minimize/Maximize/Close !

			this.sourceTextField = textField;
			this.multilingualText = multilingualText;

			this.lines          = new List<FrameBox> ();
			this.textFields     = new List<AbstractTextField> ();
			this.defaultButtons = new List<GlyphButton> ();
			this.glyphs         = new List<StaticGlyph> ();
			this.toolbars       = new List<FrameBox> ();
		}


		private bool							IsMultiline
		{
			get
			{
				return this.sourceTextField is TextFieldMulti ||
					   this.sourceTextField is TextFieldMultiEx;
			}
		}

		private double							RequiredHeight
		{
			get
			{
				if (this.IsMultiline)
				{
					return MultilingualEditionDialog.fixHeight + MultilingualEditionDialog.GetTwoLetterISOLanguageNames ().Count () * (MultilingualEditionDialog.labelHeight+MultilingualEditionDialog.multiHeight);
				}
				else
				{
					return MultilingualEditionDialog.fixHeight + MultilingualEditionDialog.GetTwoLetterISOLanguageNames ().Count () * (MultilingualEditionDialog.labelHeight+22);
				}
			}
		}


		protected override Window CreateWindow()
		{
			Window window = new Window ();

			this.SetupWindow  (window);
			this.SetupWidgets (window);

			window.AdjustWindowSize ();

			return window;
		}

		protected override void OnDialogClosed()
		{
			base.OnDialogClosed ();
		}

		
		private void SetupWindow(Window window)
		{
			this.OwnerWindow = this.sourceTextField.Window;

			window.Icon = this.sourceTextField.Window.Icon;
			window.Text = "Édition multilingue";
			window.ClientSize = new Size (this.IsMultiline ? 500 : 400, this.RequiredHeight);
			window.Root.WindowStyles = WindowStyles.DefaultDocumentWindow;  // pour avoir les boutons Minimize/Maximize/Close !

			window.WindowCloseClicked += delegate
			{
				this.OnDialogClosed ();
				this.CloseDialog ();
			};
		}

		private void SetupWidgets(Window window)
		{
			var focusableTextFields = new List<AbstractTextField> ();

			var main = this.CreateMainFrame (window.Root);

			this.CreateLeftFrame (main);
			this.CreateRightFrame (main, focusableTextFields);
			this.CreateFooterFrame (window.Root);

			this.RestoreTexts ();
			this.UpdateButtons ();

			this.SetInitialFocus (focusableTextFields);
		}


		private FrameBox CreateMainFrame(Widget container)
		{
			var frame = new FrameBox
			{
				Parent = container,
				Dock = DockStyle.Fill,
				TabIndex = 1,
			};

			frame.SizeChanged += delegate
			{
				this.AdjustGeometry (10 + frame.ActualHeight + 40);
			};
			
			return frame;
		}
		
		private void CreateLeftFrame(FrameBox container)
		{
			var leftFrame = new FrameBox
			{
				Parent = container,
				Dock = DockStyle.Left,
				Padding = new Margins (10),
				TabIndex = 1,
			};

			//	Rempli le panneau de gauche.
			var icon = new StaticText
			{
				Parent = leftFrame,
				Text = @"<img src=""manifest:Epsitec.Common.Widgets.Images.Cmd.MultilingualEdition.icon"" dx=""64"" dy=""64""/>",
				ContentAlignment = ContentAlignment.MiddleCenter,
				PreferredSize = new Size (64, 64),
				Dock = DockStyle.Top,
				Margins = new Margins (0, 0, 20, 0),
			};

			var checkButton = new CheckButton
			{
				Parent = leftFrame,
				Text = "Traductions",
				ActiveState = MultilingualEditionDialog.isTranslateVisible ? ActiveState.Yes : ActiveState.No,
				Dock = DockStyle.Top,
				Margins = new Margins (0, 0, 10, 0),
				TabIndex = 1,
			};

			checkButton.Clicked += delegate
			{
				MultilingualEditionDialog.isTranslateVisible = !MultilingualEditionDialog.isTranslateVisible;
				this.UpdateButtons ();
			};
		}
		
		private void CreateRightFrame(FrameBox frame, IList<AbstractTextField> focusedTextFields)
		{
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
				TabIndex = 1,
			};

			//	Remplit le panneau principal de droite.
			this.lines.Clear ();
			this.textFields.Clear ();
			this.defaultButtons.Clear ();
			this.glyphs.Clear ();
			this.toolbars.Clear ();

			foreach (var id in MultilingualEditionDialog.GetTwoLetterISOLanguageNames ())
			{
				AbstractTextField textField = this.CreateTextField (rightFrame, id);

				if (MultilingualEditionDialog.IsCurrentLanguage (id))
				{
					focusedTextFields.Add (textField);
				}
			}
		}
		
		private void CreateFooterFrame(Widget container)
		{
			var footer = new FrameBox
			{
				Parent = container,
				PreferredHeight = 20,
				Dock = DockStyle.Bottom,
				Padding = new Margins (10),
				TabIndex = 10,
			};

			this.cancelButton = new Button ()
			{
				Parent = footer,
				Text = "Annuler",
				ButtonStyle = ButtonStyle.DefaultCancel,
				Dock = DockStyle.Right,
				Margins = new Margins (10, 0, 0, 0),
				TabIndex = 101,
			};

			this.acceptButton = new Button ()
			{
				Parent = footer,
				Text = "D'accord",
				ButtonStyle = ButtonStyle.DefaultAccept,
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
				Parent = container,
				PreferredHeight = 1,
				Dock = DockStyle.Bottom,
			};
		}

		private AbstractTextField CreateTextField(FrameBox container, string twoLetterISOLanguageName)
		{
			int tabIndex = container.Children.Count + 1;

			var desc = string.Format ("{0} {1} :", MultilingualEditionDialog.GetIcon (twoLetterISOLanguageName), MultilingualEditionDialog.GetDescription (twoLetterISOLanguageName));

			if (MultilingualEditionDialog.IsCurrentLanguage (twoLetterISOLanguageName))
			{
				desc = string.Concat ("<b>", desc, "</b>");
			}

			var label = new StaticText
			{
				Parent = container,
				Text = desc,
				Dock = DockStyle.Top,
				Margins = new Margins (0, 0, 0, Library.UI.Constants.MarginUnderLabel),
			};

			var line = new FrameBox
			{
				Parent = container,
				Dock = DockStyle.Top,
				Margins = new Margins (0, 0, 0, Library.UI.Constants.MarginUnderTextField),
				TabIndex = tabIndex++,
			};

			this.lines.Add (line);

			AbstractTextField textField;

			if (this.IsMultiline)
			{
				line.PreferredHeight = MultilingualEditionDialog.multiHeight;

				textField = new TextFieldMulti
				{
					Parent = line,
					Dock = DockStyle.Fill,
					TabIndex = tabIndex++,
				};
			}
			else
			{
				line.PreferredHeight = 22;

				textField = new TextField
				{
					Parent = line,
					Dock = DockStyle.Fill,
					TabIndex = tabIndex++,
				};
			}

			textField.TextChanged += delegate
			{
				this.UpdateTextField (textField);

				this.isDirty = true;
				this.UpdateButtons ();
			};

			this.UpdateTextField (textField);
			this.textFields.Add (textField);

			var toolbar = new FrameBox
			{
				Parent = line,
				DrawFullFrame = true,
				PreferredWidth = 20*3,
				Dock = DockStyle.Right,
			};

			this.toolbars.Add (toolbar);

			foreach (var translateTwoLetter in MultilingualEditionDialog.GetTwoLetterISOLanguageNames ())
			{
				if (translateTwoLetter != twoLetterISOLanguageName)
				{
					string src = MultilingualEditionDialog.NormalizeId (twoLetterISOLanguageName);
					string dst = MultilingualEditionDialog.NormalizeId (translateTwoLetter);

					var translateButton = new IconButton
					{
						Parent = toolbar,
						Text = MultilingualEditionDialog.GetIcon (translateTwoLetter),
						Name = string.Concat ((this.textFields.Count-1).ToString (), "-", src, "-", dst),
						Dock = DockStyle.Left,
					};

					translateButton.Clicked += delegate
					{
						this.Translate (translateButton.Name);
					};

					string srcLanguage = MultilingualEditionDialog.GetDescription (src);
					string dstLanguage = MultilingualEditionDialog.GetDescription (dst);
					ToolTip.Default.SetToolTip (translateButton, string.Format ("Traduction {0} -> {1}", srcLanguage, dstLanguage));
				}
			}

			var glyph = new StaticGlyph
			{
				Parent = line,
				GlyphShape = GlyphShape.TriangleRight,
				PreferredWidth = 22,
				Dock = DockStyle.Right,
				Margins = new Margins (5, 0, 0, 0),
			};

			this.glyphs.Add (glyph);

			GlyphButton defaultButton = null;

			if (twoLetterISOLanguageName != MultilingualText.DefaultTwoLetterISOLanguageName)  // pas la langue par défaut ?
			{
				defaultButton = new GlyphButton
				{
					Parent = line,
					GlyphShape = GlyphShape.Close,
					Name = (this.textFields.Count-1).ToString (),
					Dock = DockStyle.Right,
					Margins = new Margins (-1, 0, 0, 0),
				};

				defaultButton.Clicked += delegate
				{
					this.SetDefaultText (int.Parse (defaultButton.Name));
				};

				ToolTip.Default.SetToolTip (defaultButton, "Utilise la langue par défaut");
			}

			this.defaultButtons.Add (defaultButton);
			return textField;
		}

		private void SetInitialFocus(IList<AbstractTextField> focusedTextFields)
		{
			if (focusedTextFields.Count == 0)
			{
				focusedTextFields.Add (this.textFields[0]);
			}

			var focusedTextField = focusedTextFields.FirstOrDefault (x => x.Text != MultilingualEditionDialog.defaultText) ??
									   focusedTextFields.FirstOrDefault ();

			focusedTextField.SelectAll ();
			focusedTextField.Focus ();
		}
		
		private void UpdateTextField(AbstractTextField textField)
		{
			textField.SetUndefinedLanguage (textField.Text == MultilingualEditionDialog.defaultText);
		}

		private void UpdateButtons()
		{
			for (int i = 0; i < MultilingualEditionDialog.GetTwoLetterISOLanguageNames ().Count (); i++)
			{
				if (this.defaultButtons[i] != null)
				{
					this.defaultButtons[i].Enable = (this.textFields[i].Text != MultilingualEditionDialog.defaultText);
				}

				this.toolbars[i].Visibility = MultilingualEditionDialog.isTranslateVisible;
				this.glyphs[i].Visibility = MultilingualEditionDialog.isTranslateVisible;
			}

			this.acceptButton.Enable = this.isDirty;
		}

		private void AdjustGeometry(double height)
		{
			if (this.IsMultiline)
			{
				double h = ((height - MultilingualEditionDialog.fixHeight) / MultilingualEditionDialog.GetTwoLetterISOLanguageNames ().Count ()) - MultilingualEditionDialog.labelHeight;
				h = System.Math.Floor (h);
				h = System.Math.Max (h, 10+14*3);  // 3 lignes au minimum

				foreach (var line in this.lines)
				{
					line.PreferredHeight = h;
				}
			}
		}

		private void Translate(string name)
		{
			//	Fait appel aux services de 'google translate' pour traduire le texte.
			var t = name.Split ('-');
			int index = int.Parse (t[0]);
			string srcLanguage = t[1];
			string dstLanguage = t[2];

			string text = this.textFields[index].Text;
			text = text.Replace ("<br/>", " ");

			string url = string.Format ("http://www.google.com/translate_t?langpair={0}|{1}&text={2}", srcLanguage, dstLanguage, text);
			System.Diagnostics.Process.Start (url);
		}

		private void SetDefaultText(int index)
		{
			this.textFields[index].Text = MultilingualEditionDialog.defaultText;
		}

		private void RestoreTexts()
		{
			int index = 0;

			foreach (var id in MultilingualEditionDialog.GetTwoLetterISOLanguageNames ())
			{
				FormattedText? text = this.multilingualText.GetText (id);

				if (text.HasValue)
				{
					this.textFields[index++].FormattedText = text.Value;
				}
				else
				{
					this.textFields[index++].FormattedText = MultilingualEditionDialog.defaultText;
				}
			}
		}

		private void SaveTexts()
		{
			int index = 0;

			foreach (var id in MultilingualEditionDialog.GetTwoLetterISOLanguageNames ())
			{
				if (this.textFields[index].FormattedText == MultilingualEditionDialog.defaultText)
				{
					this.multilingualText.SetText (id, null);
				}
				else
				{
					this.multilingualText.SetText (id, this.textFields[index].FormattedText);
				}

				index++;
			}
		}



		private static bool IsCurrentLanguage(string twoLetterISOLanguageName)
		{
			if (twoLetterISOLanguageName == Library.UI.Services.Settings.CultureForData.TwoLetterISOLanguageName)
			{
				return true;
			}

			if (MultilingualText.DefaultTwoLetterISOLanguageName == twoLetterISOLanguageName)
			{
				if (!Library.UI.Services.Settings.CultureForData.HasTwoLetterISOLanguageName)
				{
					return true;
				}
				if (Library.UI.Services.Settings.CultureForData.IsDefaultLanguage (Library.UI.Services.Settings.CultureForData.TwoLetterISOLanguageName))
				{
					return true;
				}
			}



			return false;
		}

		private static string GetIcon(string twoLetterISOLanguageName)
		{
			switch (MultilingualEditionDialog.NormalizeId (twoLetterISOLanguageName))
			{
				case "fr":
					return @"<img src=""manifest:Epsitec.Common.Widgets.Images.Flags.FlagFR.icon""/>";

				case "de":
					return @"<img src=""manifest:Epsitec.Common.Widgets.Images.Flags.FlagDE.icon""/>";

				case "en":
					return @"<img src=""manifest:Epsitec.Common.Widgets.Images.Flags.FlagGB.icon""/>";

				case "it":
					return @"<img src=""manifest:Epsitec.Common.Widgets.Images.Flags.FlagIT.icon""/>";
			}

			return twoLetterISOLanguageName;
		}

		private static string GetDescription(string twoLetterISOLanguageName)
		{
			if (twoLetterISOLanguageName == MultilingualText.DefaultTwoLetterISOLanguageName)
			{
				twoLetterISOLanguageName = "default";
			}

			switch (twoLetterISOLanguageName)
			{
				case "default":
					return "Texte par défaut";

				case "fr":
					return "Français";

				case "de":
					return "Allemand";

				case "en":
					return "Anglais";

				case "it":
					return "Italien";
			}

			return twoLetterISOLanguageName;
		}

		private static string NormalizeId(string twoLetterISOLanguageName)
		{
			if (twoLetterISOLanguageName == MultilingualText.DefaultTwoLetterISOLanguageName)
			{
				twoLetterISOLanguageName = Library.UI.Services.Settings.CultureForData.TwoLetterISOLanguageNameForDefault ?? "fr";
			}

			return twoLetterISOLanguageName;
		}

		private static IEnumerable<string> GetTwoLetterISOLanguageNames()
		{
			yield return MultilingualText.DefaultTwoLetterISOLanguageName;
			yield return "fr";
			yield return "de";
			yield return "en";
			yield return "it";
		}


		private static readonly double				multiHeight = 10+14*4;  // hauteur pour 4 lignes
		private static readonly double				labelHeight = 18;
		private static readonly double				fixHeight = 10+10+10+22+10;
		private static readonly string				defaultText = "&lt;par défaut&gt;";  // <par défaut>

		private static bool							isTranslateVisible;

		private readonly AbstractTextField			sourceTextField;
		private readonly MultilingualText			multilingualText;
		private readonly List<FrameBox>				lines;
		private readonly List<AbstractTextField>	textFields;
		private readonly List<GlyphButton>			defaultButtons;
		private readonly List<StaticGlyph>			glyphs;
		private readonly List<FrameBox>				toolbars;

		private bool								isDirty;
		private Button								acceptButton;
		private Button								cancelButton;
	}
}
