//	Copyright © 2006-2011, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

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
		/// <summary>
		/// Initializes a new instance of the <see cref="Caption"/> class.
		/// </summary>
		public Caption()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Caption"/> class.
		/// </summary>
		/// <param name="id">The id associated with this caption.</param>
		public Caption(Support.Druid id) : this ()
		{
			this.id = id;
		}

		/// <summary>
		/// Gets a value indicating whether this caption has labels.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this caption has labels; otherwise, <c>false</c>.
		/// </value>
		public bool								HasLabels
		{
			get
			{
				return this.labels == null ? false : (this.labels.Count > 0);
			}
		}

		/// <summary>
		/// Gets a value indicating whether this caption has a description.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this caption has a description; otherwise, <c>false</c>.
		/// </value>
		public bool								HasDescription
		{
			get
			{
				return this.ContainsValue (Caption.DescriptionProperty)
					&& !string.IsNullOrEmpty (this.Description);
			}
		}

		/// <summary>
		/// Gets a value indicating whether this caption has an icon.
		/// </summary>
		/// <value><c>true</c> if this caption has an icon; otherwise, <c>false</c>.</value>
		public bool								HasIcon
		{
			get
			{
				return this.ContainsValue (Caption.IconProperty)
					&& !string.IsNullOrEmpty (this.Icon);
			}
		}

		/// <summary>
		/// Gets the label collection.
		/// </summary>
		/// <value>The labels.</value>
		public IList<string>					Labels
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

		/// <summary>
		/// Gets the sorted label collection. The first label returned is the
		/// shortest.
		/// </summary>
		/// <value>The sorted labels.</value>
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

		/// <summary>
		/// Gets or sets the description of the caption.
		/// </summary>
		/// <value>The description.</value>
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

		/// <summary>
		/// Gets or sets the name of the caption.
		/// </summary>
		/// <value>The name.</value>
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

		/// <summary>
		/// Gets the id associated with the caption.
		/// </summary>
		/// <value>The id.</value>
		public Support.Druid					Id
		{
			get
			{
				return this.id;
			}
		}

		/// <summary>
		/// Gets or sets the icon of the caption.
		/// </summary>
		/// <value>The icon.</value>
		public string							Icon
		{
			get
			{
				return (string) this.GetValue (Caption.IconProperty);
			}
			set
			{
				if (value == null)
				{
					this.ClearValue (Caption.IconProperty);
				}
				else
				{
					this.SetValue (Caption.IconProperty, value);
				}
			}
		}

		/// <summary>
		/// Gets the default label.
		/// </summary>
		/// <value>The default label or an empty string.</value>
		public string							DefaultLabel
		{
			get
			{
				if ((this.labels == null) ||
					(this.labels.Count == 0))
				{
					return "";
				}
				else
				{
					return this.labels[0];
				}
			}
		}


		/// <summary>
		/// Defines the ID associated with the caption. The ID can be defined
		/// only once; redefining it throws an <see cref="T:InvalidOperationException"/>.
		/// </summary>
		/// <param name="id">The caption id.</param>
		public void DefineId(Support.Druid id)
		{
			if (this.id == id)
			{
				return;
			}
			
			if (this.id.IsValid)
			{
				throw new System.InvalidOperationException ("The id cannot be changed");
			}
			
			this.id = id;
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

		/// <summary>
		/// Deserializes the caption object from a string representation.
		/// </summary>
		/// <param name="value">The serialized caption.</param>
		public void DeserializeFromString(string value)
		{
			this.DeserializeFromString (value, Support.Resources.DefaultManager);
		}

		/// <summary>
		/// Deserializes the caption object from a string representation.
		/// </summary>
		/// <param name="value">The serialized caption.</param>
		/// <param name="manager">The resource manager.</param>
		public void DeserializeFromString(string value, Support.ResourceManager manager)
		{
			if (Support.ResourceBundle.Field.IsNullOrEmptyString (value))
			{
				return;
			}

			value = Caption.DecompressXml (value);
			
			if (value.StartsWith ("<" + Serialization.SimpleSerialization.RootElementName))
			{
				//	The caller has provided us with a fullly serialized XML stream.
				//	Deserializing is easy: restore the object and copy its fields.

				Caption caption = Serialization.SimpleSerialization.DeserializeFromString (value, manager) as Caption;

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
				context.ExternalMap.Record (Serialization.Context.WellKnownTagResourceManager, manager);

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

			AbstractType.BindComplexTypeToCaption (this);
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
			return Caption.Merge (a, b, false);
		}

		/// <summary>
		/// Merges the two captions and returns the result. If caption <c>a</c>
		/// and caption <c>b</c> both define the same properties, those of <c>b</c>
		/// will take precedence.
		/// </summary>
		/// <param name="a">The first caption object.</param>
		/// <param name="b">The second caption object.</param>
		/// <param name="sameId">If set to <c>true</c>, both captions should have the same id.</param>
		/// <returns>The merged caption object.</returns>
		public static Caption Merge(Caption a, Caption b, bool sameId)
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
				caption.DefineId (a.Id);
				DependencyObject.CopyDefinedProperties (a, caption);
			}
			if ((b != null) &&
				(a != b))
			{
				if (caption.Id.IsEmpty)
				{
					caption.DefineId (b.Id);
				}
				else if (sameId)
				{
					System.Diagnostics.Debug.Assert (caption.Id == b.Id);
				}
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

		public static string CompressXml(string xml)
		{
			//	Fold known strings found in typical Caption serializations to
			//	unambiguous shorter representations :

			xml = xml.Replace (@"s:name=""Epsitec.Common.Types.AbstractType""", @"s:name=""*aT""");
			xml = xml.Replace (@"s:name=""Epsitec.Common.Types.EnumType""", @"s:name=""*eT""");
			xml = xml.Replace (@"s:name=""Epsitec.Common.Types.AbstractNumericType""", @"s:name=""*aNT""");
			xml = xml.Replace (@"s:name=""Epsitec.Common.Types.AbstractDateTimeType""", @"s:name=""*aDTT""");
			xml = xml.Replace (@"s:name=""Epsitec.Common.Types.Caption""", @"s:name=""*C""");
			xml = xml.Replace (@"s:name=""Epsitec.Common.Types.StructuredType""", @"s:name=""*S""");
			xml = xml.Replace (@"xmlns:s=""http://www.epsitec.ch/XNS/storage-structure-1""", @"xmlns:s=""*1""");
			xml = xml.Replace (@"xmlns:f=""http://www.epsitec.ch/XNS/storage-fields-1""", @"xmlns:f=""*1""");
			xml = xml.Replace (@"SystemType=""E:Epsitec.Common.Types.NotAnEnum, Common""", @"SystemType=""*NaE""");

			return xml;
		}

		public static string DecompressXml(string xml)
		{
			//	Restores the XML to what it was before the call to CompressXml.

			xml = xml.Replace (@"s:name=""*aT""", @"s:name=""Epsitec.Common.Types.AbstractType""");
			xml = xml.Replace (@"s:name=""*eT""", @"s:name=""Epsitec.Common.Types.EnumType""");
			xml = xml.Replace (@"s:name=""*aNT""", @"s:name=""Epsitec.Common.Types.AbstractNumericType""");
			xml = xml.Replace (@"s:name=""*aDTT""", @"s:name=""Epsitec.Common.Types.AbstractDateTimeType""");
			xml = xml.Replace (@"s:name=""*C""", @"s:name=""Epsitec.Common.Types.Caption""");
			xml = xml.Replace (@"s:name=""*S""", @"s:name=""Epsitec.Common.Types.StructuredType""");
			xml = xml.Replace (@"xmlns:s=""*1""", @"xmlns:s=""http://www.epsitec.ch/XNS/storage-structure-1""");
			xml = xml.Replace (@"xmlns:f=""*1""", @"xmlns:f=""http://www.epsitec.ch/XNS/storage-fields-1""");
			xml = xml.Replace (@"SystemType=""*NaE""", @"SystemType=""E:Epsitec.Common.Types.NotAnEnum, Common""");

			return xml;
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

			private DependencyObject			root;
			private int							level;
			private int							typeCount;
			private bool						requiresRichSerialization;
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

			protected override DependencyPropertyMetadata CloneNewObject()
			{
				return new CaptionMetadata ();
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
		private Support.Druid id;
		private int suspendCounter;
		private bool hasChanged;
	}
}
