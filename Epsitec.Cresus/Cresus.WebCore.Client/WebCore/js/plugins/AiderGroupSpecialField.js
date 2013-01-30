// It would be nice if I found a way to include this file in the visual studio
// project for aider instead of here and include it in a build folder with the
// Cresus.WebCore.Client project. This would ensure that the
// Cresus.WebCore.Client would not become polluted with lots of custom plugins.

Ext.require([
  'Epsitec.cresus.webcore.field.SpecialField',
  'Epsitec.cresus.webcore.plugins.AiderGroupField'
],
function() {
  Ext.define('Epsitec.cresus.webcore.plugins.AiderGroupSpecialField', {
    extend: 'Epsitec.cresus.webcore.field.SpecialField',
    alias: 'widget.epsitec.aidergroupspecialfield',

    /* Config */

    layout: 'hbox',

    initComponent: function() {
      this.callParent();

      var field = Ext.create('Epsitec.AiderGroupField', {
        allowBlank: this.fieldConfig.allowBlank,
        readOnly: this.fieldConfig.readOnly,
        readOnlyCls: 'input-readonly',
        value: this.fieldConfig.value,
        name: this.fieldConfig.name,
        flex: 1,
        getGroupTreeUrl: this.getUrl('GetGroupTree'),
        getSubGroupsUrl: this.getUrl('GetSubGroups')
      });
      this.add(field);
    }
  });
});
