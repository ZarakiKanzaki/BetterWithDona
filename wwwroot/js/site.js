$(document).ready(function () {

    setTimeout(function () {
        if (!(typeof $('#txtMessage').attr('value') === 'undefined' || !$('#txtMessage').attr('value'))) {
            $('#mdlMessage').modal('show');
        }
        if (!(typeof $('#ErrMessage').val() === 'undefined' || !$('#ErrMessage').val())) {
            $('#mdlError').modal('show');
        }
    }, 1000);



    $('#frmOffer').validate({
        rules: {
            Name: {
                required: true
            },
            Email: {
                required: true,
                email: true
            },
            CompanyName: {
                required: true
            },
            fileUploaded: {
                required: true
            },
            Message: {
                required: true,
            }
        },
        messages: {
            Name:"Please tell who I'm talking to.",
            CompanyName: "Please insert your Company Name.",
            Message: "Please introduce yourself.",
            fileUploaded: "You should attach your formal offer in order to send a request.",
            Email: {
                required:"Please insert your e-mail.",
                email: "Please insert a real e-mail, don't be shy.",
            },
        },
        submitHandler: function (form) {
            form.submit();
        }
    });

});