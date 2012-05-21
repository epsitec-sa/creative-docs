<?xml version="1.0" encoding="utf-8"?>
<bundle name="_KF01" caption="Form.Invoice" type="Panel" culture="fr" rank="1">
  <data name="Panel">
    <xml>
      <panel xmlns:s="http://www.epsitec.ch/XNS/storage-structure-1" xmlns:f="http://www.epsitec.ch/XNS/storage-fields-1">
        <s:storage s:root="_1" s:n_ext="2" s:n_typ="14" s:n_obj="20">
          <s:external s:name="_DataSource" />
          <s:external s:name="_ResourceManager" />
          <s:type id="_1" s:name="Epsitec.Common.UI.Panel" />
          <s:type id="_2" s:name="Epsitec.Common.Support.ResourceManager" />
          <s:type id="_3" s:name="Epsitec.Common.Types.DataObject" />
          <s:type id="_4" s:name="Epsitec.Common.Widgets.FrameBox" />
          <s:type id="_5" s:name="Epsitec.Common.Widgets.Layouts.LayoutEngine" />
          <s:type id="_6" s:name="Epsitec.Common.Widgets.Layouts.GridLayoutEngine" />
          <s:type id="_7" s:name="Epsitec.Common.Widgets.Layouts.RowDefinition" />
          <s:type id="_8" s:name="Epsitec.Common.Widgets.Layouts.ColumnDefinition" />
          <s:type id="_9" s:name="Epsitec.Common.UI.Placeholder" />
          <s:type id="_10" s:name="Epsitec.Common.Widgets.GroupBox" />
          <s:type id="_11" s:name="Epsitec.Common.UI.PanelPlaceholder" />
          <s:type id="_12" s:name="Epsitec.Common.UI.DataSourceMetadata" />
          <s:type id="_13" s:name="Epsitec.Common.UI.Panels.EditPanel" />
          <s:type id="_14" s:name="Epsitec.Common.UI.Panels.SearchPanel" />
          <s:object id="_1" s:type="_1" />
          <s:object id="_2" s:type="_4" />
          <s:object id="_3" s:type="_6" />
          <s:object id="_4" s:type="_7" />
          <s:object id="_5" s:type="_7" />
          <s:object id="_6" s:type="_7" />
          <s:object id="_7" s:type="_7" />
          <s:object id="_8" s:type="_8" />
          <s:object id="_9" s:type="_8" />
          <s:object id="_10" s:type="_8" />
          <s:object id="_11" s:type="_9" />
          <s:object id="_12" s:type="_9" />
          <s:object id="_13" s:type="_4" />
          <s:object id="_14" s:type="_10" />
          <s:object id="_15" s:type="_11" />
          <s:object id="_16" s:type="_10" />
          <s:object id="_17" s:type="_11" />
          <s:object id="_18" s:type="_12" />
          <s:object id="_19" s:type="_13" />
          <s:object id="_20" s:type="_14" />
          <s:data id="_1" f:TabNavigationMode="ForwardTabPassive" f:ChildrenLayoutMode="Stacked" f:ContainerLayoutMode="HorizontalFlow" f:PreferredWidth="547" f:PreferredHeight="189" f:Anchor="BottomLeft" f:Padding="10;10;10;10" f:_2.ResourceManager="{ExtRef _ResourceManager}" f:_1.BundleId="[KF01]" f:_3.DataContext="\[Binding Mode=TwoWay\]" f:Children="{Collection {ObjRef _2}, {ObjRef _13}}" f:DataSource="{ExtRef _DataSource}" f:DataSourceMetadata="{ObjRef _18}" f:EditionPanel="{ObjRef _19}" f:SearchPanel="{ObjRef _20}" f:Text="" />
          <s:data id="_2" f:TabNavigationMode="ForwardToChildren, ForwardOnly" f:ChildrenLayoutMode="Grid" f:ContainerLayoutMode="VerticalFlow" f:Anchor="None" f:PreferredWidth="332.5" f:PreferredHeight="95" f:MinWidth="50" f:MinHeight="50" f:Padding="0;0;0;0" f:Dock="StackBegin" f:Margins="0;0;0;0" f:_5.LayoutEngine="{ObjRef _3}" f:Children="{Collection {ObjRef _11}, {ObjRef _12}}" f:Text="" />
          <s:data id="_3" f:ColumnDefinitions="{Collection {ObjRef _4}, {ObjRef _5}, {ObjRef _6}, {ObjRef _7}}" f:RowDefinitions="{Collection {ObjRef _8}, {ObjRef _9}, {ObjRef _10}}" />
          <s:data id="_4" f:Height="Auto" f:MinHeight="20" />
          <s:data id="_5" f:Height="Auto" f:MinHeight="20" />
          <s:data id="_6" f:Height="Auto" f:MinHeight="20" />
          <s:data id="_7" f:Height="Auto" f:MinHeight="20" />
          <s:data id="_8" f:Width="Auto" f:MinWidth="20" />
          <s:data id="_9" f:Width="Auto" f:MinWidth="60" />
          <s:data id="_10" f:Width="*100" f:MinWidth="20" />
          <s:data id="_11" f:Value="{Binding Path=*.Title, Mode=TwoWay}" f:TabNavigationMode="ForwardToChildren, ForwardOnly" f:Text="&lt;i&gt;TextField&lt;/i&gt;" f:MinWidth="20" f:Anchor="None" f:MinHeight="20" f:Dock="None" f:PreferredHeight="20" f:Margins="0;0;0;0" f:PreferredWidth="146" f:ContainerLayoutMode="HorizontalFlow" f:_6.ColumnSpan="3" f:_6.Column="0" f:_6.Row="0" f:VerticalAlignment="Center" />
          <s:data id="_12" f:Value="{Binding Path=*.TotalAmount, Mode=TwoWay}" f:TabNavigationMode="ForwardTabPassive" f:Text="&lt;i&gt;TextField&lt;/i&gt;" f:MinWidth="20" f:MinHeight="20" f:Anchor="None" f:Dock="None" f:_6.Column="0" f:_6.Row="2" f:ContainerLayoutMode="HorizontalFlow" />
          <s:data id="_13" f:TabNavigationMode="ForwardToChildren, ForwardOnly" f:ChildrenLayoutMode="Stacked" f:ContainerLayoutMode="VerticalFlow" f:PreferredWidth="180" f:Margins="5;0;0;2" f:PreferredHeight="100" f:MinWidth="50" f:MinHeight="50" f:Padding="0;0;0;0" f:Dock="StackBegin" f:Anchor="None" f:Children="{Collection {ObjRef _14}, {ObjRef _16}}" f:Text="" />
          <s:data id="_14" f:TabNavigationMode="ForwardToChildren, ForwardOnly" f:ChildrenLayoutMode="Stacked" f:ContainerLayoutMode="VerticalFlow" f:Anchor="None" f:PreferredWidth="160" f:PreferredHeight="75" f:MinWidth="50" f:MinHeight="50" f:Dock="StackBegin" f:Margins="5;5;5;5" f:CaptionId="[KF0I]" f:_6.Column="1" f:_6.Row="3" f:Children="{Collection {ObjRef _15}}" />
          <s:data id="_15" f:Value="{Binding Path=*.BillingAddress, Mode=TwoWay}" f:TabNavigationMode="ForwardTabPassive" f:ChildrenLayoutMode="Anchored" f:ContainerLayoutMode="HorizontalFlow" f:Anchor="None" f:Text="&lt;i&gt;Panel&lt;/i&gt;" f:PreferredWidth="137" f:PreferredHeight="57" f:MinWidth="50" f:MinHeight="50" f:_2.ResourceManager="{ExtRef _ResourceManager}" f:Dock="StackEnd" f:Margins="0;0;0;0" f:PanelId="[KF]" />
          <s:data id="_16" f:TabNavigationMode="ForwardToChildren, ForwardOnly" f:ChildrenLayoutMode="Stacked" f:ContainerLayoutMode="VerticalFlow" f:Anchor="None" f:PreferredWidth="160" f:PreferredHeight="75" f:MinWidth="50" f:MinHeight="50" f:Dock="StackBegin" f:Margins="5;5;5;5" f:CaptionId="[KF0J]" f:_6.Column="2" f:_6.Row="3" f:Children="{Collection {ObjRef _17}}" />
          <s:data id="_17" f:Value="{Binding Path=*.ShippingAddress, Mode=TwoWay}" f:TabNavigationMode="ForwardTabPassive" f:ChildrenLayoutMode="Anchored" f:ContainerLayoutMode="HorizontalFlow" f:Anchor="None" f:Text="&lt;i&gt;Panel&lt;/i&gt;" f:PreferredWidth="137" f:PreferredHeight="57" f:MinWidth="50" f:MinHeight="50" f:_2.ResourceManager="{ExtRef _ResourceManager}" f:Dock="StackEnd" f:Margins="0;0;0;0" f:PanelId="[KF]" f:TabIndex="1" />
          <s:data id="_18" f:Fields="{Collection {Text '*;[KF0G]'}}" />
          <s:data id="_19" f:TabNavigationMode="ForwardTabPassive" f:ChildrenLayoutMode="Anchored" f:ContainerLayoutMode="VerticalFlow" f:PreferredWidth="389" f:PreferredHeight="332" f:Anchor="BottomLeft" f:Dock="None" f:Margins="0;0;0;0" f:Padding="20;20;20;20" f:_3.DataContext="\[Binding Mode=TwoWay\]" f:DataSource="{ExtRef _DataSource}" f:DataSourceMetadata="{ObjRef _18}" f:EditionPanel="{ObjRef _19}" f:Owner="{ObjRef _1}" f:SearchPanel="{ObjRef _20}" f:Text="" />
          <s:data id="_20" f:TabNavigationMode="ForwardTabPassive" f:ChildrenLayoutMode="Stacked" f:ContainerLayoutMode="HorizontalFlow" f:PreferredWidth="547" f:PreferredHeight="189" f:Anchor="BottomLeft" f:Dock="None" f:Margins="0;0;0;0" f:Padding="10;10;10;10" f:_3.DataContext="\[Binding Mode=TwoWay\]" f:DataSource="{ExtRef _DataSource}" f:DataSourceMetadata="{ObjRef _18}" f:EditionPanel="{ObjRef _19}" f:Owner="{ObjRef _1}" f:SearchPanel="{ObjRef _20}" f:Text="" />
        </s:storage>
      </panel>
    </xml>
  </data>
  <data name="DefaultSize">547;189</data>
  <data name="RootEntityId"></data>
</bundle>