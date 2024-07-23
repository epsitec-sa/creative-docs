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
using System.Xml.Linq;
using Epsitec.Common.Support.Serialization;

namespace Epsitec.Common.Text
{
    using EventHandler = Epsitec.Common.Support.EventHandler;

    /// <summary>
    /// La classe GeneratorList gère la liste des générateurs, accessibles par
    /// leur nom.
    /// </summary>
    public sealed class GeneratorList : Common.Support.IXMLSerializable<GeneratorList>
    {
        public GeneratorList(Text.TextContext context)
        {
            this.context = context;
            this.generators = new();
        }

        public Generator this[string name]
        {
            get { return this.generators[name]; }
        }

        public Generator this[Properties.GeneratorProperty property]
        {
            get { return property == null ? null : this[property.Generator]; }
        }

        public Generator NewTemporaryGenerator(Generator model)
        {
            Generator generator = new Generator(null);

            generator.Restore(this.context, model.Save());
            generator.DefineName("*");

            return generator;
        }

        public Generator NewGenerator()
        {
            string name = this.context.StyleList.GenerateUniqueName();

            return this.NewGenerator(name);
        }

        public Generator NewGenerator(string name)
        {
            System.Diagnostics.Debug.Assert(this.generators.ContainsKey(name) == false);
            System.Diagnostics.Debug.Assert(name != null);
            System.Diagnostics.Debug.Assert(name.Length > 0);
            System.Diagnostics.Debug.Assert(name != "*");

            Generator generator = new Generator(name);

            this.generators[name] = generator;
            this.NotifyChanged(generator);

            return generator;
        }

        public void RedefineGenerator(
            Common.Support.OpletQueue queue,
            Generator generator,
            Generator model
        )
        {
            System.Diagnostics.Debug.Assert(this.generators.ContainsKey(generator.Name));

            if (queue != null)
            {
                System.Diagnostics.Debug.Assert(queue.IsActionDefinitionInProgress);
                TextStory.InsertOplet(queue, new RedefineOplet(this, generator));
            }

            string name = generator.Name;
            generator.Restore(this.context, model.Save());
            generator.DefineName(name);
            this.NotifyChanged(generator);
        }

        public void DisposeGenerator(Generator generator)
        {
            System.Diagnostics.Debug.Assert(this.generators.ContainsKey(generator.Name));

            this.generators.Remove(generator.Name);
        }

        public void CloneGenerators(Property[] properties)
        {
            for (int i = 0; i < properties.Length; i++)
            {
                if (properties[i].WellKnownType == Properties.WellKnownType.ManagedParagraph)
                {
                    Properties.ManagedParagraphProperty mpp =
                        properties[i] as Properties.ManagedParagraphProperty;
                    string managerName = mpp.ManagerName;
                    string[] parameters = mpp.ManagerParameters;

                    switch (managerName)
                    {
                        case "ItemList":
                            ParagraphManagers.ItemListManager.Parameters p =
                                new ParagraphManagers.ItemListManager.Parameters(
                                    this.context,
                                    parameters
                                );
                            p.Generator = this.CloneGenerator(p.Generator);
                            parameters = p.Save();
                            break;
                    }

                    properties[i] = new Properties.ManagedParagraphProperty(
                        managerName,
                        parameters
                    );
                    this.NotifyChanged(null);
                }
            }
        }

        public Generator CloneGenerator(Generator model)
        {
            Generator generator = this.NewGenerator();
            this.RedefineGenerator(null, generator, model);
            System.Diagnostics.Debug.WriteLine(
                string.Format("Cloned Generator '{0}' to '{1}'", model.Name, generator.Name)
            );
            return generator;
        }

        public void Serialize(System.Text.StringBuilder buffer)
        {
            //	Sérialise toutes les définitions des générateurs :

            buffer.Append(SerializerSupport.SerializeInt(this.generators.Count));

            foreach (Generator generator in this.generators.Values)
            {
                buffer.Append("/");
                generator.Serialize(buffer);
            }
        }

        public bool HasEquivalentData(Common.Support.IXMLWritable otherWritable)
        {
            GeneratorList other = (GeneratorList)otherWritable;
            return this.generators.Values.HasEquivalentData(other.generators.Values);
        }

        public XElement ToXML()
        {
            return new XElement(
                "GeneratorList",
                new XElement("Generators", this.generators.Values.Select(tab => tab.ToXML()))
            );
        }

        public static GeneratorList FromXML(XElement xml)
        {
            return new GeneratorList(xml);
        }

        private GeneratorList(XElement xml)
        {
            this.generators = xml.Element("Generators")
                .Elements()
                .Select(item =>
                {
                    Generator gen = Generator.FromXML(item);
                    return (gen.Name, gen);
                })
                .ToDictionary();
        }

        public void Deserialize(TextContext context, int version, string[] args, ref int offset)
        {
            int count = SerializerSupport.DeserializeInt(args[offset++]);

            for (int i = 0; i < count; i++)
            {
                Generator generator = new Generator(null);

                generator.Deserialize(context, version, args, ref offset);

                string name = generator.Name;

                this.generators[name] = generator;
            }
        }

        public void IncrementUserCount(string name)
        {
            System.Diagnostics.Debug.Assert(this.generators.ContainsKey(name));

            Generator generator = this.generators[name];
            generator.IncrementUserCount();
        }

        public void DecrementUserCount(string name)
        {
            System.Diagnostics.Debug.Assert(this.generators.ContainsKey(name));

            Generator generator = this.generators[name];
            generator.DecrementUserCount();
        }

        public void ClearUnusedGenerators()
        {
            string[] names = new string[this.generators.Count];
            this.generators.Keys.CopyTo(names, 0);

            foreach (string name in names)
            {
                Generator generator = this.generators[name];

                System.Diagnostics.Debug.WriteLine(
                    string.Format("Generator '{0}' used {1} times.", name, generator.UserCount)
                );

                if (generator.UserCount == 0)
                {
                    this.DisposeGenerator(generator);
                }
            }
        }

        #region RedefineOplet Class
        public class RedefineOplet : Common.Support.AbstractOplet
        {
            public RedefineOplet(GeneratorList list, Generator generator)
            {
                this.list = list;
                this.generator = generator;
                this.state = this.generator.Save();
            }

            public override Common.Support.IOplet Undo()
            {
                string newState = this.generator.Save();
                string oldState = this.state;

                this.state = newState;

                this.generator.Restore(this.list.context, oldState);
                this.list.NotifyChanged(this.generator);

                return this;
            }

            public override Common.Support.IOplet Redo()
            {
                return this.Undo();
            }

            public override void Dispose()
            {
                base.Dispose();
            }

            public bool MergeWith(RedefineOplet other)
            {
                if ((this.generator == other.generator) && (this.list == other.list))
                {
                    return true;
                }

                return false;
            }

            private GeneratorList list;
            private Generator generator;
            private string state;
        }
        #endregion

        private void NotifyChanged(Generator generator)
        {
            if (this.Changed != null)
            {
                this.Changed(this);
            }
        }

        public event EventHandler Changed;

        private Text.TextContext context;
        private Dictionary<string, Generator> generators;
    }
}
