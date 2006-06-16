//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Types
{
	public class Caption : DependencyObject
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
			context.StoreObjectData (0, this);

			xmlWriter.WriteEndElement ();
			xmlWriter.Flush ();
			xmlWriter.Close ();

			string xml = buffer.ToString ();

			string dataElementPrefix = string.Concat (@"<", Serialization.IO.Xml.StructurePrefix, @":data id=""_0"" ");
			string dataElementSuffix = @" /></xml>";

			int prefixPos = xml.IndexOf (dataElementPrefix);
			int suffixPos = xml.IndexOf (dataElementSuffix);

			System.Diagnostics.Debug.Assert (prefixPos > 0);
			System.Diagnostics.Debug.Assert (suffixPos > 0);

			prefixPos += dataElementPrefix.Length;

			return xml.Substring (prefixPos, suffixPos - prefixPos);
		}

		/// <summary>
		/// Deserializes the caption object from a string representation.
		/// </summary>
		/// <param name="value">The serialized caption.</param>
		public void DeserializeFromString(string value)
		{
			value = string.Concat (@"<xml xmlns:", Serialization.IO.Xml.StructurePrefix, @"=""", Serialization.IO.Xml.StructureNamespace, @""" ",
				/**/			   @"xmlns:", Serialization.IO.Xml.FieldsPrefix, @"=""", Serialization.IO.Xml.FieldsNamespace, @""">",
				/**/			   @"<", Serialization.IO.Xml.StructurePrefix, @":data id=""_0"" ", value, @" />",
				/**/			   @"</xml>");

			System.IO.StringReader stringReader = new System.IO.StringReader (value);
			System.Xml.XmlTextReader xmlReader = new System.Xml.XmlTextReader (stringReader);

			while (xmlReader.Read ())
			{
				if ((xmlReader.NodeType == System.Xml.XmlNodeType.Element) &&
					(xmlReader.LocalName == "xml"))
				{
					break;
				}
			}

			Serialization.Context context = new Serialization.DeserializerContext (new Serialization.IO.XmlReader (xmlReader));

			this.labels = null;
			this.sortedLabels = null;
			
			this.ClearAllValues ();
			
			context.RestoreObjectData (0, this);
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
