//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Marc BETTEX

using Epsitec.Common.Debug;
using Epsitec.Common.Dialogs;
using Epsitec.Common.Drawing;
using Epsitec.Common.IO;
using Epsitec.Common.Printing;
using Epsitec.Common.Support;
using Epsitec.Common.Types;
using Epsitec.Common.Widgets;

using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System;

namespace Epsitec.App.BanquePiguet
{

	/// <summary>
	/// The PrintDialog class is a window which lets the user choose on which printer he wants to 
	/// print the bv and how much copies does he want to print.
	/// </summary>
	class PrintDialog : Window
	{

		/// <summary>
		/// Initializes a new instance of the PrintDialog class.
		/// </summary>
		/// <param name="application">The Application creating this instance.</param>
		/// <param name="BvWidget">The BvWidget containing the bv data.</param>
		/// <param name="printers">The list of Printer that the user might select.</param>
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

		/// <summary>
		/// Gets or sets the Application who created this instance.
		/// </summary>
		/// <value>The Application who created this instance.</value>
		protected Application Application
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the BvWidget which contains the bv data.
		/// </summary>
		/// <value>The BvWidget which contains the bv data.</value>
		protected BvWidget BvWidget
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the list of Printers available to the user.
		/// </summary>
		/// <value>The list of Printers available to the user.</value>
		protected List<Printer> Printers
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the button used to print the bv.
		/// </summary>
		/// <value>The button used to prints the bv.</value>
		protected Button PrintButton
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the button used to close the window.
		/// </summary>
		/// <value>The button used to close the window.</value>
		protected Button CancelButton
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the text field used for the number of copies.
		/// </summary>
		/// <value>The text field used for the number of copies.</value>
		protected TextFieldUpDown NbCopiesTextField
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the text field used for the printer selection.
		/// </summary>
		/// <value>The text field used for the printer selection.</value>
		protected TextFieldCombo PrinterTextField
		{
			get;
			set;
		}

		/// <summary>
		/// Sets up the properties of the window of this instance.
		/// </summary>
		/// <remarks>
		/// This method is called at the initialization of this instance.
		/// </remarks>
		protected void SetupWindow()
		{
			this.Owner = this.Application.Window;
			this.Icon = this.Application.Window.Icon;
			this.Text = "Imprimer";
			this.WindowSize = new Size (100, 100);
			this.MakeFixedSizeWindow ();
		}

		/// <summary>
		/// Sets up the widgets of the PrintDialog, such as the PrintButton, CancelButton,
		/// NbCopiesTextField and PrinterTextField.
		/// </summary>
		/// <remarks>
		/// This method is called at the initialization of this instance.
		/// </remarks>
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
				IsReadOnly = true,
			};

			this.Printers.ForEach (printer => this.PrinterTextField.Items.Add (printer.Name));

			string preferredPrinter;
			bool preferredPrinterDefined = Settings.Load ().TryGetValue ("preferredPrinter", out preferredPrinter);

			if (preferredPrinterDefined && this.Printers.Any (printer => printer.Name == preferredPrinter))
			{
				this.PrinterTextField.Text = FormattedText.Escape (preferredPrinter);
			}
			else
			{
				this.PrinterTextField.Text = FormattedText.Escape (this.Printers[0].Name);
			}

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

		/// <summary>
		/// Sets up the events of this instance.
		/// </summary>
		/// <remarks>
		/// This method is called at the initialization of this instance.
		/// </remarks>
		protected void SetupEvents()
		{
			this.PrinterTextField.TextChanged += (sender) => this.UpdateNbPagesRange ();
			this.PrinterTextField.TextChanged += (sender) => this.CheckPrintEnabled ();
			this.NbCopiesTextField.TextChanged += (sender) => this.CheckPrintEnabled ();
			this.CancelButton.Clicked += (sender, e) => this.Close ();
			this.PrintButton.Clicked += (sender, e) => this.Print ();
			this.WindowClosed += (sender) => this.Close ();
		}

		/// <summary>
		/// Updates the range of NbCopiesTextField based on the value of PrinterTextField.
		/// </summary>
		/// <remarks>
		/// This method is called whenever the text of PrinterTextField changes.
		/// </remarks>
		protected void UpdateNbPagesRange()
		{
			PrinterSettings printer = PrinterSettings.FindPrinter(FormattedText.Unescape (this.PrinterTextField.Text));

			this.NbCopiesTextField.MinValue = 1;
			this.NbCopiesTextField.MaxValue = printer.MaximumCopies;
		}

		/// <summary>
		/// Enables or disables PrintButton according to the validity of PrinterTextField and
		/// NbCopiesTextField.
		/// </summary>
		/// <remarks>
		/// This method is called whenever the text of PrinterTextField and NbCopiesTextField
		/// changes.
		/// </remarks>
		protected void CheckPrintEnabled()
		{
			this.PrintButton.Enable = this.NbCopiesTextField.IsValid;	
		}

		/// <summary>
		/// Prints BvWidget with the printer given by PrinterTextField with the number of copies
		/// given by NbCopiesTextField and logs what has been printed to the log file.
		/// </summary>
		protected void Print()
		{
			try
			{
				this.PrintBv ();
				this.LogPrint ();
			}
			catch (System.Exception e)
			{
				MessageDialog.CreateOk ("Erreur", DialogIcon.Warning, "Une erreur s'est produite lors de l'impression").OpenDialog ();
				ErrorLogger.LogException (new System.Exception ("An error occured while printing.", e));
			}
			finally
			{
				Settings settings = Settings.Load ();
				settings["preferredPrinter"] = FormattedText.Unescape (this.PrinterTextField.Text);
				settings.Save ();
				this.Close ();
			}
		}


		/// <summary>
		/// Prints the bv corresponding to the data contained in BvWidget.
		/// </summary>
		protected void PrintBv()
		{
			Printer printer = this.Printers.Find(p => p.Name == FormattedText.Unescape (this.PrinterTextField.Text));
			PrintDocument printDocument = new PrintDocument();

			printDocument.DocumentName = String.Format ("bv {0}", this.BvWidget.BeneficiaryIban);
			printDocument.SelectPrinter(printer.Name);
			printDocument.PrinterSettings.Copies = int.Parse (FormattedText.Unescape (this.NbCopiesTextField.Text), CultureInfo.InvariantCulture);
			printDocument.DefaultPageSettings.Margins = new Margins (0, 0, 0, 0);
			printDocument.DefaultPageSettings.PaperSource = System.Array.Find (printDocument.PrinterSettings.PaperSources, paperSource => paperSource.Name == printer.Tray);

			double height = this.BvWidget.BvSize.Height;
			double width = this.BvWidget.BvSize.Width;

			double xOffset = printer.XOffset;
			double yOffset = printer.YOffset;
			PrintPort.PrintSinglePage (painter => this.BvWidget.Print (painter, new Rectangle (xOffset, yOffset, width, height)), printDocument);
		}

		/// <summary>
		/// Logs the values of PrinterTextField and NbCopiesTextField as well as part of the data
		/// of BvWidget.
		/// </summary>
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

			 Logger.Log (entry);
		}

	}

}
