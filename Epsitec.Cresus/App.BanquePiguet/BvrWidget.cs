//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Marc BETTEX


using Epsitec.Common.Widgets;
using Epsitec.Common.Drawing;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;


namespace Epsitec.App.BanquePiguet
{

	class BvrWidget : Widget
	{

		protected enum BvrFieldId {
			BankAddress1,
			BankAddress2,
			BeneficiaryIban1,
			BeneficiaryIban2,
			BeneficiaryAddress1,
			BeneficiaryAddress2,
			BankAccount1,
			BankAccount2,
			LayoutCode,
			Reason,
			ReferenceLine,
			ClearingLine,
			CcpNumber
		}

		public BvrWidget()
		{

			this.BvrFields = new Dictionary<BvrFieldId, BvrField> ();
			this.BvrFieldsDisplay = new Dictionary<BvrFieldId, bool> ();

			try
			{
				XmlDocument bvrDefinitionXml = new XmlDocument ();
				bvrDefinitionXml.LoadXml (App.BanquePiguet.Properties.Resources.BvrDefinition);

				XmlNode bvrSize = bvrDefinitionXml.GetElementsByTagName ("size").Item(0);
				double bvrHeight = double.Parse(bvrSize.SelectSingleNode("height").InnerText.Trim());
				double bvrWidth = double.Parse(bvrSize.SelectSingleNode("width").InnerText.Trim());

				this.BvrSize = new Size(bvrWidth, bvrHeight);

				XmlNodeList bvrFields = bvrDefinitionXml.GetElementsByTagName ("bvrField");

				foreach (XmlNode xmlBvrField in bvrFields)
				{
					this.SetupBvrField (xmlBvrField);
				}

			}
			catch (Exception e)
			{
				throw new Exception ("An error occured while loading the bvr fields.", e);
			}

			if (this.BvrFields.Count < Enum.GetValues (typeof (BvrFieldId)).Length)
			{
				throw new Exception (
					"An error occured while loading the bvr fields." ,
					new Exception("Some bvr fields are missing.")
				);
			}

		}

		public string BankAddress
		{
			get
			{
				return this.BvrFields[BvrFieldId.BankAddress1].Text;
			}
			set
			{
				this.BvrFields[BvrFieldId.BankAddress1].Text = value;
				this.BvrFields[BvrFieldId.BankAddress2].Text = value;
				this.Invalidate ();
			}
		}
		
		public string BeneficiaryIban
		{
			get
			{
				return this.BvrFields[BvrFieldId.BeneficiaryIban1].Text;
			}
			set
			{
				string iban = Regex.Replace(value, @"\s", "");

				for (int i = 4; i < iban.Length && i < 27; i += 5)
				{
					iban = iban.Insert (i, " ");
				}

				this.BvrFields[BvrFieldId.BeneficiaryIban1].Text = iban;
				this.BvrFields[BvrFieldId.BeneficiaryIban2].Text = iban;
				
				bool valid = BvrWidget.IsBeneficiaryIbanValid(iban);
				this.BvrFieldsDisplay[BvrFieldId.BeneficiaryIban1] = valid;
				this.BvrFieldsDisplay[BvrFieldId.BeneficiaryIban2] = valid;

				this.UpdateReferenceLine ();
				this.Invalidate ();
			}
		}

		public string BeneficiaryAddress
		{
			get
			{
				return this.BvrFields[BvrFieldId.BeneficiaryAddress1].Text;
			}
			set
			{
				this.BvrFields[BvrFieldId.BeneficiaryAddress1].Text = value;
				this.BvrFields[BvrFieldId.BeneficiaryAddress2].Text = value;

				bool valid = BvrWidget.IsbeneficiaryAddressValid (value);
				this.BvrFieldsDisplay[BvrFieldId.BeneficiaryAddress1] = valid;
				this.BvrFieldsDisplay[BvrFieldId.BeneficiaryAddress2] = valid;

				this.Invalidate ();
			}
		}

		public string BankAccount
		{
			get
			{
				return this.BvrFields[BvrFieldId.BankAccount1].Text;
			}
			set
			{
				this.BvrFields[BvrFieldId.BankAccount1].Text = value;
				this.BvrFields[BvrFieldId.BankAccount2].Text = value;
				this.Invalidate ();
			}
		}

		public string LayoutCode
		{
			get
			{
				return this.BvrFields[BvrFieldId.LayoutCode].Text;
			}
			set
			{
				this.BvrFields[BvrFieldId.LayoutCode].Text = value;
				this.Invalidate ();
			}
		}

		public string Reason
		{
			get
			{
				return this.BvrFields[BvrFieldId.Reason].Text;
			}
			set
			{
				this.BvrFields[BvrFieldId.Reason].Text = value;
				this.BvrFieldsDisplay[BvrFieldId.Reason] = BvrWidget.IsReasonValid (value);

				this.Invalidate ();
			}
		}

		public string ReferenceClientNumber
		{
			get
			{
				return this.referenceClientNumber;
			}
			set
			{
				this.referenceClientNumber = value;
				this.UpdateReferenceLine ();
			}
		}

		public string ClearingConstant
		{
			get
			{
				return this.clearingConstant;
			}
			set
			{
				this.clearingConstant = value;
				this.UpdateClearingLine ();
			}
		}

		public string ClearingBank
		{
			get
			{
				return this.clearingBank;
			}
			set
			{
				this.clearingBank = value;
				this.UpdateClearingLine ();
			}
		}

