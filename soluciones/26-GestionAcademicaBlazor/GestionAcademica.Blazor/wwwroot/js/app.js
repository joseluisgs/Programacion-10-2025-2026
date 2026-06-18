/*
   app.js — Código JavaScript de apoyo para la aplicación Blazor Server.

   ¿POR QUÉ NECESITAMOS JAVASCRIPT?
   Blazor ejecuta C# en el servidor. Todo el código C# corre allí.
   Pero algunas cosas SOLO pueden hacerse en el navegador del usuario:
   - Mostrar cuadros de diálogo nativos (confirm, alert)
   - Acceder a localStorage (almacenamiento local del navegador)
   - Manipular el DOM directamente (como cambiar atributos del <html>)
   - Copiar al portapapeles
   - Abrir ventanas/pestañas nuevas

   Para eso existe IJSRuntime en C#: permite llamar funciones JavaScript
   desde C# a través de la conexión SignalR.

   IMPORTANTE: Las funciones deben ser GLOBALES (colgar de window)
   para que Blazor pueda encontrarlas. No valen funciones anidadas
   dentro de objetos cuando se llaman con InvokeAsync - solo funciones
   sueltas como window.getTheme, window.setTheme.
*/

/*
   window.blazorInterop: objeto agrupador de utilidades JS.
   Almacena varias funciones en una sola variable para no contaminar
   el espacio global con decenas de nombres sueltos.

   Cada función envuelve una API del navegador:
*/
window.blazorInterop = {
    /*
       confirm(message): muestra un cuadro de diálogo nativo del navegador
       con los botones Aceptar/Cancelar. Devuelve true o false.
       Se usa desde Blazor con: JS.InvokeAsync<bool>("blazorInterop.confirm", msg)
       ¡OJO! blazorInterop.confirm NO funciona como identificador directo
       en InvokeAsync (los puntos no se resuelven). Para llamarla desde
       C# se usa eval("blazorInterop.confirm(...)") o se referencia como
       función anidada a través de window.
    */
    confirm: function (message) {
        // return confirm(message): llama al confirm nativo del navegador
        // y retorna true/false
        return confirm(message);
    },

    /*
       alert(message): muestra un cuadro de alerta simple.
    */
    alert: function (message) {
        alert(message);
    },

    /*
       openUrl(url): abre una URL en una nueva pestaña del navegador.
       window.open(url, '_blank'): abre ventana/pestaña nueva.
       El segundo parámetro '_blank' indica "nueva pestaña".
    */
    openUrl: function (url) {
        window.open(url, '_blank');
    },

    /*
       copyToClipboard(text): copia un texto al portapapeles del sistema.
       navigator.clipboard.writeText(text): API moderna de portapapeles.
       .catch(...): si falla (ej. usuario no dio permiso), no hace nada.
    */
    copyToClipboard: function (text) {
        navigator.clipboard.writeText(text).catch(function () { });
    }
};

// Fin del objeto blazorInterop
