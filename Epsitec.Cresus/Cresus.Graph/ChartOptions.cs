//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Jonas SCHMID, Maintainer: Pierre ARNAUD, Jonas SCHMID

using Epsitec.Common.Drawing;
using Epsitec.Common.Support;
using Epsitec.Common.Types;
using Epsitec.Common.Widgets;

using System.Xml.Linq;

namespace Epsitec.Cresus.Graph
{
    /// <summary>
    /// Allow to store options for a chart or a snapshot.
    /// Is able to save and restore from the XML file. 
    /// </summary>
    public class ChartOptions
    {
        public bool ShowFixedCaptions
        {
            get
            {
                return showFixedCaptions;
            }
            set
            {
                if (this.showFixedCaptions != value)
                {
                    var oldValue = this.showFixedCaptions;
                    this.showFixedCaptions = value;

					GraphProgram.Application.SetActiveState (Res.Commands.ChartOptions.ShowSummaryCaptions, value ? ActiveState.Yes : ActiveState.No);

                    // Fire the event if needed
                    if(this.ShowFixedCaptionsChanged != null)
                        this.ShowFixedCaptionsChanged(this, new DependencyPropertyChangedEventArgs("ShowFixedCaption", oldValue, value));
                }
            }
        }

        public bool ShowFloatingCaptions
        {
            get
            {
                return showFloatingCaptions;
            }
            set
            {
                if (this.showFloatingCaptions != value)
                {
                    var oldValue = this.showFloatingCaptions;
                    this.showFloatingCaptions = value;

                    // Fire the event if needed
                    if(this.ShowFloatingCaptionsChanged != null)
                        this.ShowFloatingCaptionsChanged(this, new DependencyPropertyChangedEventArgs("ShowFloatingCaptions", oldValue, value));
                }
            }
        }

        public Margins FixedCaptionsPosition
        {
            get
            {
                return fixedCaptionsPosition;
            }
            set
            {
                if (this.fixedCaptionsPosition != value)
                {
                    var oldValue = this.fixedCaptionsPosition;
                    this.fixedCaptionsPosition = value;

                    // Fire the event if needed
                    if (this.FixedCaptionsPositionChanged != null)
                        this.FixedCaptionsPositionChanged (this, new DependencyPropertyChangedEventArgs ("FixedCaptionsPosition", oldValue, value));
                }
            }
        }

        /// <summary>
        /// Saving the options into the XML fragment
        /// </summary>
        /// <param name="options">XML fragement to save into</param>
        public void SaveOptions(XElement options)
        {
            var fixedCaptions = new XElement("ShowFixedCaptions", this.ShowFixedCaptions);
            var floatingCaptions = new XElement("ShowFloatingCaptions", this.ShowFloatingCaptions);
            var fixedCaptionsPosition = new XElement ("FixedCaptionsPosition", this.fixedCaptionsPosition);

            options.Add(fixedCaptions);
            options.Add(floatingCaptions);
            options.Add (fixedCaptionsPosition);
        }

        /// <summary>
        /// Restoring the options from an XML fragment
        /// </summary>
        /// <param name="options">XML fragment to use</param>
        public void RestoreOptions(XElement options)
        {
            XElement tmp;

            tmp = options.Element ("ShowFixedCaptions");
            if(tmp != null)
                ShowFixedCaptions = tmp.Value == "true";

            tmp = options.Element ("ShowFloatingCaptions");
            if (tmp != null)
                ShowFloatingCaptions = tmp.Value == "true";

            tmp = options.Element ("FixedCaptionsPosition");
            if (tmp != null)
            {
                FixedCaptionsPosition = Margins.Parse (tmp.Value);
            }
            else
            {
                FixedCaptionsPosition = new Margins (0, 4, 4, 0);
            }
        }

        /// <summary>
        /// Copies values from one ChartOptions object to this one.
        /// Events are kept from this object (not copied from oldValues).
        /// </summary>
        /// <param name="oldValues">Object to copy the values from</param>
        internal void copyValues (ChartOptions oldValues)
        {
            this.showFixedCaptions = oldValues.showFixedCaptions;
            this.showFloatingCaptions = oldValues.showFloatingCaptions;
            this.fixedCaptionsPosition = oldValues.fixedCaptionsPosition;
        }

        public event EventHandler<DependencyPropertyChangedEventArgs> ShowFixedCaptionsChanged;
        public event EventHandler<DependencyPropertyChangedEventArgs> ShowFloatingCaptionsChanged;
        public event EventHandler<DependencyPropertyChangedEventArgs> FixedCaptionsPositionChanged;

        private bool showFixedCaptions;
        private bool showFloatingCaptions;
        private Margins fixedCaptionsPosition;
    }
}
