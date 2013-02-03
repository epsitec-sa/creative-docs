Ext.define('Epsitec.cresus.webcore.tools.ViewMode', {
  alternateClassName: ['Epsitec.ViewMode'],

  /* Static methods */

  statics: {

    // The set view mode is not used directly in the javascript client but only
    // on the server. The other view modes are not used at all in the webcore
    // application but only in the desktop application.

    summary: '1',
    edition: '2',
    action: '6',
    brickCreation: '8',
    brickDeletion: '9'
  }
});
