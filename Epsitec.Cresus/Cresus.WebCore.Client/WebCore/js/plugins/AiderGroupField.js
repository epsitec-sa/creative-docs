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

    /* Additional methods */

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
