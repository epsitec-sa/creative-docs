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


using Epsitec.Common.Drawing;
using Epsitec.Common.Support;
using Epsitec.Common.Types;

namespace Epsitec.Common.Widgets.Behaviors
{
    public class SlimFieldTextBehavior : SlimFieldBehavior, System.IDisposable
    {
        public SlimFieldTextBehavior(SlimField host)
            : base(host)
        {
            this.host.Entered += this.HandleHostEntered;
            this.host.Exited += this.HandleHostExited;
            this.host.IsFocusedChanged += this.HandleHostIsFocusedChanged;
        }

        public bool HasFocus
        {
            get
            {
                if (this.textField == null)
                {
                    return false;
                }
                else
                {
                    return this.textField.IsFocused;
                }
            }
        }

        public bool HasButtons
        {
            get
            {
                if (this.textField == null)
                {
                    return false;
                }
                else
                {
                    return this.textField.ComputeButtonVisibility();
                }
            }
        }

        #region IDisposable Members

        public void Dispose()
        {
            this.DisposeTextField();

            this.host.Entered -= this.HandleHostEntered;
            this.host.Exited -= this.HandleHostExited;
        }

        #endregion


        public AbstractTextField GetTextField()
        {
            return this.textField;
        }

        public void UpdatePreferredSize()
        {
            this.AdjustGeometry();
        }

        private void HandleHostEntered(object sender, MessageEventArgs e)
        {
            if (this.HasFocus || this.HasButtons)
            {
                return;
            }

            if (this.host.IsReadOnly)
            {
                return;
            }

            this.DisposeTextField();
            this.CreateTextField();
        }

        private void HandleHostExited(object sender, MessageEventArgs e)
        {
            if (this.HasFocus || this.HasButtons)
            {
                return;
            }

            this.DisposeTextField();
        }

