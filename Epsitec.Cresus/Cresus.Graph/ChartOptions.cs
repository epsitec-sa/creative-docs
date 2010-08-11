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
        public bool ShowSummaryCaptions
        {
            get
            {
                return showSummaryCaptions;
            }
            set
            {
                if (this.showSummaryCaptions != value)
                {
                    var oldValue = this.showSummaryCaptions;
                    this.showSummaryCaptions = value;

					GraphProgram.Application.SetActiveState (Res.Commands.ChartOptions.ShowSummaryCaptions, value ? ActiveState.Yes : ActiveState.No);

                    // Fire the event if needed
                    if(this.ShowSummaryCaptionsChanged != null)
                        this.ShowSummaryCaptionsChanged(this, new DependencyPropertyChangedEventArgs("ShowSummaryCaption", oldValue, value));
                }
            }
        }

        public bool ShowSeriesCaptions
        {
            get
            {
                return showSeriesCaptions;
            }
            set
            {
                if (this.showSeriesCaptions != value)
                {
                    var oldValue = this.showSeriesCaptions;
                    this.showSeriesCaptions = value;

                    GraphProgram.Application.SetActiveState (Res.Commands.ChartOptions.ShowSeriesCaptions, value ? ActiveState.Yes : ActiveState.No);

                    // Fire the event if needed
                    if(this.ShowSeriesCaptionsChanged != null)
                        this.ShowSeriesCaptionsChanged(this, new DependencyPropertyChangedEventArgs("ShowSeriesCaptions", oldValue, value));
                }
            }
        }

        public Margins SummaryCaptionsPosition
        {
            get
            {
                return summaryCaptionsPosition;
            }
            set
            {
                if (this.summaryCaptionsPosition != value)
                {
                    var oldValue = this.summaryCaptionsPosition;
                    this.summaryCaptionsPosition = value;

                    // Fire the event if needed
                    if (this.SummaryCaptionsPositionChanged != null)
                        this.SummaryCaptionsPositionChanged (this, new DependencyPropertyChangedEventArgs ("SummaryCaptionsPosition", oldValue, value));
                }
            }
        }

        /// <summary>
        /// Saving the options into the XML fragment
        /// </summary>
        /// <param name="options">XML fragement to save into</param>
        public void SaveOptions (XElement options)
        {
            var summaryCaptions = new XElement("ShowSummaryCaptions", this.ShowSummaryCaptions);
            var seriesCaptions = new XElement("ShowSeriesCaptions", this.ShowSeriesCaptions);
            var summaryCaptionsPosition = new XElement ("SummaryCaptionsPosition", this.SummaryCaptionsPosition);

            options.Add(summaryCaptions);
            options.Add(seriesCaptions);
            options.Add (summaryCaptionsPosition);
        }

        /// <summary>
        /// Restoring the options from an XML fragment
        /// </summary>
        /// <param name="options">XML fragment to use</param>
        public void RestoreOptions (XElement options)
        {

            XElement tmp;

            tmp = options.Element ("ShowSummaryCaptions");
            if (tmp != null)
                ShowSummaryCaptions = tmp.Value == "true";
            else
                ShowSummaryCaptions = true;

            tmp = options.Element ("ShowSeriesCaptions");
            if (tmp != null)
                ShowSeriesCaptions = tmp.Value == "true";
            else
                ShowSeriesCaptions = true;

            tmp = options.Element ("SummaryCaptionsPosition");
            if (tmp != null)
                SummaryCaptionsPosition = Margins.Parse (tmp.Value);
            else
                SummaryCaptionsPosition = new Margins (0, 4, 4, 0);
        }

        /// <summary>
        /// Copies values from one ChartOptions object to this one.
        /// Events are kept from this object (not copied from oldValues).
        /// </summary>
        /// <param name="oldValues">Object to copy the values from</param>
        internal void copyValues (ChartOptions oldValues)
        {
            this.ShowSummaryCaptions = oldValues.ShowSummaryCaptions;
            this.ShowSeriesCaptions = oldValues.ShowSeriesCaptions;
            this.SummaryCaptionsPosition = oldValues.SummaryCaptionsPosition;
        }

        public event EventHandler<DependencyPropertyChangedEventArgs> ShowSummaryCaptionsChanged;
        public event EventHandler<DependencyPropertyChangedEventArgs> ShowSeriesCaptionsChanged;
        public event EventHandler<DependencyPropertyChangedEventArgs> SummaryCaptionsPositionChanged;

        private bool showSummaryCaptions;
        private bool showSeriesCaptions;
        private Margins summaryCaptionsPosition;
    }
}
