window.readFileAsBase64 = (fileInput) => {
    const readAsDataURL = (fileInput) => {
        return new Promise((resolve, reject) => {
            const reader = new FileReader();

            reader.onerror = () => {
                reader.abort();
                reject(new Error("Error reading file."));
            };

            reader.addEventListener("load", () => {
                resolve(reader.result);
            }, false);

            reader.readAsDataURL(fileInput.files[0]);
        });
    };

    return readAsDataURL(fileInput);
};

window.getImageWidthHeight = (src) => {
    const getWidthHeight = (src) => {
        return new Promise((res, rej) => {
            var img = new Image();

            img.onload = function () {
                res([img.width, img.height]);
            };

            img.src = src;
        });
    };

    return getWidthHeight(src);
};