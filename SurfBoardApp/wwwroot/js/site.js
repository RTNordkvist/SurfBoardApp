// Write your JavaScript code.
function readURL(input) { //Defines a function named readURL that takes an input element as its argument.
    if (input.files && input.files[0]) { // Checks if the input element has a files property and if the first file in the array exists.
        var reader = new FileReader(); //Creates a new FileReader object.

        reader.onload = function (e) { //Defines an event handler function to be executed when the load event is fired by the FileReader.
            $('#preview').attr('src', e.target.result).show(); // Sets the src attribute of the image element with ID preview to the data URL of the loaded file, and shows the element.
        };

        reader.readAsDataURL(input.files[0]); //Starts reading the first file in the input element's files array as a data URL.
    }
}

$("#image").change(function () { //Defines an event handler function to be executed when the value of the input element with ID image changes.
    readURL(this); //Calls the readURL function with this as its argument, which refers to the input element with ID image. This triggers the loading and previewing of the selected image file.
});

