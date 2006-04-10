using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Document.Ribbons
{
	/// <summary>
	/// La classe RibbonContainer permet de réaliser des rubans horizontaux.
	/// </summary>
	public class RibbonContainer : Common.Widgets.RibbonContainer
	{
		public RibbonContainer()
		{
		}
		
		public RibbonContainer(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}


		public void NotifyChanged(string changed)
		{
			foreach ( Abstract ribbon in this.Children )
			{
				if ( ribbon == null )  continue;
				ribbon.NotifyChanged(changed);
			}
		}

		public void NotifyTextStylesChanged(System.Collections.ArrayList textStyleList)
		{
			foreach ( Abstract ribbon in this.Children )
			{
				if ( ribbon == null )  continue;
				ribbon.NotifyTextStylesChanged(textStyleList);
			}
		}

		public void NotifyTextStylesChanged()
		{
			foreach ( Abstract ribbon in this.Children )
			{
				if ( ribbon == null )  continue;
				ribbon.NotifyTextStylesChanged();
			}
		}

		public void SetDocument(DocumentType type, InstallType install, DebugMode debug, Settings.GlobalSettings gs, Document document)
		{
			foreach ( Abstract ribbon in this.Children )
			{
				if ( ribbon == null )  continue;
				ribbon.SetDocument(type, install, debug, gs, document);
			}
		}
	}
}
