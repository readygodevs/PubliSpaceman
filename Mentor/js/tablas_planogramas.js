consecutivo = 0;
incremento = 1.5;



function font_size() {
    console.log("ajustar size");
    $('.fitText').each(function () {
        width = $(this).width();
        height = $(this).height();
        px2 = width * height;//pixeles cuadrados


        horizontal = $(this).attr("class").indexOf("rotate") == -1;

        if (horizontal) { // alineación horizontal de texto
            font = 18;
            if (px2 >= 5800 && px2 < 6000) {
                font = 15;
            } else if (px2 >= 5400 && px2 < 5800) {
                font = 14;
            } else if (px2 >= 5200 && px2 < 5600) {
                font = 13;
            } else if (px2 >= 5000 && px2 < 5400) {
                font = 12;
            } else if (px2 >= 4800 && px2 < 5200) {
                font = 11;
            } else if (px2 >= 4600 && px2 < 4800) {
                font = 10;
            }
        } else {// alineación vertical de texto
            //if ("PRESERVATIVO PREMIER COLAGE MIX C/3 7.00cm" == $(this).text()) {
            //    console.log("PX2:" + px2);
            //}
            font = 10;
            //if (px2 >= 4000 && px2 < 4200) {
            //    font = 14;
            //} else if (px2 >= 3800 && px2 < 4000) {
            //    font = 13;
            //} else if (px2 >= 2800 && px2 < 3800) {
            //    font = 12;
            //} else if (px2 >= 2600 && px2 < 2800) {
            //    font = 11;
            //} else if (px2 >= 2400 && px2 < 2600) {
            //    font = 10;
            //} else
            if (px2 >= 2200 && px2 < 2400) {
                font = 9;
            } else if (px2 >= 2000 && px2 < 2200) {
                font = 8;
            } else if (px2 >= 1800 && px2 < 2000) {
                font = 7;
            } else if (px2 >= 1600 && px2 < 1800) {
                font = 6;
            } else if (px2 >= 1400 && px2 < 1600) {
                font = 6;
            }

        }


        //font = 20;
        $(this).css("font-size", font + "px");

        span = $(this).children();
        widthSpan = span.width();

        heightSpan = span.height();

        //if ("PRESERVATIVO PREMIER COLAGE MIX C/3 7.00cm" == $(this).text()) {
        //    $("body").append("<label>PRESERVATIVO PREMIER COLAGE MIX C/3 7.00cm FUENTE ANTES:" + font + "</label>")
        //}

        while (widthSpan > width) {
            font = font - 1.5;

            $(this).css("font-size", font + "px");

            span = $(this).children();
            widthSpan = span.width();

        }

        if ($(this).is("div")) {
            while (heightSpan > height) {
                font = font - 1.5;

                $(this).css("font-size", font + "px");

                span = $(this).children();
                heightSpan = span.height();
            }
        }
        //}

        //if ("PRESERVATIVO PREMIER COLAGE MIX C/3 7.00cm" == $(this).text()) {
        //    $("body").append("<label>PRESERVATIVO PREMIER COLAGE MIX C/3 7.00cm FUENTE DESPUES:" + font + "</label>")
        //}
    });
}

