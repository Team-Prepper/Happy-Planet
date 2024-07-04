mergeInto(LibraryManager.library, {
    FirestoreConnect: function(path, firebaseConfigValue) {
        
        // TODO: Add SDKs for Firebase products that you want to use
        // https://firebase.google.com/docs/web/setup#available-libraries
        
        // Your web app's Firebase configuration
        // For Firebase JS SDK v7.20.0 and later, measurementId is optional

        var firebaseConfig = JSON.parse(UTF8ToString(firebaseConfigValue));
        
        firebaseApp = firebase.initializeApp(firebaseConfig);

    },
    FirestoreAddRecord: function(path, authName, recordJson) {

    },
    FirestoreUpdateRecordAt: function(path, authName, recordJson, idx){
        
        var docRef = firebase.firestore().collection(UTF8ToString(path)).doc(UTF8ToString(authName));

        var updates = {};
        updates[idx] = UTF8ToString(recordJson);

        return docRef.update(updates)
        .then(() => {
            console.log("Document successfully updated!");
        })
        .catch((error) => {
            // The document probably doesn't exist.
            console.error("Error updating document: ", error);
        });

    },
    FirestoreGetAllRecord: function(path, authName, objectName, callback, fallback) {
        
        var parsedObjectName = UTF8ToString(objectName);
        var parsedCallback = UTF8ToString(callback);
        var parsedFallback = UTF8ToString(fallback);
        
        var docRef = firebase.firestore().collection(UTF8ToString(path)).doc(UTF8ToString(authName));
        
        docRef.get().then((doc) => {
            if (doc.exists) {
                console.log("Document data:", doc.data());
                window.unityInstance.SendMessage(parsedObjectName, parsedCallback, JSON.stringify(doc.data()));
            } else {
                // doc.data() will be undefined in this case
                console.log("No such document!");
            }
        }).catch((error) => {
            console.log("Error getting document:", error);
        });

    }
 });