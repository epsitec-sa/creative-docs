//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

[assembly: Epsitec.Common.Types.DependencyClass (typeof (Epsitec.Common.Types.Caption))]

namespace Epsitec.Common.Types
{
	public sealed class Caption : DependencyObject
	{
		public Caption()
		{
		}

		public Caption(string id) : this ()
		{
			this.id = id;
		}

		public ICollection<string>				Labels
		{
			get
			{
				if (this.labels == null)
				{
					this.labels = new Collections.HostedList<string> (this.HandleLabelInsertion, this.HandleLabelRemoval);
				}
				
				return this.labels;
			}
		}
		
		public IEnumerable<string>				SortedLabels
		{
			get
			{
				if (this.sortedLabels == null)
				{
					this.RefreshSortedLabels ();
				}
				
				return this.sortedLabels;
			}
		}

		public string							Description
		{
			get
			{
				return (string) this.GetValue (Caption.DescriptionProperty);
			}
			set
			{
				this.SetValue (Caption.DescriptionProperty, value);
			}
		}
		
		public string							Name
		{
			get
			{
				return (string) this.GetValue (Caption.NameProperty);
			}
			set
			{
				if (value == null)
				{
					this.ClearValue (Caption.NameProperty);
				}
				else
				{
					this.SetValue (Caption.NameProperty, value);
				}
			}
		}
		
		public string							Id
		{
			get
			{
				return this.id;
			}
		}

		public string							Icon
		{
			get
			{
				return (string) this.GetValue (Caption.IconProperty);
			}
			set
			{
				this.SetValue (Caption.IconProperty, value);
			}
		}


		/// <summary>
		/// Defines the ID associated with the caption. The ID can be defined
		/// only once; redefining it throws an <see cref="T:InvalidOperationException"/>.
		/// </summary>
		/// <param name="id">The caption ID (usually a DRUID representation).</param>
		public void DefineId(string id)
		{
			if (this.id == id)
			{
				return;
			}
			
			if (this.id != null)
			{
				throw new System.InvalidOperationException ("The id cannot be changed");
			}
			
			this.id = id;
		}

		private class Visitor : Serialization.IVisitor
		{
			public Visitor(DependencyObject root)
			{
				this.root = root;
			}
			
			public int TypeCount
			{
				get
				{
					return this.typeCount;
				}
			}

			public bool Complex
			{
				get
				{
					return this.complex;
				}
			}
			
			public void VisitNodeBegin(Serialization.Context context, DependencyObject obj)
			{
				if (this.root == obj)
				{
					context.ExternalMap.Record ("caption", this.root);
					System.Diagnostics.Debug.Assert (this.level == 0);
				}
				else
				{
					this.complex = true;
				}
				
				this.level++;
			}

			public void VisitNodeEnd(Serialization.Context context, DependencyObject obj)
			{
				this.level--;
				
				if (this.root == obj)
				{
					System.Diagnostics.Debug.Assert (this.level == 0);
				}
			}

			public void VisitAttached(Serialization.Context context, PropertyValuePair entry)
			{
				this.RecordType (context, entry.Property.OwnerType);
			}

			public void VisitUnknown(Serialization.Context context, object obj)
			{
				throw new System.InvalidOperationException ("Unknown object found");
			}

			private void RecordType(Serialization.Context context, System.Type type)
			{
				if (context.ObjectMap.IsTypeDefined (type) == false)
				{
					this.typeCount++;
					context.ObjectMap.RecordType (type);
					context.StoreTypeDefinition (this.typeCount, type);
				}
			}

			DependencyObject root;
			int level;
			int typeCount;
			bool complex;
		}
		
