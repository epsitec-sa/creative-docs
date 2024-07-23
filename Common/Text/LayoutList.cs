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


namespace Epsitec.Common.Text
{
    /// <summary>
    /// La classe LayoutList gère la liste des moteurs de layout.
    /// </summary>
    public sealed class LayoutList
    {
        public LayoutList(Text.TextContext context)
        {
            this.context = context;
            this.layouts = new System.Collections.Hashtable();
        }

        public Layout.BaseEngine this[string name]
        {
            get { return this.layouts[name] as Layout.BaseEngine; }
        }

        public Layout.BaseEngine NewEngine(string name, System.Type type)
        {
            Debug.Assert.IsFalse(this.layouts.Contains(name));

            Layout.BaseEngine engine = System.Activator.CreateInstance(type) as Layout.BaseEngine;

            engine.Initialize(this.context, name);

            this.layouts[name] = engine;

            return engine;
        }

        public void DisposeEngine(Layout.BaseEngine engine)
        {
            Debug.Assert.IsTrue(this.layouts.Contains(engine.Name));
            Debug.Assert.IsTrue(this.context == engine.TextContext);

            this.layouts.Remove(engine.Name);

            engine.Dispose();
        }

        private Text.TextContext context;
        private System.Collections.Hashtable layouts;
    }
}
