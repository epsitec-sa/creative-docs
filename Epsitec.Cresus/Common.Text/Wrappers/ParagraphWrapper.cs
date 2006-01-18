//	Copyright � 2005-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text.Wrappers
{
	/// <summary>
	/// La classe ParagraphWrapper simplifie l'acc�s aux r�glages li�s �
	/// la disposition d'un paragraphe (justification, marges, etc.)
	/// </summary>
	public class ParagraphWrapper : AbstractWrapper
	{
		public ParagraphWrapper()
		{
			this.active_state  = new State (this, AccessMode.ReadOnly);
			this.defined_state = new State (this, AccessMode.ReadWrite);
		}
		
		
		public State								Active
		{
			get
			{
				return this.active_state;
			}
		}
		
		public State								Defined
		{
			get
			{
				return this.defined_state;
			}
		}
		
		
		public State GetUnderlyingMarginsState()
		{
			State state = new State (this, AccessMode.ReadOnly);
			Properties.MarginsProperty property = this.ReadUnderlyingProperty (Properties.WellKnownType.Margins) as Properties.MarginsProperty;
			this.UpdateMargins (state, property);
			return state;
		}
		
		public State GetUnderlyingLeadingState()
		{
			State state = new State (this, AccessMode.ReadOnly);
			Properties.LeadingProperty property = this.ReadUnderlyingProperty (Properties.WellKnownType.Leading) as Properties.LeadingProperty;
			this.UpdateLeading (state, property);
			return state;
		}
		
		public State GetUnderlyingKeepState()
		{
			State state = new State (this, AccessMode.ReadOnly);
			Properties.KeepProperty property = this.ReadUnderlyingProperty (Properties.WellKnownType.Keep) as Properties.KeepProperty;
			this.UpdateKeep (state, property);
			return state;
		}
		
		
		internal override void InternalSynchronize(AbstractState state, StateProperty property)
		{
			if (state == this.defined_state)
			{
				this.SynchronizeMargins ();
				this.SynchronizeLeading ();
				this.SynchronizeKeep ();
				this.SynchronizeManaged ();
				
				this.defined_state.ClearValueFlags ();
			}
		}
		
		
		private void SynchronizeMargins()
		{
			int defines = 0;
			int changes = 0;
			
			double left_m_first  = double.NaN;
			double left_m_body   = double.NaN;
			double right_m_first = double.NaN;
			double right_m_body  = double.NaN;
			
			Properties.SizeUnits  units     = Properties.SizeUnits.None;
			Properties.ThreeState hyphenate = Properties.ThreeState.Undefined;
			
			double justif_body = double.NaN;
			double justif_last = double.NaN;
			double disposition = double.NaN;
			
			int level = -1;
			
			if (this.defined_state.IsValueFlagged (State.JustificationModeProperty))
			{
				changes++;
			}
			
			if (this.defined_state.IsJustificationModeDefined)
			{
				switch (this.defined_state.JustificationMode)
				{
					case JustificationMode.AlignLeft:
						justif_body = 0;
						justif_last = 0;
						disposition = 0.0;
						defines++;
						break;
					
					case JustificationMode.AlignRight:
						justif_body = 0;
						justif_last = 0;
						disposition = 1.0;
						defines++;
						break;
					
					case JustificationMode.Center:
						justif_body = 0;
						justif_last = 0;
						disposition = 0.5;
						defines++;
						break;
					
					case JustificationMode.JustifyAlignLeft:
						justif_body = 1.0;
						justif_last = 0;
						disposition = 0.0;
						defines++;
						break;
					
					case JustificationMode.JustifyCenter:
						justif_body = 1.0;
						justif_last = 0.0;
						disposition = 0.5;
						defines++;
						break;
					
					case JustificationMode.JustifyAlignRight:
						justif_body = 1.0;
						justif_last = 0;
						disposition = 1.0;
						defines++;
						break;
					
					case JustificationMode.JustifyJustfy:
						justif_body = 1.0;
						justif_last = 1.0;
						disposition = 0.0;
						defines++;
						break;
				}
			}
			
			if (this.defined_state.IsValueFlagged (State.LeftMarginFirstProperty))
			{
				changes++;
			}
			if (this.defined_state.IsLeftMarginFirstDefined)
			{
				left_m_first = this.defined_state.LeftMarginFirst;
				units        = this.defined_state.MarginUnits;
				defines++;
			}
			
			if (this.defined_state.IsValueFlagged (State.LeftMarginBodyProperty))
			{
				changes++;
			}	
			if (this.defined_state.IsLeftMarginBodyDefined)
			{
				left_m_body = this.defined_state.LeftMarginBody;
				units       = this.defined_state.MarginUnits;
				defines++;
			}
			
			if (this.defined_state.IsValueFlagged (State.RightMarginFirstProperty))
			{
				changes++;
			}
			if (this.defined_state.IsRightMarginFirstDefined)
			{
				right_m_first = this.defined_state.RightMarginFirst;
				units         = this.defined_state.MarginUnits;
				defines++;
			}
			
			if (this.defined_state.IsValueFlagged (State.RightMarginBodyProperty))
			{
				changes++;
			}
			if (this.defined_state.IsRightMarginBodyDefined)
			{
				right_m_body = this.defined_state.RightMarginBody;
				units        = this.defined_state.MarginUnits;
				defines++;
			}
			
			if (this.defined_state.IsValueFlagged (State.HyphenationProperty))
			{
				changes++;
			}
			if (this.defined_state.IsHyphenationDefined)
			{
				hyphenate = this.defined_state.Hyphenation ? Properties.ThreeState.True : Properties.ThreeState.False;
				defines++;
			}
			
			if (this.defined_state.IsValueFlagged (State.IndentationLevelProperty))
			{
				changes++;
			}
			if (this.defined_state.IsIndentationLevelDefined)
			{
				level = this.defined_state.IndentationLevel;
				defines++;
			}
			
			if (changes > 0)
			{
				if (defines > 0)
				{
					this.DefineMetaProperty (ParagraphWrapper.Margins, 0, new Properties.MarginsProperty (left_m_first, left_m_body, right_m_first, right_m_body, units, justif_body, justif_last, disposition, double.NaN, double.NaN, hyphenate, level));
				}
				else
				{
					this.ClearUniformMetaProperty (ParagraphWrapper.Margins);
				}
			}
		}

		private void SynchronizeLeading()
		{
			int defines = 0;
			int changes = 0;
			
			double leading      = double.NaN;
			double space_before = double.NaN;
			double space_after  = double.NaN;
			
			Properties.SizeUnits leading_units      = Properties.SizeUnits.None;
			Properties.SizeUnits space_before_units = Properties.SizeUnits.None;
			Properties.SizeUnits space_after_units  = Properties.SizeUnits.None;
			
			Properties.AlignMode align_mode = Properties.AlignMode.Undefined;
			
			if ((this.defined_state.IsValueFlagged (State.LeadingProperty)) ||
				(this.defined_state.IsValueFlagged (State.LeadingUnitsProperty)))
			{
				changes++;
			}
			if (this.defined_state.IsLeadingDefined)
			{
				leading       = this.defined_state.Leading;
				leading_units = this.defined_state.LeadingUnits;
				
				if (leading_units == Properties.SizeUnits.Percent)
				{
					leading_units = Properties.SizeUnits.PercentNotCombining;
				}
				
				defines++;
			}
			
			if ((this.defined_state.IsValueFlagged (State.SpaceBeforeProperty)) ||
				(this.defined_state.IsValueFlagged (State.SpaceBeforeUnitsProperty)))
			{
				changes++;
			}
			if (this.defined_state.IsSpaceBeforeDefined)
			{
				space_before       = this.defined_state.SpaceBefore;
				space_before_units = this.defined_state.SpaceBeforeUnits;
				defines++;
			}
			
			if ((this.defined_state.IsValueFlagged (State.SpaceAfterProperty)) ||
				(this.defined_state.IsValueFlagged (State.SpaceAfterUnitsProperty)))
			{
				changes++;
			}
			if (this.defined_state.IsSpaceAfterDefined)
			{
				space_after       = this.defined_state.SpaceAfter;
				space_after_units = this.defined_state.SpaceAfterUnits;
				defines++;
			}
			
			if (this.defined_state.IsValueFlagged (State.AlignModeProperty))
			{
				changes++;
			}
			if (this.defined_state.IsAlignModeDefined)
			{
				align_mode = this.defined_state.AlignMode;
				defines++;
			}
			
			if (changes > 0)
			{
				if (defines > 0)
				{
					this.DefineMetaProperty (ParagraphWrapper.Leading, 0, new Properties.LeadingProperty (leading, leading_units, space_before, space_before_units, space_after, space_after_units, align_mode));
				}
				else
				{
					this.ClearUniformMetaProperty (ParagraphWrapper.Leading);
				}
			}
		}

		private void SynchronizeKeep()
		{
			int defines = 0;
			int changes = 0;
			
			int keep_start_lines = 0;
			int keep_end_lines   = 0;
			
			Properties.ThreeState keep_with_next     = Properties.ThreeState.Undefined;
			Properties.ThreeState keep_with_previous = Properties.ThreeState.Undefined;
			
			Properties.ParagraphStartMode start_mode = Properties.ParagraphStartMode.Undefined;
			
			if (this.defined_state.IsValueFlagged (State.KeepStartLinesProperty))
			{
				changes++;
			}
			if (this.defined_state.IsKeepStartLinesDefined)
			{
				keep_start_lines = this.defined_state.KeepStartLines;
				defines++;
			}
			
			if (this.defined_state.IsValueFlagged (State.KeepEndLinesProperty))
			{
				changes++;
			}
			if (this.defined_state.IsKeepEndLinesDefined)
			{
				keep_end_lines = this.defined_state.KeepEndLines;
				defines++;
			}
			
			if (this.defined_state.IsValueFlagged (State.KeepWithNextParagraphProperty))
			{
				changes++;
			}
			if (this.defined_state.IsKeepWithNextParagraphDefined)
			{
				keep_with_next = this.defined_state.KeepWithNextParagraph ? Properties.ThreeState.True : Properties.ThreeState.False;
				defines++;
			}
			
			if (this.defined_state.IsValueFlagged (State.KeepWithPreviousParagraphProperty))
			{
				changes++;
			}
			if (this.defined_state.IsKeepWithPreviousParagraphDefined)
			{
				keep_with_previous = this.defined_state.KeepWithPreviousParagraph ? Properties.ThreeState.True : Properties.ThreeState.False;
				defines++;
			}
			
			if (this.defined_state.IsValueFlagged (State.ParagraphStartModeProperty))
			{
				changes++;
			}
			if (this.defined_state.IsParagraphStartModeDefined)
			{
				start_mode = this.defined_state.ParagraphStartMode;
				defines++;
			}
			
			if (changes > 0)
			{
				if (defines > 0)
				{
					this.DefineMetaProperty (ParagraphWrapper.Keep, 0, new Properties.KeepProperty (keep_start_lines, keep_end_lines, start_mode, keep_with_previous, keep_with_next));
				}
				else
				{
					this.ClearUniformMetaProperty (ParagraphWrapper.Keep);
				}
			}
		}
		
		private void SynchronizeManaged()
		{
			int defines = 0;
			int changes = 0;
			
			System.Collections.ArrayList list = new System.Collections.ArrayList ();
			
			if (this.defined_state.IsValueFlagged (State.ItemListInfoProperty))
			{
				changes++;
			}
			if (this.defined_state.IsItemListInfoDefined)
			{
				string name = "ItemList";
				string info = this.defined_state.ItemListInfo;
				
				list.Add (new Properties.ManagedInfoProperty (name, info));
				
				defines++;
			}
			
			if (changes > 0)
			{
				if (defines > 0)
				{
					Property[] props = (Property[]) list.ToArray (typeof (Property));
					this.DefineMetaProperty (ParagraphWrapper.Managed, 0, props);
				}
				else
				{
					this.ClearUniformMetaProperty (ParagraphWrapper.Managed);
				}
			}
		}
		
		
		internal override void UpdateState(bool active)
		{
			State state = active ? this.Active : this.Defined;
			
			System.Diagnostics.Debug.Assert (state.IsDirty == false);
			
			this.UpdateMargins (state, active);
			this.UpdateLeading (state, active);
			this.UpdateKeep (state, active);
			
			state.NotifyIfDirty ();
		}
		
		
		private void UpdateMargins(State state, bool active)
		{
			Properties.MarginsProperty margins;
			
			if (active)
			{
				margins = this.ReadAccumulatedProperty (Properties.WellKnownType.Margins) as Properties.MarginsProperty;
			}
			else
			{
				margins = this.ReadMetaProperty (ParagraphWrapper.Margins, Properties.WellKnownType.Margins) as Properties.MarginsProperty;
			}
			
			this.UpdateMargins (state, margins);
		}
		
		private void UpdateMargins(State state, Properties.MarginsProperty margins)
		{
			if ((margins != null) &&
				(double.IsNaN (margins.Disposition) == false) &&
				(double.IsNaN (margins.JustificationBody) == false) &&
				(double.IsNaN (margins.JustificationLastLine) == false))
			{
				if ((margins.JustificationBody == 0.0) &&
					(margins.JustificationLastLine == 0.0))
				{
					if (margins.Disposition == 0.0)
					{
						state.DefineValue (State.JustificationModeProperty, JustificationMode.AlignLeft);
					}
					else if (margins.Disposition == 0.5)
					{
						state.DefineValue (State.JustificationModeProperty, JustificationMode.Center);
					}
					else if (margins.Disposition == 1.0)
					{
						state.DefineValue (State.JustificationModeProperty, JustificationMode.AlignRight);
					}
					else
					{
						state.DefineValue (State.JustificationModeProperty, JustificationMode.Unknown);
					}
				}
				else if ((margins.JustificationBody == 1.0) &&
					/**/ (margins.JustificationLastLine == 0.0) &&
					/**/ (margins.Disposition == 0.0))
				{
					state.DefineValue (State.JustificationModeProperty, JustificationMode.JustifyAlignLeft);
				}
				else if ((margins.JustificationBody == 1.0) &&
					/**/ (margins.JustificationLastLine == 0.0) &&
					/**/ (margins.Disposition == 0.5))
				{
					state.DefineValue (State.JustificationModeProperty, JustificationMode.JustifyCenter);
				}
				else if ((margins.JustificationBody == 1.0) &&
					/**/ (margins.JustificationLastLine == 0.0) &&
					/**/ (margins.Disposition == 1.0))
				{
					state.DefineValue (State.JustificationModeProperty, JustificationMode.JustifyAlignRight);
				}
				else if ((margins.JustificationBody == 1.0) &&
					/**/ (margins.JustificationLastLine == 1.0) &&
					/**/ (margins.Disposition == 0.0))
				{
					state.DefineValue (State.JustificationModeProperty, JustificationMode.JustifyJustfy);
				}
				else
				{
					state.DefineValue (State.JustificationModeProperty, JustificationMode.Unknown);
				}
			}
			else
			{
				state.DefineValue (State.JustificationModeProperty);
			}
			
			if ((margins != null) &&
				(margins.Units != Properties.SizeUnits.None))
			{
				state.DefineValue (State.MarginUnitsProperty, margins.Units);
				
				if (double.IsNaN (margins.LeftMarginFirstLine))
				{
					state.DefineValue (State.LeftMarginFirstProperty);
				}
				else
				{
					state.DefineValue (State.LeftMarginFirstProperty, margins.LeftMarginFirstLine);
				}
				
				if (double.IsNaN (margins.LeftMarginBody))
				{
					state.DefineValue (State.LeftMarginBodyProperty);
				}
				else
				{
					state.DefineValue (State.LeftMarginBodyProperty, margins.LeftMarginBody);
				}
				
				if (double.IsNaN (margins.RightMarginFirstLine))
				{
					state.DefineValue (State.RightMarginFirstProperty);
				}
				else
				{
					state.DefineValue (State.RightMarginFirstProperty, margins.RightMarginFirstLine);
				}
				
				if (double.IsNaN (margins.RightMarginBody))
				{
					state.DefineValue (State.RightMarginBodyProperty);
				}
				else
				{
					state.DefineValue (State.RightMarginBodyProperty, margins.RightMarginBody);
				}
			}
			else
			{
				state.DefineValue (State.MarginUnitsProperty);
				
				state.DefineValue (State.LeftMarginFirstProperty);
				state.DefineValue (State.LeftMarginBodyProperty);
				state.DefineValue (State.RightMarginFirstProperty);
				state.DefineValue (State.RightMarginBodyProperty);
			}
			
			if (margins != null)
			{
				switch (margins.EnableHyphenation)
				{
					case Properties.ThreeState.Undefined:
						state.DefineValue (State.HyphenationProperty);
						break;
					
					case Properties.ThreeState.True:
						state.DefineValue (State.HyphenationProperty, true);
						break;
					
					case Properties.ThreeState.False:
						state.DefineValue (State.HyphenationProperty, false);
						break;
				}
			}
			else
			{
				state.DefineValue (State.HyphenationProperty);
			}
			
			if ((margins != null) &&
				(margins.Level > -1))
			{
				state.DefineValue (State.IndentationLevelProperty, margins.Level);
			}
			else
			{
				state.DefineValue (State.IndentationLevelProperty);
			}
		}
		
		private void UpdateLeading(State state, bool active)
		{
			Properties.LeadingProperty leading;
			
			if (active)
			{
				leading = this.ReadAccumulatedProperty (Properties.WellKnownType.Leading) as Properties.LeadingProperty;
			}
			else
			{
				leading = this.ReadMetaProperty (ParagraphWrapper.Leading, Properties.WellKnownType.Leading) as Properties.LeadingProperty;
			}
			
			this.UpdateLeading (state, leading);
		}
		
		private void UpdateLeading(State state, Properties.LeadingProperty leading)
		{
			if ((leading != null) &&
				(leading.LeadingUnits != Properties.SizeUnits.None) &&
				(double.IsNaN (leading.Leading) == false))
			{
				Properties.SizeUnits leading_units = leading.LeadingUnits;
				
				if (leading_units == Properties.SizeUnits.PercentNotCombining)
				{
					leading_units = Properties.SizeUnits.Percent;
				}

				state.DefineValue (State.LeadingProperty, leading.Leading);
				state.DefineValue (State.LeadingUnitsProperty, leading_units);
			}
			else
			{
				state.DefineValue (State.LeadingProperty);
				state.DefineValue (State.LeadingUnitsProperty);
			}
			
			if ((leading != null) &&
				(leading.SpaceBeforeUnits != Properties.SizeUnits.None) &&
				(double.IsNaN (leading.SpaceBefore) == false))
			{
				state.DefineValue (State.SpaceBeforeProperty, leading.SpaceBefore);
				state.DefineValue (State.SpaceBeforeUnitsProperty, leading.SpaceBeforeUnits);
			}
			else
			{
				state.DefineValue (State.SpaceBeforeProperty);
				state.DefineValue (State.SpaceBeforeUnitsProperty);
			}
			
			if ((leading != null) &&
				(leading.SpaceAfterUnits != Properties.SizeUnits.None) &&
				(double.IsNaN (leading.SpaceAfter) == false))
			{
				state.DefineValue (State.SpaceAfterProperty, leading.SpaceAfter);
				state.DefineValue (State.SpaceAfterUnitsProperty, leading.SpaceAfterUnits);
			}
			else
			{
				state.DefineValue (State.SpaceAfterProperty);
				state.DefineValue (State.SpaceAfterUnitsProperty);
			}
			
			if ((leading != null) &&
				(leading.AlignMode != Properties.AlignMode.Undefined))
			{
				state.DefineValue (State.AlignModeProperty, leading.AlignMode);
			}
			else
			{
				state.DefineValue (State.AlignModeProperty);
			}
		}
		
		private void UpdateKeep(State state, bool active)
		{
			Properties.KeepProperty keep;
			
			if (active)
			{
				keep = this.ReadAccumulatedProperty (Properties.WellKnownType.Keep) as Properties.KeepProperty;
			}
			else
			{
				keep = this.ReadMetaProperty (ParagraphWrapper.Keep, Properties.WellKnownType.Keep) as Properties.KeepProperty;
			}
			
			this.UpdateKeep (state, keep);
		}
		
		private void UpdateKeep(State state, Properties.KeepProperty keep)
		{
			if ((keep != null) &&
				(keep.StartLines > 0))
			{
				state.DefineValue (State.KeepStartLinesProperty, keep.StartLines);
			}
			else
			{
				state.DefineValue (State.KeepStartLinesProperty);
			}
			
			if ((keep != null) &&
				(keep.EndLines > 0))
			{
				state.DefineValue (State.KeepEndLinesProperty, keep.EndLines);
			}
			else
			{
				state.DefineValue (State.KeepEndLinesProperty);
			}
			
			if ((keep != null) &&
				(keep.KeepWithPreviousParagraph != Properties.ThreeState.Undefined))
			{
				state.DefineValue (State.KeepWithPreviousParagraphProperty, keep.KeepWithPreviousParagraph == Properties.ThreeState.True);
			}
			else
			{
				state.DefineValue (State.KeepWithPreviousParagraphProperty);
			}
			
			if ((keep != null) &&
				(keep.KeepWithNextParagraph != Properties.ThreeState.Undefined))
			{
				state.DefineValue (State.KeepWithNextParagraphProperty, keep.KeepWithNextParagraph == Properties.ThreeState.True);
			}
			else
			{
				state.DefineValue (State.KeepWithNextParagraphProperty);
			}
			
			if ((keep != null) &&
				(keep.ParagraphStartMode != Properties.ParagraphStartMode.Undefined))
			{
				state.DefineValue (State.ParagraphStartModeProperty, keep.ParagraphStartMode);
			}
			else
			{
				state.DefineValue (State.ParagraphStartModeProperty);
			}
		}
		
		
		
		public class State : AbstractState
		{
			internal State(AbstractWrapper wrapper, AccessMode access) : base (wrapper, access)
			{
			}
			
			
			public JustificationMode				JustificationMode
			{
				get
				{
					return (JustificationMode) this.GetValue (State.JustificationModeProperty);
				}
				set
				{
					this.SetValue (State.JustificationModeProperty, value);
				}
			}
			
			public bool								Hyphenation
			{
				get
				{
					return (bool) this.GetValue (State.HyphenationProperty);
				}
				set
				{
					this.SetValue (State.HyphenationProperty, value);
				}
			}
			
			public double							LeftMarginFirst
			{
				get
				{
					return (double) this.GetValue (State.LeftMarginFirstProperty);
				}
				set
				{
					this.SetValue (State.LeftMarginFirstProperty, value);
				}
			}
			
			public double							LeftMarginBody
			{
				get
				{
					return (double) this.GetValue (State.LeftMarginBodyProperty);
				}
				set
				{
					this.SetValue (State.LeftMarginBodyProperty, value);
				}
			}
			
			public double							RightMarginFirst
			{
				get
				{
					return (double) this.GetValue (State.RightMarginFirstProperty);
				}
				set
				{
					this.SetValue (State.RightMarginFirstProperty, value);
				}
			}
			
			public double							RightMarginBody
			{
				get
				{
					return (double) this.GetValue (State.RightMarginBodyProperty);
				}
				set
				{
					this.SetValue (State.RightMarginBodyProperty, value);
				}
			}
			
			public Properties.SizeUnits				MarginUnits
			{
				get
				{
					return (Properties.SizeUnits) this.GetValue (State.MarginUnitsProperty);
				}
				set
				{
					this.SetValue (State.MarginUnitsProperty, value);
				}
			}
			
			public int								IndentationLevel
			{
				get
				{
					return (int) this.GetValue (State.IndentationLevelProperty);
				}
				set
				{
					this.SetValue (State.IndentationLevelProperty, value);
				}
			}
			
			
			public double							Leading
			{
				get
				{
					return (double) this.GetValue (State.LeadingProperty);
				}
				set
				{
					this.SetValue (State.LeadingProperty, value);
				}
			}
			
			public Properties.SizeUnits				LeadingUnits
			{
				get
				{
					return (Properties.SizeUnits) this.GetValue (State.LeadingUnitsProperty);
				}
				set
				{
					this.SetValue (State.LeadingUnitsProperty, value);
				}
			}
			
			public double							SpaceBefore
			{
				get
				{
					return (double) this.GetValue (State.SpaceBeforeProperty);
				}
				set
				{
					this.SetValue (State.SpaceBeforeProperty, value);
				}
			}
			
			public Properties.SizeUnits				SpaceBeforeUnits
			{
				get
				{
					return (Properties.SizeUnits) this.GetValue (State.SpaceBeforeUnitsProperty);
				}
				set
				{
					this.SetValue (State.SpaceBeforeUnitsProperty, value);
				}
			}
			
			public double							SpaceAfter
			{
				get
				{
					return (double) this.GetValue (State.SpaceAfterProperty);
				}
				set
				{
					this.SetValue (State.SpaceAfterProperty, value);
				}
			}
			
			public Properties.SizeUnits				SpaceAfterUnits
			{
				get
				{
					return (Properties.SizeUnits) this.GetValue (State.SpaceAfterUnitsProperty);
				}
				set
				{
					this.SetValue (State.SpaceAfterUnitsProperty, value);
				}
			}
			
			public Properties.AlignMode				AlignMode
			{
				get
				{
					return (Properties.AlignMode) this.GetValue (State.AlignModeProperty);
				}
				set
				{
					this.SetValue (State.AlignModeProperty, value);
				}
			}
			
			
			public int								KeepStartLines
			{
				get
				{
					return (int) this.GetValue (State.KeepStartLinesProperty);
				}
				set
				{
					this.SetValue (State.KeepStartLinesProperty, value);
				}
			}
			
			public int								KeepEndLines
			{
				get
				{
					return (int) this.GetValue (State.KeepEndLinesProperty);
				}
				set
				{
					this.SetValue (State.KeepEndLinesProperty, value);
				}
			}
			
			public bool								KeepWithNextParagraph
			{
				get
				{
					return (bool) this.GetValue (State.KeepWithNextParagraphProperty);
				}
				set
				{
					this.SetValue (State.KeepWithNextParagraphProperty, value);
				}
			}
			
			public bool								KeepWithPreviousParagraph
			{
				get
				{
					return (bool) this.GetValue (State.KeepWithPreviousParagraphProperty);
				}
				set
				{
					this.SetValue (State.KeepWithPreviousParagraphProperty, value);
				}
			}
			
			public Properties.ParagraphStartMode	ParagraphStartMode
			{
				get
				{
					return (Properties.ParagraphStartMode) this.GetValue (State.ParagraphStartModeProperty);
				}
				set
				{
					this.SetValue (State.ParagraphStartModeProperty, value);
				}
			}
			
			
			public string							ItemListInfo
			{
				get
				{
					return (string) this.GetValue (State.ItemListInfoProperty);
				}
				set
				{
					this.SetValue (State.ItemListInfoProperty, value);
				}
			}
			
			
			public bool								IsJustificationModeDefined
			{
				get
				{
					return this.IsValueDefined (State.JustificationModeProperty);
				}
			}
			
			public bool								IsHyphenationDefined
			{
				get
				{
					return this.IsValueDefined (State.HyphenationProperty);
				}
			}
			
			public bool								IsLeftMarginFirstDefined
			{
				get
				{
					return this.IsValueDefined (State.LeftMarginFirstProperty);
				}
			}
			
			public bool								IsLeftMarginBodyDefined
			{
				get
				{
					return this.IsValueDefined (State.LeftMarginBodyProperty);
				}
			}
			
			public bool								IsRightMarginFirstDefined
			{
				get
				{
					return this.IsValueDefined (State.RightMarginFirstProperty);
				}
			}
			
			public bool								IsRightMarginBodyDefined
			{
				get
				{
					return this.IsValueDefined (State.RightMarginBodyProperty);
				}
			}
			
			public bool								IsMarginUnitsDefined
			{
				get
				{
					return this.IsValueDefined (State.MarginUnitsProperty);
				}
			}
			
			public bool								IsIndentationLevelDefined
			{
				get
				{
					return this.IsValueDefined (State.IndentationLevelProperty);
				}
			}
			
			
			public bool								IsLeadingDefined
			{
				get
				{
					return this.IsValueDefined (State.LeadingProperty);
				}
			}
			
			public bool								IsLeadingUnitsDefined
			{
				get
				{
					return this.IsValueDefined (State.LeadingUnitsProperty);
				}
			}
			
			public bool								IsSpaceBeforeDefined
			{
				get
				{
					return this.IsValueDefined (State.SpaceBeforeProperty);
				}
			}
			
			public bool								IsSpaceBeforeUnitsDefined
			{
				get
				{
					return this.IsValueDefined (State.SpaceBeforeUnitsProperty);
				}
			}
			
			public bool								IsSpaceAfterDefined
			{
				get
				{
					return this.IsValueDefined (State.SpaceAfterProperty);
				}
			}
			
			public bool								IsSpaceAfterUnitsDefined
			{
				get
				{
					return this.IsValueDefined (State.SpaceAfterUnitsProperty);
				}
			}
			
			public bool								IsAlignModeDefined
			{
				get
				{
					return this.IsValueDefined (State.AlignModeProperty);
				}
			}
			
			
			public bool								IsKeepStartLinesDefined
			{
				get
				{
					return this.IsValueDefined (State.KeepStartLinesProperty);
				}
			}
			
			public bool								IsKeepEndLinesDefined
			{
				get
				{
					return this.IsValueDefined (State.KeepEndLinesProperty);
				}
			}
			
			public bool								IsKeepWithNextParagraphDefined
			{
				get
				{
					return this.IsValueDefined (State.KeepWithNextParagraphProperty);
				}
			}
			
			public bool								IsKeepWithPreviousParagraphDefined
			{
				get
				{
					return this.IsValueDefined (State.KeepWithPreviousParagraphProperty);
				}
			}
			
			public bool								IsParagraphStartModeDefined
			{
				get
				{
					return this.IsValueDefined (State.ParagraphStartModeProperty);
				}
			}
			
			
			public bool								IsItemListInfoDefined
			{
				get
				{
					return this.IsValueDefined (State.ItemListInfoProperty);
				}
			}
			
			
			public void ClearJustificationMode()
			{
				this.ClearValue (State.JustificationModeProperty);
			}
			
			public void ClearHyphenation()
			{
				this.ClearValue (State.HyphenationProperty);
			}
			
			public void ClearLeftMarginFirst()
			{
				this.ClearValue (State.LeftMarginFirstProperty);
			}
			
			public void ClearLeftMarginBody()
			{
				this.ClearValue (State.LeftMarginBodyProperty);
			}
			
			public void ClearRightMarginFirst()
			{
				this.ClearValue (State.RightMarginFirstProperty);
			}
			
			public void ClearRightMarginBody()
			{
				this.ClearValue (State.RightMarginBodyProperty);
			}
			
			public void ClearMarginUnits()
			{
				this.ClearValue (State.MarginUnitsProperty);
			}
			
			public void ClearIndentationLevel()
			{
				this.ClearValue (State.IndentationLevelProperty);
			}
			
			
			public void ClearLeading()
			{
				this.ClearValue (State.LeadingProperty);
			}
			
			public void ClearLeadingUnits()
			{
				this.ClearValue (State.LeadingUnitsProperty);
			}
			
			public void ClearSpaceBefore()
			{
				this.ClearValue (State.SpaceBeforeProperty);
			}
			
			public void ClearSpaceBeforeUnits()
			{
				this.ClearValue (State.SpaceBeforeUnitsProperty);
			}
			
			public void ClearSpaceAfter()
			{
				this.ClearValue (State.SpaceAfterProperty);
			}
			
			public void ClearSpaceAfterUnits()
			{
				this.ClearValue (State.SpaceAfterUnitsProperty);
			}
			
			public void ClearAlignMode()
			{
				this.ClearValue (State.AlignModeProperty);
			}
			
			
			public void ClearKeepStartLines()
			{
				this.ClearValue (State.KeepStartLinesProperty);
			}
			
			public void ClearKeepEndLines()
			{
				this.ClearValue (State.KeepEndLinesProperty);
			}
			
			public void ClearKeepWithNextParagraph()
			{
				this.ClearValue (State.KeepWithNextParagraphProperty);
			}
			
			public void ClearKeepWithPreviousParagraph()
			{
				this.ClearValue (State.KeepWithPreviousParagraphProperty);
			}
			
			public void ClearParagraphStartMode()
			{
				this.ClearValue (State.ParagraphStartModeProperty);
			}
			
			
			public void ClearItemListInfo()
			{
				this.ClearValue (State.ItemListInfoProperty);
			}
			
			
			#region State Properties
			public static readonly StateProperty	JustificationModeProperty = new StateProperty (typeof (State), "JustificationMode", JustificationMode.Unknown);
			public static readonly StateProperty	HyphenationProperty = new StateProperty (typeof (State), "Hyphenation", false);
			public static readonly StateProperty	LeftMarginFirstProperty = new StateProperty (typeof (State), "LeftMarginFirst", double.NaN);
			public static readonly StateProperty	LeftMarginBodyProperty = new StateProperty (typeof (State), "LeftMarginBody", double.NaN);
			public static readonly StateProperty	RightMarginFirstProperty = new StateProperty (typeof (State), "RightMarginFirst", double.NaN);
			public static readonly StateProperty	RightMarginBodyProperty = new StateProperty (typeof (State), "RightMarginBody", double.NaN);
			public static readonly StateProperty	MarginUnitsProperty = new StateProperty (typeof (State), "MarginUnits", Properties.SizeUnits.None);
			public static readonly StateProperty	IndentationLevelProperty = new StateProperty (typeof (State), "IndentationLevel", -1);
			public static readonly StateProperty	LeadingProperty = new StateProperty (typeof (State), "Leading", double.NaN);
			public static readonly StateProperty	LeadingUnitsProperty = new StateProperty (typeof (State), "LeadingUnits", Properties.SizeUnits.None);
			public static readonly StateProperty	SpaceBeforeProperty = new StateProperty (typeof (State), "SpaceBefore", double.NaN);
			public static readonly StateProperty	SpaceBeforeUnitsProperty = new StateProperty (typeof (State), "SpaceBeforeUnits", Properties.SizeUnits.None);
			public static readonly StateProperty	SpaceAfterProperty = new StateProperty (typeof (State), "SpaceAfter", double.NaN);
			public static readonly StateProperty	SpaceAfterUnitsProperty = new StateProperty (typeof (State), "SpaceAfterUnits", Properties.SizeUnits.None);
			public static readonly StateProperty	AlignModeProperty = new StateProperty (typeof (State), "AlignMode", Properties.AlignMode.Undefined);
			public static readonly StateProperty	KeepStartLinesProperty = new StateProperty (typeof (State), "KeepStartLines", 0);
			public static readonly StateProperty	KeepEndLinesProperty = new StateProperty (typeof (State), "KeepEndLines", 0);
			public static readonly StateProperty	ParagraphStartModeProperty = new StateProperty (typeof (State), "ParagraphStartMode", Properties.ParagraphStartMode.Undefined);
			public static readonly StateProperty	ItemListInfoProperty = new StateProperty (typeof (State), "ItemListInfo", null);
			public static readonly StateProperty	KeepWithNextParagraphProperty = new StateProperty (typeof (State), "KeepWithNextParagraph", false);
			public static readonly StateProperty	KeepWithPreviousParagraphProperty = new StateProperty (typeof (State), "KeepWithPreviousParagraph", false);
			#endregion
		}
		
		
		private State								active_state;
		private State								defined_state;
		
		private const string						Margins = "#Pa#Margins";
		private const string						Leading = "#Pa#Leading";
		private const string						Keep	= "#Pa#Keep";
		private const string						Managed = "#Pa#Managed";
	}
}
