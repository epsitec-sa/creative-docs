//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

[assembly: Epsitec.Common.Types.DependencyClass (typeof (Epsitec.Common.Types.Caption))]

namespace Epsitec.Common.Types
{
	/// <summary>
	/// The <c>Caption</c> class encapsulates several strings as a single instance;
	/// it is used to label widgets, associate a description, an icon, etc. The
	/// <c>Caption</c> class is also used as a serialization support for commands,
	/// types, etc.
	/// </summary>
	public sealed class Caption : DependencyObject
	{
		public Caption()
		{
		}

		public Caption(Support.Druid druid) : this ()
		{
			this.druid = druid;
		}

		public bool								HasLabels
		{
			get
			{
				return this.labels == null ? false : (this.labels.Count > 0);
			}
		}

		public bool								HasDescription
		{
			get
			{
				return this.ContainsValue (Caption.DescriptionProperty)
					&& !string.IsNullOrEmpty (this.Description);
			}
		}

		public bool								HasIcon
		{
			get
			{
				return this.ContainsValue (Caption.IconProperty)
					&& !string.IsNullOrEmpty (this.Icon);
			}
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
		
		public Support.Druid					Druid
		{
			get
			{
				return this.druid;
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
		/// <param name="id">The caption DRUID.</param>
		public void DefineDruid(Support.Druid druid)
		{
			if (this.druid == druid)
			{
				return;
			}
			
			if (this.druid.IsValid)
			{
				throw new System.InvalidOperationException ("The DRUID cannot be changed");
			}
			
			this.druid = druid;
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

			using (Serialization.Context context = new Serialization.SerializerContext (writer))
			{
				Visitor visitor = new Visitor (this);
				Serialization.GraphVisitor.VisitSerializableNodes (this, context, visitor);

				if (visitor.RequiresRichSerialization)
				{
					//	We cannot use the compact serialization since the object graph
					//	contains references to other full-fledged dependency objects;
					//	just use the plain XML serialization instead:

					xmlWriter.Close ();
					stringWriter.Close ();

					return Caption.CompressXml (Serialization.SimpleSerialization.SerializeToString (this));
				}
				else
				{
					//	Simply store the object data (after the optional type definitions
					//	which get generated in case there are some attached properties).

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

					return Caption.CompressXml (xml.Substring (startElementPos, endElementPos - startElementPos));
				}
			}
		}

		private static string CompressXml(string xml)
		{
			xml = xml.Replace (@"s:name=""Epsitec.Common.Types.AbstractType""", @"s:name=""*aT""");
			xml = xml.Replace (@"s:name=""Epsitec.Common.Types.AbstractNumericType""", @"s:name=""*aNT""");
			xml = xml.Replace (@"s:name=""Epsitec.Common.Types.Caption""", @"s:name=""*C""");
			xml = xml.Replace (@"s:name=""Epsitec.Common.Types.StructuredType""", @"s:name=""*S""");
			xml = xml.Replace (@"xmlns:s=""http://www.epsitec.ch/XNS/storage-structure-1""", @"xmlns:s=""*1""");
			xml = xml.Replace (@"xmlns:f=""http://www.epsitec.ch/XNS/storage-fields-1""", @"xmlns:f=""*1""");
			
			return xml;
		}

		private static string DecompressXml(string xml)
		{
			xml = xml.Replace (@"s:name=""*aT""", @"s:name=""Epsitec.Common.Types.AbstractType""");
			xml = xml.Replace (@"s:name=""*aNT""", @"s:name=""Epsitec.Common.Types.AbstractNumericType""");
			xml = xml.Replace (@"s:name=""*C""", @"s:name=""Epsitec.Common.Types.Caption""");
			xml = xml.Replace (@"s:name=""*S""", @"s:name=""Epsitec.Common.Types.StructuredType""");
			xml = xml.Replace (@"xmlns:s=""*1""", @"xmlns:s=""http://www.epsitec.ch/XNS/storage-structure-1""");
			xml = xml.Replace (@"xmlns:f=""*1""", @"xmlns:f=""http://www.epsitec.ch/XNS/storage-fields-1""");

			return xml;
		}

		/// <summary>
		/// Deserializes the caption object from a string representation.
		/// </summary>
		/// <param name="value">The serialized caption.</param>
		public void DeserializeFromString(string value)
		{
			value = Caption.DecompressXml (value);
			
			if (value.StartsWith ("<" + Serialization.SimpleSerialization.RootElementName))
			{
				//	The caller has provided us with a fullly serialized XML stream.
				//	Deserializing is easy: restore the object and copy its fields.

				Caption caption = Serialization.SimpleSerialization.DeserializeFromString (value) as Caption;

				DependencyObject.CopyDefinedProperties (caption, this);
			}
			else
			{
				//	The caller has provided us with a compact serialized XML stream.
				//	Since we stripped information from it in order to reduce its size,
				//	we will have to re-synthesize the full XML first :
				
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
			if ((a == null) &&
				(b == null))
			{
				return null;
			}
			
			Caption caption = new Caption ();

			caption.SuspendChanged ();

			if (a != null)
			{
				DependencyObject.CopyDefinedProperties (a, caption);
			}
			if ((b != null) &&
				(a != b))
			{
				DependencyObject.CopyDefinedProperties (b, caption);
			}

			caption.ResumeChanged ();
			
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
			this.NotifyChanged ();
		}

		private void HandleLabelRemoval(string value)
		{
			this.sortedLabels = null;
			this.NotifyChanged ();
		}

		#region Methods used to handle the Changed event

		protected override void BeginMultiplePropertyChange()
		{
			this.SuspendChanged ();
			base.BeginMultiplePropertyChange ();
		}

		protected override void EndMultiplePropertyChange()
		{
			base.EndMultiplePropertyChange ();
			this.ResumeChanged ();
		}

		private void SuspendChanged()
		{
			System.Threading.Interlocked.Increment (ref this.suspendCounter);
		}

		private void ResumeChanged()
		{
			if (System.Threading.Interlocked.Decrement (ref this.suspendCounter) == 0)
			{
				if (this.hasChanged)
				{
					this.hasChanged = false;

					if (this.Changed != null)
					{
						this.Changed (this);
					}
				}
			}
		}
		
		private void NotifyChanged()
		{
			if (this.suspendCounter > 0)
			{
				this.hasChanged = true;
			}
			
			if (System.Threading.Thread.VolatileRead (ref this.suspendCounter) == 0)
			{
				this.hasChanged = false;
				
				if (this.Changed != null)
				{
					this.Changed (this);
				}
			}
		}

		#endregion

		#region Visitor Class

		private class Visitor : Serialization.IVisitor
		{
			public Visitor(DependencyObject root)
			{
				this.root = root;
			}

			public int							TypeCount
			{
				get
				{
					return this.typeCount;
				}
			}

			public bool							RequiresRichSerialization
			{
				get
				{
					return this.requiresRichSerialization;
				}
			}

			#region IVisitor Members

			void Serialization.IVisitor.VisitNodeBegin(Serialization.Context context, DependencyObject obj)
			{
				if (this.root == obj)
				{
					context.ExternalMap.Record ("caption", this.root);
					System.Diagnostics.Debug.Assert (this.level == 0);
				}
				else
				{
					this.requiresRichSerialization = true;
				}

				this.level++;
			}

			void Serialization.IVisitor.VisitNodeEnd(Serialization.Context context, DependencyObject obj)
			{
				this.level--;

				if (this.root == obj)
				{
					System.Diagnostics.Debug.Assert (this.level == 0);
				}
			}

			void Serialization.IVisitor.VisitAttached(Serialization.Context context, System.Type type)
			{
				this.RecordType (context, type);
			}

			void Serialization.IVisitor.VisitUnknown(Serialization.Context context, object obj)
			{
				throw new System.InvalidOperationException ("Unknown object found");
			}

			#endregion

			private void RecordType(Serialization.Context context, System.Type type)
			{
				if (context.ObjectMap.IsTypeDefined (type) == false)
				{
					this.typeCount++;
					context.ObjectMap.RecordType (type);
					context.StoreTypeDefinition (this.typeCount, type);
				}
			}

			DependencyObject					root;
			int									level;
			int									typeCount;
			bool								requiresRichSerialization;
		}

		#endregion

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

		#region CaptionMetadata Class

		private class CaptionMetadata : DependencyPropertyMetadata
		{
			protected override void OnPropertyInvalidated(DependencyObject sender, object oldValue, object newValue)
			{
				base.OnPropertyInvalidated (sender, oldValue, newValue);

				Caption caption = sender as Caption;
				caption.NotifyChanged ();
			}
		}

		#endregion

		private static object GetLabelsValue(DependencyObject o)
		{
			Caption that = (Caption) o;
			return that.Labels;
		}
		private static void NotifyLabelsChanged(DependencyObject o, object oldValue, object newValue)
		{
			Caption caption = o as Caption;
			caption.NotifyChanged ();
		}

		private static CaptionMetadata CaptionMetadataInstance = new CaptionMetadata ();
		
		public static readonly DependencyProperty NameProperty = DependencyProperty.Register ("Name", typeof (string), typeof (Caption), Caption.CaptionMetadataInstance);
		public static readonly DependencyProperty LabelsProperty = DependencyProperty.RegisterReadOnly ("Labels", typeof (ICollection<string>), typeof (Caption), new DependencyPropertyMetadata (Caption.GetLabelsValue, Caption.NotifyLabelsChanged).MakeReadOnlySerializable ());
		public static readonly DependencyProperty DescriptionProperty = DependencyProperty.Register ("Description", typeof (string), typeof (Caption), Caption.CaptionMetadataInstance);
		public static readonly DependencyProperty IconProperty = DependencyProperty.Register ("Icon", typeof (string), typeof (Caption), Caption.CaptionMetadataInstance);

		/// <summary>
		/// Occurs when the <c>Caption</c> object changes.
		/// </summary>
		public event Support.EventHandler Changed;

		private Collections.HostedList<string> labels;
		private string[] sortedLabels;
		private Support.Druid druid;
		private int suspendCounter;
		private bool hasChanged;
	}
}
