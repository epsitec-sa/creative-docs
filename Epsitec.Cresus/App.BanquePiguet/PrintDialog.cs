//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Marc BETTEX

using Epsitec.Common.Dialogs;
using Epsitec.Common.Drawing;
using Epsitec.Common.Printing;
using Epsitec.Common.Types;
using Epsitec.Common.Widgets;

using System.Collections.Generic;
using System.Globalization;

namespace Epsitec.App.BanquePiguet
{

	class PrintDialog : Window
	{

		public PrintDialog(Application application, BvWidget BvWidget, List<Printer> printers)
		{
			this.Application = application;
			this.BvWidget = BvWidget;
			this.Printers = printers;

			this.SetupWindow ();
			this.SetupWidgets ();
			this.SetupEvents ();
			this.AdjustWindowSize ();
		}

		protected Application Application
		{
			get;
			set;
		}

		protected BvWidget BvWidget
		{
			get;
			set;
		}

		protected List<Printer> Printers
		{
			get;
			set;
		}

		protected Button PrintButton
		{
			get;
			set;
		}

		protected Button CancelButton
		{
			get;
			set;
		}

		protected TextFieldUpDown NbCopiesTextField
		{
			get;
			set;
		}

		protected TextFieldCombo PrinterTextField
		{
			get;
			set;
		}

		protected void SetupWindow()
		{
			this.Text = "Imprimer";
			this.WindowSize = new Size (100, 100);
			this.MakeFixedSizeWindow ();
		}

		protected void SetupWidgets()
		{
			FrameBox frameBox = new FrameBox ()
			{
				ContainerLayoutMode = ContainerLayoutMode.VerticalFlow,
				Dock = DockStyle.Top,
				Parent = this.Root
			};

			GroupBox printerGroupBox = new GroupBox ()
			{
				ContainerLayoutMode = ContainerLayoutMode.VerticalFlow,
				Dock = DockStyle.Stacked,
				Margins = new Margins (10, 10, 10, 10),
				Padding = new Margins (5, 5, 0, 5),
				Parent = frameBox,
				Text = "Imprimante",
				TabIndex = 1,
			};

			this.PrinterTextField = new TextFieldCombo ()
			{
				Dock = DockStyle.Stacked,
				Parent = printerGroupBox,
				PreferredWidth = 200,
				TabIndex = 1,
				Text = this.Printers[0].Name,
				IsReadOnly = true,
			};

			this.Printers.ForEach (printer => this.PrinterTextField.Items.Add (printer.Name));

			GroupBox nbPagesGroupBox = new GroupBox ()
			{
				ContainerLayoutMode = ContainerLayoutMode.VerticalFlow,
				Dock = DockStyle.Stacked,
				Margins = new Margins (10, 10, 0, 10),
				Padding = new Margins (5, 5, 0, 5),
				Parent = frameBox,
				Text = "Nombre de bulletins",
				TabIndex = 2,
			};

			this.NbCopiesTextField = new TextFieldUpDown ()
			{
				Dock = DockStyle.Stacked,
				Parent = nbPagesGroupBox,
				PreferredWidth = 200,
				Resolution = 1,
				Step = 1,
				TabIndex = 1,
				Text = "1",
			};

			this.UpdateNbPagesRange ();

			FrameBox buttonsFrameBox = new FrameBox ()
			{
				ContainerLayoutMode = ContainerLayoutMode.HorizontalFlow,
				Dock = DockStyle.Stacked,
				Parent = frameBox,
				TabIndex = 3,
			};

			this.PrintButton = new Button ()
			{
				Dock = DockStyle.StackFill,
				Margins = new Margins (10, 10, 0, 10),
				Parent = buttonsFrameBox,
				Text = "Ok",
				TabIndex = 1,
			};

			this.CancelButton = new Button ()
			{
				Dock = DockStyle.StackFill,
				Margins = new Margins (0, 10, 0, 10),
				Parent = buttonsFrameBox,
				Text = "Annuler",
				TabIndex = 2,
			};
		}

		protected void SetupEvents()
		{
			this.PrinterTextField.TextChanged += (sender) => this.UpdateNbPagesRange ();
			this.PrinterTextField.TextChanged += (sender) => this.CheckPrintEnabled ();
			this.NbCopiesTextField.TextChanged += (sender) => this.CheckPrintEnabled ();
			this.CancelButton.Clicked += (sender, e) => this.Exit ();
			this.PrintButton.Clicked += (sender, e) => this.Print ();
			this.WindowClosed += (sender) => this.Exit ();
		}

		protected void UpdateNbPagesRange()
		{
			PrinterSettings printer = PrinterSettings.FindPrinter(FormattedText.Unescape (this.PrinterTextField.Text));

			this.NbCopiesTextField.MinValue = 1;
			this.NbCopiesTextField.MaxValue = printer.MaximumCopies;
		}

		protected void CheckPrintEnabled()
		{
			this.PrintButton.Enable = this.NbCopiesTextField.IsValid;	
		}

		protected void Print()
		{
			try
			{
				this.PrintBvs ();
				this.LogPrint ();
			}
			catch (System.Exception e)
			{
				MessageDialog.CreateOk ("Erreur", DialogIcon.Warning, "Une erreur s'est produite lors de l'impression").OpenDialog ();
				Tools.LogException (new System.Exception ("An error occured while printing." , e));
			}
			finally
			{
				this.Exit ();
			}
		}

		protected void Exit()
		{
			this.Application.DisplayPrintDialog (false);
		}

		protected void PrintBvs()
		{
			Printer printer = this.Printers.Find(p => p.Name == FormattedText.Unescape (this.PrinterTextField.Text));
			PrintDocument printDocument = new PrintDocument();

			printDocument.SelectPrinter(printer.Name);
			printDocument.PrinterSettings.Copies = int.Parse (FormattedText.Unescape (this.NbCopiesTextField.Text), CultureInfo.InvariantCulture);
			printDocument.DefaultPageSettings.Margins = new Margins (0, 0, 0, 0);
			printDocument.DefaultPageSettings.PaperSource = System.Array.Find (printDocument.PrinterSettings.PaperSources, paperSource => paperSource.Name == printer.Tray);

			double height = this.BvWidget.BvSize.Height / 10;
			double width = this.BvWidget.BvSize.Width / 10;

			double xOffset = printer.XOffset / 10;
			double yOffset = printer.YOffset / 10;

			PrintPort.PrintSinglePage (painter => this.BvWidget.Print (painter, new Rectangle (xOffset, yOffset, width, height)), printDocument, 21, (int) 29.7);
		}

		protected void LogPrint()
		{
			string entry = string.Format ("{0}\nPrinter: {1}\nNumber of copies: {2}\nBeneficiary iban: {3}\nBeneficiary address: {4}\nReason: {5}",
				"========= New entry =========",
				FormattedText.Unescape (this.PrinterTextField.Text),
				FormattedText.Unescape (this.NbCopiesTextField.Text),
				this.BvWidget.BeneficiaryIban,
				this.BvWidget.BeneficiaryAddress.Replace ("\n", "\t\t"),
				this.BvWidget.Reason.Replace ("\n", "\t\t")
			);

			Tools.LogMessage (entry);
		}

	}

}
