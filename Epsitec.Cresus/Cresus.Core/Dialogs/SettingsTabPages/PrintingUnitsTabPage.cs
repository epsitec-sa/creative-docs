//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;
using Epsitec.Common.Printing;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;
using Epsitec.Common.Widgets;

using Epsitec.Cresus.Core.Documents;
using Epsitec.Cresus.Core.Documents.Verbose;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Print;

using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Epsitec.Cresus.Core.Dialogs.SettingsTabPages
{
	/// <summary>
	/// Onglet de SettingsDialog pour choisir les unités d'impression à utiliser.
	/// </summary>
	public class PrintingUnitsTabPage : AbstractSettingsTabPage
	{
		public PrintingUnitsTabPage(ISettingsDialog container)
			: base (container)
		{
			this.optionButtons = new List<CheckButton> ();
			this.printingUnitList = PrinterApplicationSettings.GetPrintingUnitList (this.Container.Data.Host);
		}


		public override void AcceptChanges()
		{
			PrinterApplicationSettings.SetPrintingUnitList (this.Container.Data.Host, this.printingUnitList);
		}

		public override void RejectChanges()
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
				Dock = DockStyle.Fill,
				Margins = new Margins (0, 4, 0, 0),
			};

			var rightFrame = new FrameBox
			{
				Parent = frame,
				PreferredWidth = 270,
				Dock = DockStyle.Right,
				Margins = new Margins (5, 0, 0, 0),
			};

			var centerFrame = new FrameBox
			{
				Parent = frame,
				PreferredWidth = 270,
				Dock = DockStyle.Right,
				Margins = new Margins (0, 0, 0, 0),
			};

			//	Rempli le panneau de gauche.
			var leftTitle = new StaticText
			{
				Parent = leftFrame,
				PreferredHeight = 20,
				Dock = DockStyle.Top,
				Text = "<font size=\"16\">Liste des unités d'impression</font>",
				Margins = new Margins (0, 0, 0, 10),
			};

			this.listController = new Controllers.ListController<PrintingUnit> (this.printingUnitList, this.ListControllerItemToText, this.ListControllerGetTextInfo, this.ListControllerCreateItem);
			this.listController.CreateUI (leftFrame, Direction.Right, 23);

			ToolTip.Default.SetToolTip (this.listController.AddButton,      "Ajoute une nouvelle unité d'impression");
			ToolTip.Default.SetToolTip (this.listController.RemoveButton,   "Supprime l'unité d'impression");
			ToolTip.Default.SetToolTip (this.listController.MoveUpButton,   "Monte l'unité d'impression dans la liste");
			ToolTip.Default.SetToolTip (this.listController.MoveDownButton, "Descend l'unité d'impression dans la liste");

			//	Rempli le panneau central.
			var centerTitle = new StaticText
			{
				Parent = centerFrame,
				PreferredHeight = 20,
				Dock = DockStyle.Top,
				Text = "<font size=\"16\">Choix pour l'unité d'impression sélectionnée</font>",
				Margins = new Margins (0, 0, 0, 10),
			};

			this.centerBox = new FrameBox
			{
				Parent = centerFrame,
				DrawFullFrame = true,
				BackColor = Widgets.ArrowedFrame.SurfaceColors.First (),
				Dock = DockStyle.Fill,
			};

			{
				var box = new FrameBox
				{
					Parent = this.centerBox,
					DrawFullFrame = true,
					Dock = DockStyle.Top,
					Padding = new Margins (10),
					Margins = new Margins (0, 0, 0, -1),
				};


				var logicalLabel = new StaticText
				{
					Parent = box,
					Text = "Fonction de l'unité d'impression :",
					Dock = DockStyle.Top,
					Margins = new Margins (0, 0, 0, UIBuilder.MarginUnderLabel),
				};

				this.logicalField = new TextFieldEx
				{
					Parent = box,
					Dock = DockStyle.Top,
					Margins = new Margins (0, 0, 0, UIBuilder.MarginUnderTextField),
					DefocusAction = DefocusAction.AutoAcceptOrRejectEdition,
					SwallowEscapeOnRejectEdition = true,
					SwallowReturnOnAcceptEdition = true,
					TabIndex = ++tabIndex,
				};


				var commentLabel = new StaticText
				{
					Parent = box,
					Text = "Description de l'unité d'impression :",
					Dock = DockStyle.Top,
					Margins = new Margins (0, 0, 0, UIBuilder.MarginUnderLabel),
				};

				this.commentField = new TextFieldEx
				{
					Parent = box,
					Dock = DockStyle.Top,
					Margins = new Margins (0, 0, 0, UIBuilder.MarginUnderTextField),
					DefocusAction = DefocusAction.AutoAcceptOrRejectEdition,
					SwallowEscapeOnRejectEdition = true,
					SwallowReturnOnAcceptEdition = true,
					TabIndex = ++tabIndex,
				};
			}

			{
				var box = new FrameBox
				{
					Parent = this.centerBox,
					DrawFullFrame = true,
					Dock = DockStyle.Top,
					Padding = new Margins (10),
					Margins = new Margins (0, 0, 0, -1),
				};


				var physicalLabel = new StaticText
				{
					Parent = box,
					Text = "Choix de l'imprimante physique :",
					Dock = DockStyle.Top,
					Margins = new Margins (0, 0, 0, UIBuilder.MarginUnderLabel),
				};

				this.physicalField = new TextFieldCombo
				{
					IsReadOnly = true,
					Parent = box,
					Dock = DockStyle.Top,
					Margins = new Margins (0, 0, 0, UIBuilder.MarginUnderTextField),
					TabIndex = ++tabIndex,
				};


				var trayLabel = new StaticText
				{
					Parent = box,
					Text = "Choix du bac de l'imprimante :",
					Dock = DockStyle.Top,
					Margins = new Margins (0, 0, 0, UIBuilder.MarginUnderLabel),
				};

				this.trayField = new TextFieldCombo
				{
					IsReadOnly = true,
					Parent = box,
					Dock = DockStyle.Top,
					Margins = new Margins (0, 0, 0, UIBuilder.MarginUnderTextField),
					TabIndex = ++tabIndex,
				};


				var paperSizeLabel = new StaticText
				{
					Parent = box,
					Text = "Taille du papier dans le bac de l'imprimante :",
					Dock = DockStyle.Top,
					Margins = new Margins (0, 0, 0, UIBuilder.MarginUnderLabel),
				};

				this.paperSizeField = new TextFieldCombo
				{
					IsReadOnly = true,
					Parent = box,
					Dock = DockStyle.Top,
					Margins = new Margins (0, 0, 0, UIBuilder.MarginUnderTextField),
					TabIndex = ++tabIndex,
				};


				this.duplexLabel = new StaticText
				{
					Parent = box,
					Text = "Mode recto/verso de l'imprimante :",
					Dock = DockStyle.Top,
					Margins = new Margins (0, 0, 0, UIBuilder.MarginUnderLabel),
				};

				this.duplexField = new TextFieldCombo
				{
					IsReadOnly = true,
					Parent = box,
					Dock = DockStyle.Top,
					Margins = new Margins (0, 0, 0, UIBuilder.MarginUnderTextField),
					TabIndex = ++tabIndex,
				};
			}

			{
				var box = new FrameBox
				{
					Parent = this.centerBox,
					DrawFullFrame = true,
					Dock = DockStyle.Top,
					Padding = new Margins (10),
					Margins = new Margins (0, 0, 0, -1),
				};

				this.xOffsetField = PrintingUnitsTabPage.CreateTextField (box, "Décalage horizontal :", "millimètres, vers la droite si positif", ++tabIndex);
				this.yOffsetField = PrintingUnitsTabPage.CreateTextField (box, "Décalage vertical :",   "millimètres, vers le haut si positif",   ++tabIndex);
			}

			{
				var box = new FrameBox
				{
					Parent = this.centerBox,
					DrawFullFrame = true,
					Dock = DockStyle.Top,
					Padding = new Margins (10),
					Margins = new Margins (0, 0, 0, -1),
				};

				this.copiesField = PrintingUnitsTabPage.CreateTextField (box, "Nombre de copies :", "×", ++tabIndex);
			}

			//	Rempli le panneau de droite.
			new StaticText
			{
				Parent = rightFrame,
				PreferredHeight = 20,
				Dock = DockStyle.Top,
				Text = "<font size=\"16\">Options imposées</font>",
				Margins = new Margins (0, 0, 0, 10),
			};

			var rightHelpBox = new FrameBox
			{
				Parent = rightFrame,
				DrawFullFrame = true,
				BackColor = Color.FromHexa ("fffde8"),  // jaune pâle
				Dock = DockStyle.Top,
				Padding = new Margins (10),
			};

			new StaticText
			{
				Parent = rightHelpBox,
				Text = "<i>Ces options seront imposées chaque fois que cette unité d'impression sera utilisée, selon les états ci-dessous :</i>",
				PreferredHeight = 16*3,
				Dock = DockStyle.Top,
				Margins = new Margins (0, 0, 0, 5),
			};

			new CheckButton
			{
				Parent = rightHelpBox,
				Text = "<i>N'impose pas cette option</i>",
				AcceptThreeState = true,
				ActiveState = Common.Widgets.ActiveState.Maybe,
				AutoToggle = false,
				Dock = DockStyle.Top,
			};

			new CheckButton
			{
				Parent = rightHelpBox,
				Text = "<i>Impose de ne pas utiliser cette option</i>",
				AcceptThreeState = true,
				ActiveState = Common.Widgets.ActiveState.No,
				AutoToggle = false,
				Dock = DockStyle.Top,
			};

			new CheckButton
			{
				Parent = rightHelpBox,
				Text = "<i>Impose l'usage de cette option</i>",
				AcceptThreeState = true,
				ActiveState = Common.Widgets.ActiveState.Yes,
				AutoToggle = false,
				Dock = DockStyle.Top,
			};

			this.rightBox = new FrameBox
			{
				Parent = rightFrame,
				DrawFullFrame = true,
				BackColor = Widgets.ArrowedFrame.SurfaceColors.First (),
				Dock = DockStyle.Fill,
				Margins = new Margins (0, 0, -1, 0),
				Padding = new Margins (10),
			};

			this.UpdateOptions (this.rightBox);

			//	Connection des événements.
			this.listController.SelectedItemChanged += delegate
			{
				this.ActionSelectedItemChanged ();
			};

			this.listController.ItemInserted += delegate
			{
				this.ActionItemInserted ();
			};

			this.logicalField.AcceptingEdition += delegate
			{
				this.ActionLogicalChanged ();
			};

			this.commentField.AcceptingEdition += delegate
			{
				this.ActionCommentChanged ();
			};

			this.physicalField.SelectedItemChanged += delegate
			{
				this.ActionPhysicalChanged ();
			};

			this.trayField.SelectedItemChanged += delegate
			{
				this.ActionTrayChanged ();
			};

			this.paperSizeField.SelectedItemChanged += delegate
			{
				this.ActionPaperSizeChanged ();
			};

			this.duplexField.SelectedItemChanged += delegate
			{
				this.ActionDuplexChanged ();
			};

			this.xOffsetField.AcceptingEdition += delegate
			{
				this.ActionOffsetXChanged ();
			};

			this.yOffsetField.AcceptingEdition += delegate
			{
				this.ActionOffsetYChanged ();
			};

			this.copiesField.AcceptingEdition += delegate
			{
				this.ActionCopiesChanged ();
			};

			this.UpdatePhysicalField ();
			this.UpdateWidgets ();
		}

		private static TextFieldEx CreateTextField(Widget parent, FormattedText topText, FormattedText leftText, int tabIndex)
		{
			var xOffsetLabel = new StaticText
			{
				Parent = parent,
				FormattedText = topText,
				Dock = DockStyle.Top,
				Margins = new Margins (0, 0, 0, UIBuilder.MarginUnderLabel),
			};

			var box = new FrameBox
			{
				Parent = parent,
				Dock = DockStyle.Top,
				Margins = new Margins (0, 0, 0, UIBuilder.MarginUnderTextField),
			};

			var field = new TextFieldEx
			{
				Parent = box,
				PreferredWidth = 70,
				Dock = DockStyle.Left,
				DefocusAction = DefocusAction.AutoAcceptOrRejectEdition,
				SwallowEscapeOnRejectEdition = true,
				SwallowReturnOnAcceptEdition = true,
				TabIndex = tabIndex,
			};

			var label = new StaticText
			{
				Parent = box,
				FormattedText = leftText,
				Dock = DockStyle.Fill,
				Margins = new Margins (10, 0, 0, 0),
			};

			return field;
		}

		private void UpdateWidgets()
		{
			int sel = this.listController.SelectedIndex;

			this.centerBox.Enable      = sel != -1;
			this.rightBox.Enable       = sel != -1;
			this.logicalField.Enable   = sel != -1;
			this.commentField.Enable   = sel != -1;
			this.physicalField.Enable  = sel != -1;
			this.trayField.Enable      = sel != -1;
			this.paperSizeField.Enable = sel != -1;
			this.duplexField.Enable    = sel != -1;
			this.xOffsetField.Enable   = sel != -1;
			this.yOffsetField.Enable   = sel != -1;
			this.copiesField.Enable    = sel != -1;

			if (sel == -1)
			{
				this.logicalField.Text   = null;
				this.commentField.Text   = null;
				this.physicalField.Text  = null;
				this.trayField.Text      = null;
				this.paperSizeField.Text = null;
				this.duplexField.Text    = null;
				this.xOffsetField.Text   = null;
				this.yOffsetField.Text   = null;
				this.copiesField.Text    = null;

				foreach (var button in this.optionButtons)
				{
					button.ActiveState = ActiveState.Maybe;
				}
			}
			else
			{
				PrintingUnit printingUnit = this.SelectedPrinter;

				this.logicalField.Text   = printingUnit.LogicalName;
				this.commentField.Text   = printingUnit.Comment;
				this.physicalField.Text  = printingUnit.PhysicalPrinterName;
				this.trayField.Text      = printingUnit.PhysicalPrinterTray;
				this.paperSizeField.Text = PrintingUnitsTabPage.PaperSizeToNiceDescription (printingUnit.PhysicalPaperSize);
				this.duplexField.Text    = PrintingUnit.DuplexToDescription (printingUnit.PhysicalDuplexMode);
				this.xOffsetField.Text   = printingUnit.XOffset.ToString ();
				this.yOffsetField.Text   = printingUnit.YOffset.ToString ();
				this.copiesField.Text    = printingUnit.Copies.ToString ();

				foreach (var button in this.optionButtons)
				{
					var option = DocumentOptions.Parse (button.Name);

					if (printingUnit.OptionsDictionary.ContainsOption (option))
					{
						button.ActiveState = (printingUnit.OptionsDictionary.GetValue (option) == "true") ? ActiveState.Yes : ActiveState.No;
					}
					else
					{
						button.ActiveState = ActiveState.Maybe;
					}
				}
			}

			this.ErrorMessage = this.GetError ();
		}

		private void UpdatePhysicalField()
		{
			List<string> physicalNames = Common.Printing.PrinterSettings.InstalledPrinters.ToList ();

			this.physicalField.Items.Clear ();
			foreach (var physicalName in physicalNames)
			{
				if (!string.IsNullOrWhiteSpace (physicalName))
				{
					this.physicalField.Items.Add (FormattedText.Escape (physicalName));
				}
			}
		}

		private void UpdateTrayField()
		{
			PrintingUnit printingUnit = this.SelectedPrinter;
			List<string> trayNames = PrintingUnitsTabPage.GetTrayList(printingUnit);

			this.trayField.Items.Clear ();
			foreach (var trayName in trayNames)
			{
				string name = FormattedText.Escape (trayName);

				if (!string.IsNullOrWhiteSpace (name) && !this.trayField.Items.Contains (name))
				{
					this.trayField.Items.Add (name);
				}
			}

			if (this.trayField.Items.Count == 0)
			{
				this.trayField.SelectedItemIndex = -1;
			}
			else if (this.trayField.Items.Count == 1)
			{
				this.trayField.SelectedItemIndex = 0;
			}
		}

		private void UpdatePaperSizeField()
		{
			PrintingUnit printingUnit = this.SelectedPrinter;
			List<PaperSize> paperSizes = PrintingUnitsTabPage.GetPaperSizeList (printingUnit);

			this.paperSizeField.Items.Clear ();
			foreach (var paperSize in paperSizes)
			{
				string name = PrintingUnitsTabPage.PaperSizeToNiceDescription (paperSize.Size);

				if (!string.IsNullOrWhiteSpace (name) && !this.paperSizeField.Items.Contains (name))
				{
					this.paperSizeField.Items.Add (name);
				}
			}

			if (this.paperSizeField.Items.Count == 1)
			{
				this.paperSizeField.SelectedItemIndex = 0;
			}
		}

		private void UpdateDuplexField()
		{
			PrintingUnit printingUnit = this.SelectedPrinter;

			if (PrintingUnitsTabPage.CanDuplex (printingUnit))
			{
				this.duplexLabel.Enable = true;
				this.duplexField.Enable = true;
				
				this.duplexField.Items.Clear ();
				this.duplexField.Items.Add (PrintingUnit.DuplexToDescription (DuplexMode.Default));
				this.duplexField.Items.Add (PrintingUnit.DuplexToDescription (DuplexMode.Simplex));
				this.duplexField.Items.Add (PrintingUnit.DuplexToDescription (DuplexMode.Horizontal));
				this.duplexField.Items.Add (PrintingUnit.DuplexToDescription (DuplexMode.Vertical));
			}
			else
			{
				this.duplexLabel.Enable = false;
				this.duplexField.Enable = false;

				this.duplexField.Items.Clear ();
				this.duplexField.Text = null;
			}
		}

		private void UpdateOptions(FrameBox parent)
		{
			this.optionButtons.Clear ();
			int tabIndex = 0;

			foreach (var documentOption in VerboseDocumentOption.GetDefault ())
			{
				if (documentOption.Type == DocumentOptionValueType.Boolean)
				{
					var check = new CheckButton
					{
						Parent = parent,
						Name = DocumentOptions.ToString (documentOption.Option),
						Text = documentOption.Description,
						AcceptThreeState = true,
						ActiveState = ActiveState.Maybe,
						Dock = DockStyle.Top,
						AutoToggle = false,
						TabIndex = ++tabIndex,
					};

					check.Clicked += delegate
					{
						PrintingUnit printingUnit = this.SelectedPrinter;
						var option = DocumentOptions.Parse (check.Name);

						if (check.ActiveState == ActiveState.Maybe)
						{
							check.ActiveState = ActiveState.No;
							printingUnit.OptionsDictionary.Add (option, "false");
						}
						else if (check.ActiveState == ActiveState.No)
						{
							check.ActiveState = ActiveState.Yes;
							printingUnit.OptionsDictionary.Add (option, "true");
						}
						else
						{
							check.ActiveState = ActiveState.Maybe;
							printingUnit.OptionsDictionary.Remove (option);
						}
					};

					this.optionButtons.Add (check);
				}
			}

			var clearButton = new Button
			{
				Parent = parent,
				Text = "N'impose aucune option",
				Dock = DockStyle.Top,
				Margins = new Margins (0, 0, 10, 0),
			};

			clearButton.Clicked += delegate
			{
				PrintingUnit printingUnit = this.SelectedPrinter;
				printingUnit.OptionsDictionary.Clear ();
				this.UpdateWidgets ();
			};
		}


		private void ActionSelectedItemChanged()
		{
			this.UpdateWidgets ();
			this.UpdateTrayField ();
			this.UpdatePaperSizeField ();
			this.UpdateDuplexField ();
		}

		private void ActionItemInserted()
		{
			this.logicalField.SelectAll ();
			this.logicalField.Focus ();
		}

		private void ActionLogicalChanged()
		{
			int sel = this.listController.SelectedIndex;

			if (this.printingUnitList[sel].LogicalName != this.logicalField.Text)
			{
				this.printingUnitList[sel].LogicalName = this.logicalField.Text;

				this.listController.UpdateList (sel);
				this.UpdateWidgets ();
			}
		}

		private void ActionCommentChanged()
		{
			int sel = this.listController.SelectedIndex;

			if (this.printingUnitList[sel].Comment != this.commentField.Text)
			{
				this.printingUnitList[sel].Comment = this.commentField.Text;

				this.listController.UpdateList (sel);
				this.UpdateWidgets ();
			}
		}

		private void ActionPhysicalChanged()
		{
			int sel = this.listController.SelectedIndex;

			if (this.printingUnitList[sel].PhysicalPrinterName != this.physicalField.Text)
			{
				this.printingUnitList[sel].PhysicalPrinterName = this.physicalField.Text;
				this.printingUnitList[sel].PhysicalPrinterTray = null;
				this.printingUnitList[sel].XOffset = 0;
				this.printingUnitList[sel].YOffset = 0;

				this.listController.UpdateList (sel);
				this.UpdateWidgets ();
				this.UpdateTrayField ();
				this.UpdatePaperSizeField ();
				this.UpdateDuplexField ();
			}
		}

		private void ActionTrayChanged()
		{
			int sel = this.listController.SelectedIndex;

			if (this.printingUnitList[sel].PhysicalPrinterTray != this.trayField.Text)
			{
				this.printingUnitList[sel].PhysicalPrinterTray = this.trayField.Text;

				this.listController.UpdateList (sel);
				this.UpdateWidgets ();
			}
		}

		private void ActionPaperSizeChanged()
		{
			int sel = this.listController.SelectedIndex;

			var paperSize = PrintingUnitsTabPage.NiceDescriptionToPaperSize (this.paperSizeField.Text);

			if (this.printingUnitList[sel].PhysicalPaperSize != paperSize)
			{
				this.printingUnitList[sel].PhysicalPaperSize = paperSize;

				this.listController.UpdateList (sel);
				this.UpdateWidgets ();
			}
		}

		private void ActionDuplexChanged()
		{
			int sel = this.listController.SelectedIndex;

			var duplex = PrintingUnit.DescriptionToDuplex (this.duplexField.Text);

			if (this.printingUnitList[sel].PhysicalDuplexMode != duplex)
			{
				this.printingUnitList[sel].PhysicalDuplexMode = duplex;

				this.listController.UpdateList (sel);
				this.UpdateWidgets ();
			}
		}

		private void ActionOffsetXChanged()
		{
			int sel = this.listController.SelectedIndex;

			double value;
			if (double.TryParse (this.xOffsetField.Text, out value))
			{
				if (this.printingUnitList[sel].XOffset != value)
				{
					this.printingUnitList[sel].XOffset = value;

					this.listController.UpdateList (sel);
					this.UpdateWidgets ();
				}
			}
		}

		private void ActionOffsetYChanged()
		{
			int sel = this.listController.SelectedIndex;

			double value;
			if (double.TryParse (this.yOffsetField.Text, out value))
			{
				if (this.printingUnitList[sel].YOffset != value)
				{
					this.printingUnitList[sel].YOffset = value;

					this.listController.UpdateList (sel);
					this.UpdateWidgets ();
				}
			}
		}

		private void ActionCopiesChanged()
		{
			int sel = this.listController.SelectedIndex;

			int value;
			if (int.TryParse (this.copiesField.Text, out value))
			{
				value = System.Math.Max (value, 1);

				if (this.printingUnitList[sel].Copies != value)
				{
					this.printingUnitList[sel].Copies = value;

					this.listController.UpdateList (sel);
					this.UpdateWidgets ();
				}
			}
		}


		private static List<string> GetTrayList(PrintingUnit printingUnit)
		{
			var trayNames = new List<string> ();

			if (printingUnit != null && !string.IsNullOrEmpty (printingUnit.PhysicalPrinterName))
			{
				var settings = Common.Printing.PrinterSettings.FindPrinter (FormattedText.Unescape (printingUnit.PhysicalPrinterName));

				if (settings != null)
				{
					System.Array.ForEach (settings.PaperSources, x => trayNames.Add (FormattedText.Escape (x.Name.Trim ())));

					var ps = settings.PaperSizes;
				}
			}

			return trayNames;
		}

		private static List<PaperSize> GetPaperSizeList(PrintingUnit printingUnit)
		{
			var paperSizes = new List<PaperSize> ();

			if (printingUnit != null && !string.IsNullOrEmpty (printingUnit.PhysicalPrinterName))
			{
				var settings = Common.Printing.PrinterSettings.FindPrinter (FormattedText.Unescape (printingUnit.PhysicalPrinterName));

				if (settings != null)
				{
					System.Array.ForEach (settings.PaperSizes, x => paperSizes.Add (x));

					var ps = settings.PaperSizes;
				}
			}

			paperSizes.Sort (PrintingUnitsTabPage.ComparePaperSize);

			return paperSizes;
		}

		private static int ComparePaperSize(PaperSize x, PaperSize y)
		{
			if (x.Width < y.Width)
			{
				return -1;
			}
			else if (x.Width > y.Width)
			{
				return 1;
			}

			if (x.Height < y.Height)
			{
				return -1;
			}
			else if (x.Height > y.Height)
			{
				return 1;
			}

			return 0;
		}

		private static bool CanDuplex(PrintingUnit printingUnit)
		{
			if (printingUnit != null && !string.IsNullOrEmpty (printingUnit.PhysicalPrinterName))
			{
				var settings = Common.Printing.PrinterSettings.FindPrinter (FormattedText.Unescape (printingUnit.PhysicalPrinterName));

				if (settings != null)
				{
					return settings.CanDuplex;
				}
			}

			return false;
		}


		private string DefaultLogicalName
		{
			get
			{
				return string.Format ("Unité d'impression {0}", (this.printingUnitList.Count+1).ToString ());
			}
		}


		private string GetError()
		{
			for (int i = 0; i < this.printingUnitList.Count; i++)
			{
				if (string.IsNullOrWhiteSpace (this.printingUnitList[i].LogicalName))
				{
					return string.Format ("<b>Rang {0}</b>: Il faut spécifier la fonction de l'unité d'impression.", (i+1).ToString ());
				}

				if (string.IsNullOrWhiteSpace (this.printingUnitList[i].PhysicalPrinterName))
				{
					return string.Format ("<b>{0}</b>: Il faut choisir l'imprimante physique.", this.printingUnitList[i].LogicalName);
				}

				if (string.IsNullOrWhiteSpace (this.printingUnitList[i].PhysicalPrinterTray))
				{
					List<string> trayNames = PrintingUnitsTabPage.GetTrayList(this.printingUnitList[i]);
					if (trayNames.Count > 0)
					{
						return string.Format ("<b>{0}</b>: Il faut choisir le bac.", this.printingUnitList[i].LogicalName);
					}
				}

				if (this.printingUnitList[i].PhysicalPaperSize.IsEmpty)
				{
					return string.Format ("<b>{0}</b>: Il faut choisir la taille du papier.", this.printingUnitList[i].LogicalName);
				}

				if (!PrintingUnit.CheckString (this.printingUnitList[i].LogicalName))
				{
					return string.Format ("<b>{0}</b>: Ce nom de fonction est incorrect.", this.printingUnitList[i].LogicalName);
				}

				if (!PrintingUnit.CheckString (this.printingUnitList[i].Comment))
				{
					return string.Format ("<b>{0}</b>: La description est incorrecte.", this.printingUnitList[i].LogicalName);
				}

				for (int j = 0; j < this.printingUnitList.Count; j++)
				{
					if (j != i && this.printingUnitList[j].LogicalName == this.printingUnitList[i].LogicalName)
					{
						return string.Format ("<b>{0}</b>: Ces deux unités d'impression ont la même fonction.", this.printingUnitList[i].LogicalName);
					}
				}
			}

			return null;
		}


		private PrintingUnit SelectedPrinter
		{
			get
			{
				int sel = this.listController.SelectedIndex;

				if (sel == -1)
				{
					return null;
				}
				else
				{
					return this.printingUnitList[sel];
				}
			}
		}


		#region Paper size conversion
		private static Size NiceDescriptionToPaperSize(string text)
		{
			//	Conversion d'une jolie description en taille de papier.
			if (!string.IsNullOrWhiteSpace (text))
			{
				double width, height;

				int i = text.IndexOf (" ");
				if (i == -1)
				{
					return Size.Empty;
				}

				if (!double.TryParse (text.Substring (0, i), out width))
				{
					return Size.Empty;
				}

				i = text.IndexOf (" × ");
				if (i == -1)
				{
					return Size.Empty;
				}
				i += 3;

				int j = text.IndexOf ("mm", i);
				if (j == -1)
				{
					return Size.Empty;
				}

				if (!double.TryParse (text.Substring (i, j-i), out height))
				{
					return Size.Empty;
				}

				return new Size (width, height);
			}

			return Size.Empty;
		}

		private static string PaperSizeToNiceDescription(Size paperSize)
		{
			//	Conversion d'une taille de papier en une jolie description.
			if (paperSize.IsEmpty)
			{
				return "<i>Inconnu</i>";
			}
			else
			{
				double width  = paperSize.Width;
				double height = paperSize.Height;
				PrintingUnitsTabPage.PaperSizeRounding (ref width, ref height);

				string description = PrintingUnitsTabPage.PaperSizeToDescription (paperSize);

				if (description != null)
				{
					description = string.Concat (" (", description, ")");
				}

				return string.Format ("{0} × {1} mm {2}", width.ToString (), height.ToString (), description);
			}
		}

		private static string PaperSizeToDescription(Size paperSize)
		{
			//	Retourne le nom de quelques formats très courants.
			double width  = paperSize.Width;
			double height = paperSize.Height;
			PrintingUnitsTabPage.PaperSizeRounding (ref width, ref height);

			if (width == 297 && height == 420)
			{
				return "A3";
			}

			if (width == 210 && height == 297)
			{
				return "A4";
			}

			if (width == 148 && height == 210)
			{
				return "A5";
			}

			if (width == 62 && height == 29)  // petite étiquette pour Brother QL-560 ?
			{
				return "étiquette";
			}

			return null;
		}

		private static void PaperSizeRounding(ref double width, ref double height)
		{
			//	Arrondi au millimètre le plus proche.
			width  = System.Math.Floor (width  + 0.5);
			height = System.Math.Floor (height + 0.5);
		}
		#endregion


		#region ListController callbacks
		private FormattedText ListControllerItemToText(PrintingUnit printingUnit)
		{
			return printingUnit.NiceDescription;
		}

		private FormattedText ListControllerGetTextInfo(int count)
		{
			if (count == 0)
			{
				return "Aucune unité d'impression définie";
			}
			else if (count == 1)
			{
				return string.Format ("{0} unité d'impression définie", count.ToString ());
			}
			else
			{
				return string.Format ("{0} unités d'impression définies", count.ToString ());
			}
		}

		private PrintingUnit ListControllerCreateItem(int sel)
		{
			return new PrintingUnit (this.DefaultLogicalName);
		}
		#endregion


		private Controllers.ListController<PrintingUnit>	listController;
		private FrameBox									centerBox;
		private FrameBox									rightBox;
		private TextFieldEx									logicalField;
		private TextFieldEx									commentField;
		private TextFieldCombo								physicalField;
		private TextFieldCombo								trayField;
		private TextFieldCombo								paperSizeField;
		private StaticText									duplexLabel;
		private TextFieldCombo								duplexField;
		private TextFieldEx									xOffsetField;
		private TextFieldEx									yOffsetField;
		private TextFieldEx									copiesField;
		private List<CheckButton>							optionButtons;
		private List<PrintingUnit>							printingUnitList;
	}
}
