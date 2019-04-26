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