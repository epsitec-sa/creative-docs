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
using System.Collections.Generic;

[assembly: Controller(typeof(Epsitec.Common.UI.Controllers.EnumController))]

namespace Epsitec.Common.UI.Controllers
{
    public class EnumController : AbstractController
    {
        public EnumController(ControllerParameters parameters)
            : base(parameters)
        {
            switch (parameters.GetParameterValue("Mode"))
            {
                case "Combo":
                    this.helper = new ComboHelper(this);
                    break;

                case "Icons":
                    this.helper = new IconsHelper(this);
                    break;

                case "Radio":
                    break;

                default:
                    break;
            }

            if (this.helper == null)
            {
                this.helper = new ComboHelper(this);
            }
        }

        public override object GetUserInterfaceValue()
        {
            return this.helper.GetSelectedKey();
        }

        protected override void CreateUserInterface(INamedType namedType, Caption caption)
        {
            this.Placeholder.ContainerLayoutMode = ContainerLayoutMode.HorizontalFlow;

            IEnumType enumType = namedType as IEnumType;

            this.helper.CreateUserInterface(enumType, caption);
        }

        protected override void PrepareUserInterfaceDisposal()
        {
            base.PrepareUserInterfaceDisposal();

            this.helper.Dispose();
            this.helper = null;
        }

        protected override void RefreshUserInterface(object oldValue, object newValue)
        {
            if (
                (newValue != UndefinedValue.Value)
                && (newValue != InvalidValue.Value)
                && (newValue != null)
            )
            {
                this.helper.SetSelectedKey(this.ConvertFromValue(newValue));
            }
        }

        private void HandleComboTextChanged(object sender)
        {
            this.OnActualValueChanged();
        }

        private string ConvertFromValue(object newValue)
        {
            return newValue.ToString();
        }

        private enum Mode
        {
            None,
            Combo,
            Icons,
            Radio
        }

        private abstract class Helper : System.IDisposable
        {
            public Helper(EnumController host)
            {
                this.host = host;
            }

            public abstract void CreateUserInterface(IEnumType enumType, Caption caption);
            public abstract string GetSelectedKey();

            protected EnumController host;

            #region IDisposable Members

            public abstract void Dispose();

            #endregion

            public abstract void SetSelectedKey(string name);
        }

        private class ComboHelper : Helper
        {
            public ComboHelper(EnumController host)
                : base(host) { }

            public override string GetSelectedKey()
            {
                return this.combo.SelectedKey;
            }

            public override void SetSelectedKey(string name)
            {
                this.combo.SelectedKey = name;
            }

            public override void Dispose()
            {
                this.combo.TextChanged -= this.host.HandleComboTextChanged;
            }

            public override void CreateUserInterface(IEnumType enumType, Caption caption)
            {
                this.label = new StaticText();
                this.combo = new TextFieldCombo();

                this.host.AddWidget(this.label, WidgetType.Label);
                this.host.AddWidget(this.combo, WidgetType.Input);

                this.label.HorizontalAlignment = HorizontalAlignment.Right;
                this.label.VerticalAlignment = VerticalAlignment.BaseLine;
                this.label.ContentAlignment = Drawing.ContentAlignment.MiddleRight;
                this.label.Dock = DockStyle.Stacked;

                this.host.SetupLabelWidget(this.label, caption);

                this.combo.HorizontalAlignment = HorizontalAlignment.Stretch;
                this.combo.VerticalAlignment = VerticalAlignment.BaseLine;
                this.combo.TextChanged += this.host.HandleComboTextChanged;
                this.combo.PreferredWidth = 40;

                this.combo.TabIndex = 1;
                this.combo.TabNavigationMode = TabNavigationMode.ActivateOnTab;
                this.combo.Dock = DockStyle.Stacked;

                if (enumType != null)
                {
                    Support.ResourceManager manager = Widgets.Helpers.VisualTree.GetResourceManager(
                        this.host.Placeholder
                    );

                    foreach (IEnumValue enumValue in enumType.Values)
                    {
                        if (!enumValue.IsHidden)
                        {
                            Caption evCaption = Support.CaptionCache.Instance.GetCaption(
                                manager,
                                enumValue.CaptionId
                            );
                            string evCaptionText = null;

                            if ((evCaption != null) && (evCaption.Labels.Count > 0))
                            {
                                evCaptionText = Collection.Extract(evCaption.Labels, 0);
                            }

                            this.combo.Items.Add(enumValue.Name, evCaptionText ?? enumValue.Name);
                        }
                    }

                    this.combo.IsReadOnly = enumType.IsCustomizable ? false : true;
                }
            }

