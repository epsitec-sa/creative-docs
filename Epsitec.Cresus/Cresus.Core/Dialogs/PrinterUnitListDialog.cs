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
	/// Dialogue pour choisir les unités d'impression à utiliser.
	/// </summary>
	class PrinterUnitListDialog : AbstractDialog
	{
		public PrinterUnitListDialog(CoreApplication application)
		{
			this.application = application;

			this.printerUnitList = Printers.PrinterSettings.GetPrinterUnitList ();
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
			this.window.Text = "Définitions des unités d'impression disponibles";
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
			int tabIndex = 0;

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
				Text = "<font size=\"16\">Liste des unités d'impression</font>",
				Margins = new Margins (0, 0, 0, 10),
			};

			this.listController = new Controllers.ListController<PrinterUnit> (this.printerUnitList, this.ListControllerItemToText, this.ListControllerGetTextInfo, this.ListControllerCreateItem);
			this.listController.CreateUI (leftFrame, Direction.Right, 23);

			ToolTip.Default.SetToolTip (this.listController.AddButton,      "Ajoute une nouvelle unité d'impression");
			ToolTip.Default.SetToolTip (this.listController.RemoveButton,   "Supprime l'unité d'impression");
			ToolTip.Default.SetToolTip (this.listController.MoveUpButton,   "Montre l'unité d'impression dans la liste");
			ToolTip.Default.SetToolTip (this.listController.MoveDownButton, "Descend l'unité d'impression dans la liste");

			//	Rempli le panneau de droite.
			var rightTitle = new StaticText
			{
				Parent = rightFrame,
				PreferredHeight = 20,
				Dock = DockStyle.Top,
				Text = "<font size=\"16\">Choix pour l'unité d'impression sélectionnée</font>",
				Margins = new Margins (0, 0, 0, 10),
			};

			this.rightBox = new FrameBox
			{
				Parent = rightFrame,
				DrawFullFrame = true,
				BackColor = Widgets.ArrowedFrame.SurfaceColors.First (),
				Dock = DockStyle.Fill,
			};

			{
				var box = new FrameBox
				{
					Parent = this.rightBox,
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
					DefocusAction = Common.Widgets.DefocusAction.AcceptEdition,
					Parent = box,
					Dock = DockStyle.Top,
					Margins = new Margins (0, 0, 0, UIBuilder.MarginUnderTextField),
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
					DefocusAction = Common.Widgets.DefocusAction.AcceptEdition,
					Parent = box,
					Dock = DockStyle.Top,
					Margins = new Margins (0, 0, 0, UIBuilder.MarginUnderTextField),
					TabIndex = ++tabIndex,
				};
			}

			{
				var box = new FrameBox
				{
					Parent = this.rightBox,
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
			}

			{
				var box = new FrameBox
				{
					Parent = this.rightBox,
					DrawFullFrame = true,
					Dock = DockStyle.Top,
					Padding = new Margins (10),
					Margins = new Margins (0, 0, 0, -1),
				};

				this.xOffsetField = PrinterUnitListDialog.CreateTextField (box, "Décalage horizontal :", "[millimètres, vers la droite si positif]", ++tabIndex);
				this.yOffsetField = PrinterUnitListDialog.CreateTextField (box, "Décalage vertical :",   "[millimètres, vers le haut si positif]",   ++tabIndex);
			}

			{
				var box = new FrameBox
				{
					Parent = this.rightBox,
					DrawFullFrame = true,
					Dock = DockStyle.Top,
					Padding = new Margins (10),
					Margins = new Margins (0, 0, 0, -1),
				};

				this.copiesField = PrinterUnitListDialog.CreateTextField (box, "Nombre de copies :", "[×]", ++tabIndex);
			}

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

			this.copiesField.AcceptingEdition += delegate
			{
				this.ActionCopiesChanged ();
			};

			this.acceptButton.Clicked += delegate
			{
				Printers.PrinterSettings.SetPrinterList (this.printerUnitList);

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
				DefocusAction = Common.Widgets.DefocusAction.AcceptEdition,
				Parent = box,
				PreferredWidth = 80,
				Dock = DockStyle.Left,
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

			this.rightBox.Enable      = sel != -1;
			this.logicalField.Enable  = sel != -1;
			this.commentField.Enable  = sel != -1;
			this.physicalField.Enable = sel != -1;
			this.trayField.Enable     = sel != -1;
			this.xOffsetField.Enable  = sel != -1;
			this.yOffsetField.Enable  = sel != -1;
			this.copiesField.Enable   = sel != -1;

			if (sel == -1)
			{
				this.logicalField.Text  = null;
				this.commentField.Text  = null;
				this.physicalField.Text = null;
				this.trayField.Text     = null;
				this.xOffsetField.Text  = null;
				this.yOffsetField.Text  = null;
				this.copiesField.Text   = null;
			}
			else
			{
				PrinterUnit printerUnit = this.SelectedPrinter;

				this.logicalField.Text  = printerUnit.LogicalName;
				this.commentField.Text  = printerUnit.Comment;
				this.physicalField.Text = printerUnit.PhysicalPrinterName;
				this.trayField.Text     = printerUnit.PhysicalPrinterTray;
				this.xOffsetField.Text  = printerUnit.XOffset.ToString ();
				this.yOffsetField.Text  = printerUnit.YOffset.ToString ();
				this.copiesField.Text   = printerUnit.Copies.ToString ();
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
			PrinterUnit printerUnit = this.SelectedPrinter;
			List<string> trayNames = PrinterUnitListDialog.GetTrayList(printerUnit);

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

			if (this.printerUnitList[sel].LogicalName != this.logicalField.Text)
			{
				this.printerUnitList[sel].LogicalName = this.logicalField.Text;

				this.listController.UpdateList (sel);
				this.UpdateWidgets ();
			}
		}

		private void ActionCommentChanged()
		{
			int sel = this.listController.SelectedIndex;

			if (this.printerUnitList[sel].Comment != this.commentField.Text)
			{
				this.printerUnitList[sel].Comment = this.commentField.Text;

				this.listController.UpdateList (sel);
				this.UpdateWidgets ();
			}
		}

		private void ActionPhysicalChanged()
		{
			int sel = this.listController.SelectedIndex;

			if (this.printerUnitList[sel].PhysicalPrinterName != this.physicalField.Text)
			{
				this.printerUnitList[sel].PhysicalPrinterName = this.physicalField.Text;
				this.printerUnitList[sel].PhysicalPrinterTray = null;
				this.printerUnitList[sel].XOffset = 0;
				this.printerUnitList[sel].YOffset = 0;

				this.listController.UpdateList (sel);
				this.UpdateWidgets ();
				this.UpdateTrayField ();
			}
		}

		private void ActionTrayChanged()
		{
			int sel = this.listController.SelectedIndex;

			if (this.printerUnitList[sel].PhysicalPrinterTray != this.trayField.Text)
			{
				this.printerUnitList[sel].PhysicalPrinterTray = this.trayField.Text;

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
				if (this.printerUnitList[sel].XOffset != value)
				{
					this.printerUnitList[sel].XOffset = value;

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
				if (this.printerUnitList[sel].YOffset != value)
				{
					this.printerUnitList[sel].YOffset = value;

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

				if (this.printerUnitList[sel].Copies != value)
				{
					this.printerUnitList[sel].Copies = value;

					this.listController.UpdateList (sel);
					this.UpdateWidgets ();
				}
			}
		}


		private static List<string> GetTrayList(PrinterUnit printerUnit)
		{
			List<string> trayNames = new List<string> ();

			if (printerUnit != null && !string.IsNullOrEmpty (printerUnit.PhysicalPrinterName))
			{
				var settings = Common.Printing.PrinterSettings.FindPrinter (FormattedText.Unescape (printerUnit.PhysicalPrinterName));

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
				return string.Format ("Unité d'impression {0}", (this.printerUnitList.Count+1).ToString ());
			}
		}


		private string GetError()
		{
			for (int i = 0; i < this.printerUnitList.Count; i++)
			{
				if (string.IsNullOrWhiteSpace (this.printerUnitList[i].LogicalName))
				{
					return string.Format ("<b>Rang {0}</b>: Il faut spécifier la fonction de l'unité d'impression.", (i+1).ToString ());
				}

				if (string.IsNullOrWhiteSpace (this.printerUnitList[i].PhysicalPrinterName))
				{
					return string.Format ("<b>{0}</b>: Il faut choisir l'imprimante physique.", this.printerUnitList[i].LogicalName);
				}

				if (string.IsNullOrWhiteSpace (this.printerUnitList[i].PhysicalPrinterTray))
				{
					return string.Format ("<b>{0}</b>: Il faut choisir le bac.", this.printerUnitList[i].LogicalName);
				}

				if (!PrinterUnit.CheckString (this.printerUnitList[i].LogicalName))
				{
					return string.Format ("<b>{0}</b>: Ce nom de fonction est incorrect.", this.printerUnitList[i].LogicalName);
				}

				if (!PrinterUnit.CheckString (this.printerUnitList[i].Comment))
				{
					return string.Format ("<b>{0}</b>: La description est incorrecte.", this.printerUnitList[i].LogicalName);
				}

				for (int j = 0; j < this.printerUnitList.Count; j++)
				{
					if (j != i && this.printerUnitList[j].LogicalName == this.printerUnitList[i].LogicalName)
					{
						return string.Format ("<b>{0}</b>: Ces deux unités d'impression ont la même fonction.", this.printerUnitList[i].LogicalName);
					}
				}
			}

			return null;
		}


		private PrinterUnit SelectedPrinter
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
					return this.printerUnitList[sel];
				}
			}
		}


		#region ListController callbacks
		private FormattedText ListControllerItemToText(PrinterUnit printerUnit)
		{
			return printerUnit.NiceDescription;
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
		
		private PrinterUnit ListControllerCreateItem(int sel)
		{
			return new PrinterUnit (this.DefaultLogicalName);
		}
		#endregion


		private readonly CoreApplication				application;

		private Window									window;
		private Controllers.ListController<PrinterUnit>	listController;
		private FrameBox								rightBox;
		private TextFieldEx								logicalField;
		private TextFieldEx								commentField;
		private TextFieldCombo							physicalField;
		private TextFieldCombo							trayField;
		private TextFieldEx								xOffsetField;
		private TextFieldEx								yOffsetField;
		private TextFieldEx								copiesField;
		private StaticText								errorInfo;
		private Button									acceptButton;
		private Button									cancelButton;
		private List<PrinterUnit>						printerUnitList;
	}
}
