function mostrarLoad() {
    
    $(".bg_load").fadeIn("slow");
    $(".wrapper").fadeIn("slow");

    console.log($(".bg_load"));
}
function ocultarLoad() {
    $(".bg_load").fadeOut("slow");
    $(".wrapper").fadeOut("slow");
}

function PeticionAjaxForm(data, urlAjax, codeSuccess, codeError) {
    mostrarLoad();
    $.ajax({
        type: "POST",
        dataType: "json",
        url: urlAjax,
        data: data,
        success: function (msg) {
            if (!msg.Error) {
                eval(codeSuccess);
            }
            else {
                toastr["error"](msg.Mensaje);
                eval(codeError);
            }
        }
    });
}

function ValidatorForm(form, urlAjax, codeSuccess, codeError) {
    console.log(urlAjax);
    form.submit(function () {
        mostrarLoad();

        $("#errormessage").removeClass("show");
        var f = $(this).find('.form-group,.input-group'),
        ferror = false,
        emailExp = /^[^\s()<>@,;:\/]+@\w[\w\.-]+\.[a-z]{2,}$/i;

        f.children('input').each(function () { // run all inputs

            var i = $(this); // current input
            var rule = i.attr('data-rule');

            if (rule !== undefined) {
                var ierror = false; // error flag for current input
                var pos = rule.indexOf(':', 0);
                if (pos >= 0) {
                    var exp = rule.substr(pos + 1, rule.length);
                    rule = rule.substr(0, pos);
                } else {
                    rule = rule.substr(pos + 1, rule.length);
                }

                switch (rule) {
                    case 'required':
                        if (i.val() === '') { ferror = ierror = true; }
                        break;

                    case 'minlen':
                        if (i.val().length < parseInt(exp)) { ferror = ierror = true; }
                        break;

                    case 'email':
                        if (!emailExp.test(i.val())) { ferror = ierror = true; }
                        break;

                    case 'checked':
                        if (!i.attr('checked')) { ferror = ierror = true; }
                        break;

                    case 'regexp':
                        exp = new RegExp(exp);
                        if (!exp.test(i.val())) { ferror = ierror = true; }
                        break;
                }
                i.next('.validation').html((ierror ? (i.attr('data-msg') !== undefined ? i.attr('data-msg') : 'wrong Input') : '')).show('blind');
            }
        });
        f.children('textarea').each(function () { // run all inputs

            var i = $(this); // current input
            var rule = i.attr('data-rule');

            if (rule !== undefined) {
                var ierror = false; // error flag for current input
                var pos = rule.indexOf(':', 0);
                if (pos >= 0) {
                    var exp = rule.substr(pos + 1, rule.length);
                    rule = rule.substr(0, pos);
                } else {
                    rule = rule.substr(pos + 1, rule.length);
                }

                switch (rule) {
                    case 'required':
                        if (i.val() === '') { ferror = ierror = true; }
                        break;

                    case 'minlen':
                        if (i.val().length < parseInt(exp)) { ferror = ierror = true; }
                        break;
                }
                i.next('.validation').html((ierror ? (i.attr('data-msg') != undefined ? i.attr('data-msg') : 'wrong Input') : '')).show('blind');
            }
        });
        if (ferror) {
            ocultarLoad();
            return false;
        }
        else {
            var str = $(this).serialize();
            console.log("hace peticion");
            $("#sendmessage").addClass("show");
            $.ajax({
                type: "POST",
                dataType: "json",
                url: urlAjax,
                data: str,
                success: function (msg) {
                    //alert(msg);
                    $("#sendmessage").removeClass("show");
                    //console.log(msg);
                    if (!msg.Error) {
                        $("#errormessage").removeClass("show");
                        eval(codeSuccess);
                    }
                    else {
                        $("#errormessage").addClass("show");
                        $('#errormessage').html(msg.Mensaje);
                        eval(codeError);
                        ocultarLoad();
                    }

                    

                }
            });

            return false;
        }
    });
}