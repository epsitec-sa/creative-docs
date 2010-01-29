//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Marc BETTEX

using Epsitec.Common.Widgets;
using Epsitec.Common.Drawing;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;

namespace Epsitec.App.BanquePiguet
{

	class Bvr303Widget : Widget
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
			CcpNumberLine
		}

		public Bvr303Widget() : this (null)
		{
		}

		public Bvr303Widget(Widget embedder) : base (embedder)
		{
			this.BvrFields = new Dictionary<BvrFieldId, Bvr303Field> ();

			try
			{

				XElement xBvrDefinition;
				
				using (XmlReader xmlReader = XmlReader.Create (Tools.GetResourceStream("BvrDefinition.xml")))
				{
					xBvrDefinition = XElement.Load (xmlReader);	
				}

				XElement xSize = xBvrDefinition.Element ("size");
				this.BvrSize = new Size (
					Double.Parse (xSize.Element ("width").Value, CultureInfo.InvariantCulture),
					Double.Parse (xSize.Element ("height").Value, CultureInfo.InvariantCulture)
				);

				IEnumerable<XElement> xBvrfields = xBvrDefinition.Element ("fields").Elements ("field");

				foreach (XElement xBvrField in xBvrfields)
				{
					string name = (string) xBvrField.Element ("name");
					BvrFieldId id = (BvrFieldId) Enum.Parse (typeof (BvrFieldId), name);
					this.BvrFields[id] = Bvr303Field.GetInstance (xBvrField, this.BvrSize);
				}

			}
			catch (Exception e)
			{
				throw new Exception ("An error occured while loading the bvr fields.", e);
			}

			if (this.BvrFields.Count < Enum.GetValues (typeof (BvrFieldId)).Length)
			{
				throw new Exception ("An error occured while loading the bvr fields.", new Exception ("Some fields are missing."));
			}

			this.BackgroundImage = (Bitmap) Bitmap.FromManifestResource ("Epsitec.App.BanquePiguet.Resources.bvr.jpg", System.Reflection.Assembly.GetExecutingAssembly ());

			this.referenceClientNumber = "";
			this.clearingConstant = "";
			this.clearingBank = "";
			this.clearingBankKey = "";
			this.ccpNumber = "";
		}
        
		public string BankAddress
		{
			get
			{
				return this.BvrFields[BvrFieldId.BankAddress1].Text;
			}

			set
			{
				if (!Bvr303Helper.CheckBankAddress (value))
				{
					throw new ArgumentException (String.Format ("The provided value is not valid: {0}", value));
				}

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
				string iban = Bvr303Helper.BuildNormalizedIban (value);

				if (!Bvr303Helper.CheckBeneficiaryIban (iban))
				{
					throw new ArgumentException (String.Format ("The provided value is not valid: {0}", value));
				}

				this.BvrFields[BvrFieldId.BeneficiaryIban1].Text = iban;
				this.BvrFields[BvrFieldId.BeneficiaryIban2].Text = iban;

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
				if (!Bvr303Helper.CheckBeneficiaryAddress (value))
				{
					throw new ArgumentException (String.Format ("The provided value is not valid: {0}", value));
				}

				this.BvrFields[BvrFieldId.BeneficiaryAddress1].Text = value;
				this.BvrFields[BvrFieldId.BeneficiaryAddress2].Text = value;

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
				if (!Bvr303Helper.CheckBankAccount (value))
				{
					throw new ArgumentException (String.Format ("The provided value is not valid: {0}", value));
				}

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
				if (!Bvr303Helper.CheckLayoutCode (value))
				{
					throw new ArgumentException (String.Format ("The provided value is not valid: {0}", value));
				}

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
				if (!Bvr303Helper.CheckReason (value))
				{
					throw new ArgumentException (String.Format ("The provided value is not valid: {0}", value));
				}

				this.BvrFields[BvrFieldId.Reason].Text = value;
				
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
				if (!Bvr303Helper.CheckReferenceClientNumber (value))
				{
					throw new ArgumentException (String.Format ("The provided value is not valid: {0}", value));
				}

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
				if (!Bvr303Helper.CheckClearingConstant (value))
				{
					throw new ArgumentException (String.Format ("The provided value is not valid: {0}", value));
				}

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
				if (!Bvr303Helper.CheckClearingBank (value))
				{
					throw new ArgumentException (String.Format ("The provided value is not valid: {0}", value));
				}

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
				if (!Bvr303Helper.CheckClearingBankKey (value))
				{
					throw new ArgumentException (String.Format ("The provided value is not valid: {0}", value));
				}

				this.clearingBankKey = value;

				this.UpdateClearingLine ();
			}
		}


		public string CcpNumber
		{
			get
			{
				return this.ccpNumber;
			}
			set
			{
				if (!Bvr303Helper.CheckBeneficiaryAddress (value))
				{
					throw new ArgumentException (String.Format ("The provided value is not valid: {0}", value));
				}

				this.ccpNumber = value;

				this.UpdateCcpNumberLine ();
			}
		}

		public Size BvrSize
		{
			get;
			protected set;
		}

		protected Dictionary<BvrFieldId, Bvr303Field> BvrFields
		{
			get;
			set;
		}

		protected Bitmap BackgroundImage
		{
			get;
			set;
		}

		public void Print(IPaintPort paintPort, Rectangle bounds)
		{
			this.PaintBvr303Fields (paintPort, bounds);
		}

		protected override void PaintBackgroundImplementation(Graphics graphics, Rectangle clipRect)
		{
			Rectangle bounds = this.Client.Bounds;

			this.PaintBackgroundImage (graphics, bounds);
			this.PaintBvr303Fields (graphics, bounds);
		}

		protected void PaintBackgroundImage(IPaintPort port, Rectangle bounds)
		{
			port.PaintImage (this.BackgroundImage, bounds);

			using (Path path = Path.FromRectangle (bounds))
			{
				port.Color = Color.FromRgb (0, 0, 0);
				port.PaintOutline (path);
			}
		}

		protected void PaintBvr303Fields(IPaintPort paintPort, Rectangle bounds)
		{

			foreach (BvrFieldId fieldId in this.BvrFields.Keys)
			{
				this.BvrFields[fieldId].Paint (paintPort, bounds);
			}

		}

		protected void UpdateReferenceLine()
		{
			string iban = this.BeneficiaryIban;
			string reference = this.ReferenceClientNumber;

			if (Bvr303Helper.CheckReferenceLine (iban, reference))
			{
				this.BvrFields[BvrFieldId.ReferenceLine].Text = Bvr303Helper.BuildReferenceLine (iban, reference);
				this.Invalidate ();
			}
		}

		protected void UpdateClearingLine()
		{
			string constant = this.ClearingConstant;
			string clearing = this.ClearingBank;
			string key = this.ClearingBankKey;

			if (Bvr303Helper.CheckClearingLine(constant, clearing, key))
			{
				this.BvrFields[BvrFieldId.ClearingLine].Text = Bvr303Helper.BuildClearingLine (constant, clearing, key);
				this.Invalidate ();
			}
		}

		protected void UpdateCcpNumberLine()
		{
			string ccp = this.CcpNumber;

			if (Bvr303Helper.CheckCcpNumber (ccp))
			{
				this.BvrFields[BvrFieldId.CcpNumberLine].Text = Bvr303Helper.BuildCcpNumberLine (ccp);
				this.Invalidate ();
			}
		}
		

		protected string referenceClientNumber;
		protected string clearingConstant;
		protected string clearingBank;
		protected string clearingBankKey;
		protected string ccpNumber;

	}

}
