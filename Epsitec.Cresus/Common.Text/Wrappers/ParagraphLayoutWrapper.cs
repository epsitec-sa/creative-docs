//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text.Wrappers
{
	/// <summary>
	/// La classe ParagraphLayoutWrapper simplifie l'accès aux réglages liés à
	/// la disposition d'un paragraphe (justification, marges, etc.)
	/// </summary>
	public class ParagraphLayoutWrapper : AbstractWrapper
	{
		public ParagraphLayoutWrapper()
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
		
		
		internal override void InternalSynchronise(AbstractState state, StateProperty property)
		{
			if (state != this.defined_state)
			{
				return;
			}
			
			if (this.defined_state.CountFlaggedValues () == 0)
			{
				return;
			}
			
			int defines = 0;
			
			double left_m_first  = double.NaN;
			double left_m_body   = double.NaN;
			double right_m_first = double.NaN;
			double right_m_body  = double.NaN;
			
			Properties.SizeUnits  units     = Properties.SizeUnits.None;
			Properties.ThreeState hyphenate = Properties.ThreeState.Undefined;
			
			double justif_body = double.NaN;
			double justif_last = double.NaN;
			double disposition = double.NaN;
			
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
			}
			
			if (this.defined_state.IsLeftMarginFirstDefined)
			{
				left_m_first = this.defined_state.LeftMarginFirst;
				units        = this.defined_state.Units;
				defines++;
			}
			
			if (this.defined_state.IsLeftMarginBodyDefined)
			{
				left_m_body = this.defined_state.LeftMarginBody;
				units       = this.defined_state.Units;
				defines++;
			}
			
			if (this.defined_state.IsRightMarginFirstDefined)
			{
				right_m_first = this.defined_state.RightMarginFirst;
				units         = this.defined_state.Units;
				defines++;
			}
			
			if (this.defined_state.IsRightMarginBodyDefined)
			{
				right_m_body = this.defined_state.RightMarginBody;
				units        = this.defined_state.Units;
				defines++;
			}
			
			if (this.defined_state.IsHyphenationDefined)
			{
				hyphenate = this.defined_state.Hyphenation ? Properties.ThreeState.True : Properties.ThreeState.False;
				defines++;
			}
			
			if (defines > 0)
			{
				this.DefineMetaProperty (ParagraphLayoutWrapper.Margins, 0, new Properties.MarginsProperty (left_m_first, left_m_body, right_m_first, right_m_body, units, justif_body, justif_last, disposition, double.NaN, double.NaN, hyphenate));
			}
			else
			{
				this.ClearMetaProperty (ParagraphLayoutWrapper.Margins);
			}
		}

		internal override void Update(bool active)
		{
			State state = active ? this.Active : this.Defined;
			
			System.Diagnostics.Debug.Assert (state.IsDirty == false);
			
			this.UpdateMargins (state, active);
			
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
				margins = this.ReadMetaProperty (ParagraphLayoutWrapper.Margins, Properties.WellKnownType.Margins) as Properties.MarginsProperty;
			}
			
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
				state.DefineValue (State.UnitsProperty, margins.Units);
				
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
				state.DefineValue (State.UnitsProperty);
				
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
			
			public Properties.SizeUnits				Units
			{
				get
				{
					return (Properties.SizeUnits) this.GetValue (State.UnitsProperty);
				}
				set
				{
					this.SetValue (State.UnitsProperty, value);
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
			
			public bool								IsUnitsDefined
			{
				get
				{
					return this.IsValueDefined (State.UnitsProperty);
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
			
			public void ClearUnits()
			{
				this.ClearValue (State.UnitsProperty);
			}
			
			
			#region State Properties
			public static readonly StateProperty	JustificationModeProperty = new StateProperty (typeof (State), "JustificationMode", JustificationMode.Unknown);
			public static readonly StateProperty	HyphenationProperty = new StateProperty (typeof (State), "Hyphenation", false);
			public static readonly StateProperty	LeftMarginFirstProperty = new StateProperty (typeof (State), "LeftMarginFirst", double.NaN);
			public static readonly StateProperty	LeftMarginBodyProperty = new StateProperty (typeof (State), "LeftMarginBody", double.NaN);
			public static readonly StateProperty	RightMarginFirstProperty = new StateProperty (typeof (State), "RightMarginFirst", double.NaN);
			public static readonly StateProperty	RightMarginBodyProperty = new StateProperty (typeof (State), "RightMarginBody", double.NaN);
			public static readonly StateProperty	UnitsProperty = new StateProperty (typeof (State), "Units", Properties.SizeUnits.None);
			#endregion
		}
		
		
		private State								active_state;
		private State								defined_state;
		
		private const string						Margins = "Margins";
	}
}
