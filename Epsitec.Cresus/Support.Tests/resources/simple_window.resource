<bundle>
 <field name='class'>Window</field>
 <field name='name'>my_window</field>
 <field name='text'>Ressource: démo</field>
 <field name='size'>240;280</field>
 <field name='icon'><ref target='file:icon' type='binary'/></field>
 <list name='widgets'>
  <bundle>
   <field name='class'>CheckButton</field>
   <field name='text'>Bouton A</field>
   <field name='name'>checkA</field>
   <field name='cmd'>on</field>
   <field name='size'>200;*</field>
   <field name='pos'>10;210</field>
  </bundle>
  <bundle>
   <field name='class'>GroupBox</field>
   <field name='anchor'>Left, Right, Top, Bottom</field>
   <field name='text'><![CDATA[Choix <i>cornélien</i>...]]></field>
   <field name='size'>215;85</field>
   <field name='pos'>10;60</field>
   <field name='dock_h'>no</field>
   <field name='dock_m'>5;5;20;5</field>
   <list name='widgets'>
    <bundle>
     <field name='class'>RadioButton</field>
     <field name='text'>Bouton B</field>
     <field name='dock'>Fill</field>
    </bundle>
    <bundle>
     <field name='class'>RadioButton</field>
     <field name='text'>Bouton C</field>
     <field name='dock'>Fill</field>
    </bundle>
    <bundle>
     <field name='class'>RadioButton</field>
     <field name='text'>Bouton D</field>
     <field name='dock'>Fill</field>
    </bundle>
   </list>
  </bundle>
  <bundle>
   <ref target='button.cancel'/>
   <field name='cmd'>on</field>
   <field name='pos'>10;10</field>
  </bundle>
 </list>
</bundle>
