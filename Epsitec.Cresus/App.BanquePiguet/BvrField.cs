//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Marc BETTEX

using Epsitec.Common.Drawing;

using System;
using System.IO;
using System.Reflection;
using System.Xml.Linq;

namespace Epsitec.App.BanquePiguet
{

	class BvrField
	{

		static BvrField()
		{
			using (Stream stream = Assembly.GetExecutingAssembly ().GetManifestResourceStream ("Epsitec.App.BanquePiguet.Resources.OCR_BB.tff"))
			{
				Font.RegisterDynamicFont (stream);
			}
		}
		
		public BvrField(XElement xBvrField, Size bvrSize)
		{
			this.Text = "";
			this.TextFont = Font.GetFont ("OCR-B Bold", "Regular");
			this.TextRelativeHeight = (double) xBvrField.Element ("textHeight") / bvrSize.Height;
			this.XRelativePosition = (double) xBvrField.Element ("xPosition") / bvrSize.Width;
			this.YRelativePosition = (double) xBvrField.Element ("yPosition") / bvrSize.Height;
			this.Valid = true;
		}
        
		public string Text
		{
			get;
			set;
		}

		public double TextRelativeHeight
		{
			get;
			set;
		}

		public Font TextFont
		{
			get;
			set;
		}


		public double XRelativePosition
		{
			get;
			set;
		}

		public double YRelativePosition
		{
			get;
			set;
		}

		public bool Valid
		{
			get;
			set;
		}

		public void Paint(IPaintPort port, Rectangle bounds)
		{
			if (this.Valid)
			{
				this.PaintImplementation (port, bounds);
			}
		}

		protected virtual void PaintImplementation(IPaintPort port, Rectangle bounds)
		{
			double xPosition = this.ComputeAbsoluteXPosition (bounds);
			double yPosition = this.ComputeAbsoluteYPosition (bounds);
			double textHeight = this.ComputeTextAbsoluteHeight (bounds);

			port.Color = Color.FromRgb (0, 0, 0);
			port.PaintText (xPosition, yPosition, this.Text, this.TextFont, textHeight);
		}

		protected double ComputeAbsoluteXPosition(Rectangle bounds)
		{
			return this.XRelativePosition * bounds.Width;
		}

		protected double ComputeAbsoluteYPosition(Rectangle bounds)
		{
			return this.YRelativePosition * bounds.Height;
		}

		protected double ComputeTextAbsoluteHeight(Rectangle bounds)
		{
			return this.TextRelativeHeight * bounds.Height;
		}

		public static BvrField GetInstance(XElement xBvrField, Size bvrSize)
		{
			string type = (string) xBvrField.Element ("type");

			BvrField bvrField;

			switch (type)
			{
				case "BvrField":
					bvrField = new BvrField (xBvrField, bvrSize);
					break;
				case "BvrFieldMultiLine":
					bvrField = new BvrFieldMultiLine (xBvrField, bvrSize);
					break;
				case "BvrFieldMultiLineColumn":
					bvrField = new BvrFieldMultiLineColumn (xBvrField, bvrSize);
					break;
				default:
					throw new Exception (String.Format ("Invalid bvrField type: {0}.", type));
			}

			return bvrField;

		}

	}

}
