//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;
using Epsitec.Common.Printing;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;
using Epsitec.Common.Widgets;

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Documents;
using Epsitec.Cresus.Core.Documents.Verbose;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Print;
using Epsitec.Cresus.Core.Widgets;

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
			this.printingUnitList = PrinterApplicationSettings.GetPrintingUnitList (this.Container.Data.Host);

			this.documentPrintingUnits = this.Container.Data.GetAllEntities<DocumentPrintingUnitsEntity> ().ToList ();
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

			var column1 = new FrameBox
			{
				Parent = frame,
				Dock = DockStyle.Fill,
				Margins = new Margins (0, 10, 0, 0),
			};

			var column3 = new FrameBox
			{
				Parent = frame,
				PreferredWidth = 250,
				Dock = DockStyle.Right,
				Margins = new Margins (10, 0, 0, 0),
			};

			var column2 = new FrameBox
			{
				Parent = frame,
				PreferredWidth = 250,
				Dock = DockStyle.Right,
				Margins = new Margins (0, 0, 0, 0),
			};

			//	Rempli la colonne de gauche.
			var columnTitle1 = new StaticText (column1);
			columnTitle1.SetColumnTitle ("Liste des unités d'impression");

			this.table = new CellTable
			{
				Parent = column1,
				PreferredWidth = 310,
				Dock = DockStyle.Fill,
				StyleH = CellArrayStyles.Separator | CellArrayStyles.ScrollNorm | CellArrayStyles.Header,
				StyleV = CellArrayStyles.Separator | CellArrayStyles.ScrollNorm | CellArrayStyles.SelectLine,
			};

			//	Rempli la deuxième colonne.
			var columnTitle2 = new StaticText (column2);
			columnTitle2.SetColumnTitle ("Choix pour l'unité d'impression");

			this.printerBox = new FrameBox
			{
				Parent = column2,
				DrawFullFrame = true,
				BackColor = Widgets.ArrowedFrame.SurfaceColors.First (),
				Dock = DockStyle.Fill,
			};

			{
				var box = new FrameBox
				{
					Parent = this.printerBox,
					DrawFullFrame = true,
					Dock = DockStyle.Top,
					Padding = new Margins (10),
					Margins = new Margins (0, 0, 0, -1),
				};

				PrintingUnitsTabPage.CreateTextField (box, "Choix de l'imprimante physique", ref tabIndex, out this.physicalBox,  out this.physicalField);
				PrintingUnitsTabPage.CreateTextField (box, "Choix du bac",                   ref tabIndex, out this.trayBox,      out this.trayField);
				PrintingUnitsTabPage.CreateTextField (box, "Taille du papier dans le bac",   ref tabIndex, out this.paperSizeBox, out this.paperSizeField);
				PrintingUnitsTabPage.CreateTextField (box, "Mode recto/verso",               ref tabIndex, out this.duplexBox,    out this.duplexField);
			}

			{
				var box = new FrameBox
				{
					Parent = this.printerBox,
					DrawFullFrame = true,
					Dock = DockStyle.Top,
					Padding = new Margins (10),
					Margins = new Margins (0, 0, 0, -1),
				};

				PrintingUnitsTabPage.CreateTextField (box, "Décalage horizontal", "mm, vers la droite si positif", ref tabIndex, out this.xOffsetBox, out this.xOffsetField);
				PrintingUnitsTabPage.CreateTextField (box, "Décalage vertical",   "mm, vers le haut si positif",   ref tabIndex, out this.yOffsetBox, out this.yOffsetField);
			}

			{
				var box = new FrameBox
				{
					Parent = this.printerBox,
					DrawFullFrame = true,
					Dock = DockStyle.Top,
					Padding = new Margins (10),
					Margins = new Margins (0, 0, 0, -1),
				};

				PrintingUnitsTabPage.CreateTextField (box, "Nombre de copies", "×", ref tabIndex, out this.copiesBox, out this.copiesField);
			}

			//	Rempli la colonne de droite.
			var columnTitle4 = new StaticText (column3);
			columnTitle4.SetColumnTitle ("Options imposées");

			var optionsHelpBox = new FrameBox
			{
				Parent = column3,
				DrawFullFrame = true,
				BackColor = Color.FromHexa ("fffde8"),  // jaune pâle
				Dock = DockStyle.Top,
				Padding = new Margins (10),
			};

			new StaticText
			{
				Parent = optionsHelpBox,
				Text = "<i>Ces options seront imposées chaque fois que cette unité d'impression sera utilisée, selon les états ci-dessous :</i>",
				PreferredHeight = 16*3,
				Dock = DockStyle.Top,
				Margins = new Margins (0, 0, 0, 5),
			};

			new CheckButton
			{
				Parent = optionsHelpBox,
				Text = "<i>N'impose pas cette option</i>",
				AcceptThreeState = true,
				ActiveState = Common.Widgets.ActiveState.Maybe,
				AutoToggle = false,
				Dock = DockStyle.Top,
			};

			new CheckButton
			{
				Parent = optionsHelpBox,
				Text = "<i>Impose l'usage de cette option</i>",
				AcceptThreeState = true,
				ActiveState = Common.Widgets.ActiveState.Yes,
				AutoToggle = false,
				Dock = DockStyle.Top,
			};

			new CheckButton
			{
				Parent = optionsHelpBox,
				Text = "<i>Impose de ne pas utiliser cette option</i>",
				AcceptThreeState = true,
				ActiveState = Common.Widgets.ActiveState.No,
				AutoToggle = false,
				Dock = DockStyle.Top,
			};

			var rightBox = new FrameBox
			{
				Parent = column3,
				DrawFullFrame = true,
				BackColor = Widgets.ArrowedFrame.SurfaceColors.First (),
				Dock = DockStyle.Fill,
				Margins = new Margins (0, 0, -1, 0),
			};

			this.optionsBox = new Scrollable
			{
				Parent = rightBox,
				Dock = DockStyle.Fill,
				HorizontalScrollerMode = ScrollableScrollerMode.HideAlways,
				VerticalScrollerMode = ScrollableScrollerMode.Auto,
				PaintViewportFrame = false,
			};
			this.optionsBox.Viewport.IsAutoFitting = true;

			//	Connection des événements.
			this.table.SelectionChanged += delegate
			{
				this.TableSelectionChanged ();
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

			this.UpdateTable ();
			this.UpdatePhysicalField ();
			this.UpdateOptions ();
			this.UpdateWidgets ();
		}


		private static void CreateTextField(Widget parent, FormattedText topText, ref int tabIndex, out FrameBox box, out TextFieldCombo field)
		{
			box = new FrameBox
			{
				Parent = parent,
				Dock = DockStyle.Top,
				TabIndex = ++tabIndex,
			};

			new StaticText
			{
				Parent = box,
				FormattedText = FormattedText.Concat (topText, " :"),
				Dock = DockStyle.Top,
				Margins = new Margins (0, 0, 0, UIBuilder.MarginUnderLabel),
			};

			field = new TextFieldCombo
			{
				IsReadOnly = true,
				Parent = box,
				Dock = DockStyle.Top,
				Margins = new Margins (0, 0, 0, UIBuilder.MarginUnderTextField),
				TabIndex = ++tabIndex,
			};
		}

		private static void CreateTextField(Widget parent, FormattedText topText, FormattedText leftText, ref int tabIndex, out FrameBox box, out TextFieldEx field)
		{
			box = new FrameBox
			{
				Parent = parent,
				Dock = DockStyle.Top,
				TabIndex = ++tabIndex,
			};

			new StaticText
			{
				Parent = box,
				FormattedText = FormattedText.Concat (topText, " :"),
				Dock = DockStyle.Top,
				Margins = new Margins (0, 0, 0, UIBuilder.MarginUnderLabel),
			};

			var line = new FrameBox
			{
				Parent = box,
				Dock = DockStyle.Top,
				Margins = new Margins (0, 0, 0, UIBuilder.MarginUnderTextField),
			};

			field = new TextFieldEx
			{
				Parent = line,
				PreferredWidth = 70,
				Dock = DockStyle.Left,
				DefocusAction = DefocusAction.AutoAcceptOrRejectEdition,
				SwallowEscapeOnRejectEdition = true,
				SwallowReturnOnAcceptEdition = true,
				TabIndex = tabIndex,
			};

			var label = new StaticText
			{
				Parent = line,
				FormattedText = leftText,
				Dock = DockStyle.Fill,
				Margins = new Margins (5, 0, 0, 0),
			};
		}


		private void UpdateTable()
		{
			int sel = this.table.SelectedRow;

			int rows = this.documentPrintingUnits.Count;
			this.table.SetArraySize (4, rows);

			this.table.SetWidthColumn (0, 110);
			this.table.SetWidthColumn (1, 150);
			this.table.SetWidthColumn (2, 100);
			this.table.SetWidthColumn (3, 120);

			this.table.SetHeaderTextH (0, "Unité d'impression");
			this.table.SetHeaderTextH (1, "Imprimante physique");
			this.table.SetHeaderTextH (2, "Bac");
			this.table.SetHeaderTextH (3, "Papier");

			ContentAlignment[] alignments =
			{
				ContentAlignment.MiddleLeft,
				ContentAlignment.MiddleLeft,
				ContentAlignment.MiddleLeft,
				ContentAlignment.MiddleLeft,
			};

			for (int row=0; row<rows; row++)
			{
				this.table.FillRow (row, alignments);
				this.table.UpdateRow (row, this.GetRowTexts (row));
			}

			this.table.SelectRow (sel, true);
		}

		private string[] GetRowTexts(int row)
		{
			var result = new string[4];


			var printingUnit = this.GetPrintingUnit (this.documentPrintingUnits[row].Code);

			if (printingUnit == null)
			{
				result[0] = this.documentPrintingUnits[row].Name.ApplyItalic ().ToString ();
				result[1] = "";
				result[2] = "";
				result[3] = "";
			}
			else
			{
				result[0] = this.documentPrintingUnits[row].Name.ApplyBold ().ToString ();
				result[1] = printingUnit.PhysicalPrinterName;
				result[2] = printingUnit.PhysicalPrinterTray;
				result[3] = PrintingUnitsTabPage.PaperSizeToNiceDescription (printingUnit.PhysicalPaperSize);
			}

			return result;
		}
	
		
		private void UpdateWidgets()
		{
			int sel = this.table.SelectedRow;
			PrintingUnit printingUnit = this.SelectedPrintingUnit;

			this.printerBox.Enable   = (sel != -1);
			this.physicalBox.Enable  = (sel != -1);
			this.trayBox.Enable      = (sel != -1 && printingUnit != null);
			this.paperSizeBox.Enable = (sel != -1 && printingUnit != null);
			this.duplexBox.Enable    = (sel != -1 && printingUnit != null && PrintingUnitsTabPage.CanDuplex (this.physicalField.Text));
			this.xOffsetBox.Enable   = (sel != -1 && printingUnit != null);
			this.yOffsetBox.Enable   = (sel != -1 && printingUnit != null);
			this.copiesBox.Enable    = (sel != -1 && printingUnit != null);

			this.optionsBox.Enable   = (sel != -1 && printingUnit != null);

			if (printingUnit == null)
			{
				this.physicalField.Text  = (sel == -1) ? null : PrintingUnitsTabPage.nullPrinter;
				this.trayField.Text      = null;
				this.paperSizeField.Text = null;
				this.duplexField.Text    = null;
				this.xOffsetField.Text   = null;
				this.yOffsetField.Text   = null;
				this.copiesField.Text    = null;
			}
			else
			{
				this.physicalField.Text  = printingUnit.PhysicalPrinterName;
				this.trayField.Text      = printingUnit.PhysicalPrinterTray;
				this.paperSizeField.Text = PrintingUnitsTabPage.PaperSizeToNiceDescription (printingUnit.PhysicalPaperSize);
				this.duplexField.Text    = PrintingUnit.DuplexToDescription (printingUnit.PhysicalDuplexMode);
				this.xOffsetField.Text   = printingUnit.XOffset.ToString ();
				this.yOffsetField.Text   = printingUnit.YOffset.ToString ();
				this.copiesField.Text    = printingUnit.Copies.ToString ();
			}

			this.ErrorMessage = this.GetError ();
		}

		private void UpdatePhysicalField()
		{
			this.physicalField.Items.Clear ();

			this.physicalField.Items.Add (PrintingUnitsTabPage.nullPrinter);  // toujours une 1ère ligne avec "(aucune)"

			var physicalNames = Common.Printing.PrinterSettings.InstalledPrinters;
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
			this.trayField.Items.Clear ();

			List<string> trayNames = PrintingUnitsTabPage.GetTrayList (this.physicalField.Text);
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
			this.paperSizeField.Items.Clear ();

			List<PaperSize> paperSizes = PrintingUnitsTabPage.GetPaperSizeList (this.physicalField.Text);
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
			this.duplexField.Items.Clear ();

			if (PrintingUnitsTabPage.CanDuplex (this.physicalField.Text))
			{
				this.duplexField.Items.Add (PrintingUnit.DuplexToDescription (DuplexMode.Default));
				this.duplexField.Items.Add (PrintingUnit.DuplexToDescription (DuplexMode.Simplex));
				this.duplexField.Items.Add (PrintingUnit.DuplexToDescription (DuplexMode.Horizontal));
				this.duplexField.Items.Add (PrintingUnit.DuplexToDescription (DuplexMode.Vertical));
			}
			else
			{
				this.duplexField.Text = null;
			}
		}

		private void UpdateOptions()
		{
			this.optionsBox.Viewport.Children.Clear ();

			var optionsKeys = new PrintingOptionDictionary ();
			foreach (var option in VerboseDocumentOption.GetDefault ())
			{
				optionsKeys[option.Option] = "";
			}

			PrintingUnit printingUnit = this.SelectedPrintingUnit;
			if (printingUnit != null)
			{
				var optionsValues = printingUnit.Options;

				var controller = new DocumentOptionsEditor.OptionsController (optionsKeys, optionsValues, true);
				controller.CreateUI (this.optionsBox.Viewport, null);
			}
		}


		private void TableSelectionChanged()
		{
			this.UpdateTrayField ();
			this.UpdatePaperSizeField ();
			this.UpdateDuplexField ();
			this.UpdateWidgets ();
			this.UpdateOptions ();
		}


		private void ActionPhysicalChanged()
		{
			if (this.physicalField.Text == PrintingUnitsTabPage.nullPrinter)
			{
				this.RemovePrintingUnit (this.SelectedCode);

				this.UpdateTrayField ();
				this.UpdatePaperSizeField ();
				this.UpdateDuplexField ();
				this.UpdateTable ();
				this.UpdateWidgets ();
				this.UpdateOptions ();
			}
			else
			{
				var printingUnit = this.CreatePrintingUnit (this.SelectedCode);

				printingUnit.PhysicalPrinterName = this.physicalField.Text;
				printingUnit.PhysicalPrinterTray = null;
				printingUnit.PhysicalPaperSize = Size.Zero;
				printingUnit.PhysicalDuplexMode = DuplexMode.Default;
				printingUnit.XOffset = 0;
				printingUnit.YOffset = 0;
				printingUnit.Copies = 1;

				this.UpdateTrayField ();
				this.UpdatePaperSizeField ();
				this.UpdateDuplexField ();

				//	Initialise des valeurs par défaut.
				if (this.trayField.Items.Count > 0)
				{
					//	Utilise le premier bac, généralement "Sélection automatique".
					printingUnit.PhysicalPrinterTray = this.trayField.Items[0];
				}

				var a4 = new Size(210, 297);  // A4 vertical
				if (this.paperSizeField.Items.Contains (PrintingUnitsTabPage.PaperSizeToNiceDescription (a4)))
				{
					//	Utilise le format A4 vertical s'il existe.
					printingUnit.PhysicalPaperSize = a4;
				}
				else if (this.paperSizeField.Items.Count > 0)
				{
					//	S'il n'existe pas de A4, sélectionne le premier format, qui sera prioritairement
					//	un format connu, grâce au tri ComparePaperSize.
					printingUnit.PhysicalPaperSize = PrintingUnitsTabPage.NiceDescriptionToPaperSize (this.paperSizeField.Items[0]);
				}

				this.UpdateTable ();
				this.UpdateWidgets ();
				this.UpdateOptions ();
			}
		}

		private void ActionTrayChanged()
		{
			PrintingUnit printingUnit = this.SelectedPrintingUnit;

			if (printingUnit != null && printingUnit.PhysicalPrinterTray != this.trayField.Text)
			{
				printingUnit.PhysicalPrinterTray = this.trayField.Text;

				this.UpdateTable ();
				this.UpdateWidgets ();
			}
		}

		private void ActionPaperSizeChanged()
		{
			PrintingUnit printingUnit = this.SelectedPrintingUnit;

			var paperSize = PrintingUnitsTabPage.NiceDescriptionToPaperSize (this.paperSizeField.Text);

			if (printingUnit != null && printingUnit.PhysicalPaperSize != paperSize)
			{
				printingUnit.PhysicalPaperSize = paperSize;

				this.UpdateTable ();
				this.UpdateWidgets ();
			}
		}

		private void ActionDuplexChanged()
		{
			PrintingUnit printingUnit = this.SelectedPrintingUnit;

			var duplex = PrintingUnit.DescriptionToDuplex (this.duplexField.Text);

			if (printingUnit != null && printingUnit.PhysicalDuplexMode != duplex)
			{
				printingUnit.PhysicalDuplexMode = duplex;

				this.UpdateTable ();
				this.UpdateWidgets ();
			}
		}

		private void ActionOffsetXChanged()
		{
			PrintingUnit printingUnit = this.SelectedPrintingUnit;

			double value;
			if (double.TryParse (this.xOffsetField.Text, out value))
			{
				if (printingUnit != null && printingUnit.XOffset != value)
				{
					printingUnit.XOffset = value;

					this.UpdateTable ();
					this.UpdateWidgets ();
				}
			}
		}

		private void ActionOffsetYChanged()
		{
			PrintingUnit printingUnit = this.SelectedPrintingUnit;

			double value;
			if (double.TryParse (this.yOffsetField.Text, out value))
			{
				if (printingUnit != null && printingUnit.YOffset != value)
				{
					printingUnit.YOffset = value;

					this.UpdateTable ();
					this.UpdateWidgets ();
				}
			}
		}

		private void ActionCopiesChanged()
		{
			PrintingUnit printingUnit = this.SelectedPrintingUnit;

			int value;
			if (int.TryParse (this.copiesField.Text, out value))
			{
				value = System.Math.Max (value, 1);

				if (printingUnit != null && printingUnit.Copies != value)
				{
					printingUnit.Copies = value;

					this.UpdateTable ();
					this.UpdateWidgets ();
				}
			}
		}


		private static List<string> GetTrayList(string physicalPrinterName)
		{
			var trayNames = new List<string> ();

			if (!string.IsNullOrEmpty (physicalPrinterName) && physicalPrinterName != PrintingUnitsTabPage.nullPrinter)
			{
				var settings = Common.Printing.PrinterSettings.FindPrinter (FormattedText.Unescape (physicalPrinterName));

				if (settings != null)
				{
					System.Array.ForEach (settings.PaperSources, x => trayNames.Add (FormattedText.Escape (x.Name.Trim ())));

					var ps = settings.PaperSizes;
				}
			}

			return trayNames;
		}

		private static List<PaperSize> GetPaperSizeList(string physicalPrinterName)
		{
			var paperSizes = new List<PaperSize> ();

			if (!string.IsNullOrEmpty (physicalPrinterName) && physicalPrinterName != PrintingUnitsTabPage.nullPrinter)
			{
				var settings = Common.Printing.PrinterSettings.FindPrinter (FormattedText.Unescape (physicalPrinterName));

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
			string xDescription = PrintingUnitsTabPage.PaperSizeToDescription (x.Size);
			string yDescription = PrintingUnitsTabPage.PaperSizeToDescription (y.Size);

			//	Met en premier les tailles de papier connues (A4, A5, etc.).
			if (xDescription != null && yDescription != null)
			{
				return xDescription.CompareTo (yDescription);
			}

			if (xDescription != null)
			{
				return -1;
			}

			if (yDescription != null)
			{
				return 1;
			}

			if (x.Width != y.Width)
			{
				return x.Width.CompareTo (y.Width);
			}

			if (x.Height != y.Height)
			{
				return x.Height.CompareTo (y.Height);
			}

			return 0;
		}

		private static bool CanDuplex(string physicalPrinterName)
		{
			if (!string.IsNullOrEmpty (physicalPrinterName) && physicalPrinterName != PrintingUnitsTabPage.nullPrinter)
			{
				var settings = Common.Printing.PrinterSettings.FindPrinter (FormattedText.Unescape (physicalPrinterName));

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
			foreach (var documentPrintingUnit in this.documentPrintingUnits)
			{
				var printingUnit = this.GetPrintingUnit (documentPrintingUnit.Code);

				if (printingUnit == null)
				{
					continue;
				}

				if (printingUnit.PhysicalPrinterName == PrintingUnitsTabPage.nullPrinter)
				{
					continue;
				}

				if (string.IsNullOrWhiteSpace (printingUnit.PhysicalPrinterTray))
				{
					if (this.trayField.Items.Count > 0)
					{
						return string.Format ("<b>{0}</b>: Il faut choisir le bac.", documentPrintingUnit.Name);
					}
				}

				if (printingUnit.PhysicalPaperSize.IsEmpty)
				{
					return string.Format ("<b>{0}</b>: Il faut choisir la taille du papier.", documentPrintingUnit.Name);
				}
			}

			return null;  // ok
		}


		private PrintingUnit SelectedPrintingUnit
		{
			get
			{
				int sel = this.table.SelectedRow;

				if (sel == -1)
				{
					return null;
				}
				else
				{
					var documentPrintingUnit = this.documentPrintingUnits[sel];
					return this.GetPrintingUnit (documentPrintingUnit.Code);
				}
			}
		}

		private string SelectedCode
		{
			get
			{
				int sel = this.table.SelectedRow;

				if (sel == -1)
				{
					return null;
				}
				else
				{
					var documentPrintingUnit = this.documentPrintingUnits[sel];
					return documentPrintingUnit.Code;
				}
			}
		}

		private PrintingUnit CreatePrintingUnit(string code)
		{
			var printingUnit = this.GetPrintingUnit (code);

			if (printingUnit == null)
			{
				printingUnit = new PrintingUnit
				{
					DocumentPrintingUnitCode = code,
				};

				this.printingUnitList.Add (printingUnit);
			}

			return printingUnit;
		}

		private void RemovePrintingUnit(string code)
		{
			var printingUnit = this.GetPrintingUnit (code);

			if (printingUnit != null)
			{
				this.printingUnitList.Remove (printingUnit);
			}
		}

		private PrintingUnit GetPrintingUnit(string code)
		{
			return this.printingUnitList.Where (x => x.DocumentPrintingUnitCode == code).FirstOrDefault ();
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


		private static readonly string						nullPrinter = "(aucune)";

		private readonly List<PrintingUnit>					printingUnitList;
		private readonly List<DocumentPrintingUnitsEntity>	documentPrintingUnits;

		private CellTable									table;
		private FrameBox									printerBox;
		private Scrollable									optionsBox;

		private FrameBox									physicalBox;
		private TextFieldCombo								physicalField;

		private FrameBox									trayBox;
		private TextFieldCombo								trayField;

		private FrameBox									paperSizeBox;
		private TextFieldCombo								paperSizeField;

		private FrameBox									duplexBox;
		private TextFieldCombo								duplexField;

		private FrameBox									xOffsetBox;
		private TextFieldEx									xOffsetField;

		private FrameBox									yOffsetBox;
		private TextFieldEx									yOffsetField;

		private FrameBox									copiesBox;
		private TextFieldEx									copiesField;
	}
}
