// ============================================================
// ContadorViewModel.cs - ViewModel con CommunityToolkit.Mvvm
// ============================================================
//
// =================================================================
// GUIA PARA EL ALUMNO: POR QUÉ NECESITAMOS ESTAS COSAS
// =================================================================
//
// En WPF, la UI (vista) y la logica (viewmodel) están separadas.
// Esto es bueno para mantener el código ordenado.
//
// PROBLEMA: Cuando cambiamos un dato en el ViewModel, la UI no lo sabe.
// SOLUCION: Necesitamos un mecanismo para que la UI se entere.
//
// =================================================================
// 1. POR QUÉ NECESITAMOS NOTIFICAR CAMBIOS A LA UI?
// =================================================================
//
// En una aplicación WPF, tenemos dos mundos:
// - Mundo UI (XAML): Lo que ve el usuario
// - Mundo Datos (C#): La logica de la aplicación
//
// Si cambiamos un dato en C#, la UI no lo sabe automáticamente.
//
// Ejemplo sin notificación:
//   ViewModel: contador = 5
//   UI: sigue mostrando 0 (no se entera del cambio)
//
// Ejemplo con notificación:
//   ViewModel: contador = 5 + notificamos cambio
//   UI: se entera y muestra 5
//
// POR QUÉ PASA ESTO?
// - La UI no puede "adivinar" que los datos cambiaron
// - Necesitamos un sistema de comunicación
// - Ese sistema es INotifyPropertyChanged
//
// QUÉ CONSEGUIMOS CON ESTO?
// - La UI se actualiza automáticamente
// - No necesitamos código manual para actualizar la UI
// - Separation of Concerns: el ViewModel no conoce la UI
//

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace WpfMVVMCommunityToolkit.ViewModels;

/// <summary>
/// ViewModel para un contador simple.
/// </summary>
// =================================================================
// 2. POR QUÉ USAMOS ObservableObject?
// =================================================================
//
// ObservableObject es una clase que ya implementa INotifyPropertyChanged.
// Es la base que nos da la capacidad de notificar cambios.
//
// SIN ella tendriamos que escribir ~15 lineas de código para implementar
// INotifyPropertyChanged en cada ViewModel.
//
// CON ella solo heredas y ya tienes esa capacidad.
//
// QUÉ CONSEGUIMOS?
// - Menos código repetitivo
// - Menos errores
// - Código más limpio
//
public partial class ContadorViewModel : ObservableObject
{
    // =================================================================
    // 3. POR QUÉ USAMOS [ObservableProperty]?
    // =================================================================
    //
    // PROBLEMA: Cuando cambian los datos, la UI no se entera.
    //
    // EJEMPLO SIN NOTIFICACIÓN:
    //   private int _contador;  // Si cambio esto...
    //   // La UI no lo sabe, sigue mostrando el valor antiguo
    //
    // EJEMPLO CON NOTIFICACIÓN:
    //   private int _contador;  // Si cambio esto...
    //   // Al mismo tiempo, notifico a la UI: "oye, el valor cambió"
    //   // La UI dice: "ah, pues me actualizo"
    //
    // QUÉ HACE [ObservableProperty]?
    // Le dice al compilador: "cuando alguien cambie este campo,
    //                       notifica automáticamente a la UI"
    //
    // QUÉ CONSEGUIMOS?
    // - No tenemos que escribir código de notificación manualmente
    // - Cuando cambiemos _contador, la UI se entera automáticamente
    // - La UI se actualiza sin que nosotros hagamos nada
    //
    // EN RESUMEN: Es la forma dedecirle a la UI que un dato cambió
    //
    [ObservableProperty]
    private int _contador;

    // =================================================================
    // 4. POR QUÉ USAMOS [RelayCommand]?
    // =================================================================
    //
    // PROBLEMA: En MVVM, la UI no debe tener lógica.
    //           ¿Cómo conectamos un botón (UI) con una acción (código)?
    //
    // SOLUCIÓN: Los comandos (ICommand)
    //
    // SIN comandos:
    //   <Button Click="BtnIncrementar_Click"/>  // UI conoce el código
    //   Esto viola MVVM porque la UI conoce la lógica
    //
    // CON comandos:
    //   <Button Command="{Binding IncrementarCommand}"/>
    //   - La UI NO conoce el código
    //   - Solo dice: "ejecuta el comando que está vinculado"
    //   - El ViewModel decide qué código ejecutar
    //
    // QUÉ HACE [RelayCommand]?
    // Crea automáticamente un objeto "comando" que puede ejecutar una acción.
    // Ese comando se vincula al botón mediante el binding.
    //
    // QUÉ CONSEGUIMOS?
    // - Separación total: la UI no conoce la lógica
    // - Código limpio: la lógica está en el ViewModel
    // - Testeable: podemos probar los comandos sin la UI
    // - Reutilizable: el mismo comando en varios botones
    //
    // EN RESUMEN: Es la forma de conectar botones con lógica
    //             sin que la UI conozca la lógica
    //
    [RelayCommand]
    private void Incrementar()
    {
        Contador++;
    }