		public string ClearingBankKey
		{
			get
			{
				return this.clearingBankKey;
			}
			set
			{
				this.clearingBankKey = value;
				this.UpdateClearingLine ();
			}
		}


		public string CcpNumber
		{
			get
			{
				string text = this.BvrFields[BvrFieldId.CcpNumber].Text;
				return text.Substring (0, text.Length - 1);
			}
			set
			{
				this.BvrFields[BvrFieldId.CcpNumber].Text = String.Format ("{0}>", value);
				this.Invalidate ();
			}
		}

		public Size BvrSize
		{
			get;
			set;
		}

		protected Dictionary<BvrFieldId, BvrField> BvrFields
		{
			get;
			set;
		}

		protected Dictionary<BvrFieldId, bool> BvrFieldsDisplay
		{
			get;
			set;
		}

		public bool IsBeneficiaryIbanValid()
		{
			return BvrWidget.IsBeneficiaryIbanValid (this.BeneficiaryIban);
		}

		public bool IsBeneficiaryAddressValid()
		{
			return BvrWidget.IsbeneficiaryAddressValid (this.BeneficiaryAddress);
		}

		public bool IsReasonValid()
		{
			return BvrWidget.IsReasonValid (this.Reason);
		}

		public bool IsValid()
		{
			return this.IsBeneficiaryIbanValid () && this.IsBeneficiaryAddressValid () && this.IsReasonValid ();
		}

		public void Print(IPaintPort paintPort, Rectangle bounds)
		{
			this.PaintBvrWidgets (paintPort, bounds);
		}

		protected override void PaintBackgroundImplementation(Graphics graphics, Rectangle clipRect)
		{
			Rectangle bounds = this.Client.Bounds;

			this.PaintBackground (graphics, bounds);
			this.PaintBvrWidgets (graphics, bounds);
		}

		protected void PaintBackground(IPaintPort port, Rectangle bounds)
		{
			using (Path path = Path.FromRectangle (bounds))
			{
				port.Color = Color.FromRgb (1.0, 0.8, 0.8);
				port.PaintSurface (path);
			}
		}

		protected void PaintBvrWidgets(IPaintPort paintPort, Rectangle bounds)
		{

			foreach (BvrFieldId fieldId in this.BvrFields.Keys)
			{
				if (this.BvrFieldsDisplay[fieldId])
				{
					this.BvrFields[fieldId].Paint (paintPort, bounds);
				}
			}

		}

		protected void SetupBvrField(XmlNode xmlBvrField)
		{
			string name = xmlBvrField.SelectSingleNode ("name").InnerText.Trim();
			string type = xmlBvrField.SelectSingleNode ("type").InnerText.Trim();

			BvrFieldId id = (BvrFieldId) Enum.Parse (typeof (BvrFieldId), name);
			BvrField field;

			switch (type)
			{
				case "BvrField":
					field = new BvrField (this, xmlBvrField);
					break;
				case "BvrFieldMultiLine":
					field = new BvrFieldMultiLine (this, xmlBvrField);
					break;
				case "BvrFieldMultiLineColumn":
					field = new BvrFieldMultiLineColumn (this, xmlBvrField);
					break;
				default:
					throw new Exception (String.Format ("Invalid bvrField type: {0}.", type));
			}

			this.BvrFields[id] = field;
			this.BvrFieldsDisplay[id] = true;

		}

		protected void UpdateReferenceLine()
		{

			if (this.IsBeneficiaryIbanValid ())
			{
				string part0 = this.ReferenceClientNumber;
				string part1 = String.Format ("0000{0}", Regex.Replace (this.BeneficiaryIban, @"\s", "").Substring (9, 12));
				string part2 = this.ComputeReferenceKey ();

				this.BvrFields[BvrFieldId.ReferenceLine].Text = String.Format ("{0}{1}{2}+", part0, part1, part2);
				this.BvrFieldsDisplay[BvrFieldId.ReferenceLine] = true;
			}
			else
			{
				this.BvrFieldsDisplay[BvrFieldId.ReferenceLine] = false;
			}
			this.Invalidate ();
		}

		protected void UpdateClearingLine()
		{
			string part0 = this.ClearingConstant;
			string part1 = this.ClearingBank;
			string part2 = this.ClearingBankKey;
			string part3 = this.ComputeClearingKey ();

			this.BvrFields[BvrFieldId.ClearingLine].Text = String.Format ("{0}{1}{2}{3}>", part0, part1, part2, part3);
			this.Invalidate ();
		}

		protected string ComputeReferenceKey()
		{
			return "X";
		}

		protected string ComputeClearingKey()
		{
			return "X";
		}

		protected static bool IsBeneficiaryIbanValid(string iban)
		{
			return Regex.IsMatch (iban, @"^CH\d{2} \d{4} \d{4} \d{4} \d{4} \d{1}$");
		}

		protected static bool IsbeneficiaryAddressValid(string address)
		{
			string[] lines = address.Split ('\n');
			return (lines.Length <= 4) && lines.All (line => line.Length <= 27);
		}

		protected static bool IsReasonValid(string reason)
		{
			string[] lines = reason.Split ('\n');
			return (lines.Length <= 3) && lines.All (line => line.Length <= 10);
		}

		protected string referenceClientNumber;
		protected string clearingConstant;
		protected string clearingBank;
		protected string clearingBankKey;

	}

}
