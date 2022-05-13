//FUNCION IR A VISTA
function go_to_view(url) {
    show_loading();
    setTimeout(function () { window.location.href = url; }, 0);
}

//FUNCION IR A VISTA
function go_to(url) {
    window.location.href = url;
}

//MOSTRAR MODAL DE LOADING
function show_loading() {
    var html_load = '';

    html_load += '     <div id="loader-outx" > ';
    html_load += '          <div class="ctn-loading"> ';
    html_load += '              <span class="spanin-l" > Procesando</span > ';
    html_load += '              <span class="spanin-li l-1"></span> ';
    html_load += '              <span class="spanin-li l-2"></span> ';
    html_load += '              <span class="spanin-li l-3"></span> ';
    html_load += '              <span class="spanin-li l-4"></span> ';
    html_load += '              <span class="spanin-li l-5"></span> ';
    html_load += '              <span class="spanin-li l-6"></span> ';
    html_load += '          </div > ';
    html_load += '     </div> ';

    $("body").append(html_load);
}

//MOSTRAR LOADING SECCION
function show_load_seccion() {

    var html_load = '';//'<div class="sk-rotating-plane"></div>';
    html_load += '<div style="width:61px; height:61px; text-align:center; margin:35% auto;">';
    html_load += '    <img src="../Content/img/load.gif" width="60" height="60" />';
    html_load += '</div>';

    return html_load;
}

//CERRAR MODAL DE LOADING
function close_loading() {
    $('#loader-outx').fadeOut();
    $('#loader-outx').remove();
}

//SHOW EXITO
function show_exito(msg) {
    var html_five = get_html_mensaje('success', msg);
    $("body").append(html_five);

    setTimeout(function () {
        $('.appToast').addClass('appToast-active').delay(5500)
        .queue(function () {
            $(this).removeClass("appToast-active");
            $(this).dequeue();
            setTimeout(function () { $(".appToast").remove(); }, 15);
        }); }, 60);
}

//SHOW ERROR
function show_error(msg) {
    var html_five = get_html_mensaje('error', msg);
    $("body").append(html_five);
    setTimeout(function () {
        $('.appToast').addClass('appToast-active').delay(5500)
            .queue(function () {
                $(this).removeClass("appToast-active");
                $(this).dequeue();
                setTimeout(function () { $(".appToast").remove(); }, 15);
            });
    }, 60);
}

//SHOW WARNING
function show_warning(msg) {
    var html_five = get_html_mensaje('warning', msg);
    $("body").append(html_five);
    setTimeout(function () {
        $('.appToast').addClass('appToast-active').delay(5500)
            .queue(function () {
                $(this).removeClass("appToast-active");
                $(this).dequeue();
                setTimeout(function () { $(".appToast").remove(); }, 15);
            });
    }, 60);
}

//SHOW INFO
function show_info(msg) {
    var html_five = get_html_mensaje('info', msg);
    $("body").append(html_five);
    setTimeout(function () {
        $('.appToast').addClass('appToast-active').delay(5500)
            .queue(function () {
                $(this).removeClass("appToast-active");
                $(this).dequeue();
                setTimeout(function () { $(".appToast").remove(); }, 15);
            });
    }, 60);
}

function closedWind() {
    $('.message-original').removeClass('is-visible');
    setTimeout(function () { $('.modalConfirm').remove(); }, 50);
}

//GET MENSAJE 
function get_html_mensaje(tipo, msg) {
    var clase = '';
    var imagen = '';
    var title = '';

    if (tipo == 'success') {
        imagen = 'fa-check';
        clase = 'success-notify';
        title = 'Éxito';
        //Proceso llevado correctamente
    }
    if (tipo == 'error') {
        imagen = 'fa-close';
        clase = 'dangeryn';
        title = 'Error';
        //Ocurrio un error dentro del proceso
    }
    if (tipo == 'warning') {
        imagen = 'fa-exclamation';
        clase = 'warningyn';
        title = 'Precaución';
        //Este proceso presento un inconveniente
    }
    if (tipo == 'info') {
        imagen = 'fa-info';
        clase = 'infoyn';
        title = 'Información';
        // El proceso es incompleto debe ...
    }

    var accion_btn = 'closedWind();';

    var html_ = '';

    html_ += '<div class="appToast appToast-top-right"> ';
    html_ += '    <div class="notifyn ' + clase+'"> ';
    html_ += '        <h3 class="title-notify"><i class="fa ' + imagen + '"></i> ' + title + '</h3> ';
    html_ += '        <p class="message-notify">'+msg+'</p> ';
    html_ += '    </div> ';
    html_ += '</div ';

    return html_;
}

function showConfirmOverride(msg, funcionConfirmar) {
    var html_ = '';

    html_ += '<div id="loader-outx" class="modalConfirm"> ';
    html_ += '  <div class="message-original"> ';
    html_ += '    <div class="closedin"> ';
    html_ += '        <i onclick="closedWind();" class="fa fa-close"></i> ';
    html_ += '    </div> ';
    html_ += '    <div class="icono-msg"> ';
    html_ += '        <img src="../../Content/img/_question.png"/> ';
    html_ += '    </div> ';
    html_ += '    <div class="title-msgin"> ';
    html_ += '        <p> Confirmación </p> ';
    html_ += '    </div> ';
    html_ += '    <div class="msg-descrip-mdl"> ';
    html_ += '        <p> ' + msg + ' </p> ';
    html_ += '    </div> ';
    html_ += '    <div class="div-btn-msg"> ';
    html_ += '        <button type="button" onclick="closedWind();" class="btn btn-raised btn-danger btn-minin" data-toggle="tooltip" data-placement="top" title="" data-original-title="Rechazar" style="height: 40px; width: 40px; float:none;"> <i class="fa fa-close"></i> </button> ';
    html_ += '        <button type="button" onclick="' + funcionConfirmar + '" class="btn btn-raised btn-success btn-minin" data-toggle="tooltip" data-placement="top" title="" data-original-title="Aceptar" style="height: 40px; width: 40px; float:none;"> <i class="fa fa-check"></i> </button> ';
    html_ += '    </div> ';
    html_ += '  </div> ';
    html_ += '</div> ';


    $("body").append(html_);
    setTimeout(function () { $('.message-original').addClass('is-visibling'); }, 60);
}


//Funcion para hacer un input solo numeros con decimal
function justNumberDecimal(elemento) {

    $("#" + elemento).keydown(function (e) {
        // Allow: backspace, delete, tab, escape, enter and .
        if ($.inArray(e.keyCode, [46, 8, 9, 27, 13, 110, 190]) !== -1 ||
            // Allow: Ctrl+A, Command+A
            (e.keyCode === 65 && (e.ctrlKey === true || e.metaKey === true)) ||
            // Allow: home, end, left, right, down, up
            (e.keyCode >= 35 && e.keyCode <= 40)) {
            // let it happen, don't do anything
            return;
        }
        // Ensure that it is a number and stop the keypress
        if ((e.shiftKey || (e.keyCode < 48 || e.keyCode > 57)) && (e.keyCode < 96 || e.keyCode > 105)) {
            e.preventDefault();
        }
        //justNumberDecimal(e);
    });

}