            private TextFieldCombo combo;
            private StaticText label;
        }

        private class IconsHelper : Helper
        {
            public IconsHelper(EnumController host)
                : base(host) { }

            public override string GetSelectedKey()
            {
                if (this.enumType.IsDefinedAsFlags)
                {
                    int flags = this.combo.SelectedValue;
                    IEnumerable<IEnumValue> values = EnumType.ConvertEnumValuesFromFlags(
                        this.enumType,
                        flags
                    );
                    return EnumType.ConvertToString(values);
                }
                else
                {
                    int rank = this.combo.SelectedValue;
                    IEnumValue value = this.enumType[rank];

                    if (value != null)
                    {
                        return value.Name;
                    }
                    else
                    {
                        return null;
                    }
                }
            }

            public override void SetSelectedKey(string name)
            {
                if (this.enumType.IsDefinedAsFlags)
                {
                    IEnumerable<IEnumValue> values = EnumType.ConvertFromString(
                        this.enumType,
                        name
                    );
                    int flags = EnumType.ConvertEnumValuesToFlags(values);
                    this.combo.SelectedValue = flags;
                }
                else
                {
                    IEnumValue value = this.enumType[name];
                    if (value != null)
                    {
                        this.combo.SelectedValue = value.Rank;
                    }
                    else
                    {
                        this.combo.SelectedValue = -1;
                    }
                }
            }

            public override void Dispose()
            {
                this.combo.SelectionChanged -= this.host.HandleComboTextChanged;
            }

            public override void CreateUserInterface(IEnumType enumType, Caption caption)
            {
                this.enumType = enumType;

                this.label = new StaticText();
                this.combo = enumType.IsDefinedAsFlags ? new CheckIconGrid() : new RadioIconGrid();

                this.host.AddWidget(this.label, WidgetType.Label);
                this.host.AddWidget(this.combo, WidgetType.Input);

                this.label.HorizontalAlignment = HorizontalAlignment.Right;
                this.label.VerticalAlignment = VerticalAlignment.Center;
                this.label.ContentAlignment = Drawing.ContentAlignment.MiddleRight;
                this.label.Dock = DockStyle.Stacked;

                this.host.SetupLabelWidget(this.label, caption);

                this.combo.HorizontalAlignment = HorizontalAlignment.Stretch;
                this.combo.VerticalAlignment = VerticalAlignment.Center;
                this.combo.SelectionChanged += this.host.HandleComboTextChanged;
                this.combo.PreferredWidth = 40;
                this.combo.PreferredHeight = 22;

                this.combo.TabIndex = 1;
                this.combo.TabNavigationMode = TabNavigationMode.ActivateOnTab;
                this.combo.Dock = DockStyle.Stacked;

                if (enumType != null)
                {
                    Support.ResourceManager manager = Widgets.Helpers.VisualTree.GetResourceManager(
                        this.host.Placeholder
                    );

                    foreach (IEnumValue enumValue in enumType.Values)
                    {
                        if (!enumValue.IsHidden)
                        {
                            Caption evCaption = Support.CaptionCache.Instance.GetCaption(
                                manager,
                                enumValue.CaptionId
                            );
                            string evCaptionText = null;
                            string evCaptionDesc = null;

                            if ((evCaption != null) && (evCaption.Labels.Count > 0))
                            {
                                evCaptionText = Collection.Extract(evCaption.Labels, 0);
                            }

                            if (evCaption != null)
                            {
                                evCaptionDesc = evCaption.Description;
                            }

                            string icon = evCaption.Icon;
                            string text = evCaptionText ?? enumValue.Name;
                            string tip = evCaptionDesc ?? text;

                            int value = this.enumType.IsDefinedAsFlags
                                ? EnumType.ConvertToInt(enumValue.Value)
                                : enumValue.Rank;

                            this.combo.AddRadioIcon(icon, tip, value, false);
                        }
                    }
                }
            }

            private RadioIconGrid combo;
            private StaticText label;
            private IEnumType enumType;
        }

        private Helper helper;
    }
}
