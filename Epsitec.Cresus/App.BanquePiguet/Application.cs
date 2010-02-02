//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Marc BETTEX

using Epsitec.Common.Dialogs;
using Epsitec.Common.Drawing;
using Epsitec.Common.Printing;
using Epsitec.Common.Types;
using Epsitec.Common.Widgets;
using Epsitec.Common.Widgets.Validators;

using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace Epsitec.App.BanquePiguet
{

	class Application : Epsitec.Common.Widgets.Application
	{

		public Application(bool adminMode)
		{
			this.AdminMode = adminMode;
			this.SetupWindow ();
			this.SetupForm ();
			this.SetupBvWidget ();
			this.SetupEvents ();
			this.SetupValidators ();
			this.CheckPrintButtonEnbled ();
			this.Window.AdjustWindowSize ();

			//this.BenefeciaryIbanTextField.Text = "CH01 2345 6789 0123 4567 8";
			//this.BeneficiaryAddressTextField.Text = FormattedText.Escape("Monsieur Alfred DUPOND\nRue de la tarte 85 bis\n7894 Tombouctou\nCocagne Land");
			//this.ReasonTextField.Text = FormattedText.Escape("0123456789\n0123456789\n0123456789");
			//this.DisplayPrintersManager (true);
			//this.DisplayPrintDialog (true);
		}

		public override string ShortWindowTitle
		{
			get
			{
				return "Banque Piguet";
			}
		}

		public bool AdminMode
		{
			get;
			protected set;
		}

		protected TextField BenefeciaryIbanTextField
		{
			get;
			set;
		}

		protected TextFieldMulti BeneficiaryAddressTextField
		{
			get;
			set;
		}

		protected TextFieldMulti ReasonTextField
		{
			get;
			set;
		}

		protected BvWidget BvWidget
		{
			get;
			set;
		}

		protected Button PrintButton
		{
			get;
			set;
		}

		protected Button OptionsButton
		{
			get;
			set;
		}

		protected PrintersManager PrintersManager
		{
			get;
			set;
		}

		protected PrintDialog PrintDialog
		{
			get;
			set;
		}

		protected void SetupWindow()
		{
			this.Window = new Window ()
			{
				Text = this.ShortWindowTitle,
			};
			this.Window.MakeFixedSizeWindow ();
		}

		protected void SetupForm()
		{

			FrameBox formFrameBox = new FrameBox ()
			{
				ContainerLayoutMode = ContainerLayoutMode.VerticalFlow,
				Dock = DockStyle.Left,
				Parent = this.Window.Root
			};

			GroupBox beneficiaryIbanGroupBox = new GroupBox ()
			{
				ContainerLayoutMode = ContainerLayoutMode.VerticalFlow,
				Dock = DockStyle.Stacked,
				Margins = new Margins (10, 10, 10, 10),
				Padding = new Margins (5, 5, 0, 5),
				Parent = formFrameBox,
				Text = "N° IBAN du bénéficiaire",
				TabIndex = 1,
			};

			this.BenefeciaryIbanTextField = new TextField ()
			{
				Dock = DockStyle.Stacked,
				Parent = beneficiaryIbanGroupBox,
				PreferredWidth = 200,
				TabIndex = 1,
			};
			this.BenefeciaryIbanTextField.Focus ();
			
			GroupBox beneficiaryAddressGroupBox = new GroupBox ()
			{
				ContainerLayoutMode = ContainerLayoutMode.VerticalFlow,
				Dock = DockStyle.Stacked,
				Margins = new Margins (10, 10, 0, 10),
				Padding = new Margins (5, 5, 0, 5),
				Parent = formFrameBox,
				Text = "Nom et adresse du bénéficiaire",
				TabIndex = 2,
			};

			this.BeneficiaryAddressTextField = new TextFieldMulti ()
			{
				Dock = DockStyle.Stacked,
				Parent = beneficiaryAddressGroupBox,
				PreferredHeight = 65,
				PreferredWidth = 200,
				ScrollerVisibility = false,
				TabIndex = 2,
			};

			GroupBox reasonGroupBox = new GroupBox ()
			{
				ContainerLayoutMode = ContainerLayoutMode.VerticalFlow,
				Dock = DockStyle.Stacked,
				Margins = new Margins (10, 10, 0, 10),
				Padding = new Margins (5, 5, 0, 5),
				Parent = formFrameBox,
				Text = "Motif du versement",
				TabIndex = 3,
			};

			this.ReasonTextField = new TextFieldMulti ()
			{
				Dock = DockStyle.Stacked,
				Parent = reasonGroupBox,
				PreferredHeight = 52,
				PreferredWidth = 200,
				ScrollerVisibility = false,
				TabIndex = 3,
			};

			FrameBox buttonsFrameBox = new FrameBox ()
			{
				ContainerLayoutMode = ContainerLayoutMode.HorizontalFlow,
				Dock = DockStyle.Stacked,
				Parent = formFrameBox,
				TabIndex = 10,
			};

			this.PrintButton = new Button ()
			{
				Dock = DockStyle.StackFill,
				Margins = new Margins (10, 10, 0, 10),
				Parent = buttonsFrameBox,
				Text = "Imprimer",
				TabIndex = 1,
			};

			if (this.AdminMode)
			{
				this.OptionsButton = new Button ()
				{
					Dock = DockStyle.StackFill,
					Margins = new Margins (0, 10, 0, 10),
					Parent = buttonsFrameBox,
					Text = "Options",
					TabIndex = 2,
				};
			}
		}

		protected void SetupBvWidget()
		{
			this.BvWidget = new BvWidget ()
			{
				Dock = DockStyle.Left,
				Margins = new Margins (10, 10, 10, 10),
				Parent = this.Window.Root,
				PreferredSize = new Size (525, 265),
			};

			try
			{
				XElement xBvValues;

				using (XmlReader xmlReader = XmlReader.Create (Tools.GetResourceStream("BvValues.xml")))
				{
					xBvValues = XElement.Load (xmlReader);
				}
				
				int nbValues = 0;

				foreach (XElement value in xBvValues.Elements("value"))
				{
					string name = (string) value.Element ("name");
					string text = (string) value.Element ("text");
					
					if (text.Contains ("\\n"))
					{
						string[] lines = text.Replace("\\n", "\n").Split ('\n');
						text = lines.Aggregate ((a, b) => string.Format ("{0}\n{1}", a, b));
					}

					switch (name)
					{
						case "BankAddress":
							this.BvWidget.BankAddress = text;
							break;
						case "BankAccount":
							this.BvWidget.BankAccount = text;
							break;
						case "LayoutCode":
							this.BvWidget.LayoutCode = text;
							break;
						case "ReferenceClientNumber":
							this.BvWidget.ReferenceClientNumber = text;
							break;
						case "ClearingConstant":
							this.BvWidget.ClearingConstant = text;
							break;
						case "ClearingBank":
							this.BvWidget.ClearingBank = text;
							break;
						case "ClearingBankKey":
							this.BvWidget.ClearingBankKey = text;
							break;
						case "CcpNumber":
							this.BvWidget.CcpNumber = text;
							break;
						default:
							throw new System.Exception (string.Format ("Unknown value: {0}", name));
					}

					nbValues++;
				}

				if (nbValues < 8)
				{
					throw new System.Exception ("Some bv values are missing.");
				}

			
			}
			catch (System.Exception e)
			{
				Tools.Error (new System.Exception ("An error occured while loading the bv values.", e));
			}
		}

		protected void SetupEvents()
		{
			this.BenefeciaryIbanTextField.TextChanged += (sender) =>
			{
				if (this.CheckBeneficiaryIban ())
				{
					this.BvWidget.BeneficiaryIban = FormattedText.Unescape (this.BenefeciaryIbanTextField.Text);
				}

				this.CheckPrintButtonEnbled ();
			};

			this.BeneficiaryAddressTextField.TextChanged += (sender) =>
			{
				if (this.CheckBeneficiaryAddress ())
				{
					this.BvWidget.BeneficiaryAddress = FormattedText.Unescape (this.BeneficiaryAddressTextField.Text);
				}

				this.CheckPrintButtonEnbled ();
			};

			this.ReasonTextField.TextChanged += (sender) =>
			{
				if (this.CheckReason ())
				{
					this.BvWidget.Reason = FormattedText.Unescape(this.ReasonTextField.Text);
				}

				this.CheckPrintButtonEnbled ();
			};

			if (this.AdminMode)
			{
				this.OptionsButton.Clicked += (sender, e) => this.DisplayPrintersManager (true);
			}

			this.PrintButton.Clicked += (sender, e) => this.DisplayPrintDialog (true);
		}

		protected void SetupValidators()
		{
			new PredicateValidator (
				this.BenefeciaryIbanTextField,
				() => this.CheckBeneficiaryIban ()
			);
			
			new PredicateValidator (
				this.BeneficiaryAddressTextField,
				() => this.CheckBeneficiaryAddress ()
			);
			
			new PredicateValidator (
				this.ReasonTextField,
				() => this.CheckReason ());
		}

		protected void SetupPrintersManager()
		{
			this.PrintersManager = new PrintersManager (this);
		}

		protected bool CheckBeneficiaryIban()
		{
			string text = FormattedText.Unescape (this.BenefeciaryIbanTextField.Text);
			string iban = BvHelper.BuildNormalizedIban (text);
			return BvHelper.CheckBeneficiaryIban (iban);
		}

		protected bool CheckBeneficiaryAddress()
		{
			string address = FormattedText.Unescape (this.BeneficiaryAddressTextField.Text);
			return BvHelper.CheckBeneficiaryAddress (address);
		}

		protected bool CheckReason()
		{
			string reason = FormattedText.Unescape (this.ReasonTextField.Text);
			return BvHelper.CheckReason (reason);
		}

		protected void CheckPrintButtonEnbled()
		{
			bool check  =  BvHelper.CheckBv (this.BvWidget)
						&& this.CheckBeneficiaryIban ()
						&& this.CheckBeneficiaryAddress ()
						&& this.CheckReason ();

			this.PrintButton.Enable = check;
		}

		public void DisplayPrintersManager(bool display)
		{
			if (display)
			{
				this.PrintersManager = new PrintersManager (this);
				this.PrintersManager.Show ();
				this.Window.IsFrozen = true;
			}
			else
			{
				this.PrintersManager.Dispose ();
				this.PrintersManager = null;
				this.Window.IsFrozen = false;
			}
		}

		public void DisplayPrintDialog(bool display)
		{
			if (display)
			{
				List<Printer> printers = new List<Printer>
				(
					from printer in Printer.Load ()
					where PrinterSettings.InstalledPrinters.Contains (printer.Name)
					select printer
				);

				bool checkPrintersList = (printers.Count > 0);
				bool checkPrintersTrays = printers.All (printer =>
				{
					PaperSource[] trays = PrinterSettings.FindPrinter (printer.Name).PaperSources;
					return trays.Any (tray => (tray.Name == printer.Tray));  
				});

				if (!checkPrintersList)
				{
					MessageDialog.CreateOk ("Erreur", DialogIcon.Warning, "Aucune imprimante n'est configurée pour cet ordinateur.").OpenDialog ();
				}
				else if (!checkPrintersTrays)
				{
					MessageDialog.CreateOk ("Erreur", DialogIcon.Warning, "Le bac d'une imprimante est mal configuré.").OpenDialog ();
				}
				else
				{
					this.PrintDialog = new PrintDialog (this, this.BvWidget, printers);
					this.PrintDialog.Show ();
					this.Window.IsFrozen = true;
				}
			}
			else
			{
				this.PrintDialog.Dispose ();
				this.PrintDialog = null;
				this.Window.IsFrozen = false;
			}
		}

	}	

}
