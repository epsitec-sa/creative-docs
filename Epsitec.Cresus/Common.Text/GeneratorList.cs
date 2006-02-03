//	Copyright © 2005-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text
{
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
		
		
		public Generator NewGenerator()
		{
			string name = this.context.StyleList.GenerateUniqueName ();
			
			return this.NewGenerator (name);
		}
		
		public Generator NewGenerator(string name)
		{
			System.Diagnostics.Debug.Assert (this.generators.Contains (name) == false);
			
			Generator generator = new Generator (name);
			
			this.generators[name] = generator;
			
			return generator;
		}
		
		public void DisposeGenerator(Generator generator)
		{
			System.Diagnostics.Debug.Assert (this.generators.Contains (generator.Name));
			
			this.generators.Remove (generator.Name);
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
		
		
		private Text.TextContext				context;
		private System.Collections.Hashtable	generators;
	}
}
