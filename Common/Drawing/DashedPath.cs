//	Copyright © 2003-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Drawing
{
    /// <summary>
    /// The DashedPath class represents a ... dashed path.
    /// </summary>
    public class DashedPath : Path
    {
        public DashedPath() { }

        public double DashOffset
        {
            get { return this.start; }
            set
            {
                this.start = value;
                this.path.SetDashOffset(this.start);
            }
        }

        public void ResetDash()
        {
            this.path.ResetDash();
        }

        public void AddDash(double dashLength, double gapLength)
        {
            this.path.AddDash(dashLength, gapLength);
        }

        public Path GenerateDashedPath()
        {
            return this.GenerateDashedPath(this.defaultZoom);
        }

        public Path GenerateDashedPath(double approximationZoom)
        {
            if (this.IsEmpty)
            {
                return null;
            }

            AntigrainSharp.Path path = new AntigrainSharp.Path();

            //path.InternalCreateNonEmpty();

            path.AppendDashedPath(this.path, approximationZoom);

            return new Path(path);
        }

        private double start;
    }
}
