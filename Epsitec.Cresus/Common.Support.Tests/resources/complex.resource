<bundle>
 <field name='class'>GroupBox</field>
 <field name='name'>my_group</field>
 <field name='size'>500;300</field>
 <list name='widgets'>				<!-- la liste 'widgets' est constitué par une collection de bundles -->
  <bundle>							<!-- premier bundle -->
   <field name='class'>CheckButton</field>
  </bundle>
  <bundle>							<!-- second bundle -->
   <field name='class'>RadioButton</field>
  </bundle>
  <ref target='button.cancel'/>
 </list>							<!-- fin de la liste -->
</bundle>
