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
			this.sample.IsSampleAbc = true;
			this.sample.IsCenter = true;
			this.sample.Dock = DockStyle.Fill;
			this.sample.Margins = new Margins(0, 0, 3, 2);
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

		
		protected override void OnActiveStateChanged()
		{
			base.OnActiveStateChanged();
			this.sample.ActiveState = this.ActiveState;
		}


		protected FontSample				sample;
	}
}
