﻿//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
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

			this.documentPrintingUnits = this.Container.Data.GetAllEntities<DocumentPrintingUnitsEntity> ();
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
				PreferredWidth = 300,
				Dock = DockStyle.Right,
				Margins = new Margins (10, 0, 0, 0),
			};

			var column2 = new FrameBox
			{
				Parent = frame,
				PreferredWidth = 300,
				Dock = DockStyle.Right,
				Margins = new Margins (0, 0, 0, 0),
			};

			//	Rempli la colonne de gauche.
			var columnTitle1 = new StaticText (column1);
			columnTitle1.SetColumnTitle ("Liste des unités d'impression");

			this.list = new ScrollList
			{
				Parent = column1,
				Dock = DockStyle.Fill,
			};

			//	Rempli la deuxième colonne.
			var columnTitle2 = new StaticText (column2);
			columnTitle2.SetColumnTitle ("Choix pour l'unité d'impression sélectionnée");

			this.physicalBox = new FrameBox
			{
				Parent = column2,
				DrawFullFrame = true,
				BackColor = Widgets.ArrowedFrame.SurfaceColors.First (),
				Dock = DockStyle.Fill,
			};

			{
				var box = new FrameBox
				{
					Parent = this.physicalBox,
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
					Parent = this.physicalBox,
					DrawFullFrame = true,
					Dock = DockStyle.Top,
					Padding = new Margins (10),
					Margins = new Margins (0, 0, 0, -1),
				};

				this.xOffsetField = PrintingUnitsTabPage.CreateTextField (box, "Décalage horizontal :", "mm, vers la droite si positif", ++tabIndex);
				this.yOffsetField = PrintingUnitsTabPage.CreateTextField (box, "Décalage vertical :",   "mm, vers le haut si positif",   ++tabIndex);
			}

			{
				var box = new FrameBox
				{
					Parent = this.physicalBox,
					DrawFullFrame = true,
					Dock = DockStyle.Top,
					Padding = new Margins (10),
					Margins = new Margins (0, 0, 0, -1),
				};

				this.copiesField = PrintingUnitsTabPage.CreateTextField (box, "Nombre de copies :", "×", ++tabIndex);
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
			this.list.SelectedItemChanged += delegate
			{
				this.ListChanged ();
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

			this.UpdateList ();
			this.UpdatePhysicalField ();
			this.UpdateOptions ();
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
				Margins = new Margins (5, 0, 0, 0),
			};

			return field;
		}

		private void UpdateList()
		{
			this.list.Items.Clear ();

			foreach (var x in this.documentPrintingUnits)
			{
				this.list.Items.Add (x.Name);
			}
		}

		private void UpdateWidgets()
		{
			int sel = this.list.SelectedItemIndex;

			this.physicalBox.Enable    = sel != -1;
			this.optionsBox.Enable     = sel != -1;
			this.physicalField.Enable  = sel != -1;
			this.trayField.Enable      = sel != -1;
			this.paperSizeField.Enable = sel != -1;
			this.duplexField.Enable    = sel != -1;
			this.xOffsetField.Enable   = sel != -1;
			this.yOffsetField.Enable   = sel != -1;
			this.copiesField.Enable    = sel != -1;

			if (sel == -1)
			{
				this.physicalField.Text  = null;
				this.trayField.Text      = null;
				this.paperSizeField.Text = null;
				this.duplexField.Text    = null;
				this.xOffsetField.Text   = null;
				this.yOffsetField.Text   = null;
				this.copiesField.Text    = null;
			}
			else
			{
				PrintingUnit printingUnit = this.SelectedPrinter;

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

		private void UpdateOptions()
		{
			this.optionsBox.Viewport.Children.Clear ();

			var optionsKeys = new PrintingOptionDictionary ();
			foreach (var option in VerboseDocumentOption.GetDefault ())
			{
				optionsKeys[option.Option] = "";
			}

			PrintingUnit printingUnit = this.SelectedPrinter;
			if (printingUnit != null)
			{
				var optionsValues = printingUnit.Options;

				var controller = new DocumentOptionsEditor.OptionsController (optionsKeys, optionsValues, true);
				controller.CreateUI (this.optionsBox.Viewport, null);
			}
		}


		private void ListChanged()
		{
			this.UpdateWidgets ();
			this.UpdateTrayField ();
			this.UpdatePaperSizeField ();
			this.UpdateDuplexField ();
		}


		private void ActionPhysicalChanged()
		{
			int sel = this.list.SelectedItemIndex;

			if (this.printingUnitList[sel].PhysicalPrinterName != this.physicalField.Text)
			{
				this.printingUnitList[sel].PhysicalPrinterName = this.physicalField.Text;
				this.printingUnitList[sel].PhysicalPrinterTray = null;
				this.printingUnitList[sel].XOffset = 0;
				this.printingUnitList[sel].YOffset = 0;

				this.UpdateWidgets ();
				this.UpdateTrayField ();
				this.UpdatePaperSizeField ();
				this.UpdateDuplexField ();
			}
		}

		private void ActionTrayChanged()
		{
			int sel = this.list.SelectedItemIndex;

			if (this.printingUnitList[sel].PhysicalPrinterTray != this.trayField.Text)
			{
				this.printingUnitList[sel].PhysicalPrinterTray = this.trayField.Text;

				this.UpdateWidgets ();
			}
		}

		private void ActionPaperSizeChanged()
		{
			int sel = this.list.SelectedItemIndex;

			var paperSize = PrintingUnitsTabPage.NiceDescriptionToPaperSize (this.paperSizeField.Text);

			if (this.printingUnitList[sel].PhysicalPaperSize != paperSize)
			{
				this.printingUnitList[sel].PhysicalPaperSize = paperSize;

				this.UpdateWidgets ();
			}
		}

		private void ActionDuplexChanged()
		{
			int sel = this.list.SelectedItemIndex;

			var duplex = PrintingUnit.DescriptionToDuplex (this.duplexField.Text);

			if (this.printingUnitList[sel].PhysicalDuplexMode != duplex)
			{
				this.printingUnitList[sel].PhysicalDuplexMode = duplex;

				this.UpdateWidgets ();
			}
		}

		private void ActionOffsetXChanged()
		{
			int sel = this.list.SelectedItemIndex;

			double value;
			if (double.TryParse (this.xOffsetField.Text, out value))
			{
				if (this.printingUnitList[sel].XOffset != value)
				{
					this.printingUnitList[sel].XOffset = value;

					this.UpdateWidgets ();
				}
			}
		}

		private void ActionOffsetYChanged()
		{
			int sel = this.list.SelectedItemIndex;

			double value;
			if (double.TryParse (this.yOffsetField.Text, out value))
			{
				if (this.printingUnitList[sel].YOffset != value)
				{
					this.printingUnitList[sel].YOffset = value;

					this.UpdateWidgets ();
				}
			}
		}

		private void ActionCopiesChanged()
		{
			int sel = this.list.SelectedItemIndex;

			int value;
			if (int.TryParse (this.copiesField.Text, out value))
			{
				value = System.Math.Max (value, 1);

				if (this.printingUnitList[sel].Copies != value)
				{
					this.printingUnitList[sel].Copies = value;

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
#if false
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
				
				for (int j = 0; j < this.printingUnitList.Count; j++)
				{
					if (j != i && this.printingUnitList[j].LogicalName == this.printingUnitList[i].LogicalName)
					{
						return string.Format ("<b>{0}</b>: Ces deux unités d'impression ont la même fonction.", this.printingUnitList[i].LogicalName);
					}
				}

				if (this.printingUnitList[i].PageTypes.Count == 0)
				{
					return string.Format ("<b>Rang {0}</b>: Il faut spécifier au moins un type de page à imprimer.", (i+1).ToString ());
				}
			}
#endif

			return null;
		}


		private PrintingUnit SelectedPrinter
		{
			get
			{
				int sel = this.list.SelectedItemIndex;

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


		private readonly List<PrintingUnit>					printingUnitList;
		private readonly IEnumerable<DocumentPrintingUnitsEntity> documentPrintingUnits;

		private ScrollList									list;
		private FrameBox									physicalBox;
		private Scrollable									optionsBox;
		private TextFieldCombo								physicalField;
		private TextFieldCombo								trayField;
		private TextFieldCombo								paperSizeField;
		private StaticText									duplexLabel;
		private TextFieldCombo								duplexField;
		private TextFieldEx									xOffsetField;
		private TextFieldEx									yOffsetField;
		private TextFieldEx									copiesField;
	}
}
