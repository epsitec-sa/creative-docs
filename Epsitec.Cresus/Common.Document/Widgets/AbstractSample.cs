using Epsitec.Common.Widgets;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Document.Widgets
{
	/// <summary>
	/// La classe AbstractSample est un widget affichant un �chantillon quelconque.
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
			//	Document associ�.
			get
			{
				return this.document;
			}

			set
			{
				this.document = value;
			}
		}

		public bool IsDeep
		{
			get
			{
				return this.isDeep;
			}
			
			set
			{
				this.isDeep = value;
			}
		}


		protected Document						document;
		protected bool							isDeep;
	}
}