    // =================================================================
    // 5. POR QUÉ USAMOS CanExecute?
    // =================================================================
    //
    // PROBLEMA: A veces un botón no debe hacer nada según el estado.
    //           Ejemplo: No podemos decrementar si el contador es 0.
    //
    // SIN CanExecute:
    //   - El botón siempre está habilitado
    //   - Tenemos que validar dentro del método
    //   - El usuario ve un botón que no debería poder pulsar
    //
    // CON CanExecute:
    //   - El botón se deshabilita automáticamente
    //   - El usuario ve claramente que no puede usarlo
    //   - Es más intuitivo
    //
    // QUÉ CONSEGUIMOS?
    // - Mejor UX: el usuario sabe qué puede y no puede hacer
    // - Menos errores: no puede pulsar algo que no debe
    // - Código más limpio: la condición está declarada, no en ifs
    //
    // EN RESUMEN: Es la forma de habilitar/deshabilitar botones
    //             automáticamente según el estado
    //
    [RelayCommand(CanExecute = nameof(CanDecrementar))]
    private void Decrementar()
    {
        Contador--;
    }

    private bool CanDecrementar() => Contador > 0;

    [RelayCommand]
    private void Reiniciar()
    {
        Contador = 0;
    }

    // =================================================================
    // 6. POR QUÉ USAMOS OnContadorChanged?
    // =================================================================
    //
    // PROBLEMA: Cuando Contador cambia, CanExecute de Decrementar
    //           podría necesitar actualizarse.
    //           Ejemplo: Contador pasa de 1 a 0.
    //           Antes: Decrementar habilitado (porque 1 > 0)
    //           Ahora: Decrementar debería deshabilitarse (porque 0 no > 0)
    //
    // SIN OnContadorChanged:
    //   - CanExecute no se reevalúa automáticamente
    //   - El botón podría quedarse habilitado incorrectamente
    //
    // CON OnContadorChanged:
    //   - Se ejecuta automáticamente cuando Contador cambia
    //   - Podemos actualizar lo que necesitemos
    //   - En este caso, actualizamos CanExecute de Decrementar
    //
    // QUÉ CONSEGUIMOS?
    // - Sincronización automática entre datos y UI
    // - El botón siempre refleja el estado correcto
    //
    // EN RESUMEN: Es un "hook" para ejecutar código cuando
    //             una propiedad cambia
    //
    partial void OnContadorChanged(int value)
    {
        DecrementarCommand.NotifyCanExecuteChanged();
    }
}

// =================================================================
// RESUMEN: QUÉ QUEREMOS LOGRAR CON MVVM?
// =================================================================
//
// 1. SEPARACIÓN DE RESPONSABILIDADES
//    - Vista (XAML): solo define cómo se ve
//    - ViewModel (C#): solo define la lógica y datos
//    - No se mezclan
//
// 2. UI ACTUALIZADA AUTOMÁTICAMENTE
//    - Cuando cambian los datos, la UI cambia sola
//    - No necesitamos actualizar la UI manualmente
//
// 3. BOTONES INTELIGENTES
//    - Se habilitan/deshabilitan según el estado
//    - El usuario sabe qué puede hacer
//
// 4. CÓDIGO LIMPIO
//    - Sin código de UI en la lógica
//    - Sin código de lógica en la UI
//
// 5. TESTABLE
//    - Podemos probar la lógica sin la UI
//    - Podemos cambiar la UI sin afectar la lógica
//
// =================================================================
// EN RESUMEN: COMMUNITYTOOLKIT.MVVM NOS AYUDA A:
// =================================================================
//
// - [ObservableProperty]: Notifica cambios a la UI automáticamente
// - [RelayCommand]: Conecta botones con lógica sin acoplamiento
// - CanExecute: Habilita/deshabilita botones automáticamente
// - OnChanged: Ejecuta código cuando algo cambia
//
// Todo esto lo conseguimos con menos código y menos errores.
//
