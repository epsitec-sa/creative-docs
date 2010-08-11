//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Jonas SCHMID, Maintainer: Pierre ARNAUD, Jonas SCHMID

using Epsitec.Common.Drawing;
using Epsitec.Common.Graph.Widgets;
using Epsitec.Common.UI;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Graph.Widgets;
using Epsitec.Common.Support;
using System;

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
        /// <param name="commandBar">CommandSelectionBar where to put the configuration buttons</param>
        /// <param name="summaryCaptions">FrameBox showing the captions to show/hide</param>
        /// <param name="seriesCaptions">FrameBox showing the floating captions to show/hide</param>
        public ChartOptionsController (CommandSelectionBar commandBar, AnchoredPalette summaryCaptions, SeriesCaptionsView seriesCaptions)
        {
            this.chartOptions = new ChartOptions()
            {
                ShowSummaryCaptions = true,
                ShowSeriesCaptions = true,
                SummaryCaptionsPosition = new Margins (0, 4, 4, 0)
            };

			this.dispatcher = new CommandDispatcher ("ChartOptions Dispatcher", CommandDispatcherLevel.Secondary);
			this.dispatcher.RegisterController (this);

            // Change visibility when value is changed
            this.ChartOptions.ShowSummaryCaptionsChanged +=
                (sender, e) =>
                {
                    summaryCaptions.Visibility = (bool) e.NewValue;
                };

            // Change visibility when value is changed
            this.ChartOptions.ShowSeriesCaptionsChanged +=
                (sender, e) =>
                {
                    seriesCaptions.Visibility = (bool) e.NewValue;
                };

            this.ChartOptions.SummaryCaptionsPositionChanged +=
                (sender, e) =>
                {
                    summaryCaptions.Margins = (Margins)e.NewValue;
                };

            summaryCaptions.MarginsChanged +=
                (sender, e) =>
                {
                    this.ChartOptions.SummaryCaptionsPosition = (Margins)e.NewValue;
                };

            this.SetupUI(commandBar);
        }

        /// <summary>
        /// Options handled by the controller
        /// </summary>
        internal ChartOptions ChartOptions
        {
            get
            {
                return this.chartOptions;
            }
            set
            {
                this.chartOptions.copyValues (value);
            }
        }

        private void SetupUI (CommandSelectionBar commandBar)
        {
            new Separator()
            {
                IsVerticalLine = true,
                Dock = DockStyle.Stacked,
                PreferredWidth = 1,
                Parent = commandBar,
                Margins = new Margins(2, 2, 0, 0),
            };

            // Buttons container
            var frame = new FrameBox ()
            {
                Dock = DockStyle.Stacked,
                Parent = commandBar,
                ContainerLayoutMode = ContainerLayoutMode.HorizontalFlow,
            };

            // Used to dispatch the commands
            CommandDispatcher.SetDispatcher (frame, this.dispatcher);

            // Create the buttons
            CreateCommandButton (frame, Res.Commands.ChartOptions.ShowSummaryCaptions);
            CreateCommandButton (frame, Res.Commands.ChartOptions.ShowSeriesCaptions);
        }

        /// <summary>
        /// Create a button into a Widget using a command. Allows to easily create buttons with the same layout
        /// </summary>
        /// <param name="parent">Where to put the button</param>
        /// <param name="command">Associated command</param>
        private static void CreateCommandButton (Widget parent, Command command)
        {
            new MetaButton ()
            {
                CommandObject = command,
                Parent = parent,
                Dock = ChartOptionsController.preferredDockStyle,
                ButtonClass = ChartOptionsController.preferredButtonClass,
                PreferredSize = ChartOptionsController.preferredSize,
                Padding = ChartOptionsController.preferredPadding
            };
        }

        /// <summary>
        /// Called when clicking the summary captions button
        /// </summary>
        [Command (Res.CommandIds.ChartOptions.ShowSummaryCaptions)]
        private void ExecuteShowSummaryCaptionsCommand ()
        {
            this.ChartOptions.ShowSummaryCaptions = !this.ChartOptions.ShowSummaryCaptions;
        }

        /// <summary>
        /// Called when clicking the series captions button
        /// </summary>
        [Command (Res.CommandIds.ChartOptions.ShowSeriesCaptions)]
        private void ExecuteShowSeriesCaptionsCommand ()
        {
            this.ChartOptions.ShowSeriesCaptions = !this.ChartOptions.ShowSeriesCaptions;
        }

        // Options for the buttons
        private static readonly Size preferredSize = new Size (40, 40);
        private static readonly Margins preferredPadding = new Margins (4, 4, 0, 0);
        private static readonly ButtonClass preferredButtonClass = ButtonClass.FlatButton;
        private static readonly DockStyle preferredDockStyle = DockStyle.Left;

        private readonly ChartOptions chartOptions;
		private readonly CommandDispatcher dispatcher;
    }
}
