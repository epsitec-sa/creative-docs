using System;
using System.Collections.Generic;
using System.Text;

// La classe CopyPasetContext définit un context nécessaire au copier/coller
// c-à-d:
//  - TextWrapper 
//  - ParagraphWrapper
//  - TextNavigator
//  - TextStory
//
// Responsable: Michael Walz


namespace Epsitec.Common.Text.Exchange
{
	class CopyPasteContext : IDisposable
	{
		public CopyPasteContext(TextStory story)
		{
			story.DisableOpletQueue ();
			opletQueueDisabled = true;
			TextNavigator navigator =  new TextNavigator (story);
			this.Initialize (story, navigator);
		}

		public CopyPasteContext(TextStory story, TextNavigator navigator)
		{
			this.Initialize (story, navigator);
		}

		private void Initialize(TextStory story, TextNavigator navigator)
		{
			textWrapper = new Wrappers.TextWrapper ();
			paraWrapper = new Wrappers.ParagraphWrapper ();

			textWrapper.Attach (navigator);
			paraWrapper.Attach (navigator);

			this.navigator = navigator;
			this.story = story;
		}


		public Wrappers.TextWrapper TextWrapper
		{
			get
			{
				return this.textWrapper;
			}
		}

		public Wrappers.ParagraphWrapper ParaWrapper
		{
			get
			{
				return this.paraWrapper;
			}
		}

		public TextNavigator Navigator
		{
			get
			{
				return this.navigator;
			}
		}

		public TextStory Story
		{
			get
			{
				return this.story;
			}
		}


		~CopyPasteContext()
		{
			System.Diagnostics.Debug.Assert (false, "La classe CopyPasteContext a été utilisé sans using");
			this.Dispose (false);
		}

		public void Dispose()
		{
			this.Dispose (true);
			GC.SuppressFinalize (this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!isDisposed)
			{
				if (disposing)
				{

				}

				if (opletQueueDisabled)
				{
					this.navigator.Dispose ();
					this.story.EnableOpletQueue ();
				}

			}
			this.isDisposed = true;
		}


		private Wrappers.TextWrapper textWrapper;
		private Wrappers.ParagraphWrapper paraWrapper;
		private TextNavigator navigator;
		private TextStory story;

		private bool opletQueueDisabled = false;
		private bool isDisposed = false;
	}
}
