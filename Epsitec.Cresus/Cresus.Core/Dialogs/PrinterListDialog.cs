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
	/// <summary>
	/// Dialogue pour choisir les imprimantes à utiliser.
	/// </summary>
	class PrinterListDialog : AbstractDialog
	{
		public PrinterListDialog(CoreApplication application)
		{
			this.application = application;

			this.printerList = Printers.PrinterSettings.GetPrinterList ();
		}


		protected override Window CreateWindow()
		{
			this.window = new Window ();

			this.SetupWindow ();
			this.SetupWidgets ();
			this.UpdateWidgets ();

			this.window.AdjustWindowSize ();

			return this.window;
		}

		private void SetupWindow()
		{
			this.OwnerWindow = this.application.Window;
			this.window.Icon = this.application.Window.Icon;
			this.window.Text = "Définitions des imprimantes et bacs disponibles";
			this.window.MakeFixedSizeWindow ();
			this.window.ClientSize = new Size (640, 402);

			window.WindowCloseClicked += delegate
			{
				this.OnDialogClosed ();
				this.CloseDialog ();
			};
		}

		private void SetupWidgets()
		{
			var frame = new FrameBox
			{
				Parent = this.window.Root,
				Dock = DockStyle.Fill,
				Margins = new Margins (10, 10, 10, 0),
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
				PreferredWidth = 300,
				Dock = DockStyle.Right,
				Margins = new Margins (0, 0, 0, 0),
			};

			//	Rempli le panneau de gauche.
			var leftTitle = new StaticText
			{
				Parent = leftFrame,
				PreferredHeight = 20,
				Dock = DockStyle.Top,
				Text = "<font size=\"16\">Liste des imprimantes</font>",
				Margins = new Margins (0, 0, 0, 10),
			};

			this.listController = new Controllers.ListController<Printer> (this.printerList, this.ListControllerItemToText, this.ListControllerGetTextInfo, this.ListControllerCreateItem);
			this.listController.CreateUI (leftFrame, Direction.Right, 23);

			ToolTip.Default.SetToolTip (this.listController.AddButton,      "Ajoute une nouvelle impriante");
			ToolTip.Default.SetToolTip (this.listController.RemoveButton,   "Supprime l'imprimante");
			ToolTip.Default.SetToolTip (this.listController.MoveUpButton,   "Montre l'imprimante dans la liste");
			ToolTip.Default.SetToolTip (this.listController.MoveDownButton, "Descend l'imprimante dans la liste");

			//	Rempli le panneau de droite.
			var rightTitle = new StaticText
			{
				Parent = rightFrame,
				PreferredHeight = 20,
				Dock = DockStyle.Top,
				Text = "<font size=\"16\">Choix pour l'imprimante sélectionnée</font>",
				Margins = new Margins (0, 0, 0, 10),
			};

			var rightBox = new FrameBox
			{
				Parent = rightFrame,
				DrawFullFrame = true,
				BackColor = Widgets.ArrowedFrame.SurfaceColors.First (),
				Dock = DockStyle.Fill,
				Padding = new Margins (10),
			};

			this.logicalLabel = new StaticText
			{
				Parent = rightBox,
				Text = "Fonction de l'imprimante :",
				Dock = DockStyle.Top,
				Margins = new Margins (0, 0, 0, UIBuilder.MarginUnderLabel),
			};

			this.logicalField = new TextFieldEx
			{
				DefocusAction = Common.Widgets.DefocusAction.AcceptEdition,
				Parent = rightBox,
				Dock = DockStyle.Top,
				Margins = new Margins (0, 0, 0, 5),
				TabIndex = 1,
			};

			this.commentLabel = new StaticText
			{
				Parent = rightBox,
				Text = "Description :",
				Dock = DockStyle.Top,
				Margins = new Margins (0, 0, 0, UIBuilder.MarginUnderLabel),
			};

			this.commentField = new TextFieldEx
			{
				DefocusAction = Common.Widgets.DefocusAction.AcceptEdition,
				Parent = rightBox,
				Dock = DockStyle.Top,
				Margins = new Margins (0, 0, 0, 25),
				TabIndex = 2,
			};

			this.physicalLabel = new StaticText
			{
				Parent = rightBox,
				Text = "Choix de l'imprimante physique :",
				Dock = DockStyle.Top,
				Margins = new Margins (0, 0, 0, UIBuilder.MarginUnderLabel),
			};

			this.physicalField = new TextFieldCombo
			{
				IsReadOnly = true,
				Parent = rightBox,
				Dock = DockStyle.Top,
				Margins = new Margins (0, 0, 0, 5),
				TabIndex = 3,
			};

			this.trayLabel = new StaticText
			{
				Parent = rightBox,
				Text = "Choix du bac de l'imprimante :",
				Dock = DockStyle.Top,
				Margins = new Margins (0, 0, 0, UIBuilder.MarginUnderLabel),
			};

			this.trayField = new TextFieldCombo
			{
				IsReadOnly = true,
				Parent = rightBox,
				Dock = DockStyle.Top,
				Margins = new Margins (0, 0, 0, 25),
				TabIndex = 4,
			};

			this.xOffsetLabel = new StaticText
			{
				Parent = rightBox,
				Text = "Décalage horizontal en millimètres (+ = à droite) :",
				Dock = DockStyle.Top,
				Margins = new Margins (0, 0, 0, UIBuilder.MarginUnderLabel),
			};

			this.xOffsetField = new TextFieldEx
			{
				DefocusAction = Common.Widgets.DefocusAction.AcceptEdition,
				Parent = rightBox,
				Dock = DockStyle.Top,
				Margins = new Margins (0, 200, 0, 5),
				TabIndex = 5,
			};

			this.yOffsetLabel = new StaticText
			{
				Parent = rightBox,
				Text = "Décalage vertical en millimètres (+ = en haut) :",
				Dock = DockStyle.Top,
				Margins = new Margins (0, 0, 0, UIBuilder.MarginUnderLabel),
			};

			this.yOffsetField = new TextFieldEx
			{
				DefocusAction = Common.Widgets.DefocusAction.AcceptEdition,
				Parent = rightBox,
				Dock = DockStyle.Top,
				Margins = new Margins (0, 200, 0, 5),
				TabIndex = 6,
			};

			//	Rempli le pied de page.
			var footer = new FrameBox
			{
				Parent = this.window.Root,
				PreferredHeight = 20,
				Dock = DockStyle.Bottom,
				Margins = new Margins (10, 10, 10, 10),
			};

			this.errorInfo = new StaticText
			{
				Parent = footer,
				ContentAlignment = Common.Drawing.ContentAlignment.MiddleCenter,
				Dock = DockStyle.Fill,
				Margins = new Margins (0, 10, 0, 0),
			};

			this.cancelButton = new Button ()
			{
				Parent = footer,
				Text = "Annuler",
				Dock = DockStyle.Right,
				Margins = new Margins (10, 0, 0, 0),
				TabIndex = 101,
			};

			this.acceptButton = new Button ()
			{
				Parent = footer,
				Text = "D'accord",
				Dock = DockStyle.Right,
				TabIndex = 100,
			};

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

			this.xOffsetField.AcceptingEdition += delegate
			{
				this.ActionOffsetXChanged ();
			};

			this.yOffsetField.AcceptingEdition += delegate
			{
				this.ActionOffsetYChanged ();
			};

			this.acceptButton.Clicked += delegate
			{
				Printers.PrinterSettings.SetPrinterList (this.printerList);

				this.Result = DialogResult.Accept;
				this.CloseDialog ();
			};

			this.cancelButton.Clicked += delegate
			{
				this.Result = DialogResult.Cancel;
				this.CloseDialog ();
			};

			this.UpdatePhysicalField ();
			this.UpdateWidgets ();
		}

		private void UpdateWidgets()
		{
			int sel = this.listController.SelectedIndex;

			this.logicalLabel.Enable = sel != -1;
			this.logicalField.Enable = sel != -1;

			this.commentLabel.Enable = sel != -1;
			this.commentField.Enable = sel != -1;

			this.physicalLabel.Enable = sel != -1;
			this.physicalField.Enable = sel != -1;

			this.trayLabel.Enable = sel != -1;
			this.trayField.Enable = sel != -1;

			this.xOffsetLabel.Enable = sel != -1;
			this.xOffsetField.Enable = sel != -1;

			this.yOffsetLabel.Enable = sel != -1;
			this.yOffsetField.Enable = sel != -1;

			if (sel == -1)
			{
				this.logicalField.Text  = null;
				this.commentField.Text  = null;
				this.physicalField.Text = null;
				this.trayField.Text     = null;
				this.xOffsetField.Text  = null;
				this.yOffsetField.Text  = null;
			}
			else
			{
				Printer printer = this.SelectedPrinter;

				this.logicalField.Text  = printer.LogicalName;
				this.commentField.Text  = printer.Comment;
				this.physicalField.Text = printer.PhysicalPrinterName;
				this.trayField.Text     = printer.PhysicalPrinterTray;
				this.xOffsetField.Text  = printer.XOffset.ToString ();
				this.yOffsetField.Text  = printer.YOffset.ToString ();
			}

			string error = this.GetError ();

			if (string.IsNullOrEmpty (error))
			{
				this.errorInfo.Text = null;
				this.errorInfo.BackColor = Color.Empty;

				this.acceptButton.Enable = true;
			}
			else
			{
				this.errorInfo.Text = error;
				this.errorInfo.BackColor = Color.FromName ("Gold");

				this.acceptButton.Enable = false;
			}
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
			Printer printer = this.SelectedPrinter;
			List<string> trayNames = PrinterListDialog.GetTrayList(printer);

			this.trayField.Items.Clear ();
			foreach (var trayName in trayNames)
			{
				string name = FormattedText.Escape (trayName);

				if (!string.IsNullOrWhiteSpace (name) && !this.trayField.Items.Contains (name))
				{
					this.trayField.Items.Add (name);
				}
			}

			if (this.trayField.Items.Count == 1)
			{
				this.trayField.SelectedItemIndex = 0;
			}
		}


		private void ActionSelectedItemChanged()
		{
			this.UpdateWidgets ();
			this.UpdateTrayField ();
		}

		private void ActionItemInserted()
		{
			this.logicalField.SelectAll ();
			this.logicalField.Focus ();
		}

		private void ActionLogicalChanged()
		{
			int sel = this.listController.SelectedIndex;

			if (this.printerList[sel].LogicalName != this.logicalField.Text)
			{
				this.printerList[sel].LogicalName = this.logicalField.Text;

				this.listController.UpdateList (sel);
				this.UpdateWidgets ();
			}
		}

		private void ActionCommentChanged()
		{
			int sel = this.listController.SelectedIndex;

			if (this.printerList[sel].Comment != this.commentField.Text)
			{
				this.printerList[sel].Comment = this.commentField.Text;

				this.listController.UpdateList (sel);
				this.UpdateWidgets ();
			}
		}

		private void ActionPhysicalChanged()
		{
			int sel = this.listController.SelectedIndex;

			if (this.printerList[sel].PhysicalPrinterName != this.physicalField.Text)
			{
				this.printerList[sel].PhysicalPrinterName = this.physicalField.Text;
				this.printerList[sel].PhysicalPrinterTray = null;
				this.printerList[sel].XOffset = 0;
				this.printerList[sel].YOffset = 0;

				this.listController.UpdateList (sel);
				this.UpdateWidgets ();
				this.UpdateTrayField ();
			}
		}

		private void ActionTrayChanged()
		{
			int sel = this.listController.SelectedIndex;

			if (this.printerList[sel].PhysicalPrinterTray != this.trayField.Text)
			{
				this.printerList[sel].PhysicalPrinterTray = this.trayField.Text;

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
				if (this.printerList[sel].XOffset != value)
				{
					this.printerList[sel].XOffset = value;

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
				if (this.printerList[sel].YOffset != value)
				{
					this.printerList[sel].YOffset = value;

					this.listController.UpdateList (sel);
					this.UpdateWidgets ();
				}
			}
		}


		private static List<string> GetTrayList(Printer printer)
		{
			List<string> trayNames = new List<string> ();

			if (printer != null && !string.IsNullOrEmpty (printer.PhysicalPrinterName))
			{
				var settings = Common.Printing.PrinterSettings.FindPrinter (FormattedText.Unescape (printer.PhysicalPrinterName));

				if (settings != null)
				{
					System.Array.ForEach (settings.PaperSources, x => trayNames.Add (FormattedText.Escape (x.Name.Trim ())));
				}
			}

			return trayNames;
		}


		private string DefaultLogicalName
		{
			get
			{
				return string.Format ("Imprimante {0}", (this.printerList.Count+1).ToString ());
			}
		}


		private string GetError()
		{
			for (int i = 0; i < this.printerList.Count; i++)
			{
				if (string.IsNullOrWhiteSpace (this.printerList[i].LogicalName))
				{
					return string.Format ("<b>Rang {0}</b>: Il faut spécifier la fonction de l'imprimante.", (i+1).ToString ());
				}

				if (string.IsNullOrWhiteSpace (this.printerList[i].PhysicalPrinterName))
				{
					return string.Format ("<b>{0}</b>: Il faut choisir l'imprimante physique.", this.printerList[i].LogicalName);
				}

				if (string.IsNullOrWhiteSpace (this.printerList[i].PhysicalPrinterTray))
				{
					return string.Format ("<b>{0}</b>: Il faut choisir le bac.", this.printerList[i].LogicalName);
				}

				if (!Printer.CheckString (this.printerList[i].LogicalName))
				{
					return string.Format ("<b>{0}</b>: Ce nom de fonction est incorrect.", this.printerList[i].LogicalName);
				}

				if (!Printer.CheckString (this.printerList[i].Comment))
				{
					return string.Format ("<b>{0}</b>: La description est incorrecte.", this.printerList[i].LogicalName);
				}

				for (int j = 0; j < this.printerList.Count; j++)
				{
					if (j != i && this.printerList[j].LogicalName == this.printerList[i].LogicalName)
					{
						return string.Format ("<b>{0}</b>: Ces deux imprimantes ont la même fonction.", this.printerList[i].LogicalName);
					}
				}
			}

			return null;
		}


		private Printer SelectedPrinter
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
					return this.printerList[sel];
				}
			}
		}


		#region ListController callbacks
		private FormattedText ListControllerItemToText(Printer printer)
		{
			return printer.NiceDescription;
		}

		private FormattedText ListControllerGetTextInfo(int count)
		{
			if (count == 0)
			{
				return "Aucune imprimante définie";
			}
			else if (count == 1)
			{
				return string.Format ("{0} imprimante définie", count.ToString ());
			}
			else
			{
				return string.Format ("{0} imprimantes définies", count.ToString ());
			}
		}
		
		private Printer ListControllerCreateItem(int sel)
		{
			return new Printer (this.DefaultLogicalName);
		}
		#endregion


		private readonly CoreApplication				application;

		private Window									window;
		private Controllers.ListController<Printer>		listController;
		private StaticText								logicalLabel;
		private StaticText								commentLabel;
		private StaticText								physicalLabel;
		private StaticText								trayLabel;
		private StaticText								xOffsetLabel;
		private StaticText								yOffsetLabel;
		private TextFieldEx								logicalField;
		private TextFieldEx								commentField;
		private TextFieldCombo							physicalField;
		private TextFieldCombo							trayField;
		private TextFieldEx								xOffsetField;
		private TextFieldEx								yOffsetField;
		private StaticText								errorInfo;
		private Button									acceptButton;
		private Button									cancelButton;
		private List<Printer>							printerList;
	}
}
