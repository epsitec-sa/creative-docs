using Epsitec.Common.Support;
using Epsitec.Common.Widgets;
using Epsitec.Common.Drawing;
using Epsitec.Common.Text;

namespace Epsitec.Common.Document
{
	/// <summary>
	/// Summary description for Wrappers.
	/// </summary>
	public class Wrappers
	{
		public Wrappers(Document document)
		{
			this.document = document;

			this.textWrapper = new Text.Wrappers.TextWrapper();
			this.paragraphLayoutWrapper = new Text.Wrappers.ParagraphWrapper();
		}

		// Wrapper pour la fonte.
		public Text.Wrappers.TextWrapper TextWrapper
		{
			get { return this.textWrapper; }
		}

		// Wrapper pour le paragraphe.
		public Text.Wrappers.ParagraphWrapper ParagraphWrapper
		{
			get { return this.paragraphLayoutWrapper; }
		}

		// Indique si les wrappers sont attachés.
		public bool IsWrappersAttached
		{
			get
			{
				return this.textWrapper.IsAttached;
			}
		}

		// Attache tous les wrappers à un texte.
		public void WrappersAttach(TextFlow textFlow)
		{
			this.textWrapper.Attach(textFlow.TextNavigator);
			this.paragraphLayoutWrapper.Attach(textFlow.TextNavigator);
		}

		// Détache tous les wrappers.
		public void WrappersDetach()
		{
			this.textWrapper.Detach();
			this.paragraphLayoutWrapper.Detach();
		}


		protected Document								document;
		protected Text.Wrappers.TextWrapper				textWrapper;
		protected Text.Wrappers.ParagraphWrapper		paragraphLayoutWrapper;
	}
}
