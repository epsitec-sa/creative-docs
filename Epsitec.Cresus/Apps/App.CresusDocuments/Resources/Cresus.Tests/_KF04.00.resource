<?xml version="1.0" encoding="utf-8"?>
<bundle name="_KF04" caption="Form.ContactList" type="Panel" culture="fr" rank="2">
  <data name="Panel">
    <xml>
      <panel xmlns:s="http://www.epsitec.ch/XNS/storage-structure-1" xmlns:f="http://www.epsitec.ch/XNS/storage-fields-1">
        <s:storage s:root="_1" s:n_ext="2" s:n_typ="11" s:n_obj="15">
          <s:external s:name="_DataSource" />
          <s:external s:name="_ResourceManager" />
          <s:type id="_1" s:name="Epsitec.Common.UI.Panel" />
          <s:type id="_2" s:name="Epsitec.Common.Support.ResourceManager" />
          <s:type id="_3" s:name="Epsitec.Common.Types.DataObject" />
          <s:type id="_4" s:name="Epsitec.Common.UI.TablePlaceholder" />
          <s:type id="_5" s:name="Epsitec.Common.UI.ItemTableColumn" />
          <s:type id="_6" s:name="Epsitec.Common.Widgets.FrameBox" />
          <s:type id="_7" s:name="Epsitec.Common.Widgets.MetaButton" />
          <s:type id="_8" s:name="Epsitec.Common.UI.PanelPlaceholder" />
          <s:type id="_9" s:name="Epsitec.Common.UI.DataSourceMetadata" />
          <s:type id="_10" s:name="Epsitec.Common.UI.Panels.EditPanel" />
          <s:type id="_11" s:name="Epsitec.Common.UI.Panels.SearchPanel" />
          <s:object id="_1" s:type="_1" />
          <s:object id="_2" s:type="_4" />
          <s:object id="_3" s:type="_5" />
          <s:object id="_4" s:type="_5" />
          <s:object id="_5" s:type="_5" />
          <s:object id="_6" s:type="_5" />
          <s:object id="_7" s:type="_5" />
          <s:object id="_8" s:type="_5" />
          <s:object id="_9" s:type="_5" />
          <s:object id="_10" s:type="_6" />
          <s:object id="_11" s:type="_7" />
          <s:object id="_12" s:type="_8" />
          <s:object id="_13" s:type="_9" />
          <s:object id="_14" s:type="_10" />
          <s:object id="_15" s:type="_11" />
          <s:data id="_1" f:TabNavigationMode="ForwardTabPassive" f:ChildrenLayoutMode="Stacked" f:ContainerLayoutMode="VerticalFlow" f:PreferredWidth="475" f:PreferredHeight="350" f:Anchor="BottomLeft" f:Padding="20;20;16;16" f:_2.ResourceManager="{ExtRef _ResourceManager}" f:_1.BundleId="[KF04]" f:_3.DataContext="\[Binding Mode=TwoWay\]" f:Children="{Collection {ObjRef _2}, {ObjRef _10}, {ObjRef _12}}" f:DataSource="{ExtRef _DataSource}" f:DataSourceMetadata="{ObjRef _13}" f:EditionPanel="{ObjRef _14}" f:SearchPanel="{ObjRef _15}" f:Text="" />
          <s:data id="_2" f:Items="{Binding Path=*.Employees, Mode=TwoWay}" f:TabNavigationMode="ForwardToChildren, ForwardOnly" f:Margins="0;0;5;5" f:Text="&lt;i&gt;Table&lt;/i&gt;" f:Dock="StackFill" f:Anchor="None" f:PreferredHeight="99" f:Columns="{Collection {ObjRef _3}, {ObjRef _4}, {ObjRef _5}, {ObjRef _6}, {ObjRef _7}, {ObjRef _8}, {ObjRef _9}}" f:SourceType="[KF0E]" />
          <s:data id="_3" f:FieldId="FirstName" />
          <s:data id="_4" f:FieldId="LastName" />
          <s:data id="_5" f:FieldId="Company" />
          <s:data id="_6" f:FieldId="Address1" />
          <s:data id="_7" f:FieldId="Address2" />
          <s:data id="_8" f:FieldId="Zip" />
          <s:data id="_9" f:FieldId="City" />
          <s:data id="_10" f:TabNavigationMode="ForwardToChildren, ForwardOnly" f:ChildrenLayoutMode="Stacked" f:ContainerLayoutMode="HorizontalFlow" f:PreferredWidth="200" f:Margins="0;0;0;0" f:PreferredHeight="40" f:Padding="0;0;0;0" f:Dock="StackEnd" f:Anchor="None" f:Children="{Collection {ObjRef _11}}" f:Text="" />
          <s:data id="_11" f:AutoFocus="True" f:AutoEngage="True" f:Aspect="IconButton" f:ContentAlignment="MiddleLeft" f:MinWidth="22" f:TabNavigationMode="None" f:MinHeight="22" f:Margins="5;5;5;5" f:Dock="StackEnd" f:Anchor="None" f:PreferredHeight="22" f:CommandObject="[0009]" f:ActiveState="No" f:PreferredWidth="100" f:VerticalAlignment="Center" />
          <s:data id="_12" f:Value="{Binding Path=*.Employees, Mode=TwoWay}" f:TabNavigationMode="ForwardToChildren, ForwardOnly" f:ChildrenLayoutMode="Stacked" f:ContainerLayoutMode="VerticalFlow" f:Text="&lt;i&gt;Panel&lt;/i&gt;" f:PreferredWidth="260" f:Margins="0;0;5;5" f:PreferredHeight="160" f:_2.ResourceManager="{ExtRef _ResourceManager}" f:Dock="StackEnd" f:Anchor="None" f:PanelId="[KF03]" />
          <s:data id="_13" f:Fields="{Collection {Text '*;[KF0M]'}}" />
          <s:data id="_14" f:TabNavigationMode="ForwardTabPassive" f:ChildrenLayoutMode="Stacked" f:ContainerLayoutMode="VerticalFlow" f:PreferredWidth="475" f:PreferredHeight="200" f:Anchor="BottomLeft" f:Dock="None" f:Margins="0;0;0;0" f:Padding="20;20;20;20" f:_3.DataContext="\[Binding Mode=TwoWay\]" f:DataSource="{ExtRef _DataSource}" f:DataSourceMetadata="{ObjRef _13}" f:EditionPanel="{ObjRef _14}" f:Owner="{ObjRef _1}" f:SearchPanel="{ObjRef _15}" f:Text="" />
          <s:data id="_15" f:TabNavigationMode="ForwardTabPassive" f:ChildrenLayoutMode="Stacked" f:ContainerLayoutMode="VerticalFlow" f:PreferredWidth="475" f:PreferredHeight="350" f:Anchor="BottomLeft" f:Dock="None" f:Margins="0;0;0;0" f:Padding="20;20;16;16" f:_3.DataContext="\[Binding Mode=TwoWay\]" f:DataSource="{ExtRef _DataSource}" f:DataSourceMetadata="{ObjRef _13}" f:EditionPanel="{ObjRef _14}" f:Owner="{ObjRef _1}" f:SearchPanel="{ObjRef _15}" f:Text="" />
        </s:storage>
      </panel>
    </xml>
  </data>
  <data name="DefaultSize">475;350</data>
  <data name="RootEntityId">
  </data>
</bundle>