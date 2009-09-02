//	Copyright © 2005-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text
{
	using EventHandler = Epsitec.Common.Support.EventHandler;
	
	/// <summary>
	/// La classe GeneratorList gère la liste des générateurs, accessibles par
	/// leur nom.
	/// </summary>
	public sealed class GeneratorList
	{
		public GeneratorList(Text.TextContext context)
		{
			this.context    = context;
			this.generators = new System.Collections.Hashtable ();
		}
		
		
		public Generator						this[string name]
		{
			get
			{
				return this.generators[name] as Generator;
			}
		}
		
		public Generator						this[Properties.GeneratorProperty property]
		{
			get
			{
				return property == null ? null : this[property.Generator];
			}
		}
		
		
		public Generator NewTemporaryGenerator(Generator model)
		{
			Generator generator = new Generator (null);
			
			generator.Restore (this.context, model.Save ());
			generator.DefineName ("*");
			
			return generator;
		}
		
		
		public Generator NewGenerator()
		{
			string name = this.context.StyleList.GenerateUniqueName ();
			
			return this.NewGenerator (name);
		}
		
		public Generator NewGenerator(string name)
		{
			System.Diagnostics.Debug.Assert (this.generators.Contains (name) == false);
			System.Diagnostics.Debug.Assert (name != null);
			System.Diagnostics.Debug.Assert (name.Length > 0);
			System.Diagnostics.Debug.Assert (name != "*");
			
			Generator generator = new Generator (name);
			
			this.generators[name] = generator;
			this.NotifyChanged (generator);
			
			return generator;
		}
		
		public void RedefineGenerator(Common.Support.OpletQueue queue, Generator generator, Generator model)
		{
			System.Diagnostics.Debug.Assert (this.generators.Contains (generator.Name));
			
			if (queue != null)
			{
				System.Diagnostics.Debug.Assert (queue.IsActionDefinitionInProgress);
				TextStory.InsertOplet (queue, new RedefineOplet (this, generator));
			}
			
			string name = generator.Name;
			generator.Restore (this.context, model.Save ());
			generator.DefineName (name);
			this.NotifyChanged (generator);
		}
		
		public void DisposeGenerator(Generator generator)
		{
			System.Diagnostics.Debug.Assert (this.generators.Contains (generator.Name));
			
			this.generators.Remove (generator.Name);
		}
		
		
		public void CloneGenerators(Property[] properties)
		{
			for (int i = 0; i < properties.Length; i++)
			{
				if (properties[i].WellKnownType == Properties.WellKnownType.ManagedParagraph)
				{
					Properties.ManagedParagraphProperty mpp = properties[i] as Properties.ManagedParagraphProperty;
					string   managerName = mpp.ManagerName;
					string[] parameters   = mpp.ManagerParameters;
					
					switch (managerName)
					{
						case "ItemList":
							ParagraphManagers.ItemListManager.Parameters p = new ParagraphManagers.ItemListManager.Parameters (this.context, parameters);
							p.Generator = this.CloneGenerator (p.Generator);
							parameters = p.Save ();
							break;
					}
					
					properties[i] = new Properties.ManagedParagraphProperty (managerName, parameters);
					this.NotifyChanged (null);
				}
			}
		}
		
		public Generator CloneGenerator(Generator model)
		{
			Generator generator = this.NewGenerator ();
			this.RedefineGenerator (null, generator, model);
			System.Diagnostics.Debug.WriteLine (string.Format ("Cloned Generator '{0}' to '{1}'", model.Name, generator.Name));
			return generator;
		}
		
		
		public void Serialize(System.Text.StringBuilder buffer)
		{
			//	Sérialise toutes les définitions des générateurs :
			
			buffer.Append (SerializerSupport.SerializeInt (this.generators.Count));
			
			foreach (Generator generator in this.generators.Values)
			{
				buffer.Append ("/");
				generator.Serialize (buffer);
			}
		}
		
		public void Deserialize(TextContext context, int version, string[] args, ref int offset)
		{
			int count = SerializerSupport.DeserializeInt (args[offset++]);
			
			for (int i = 0; i < count; i++)
			{
				Generator generator = new Generator (null);
				
				generator.Deserialize (context, version, args, ref offset);
				
				string name = generator.Name;
				
				this.generators[name] = generator;
			}
		}
		
		
		public void IncrementUserCount(string name)
		{
			System.Diagnostics.Debug.Assert (this.generators.Contains (name));
			
			Generator generator = this.generators[name] as Generator;
			generator.IncrementUserCount ();
		}
		
		public void DecrementUserCount(string name)
		{
			System.Diagnostics.Debug.Assert (this.generators.Contains (name));
			
			Generator generator = this.generators[name] as Generator;
			generator.DecrementUserCount ();
		}
		
		public void ClearUnusedGenerators()
		{
			string[] names = new string[this.generators.Count];
			this.generators.Keys.CopyTo (names, 0);
			
			foreach (string name in names)
			{
				Generator generator = this.generators[name] as Generator;
				
				System.Diagnostics.Debug.WriteLine (string.Format ("Generator '{0}' used {1} times.", name, generator.UserCount));
				
				if (generator.UserCount == 0)
				{
					this.DisposeGenerator (generator);
				}
			}
		}
		
		
		#region RedefineOplet Class
		public class RedefineOplet : Common.Support.AbstractOplet
		{
			public RedefineOplet(GeneratorList list, Generator generator)
			{
				this.list      = list;
				this.generator = generator;
				this.state     = this.generator.Save ();
			}
			
			
			public override Common.Support.IOplet Undo()
			{
				string newState = this.generator.Save ();
				string oldState = this.state;
				
				this.state = newState;
				
				this.generator.Restore (this.list.context, oldState);
				this.list.NotifyChanged (this.generator);
				
				return this;
			}
			
			public override Common.Support.IOplet Redo()
			{
				return this.Undo ();
			}
			
			public override void Dispose()
			{
				base.Dispose ();
			}
			
			
			public bool MergeWith(RedefineOplet other)
			{
				if ((this.generator == other.generator) &&
					(this.list == other.list))
				{
					return true;
				}
				
				return false;
			}
						
			
			private GeneratorList				list;
			private Generator					generator;
			private string						state;
		}
		#endregion
		
		private void NotifyChanged(Generator generator)
		{
			if (this.Changed != null)
			{
				this.Changed (this);
			}
		}
		
		
		public event EventHandler				Changed;
		
		private Text.TextContext				context;
		private System.Collections.Hashtable	generators;
	}
}
