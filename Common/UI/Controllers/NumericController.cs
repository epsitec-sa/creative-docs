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


using Epsitec.Common.Types;
using Epsitec.Common.UI;
using Epsitec.Common.Widgets;

[assembly: Controller(typeof(Epsitec.Common.UI.Controllers.NumericController))]

namespace Epsitec.Common.UI.Controllers
{
    public class NumericController : AbstractController
    {
        public NumericController(ControllerParameters parameters)
            : base(parameters) { }

        public override object GetUserInterfaceValue()
        {
            return this.field.Text;
        }

        protected override void CreateUserInterface(INamedType namedType, Caption caption)
        {
            this.Placeholder.ContainerLayoutMode = ContainerLayoutMode.HorizontalFlow;

            if (this.Placeholder.IsReadOnlyValueBinding)
            {
                this.CreateReadOnlyUserInterface(caption);
            }
            else
            {
                this.CreateReadWriteUserInterface(caption, namedType);
            }
        }

        private void CreateReadOnlyUserInterface(Caption caption)
        {
            this.label = new StaticText();
            this.field = null;

            this.AddWidget(this.label, WidgetType.Content);

            this.label.HorizontalAlignment = HorizontalAlignment.Stretch;
            this.label.VerticalAlignment = VerticalAlignment.BaseLine;
            this.label.ContentAlignment = Drawing.ContentAlignment.MiddleLeft;
            this.label.Dock = DockStyle.Stacked;

            this.SetupToolTip(this.label, caption);
        }

        private void CreateReadWriteUserInterface(Caption caption, INamedType namedType)
        {
            this.label = new StaticText();
            this.field = new TextFieldSlider();

            this.AddWidget(this.label, WidgetType.Label);
            this.AddWidget(this.field, WidgetType.Input);

            this.label.HorizontalAlignment = HorizontalAlignment.Right;
            this.label.VerticalAlignment = VerticalAlignment.BaseLine;
            this.label.ContentAlignment = Drawing.ContentAlignment.MiddleRight;
            this.label.Dock = DockStyle.Stacked;

            this.SetupLabelWidget(this.label, caption);

            this.field.HorizontalAlignment = HorizontalAlignment.Stretch;
            this.field.VerticalAlignment = VerticalAlignment.BaseLine;
            this.field.TextChanged += this.HandleFieldTextChanged;
            this.field.PreferredWidth = 40;

            this.field.TabIndex = 1;
            this.field.TabNavigationMode = TabNavigationMode.ActivateOnTab;
            this.field.Dock = DockStyle.Stacked;

            INumericType numType = namedType as INumericType;

            if (numType != null)
            {
                if (!numType.Range.IsEmpty)
                {
                    this.field.MinValue = numType.Range.Minimum;
                    this.field.MaxValue = numType.Range.Maximum;
                    this.field.Resolution = numType.Range.Resolution;
                }

                if (!numType.PreferredRange.IsEmpty)
                {
                    this.field.PreferredRange = numType.PreferredRange;
                }
            }

            this.validator = new Validators.ControllerBasedValidator(this.field, this);
        }

        protected override void PrepareUserInterfaceDisposal()
        {
            base.PrepareUserInterfaceDisposal();

            if (this.field != null)
            {
                this.field.TextChanged -= this.HandleFieldTextChanged;
            }
        }

        protected override void RefreshUserInterface(object oldValue, object newValue)
        {
            string text = "";

            if (
                (newValue != UndefinedValue.Value)
                && (newValue != InvalidValue.Value)
                && (newValue != null)
            )
            {
                text = this.ConvertFromValue(newValue);
            }

            if (this.field != null)
            {
                this.field.Text = text;
            }
            else
            {
                this.label.Text = text;
                this.label.PreferredWidth = this.label.GetBestFitSize().Width;
            }
        }

        private void HandleFieldTextChanged(object sender)
        {
            if (this.field.IsValid)
            {
                this.OnActualValueChanged();
            }
        }

        private string ConvertFromValue(object newValue)
        {
            return newValue.ToString();
        }

        private TextFieldSlider field;
        private StaticText label;
        private IValidator validator;
    }
}
