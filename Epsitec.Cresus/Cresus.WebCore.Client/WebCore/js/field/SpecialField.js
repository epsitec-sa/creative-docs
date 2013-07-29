// This class is the base class of all special fields. Special fields are some
// kind of application defined plugin fields, that may behave in specific ways
// and access the server through AJAX requests.

Ext.require([
  'Epsitec.cresus.webcore.tools.Tools'
],
function() {
  Ext.define('Epsitec.cresus.webcore.field.SpecialField', {
    extend: 'Ext.form.FieldContainer',

    /* Properties */

    entityId: null,
    controllerName: null,
    fieldData: null,
    fieldConfig: null,

    /* Methods */

    callServer: function(name, callback, parameters) {
      Ext.Ajax.request({
        url: this.getUrl(name),
        params: parameters,
        method: 'GET',
        callback: function(options, success, response) {
          this.callServerCallback(success, response, callback);
        },
        scope: this
      });
    },

    callServerCallback: function(success, response, callback) {
      var json = Epsitec.Tools.processResponse(success, response);
      if (json === null) {
        return;
      }
      callback.execute([json.content.data]);
    },

    getUrl: function(name) {
      return 'proxy/specialField/' + this.controllerName + '/' + this.entityId +
          '/' + name;
    }
  });
});
