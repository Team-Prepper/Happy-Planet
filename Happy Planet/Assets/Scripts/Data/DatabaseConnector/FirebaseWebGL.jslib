mergeInto(LibraryManager.library, {
    FirebaseConnect: function(path, firebaseConfigValue) {
        
        // TODO: Add SDKs for Firebase products that you want to use
        // https://firebase.google.com/docs/web/setup#available-libraries
        
        // Your web app's Firebase configuration
        // For Firebase JS SDK v7.20.0 and later, measurementId is optional

        var firebaseConfig = JSON.parse(UTF8ToString(firebaseConfigValue));
        
        firebaseApp = firebase.initializeApp(firebaseConfig);

    },
    FirebaseAddRecord: function(path, authName, recordJson, idx) {

        var docRef = firebase.database().ref(UTF8ToString(path) + "/" + UTF8ToString(authName));
        
        var up = {};
        up[idx] = JSON.parse(UTF8ToString(recordJson));

        docRef.set(up)
        .then(() => {
            console.log("Document successfully written!");
        })
        .catch((error) => {
            console.error("Error writing document: ", error);
        });

    },
    FirebaseUpdateRecordAt: function(path, authName, recordJson, idx){
        
        var docRef = firebase.database().ref(UTF8ToString(path) + "/" + UTF8ToString(authName));

        var updates = {};
        updates[idx] = JSON.parse(UTF8ToString(recordJson));
        updates[idx + 1] = null;

        docRef.update(updates)
        .then(() => {
            console.log("Document successfully updated!");
        })
        .catch((error) => {
            console.error("Error writing document: ", error);
        });

    },
    FirebaseGetAllRecord: function(path, authName, objectName, callback, fallback) {
        
        var parsedObjectName = UTF8ToString(objectName);
        var parsedCallback = UTF8ToString(callback);
        var parsedFallback = UTF8ToString(fallback);
        
        var docRef = firebase.database().ref(UTF8ToString(path) + "/" + UTF8ToString(authName));
        
        docRef.get().then((snapshot) => {
            if (snapshot.exists()) {
                console.log("Document data:", snapshot.toJSON());
                window.unityInstance.SendMessage(parsedObjectName, parsedCallback, JSON.stringify(snapshot.toJSON()));
            } else {
                console.log("No such document!");
                window.unityInstance.SendMessage(parsedObjectName, parsedFallback, "Network Error");
            }
        }).catch((error) => {
            console.log("Error getting document:", error);
            window.unityInstance.SendMessage(parsedObjectName, parsedFallback, "Network Error");
        });

    }
 });