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
// Responsable: Michjael Walz


namespace Epsitec.Common.Text.Exchange
{
	class CopyPasteContect
	{
		public CopyPasteContect(TextStory story)
		{
			this.navigator =  new TextNavigator (story);
			this.Initialize (story, this.navigator);
		}

		public CopyPasteContect(TextStory story, TextNavigator navigator)
		{
			this.navigator =  navigator;
			this.Initialize (story, this.navigator);
		}

		private void Initialize(TextStory story, TextNavigator navigator)
		{
			this.navigator =  new TextNavigator (story);

			textWrapper = new Wrappers.TextWrapper ();
			paraWrapper = new Wrappers.ParagraphWrapper ();

			textWrapper.Attach (navigator);
			paraWrapper.Attach (navigator);

			this.textWrapper = textWrapper;
			this.paraWrapper = paraWrapper;
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
