using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;
using Epsitec.Common.Document;

namespace Epsitec.App.DocumentEditor.Dialogs
{
	using GlobalSettings = Common.Document.Settings.GlobalSettings;

	/// <summary>
	/// Classe de base.
	/// </summary>
	public abstract class Abstract
	{
		public Abstract(DocumentEditor editor)
		{
			this.editor = editor;
			this.globalSettings = editor.GlobalSettings;
			this.window = null;
		}

		// Crée et montre la fenêtre du dialogue.
		public virtual void Show()
		{
		}

		// Cache la fenêtre du dialogue.
		public virtual void Hide()
		{
			if ( this.window != null )
			{
				this.window.Hide();
			}
		}

		// Enregistre la position de la fenêtre du dialogue.
		public virtual void Save()
		{
		}

		// Reconstruit le dialogue.
		public virtual void Rebuild()
		{
		}


		// Donne les frontières de l'application.
		protected Rectangle CurrentBounds
		{
			get
			{
				if ( this.editor.Window == null )
				{
					return new Rectangle(this.globalSettings.WindowLocation, this.globalSettings.WindowSize);
				}
				else
				{
					return new Rectangle(this.editor.Window.WindowLocation, this.editor.Window.WindowSize);
				}
			}
		}


		protected DocumentEditor				editor;
		protected GlobalSettings				globalSettings;
		protected Window						window;
	}
}