jQuery(document).ready(function ($) {

    //$(".panel").each(function () {

    //    //$("#details").clone().removeAttr("id")
    //    //  .css({ 'margin-top': '50px', 'background': 'red' })
    //    //  .appendTo("#wrapper").children("#details-child")
    //    //  .removeAttr("id").css({ background: "red" });

    //    incrementoPanel = incremento;

    //    while ($(this).height() * incrementoPanel > 1100) {
    //        incrementoPanel = incrementoPanel - 0.1;
    //    }

    //    $($(this)).clone()
    //      .css({
    //          "margin-left": $(this).width() > 300 ? '50%' : '30%',
    //          "width": $(this).width() * incrementoPanel,
    //          "height": $(this).height() * incrementoPanel
    //      })
    //      .attr("id", "copia" + consecutivo)
    //      .appendTo($(this).parent());



    //    $("#copia" + consecutivo).children().each(function () {

    //        $(this).width($(this).width() * incrementoPanel);
    //        $(this).height($(this).height() * incrementoPanel);
    //        $(this).css("bottom", Number($(this).css("bottom").replace("px", "")) * incrementoPanel);
    //        $(this).css("left", Number($(this).css("left").replace("px", "")) * incrementoPanel);


    //        //si es imagen sustituir por div de descripcion
    //        if ($(this).is("img")) {
    //            //$(this).load(function () {

    //            producto = $(this).attr("title");
    //            valoresClase = $(this).attr("class");
    //            vertical = 0;
    //            if (valoresClase != "h") vertical = valoresClase.split("_")[1];

    //            //if (producto == "QUESO CUBICADO PROVIDENCIA 250 G 14.00cm") {
    //            //    console.log(valoresClase);
    //            //}

    //            $("#copia" + consecutivo).append("<div style='" +
    //                (valoresClase != "h" ?
    //                "border:1px solid;"
    //                :
    //                "") +
    //                "width:" + $(this).width() + "px;" +
    //                "height:" + $(this).height() + "px;" +
    //                "bottom:" + $(this).css("bottom") + ";" +
    //                "left:" + $(this).css("left") + ";'" +
    //                " class='descripcion'>" +
    //                (valoresClase != "h" ? "" : (($(this).height() > $(this).width() * incrementoPanel) ?
    //                "<div class='fitText rotate' style='height:" + $(this).width() +
    //                "px;width:" + $(this).height() + "px;'><span style='height:" + $(this).width() +
    //                "px;width:" + $(this).height() + "px;'>" + producto + "</span></div>"
    //                :
    //                "<div class='fitText' style='width:" + $(this).width() +
    //                "px;height:" + $(this).height() + "px;'><span>" + producto + "</span></div>"
    //                )) +
    //                "</div>");

    //            if (valoresClase != "v" && valoresClase != "h") {
    //                if (vertical > 0) {

    //                    $("#copia" + consecutivo).append(
    //                        "<div style='" +
    //                    "z-index: 10;" +
    //"background: none;" +
    //"border: none;" +
    //                    "width:" + $(this).width() + "px;" +
    //                    "height:" + $(this).height() * vertical + "px;" +
    //                    "bottom:" + $(this).css("bottom") + ";" +
    //                    "left:" + $(this).css("left") + ";'" +
    //                    " class='descripcion'>" +
    //                    (($(this).height() * vertical > $(this).width() * incrementoPanel) ?
    //"<div class='fitText rotate' style=';height:" + $(this).width() +
    //"px;width:" + ($(this).height() * vertical) + "px;'><span>" + producto + "</span></div>"
    //                    :
    //"<div class='fitText' style='width:" + $(this).width() +
    //"px;height:" + ($(this).height() * vertical) + "px;'><span>" + producto + "</span></div>"
    //                    ) +
    //                    "</div>");
    //                }
    //            }



    //            $(this).remove();

    //            //});
    //        }
    //    });

    //    consecutivo++;
    //});

    font_size();

    //$('.fitText').textfill({
    //    minFontPixels: 0,
    //    maxFontPixels: 20,
    //    debug: true
    //}); 
    //$(".fitText").fitText(1.5, { maxFontSize: '8.3px' }); //1.2, { minFontSize: '4px', maxFontSize: '75px' }

    //$("img").each(function () {

    //    $(this).load(function () {

    //        id = $(this).attr("id");

    //        $('[name="' + id + '"]').css({ "width": ($(this).width() * 1.25), "height": ($(this).height()) });

    //        producto = $(this).attr("title");
    //        //console.log(producto);

    //        if ($(this).attr("class") == "ignorar") return;


    //        if ($(this).attr("class") != "") {
    //            console.log($(this).attr("class"));
    //            valores = $(this).attr("class").split('_');

    //            //$('[name="' + id.spl + '"]').append("<div style='width:" + $(this).height() + "px;height:" + $(this).width() + "px;' class='rotate fitText'>" + producto + "</div>");
    //            width = $('[name="' + id + '"]').width();
    //            height = $('[name="' + id + '"]').height();
    //            left = $('[name="' + id + '"]').css("left");

    //            $divSegment = $('[name="' + id + '"]').parents('div[class="segment"]').eq(0);
    //            if (width < 50) {
    //                $divSegment.
    //                    append("<div style='left:" + left + ";position: absolute;bottom: 0;height:" + height * Number(valores[2]) + "px;width:" + width +
    //                    "px;'>" +
    //                    "<div class='fitText rotate' style='font-weight:bold;width:" + height * Number(valores[2]) +
    //                    "px;height:" + width + "px;'>" + producto + "</div></div>");

    //            } else {
    //                $divSegment.
    //                    append("<div class='fitText' style='position: absolute;bottom: 0;width:" + width +
    //                    "px;height:" + height * Number(valores[2]) + "px;left:" + left + "'>" + producto + "</div>");
    //            }

    //        } else {



    //            if ($(this).width() < 50) {
    //                $('[name="' + id + '"]').height($(this).height() * 1.25);
    //                //$('[name="' + id + '"]').width($(this).width() * 1.25);
    //                //$('[name="' + id + '"]').css("bottom", (Number(($(this).css("bottom").replace("px", ""))) * 0.8) + "px");
    //                if ($('[name="' + id + '"]').css("bottom") != "0px") {
    //                    console.log($('[name="' + id + '"]').css("bottom"));
    //                    $('[name="' + id + '"]').css("bottom", "");
    //                    $('[name="' + id + '"]').css("top", "5px");
    //                }
    //                $('[name="' + id + '"]').append("<div style='width:" + $(this).height()*1.25 + "px;height:" + $(this).width()*1.25 + "px;' class='rotate fitText'>" + producto + "</div>");
    //            } else {
    //                //console.log("producto:" + producto + ",height:" + $(this).height());
    //                $('[name="' + id + '"]').append("<div class='fitText'>" + producto + "</div>");
    //            }
    //        }


    //        $(".fitText").fitText(1.2, { minFontSize: '5.5px', maxFontSize: '75px' });
    //        //alert($(".segment").width());
    //        //}


    //    });
    //});
});