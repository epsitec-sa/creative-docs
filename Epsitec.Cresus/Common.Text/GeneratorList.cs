//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
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
		
		
		
		private Text.TextContext				context;
		private System.Collections.Hashtable	generators;
	}
}
