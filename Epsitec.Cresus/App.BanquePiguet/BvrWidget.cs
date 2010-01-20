//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Marc BETTEX


using Epsitec.Common.Widgets;
using Epsitec.Common.Drawing;

using System.Collections.Generic;
using System.Linq;


namespace Epsitec.App.BanquePiguet
{

	class BvrWidget : Widget
	{

		public string BeneficiaryIban { get; set; }


		public string BeneficiaryAddress { get; set; }


		public string Reason { get; set; }
		
		
		public string BankAddress { get; set; }
		
		
		public string BankAccount { get; set; }


		public string LayoutCode { get; set; }


		public string CodeLine1 { get; set; }


		public string CodeLine2 { get; set; }


		private string CcpNumper { get; set; }


		protected override void PaintBackgroundImplementation(Graphics graphics, Rectangle clipRect)
		{
			this.paintBackground (graphics);
			this.paintBankAddress (graphics);
			this.paintBeneficiaryIban (graphics);
			this.paintBeneficiaryAccount (graphics);
			this.paintReason (graphics);
			this.paintLayoutCode (graphics);
			this.paintCodeLine1 (graphics);
			this.paintCodeLine2 (graphics);
			this.paintCcpNumber (graphics);
		}


		private void paintBackground(Graphics graphics)
		{
			Rectangle bounds = this.Client.Bounds;

			using (Path path = Path.FromRectangle (bounds))
			{
				graphics.Color = Color.FromRgb (1.0, 0.8, 0.8);
				graphics.PaintSurface (path);
			}

			using (Path path = Path.FromRectangle (Rectangle.Deflate (bounds, 0.5, 0.5)))
			{
				graphics.Color = Color.FromRgb (1, 0, 0);
				graphics.LineWidth = 1;
				graphics.LineJoin = JoinStyle.Miter;
				graphics.PaintOutline (path);
			}
		}


		private void paintBankAddress(Graphics graphics)
		{
			graphics.PaintText(10, 10, "coucou", Font.DefaultFont, 10);
		}


		private void paintBeneficiaryIban(Graphics graphics)
		{
		}


		private void paintBeneficiaryAccount(Graphics graphics)
		{
		}


		private void paintReason(Graphics graphics)
		{
		}


		private void paintLayoutCode(Graphics graphics)
		{
		}


		private void paintCodeLine1(Graphics graphics)
		{
		}


		private void paintCodeLine2(Graphics graphics)
		{
		}


		private void paintCcpNumber(Graphics graphics)
		{
		}


	}


}
