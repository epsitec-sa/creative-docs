//	Copyright © 2005-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text.Wrappers
{
	/// <summary>
	/// La classe AbstractWrapper sert de base aux "wrappers" qui simplifient
	/// l'accès aux réglages internes d'un texte ou d'un style.
	/// </summary>
	public abstract class AbstractWrapper
	{
		protected AbstractWrapper()
		{
		}
		
		
		public bool								IsAttached
		{
			get
			{
				return this.context != null;
			}
		}
		
		public bool								IsAttachedToDefaultParagraphStyle
		{
			get
			{
				if ((this.style != null) &&
					(this.style_list != null) &&
					(this.style_list.IsDefaultParagraphTextStyle (this.style)))
				{
					return true;
				}
				else
				{
					return false;
				}
			}
		}
		
		public bool								IsAttachedToDefaultTextStyle
		{
			get
			{
				if ((this.style != null) &&
					(this.style_list != null) &&
					(this.style_list.IsDefaultTextTextStyle (this.style)))
				{
					return true;
				}
				else
				{
					return false;
				}
			}
		}
		
		public Attachment						Attachment
		{
			get
			{
				return this.attachment;
			}
		}
		
		public TextStyleClass					AttachedTextStyleClass
		{
			get
			{
				if (this.style == null)
				{
					return TextStyleClass.Invalid;
				}
				else
				{
					return this.style.TextStyleClass;
				}
			}
		}
		
		public TextStyle						AttachedStyle
		{
			get
			{
				return this.style;
			}
		}
		
		public TextNavigator					AttachedTextNavigator
		{
			get
			{
				return this.navigator;
			}
		}
		
		public TextContext						AttachedTextContext
		{
			get
			{
				return this.context;
			}
		}
		
		
		public void Attach(Text.TextNavigator navigator)
		{
			//	Attache le wrapper au navigateur. Ceci permet d'accéder aux réglages
			//	d'une sélection de texte ou aux réglages associés à la position cou-
			//	rante du curseur dans le texte.
			
			//	Attachment => Attachment.Text
			
			this.InternalDetach ();
			
			this.navigator   = navigator;
			this.story       = this.navigator.TextStory;
			this.context     = this.navigator.TextContext;
			this.style_list  = this.context.StyleList;
			this.oplet_queue = this.story.OpletQueue;
			
			this.navigator.TextChanged += this.HandleNavigatorTextChanged;
			this.navigator.CursorMoved += this.HandleNavigatorCursorMoved;
			
			this.attachment = Attachment.Text;
			
			this.UpdateAll ();
			this.NotifyChanged ();
		}
		
		public void Attach(Text.TextStyle style, Text.TextContext context, Common.Support.OpletQueue oplet_queue)
		{
			//	Attache le wrapper à un style. Ceci permet de modifier le style en
			//	spécifiant des réglages pour ses diverses propriétés.
			
			//	Attachment => Attachment.Style
			
			//	Crée un TextStory temporaire (uniquement utilisé pour simplifier
			//	la gestion du undo/redo) dans le code de synchronisation :
			
			TextStory story = new TextStory (oplet_queue, context);
			
			this.Attach (style, story);
		}
		
		public void Attach(Text.TextStyle style, Text.TextStory story)
		{
			this.InternalDetach ();
			
			this.story       = story;
			this.context     = this.story.TextContext;
			this.style       = style;
			this.style_list  = this.context.StyleList;
			this.oplet_queue = this.story.OpletQueue;
			
			this.style_list.StyleAdded += this.HandleStyleListStyleAdded;
			this.style_list.StyleRemoved += this.HandleStyleListStyleRemoved;
			this.style_list.StyleRedefined += this.HandleStyleListStyleRedefined;
			
			this.attachment = Attachment.Style;
			this.style.NotifyAttach (this);
			
			this.UpdateAll ();
			this.NotifyChanged ();
		}
		
		public void Detach()
		{
			//	Détache le wrapper de sa source.
			
			//	Attachment => Attachment.None
			
			this.InternalDetach ();
			
			this.NotifyChanged ();
		}
		
		
		public void SuspendSynchronizations()
		{
			this.suspend_synchronisations++;
		}
		
		public void DefineOperationName(string name, string caption)
		{
			//	Définit un nom d'opération (name = nom interne, caption = nom qui
			//	sera attaché à l'oplet dans OpletQueue, lequel sera visible par
			//	l'utilisateur).
			
			if (this.oplet_queue != null)
			{
				if ((this.last_op_name != name) &&
					(name != null) &&
					(name.Length > 0))
				{
					this.oplet_queue.DisableMerge ();
				}
			}
			
			this.last_op_name = name;
			this.last_op_caption = caption;
		}
		
		public void ResumeSynchronizations()
		{
			System.Diagnostics.Debug.Assert (this.suspend_synchronisations > 0);
			
			this.suspend_synchronisations--;
			
			if (this.suspend_synchronisations == 0)
			{
				int id = this.change_id++;
				
				switch (this.attachment)
				{
					case Attachment.Text:
						this.SynchronizeText (id);
						break;
					
					case Attachment.Style:
						this.SynchronizeStyle (id);
						break;
				}
			}
		}
		
		
		public void UpdateAll()
		{
			this.UpdateState (false);
			this.UpdateState (true);
		}
		
		
		private void SynchronizeText(int id)
		{
			System.Diagnostics.Debug.Assert (this.navigator != null);
			System.Diagnostics.Debug.Assert (this.style_list != null);
			
			this.navigator.SuspendNotifications ();
			
			using (this.story.BeginAction (this.last_op_caption))
			{
				foreach (AbstractState state in this.states)
				{
					foreach (StateProperty property in state.GetPendingProperties ())
					{
						state.NotifyChanged (property, id);
						this.InternalSynchronize (state, property);
					}
					
					state.ClearPendingProperties ();
				}
				
				this.story.ValidateAction ();
			}
			
			this.navigator.ResumeNotifications ();
		}
		
		private void SynchronizeStyle(int id)
		{
			System.Diagnostics.Debug.Assert (this.style != null);
			System.Diagnostics.Debug.Assert (this.style_list != null);
			
			AbstractWrapper[]            wrappers   = this.style.GetWrappers ();
			System.Collections.ArrayList properties = new System.Collections.ArrayList ();
			
			foreach (AbstractWrapper wrapper in wrappers)
			{
				foreach (AbstractState state in wrapper.states)
				{
					if (state.AccessMode == AccessMode.ReadWrite)
					{
						wrapper.pending_style_properties = properties;
						
						foreach (StateProperty property in state.GetPendingProperties ())
						{
							state.NotifyChanged (property, id);
						}
						
						state.ClearPendingProperties ();
						state.FlagAllDefinedProperties ();
						
						foreach (StateProperty property in state.GetDefinedProperties ())
						{
							wrapper.InternalSynchronize (state, property);
						}
						
						wrapper.pending_style_properties = null;
					}
				}
			}
			
			this.RedefineStyle (properties);
		}
		
		private void RedefineStyle(System.Collections.ArrayList properties)
		{
			using (this.story.BeginAction (this.last_op_caption))
			{
				TextStyle[] parent_styles = this.style.ParentStyles;
				
				Styles.PropertyContainer.Accumulator accumulator = new Styles.PropertyContainer.Accumulator ();
				
				accumulator.SkipSymbolProperties = true;
				accumulator.Accumulate (properties);
				
				this.style_list.RedefineTextStyle (this.oplet_queue, this.style, accumulator.AccumulatedProperties, parent_styles);
				this.story.ValidateAction ();
			}
		}
		
		
		private void InternalDetach()
		{
			if (this.style != null)
			{
				this.style.NotifyDetach (this);
			}
			if (this.navigator != null)
			{
				this.navigator.CursorMoved -= this.HandleNavigatorCursorMoved;
				this.navigator.TextChanged -= this.HandleNavigatorTextChanged;
			}
			if (this.style_list != null)
			{
				this.style_list.StyleAdded -= this.HandleStyleListStyleAdded;
				this.style_list.StyleRemoved -= this.HandleStyleListStyleRemoved;
				this.style_list.StyleRedefined -= this.HandleStyleListStyleRedefined;
			}
			
			this.story       = null;
			this.navigator   = null;
			this.style       = null;
			this.style_list  = null;
			this.oplet_queue = null;
			this.context     = null;
			
			this.attachment  = Attachment.None;
		}
		
		
		private void HandleNavigatorTextChanged(object sender)
		{
			this.UpdateAll ();
		}

		private void HandleNavigatorCursorMoved(object sender)
		{
			this.UpdateAll ();
		}
		
		private void HandleStyleListStyleAdded(object sender)
		{
		}
		
		private void HandleStyleListStyleRemoved(object sender)
		{
		}
		
		private void HandleStyleListStyleRedefined(object sender)
		{
			this.UpdateAll ();
		}
		
		
		protected void DefineMetaProperty(string meta, int priority, params Property[] properties)
		{
			if (this.attachment == Attachment.Text)
			{
				TextStyle style = this.style_list.CreateOrGetMetaProperty (meta, priority, properties);
				
				if (this.navigator.IsSelectionActive)
				{
					this.navigator.EndSelection ();
				}
				
				this.navigator.SetMetaProperties (Properties.ApplyMode.Set, style);
			}
			else if (this.attachment == Attachment.Style)
			{
#if true
				this.pending_style_properties.AddRange (properties);
#else
				System.Diagnostics.Debug.Assert (this.style != null);
				System.Diagnostics.Debug.Assert (this.style_list != null);
				
				System.Collections.ArrayList list = new System.Collections.ArrayList ();
				TextStyle[] parent_styles = this.style.ParentStyles;
				
				list.AddRange (this.style.StyleProperties);
				
				AbstractWrapper.RemoveStyleMetaProperties (list, meta);
				AbstractWrapper.InsertStyleMetaProperties (list, meta, priority, properties);
				
				this.style_list.RedefineTextStyle (this.oplet_queue, style, list, parent_styles);
#endif
			}
		}
		
		protected void ClearMetaProperty(string meta)
		{
			this.ClearMetaProperty (meta, Properties.ApplyMode.Clear);
		}
		
		protected void ClearUniformMetaProperty(string meta)
		{
			this.ClearMetaProperty (meta, Properties.ApplyMode.ClearUniform);
		}
		
		
		private void ClearMetaProperty(string meta, Properties.ApplyMode mode)
		{
			if (this.attachment == Attachment.Text)
			{
				TextStyle style = this.style_list.CreateOrGetMetaProperty (meta, new Property[0]);
				
				if (this.navigator.IsSelectionActive)
				{
					this.navigator.EndSelection ();
				}
				
				this.navigator.SetMetaProperties (mode, style);
			}
			else if (this.attachment == Attachment.Style)
			{
#if false
				System.Diagnostics.Debug.Assert (this.style != null);
				System.Diagnostics.Debug.Assert (this.style_list != null);
				
				System.Collections.ArrayList list = new System.Collections.ArrayList ();
				TextStyle[] parent_styles = this.style.ParentStyles;
				
				list.AddRange (this.style.StyleProperties);
				
				AbstractWrapper.RemoveStyleMetaProperties (list, meta);
				
				this.style_list.RedefineTextStyle (this.oplet_queue, style, list, parent_styles);
#endif
			}
		}
		
		
		private static void RemoveStyleMetaProperties(System.Collections.ArrayList list, string name)
		{
			for (int i = 0; i < list.Count; )
			{
				Property p = list[i] as Property;
				
				if (p.InternalName == name)
				{
					list.RemoveAt (i);
					continue;
				}

				i++;
			}
		}
		
		private static void InsertStyleMetaProperties(System.Collections.ArrayList list, string name, int priority, Property[] properties)
		{
			for (int i = 0; i < properties.Length; i++)
			{
				Property p = properties[i];
				
				p.InternalName = name;
				
				if (priority == 0)
				{
					list.Insert (i, p);
				}
				else
				{
					list.Add (p);
				}
			}
		}
		
		
		protected Property ReadAccumulatedProperty(Properties.WellKnownType type)
		{
			Property[] properties = null;
			
			if (this.attachment == Attachment.Text)
			{
				properties = this.navigator.AccumulatedTextProperties;
			}
			else if (this.attachment == Attachment.Style)
			{
				Styles.PropertyContainer.Accumulator accumulator = new Styles.PropertyContainer.Accumulator ();
				
				accumulator.SkipSymbolProperties = true;
				accumulator.Accumulate (this.story.FlattenStylesAndProperties (new TextStyle[1] { this.style }, new Property[0]));
				
				properties = accumulator.AccumulatedProperties;
			}
			
			if ((properties != null) &&
				(properties.Length > 0))
			{
				for (int i = 0; i < properties.Length; i++)
				{
					if (properties[i].WellKnownType == type)
					{
						return properties[i];
					}
				}
			}
			
			return null;
		}
		
		protected Property[] ReadAccumulatedProperties(Properties.WellKnownType type)
		{
			Property[] properties = null;
			
			if (this.attachment == Attachment.Text)
			{
				properties = this.navigator.AccumulatedTextProperties;
			}
			else if (this.attachment == Attachment.Style)
			{
				Styles.PropertyContainer.Accumulator accumulator = new Styles.PropertyContainer.Accumulator ();
				
				accumulator.SkipSymbolProperties = true;
				accumulator.Accumulate (this.story.FlattenStylesAndProperties (new TextStyle[1] { this.style }, new Property[0]));
				
				properties = accumulator.AccumulatedProperties;
			}
			
			if ((properties != null) &&
				(properties.Length > 0))
			{
				System.Collections.ArrayList list = new System.Collections.ArrayList ();
				
				for (int i = 0; i < properties.Length; i++)
				{
					if (properties[i].WellKnownType == type)
					{
						list.Add (properties[i]);
					}
				}
				
				return (Property[]) list.ToArray (typeof (Property));
			}
			
			return null;
		}
		
		protected Property ReadUnderlyingProperty(Properties.WellKnownType type)
		{
			TextStyle[] styles = null;
			
			if (this.attachment == Attachment.Text)
			{
				styles = this.navigator.TextStyles;
			}
			else if (this.attachment == Attachment.Style)
			{
				styles = this.style.ParentStyles;
			}
			
			if ((styles != null) &&
				(styles.Length > 0))
			{
				System.Collections.ArrayList list = new System.Collections.ArrayList ();
				
				foreach (TextStyle style in styles)
				{
					if (style.TextStyleClass != TextStyleClass.MetaProperty)
					{
						list.Add (style);
					}
				}
				
				if (list.Count > 0)
				{
					Property[] properties;
					
					this.context.GetFlatProperties (list, out styles, out properties);
					this.context.AccumulateProperties (properties, out properties);
					
					for (int i = 0; i < properties.Length; i++)
					{
						if (properties[i].WellKnownType == type)
						{
							return properties[i];
						}
					}
				}
			}
			
			return null;
		}
		
		protected Property ReadMetaProperty(string meta, Properties.WellKnownType type)
		{
			if (this.attachment == Attachment.Text)
			{
				TextStyle[] styles = this.navigator.TextStyles;
				
				if ((styles != null) &&
					(styles.Length > 0))
				{
					foreach (TextStyle style in styles)
					{
						if (style.MetaId == meta)
						{
							return style[type];
						}
					}
				}
			}
			else if (this.attachment == Attachment.Style)
			{
				Property[] properties = this.style.StyleProperties;
				
				foreach (Property property in properties)
				{
					if (property.WellKnownType == type)
					{
						return property;
					}
				}
			}
			
			return null;
		}
		
		protected Property[] ReadMetaProperties(string meta, Properties.WellKnownType type)
		{
			if (this.attachment == Attachment.Text)
			{
				TextStyle[] styles = this.navigator.TextStyles;
				
				if ((styles != null) &&
					(styles.Length > 0))
				{
					foreach (TextStyle style in styles)
					{
						if (style.MetaId == meta)
						{
							return style.FindProperties (type);
						}
					}
				}
			}
			else if (this.attachment == Attachment.Style)
			{
				System.Collections.ArrayList list = new System.Collections.ArrayList ();
				
				Property[] properties = this.style.StyleProperties;
				
				foreach (Property property in properties)
				{
					if (property.WellKnownType == type)
					{
						list.Add (property);
					}
				}
				
				if (list.Count > 0)
				{
					return (Property[]) list.ToArray (typeof (Property));
				}
			}
			
			return null;
		}
		
		
		internal void Register(AbstractState state)
		{
			if (this.states == null)
			{
				this.states = new System.Collections.ArrayList (2);
			}
			
			this.states.Add (state);
		}
		
		internal void NotifyChanged()
		{
			foreach (AbstractState state in this.states)
			{
				state.NotifyWrapperChanged ();
			}
		}
		
		
		internal void Synchronize(AbstractState state, StateProperty property)
		{
			int id = this.change_id++;
			
			if (state.AccessMode == AccessMode.ReadOnly)
			{
				//	Si les réglages sont définis en lecture seule, la synchronisation
				//	ne doit peut pas affecter le texte sous-jacent :
				
				state.NotifyChanged (property, id);
			}
			else
			{
				if (this.suspend_synchronisations > 0)
				{
					state.AddPendingProperty (property);
				}
				else
				{
					//	La logique de synchronisation se trouve dans ResumeSynchronizations
					//	et on ne veut pas la répliquer ici. On triche donc pour forcer une
					//	paire suspend/resume :
					
					this.SuspendSynchronizations ();
					state.AddPendingProperty (property);
					this.ResumeSynchronizations ();
				}
			}
		}
		
		
		internal abstract void InternalSynchronize(AbstractState state, StateProperty property);
		internal abstract void UpdateState(bool active);
		
		private Attachment						attachment;
		private TextStory						story;
		private TextContext						context;
		private TextNavigator					navigator;
		private Common.Support.OpletQueue		oplet_queue;
		private StyleList						style_list;
		private TextStyle						style;
		private System.Collections.ArrayList	states;
		private System.Collections.ArrayList	pending_style_properties;
		
		private string							last_op_name;
		private string							last_op_caption;
		
		private int								suspend_synchronisations;
		private int								change_id = 1;
	}
}
