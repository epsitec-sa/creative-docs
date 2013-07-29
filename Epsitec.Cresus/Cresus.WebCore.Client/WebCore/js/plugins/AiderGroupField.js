// This class is an edition field that allows a user to pick a group. It is used
// as a child of AiderGroupSpecialField. We need a separate classe because
// AiderGroupSpecialField inherits from SpecialField and we need to inherit
// ReferenceField. And there is no multiple inheritance in the ExtJs class
// system.

Ext.require([
  'Epsitec.cresus.webcore.field.ReferenceField',
  'Epsitec.cresus.webcore.plugins.AiderGroupPicker',
  'Epsitec.cresus.webcore.tools.Callback'
],
function() {
  Ext.define('Epsitec.cresus.webcore.plugins.AiderGroupField', {
    extend: 'Epsitec.cresus.webcore.field.ReferenceField',
    alternateClassName: ['Epsitec.AiderGroupField'],

    /* Properties */

    getGroupTreeUrl: null,
    getSubGroupsUrl: null,

    /* Methods */

    onPickClick: function() {
      Ext.Ajax.request({
        url: this.getGroupTreeUrl,
        params: {
          group: this.getSubmitValue()
        },
        method: 'GET',
        callback: this.getGroupTreeCallback,
        scope: this
      });
    },

    getGroupTreeCallback: function(options, success, response) {
      var json, picker;

      json = Epsitec.Tools.processResponse(success, response);
      if (json === null) {
        return;
      }

      picker = Ext.create('Epsitec.AiderGroupPicker', {
        callback: Epsitec.Callback.create(this.onPickClickCallback, this),
        selectedGroupId: this.getSubmitValue(),
        groups: json.content.data,
        url: this.getSubGroupsUrl
      });
      picker.show();
    }
  });
});
