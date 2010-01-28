//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Marc BETTEX

using Epsitec.Common;
using Epsitec.Common.Drawing;
using Epsitec.Common.Printing;
using Epsitec.Common.Support;
using Epsitec.Common.Widgets;
using Epsitec.Common.Widgets.Validators;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Epsitec.App.BanquePiguet
{

	class PrintersManager : Window
	{

		public PrintersManager(Application application)
		{
			this.Application = application;
			this.LoadPrinters ();

			this.SetupWindow ();
			this.SetupButtons ();
			this.SetupPrintersCellTable ();
			this.SetupEvents ();
			this.AdjustWindowSize ();
		}

		protected Application Application
		{
			get;
			set;
		}

		protected List<Printer> Printers
		{
			get;
			set;
		}

		protected CellTable PrintersCellTable
		{
			get;
			set;
		}

		protected List<AbstractTextField> Cells
		{
			get;
			set;
		}

		protected Button SaveButton
		{
			get;
			set;
		}

		protected Button CancelButton
		{
			get;
			set;
		}

		protected Button AddPrinterButton
		{
			get;
			set;
		}

		protected Button RemovePrinterButton
		{
			get;
			set;
		}

		protected void SetupWindow()
		{
			this.Text = "Configuration des imprimantes";
		}

		protected void SetupPrintersCellTable()
		{
			this.PrintersCellTable = new CellTable ()
			{
				Dock = DockStyle.Fill,
				Margins = new Margins (10, 10, 10, 10),
				Parent = this.Root,

			};

			this.Cells = new List<AbstractTextField> ();

			this.PrintersCellTable.StyleH  = CellArrayStyles.None;
			this.PrintersCellTable.StyleH |= CellArrayStyles.Header;
			this.PrintersCellTable.StyleH |= CellArrayStyles.Separator;
			this.PrintersCellTable.StyleH |= CellArrayStyles.Mobile;
			this.PrintersCellTable.StyleH |= CellArrayStyles.Stretch;

			this.PrintersCellTable.StyleV  = CellArrayStyles.None;
			this.PrintersCellTable.StyleV |= CellArrayStyles.ScrollNorm;
			this.PrintersCellTable.StyleV |= CellArrayStyles.Separator;
			this.PrintersCellTable.StyleV |= CellArrayStyles.SelectLine;

			this.PrintersCellTable.SetArraySize (5, this.Printers.Count);
			this.PrintersCellTable.SetWidthColumn (0, 25);

			this.PrintersCellTable.SetHeaderTextH (1, "Nom");
			this.PrintersCellTable.SetHeaderTextH (2, "Bac");
			this.PrintersCellTable.SetHeaderTextH (3, "Décalage x");
			this.PrintersCellTable.SetHeaderTextH (4, "Décalage y");

			for (int i = 0; i < this.Printers.Count; i++)
			{
				Printer printer = this.Printers[i];

				TextFieldCombo nameTextField = new TextFieldCombo ()
				{
					Dock = DockStyle.Fill,
					Text = printer.Name,
					
				};

				foreach (string printerName in PrinterSettings.InstalledPrinters)
				{
					nameTextField.Items.Add (printerName);
				}

				if (!nameTextField.Items.Contains (printer.Name) && printer.Name != "")
				{
					nameTextField.Items.Add (printer.Name);
				}

				new PredicateValidator (nameTextField, () => nameTextField.Text.Trim().Length > 0).Validate();

				TextFieldCombo trayTextField = new TextFieldCombo ()
				{
					Dock = DockStyle.Fill,
					Text = printer.Tray,
				};

				this.UpdateTrays (printer, trayTextField);

				new PredicateValidator (trayTextField, () => trayTextField.Text.Trim().Length > 0).Validate ();

				TextFieldUpDown xOffsetTextField = new TextFieldUpDown ()
				{
					Dock = DockStyle.Fill,
					MaxValue = Decimal.MaxValue,
					MinValue = Decimal.MinValue,
					Resolution = 0.1M,
					Step = 0.1M,
					Text = printer.XOffset.ToString (System.Globalization.CultureInfo.InvariantCulture),
				};

				TextFieldUpDown yOffsetTextField = new TextFieldUpDown ()
				{
					Dock = DockStyle.Fill,
					MaxValue = Decimal.MaxValue,
					MinValue = Decimal.MinValue,
					Resolution = 0.1M,
					Step = 0.1M,
					Text = printer.YOffset.ToString (System.Globalization.CultureInfo.InvariantCulture),
				};

				nameTextField.TextChanged += (sender) =>
				{
					printer.Name = nameTextField.Text;
					this.UpdateTrays (printer, trayTextField);
				};

				trayTextField.TextChanged += (sender) =>
				{
					printer.Tray = trayTextField.Text;
				};

				xOffsetTextField.TextChanged += (sender) =>
				{
					try
					{
						printer.XOffset = Double.Parse (xOffsetTextField.Text);
					}
					catch (Exception e)
					{
					}
				};

				yOffsetTextField.TextChanged += (sender) =>
				{
					try
					{
						printer.YOffset = Double.Parse (yOffsetTextField.Text);
					}
					catch (Exception e)
					{
					}
				};

				this.PrintersCellTable[1, i].Insert(nameTextField);
				this.PrintersCellTable[2, i].Insert (trayTextField);
				this.PrintersCellTable[3, i].Insert (xOffsetTextField);
				this.PrintersCellTable[4, i].Insert (yOffsetTextField);

				this.Cells.Add (nameTextField);
				this.Cells.Add (trayTextField);
				this.Cells.Add (xOffsetTextField);
				this.Cells.Add (yOffsetTextField);
			}

			this.PrintersCellTable.FinalSelectionChanged += (sender) => this.RemovePrinterButton.Enable = (this.PrintersCellTable.SelectedRow >= 0);
			this.Cells.ForEach (c => c.TextChanged += (sender) => this.CheckPrintersValidity ());
			this.CheckPrintersValidity ();
		}

		protected void UpdateTrays(Printer printer, TextFieldCombo trayTextField)
		{
			if (trayTextField.Items.Count > 0)
			{
				trayTextField.Items.Clear ();
			}

			PrinterSettings settings = PrinterSettings.FindPrinter (printer.Name);
			if (settings != null)
			{
				foreach (PaperSource trayName in settings.PaperSources)
				{
					trayTextField.Items.Add (trayName.Name);
				}
			}

			if (!trayTextField.Items.Contains (printer.Tray) && printer.Tray != "")
			{
				trayTextField.Items.Add (printer.Tray);
			}

		}

		protected void UpdatePrintersCellTable()
		{
			this.PrintersCellTable.Dispose ();
			this.SetupPrintersCellTable ();
		}

		protected void SetupButtons()
		{
			FrameBox buttonsFrameBox = new FrameBox ()
			{
				ContainerLayoutMode = ContainerLayoutMode.HorizontalFlow,
				Dock = DockStyle.Bottom,
				Parent = this.Root,
			};

			this.CancelButton = new Button ()
			{
				Dock = DockStyle.Right,
				Margins = new Margins (10, 10, 10, 10),
				Parent = buttonsFrameBox,
				Text = "Annuler",
			};

			this.SaveButton = new Button ()
			{
				Dock = DockStyle.Right,
				Margins = new Margins (10, 0, 10, 10),
				Parent = buttonsFrameBox,
				Text = "Sauvegarder",
			};

			this.AddPrinterButton = new Button ()
			{
				Dock = DockStyle.Left,
				Margins = new Margins (10, 10, 10, 10),
				Parent = buttonsFrameBox,
				Text = "Ajouter",
			};

			this.RemovePrinterButton = new Button ()
			{
				Dock = DockStyle.Left,
				Margins = new Margins (0, 0, 10, 10),
				Parent = buttonsFrameBox,
				Text = "Supprimer",
			};

		}

		protected void SetupEvents()
		{
			this.AddPrinterButton.Clicked += (sender, e) => this.AddPrinter ();
			this.RemovePrinterButton.Clicked += (sender, e) => this.RemovePrinter ();
			this.SaveButton.Clicked += (sender, e) => this.Save ();
			this.CancelButton.Clicked += (sender, e) => this.Cancel ();
			this.WindowClosed += (sender) => this.Cancel();
		}

		protected void CheckPrintersValidity()
		{
			this.SaveButton.Enable = this.Cells.All (c => c.IsValid);
			Console.WriteLine (this.SaveButton.Enable);
		}

		protected void LoadPrinters()
		{
			this.Printers = Printer.Load ();
		}

		protected void SavePrinters()
		{
			Printer.Save (this.Printers);
		}

		protected void Save()
		{
			this.SavePrinters ();
			this.Exit ();
		}

		protected void Cancel()
		{
			this.LoadPrinters ();
			this.Exit ();
		}

		protected void AddPrinter()
		{
			this.Printers.Add (new Printer ("", "", 0, 0));
			this.UpdatePrintersCellTable ();
		}

		protected void RemovePrinter()
		{
			int selectedRow = this.PrintersCellTable.SelectedRow;

			if (selectedRow >= 0)
			{
				this.Printers.RemoveAt (selectedRow);
			}

			this.UpdatePrintersCellTable ();
		}

		protected void Exit()
		{
			this.Application.DisplayPrintersManager (false);
		}

	}

}
