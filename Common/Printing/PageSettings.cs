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


namespace Epsitec.Common.Printing
{
    /// <summary>
    /// La classe PageSettings décrit une page (dimensions imprimables, marges, taille
    /// et type de papier, orientation, source du papier, etc.)
    /// </summary>
    public class PageSettings
    {
        internal PageSettings(System.Drawing.Printing.PageSettings ps)
        {
            this.ps = ps;
        }

        public static PageSettings FromObject(object o)
        {
            System.Drawing.Printing.PageSettings ps = o as System.Drawing.Printing.PageSettings;

            if (ps != null)
            {
                return new PageSettings(ps);
            }

            return null;
        }

        public Drawing.Rectangle Bounds
        {
            get
            {
                double x = this.ps.Bounds.Left * PageSettings.Millimeters;
                double y = this.ps.Bounds.Top * PageSettings.Millimeters;
                double dx = this.ps.Bounds.Width * PageSettings.Millimeters;
                double dy = this.ps.Bounds.Height * PageSettings.Millimeters;

                return new Drawing.Rectangle(x, y, dx, dy);
            }
        }

        public Drawing.Margins Margins
        {
            get
            {
                double left = this.ps.Margins.Left * PageSettings.Millimeters;
                double right = this.ps.Margins.Right * PageSettings.Millimeters;
                double top = this.ps.Margins.Top * PageSettings.Millimeters;
                double bottom = this.ps.Margins.Bottom * PageSettings.Millimeters;

                return new Drawing.Margins(left, right, top, bottom);
            }
            set
            {
                System.Drawing.Printing.Margins margins = new System.Drawing.Printing.Margins();

                margins.Left = (int)(value.Left / PageSettings.Millimeters);
                margins.Right = (int)(value.Right / PageSettings.Millimeters);
                margins.Top = (int)(value.Top / PageSettings.Millimeters);
                margins.Bottom = (int)(value.Bottom / PageSettings.Millimeters);

                this.ps.Margins = margins;
            }
        }

        public PaperSize PaperSize
        {
            get { return new PaperSize(this.ps.PaperSize); }
            set { this.ps.PaperSize = value.GetPaperSize(); }
        }

        public bool Landscape
        {
            get { return this.ps.Landscape; }
            set { this.ps.Landscape = value; }
        }

        public PaperSource PaperSource
        {
            get { return new PaperSource(this.ps.PaperSource); }
            set
            {
                if (value != null)
                {
                    this.ps.PaperSource = value.GetPaperSource();
                }
            }
        }

        public PrinterResolution PrinterResolution
        {
            get { return new PrinterResolution(this.ps.PrinterResolution); }
        }

        public PrinterSettings PrinterSettings
        {
            get { return new PrinterSettings(this.ps.PrinterSettings); }
        }

        public System.IntPtr GetDevMode()
        {
            return this.ps.PrinterSettings.GetHdevmode(this.ps);
        }

        public void SetDevMode(System.IntPtr devMode)
        {
            this.ps.PrinterSettings.SetHdevmode(devMode);
        }

        private const double Millimeters = 25.4 / 100;
        System.Drawing.Printing.PageSettings ps;
    }
}
