//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text.Properties
{
	/// <summary>
	/// La classe ManagedInfoProperty associe à un paragraphe marqué par une
	/// propriété ManagedParagraphProperty une information spécifique utilisée
	/// pour modifier le fonctionnement du générateur.
	/// </summary>
	public class ManagedInfoProperty : Property
	{
		public ManagedInfoProperty()
		{
		}
		
		public ManagedInfoProperty(string manager_name, string manager_info)
		{
			this.manager_name = manager_name;
			this.manager_info = manager_info;
		}
		
		
		public override WellKnownType			WellKnownType
		{
			get
			{
				return WellKnownType.ManagedInfo;
			}
		}
		
		public override PropertyType			PropertyType
		{
			get
			{
				return PropertyType.ExtraSetting;
			}
		}
		
		public override CombinationMode			CombinationMode
		{
			get
			{
				return CombinationMode.Accumulate;
			}
		}
		
		public override bool					RequiresUniformParagraph
		{
			get
			{
				return true;
			}
		}
		
		
		public string							ManagerName
		{
			get
			{
				return this.manager_name;
			}
		}
		
		public string							ManagerInfo
		{
			get
			{
				return this.manager_info;
			}
		}
		
		
		public override Property EmptyClone()
		{
			return new ManagedInfoProperty ();
		}
		
		public override void SerializeToText(System.Text.StringBuilder buffer)
		{
			SerializerSupport.Join (buffer,
				/**/				SerializerSupport.SerializeString (this.manager_name),
				/**/				SerializerSupport.SerializeString (this.manager_info));
		}

		public override void DeserializeFromText(TextContext context, string text, int pos, int length)
		{
			string[] args = SerializerSupport.Split (text, pos, length);
			
			Debug.Assert.IsTrue (args.Length == 2);
			
			string manager_name = SerializerSupport.DeserializeString (args[0]);
			string manager_info = SerializerSupport.DeserializeString (args[1]);
			
			this.manager_name = manager_name;
			this.manager_info = manager_info;
		}
		
		
		public override Property GetCombination(Property property)
		{
			throw new System.NotImplementedException ();
		}

		
		public override void UpdateContentsSignature(IO.IChecksum checksum)
		{
			checksum.UpdateValue (this.manager_name);
			checksum.UpdateValue (this.manager_info);
		}
		
		public override bool CompareEqualContents(object value)
		{
			return ManagedInfoProperty.CompareEqualContents (this, value as ManagedInfoProperty);
		}
		
		
		public static ManagedInfoProperty Find(Property[] properties, string name)
		{
			foreach (Property property in properties)
			{
				if (property.WellKnownType == WellKnownType.ManagedInfo)
				{
					ManagedInfoProperty managed = property as ManagedInfoProperty;
					
					if ((name == null) ||
						(managed.ManagerName == name))
					{
						return managed;
					}
				}
			}
			
			return null;
		}
		
		public static ManagedInfoProperty[] Filter(System.Collections.ICollection properties)
		{
			int count = 0;
			
			foreach (Property property in properties)
			{
				if (property is ManagedInfoProperty)
				{
					count++;
				}
			}
			
			ManagedInfoProperty[] filtered = new ManagedInfoProperty[count];
			
			int index = 0;
			
			foreach (Property property in properties)
			{
				if (property is ManagedInfoProperty)
				{
					filtered[index++] = property as ManagedInfoProperty;
				}
			}
			
			System.Diagnostics.Debug.Assert (index == count);
			
			return filtered;
		}
		
		
		private static bool CompareEqualContents(ManagedInfoProperty a, ManagedInfoProperty b)
		{
			return a.manager_name == b.manager_name
				&& a.manager_info == b.manager_info;
		}
		
		
		private string							manager_name;
		private string							manager_info;
	}
}