		/// <summary>
		/// Serializes the caption object to a string representation.
		/// </summary>
		/// <returns>The serialized caption.</returns>
		public string SerializeToString()
		{
			System.Text.StringBuilder buffer = new System.Text.StringBuilder ();
			System.IO.StringWriter stringWriter = new System.IO.StringWriter (buffer);
			System.Xml.XmlTextWriter xmlWriter = new System.Xml.XmlTextWriter (stringWriter);
			Serialization.IO.AbstractWriter writer = new Serialization.IO.XmlWriter (xmlWriter);

			xmlWriter.Formatting = System.Xml.Formatting.None;
			xmlWriter.WriteStartElement ("xml");

			writer.WriteAttributeStrings ();

			Serialization.Context context = new Serialization.SerializerContext (writer);

			Visitor visitor = new Visitor (this);
			Serialization.GraphVisitor.VisitSerializableNodes (this, context, visitor);

			if (visitor.Complex)
			{
				return Serialization.SimpleSerialization.SerializeToString (this);
			}
			
			context.StoreObjectData (0, this);

			xmlWriter.WriteEndElement ();
			xmlWriter.Flush ();
			xmlWriter.Close ();

			string xml = buffer.ToString ();

			string typeElementPrefix = string.Concat (@"<", Serialization.IO.Xml.StructurePrefix, @":type ");
			string dataElementPrefix = string.Concat (@"<", Serialization.IO.Xml.StructurePrefix, @":data ");
			string endElement = @"</xml>";

			int typeElementPos = xml.IndexOf (typeElementPrefix);
			int dataElementPos = xml.IndexOf (dataElementPrefix);
			int endElementPos   = xml.IndexOf (endElement);
			int startElementPos = typeElementPos > 0 ? typeElementPos : dataElementPos;

			System.Diagnostics.Debug.Assert (startElementPos > 0);
			System.Diagnostics.Debug.Assert (endElementPos > 0);

			return xml.Substring (startElementPos, endElementPos - startElementPos);
		}


