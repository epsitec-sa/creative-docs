using Epsitec.Common.Widgets;
using Epsitec.Common.Drawing;
using Epsitec.Common.OpenType;

namespace Epsitec.Common.Document.Widgets
{
	/// <summary>
	/// La classe FontWindow est une fenêtre qui permet de choisir une police.
	/// </summary>
	public class FontWindow
	{
		public FontWindow(Point pos, Size size, bool enableSymbols, System.Collections.ArrayList quickList, Window owner)
		{
			this.window = new Window();
			this.window.MakeFramelessWindow();
			this.window.MakeFloatingWindow();
			this.window.DisableMouseActivation();
			this.window.MakeLayeredWindow(true);
			this.window.Root.SetSyncPaint(true);
			this.window.WindowSize = size;
			this.window.WindowLocation = pos;
			this.window.Owner = owner;

			this.selector = new FontSelector(this.window.Root);
			this.selector.Anchor = AnchorStyles.All;
			this.selector.Build(enableSymbols, quickList);
			this.selector.SelectionChanged += new Epsitec.Common.Support.EventHandler(this.HandleSelectorClose);
			this.selector.CloseNeeded += new Epsitec.Common.Support.EventHandler(this.HandleSelectorClose);
			this.selector.Focus();
		}

		public string Action(string fontFace)
		{
			this.selector.SelectedFontFace = fontFace;
			this.window.ShowDialog();
			return this.selector.SelectedFontFace;
		}


		private void HandleSelectorClose(object sender)
		{
			this.window.Close();
		}


		protected Window								window;
		protected Common.Document.Widgets.FontSelector	selector;
	}
}
