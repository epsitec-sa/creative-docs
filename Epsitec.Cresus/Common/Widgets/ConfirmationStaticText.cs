using Epsitec.Common.Types;

namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// La class ConfirmationStaticText représente un bouton pour le dialogue ConfirmationDialog.
	/// </summary>
	public class ConfirmationStaticText : StaticText
	{
		public ConfirmationStaticText()
		{
			this.ContentAlignment = Drawing.ContentAlignment.MiddleLeft;
		}
		
		public ConfirmationStaticText(string text)
		{
			this.Text = text;
		}
		
		public ConfirmationStaticText(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}
		
		
		protected override Drawing.Size GetTextLayoutSize()
		{
			Drawing.Size size = this.IsActualGeometryValid ? this.Client.Size : this.PreferredSize;
			size.Width -= ConfirmationStaticText.marginX*2;
			size.Height -= ConfirmationStaticText.marginY*2;
			return size;
		}

		protected override Drawing.Point GetTextLayoutOffset()
		{
			return new Drawing.Point(ConfirmationStaticText.marginX, ConfirmationStaticText.marginY);
		}

		protected override void OnSizeChanged(Drawing.Size oldValue, Drawing.Size newValue)
		{
			base.OnSizeChanged(oldValue, newValue);

			if (oldValue.Width != newValue.Width)  // largeur changée ?
			{
				double h = this.TextLayout.FindTextHeight();
				this.PreferredHeight = h+ConfirmationStaticText.marginY*2+2;  // TODO: pourquoi +2 ?
			}
		}

		protected static readonly double marginX = 0;
		protected static readonly double marginY = 0;
	}
}
