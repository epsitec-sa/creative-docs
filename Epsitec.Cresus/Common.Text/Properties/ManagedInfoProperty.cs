//	Copyright © 2006-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
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
		
		public ManagedInfoProperty(string managerName, string managerInfo)
		{
			this.managerName = managerName;
			this.managerInfo = managerInfo;
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
				return this.managerName;
			}
		}
		
		public string							ManagerInfo
		{
			get
			{
				return this.managerInfo;
			}
		}
		
		
		public override Property EmptyClone()
		{
			return new ManagedInfoProperty ();
		}
		
		public override void SerializeToText(System.Text.StringBuilder buffer)
		{
			SerializerSupport.Join (buffer,
				/**/				SerializerSupport.SerializeString (this.managerName),
				/**/				SerializerSupport.SerializeString (this.managerInfo));
		}

		public override void DeserializeFromText(TextContext context, string text, int pos, int length)
		{
			string[] args = SerializerSupport.Split (text, pos, length);
			
			Debug.Assert.IsTrue (args.Length == 2);
			
			string managerName = SerializerSupport.DeserializeString (args[0]);
			string managerInfo = SerializerSupport.DeserializeString (args[1]);
			
			this.managerName = managerName;
			this.managerInfo = managerInfo;
		}
		
		
		public override Property GetCombination(Property property)
		{
			throw new System.NotImplementedException ();
		}

		
		public override void UpdateContentsSignature(IO.IChecksum checksum)
		{
			checksum.UpdateValue (this.managerName);
			checksum.UpdateValue (this.managerInfo);
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
			return a.managerName == b.managerName
				&& a.managerInfo == b.managerInfo;
		}
		
		
		private string							managerName;
		private string							managerInfo;
	}
}
