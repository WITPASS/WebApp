window.getImageInfo = (fileInput) => {
    const readAsDataURL = (fileInput) => {
        return new Promise((resolve, reject) => {
            const reader = new FileReader();
            var file = fileInput.files[0];

            reader.onerror = () => {
                reader.abort();
                reject(new Error("Error reading file"));
            };

            reader.addEventListener("load", () => {
                var img = new Image();

                img.onload = function () {
                    resolve([img.width.toString(), img.height.toString(), file.size.toString(), reader.result]);
                };
        
                img.src = reader.result;

            }, false);

            reader.readAsDataURL(file);
        });
    };

    return readAsDataURL(fileInput);
};

window.localStorageSetItem = (key, value) => {
    const setItem = (key, value) => {
        return localStorage.setItem(key, value);
    };

    return setItem(key, value);
};

window.localStorageGetItem = (key) => {
    const getItem = (key) => {
        return localStorage.getItem(key);
    };

    return getItem(key);
};

window.localStorageRemoveItem = (key) => {
    const removeItem = (key) => {
        return localStorage.removeItem(key);
    };

    return removeItem(key);
}
