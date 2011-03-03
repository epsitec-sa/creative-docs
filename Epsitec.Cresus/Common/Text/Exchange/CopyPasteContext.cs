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
	public class CopyPasteContext : IDisposable
	{
		public CopyPasteContext(TextStory story)
		{
			story.DisableOpletQueue ();
			opletQueueDisabled = true;
			TextNavigator navigator =  new TextNavigator (story);
			this.InternalInitialize (story, navigator);
		}

		public CopyPasteContext(TextStory story, TextNavigator navigator)
		{
			this.InternalInitialize (story, navigator);
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
					this.textWrapper.Detach ();
					this.paraWrapper.Detach ();

					if (opletQueueDisabled)
					{
						this.navigator.Dispose ();
						this.story.EnableOpletQueue ();
					}
				}
				else
				{
					System.Diagnostics.Debug.Assert (false, "La classe CopyPasteContext a été utilisé sans using");
				}
			}
			this.isDisposed = true;
		}


		private void InternalInitialize(TextStory story, TextNavigator navigator)
		{
			this.textWrapper = new Wrappers.TextWrapper ();
			this.paraWrapper = new Wrappers.ParagraphWrapper ();

			this.textWrapper.Attach (navigator);
			this.paraWrapper.Attach (navigator);

			this.navigator = navigator;
			this.story = story;
		}


		private Wrappers.TextWrapper textWrapper;
		private Wrappers.ParagraphWrapper paraWrapper;
		private TextNavigator navigator;
		private TextStory story;

		private bool opletQueueDisabled = false;
		private bool isDisposed = false;
	}
}
