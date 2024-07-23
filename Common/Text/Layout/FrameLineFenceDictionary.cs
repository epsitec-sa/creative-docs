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


namespace Epsitec.Common.Text.Layout
{
    /// <summary>
    /// La classe FrameLineFenceDictionary permet d'associer des lignes "barrières"
    /// pour les divers frames d'un texte.
    /// </summary>
    public class FrameLineFenceDictionary
    {
        public FrameLineFenceDictionary() { }

        public int this[int frame]
        {
            get
            {
                if (this.frames == null)
                {
                    return -1;
                }
                for (int i = 0; i < this.frames.Length; i++)
                {
                    if (this.frames[i] == frame)
                    {
                        return this.fences[i];
                    }
                }
                return -1;
            }
        }

        public int Count
        {
            get
            {
                if (this.frames == null)
                {
                    return 0;
                }
                else
                {
                    return this.frames.Length;
                }
            }
        }

        public void Clear()
        {
            this.frames = null;
            this.fences = null;
        }

        public void Add(int frame, int fence)
        {
            if (frame > 0)
            {
                if (this[frame - 1] == -1)
                {
                    this.Add(frame - 1, fence);
                }
            }

            if (this.frames == null)
            {
                this.frames = new int[1];
                this.fences = new int[1];

                this.frames[0] = frame;
                this.fences[0] = fence;

                return;
            }

            for (int i = 0; i < this.frames.Length; i++)
            {
                if (this.frames[i] == frame)
                {
                    this.frames[i] = frame;
                    this.fences[i] = System.Math.Min(this.fences[i], fence);
                    return;
                }
                if (this.frames[i] < frame)
                {
                    if (this.fences[i] > fence)
                    {
                        this.fences[i] = fence;
                    }
                }
            }

            int[] tempFrames = new int[this.frames.Length + 1];
            int[] tempFences = new int[this.fences.Length + 1];

            System.Array.Copy(this.frames, 0, tempFrames, 0, this.frames.Length);
            System.Array.Copy(this.fences, 0, tempFences, 0, this.fences.Length);

            tempFrames[this.frames.Length] = frame;
            tempFences[this.fences.Length] = fence;

            this.frames = tempFrames;
            this.fences = tempFences;
        }

        public void Add(FrameLineFenceDictionary dictionary)
        {
            if (dictionary.frames == null)
            {
                return;
            }

            if ((this.frames == null) || (this.frames.Length == 0))
            {
                this.frames = new int[dictionary.frames.Length];
                this.fences = new int[dictionary.fences.Length];

                System.Array.Copy(dictionary.frames, 0, this.frames, 0, this.frames.Length);
                System.Array.Copy(dictionary.fences, 0, this.fences, 0, this.fences.Length);
            }
            else
            {
                for (int i = 0; i < dictionary.frames.Length; i++)
                {
                    this.Add(dictionary.frames[i], dictionary.fences[i]);
                }
            }
        }

        private int[] frames;
        private int[] fences;
    }
}
