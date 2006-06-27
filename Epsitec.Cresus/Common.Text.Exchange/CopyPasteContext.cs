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
	class CopyPasteContext
	{
		public CopyPasteContext(TextStory story)
		{
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


		private Wrappers.TextWrapper textWrapper;
		private Wrappers.ParagraphWrapper paraWrapper;
		private TextNavigator navigator;
		private TextStory story;
	}
}
