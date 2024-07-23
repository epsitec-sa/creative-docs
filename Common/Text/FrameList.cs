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


namespace Epsitec.Common.Text
{
    /// <summary>
    /// La classe FrameList gère la liste des cadres utilisés pour couler le
    /// texte.
    /// </summary>
    public sealed class FrameList
    {
        public FrameList(TextFitter fitter)
        {
            this.fitter = fitter;
            this.frames = new System.Collections.ArrayList();
            this.cursorMap = new System.Collections.Hashtable();
        }

        public ITextFrame this[int index]
        {
            get { return this.frames[index] as ITextFrame; }
        }

        public int Count
        {
            get { return this.frames.Count; }
        }

        public TextFitter TextFitter
        {
            get { return this.fitter; }
        }

        public IPageCollection PageCollection
        {
            get { return this.fitter.PageCollection; }
        }

        public void ClearCursorMap()
        {
            this.cursorMap.Clear();
        }

        public int IndexOf(ITextFrame frame)
        {
            return this.frames.IndexOf(frame);
        }

        public Cursors.FitterCursor FindLastCursor(int index)
        {
            //	Recherche le dernier curseur contenu dans le texte qui décrit
            //	un paragraphe occupant au moins partiellement le ITextFrame
            //	à trouver :

            TextStory story = this.TextFitter.TextStory;
            Internal.TextTable text = story.TextTable;
            int length = story.TextLength;
            CursorInfo.Filter filter = Cursors.FitterCursor.GetFrameFilter(index);
            CursorInfo[] infos = text.FindCursors(0, length, filter, false);

            if (index != -1)
            {
                Debug.Assert.IsInBounds(index, 0, this.frames.Count - 1);
            }

            if (infos.Length > 0)
            {
                return text.GetCursorInstance(infos[infos.Length - 1].CursorId)
                    as Cursors.FitterCursor;
            }
            else
            {
                return null;
            }
        }

        public Cursors.FitterCursor FindFirstCursor(int index)
        {
            //	Trouve le curseur correspondant au paragraphe qui se trouve au
            //	début du ITextFrame.

            //	Note: un paragraphe peut couvrir plusieurs ITextFrame, ce qui
            //	complique légèrement les choses.

            ITextFrame frame = this[index];

            if (this.cursorMap.Contains(frame))
            {
                return this.cursorMap[frame] as Cursors.FitterCursor;
            }

            //	Recherche le premier curseur contenu dans le texte qui décrit
            //	un paragraphe occupant au moins partiellement le ITextFrame
            //	à trouver :

            TextStory story = this.TextFitter.TextStory;
            Internal.TextTable text = story.TextTable;
            int length = story.TextLength;
            CursorInfo.Filter filter = Cursors.FitterCursor.GetFrameFilter(index);
            CursorInfo[] infos = text.FindCursors(0, length, filter, true);

            Debug.Assert.IsInBounds(index, 0, this.frames.Count - 1);

            if (infos.Length == 1)
            {
                Cursors.FitterCursor cursor =
                    text.GetCursorInstance(infos[0].CursorId) as Cursors.FitterCursor;

                this.cursorMap[frame] = cursor;
                return cursor;
            }

            return null;
        }

        public Cursors.FitterCursor FindFirstCursor(ITextFrame frame)
        {
            if (this.cursorMap.Contains(frame))
            {
                return this.cursorMap[frame] as Cursors.FitterCursor;
            }

            return this.FindFirstCursor(this.IndexOf(frame));
        }

        public void Add(ITextFrame frame)
        {
            this.InsertAt(this.frames.Count, frame);
        }

        public void InsertAt(int index, ITextFrame newFrame)
        {
            this.frames.Insert(index, newFrame);
            this.HandleInsertion(newFrame);
        }

        public void InsertBefore(ITextFrame existingFrame, ITextFrame newFrame)
        {
            Debug.Assert.IsFalse(this.frames.Contains(newFrame));
            Debug.Assert.IsTrue(this.frames.Contains(existingFrame));

            this.frames.Insert(this.frames.IndexOf(existingFrame) + 0, newFrame);
            this.HandleInsertion(newFrame);
        }

        public void InsertAfter(ITextFrame existingFrame, ITextFrame newFrame)
        {
            Debug.Assert.IsFalse(this.frames.Contains(newFrame));
            Debug.Assert.IsTrue(this.frames.Contains(existingFrame));

            this.frames.Insert(this.frames.IndexOf(existingFrame) + 1, newFrame);
            this.HandleInsertion(newFrame);
        }

        public void Remove(ITextFrame frame)
        {
            this.frames.Remove(frame);
            this.HandleRemoval(frame);
        }

        public void Reset(System.Collections.ICollection frames)
        {
            this.frames.Clear();
            this.frames.AddRange(frames);
            this.ClearCursorMap();
        }

        private void HandleInsertion(ITextFrame frame)
        {
            this.ClearCursorMap();
        }

        private void HandleRemoval(ITextFrame frame)
        {
            this.ClearCursorMap();
        }

        private TextFitter fitter;
        private System.Collections.ArrayList frames;
        private System.Collections.Hashtable cursorMap; //	cache: ITextFrame --> FitterCursor
    }
}
