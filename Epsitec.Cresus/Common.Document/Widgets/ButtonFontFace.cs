using Epsitec.Common.Widgets;
using Epsitec.Common.Drawing;
using Epsitec.Common.Text;

namespace Epsitec.Common.Document.Widgets
{
	/// <summary>
	/// La classe ButtonFontFace est un IconButton contenant un échantillon de police FontSample.
	/// </summary>
	public class ButtonFontFace : IconButton
	{
		public ButtonFontFace()
		{
			this.sample = new FontSample(this);
			this.sample.SampleAbc = true;
			this.sample.Dock = DockStyle.Fill;
			this.sample.DockMargins = new Margins(0, 0, 3, 2);
		}

		public ButtonFontFace(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}

		
		public OpenType.FontIdentity FontIdentity
		{
			get
			{
				return this.sample.FontIdentity;
			}

			set
			{
				this.sample.FontIdentity = value;
			}
		}


		public override ActiveState ActiveState
		{
			get
			{
				return base.ActiveState;
			}

			set
			{
				base.ActiveState = value;
				this.sample.ActiveState = value;
			}
		}


		protected override void PaintBackgroundImplementation(Graphics graphics, Rectangle clipRect)
		{
			//?this.sample.ActiveState = this.ActiveState;
			base.PaintBackgroundImplementation(graphics, clipRect);
		}


		protected FontSample				sample;
	}
}
