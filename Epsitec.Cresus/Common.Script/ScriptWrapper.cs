//	Copyright � 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Script
{
	/// <summary>
	/// Summary description for ScriptWrapper.
	/// </summary>
	public class ScriptWrapper : Support.ICommandDispatcher
	{
		public ScriptWrapper()
		{
			this.info_store = new Helpers.ParameterInfoStore ();
			this.info_store.IncludeVoidType ();
		}
		
		
		public Source							Source
		{
			get
			{
				return this.source;
			}
			set
			{
				if (this.source != value)
				{
					this.source = value;
					
					if (this.script != null)
					{
						this.script.Dispose ();
						this.script = null;
					}
				}
			}
		}
		
		public Script							Script
		{
			get
			{
				if ((this.script == null) &&
					(this.source != null))
				{
					Engine engine = new Engine ();
					
					//	Compile le script � partir de sa version "source", � la vol�e :
					
					this.script = engine.Compile (this.Source);
				}
				
				return this.script;
			}
		}
		
		
		public System.Xml.XmlDocument CreateXmlDocument(bool include_declaration)
		{
			System.Xml.XmlDocument xmldoc  = new System.Xml.XmlDocument ();
			System.Xml.XmlNode     xmlnode = this.CreateXmlNode (xmldoc);
			
			if (include_declaration)
			{
				xmldoc.AppendChild (xmldoc.CreateXmlDeclaration ("1.0", "utf-8", null));
			}
			
			xmldoc.AppendChild (xmlnode);
			
			return xmldoc;
		}
		
		public System.Xml.XmlNode CreateXmlNode(System.Xml.XmlDocument xmldoc)
		{
			System.Xml.XmlElement   script_node = xmldoc.CreateElement ("script");
			System.Xml.XmlAttribute name_attr   = xmldoc.CreateAttribute ("name");
			System.Xml.XmlAttribute about_attr  = xmldoc.CreateAttribute ("about");
			
			name_attr.Value  = this.source.Name;
			about_attr.Value = this.source.About;
			
			if (name_attr.Value != "")
			{
				script_node.Attributes.Append (name_attr);
			}
			if (about_attr.Value != "")
			{
				script_node.Attributes.Append (about_attr);
			}
			
			foreach (Source.Method method in this.source.Methods)
			{
				script_node.AppendChild (this.CreateXmlNode (xmldoc, method));
			}
			
			foreach (Types.IDataValue value in this.source.Values)
			{
				script_node.AppendChild (this.CreateXmlNode (xmldoc, value));
			}
			
			return script_node;
		}
		
		
		public void LoadFromXml(string xml)
		{
			System.Xml.XmlDocument doc = new System.Xml.XmlDocument ();
			
			doc.LoadXml (xml);
			
			this.LoadFromXml (doc.DocumentElement);
		}
		
		public void LoadFromXml(System.Xml.XmlElement xmlroot)
		{
			System.Diagnostics.Debug.Assert (xmlroot.Name == "script");
			
			string arg_name  = xmlroot.GetAttribute ("name");
			string arg_about = xmlroot.GetAttribute ("about");
			
			System.Collections.ArrayList method_list = new System.Collections.ArrayList ();
			System.Collections.ArrayList value_list  = new System.Collections.ArrayList ();
			
			foreach (System.Xml.XmlNode node in xmlroot.ChildNodes)
			{
				if (node.NodeType == System.Xml.XmlNodeType.Element)
				{
					System.Xml.XmlElement elem = node as System.Xml.XmlElement;
					
					switch (elem.Name)
					{
						case "method":
							method_list.Add (this.LoadMethodFromXml (elem));
							break;
						
						case "value":
							value_list.Add (this.LoadValueFromXml (elem));
							break;
						
						default:
							System.Diagnostics.Debug.WriteLine ("Ignoring node " + elem.Name + " in script definition.");
							break;
					}
				}
			}
			
			Source.Method[]    methods = new Source.Method[method_list.Count];
			Types.IDataValue[] values  = new Types.IDataValue[value_list.Count];
			
			method_list.CopyTo (methods);
			value_list.CopyTo (values);
			
			this.Source = new Source (arg_name, methods, values, arg_about);
		}
		
		
		protected System.Xml.XmlNode CreateXmlNode(System.Xml.XmlDocument xmldoc, Source.Method method)
		{
			System.Xml.XmlElement   method_node   = xmldoc.CreateElement ("method");
			System.Xml.XmlAttribute name_attr     = xmldoc.CreateAttribute ("name");
			System.Xml.XmlAttribute ret_type_attr = xmldoc.CreateAttribute ("return");
			
			name_attr.Value     = method.Name;
			ret_type_attr.Value = this.info_store.GetNameFromType (method.ReturnType);
			
			method_node.Attributes.Append (name_attr);
			method_node.Attributes.Append (ret_type_attr);
			
			foreach (Source.ParameterInfo param in method.Parameters)
			{
				method_node.AppendChild (this.CreateXmlNode (xmldoc, param));
			}
			
			foreach (Source.CodeSection code in method.CodeSections)
			{
				method_node.AppendChild (this.CreateXmlNode (xmldoc, code));
			}
			
			return method_node;
		}
		
		protected System.Xml.XmlNode CreateXmlNode(System.Xml.XmlDocument xmldoc, Source.ParameterInfo param)
		{
			System.Xml.XmlElement   param_node = xmldoc.CreateElement ("param");
			System.Xml.XmlAttribute dir_attr   = xmldoc.CreateAttribute ("dir");
			System.Xml.XmlAttribute type_attr  = xmldoc.CreateAttribute ("type");
			System.Xml.XmlAttribute name_attr  = xmldoc.CreateAttribute ("name");
			
			dir_attr.Value  = this.info_store.GetNameFromDirection (param.Direction);
			type_attr.Value = this.info_store.GetNameFromType (param.Type);
			name_attr.Value = param.Name;
			
			param_node.Attributes.Append (dir_attr);
			param_node.Attributes.Append (type_attr);
			param_node.Attributes.Append (name_attr);
			
			return param_node;
		}
		
		protected System.Xml.XmlNode CreateXmlNode(System.Xml.XmlDocument xmldoc, Source.CodeSection code)
		{
			System.Xml.XmlElement code_node = xmldoc.CreateElement ("code");
			
			code_node.InnerText = code.Code;
			
			return code_node;
		}
		
		protected System.Xml.XmlNode CreateXmlNode(System.Xml.XmlDocument xmldoc, Types.IDataValue value)
		{
			System.Xml.XmlElement   value_node = xmldoc.CreateElement ("value");
			System.Xml.XmlAttribute type_attr  = xmldoc.CreateAttribute ("type");
			System.Xml.XmlAttribute name_attr  = xmldoc.CreateAttribute ("name");
			
			type_attr.Value = this.info_store.GetNameFromType (value.DataType);
			name_attr.Value = value.Name;
			
			value_node.Attributes.Append (name_attr);
			value_node.Attributes.Append (type_attr);
			
			return value_node;
		}
		
		
		protected Source.Method LoadMethodFromXml(System.Xml.XmlElement xmlroot)
		{
			string arg_name     = xmlroot.GetAttribute ("name");
			string arg_ret_type = xmlroot.GetAttribute ("return");
			
			System.Collections.ArrayList param_list = new System.Collections.ArrayList ();
			System.Collections.ArrayList code_list  = new System.Collections.ArrayList ();
			
			foreach (System.Xml.XmlNode node in xmlroot.ChildNodes)
			{
				if (node.NodeType == System.Xml.XmlNodeType.Element)
				{
					System.Xml.XmlElement elem = node as System.Xml.XmlElement;
					
					switch (elem.Name)
					{
						case "param":
							param_list.Add (this.LoadParamFromXml (elem));
							break;
						
						case "code":
							code_list.Add (this.LoadCodeFromXml (elem));
							break;
						
						default:
							System.Diagnostics.Debug.WriteLine ("Ignoring node " + elem.Name + " in method definition.");
							break;
					}
				}
			}
			
			string                 name     = arg_name;
			Types.INamedType       ret_type = this.info_store.GetTypeFromName (arg_ret_type);
			Source.ParameterInfo[] par_info = new Source.ParameterInfo[param_list.Count];
			Source.CodeSection[]   code_sec = new Source.CodeSection[code_list.Count];
			
			param_list.CopyTo (par_info);
			code_list.CopyTo (code_sec);
			
			return new Source.Method (name, ret_type, par_info, code_sec);
		}
		
		protected Source.ParameterInfo LoadParamFromXml(System.Xml.XmlElement xmlroot)
		{
			string arg_dir   = xmlroot.GetAttribute ("dir");
			string arg_type  = xmlroot.GetAttribute ("type");
			string arg_name  = xmlroot.GetAttribute ("name");
			
			Source.ParameterDirection dir  = this.info_store.GetDirectionFromName (arg_dir);
			Types.INamedType          type = this.info_store.GetTypeFromName (arg_type);
			
			return new Source.ParameterInfo (dir, type, arg_name);
		}
		
		protected Source.CodeSection LoadCodeFromXml(System.Xml.XmlElement xmlroot)
		{
			Source.CodeType type = Source.CodeType.Local;
			string          code = xmlroot.InnerText;
			
			return new Source.CodeSection (type, code);
		}
		
		protected Types.IDataValue LoadValueFromXml(System.Xml.XmlElement xmlroot)
		{
			string arg_type  = xmlroot.GetAttribute ("type");
			string arg_name  = xmlroot.GetAttribute ("name");
			
			Types.INamedType type = this.info_store.GetTypeFromName (arg_type);
			
			return new DataValue (arg_name, type);
		}
		
		
		
		#region ICommandDispatcher Members
		public bool DispatchCommand(Support.CommandDispatcher sender, Support.CommandEventArgs e)
		{
			Script script = this.Script;
			
			if (script.HasErrors == false)
			{
				Support.ICommandDispatcher dispatcher = script as Support.ICommandDispatcher;
				
				if (dispatcher != null)
				{
					dispatcher.DispatchCommand (sender, e);
				}
			}
			
			return false;
		}

		public string							Name
		{
			get
			{
				return this.source == null ? null : this.source.Name;
			}
		}
		#endregion
		
		protected class DataValue : Types.IDataValue
		{
			public DataValue(string name, Types.INamedType type)
			{
				this.name = name;
				this.type = type;
			}
			
			
			#region IDataValue Members
			public Types.INamedType				DataType
			{
				get
				{
					return this.type;
				}
			}

			public Types.IDataConstraint		DataConstraint
			{
				get
				{
					return null;
				}
			}
			
			public bool							IsValueValid
			{
				get
				{
					return false;
				}
			}
			
			public object ReadValue()
			{
				return null;
			}
			
			public void NotifyInvalidData()
			{
			}
			
			public void WriteValue(object value)
			{
			}
			
			
			public event Support.EventHandler	Changed;
			#endregion
			
			#region IDataItem Members
			public Types.DataItemClasses		Classes
			{
				get
				{
					return Types.DataItemClasses.Value;
				}
			}
			#endregion
			
			#region INameCaption Members
			public string						Description
			{
				get
				{
					return null;
				}
			}
			
			public string						Caption
			{
				get
				{
					return null;
				}
			}
			
			public string						Name
			{
				get
				{
					return this.name;
				}
			}
			#endregion
			
			protected void OnChanged()
			{
				if (this.Changed != null)
				{
					this.Changed (this);
				}
			}
			
			
			private Types.INamedType			type;
			private string						name;
		}
		
		private Source							source;
		private Script							script;
		private Helpers.ParameterInfoStore		info_store;
	}
}