		/// <summary>
		/// Deserializes the caption object from a string representation.
		/// </summary>
		/// <param name="value">The serialized caption.</param>
		public void DeserializeFromString(string value)
		{
			if ((value.Length > 1) &&
				(value[0] == '<') &&
				(value.IndexOf (Serialization.SimpleSerialization.RootElementName, 1, Serialization.SimpleSerialization.RootElementName.Length+1) == 1))
			{
				Caption caption = Serialization.SimpleSerialization.DeserializeFromString (value) as Caption;
				
				DependencyObject.CopyDefinedProperties (caption, this);
				return;
			}
			
			System.Text.StringBuilder buffer = new System.Text.StringBuilder ();
			
			buffer.Append (@"<xml ");
			buffer.Append (@"xmlns:");
			buffer.Append (Serialization.IO.Xml.StructurePrefix);
			buffer.Append (@"=""");
			buffer.Append (Serialization.IO.Xml.StructureNamespace);
			buffer.Append (@""" ");
			buffer.Append (@"xmlns:");
			buffer.Append (Serialization.IO.Xml.FieldsPrefix);
			buffer.Append (@"=""");
			buffer.Append (Serialization.IO.Xml.FieldsNamespace);
			buffer.Append (@""">");
			buffer.Append (value);
			buffer.Append (@"</xml>");

			string typeElementPrefix = string.Concat (@"<", Serialization.IO.Xml.StructurePrefix, @":type ");
			string xml = buffer.ToString ();
			
			System.IO.StringReader stringReader = new System.IO.StringReader (xml);
			System.Xml.XmlTextReader xmlReader = new System.Xml.XmlTextReader (stringReader);

			Serialization.Context context = new Serialization.DeserializerContext (new Serialization.IO.XmlReader (xmlReader));
			
			context.ExternalMap.Record ("caption", this);
			
			while (xmlReader.Read ())
			{
				if ((xmlReader.NodeType == System.Xml.XmlNodeType.Element) &&
					(xmlReader.LocalName == "xml"))
				{
					break;
				}
			}

			int pos = value.IndexOf (typeElementPrefix, 0);
			int typeCount = 0;

			while (pos >= 0)
			{
				typeCount++;

				context.ObjectMap.RecordType (context.RestoreTypeDefinition ());
				
				pos = value.IndexOf (typeElementPrefix, pos+typeElementPrefix.Length);
			}
			

			this.labels = null;
			this.sortedLabels = null;
			
			this.ClearAllValues ();
			
			context.RestoreObjectData (0, this);
		}

		/// <summary>
		/// Transforms all the texts used by this object. This will call the
		/// transform callback for every text; the text gets replaced by the
		/// value returned by the callback.
		/// </summary>
		/// <param name="transform">The transform callback.</param>
		public void TransformTexts(Support.TransformCallback<string> transform)
		{
			if (this.labels != null)
			{
				for (int i = 0; i < this.labels.Count; i++)
				{
					string oldText = this.labels[i];
					string newText = transform (oldText);
					
					if (newText != oldText)
					{
						this.labels[i] = newText;
					}
				}
			}

			//	We have to make a temporary copy of the locally defined properties,
			//	since we may not modify the local properties while running through
			//	the enumerator :
			
			List<DependencyProperty> properties = new List<DependencyProperty> ();
			properties.AddRange (this.LocalProperties);

			foreach (DependencyProperty property in properties)
			{
				if (property.PropertyType == typeof (string))
				{
					string oldText = this.GetLocalValue (property) as string;
					string newText = transform (oldText);

					if (newText != oldText)
					{
						this.SetValueBase (property, newText);
					}
				}
			}
		}

		/// <summary>
		/// Merges the two captions and returns the result. If caption <c>a</c>
		/// and caption <c>b</c> both define the same properties, those of <c>b</c>
		/// will take precedence.
		/// </summary>
		/// <param name="a">The first caption object.</param>
		/// <param name="b">The second caption object.</param>
		/// <returns>The merged caption object.</returns>
		public static Caption Merge(Caption a, Caption b)
		{
			if ((a == null) ||
				(b == null))
			{
				throw new System.ArgumentNullException ();
			}
			
			Caption caption = new Caption ();

			DependencyObject.CopyDefinedProperties (a, caption);
			DependencyObject.CopyDefinedProperties (b, caption);
			
			return caption;
		}
		
		private void RefreshSortedLabels()
		{
			string[] labels = this.labels.ToArray ();

			System.Array.Sort (labels, new StringLengthComparer ());
			
			this.sortedLabels = labels;
		}
		
		private void HandleLabelInsertion(string value)
		{
			this.sortedLabels = null;
		}

		private void HandleLabelRemoval(string value)
		{
			this.sortedLabels = null;
		}

		#region StringLengthComparer Class

		private class StringLengthComparer : IComparer<string>
		{
			#region IComparer<string> Members

			public int Compare(string x, string y)
			{
				int lengthX = string.IsNullOrEmpty (x) ? 0 : x.Length;
				int lengthY = string.IsNullOrEmpty (y) ? 0 : y.Length;

				if (lengthX < lengthY)
				{
					return -1;
				}
				else if (lengthX > lengthY)
				{
					return 1;
				}
				else
				{
					return 0;
				}
			}

			#endregion
		}

		#endregion

		private static object GetLabelsValue(DependencyObject o)
		{
			Caption that = (Caption) o;
			return that.Labels;
		}

		public static readonly DependencyProperty NameProperty = DependencyProperty.Register ("Name", typeof (string), typeof (Caption));
		public static readonly DependencyProperty LabelsProperty = DependencyProperty.RegisterReadOnly ("Labels", typeof (ICollection<string>), typeof (Caption), new DependencyPropertyMetadata (Caption.GetLabelsValue).MakeReadOnlySerializable ());
		public static readonly DependencyProperty DescriptionProperty = DependencyProperty.Register ("Description", typeof (string), typeof (Caption));
		public static readonly DependencyProperty IconProperty = DependencyProperty.Register ("Icon", typeof (string), typeof (Caption));

		private Collections.HostedList<string> labels;
		private string[] sortedLabels;
		private string id;
	}
}
