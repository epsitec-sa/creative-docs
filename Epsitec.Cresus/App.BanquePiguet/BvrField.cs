//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Marc BETTEX


using Epsitec.Common.Drawing;

using System.IO;
using System.Reflection;
using System.Xml;

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
		
		public BvrField(BvrWidget parent, XmlNode xmlBvrField)
		{
			this.Text = "";
			this.TextRelativeHeight = double.Parse (xmlBvrField.SelectSingleNode ("textHeight").InnerText.Trim()) / parent.BvrSize.Height;
			this.TextFont = Font.GetFont ("OCR-B Bold", "Regular");
			this.XRelativePosition = double.Parse (xmlBvrField.SelectSingleNode ("xPosition").InnerText.Trim()) / parent.BvrSize.Width;
			this.YRelativePosition = double.Parse (xmlBvrField.SelectSingleNode ("yPosition").InnerText.Trim()) / parent.BvrSize.Height;
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

		public virtual void Paint(IPaintPort port, Rectangle bounds)
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

	}

}
