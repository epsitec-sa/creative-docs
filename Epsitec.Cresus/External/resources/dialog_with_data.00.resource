<bundle name="dialog_with_data">
  <data name="class">Window</data>
  <data name="WindowStyles">None</data>
  <data name="WindowType">Dialog</data>
  <data name="Size">330;260</data>
  <data name="Name">SettingsDialog</data>
  <data name="Text">[res:strings#title.SettingsWindow]</data>
  <list name="widgets">
    <bundle>
      <data name="class">DataWidget</data>
      <data name="Representation">TextField</data>
      <data name="CaptionWidth">100</data>
      <data name="Anchor">Top, LeftAndRight</data>
      <data name="AnchorMargins">8;8;16;173</data>
      <data name="Size">314;21</data>
      <data name="TabIndex">1</data>
      <data name="BindingInfo">&lt;bind path="UserName"/&gt;</data>
    </bundle>
    <bundle>
      <data name="class">DataWidget</data>
      <data name="Representation">NumericUpDown</data>
      <data name="CaptionWidth">100</data>
      <data name="Anchor">TopLeft</data>
      <data name="AnchorMargins">8;175;39;200</data>
      <data name="Size">147;21</data>
      <data name="TabIndex">2</data>
      <data name="BindingInfo">&lt;bind path="UserAge"/&gt;</data>
    </bundle>
    <bundle>
      <data name="class">DataWidget</data>
      <data name="Representation">RadioColumns</data>
      <data name="Anchor">Top, LeftAndRight</data>
      <data name="AnchorMargins">107;8;68;121</data>
      <data name="Size">215;71</data>
      <data name="TabIndex">3</data>
      <data name="BindingInfo">&lt;bind path="AccessMode"/&gt;</data>
    </bundle>
    <bundle name="OkButton">
      <data name="class">Button</data>
      <data name="Anchor">BottomRight</data>
      <data name="AnchorMargins">66;184;221;16</data>
      <data name="Name">OkButton</data>
      <data name="Command">Ok</data>
      <data name="Text">[res:strings#label.OK]</data>
      <data name="TabIndex">5</data>
    </bundle>
    <bundle name="CancelButton">
      <data name="class">Button</data>
      <data name="Anchor">BottomRight</data>
      <data name="AnchorMargins">154;96;221;16</data>
      <data name="Name">CancelButton</data>
      <data name="Command">Cancel</data>
      <data name="Text">[res:strings#label.Cancel]</data>
      <data name="TabIndex">6</data>
    </bundle>
    <bundle name="ApplyButton">
      <data name="class">Button</data>
      <data name="Anchor">BottomRight</data>
      <data name="AnchorMargins">276;8;171;16</data>
      <data name="Name">ApplyButton</data>
      <data name="Command">Apply</data>
      <data name="Text">[res:strings#label.Apply]</data>
      <data name="TabIndex">7</data>
    </bundle>
    <bundle>
      <data name="class">DataWidget</data>
      <data name="Representation">CheckColumns</data>
      <data name="Anchor">Top, LeftAndRight</data>
      <data name="AnchorMargins">107;8;141;65</data>
      <data name="Size">215;54</data>
      <data name="TabIndex">4</data>
      <data name="BindingInfo">&lt;bind path="LoginOptions"/&gt;</data>
    </bundle>
  </list>
</bundle>