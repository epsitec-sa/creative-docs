//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Jonas SCHMID, Maintainer: Pierre ARNAUD, Jonas SCHMID

using Epsitec.Cresus.Graph.Widgets;
using Epsitec.Common.Graph.Widgets;
using Epsitec.Common.Widgets;
using Epsitec.Common.Drawing;
using Epsitec.Common.UI;
using System.Xml.Linq;

namespace Epsitec.Cresus.Graph.Controllers
{
    /// <summary>
    /// Controller for <see cref="ChartOptions"/>.
    /// Shows a commandbar in the chart window to change the options.
    /// </summary>
    public class ChartOptionsController
    {
        /// <summary>
        /// Create the controller into a frame
        /// </summary>
        /// <param name="commandBar">FrameBox where to put the configuration buttons</param>
        /// <param name="captions">FrameBox showing the captions to show/hide</param>
        /// <param name="floatingCaptions">FrameBox showing the floating captions to show/hide</param>
        public ChartOptionsController(FrameBox commandBar, AnchoredPalette captions, FloatingCaptionsView floatingCaptions)
        {
            this.chartOptions = new ChartOptions()
            {
                ShowFixedCaptions = true,
                ShowFloatingCaptions = true
            };

            this.SetupUI(commandBar, captions, floatingCaptions);
        }

        /// <summary>
        /// Options handled by the controller
        /// </summary>
        internal ChartOptions ChartOptions
        {
            get
            {
                return chartOptions;
            }
        }

        private void SetupUI (FrameBox commandBar, AnchoredPalette fixedCaptions, FloatingCaptionsView floatingCaptions)
        {
            new Separator()
            {
                IsVerticalLine = true,
                Dock = DockStyle.Stacked,
                PreferredWidth = 1,
                Parent = commandBar,
                Margins = new Margins(2, 2, 0, 0),
            };

            var frame = new FrameBox()
            {
                Dock = DockStyle.Stacked,
                Parent = commandBar,
                ContainerLayoutMode = ContainerLayoutMode.HorizontalFlow,
            };

            var showCaptionsButton = new MetaButton()
            {
                Dock = DockStyle.Left,
                IconUri = "manifest:Epsitec.Cresus.Graph.Images.Captions.icon",
                ButtonClass = ButtonClass.FlatButton,
                Parent = frame,
                PreferredSize = new Size(40, 40),
                Padding = new Margins(4, 4, 0, 0),
            };

            // Change visibility when value is changed
            this.ChartOptions.ShowFixedCaptionsChanged +=
                (sender, e) =>
                {
                    fixedCaptions.Visibility = (bool) e.NewValue;
                };

            // Change value when then button is click
            showCaptionsButton.Clicked +=
                (sender, e) =>
                {
                    this.ChartOptions.ShowFixedCaptions = !this.ChartOptions.ShowFixedCaptions;
                };

            var showFloatingCaptionsButton = new MetaButton()
            {
                Dock = DockStyle.Left,
                IconUri = "manifest:Epsitec.Cresus.Graph.Images.FloatingCaptions.icon",
                ButtonClass = ButtonClass.FlatButton,
                Parent = frame,
                PreferredSize = new Size(40, 40),
                Padding = new Margins(4, 4, 0, 0),
            };

            // Change visibility when value is changed
            this.ChartOptions.ShowFloatingCaptionsChanged +=
                (sender, e) =>
                {
                    floatingCaptions.Visibility = (bool) e.NewValue;
                };

            // Change value when then button is click
            showFloatingCaptionsButton.Clicked +=
                (sender, e) =>
                {
                    this.ChartOptions.ShowFloatingCaptions = !this.ChartOptions.ShowFloatingCaptions;
                };
        }


        private readonly ChartOptions chartOptions;
    }
}
