/*
This file is part of CreativeDocs.

Copyright © 2003-2024, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland

CreativeDocs is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
any later version.

CreativeDocs is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/

using System;

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
            story.DisableOpletQueue();
            opletQueueDisabled = true;
            TextNavigator navigator = new TextNavigator(story);
            this.InternalInitialize(story, navigator);
        }

        public CopyPasteContext(TextStory story, TextNavigator navigator)
        {
            this.InternalInitialize(story, navigator);
        }

        public Wrappers.TextWrapper TextWrapper
        {
            get { return this.textWrapper; }
        }

        public Wrappers.ParagraphWrapper ParaWrapper
        {
            get { return this.paraWrapper; }
        }

        public TextNavigator Navigator
        {
            get { return this.navigator; }
        }

        public TextStory Story
        {
            get { return this.story; }
        }

        ~CopyPasteContext()
        {
            this.Dispose(false);
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!isDisposed)
            {
                if (disposing)
                {
                    this.textWrapper.Detach();
                    this.paraWrapper.Detach();

                    if (opletQueueDisabled)
                    {
                        this.navigator.Dispose();
                        this.story.EnableOpletQueue();
                    }
                }
                else
                {
                    System.Diagnostics.Debug.Assert(
                        false,
                        "La classe CopyPasteContext a été utilisé sans using"
                    );
                }
            }
            this.isDisposed = true;
        }

        private void InternalInitialize(TextStory story, TextNavigator navigator)
        {
            this.textWrapper = new Wrappers.TextWrapper();
            this.paraWrapper = new Wrappers.ParagraphWrapper();

            this.textWrapper.Attach(navigator);
            this.paraWrapper.Attach(navigator);

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
