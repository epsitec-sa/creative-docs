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


using Epsitec.Common.Widgets;

namespace Epsitec.Common.Dialogs
{
    /// <summary>
    /// Summary description for SelectFromList.
    /// </summary>
    public class SelectFromList : AbstractOkCancelDialog
    {
        public SelectFromList(
            string title,
            string caption,
            string[] data,
            string commandTemplate,
            CommandDispatcher commandDispatcher
        )
            : base(title, null, null, commandTemplate, commandDispatcher)
        {
            this.caption = caption;
            this.data = data;
        }

        public virtual string[] Items
        {
            get { return this.data; }
            set
            {
                this.data = value;

                if (this.list != null)
                {
                    this.list.Items.Clear();
                    this.list.Items.AddRange(this.data);
                    this.list.SelectedItemIndex = 0;
                }
            }
        }

        public override string[] CommandArgs
        {
            get
            {
                string[] values = new string[1];

                values[0] = this.list.Items[this.list.SelectedItemIndex];

                return values;
            }
        }

        protected virtual double ExtraHeight
        {
            get { return 0; }
        }

        protected virtual void AddExtraWidgets(Widget body) { }

        protected override Widget CreateBodyWidget()
        {
            Widget body = new Widget();
            double extra = this.ExtraHeight;
            double height = System.Math.Max(extra + 160, 200);

            body.SetManualBounds(new Drawing.Rectangle(0, 0, 320, height));

            StaticText label;

            label = new StaticText(body);
            label.SetManualBounds(
                new Drawing.Rectangle(
                    0,
                    body.ActualHeight - label.PreferredHeight,
                    body.ActualWidth,
                    label.PreferredHeight
                )
            );
            label.Text = this.caption;

            this.list = new ScrollList(body);

            this.list.SetManualBounds(
                new Drawing.Rectangle(
                    0,
                    extra,
                    body.ActualWidth,
                    label.ActualLocation.Y - 4 - extra
                )
            );
            this.list.Items.AddRange(this.data);
            this.list.TabIndex = 1;
            this.list.TabNavigationMode = TabNavigationMode.ActivateOnTab;
            this.list.DoubleClicked += this.HandleListDoubleClicked;

            this.AddExtraWidgets(body);

            return body;
        }

        private void HandleListDoubleClicked(object sender, MessageEventArgs e)
        {
            Widget widget = this.list.Window.Root.FindCommandWidget(Res.Commands.Dialog.Generic.Ok);

            System.Diagnostics.Debug.Assert(widget != null);
            System.Diagnostics.Debug.Assert(widget.Parent == this.list.Window.Root);

            widget.ExecuteCommand();
        }

        protected string[] data;
        protected string caption;
        protected ScrollList list;
    }
}
