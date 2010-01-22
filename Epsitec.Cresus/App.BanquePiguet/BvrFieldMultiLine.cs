//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Marc BETTEX


using Epsitec.Common.Drawing;

using System.Xml;


namespace Epsitec.App.BanquePiguet
{

	class BvrFieldMultiLine : BvrField
	{

		public BvrFieldMultiLine(XmlNode xmlBvrField)
			: base (xmlBvrField)
		{
			this.VerticalSpace = double.Parse (xmlBvrField.SelectSingleNode ("verticalSpace").InnerText.Trim());
		}

		public double VerticalSpace
		{
			get;
			set;
		}

		public override void Paint(IPaintPort port, Rectangle bounds)
		{
			port.Color = Color.FromRgb (0, 0, 0);

			double xPosition = this.ComputeAbsoluteXPosition (this.XRelativePosition, bounds);
			double yPositionBase = this.ComputeAbsoluteYPosition (this.YRelativePosition, bounds);
			double textHeight = this.ComputeTextAbsoluteHeight (bounds);
			
			string[] ligns = this.Text.Split ('\n');

			for (int i = 0; i < ligns.Length; i++)
			{
				double yPosition = yPositionBase + this.ComputeVerticalOffset (i, bounds);
				port.PaintText (xPosition, yPosition, ligns[i], this.TextFont, textHeight);
			}
		}

		protected double ComputeVerticalOffset(int lignNumber, Rectangle bounds)
		{
			return -lignNumber * this.VerticalSpace * bounds.Height;
		}

	}

}
