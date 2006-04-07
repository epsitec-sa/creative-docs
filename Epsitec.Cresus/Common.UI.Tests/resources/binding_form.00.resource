<bundle>
 <data name="class">Window</data>
 <data name="Name">my_binding_form</data>
 <data name="Text">Binding...</data>
 <data name="Size">160;200</data>
 <data name="Icon">file:icon</data>
 <data name="Padding">8;8;12;12</data>
 <list name="widgets">
  <bundle>
   <data name="class">StaticText</data>
   <data name="Dock">Top</data>
   <data name="Text"><xml>Démonstration du <i>binding</i></xml></data>
  </bundle>
  <bundle>
   <data name="class">TextField</data>
   <data name="Dock">Top</data>
   <data name="Margins">0;0;8;0</data>
   <data name="BindingInfo"><xml><bind path="FontName"/></xml></data>
  </bundle>
  <bundle>
   <data name="class">CheckButton</data>
   <data name="Margins">0;0;8;0</data>
   <data name="Dock">Top</data>
   <data name="BindingInfo"><xml><bind path="UseHyphen"/></xml></data>
  </bundle>
  <bundle>
   <data name="class">RadioButton</data>
   <data name="Text">radio 1</data>
   <data name="Dock">Top</data>
   <data name="Margins">0;0;8;0</data>
   <data name="Group">g1</data>
   <data name="Index">0</data>
   <data name="BindingInfo"><xml><bind path="Quality"/></xml></data>
  </bundle>
  <bundle>
   <data name="class">RadioButton</data>
   <data name="Text">radio 2</data>
   <data name="Dock">Top</data>
   <data name="Group">g1</data>
   <data name="Index">1</data>
  </bundle>
  <bundle>
   <data name="class">RadioButton</data>
   <data name="Text">radio 3</data>
   <data name="Dock">Top</data>
   <data name="Group">g1</data>
   <data name="Index">2</data>
  </bundle>
  <bundle>
   <data name="class">TextFieldCombo</data>
   <data name="Margins">0;0;8;0</data>
   <data name="Dock">Top</data>
   <data name="BindingInfo"><xml><bind path="Quality"/></xml></data>
  </bundle>
  <bundle>
   <data name="class">TextFieldCombo</data>
   <data name="Margins">0;0;4;0</data>
   <data name="Dock">Top</data>
   <data name="BindingInfo"><xml><bind path="Optical"/></xml></data>
  </bundle>
 </list>
</bundle>