        private void HandleHostIsFocusedChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue == true)
            {
                this.DisposeTextField();
                this.CreateTextField();
                this.textField.Focus();
            }
        }

        private void HandleTextIsFocusedChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue == true)
            {
                this.StartTextFieldEdition();
            }
            else
            {
                this.StopTextFieldEdition();
            }
        }

        private void HandleTextEditionStarting(object sender, CancelEventArgs e)
        {
            this.textBeforeEdition = this.host.FieldText;
            this.OnTextEditionStarting(e);
        }

        private void HandleTextEditionStarted(object sender)
        {
            this.StartTextFieldEdition();
            this.TransitionToState(EditionState.Started);
        }

        private void HandleTextEditionAccepted(object sender)
        {
            this.host.FieldText = this.textField.FormattedText.ToSimpleText();
            this.StopTextFieldEdition();
        }

        private void HandleTextEditionRejected(object sender)
        {
            this.host.FieldText = this.textBeforeEdition;
            this.StopTextFieldEdition();
        }

        private void HandleTextTextEdited(object sender)
        {
            var text = this.textField.FormattedText.ToSimpleText();

            if ((text.Length == 0) && (this.HasButtons))
            {
                //	Cheat: we don't want the default label to show up once we have started
                //	editing the text value, so we replace the empty text with an almost empty
                //	one...

                text = " ";
            }

            this.host.FieldText = text;

            this.TransitionToState(EditionState.Edited);
            this.AdjustGeometry();
        }

        private void CreateTextField()
        {
            this.textField = new TextFieldEx(TextFieldStyle.Flat)
            {
                Parent = this.host,
                Dock = DockStyle.Fill,
                FormattedText = FormattedText.FromSimpleText(this.host.FieldText),
                DefocusAction = DefocusAction.Modal,
                SwallowEscapeOnRejectEdition = true,
                SwallowReturnOnAcceptEdition = true,
                IsReadOnly = this.host.IsReadOnly,
            };

            this.textField.SelectAll();

            this.textField.IsFocusedChanged += this.HandleTextIsFocusedChanged;
            this.textField.EditionStarted += this.HandleTextEditionStarted;
            this.textField.EditionStarting += this.HandleTextEditionStarting;
            this.textField.EditionAccepted += this.HandleTextEditionAccepted;
            this.textField.EditionRejected += this.HandleTextEditionRejected;
            this.textField.TextEdited += this.HandleTextTextEdited;

            switch (this.host.DisplayMode)
            {
                case SlimFieldDisplayMode.Label:
                    this.host.DisplayMode = SlimFieldDisplayMode.LabelEdition;
                    break;

                case SlimFieldDisplayMode.Text:
                    this.host.DisplayMode = SlimFieldDisplayMode.TextEdition;
                    break;
            }

            this.AdjustGeometry();
        }

        private void DisposeTextField()
        {
            if (this.textField != null)
            {
                this.textField.IsFocusedChanged -= this.HandleTextIsFocusedChanged;
                this.textField.EditionStarted -= this.HandleTextEditionStarted;
                this.textField.EditionStarting -= this.HandleTextEditionStarting;
                this.textField.EditionAccepted -= this.HandleTextEditionAccepted;
                this.textField.EditionRejected -= this.HandleTextEditionRejected;
                this.textField.TextEdited -= this.HandleTextTextEdited;

                this.textField.Dispose();
                this.textField = null;

                switch (this.host.DisplayMode)
                {
                    case SlimFieldDisplayMode.LabelEdition:
                        this.host.DisplayMode = SlimFieldDisplayMode.Label;
                        break;

                    case SlimFieldDisplayMode.TextEdition:
                        this.host.DisplayMode = SlimFieldDisplayMode.Text;
                        break;
                }

                this.AdjustGeometry();
            }
        }

        private void StartTextFieldEdition()
        {
            System.Diagnostics.Debug.Assert(this.textField != null);

            this.AdjustGeometry();
        }

        private void StopTextFieldEdition()
        {
            if (this.textField == null)
            {
                return;
            }

            this.TransitionToState(EditionState.Inactive);

            if (this.HasFocus || this.HasButtons)
            {
                this.textField.SelectAll();
            }
            else
            {
                this.DisposeTextField();
            }

            this.AdjustGeometry();
        }

        private void AdjustGeometry()
        {
            var mode = this.host.GetActiveDisplayMode();

            if (this.textField != null)
            {
                //-				this.host.FieldText = this.textField.FormattedText.ToSimpleText ();

                if (mode == SlimFieldDisplayMode.Label)
                {
                    this.textField.TextDisplayMode = TextFieldDisplayMode.Transparent;
                }
                else
                {
                    this.textField.TextDisplayMode = TextFieldDisplayMode.Default;
                }
            }

            var width = this.host.MeasureWidth(SlimFieldDisplayMode.MeasureTextOnly);
            var total = this.host.MeasureWidth(mode);

            if (this.HasFocus && (mode == SlimFieldDisplayMode.Text))
            {
                total += System.Math.Max(20, width) - width;
            }

            this.AdjustHostPreferredWidth(total);
            this.AdjustTextFieldMargins();

            this.host.Invalidate();
        }

        private void AdjustHostPreferredWidth(double total)
        {
            if (this.HasButtons)
            {
                var padding = this.textField.GetInternalPadding();
                var width = total + padding.Width - 1;

                this.host.PreferredWidth = System.Math.Ceiling(width);
            }
            else
            {
                this.host.PreferredWidth = System.Math.Ceiling(total);
            }
        }

        private void AdjustTextFieldMargins()
        {
            if (this.textField == null)
            {
                return;
            }

            var prefix = this.host.MeasureWidth(SlimFieldDisplayMode.MeasureTextPrefix) + 1;
            var suffix = this.host.MeasureWidth(SlimFieldDisplayMode.MeasureTextSuffix);

            //	We do not want to round the position, or else we would see the text move
            //	a little bit horizontally whenever the slim field is being hovered:

            double left = prefix;
            double right = suffix;

            this.textField.Margins = new Margins(left, right, 0, 0);
        }

        private void TransitionToState(EditionState newState)
        {
            var oldState = this.textFieldEditionState;

            if (newState == oldState)
            {
                return;
            }

            this.textFieldEditionState = newState;

            switch (newState)
            {
                case EditionState.Inactive:
                    this.OnTextEditionEnded();
                    break;

                case EditionState.Started:
                    this.OnTextEditionStarted();
                    break;

                case EditionState.Edited:
                    if (oldState != EditionState.Started)
                    {
                        this.OnTextEditionStarted();
                    }
                    break;
            }
        }

        #region EditionState Enum

        private enum EditionState
        {
            Inactive,
            Started,
            Edited,
        }

        #endregion


        private TextFieldEx textField;
        private EditionState textFieldEditionState;
        private string textBeforeEdition;
    }
}
