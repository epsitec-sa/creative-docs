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
	public class MultilingualEditionDialog : AbstractDialog
	{
		public MultilingualEditionDialog(AbstractTextField textField, MultilingualText multilingualText)
		{
			this.IsApplicationWindow = true;  // pour avoir les boutons Minimize/Maximize/Close !

			this.textField = textField;
			this.multilingualText = multilingualText;

			this.lines          = new List<FrameBox> ();
			this.textFields     = new List<AbstractTextField> ();
			this.defaultButtons = new List<GlyphButton> ();
			this.glyphs         = new List<Widgets.StaticGlyph> ();
			this.toolbars       = new List<FrameBox> ();
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
				ContentAlignment = Common.Drawing.ContentAlignment.MiddleCenter,
				PreferredSize = new Size (64, 64),
				Dock = DockStyle.Top,
				Margins = new Margins (0, 0, 20, 0),
			};

			var checkButton = new CheckButton
			{
				Parent = leftFrame,
				Text = "Traductions",
				ActiveState = MultilingualEditionDialog.isTraduceVisible ? ActiveState.Yes : ActiveState.No,
				Dock = DockStyle.Top,
				Margins = new Margins (0, 0, 10, 0),
			};

			checkButton.Clicked += delegate
			{
				MultilingualEditionDialog.isTraduceVisible = !MultilingualEditionDialog.isTraduceVisible;
				this.UpdateButtons ();
			};

			//	Rempli le panneau principal de droite.
			this.lines.Clear ();
			this.textFields.Clear ();
			this.defaultButtons.Clear ();
			this.glyphs.Clear ();
			this.toolbars.Clear ();

			foreach (var id in MultilingualEditionDialog.GetLanguageIds)
			{
				var desc = string.Format ("{0} {1} :", MultilingualEditionDialog.GetIcon (id), MultilingualEditionDialog.GetDescription (id));

				if (MultilingualEditionDialog.IsCurrentLanguage (id))
				{
					desc = string.Concat ("<b>", desc, "</b>");
				}

				var label = new StaticText
				{
					Parent = rightFrame,
					Text = desc,
					Dock = DockStyle.Top,
					Margins = new Margins (0, 0, 0, UI.MarginUnderLabel),
				};

				var line = new FrameBox
				{
					Parent = rightFrame,
					Dock = DockStyle.Top,
					Margins = new Margins (0, 0, 0, UI.MarginUnderTextField),
					TabIndex = tabIndex++,
				};

				this.lines.Add (line);

				AbstractTextField textField;

				if (this.IsMulti)
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

				foreach (var translateId in MultilingualEditionDialog.GetLanguageIds)
				{
					if (translateId != id)
					{
						string src = MultilingualEditionDialog.NormalizeId (id);
						string dst = MultilingualEditionDialog.NormalizeId (translateId);

						var translateButton = new IconButton
						{
							Parent = toolbar,
							Text = MultilingualEditionDialog.GetIcon (translateId),
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

				var glyph = new Widgets.StaticGlyph
				{
					Parent = line,
					GlyphShape = GlyphShape.TriangleRight,
					PreferredWidth = 22,
					Dock = DockStyle.Right,
					Margins = new Margins (5, 0, 0, 0),
				};

				this.glyphs.Add (glyph);

				GlyphButton defaultButton = null;

				if (id != MultilingualText.DefaultLanguageId)  // pas la langue par défaut ?
				{
					defaultButton = new GlyphButton
					{
						Parent = line,
						GlyphShape = Common.Widgets.GlyphShape.Close,
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

			this.RestoreTexts ();
			this.UpdateButtons ();

			//	Initialise le focus.
			if (focusedTextField == null)
			{
				focusedTextField = this.textFields[0];
			}

			focusedTextField.SelectAll ();
			focusedTextField.Focus ();
		}


		private void UpdateTextField(AbstractTextField textField)
		{
			textField.SetUndefinedLanguage (textField.Text == MultilingualEditionDialog.defaultText);
		}

		private void UpdateButtons()
		{
			for (int i = 0; i < MultilingualEditionDialog.GetLanguageIds.Count (); i++)
			{
				if (this.defaultButtons[i] != null)
				{
					this.defaultButtons[i].Enable = (this.textFields[i].Text != MultilingualEditionDialog.defaultText);
				}

				this.toolbars[i].Visibility = MultilingualEditionDialog.isTraduceVisible;
				this.glyphs[i].Visibility = MultilingualEditionDialog.isTraduceVisible;
			}

			this.acceptButton.Enable = this.isDirty;
		}

		private void AdjustGeometry(double height)
		{
			if (this.IsMulti)
			{
				double h = ((height - MultilingualEditionDialog.fixHeight) / MultilingualEditionDialog.GetLanguageIds.Count ()) - MultilingualEditionDialog.labelHeight;
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

			foreach (var id in MultilingualEditionDialog.GetLanguageIds)
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

			foreach (var id in MultilingualEditionDialog.GetLanguageIds)
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
			switch (MultilingualEditionDialog.NormalizeId (languageId))
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

			return languageId;
		}

		private static string GetDescription(string languageId)
		{
			if (languageId == MultilingualText.DefaultLanguageId)
			{
				languageId = "default";
			}

			switch (languageId)
			{
				case "default":
					return "Français (par défaut)";

				case "fr":
					return "Français";

				case "de":
					return "Allemand";

				case "en":
					return "Anglais";

				case "it":
					return "Italien";
			}

			return languageId;
		}

		private static string NormalizeId(string languageId)
		{
			if (languageId == MultilingualText.DefaultLanguageId)
			{
				languageId = "fr";
			}

			return languageId;
		}

		private static IEnumerable<string> GetLanguageIds
		{
			//	Retourne la liste des langues éditables dans le dialogue.
			get
			{
				yield return MultilingualText.DefaultLanguageId;
				yield return "de";
				yield return "en";
				yield return "it";
			}
		}


		private static readonly double					multiHeight = 10+14*4;  // hauteur pour 4 lignes
		private static readonly double					labelHeight = 18;
		private static readonly double					fixHeight = 10+10+10+22+10;
		private static readonly string					defaultText = "&lt;par défaut&gt;";  // <par défaut>

		private static bool								isTraduceVisible;

		private readonly AbstractTextField				textField;
		private readonly MultilingualText				multilingualText;
		private readonly List<FrameBox>					lines;
		private readonly List<AbstractTextField>		textFields;
		private readonly List<GlyphButton>				defaultButtons;
		private readonly List<Widgets.StaticGlyph>		glyphs;
		private readonly List<FrameBox>					toolbars;

		private bool									isDirty;
		private Button									acceptButton;
		private Button									cancelButton;
	}
}
