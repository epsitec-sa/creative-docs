//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Marc BETTEX

using Epsitec.Common.Dialogs;
using Epsitec.Common.Drawing;
using Epsitec.Common.Printing;
using Epsitec.Common.Support;
using Epsitec.Common.Types;
using Epsitec.Common.Widgets;
using Epsitec.Common.Widgets.Validators;

using System;
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

			this.DisplayPrintersManager (true);
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
				CommandObject = ApplicationCommands.Print,
				Dock = DockStyle.StackFill,
				Margins = new Margins (10, 10, 0, 10),
				Parent = buttonsFrameBox,
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
						text = lines.Aggregate ((a, b) => String.Format ("{0}\n{1}", a, b));
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
							throw new Exception (String.Format ("Unknown value: {0}", name));
					}

					nbValues++;
				}

				if (nbValues < 8)
				{
					throw new Exception ("Some bv values are missing.");
				}

			
			}
			catch (Exception e)
			{
				Tools.Error(new Exception ("An error occured while loading the bv values.", e));
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
				this.OptionsButton.Clicked += (sender, e) =>
				{
					this.DisplayPrintersManager (true);
				};
			}
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

			this.SetEnable (ApplicationCommands.Print, check);
		}

		public void DisplayPrintersManager(bool display)
		{
			if (display)
			{
				if (this.PrintersManager == null)
				{
					this.PrintersManager = new PrintersManager (this);

				}

				this.PrintersManager.Show ();
				this.Window.IsFrozen = true;
			}
			else
			{
				if (this.PrintersManager != null)
				{
					this.PrintersManager.Dispose ();
					this.PrintersManager = null;
				}
				this.Window.IsFrozen = false;
			}
		}


		[Command (ApplicationCommands.Id.Print)]
		protected void ExecuteCommandPrint()
		{

			PrintDialog dialog = new PrintDialog
			{
				Owner = this.Window,
				AllowFromPageToPage = false,
				AllowSelectedPages = false,
			};

			dialog.Document.PrinterSettings.FromPage = 1;
			dialog.Document.PrinterSettings.ToPage = 1;
			dialog.OpenDialog ();

			if (dialog.Result == DialogResult.Accept)
			{
				PrintPort.PrintSinglePage (painter => this.BvWidget.Print (painter, new Rectangle (0, 0, 21.0, 10.6)), dialog.Document, 25, 25);
				this.LogPrintCommand (0);
			}
		}

		protected void LogPrintCommand(int nbPrints)
		{
			string entry = String.Format ("{0}\n{1}\n{2}\n{3}\n{4}",
				"========= New entry =========",
				String.Format ("Number of page printed: {0}", nbPrints),
				String.Format ("Beneficiary iban: {0}", this.BvWidget.BeneficiaryIban),
				String.Format ("Beneficiary address: {0}", this.BvWidget.BeneficiaryAddress.Replace ("\n", "\t\t")),
				String.Format ("Reason: {0}", this.BvWidget.Reason.Replace ("\n", "\t\t"))
			);

			Tools.LogMessage (entry);
		}

	}	

}
