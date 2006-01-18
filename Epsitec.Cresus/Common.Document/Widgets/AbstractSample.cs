using Epsitec.Common.Widgets;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Document.Widgets
{
	/// <summary>
	/// La classe AbstractSample est un widget affichant un échantillon quelconque.
	/// </summary>
	public abstract class AbstractSample : Widget
	{
		public AbstractSample()
		{
		}

		public AbstractSample(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}


		public Document Document
		{
			//	Document associé.
			get
			{
				return this.document;
			}

			set
			{
				this.document = value;
			}
		}


		protected Document						document;
	}
}
