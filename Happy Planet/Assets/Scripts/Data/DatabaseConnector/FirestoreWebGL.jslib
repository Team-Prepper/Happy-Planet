mergeInto(LibraryManager.library, {
    WebGLConnect: function(path, firebaseConfigValue) {
        
        // TODO: Add SDKs for Firebase products that you want to use
        // https://firebase.google.com/docs/web/setup#available-libraries
        
        // Your web app's Firebase configuration
        // For Firebase JS SDK v7.20.0 and later, measurementId is optional

        var firebaseConfig = JSON.parse(UTF8ToString(firebaseConfigValue));
        
        firebaseApp = firebase.initializeApp(firebaseConfig);
        auth = firebaseApp.auth();

    },
    WebGLAddRecord: function(path, recordJson) {

    },
    WebGLUpdateRecordAt: function(path, recordJson, idx){


    },
    WebGLGetAllRecord: function(path, objectName, callback, fallback) {


    }
 });