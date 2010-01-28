//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Marc BETTEX

using Epsitec.Common.Drawing;

using System;
using System.Globalization;
using System.Xml.Linq;

namespace Epsitec.App.BanquePiguet
{

	class BvrFieldMultiLineColumn : BvrFieldMultiLine
	{

		public BvrFieldMultiLineColumn(XElement xBvrField, Size bvrSize) : base (xBvrField, bvrSize)
		{
			this.HorizontalSpace = Double.Parse (xBvrField.Element ("horizontalSpace").Value, CultureInfo.InvariantCulture) / bvrSize.Width;
		}

		public double HorizontalSpace
		{
			get;
			set;
		}

		protected override void PaintImplementation(IPaintPort port, Rectangle bounds)
		{
			port.Color = Color.FromRgb (0, 0, 0);

			double xPositionBase = this.ComputeAbsoluteXPosition (bounds);
			double yPositionBase = this.ComputeAbsoluteYPosition (bounds);
			double textHeight = this.ComputeTextAbsoluteHeight (bounds);

			string[] ligns = this.Text.Split ('\n');

			for (int i = 0; i < ligns.Length; i++)
			{
				double yPosition = yPositionBase + this.ComputeVerticalOffset (i, bounds);

				char[] lign = ligns[i].ToCharArray ();
				
				for (int j = 0; j < lign.Length; j++)
				{
					double xPosition = xPositionBase + this.ComputeHorizontalOffset (j, bounds);
					string text = String.Format ("{0}", lign[j]);

					port.PaintText (xPosition, yPosition, text, this.TextFont, textHeight);
				}
			}
		}

		private double ComputeHorizontalOffset(int charNumber, Rectangle bounds)
		{
			return charNumber * this.HorizontalSpace * bounds.Width;
		}

	}

}
