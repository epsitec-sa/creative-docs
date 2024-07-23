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
    /// La classe Break stocke des informations sur un point de découpe dans la
    /// ligne.
    /// </summary>
    public class Break
    {
        public Break(
            int offset,
            double advance,
            double spacePenalty,
            double breakPenalty,
            StretchProfile profile
        )
        {
            this.offset = offset;
            this.advance = advance;
            this.spacePenalty = spacePenalty;
            this.breakPenalty = breakPenalty;
            this.profile = new StretchProfile(profile);
        }

        public int Offset
        {
            get { return this.offset; }
        }

        public double Advance
        {
            get { return this.advance; }
        }

        public double SpacePenalty
        {
            get { return this.spacePenalty; }
        }

        public double BreakPenalty
        {
            get { return this.breakPenalty; }
        }

        public StretchProfile Profile
        {
            get { return this.profile; }
        }

        private int offset;
        private double advance;
        private double spacePenalty;
        private double breakPenalty;
        private StretchProfile profile;
    }
}
