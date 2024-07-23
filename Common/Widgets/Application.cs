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


using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Types;

namespace Epsitec.Common.Widgets
{
    /// <summary>
    /// The <c>Application</c> class offers basic, application related, services.
    /// </summary>
    public abstract class Application : DependencyObject
    {
        protected Application()
        {
            this.applicationStartStatus = Platform.AppSupport.CreateSemaphore(
                this.ApplicationIdentifier
            );

            this.commandDispatcher = CommandDispatcher.DefaultDispatcher;
            this.commandContext = new CommandContext();

            this.resourceManager = Support.Resources.DefaultManager;
            this.resourceManagerPool = this.resourceManager.Pool;
        }

        /// <summary>
        /// Gets the application window.
        /// </summary>
        /// <value>The application window.</value>
        public Window Window
        {
            get { return this.window; }
            protected set
            {
                System.Diagnostics.Debug.Assert(this.window == null);

                this.window = value;

                this.CommandDispatcher.RegisterController(this);

                this.SetupApplicationWindow(this.window);
            }
        }

        /// <summary>
        /// Gets the application command dispatcher.
        /// </summary>
        /// <value>The application command dispatcher.</value>
        public CommandDispatcher CommandDispatcher
        {
            get { return this.commandDispatcher; }
        }

        /// <summary>
        /// Gets the application command context.
        /// </summary>
        /// <value>The application command context.</value>
        public CommandContext CommandContext
        {
            get { return this.commandContext; }
        }

        /// <summary>
        /// Gets the application resource manager.
        /// </summary>
        /// <value>The application resource manager.</value>
        public Support.ResourceManager ResourceManager
        {
            get { return this.resourceManager; }
        }

        /// <summary>
        /// Gets the application resource manager pool.
        /// </summary>
        /// <value>The application resource manager pool.</value>
        public Support.ResourceManagerPool ResourceManagerPool
        {
            get { return this.resourceManagerPool; }
        }

        public abstract string ShortWindowTitle { get; }

        public abstract string ApplicationIdentifier { get; }

        public ApplicationStartStatus ApplicationStartStatus
        {
            get { return this.applicationStartStatus; }
        }

        public static void SetWaitCursor()
        {
            Platform.PlatformWindow.UseWaitCursor = true;
        }

        public static void ClearWaitCursor()
        {
            Platform.PlatformWindow.UseWaitCursor = false;
        }

        public void SetEnable(Command command, bool enable)
        {
            if (this.commandContext != null)
            {
                this.commandContext.GetCommandState(command).Enable = enable;
            }
        }

        public void SetActiveState(Command command, ActiveState activeState)
        {
            if (this.commandContext != null)
            {
                this.commandContext.GetCommandState(command).ActiveState = activeState;
            }
        }

        public static string GetCommandLineProgram()
        {
            var commandLine = System.Environment.CommandLine;
            var splitMode = System.StringSplitOptions.RemoveEmptyEntries;

            return Support.Utilities.StringToTokens(commandLine, ' ', splitMode).FirstOrDefault();
        }

        public static IList<string> GetCommandLineArguments()
        {
            var commandLine = System.Environment.CommandLine;
            var splitMode = System.StringSplitOptions.RemoveEmptyEntries;

            return new List<string>(
                Support.Utilities.StringToTokens(commandLine, ' ', splitMode).Skip(1)
            );
        }

        protected override void Dispose(bool disposing)
        {
            this.window.Dispose();
            base.Dispose(disposing);
        }

        [Support.Command(ApplicationCommands.Id.Quit)]
        protected virtual void ExecuteQuit(CommandDispatcher dispatcher, CommandEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Application: Quit executed");
            e.Executed = true;
            Window.Quit();
        }

        private void SetupApplicationWindow(Window window)
        {
            CommandDispatcher.SetDispatcher(window, this.CommandDispatcher);
            CommandContext.SetContext(window, this.CommandContext);
            Application.SetApplication(window, this);

            Support.ResourceManager.SetResourceManager(window, this.ResourceManager);

            window.Root.WindowType = WindowType.Document;
            window.Name = "Application";
            window.PreventAutoClose = true;
            window.PreventAutoQuit = false;
        }

        public static void Invoke(Support.SimpleCallback callback)
        {
            callback.DynamicInvoke();
        }

        public static void SetApplication(DependencyObject obj, Application application)
        {
            obj.SetValue(Application.ApplicationProperty, application);
        }

        public static Application GetApplication(DependencyObject obj)
        {
            return (Application)obj.GetValue(Application.ApplicationProperty);
        }

        public static readonly DependencyProperty ApplicationProperty =
            DependencyProperty.RegisterAttached(
                "Application",
                typeof(Application),
                typeof(Application)
            );

        private readonly CommandDispatcher commandDispatcher;
        private readonly CommandContext commandContext;
        private readonly Support.ResourceManager resourceManager;
        private readonly Support.ResourceManagerPool resourceManagerPool;
        private readonly ApplicationStartStatus applicationStartStatus;

        private Window window;
    }
}
