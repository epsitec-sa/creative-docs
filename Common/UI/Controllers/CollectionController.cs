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
using Epsitec.Common.UI.Controllers;
using Epsitec.Common.Widgets;

[assembly: Controller(typeof(CollectionController))]

namespace Epsitec.Common.UI.Controllers
{
    public class CollectionController : AbstractController
    {
        public CollectionController(ControllerParameters parameters)
            : base(parameters) { }

        protected override void CreateUserInterface(INamedType namedType, Caption caption)
        {
            this.label = new StaticText();
            this.field = new TextFieldMulti();
            this.field.PreferredHeight = this.Placeholder.PreferredHeight;

            this.AddWidget(this.label, WidgetType.Label);
            this.AddWidget(this.field, WidgetType.Input);

            this.label.HorizontalAlignment = HorizontalAlignment.Right;
            this.label.VerticalAlignment = VerticalAlignment.BaseLine;
            this.label.ContentAlignment = Drawing.ContentAlignment.MiddleRight;
            this.label.Dock = DockStyle.Stacked;

            this.SetupLabelWidget(this.label, caption);

            this.field.HorizontalAlignment = HorizontalAlignment.Stretch;
            this.field.VerticalAlignment = VerticalAlignment.BaseLine;
            this.field.PreferredWidth = 40;

            this.field.TabIndex = 1;
            this.field.TabNavigationMode = TabNavigationMode.ActivateOnTab;
            this.field.Dock = DockStyle.Stacked;

            this.SetupToolTip(this.field, caption);

            this.label.Name = null;
            this.field.Name = caption.Name;
            this.field.IsReadOnly = true;
        }

        protected override void RefreshUserInterface(object oldValue, object newValue)
        {
            CollectionView cv = newValue as CollectionView;

            if (cv == null)
            {
                this.field.FormattedText = FormattedText.Empty;
            }
            else
            {
#if false
				string data = entity.DumpFlatData (field => field.Source == FieldSource.Value);

				data = data.Replace ("<UndefinedValue>", "");
				data = data.Replace ("<null>", "");

				while (data.Contains ("\n\n"))
				{
					data = data.Replace ("\n\n", "\n");
				}

				data = data.Trim ();
				data = data.Replace ("\n", ", ");

				this.field.FormattedText = data;
#else
                this.field.FormattedText = FormattedText.FromSimpleText(
                    string.Format("Collection has {0} items", cv.Count)
                );
#endif
            }
        }

        private AbstractTextField field;
        private StaticText label;
    }
}
