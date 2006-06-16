//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Types
{
	public sealed class Caption : DependencyObject
	{
		public Caption()
		{
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
				return (string) this.GetValue (Caption.DecriptionProperty);
			}
			set
			{
				this.SetValue (Caption.DecriptionProperty, value);
			}
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

			xmlWriter.Formatting = System.Xml.Formatting.None;
			xmlWriter.WriteStartElement ("xml");

			Serialization.Context context = new Serialization.SerializerContext (new Serialization.IO.XmlWriter (xmlWriter));
			
			context.ActiveWriter.WriteAttributeStrings ();

			int typeCount = 0;

			foreach (DependencyProperty property in this.AttachedProperties)
			{
				typeCount++;

				context.ObjectMap.RecordType (property.OwnerType);
				context.StoreTypeDefinition (typeCount, property.OwnerType);
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

			foreach (DependencyProperty property in this.LocalProperties)
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

		public static Caption Merge(Caption a, Caption b)
		{
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
		
		public static readonly DependencyProperty LabelsProperty = DependencyProperty.RegisterReadOnly ("Labels", typeof (ICollection<string>), typeof (Caption), new DependencyPropertyMetadata (Caption.GetLabelsValue).MakeReadOnlySerializable ());
		public static readonly DependencyProperty DecriptionProperty = DependencyProperty.Register ("Description", typeof (string), typeof (Caption));

		private Collections.HostedList<string> labels;
		private string[] sortedLabels;
	}
}
