//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Jonas SCHMID, Maintainer: Pierre ARNAUD, Jonas SCHMID

using System.Xml.Linq;
using Epsitec.Common.Support;
using Epsitec.Common.Types;
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

        /// <summary>
        /// Saving the options into the XML fragment
        /// </summary>
        /// <param name="options">XML fragement to save into</param>
        public void SaveOptions(XElement options)
        {
            var fixedCaptions = new XElement("ShowFixedCaptions", this.ShowFixedCaptions);
            var floatingCaptions = new XElement("ShowFloatingCaptions", this.ShowFloatingCaptions);

            options.Add(fixedCaptions);
            options.Add(floatingCaptions);
        }

        /// <summary>
        /// Restoring the options from an XML fragment
        /// </summary>
        /// <param name="options">XML fragment to use</param>
        public void RestoreOptions(XElement options)
        {
            ShowFixedCaptions = options.Element ("ShowFixedCaptions").Value == "true";
            ShowFloatingCaptions = options.Element ("ShowFloatingCaptions").Value == "true";
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
        }

        public event EventHandler<DependencyPropertyChangedEventArgs> ShowFixedCaptionsChanged;
        public event EventHandler<DependencyPropertyChangedEventArgs> ShowFloatingCaptionsChanged;

        private bool showFixedCaptions;
        private bool showFloatingCaptions;
    }
}
