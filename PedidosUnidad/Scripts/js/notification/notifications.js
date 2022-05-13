var i = 1;
var interval = setInterval(function () {


    var rnd = getRandomInt(1, 100);

    if (rnd > 8 && rnd < 15)
        notifyMe(i, rnd);
        
    if (i > 120)
        clearInterval(interval);
    i++;
    

}, 7000);

document.addEventListener('DOMContentLoaded', function () {
    if (!Notification) {
        alert('Desktop notifications not available in your browser. Try Chromium.');
        return;
    }

    if (Notification.permission !== "granted")
        Notification.requestPermission();
});

function notifyMe(noty, cadena) {
    if (Notification.permission !== "granted")
        Notification.requestPermission();
    else {
        var notification = new Notification(noty+'.- Notificación Referencia', {
            icon: '../Content/img/icn_notify.png',
            body: "Información ("+cadena+") Referencias sin iniciar, atrasadas, sin atender, etc...",
        });

        notification.onclick = function () {
            window.open("http://google.com.mx");
        };

    }

}


function getRandomInt(min, max) {
    return Math.floor(Math.random() * (max - min)) + min;
}