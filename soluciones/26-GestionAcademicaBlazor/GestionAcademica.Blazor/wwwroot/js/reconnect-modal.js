/*
   reconnect-modal.js — Gestión de la reconexión de Blazor Server.

   ¿POR QUÉ ES NECESARIO?
   Blazor Server funciona con una conexión permanente llamada SignalR
   (similar a WebSockets). El servidor envía actualizaciones de la UI
   al navegador a través de esta conexión.

   Si el usuario pierde internet, el servidor se reinicia o hay un corte
   temporal, la conexión SignalR se rompe. Blazor detecta esto y:

   1. Dispara eventos en el DOM para que podamos mostrar/ocultar un modal
   2. Ofrece APIs (Blazor.reconnect(), Blazor.resumeCircuit()) para
      intentar reconectar sin perder el estado de la aplicación

   SIN ESTE SCRIPT: si se pierde la conexión, el usuario ve una página
   congelada sin saber qué pasa, y la única solución es recargar (F5).
*/

/*
   ============================================
   PASO 1: OBTENER REFERENCIAS A LOS ELEMENTOS DEL DOM
   ============================================

   document.getElementById("id"): busca en el HTML un elemento que tenga
   ese id. Equivalente a: <dialog id="components-reconnect-modal"> ...
   Devuelve el objeto del DOM (no un string, sino el elemento en sí).

   Estos elementos se definen en el componente Blazor ReconnectModal.razor
*/
const reconnectModal = document.getElementById("components-reconnect-modal");
const retryButton = document.getElementById("components-reconnect-button");
const resumeButton = document.getElementById("components-resume-button");

/*
   ============================================
   PASO 2: REGISTRAR ESCUCHADORES DE EVENTOS
   ============================================

   addEventListener(tipoEvento, funcionManejadora):
   - "click": cuando el usuario hace clic en el botón
   - "components-reconnect-state-changed": evento PERSONALIZADO que
     Blazor dispara en el elemento del modal cuando cambia el estado
     de la conexión SignalR.

   Los manejadores se definen como funciones más abajo (hoisting:
   en JavaScript, las funciones con "function nombre()" pueden
   usarse antes de declararlas).
*/
reconnectModal.addEventListener("components-reconnect-state-changed", handleReconnectStateChanged);
retryButton.addEventListener("click", retry);
resumeButton.addEventListener("click", resume);

/*
   ============================================
   PASO 3: MANEJAR CAMBIOS DE ESTADO DE LA CONEXIÓN
   ============================================

   Blazor dispara este evento con diferentes estados en event.detail.state:
   - "show":   mostramos el modal de reconexión
   - "hide":   ocultamos el modal (la conexión se restauró)
   - "failed": la reconexión falló, esperamos a que el usuario
               vuelva a la pestaña para reintentar
   - "rejected": el servidor rechazó la reconexión, recargamos
*/
function handleReconnectStateChanged(event) {
    if (event.detail.state === "show") {
        reconnectModal.showModal(); // Abre el <dialog> como modal
    } else if (event.detail.state === "hide") {
        reconnectModal.close();     // Cierra el <dialog>
    } else if (event.detail.state === "failed") {
        /*
           Cuando la reconexión falla, escuchamos el evento
           "visibilitychange": se dispara cuando el usuario cambia
           de pestaña o vuelve a esta. Intentamos reconectar
           automáticamente cuando el usuario regrese.
        */
        document.addEventListener("visibilitychange", retryWhenDocumentBecomesVisible);
    } else if (event.detail.state === "rejected") {
        /*
           "rejected": el servidor dijo "no, no puedo recuperar
           tu sesión". La única opción es recargar la página.
           location.reload(): recarga la página actual (como F5).
           Esto pierde el estado actual de la aplicación.
        */
        location.reload();
    }
}

/*
   ============================================
   PASO 4: FUNCIONES DE RECONEXIÓN
   ============================================

   async function: igual que en C#, permite usar await dentro.
   Blazor.reconnect(): intenta reconectar al mismo circuito SignalR.
   - Si funciona (successful = true): el modal se cierra y todo sigue
   - Si NO funciona (successful = false): probamos Blazor.resumeCircuit()
     que intenta crear un nuevo circuito manteniendo el estado.
   - Si todo falla: recargamos la página (location.reload())
*/
async function retry() {
    /*
       Quitamos el listener de visibilidad para no reintentar
       múltiples veces si el usuario vuelve varias veces.
    */
    document.removeEventListener("visibilitychange", retryWhenDocumentBecomesVisible);

    try {
        /*
           Blazor.reconnect(): API interna de Blazor que intenta
           restablecer la conexión SignalR con el mismo circuito.
           Devuelve true si se reconectó correctamente.
        */
        const successful = await Blazor.reconnect();
        if (!successful) {
            /*
               Blazor.resumeCircuit(): intenta crear un nuevo circuito
               SignalR manteniendo el estado de la aplicación.
               Es un "plan B" si reconnect() falla.
            */
            const resumeSuccessful = await Blazor.resumeCircuit();
            if (!resumeSuccessful) {
                location.reload(); // Plan C: recargar todo
            } else {
                reconnectModal.close(); // ¡Se logró! Cerramos el modal
            }
        }
    } catch (err) {
        /*
           Si hay una excepción (ej. el servidor está caído),
           volvemos a escuchar el evento de visibilidad para
           reintentar cuando el usuario vuelva.
        */
        document.addEventListener("visibilitychange", retryWhenDocumentBecomesVisible);
    }
}

/*
   async function resume(): se ejecuta cuando el usuario hace clic
   en el botón "Reanudar" del modal. Intenta reanudar el circuito.
*/
async function resume() {
    try {
        const successful = await Blazor.resumeCircuit();
        if (!successful) {
            location.reload();
        }
    } catch {
        /*
           Si falla la reanudación, cambiamos la clase CSS del modal
           para que muestre un mensaje de error visual (el CSS cambia
           el color y el texto del modal).
           classList.replace(): reemplaza una clase CSS por otra.
        */
        reconnectModal.classList.replace("components-reconnect-paused", "components-reconnect-resume-failed");
    }
}

/*
   async function retryWhenDocumentBecomesVisible():
   Se ejecuta cuando el usuario vuelve a la pestaña después de
   haber estado en otra (evento visibilitychange).
   document.visibilityState: "visible" si el usuario está viendo
   la pestaña, "hidden" si está en otra pestaña o minimizó.
   Si la pestaña es visible, intentamos reconectar.
*/
async function retryWhenDocumentBecomesVisible() {
    if (document.visibilityState === "visible") {
        await retry();
    }
}
