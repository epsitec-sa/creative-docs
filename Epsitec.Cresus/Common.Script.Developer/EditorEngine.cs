//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using Epsitec.Common.Widgets;
using Epsitec.Common.Support;

namespace Epsitec.Common.Script.Developer
{
	/// <summary>
	/// Summary description for EditorEngine.
	/// </summary>
	public class EditorEngine : IEditorEngine
	{
		public EditorEngine()
		{
			Common.Widgets.Widget.Initialise ();
			Common.Pictogram.Engine.Initialise ();
		}
		
		
		public object CreateDocument(Source source)
		{
			return new Document (source);
		}
		
		public void ShowMethod(object document, string name)
		{
			Document doc = document as Document;
			
			doc.Window.Show ();
			doc.Controller.ShowMethod (name);
		}
		
		
		protected class Document
		{
			public Document(Source source)
			{
				this.controller = new EditionController ();
				this.window     = new Window ();
				
				this.window.Root.Text  = "[res:strings#label.ScriptEditor]";
				this.window.ClientSize = new Drawing.Size (600, 400);
				
				this.controller.Source = source;
				this.controller.CreateWidgets (this.window.Root);
			}
			
			
			public Window						Window
			{
				get
				{
					return this.window;
				}
			}
			
			public EditionController			Controller
			{
				get
				{
					return this.controller;
				}
			}
			
			
			private Window						window;
			private EditionController			controller;
		}
	}
}
