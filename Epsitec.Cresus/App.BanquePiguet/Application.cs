//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Marc BETTEX

using Epsitec.Common.Dialogs;
using Epsitec.Common.Drawing;
using Epsitec.Common.Printing;
using Epsitec.Common.Support;
using Epsitec.Common.Widgets;
using Epsitec.Common.Widgets.Validators;

using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
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
			this.SetupBvrWidget ();
			this.SetupEvents ();
			this.SetupValidators ();
			this.SetupPrintersManager ();
			this.CheckPrintButtonEnbled ();
			this.Window.AdjustWindowSize ();

			this.BenefeciaryIbanTextField.Text = "CH00 0000 0000 0000 0000 0";
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

		protected BvrWidget BvrWidget
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

		protected void SetupBvrWidget()
		{
			this.BvrWidget = new BvrWidget ()
			{
				Dock = DockStyle.Left,
				Margins = new Margins (10, 10, 10, 10),
				Parent = this.Window.Root,
				PreferredSize = new Size (525, 265),
			};

			try
			{
				XElement xBvrValues = XElement.Parse (App.BanquePiguet.Properties.Resources.BvrValues);
				
				int nbValues = 0;

				foreach (XElement value in xBvrValues.Elements("value"))
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
							this.BvrWidget.BankAddress = text;
							break;
						case "BankAccount":
							this.BvrWidget.BankAccount = text;
							break;
						case "LayoutCode":
							this.BvrWidget.LayoutCode = text;
							break;
						case "ReferenceClientNumber":
							this.BvrWidget.ReferenceClientNumber = text;
							break;
						case "ClearingConstant":
							this.BvrWidget.ClearingConstant = text;
							break;
						case "ClearingBank":
							this.BvrWidget.ClearingBank = text;
							break;
						case "ClearingBankKey":
							this.BvrWidget.ClearingBankKey = text;
							break;
						case "CcpNumber":
							this.BvrWidget.CcpNumber = text;
							break;
						default:
							throw new Exception (String.Format ("Unknown value: {0}", name));
					}

					nbValues++;
				}

				if (nbValues < 8)
				{
					throw new Exception ("Some bvr values are missing.");
				}


			}
			catch (Exception e)
			{
				throw new Exception ("An error occured while loading the bvr values.", e);
			}
		}

		protected void SetupEvents()
		{
			this.BenefeciaryIbanTextField.TextChanged += (sender) =>
			{
				this.BvrWidget.BeneficiaryIban = this.BenefeciaryIbanTextField.Text;
				this.CheckPrintButtonEnbled ();
			};

			this.BeneficiaryAddressTextField.TextChanged += (sender) =>
			{
				string text = this.BeneficiaryAddressTextField.Text.Replace ("<br/>", "\n");
				this.BvrWidget.BeneficiaryAddress = text;
				this.CheckPrintButtonEnbled ();
			};

			this.ReasonTextField.TextChanged += (sender) =>
			{
				string text = this.ReasonTextField.Text.Replace ("<br/>", "\n");
				this.BvrWidget.Reason = text;
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
			List<IValidator> validators = new List<IValidator> ();

			validators.Add(new PredicateValidator (
				this.BenefeciaryIbanTextField,
				() => this.BvrWidget.IsBeneficiaryIbanValid ()
			));

			validators.Add(new PredicateValidator (
				this.BeneficiaryAddressTextField,
				() => this.BvrWidget.IsBeneficiaryAddressValid ()
			));

			validators.Add(new PredicateValidator (
				this.ReasonTextField,
				() => this.BvrWidget.IsReasonValid ()
			));

			validators.ForEach (validator => validator.Validate());
		}

		protected void SetupPrintersManager()
		{
			this.PrintersManager = new PrintersManager (this);
		}


		protected void CheckPrintButtonEnbled()
		{
			this.SetEnable (ApplicationCommands.Print, this.BvrWidget.IsValid ());
		}

		public void DisplayPrintersManager(bool display)
		{
			if (display)
			{
				this.PrintersManager.Show ();
				this.Window.IsFrozen = true;
			}
			else
			{
				this.PrintersManager.Hide ();
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
				PrintPort.PrintSinglePage (painter => this.BvrWidget.Print (painter, new Rectangle (0, 0, 21.0, 10.6)), dialog.Document, 25, 25);
				this.LogPrintCommand (0);
			}
		}

		protected void LogPrintCommand(int number)
		{
			DateTime date = DateTime.Now;
			string iban = this.BvrWidget.BeneficiaryIban;
			string address = this.BvrWidget.BeneficiaryAddress;
			string reason = this.BvrWidget.Reason;

			using (StreamWriter streamWriter = File.AppendText(App.BanquePiguet.Properties.Resources.LogFile))
			{
				streamWriter.WriteLine ("========= Entry =========");
				streamWriter.WriteLine (String.Format ("Date: {0}", date));
				streamWriter.WriteLine (String.Format ("Number: {0}", number));
				streamWriter.WriteLine (String.Format ("Beneficiary iban: {0}", iban));
				streamWriter.WriteLine (String.Format ("Beneficiary address: {0}", address.Replace ('\n', ' ')));
				streamWriter.WriteLine (String.Format ("Reason: {0}", reason.Replace ('\n', ' ')));
			}
			
		}

	}	

}
