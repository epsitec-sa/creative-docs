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

namespace Epsitec.Common.Widgets
{
    /// <summary>
    /// The <c>CheckIconGrid</c> class represents and manages a grid of icons
    /// where zero, one or many items can be selected at once.
    /// </summary>
    public class CheckIconGrid : RadioIconGrid
    {
        public CheckIconGrid()
        {
            this.selectedEnumValues = new List<int>();
        }

        public CheckIconGrid(Widget embedder)
            : this()
        {
            this.SetEmbedder(embedder);
        }

        protected override bool IsIconSelected(RadioIcon icon)
        {
            if (this.selectedEnumValues.Contains(icon.EnumValue))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public override int SelectedValue
        {
            get
            {
                if (this.selectedEnumValues.Count == 0)
                {
                    return 0;
                }
                else
                {
                    int value = 0;

                    foreach (int item in this.selectedEnumValues)
                    {
                        value |= item;
                    }

                    return value;
                }
            }
            set
            {
                if (this.SelectedValue != value)
                {
                    this.suspendCounter++;

                    try
                    {
                        foreach (RadioIcon icon in this.list)
                        {
                            if ((icon.EnumValue & value) == icon.EnumValue)
                            {
                                icon.ActiveState = ActiveState.Yes;
                            }
                            else
                            {
                                icon.ActiveState = ActiveState.No;
                            }
                        }
                    }
                    finally
                    {
                        this.suspendCounter--;
                    }

                    if (this.pendingCounter > 0)
                    {
                        this.OnSelectionChanged();
                    }
                }
            }
        }

        protected override void SetupController()
        {
            this.controller = null;
        }

        protected override void AttachRadioIcon(RadioIcon icon)
        {
            base.AttachRadioIcon(icon);

            icon.AutoRadio = false;

            icon.ActiveStateChanged += this.HandleRadioIconActiveStateChanged;
        }

        protected override void DetachRadioIcon(RadioIcon icon)
        {
            base.DetachRadioIcon(icon);

            icon.ActiveStateChanged -= this.HandleRadioIconActiveStateChanged;
        }

        private void HandleRadioIconActiveStateChanged(object sender)
        {
            RadioIcon icon = sender as RadioIcon;

            switch (icon.ActiveState)
            {
                case ActiveState.Yes:
                    if (!this.selectedEnumValues.Contains(icon.EnumValue))
                    {
                        this.selectedEnumValues.Add(icon.EnumValue);
                        this.OnSelectionChanged();
                    }
                    break;

                case ActiveState.No:
                    if (this.selectedEnumValues.Contains(icon.EnumValue))
                    {
                        this.selectedEnumValues.Remove(icon.EnumValue);
                        this.OnSelectionChanged();
                    }
                    break;
            }
        }

        protected override void OnSelectionChanged()
        {
            if (this.suspendCounter == 0)
            {
                this.pendingCounter = 0;
                base.OnSelectionChanged();
            }
            else
            {
                this.pendingCounter++;
            }
        }

        private readonly List<int> selectedEnumValues;
        private int suspendCounter;
        private int pendingCounter;
    }
}
