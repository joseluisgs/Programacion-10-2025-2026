// ============================================================
// RelayCommand.cs - Implementacion manual de ICommand
// ============================================================
// Este archivo implementa el patron Command de forma manual,
// sin usar librerias externas como CommunityToolkit.Mvvm.
//
// CONCEPTOS IMPORTANTES:
//
// 1. INTERFAZ ICommand:
//    - Execute(object?): Ejecuta la accion del comando
//    - CanExecute(object?): Determina si el comando puede ejecutarse
//    - CanExecuteChanged: Evento que se dispara cuando cambia CanExecute
//
// 2. POR QUE USAR COMANDOS:
//    - Encapsulan la logica de negocio en objetos separados
//    - Permiten binding desde XAML
//    - Habilitan/deshabilitan automaticamente botones con CanExecute
//
// 3. DELEGADOS:
//    - Action: Representa un metodo que no devuelve valor
//    - Func<bool>: Representa un metodo que devuelve bool (para CanExecute)

using System.Windows.Input;

namespace WpfMVVMBasico.Commands;

/// <summary>
/// Implementacion de ICommand que permite ejecutar acciones mediante binding.
/// Este es el equivalente manual del [RelayCommand] de CommunityToolkit.Mvvm.
///
/// Uso en XAML:
///   Command="{Binding IncrementarCommand}"
///   
/// El boton se habilita/deshabilita automaticamente segun CanExecute.
/// </summary>
public class RelayCommand : ICommand
{
    // ============================================================
    // ATRIBUTOS PRIVADOS
    // ============================================================

    /// <summary>
    /// Accion a ejecutar cuando se invoca el comando.
    /// Es un delegado que no devuelve ningun valor.
    /// </summary>
    private readonly Action _execute;

    /// <summary>
    /// Funcion que determina si el comando puede ejecutarse.
    /// Devuelve true si el comando esta habilitado, false en caso contrario.
    /// Es opcional (puede ser null).
    /// </summary>
    private readonly Func<bool>? _canExecute;

    // ============================================================
    // CONSTRUCTOR
    // ============================================================

    /// <summary>
    /// Constructor que recibe la accion a ejecutar.
    /// </summary>
    /// <param name="execute">Accion a ejecutar cuando se invoque el comando</param>
    public RelayCommand(Action execute) : this(execute, null)
    {
    }

    /// <summary>
    /// Constructor completo con accion y condicion de habilitacion.
    /// </summary>
    /// <param name="execute">Accion a ejecutar</param>
    /// <param name="canExecute">Funcion que determina si se puede ejecutar (opcional)</param>
    public RelayCommand(Action execute, Func<bool>? canExecute)
    {
        // Validar que la accion no sea null
        _execute = execute ?? throw new ArgumentNullException(nameof(execute));
        
        // La condicion CanExecute es opcional
        _canExecute = canExecute;
    }

    // ============================================================
    // EVENTO CanExecuteChanged
    // ============================================================

    /// <summary>
    /// Evento que se dispara cuando cambia la capacidad de ejecucion del comando.
    /// La UI escucha este evento para actualizar el estado de los botones.
    /// 
    /// Importante: Se usa += y -= para suscribir/desuscribir observadores.
    /// </summary>
    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }

    // ============================================================
    // METODO CanExecute
    // ============================================================

    /// <summary>
    /// Determina si el comando puede ejecutarse en el estado actual.
    /// La UI llama automaticamente a este metodo para habilitar/deshabilitar botones.
    /// </summary>
    /// <param name="parameter">Parametro (no usado en esta implementacion simple)</param>
    /// <returns>True si puede ejecutarse, false en caso contrario</returns>
    public bool CanExecute(object? parameter)
    {
        // Si no hay condicion CanExecute, siempre se puede ejecutar
        // Si hay condicion, evaluarla
        return _canExecute == null || _canExecute();
    }

    // ============================================================
    // METODO Execute
    // ============================================================

    /// <summary>
    /// Ejecuta la accion del comando.
    /// Se llama cuando el usuario hace clic en un boton vinculado a este comando.
    /// </summary>
    /// <param name="parameter">Parametro (no usado en esta implementacion simple)</param>
    public void Execute(object? parameter)
    {
        // Ejecutar la accion
        _execute();
    }

    // ============================================================
    // METODO辅助 (opcional)
    // ============================================================

    /// <summary>
    /// Metodo auxiliar para notificar manualmente que CanExecute ha cambiado.
    /// Useful when the CanExecute condition changes without the CommandManager detecting it.
    /// </summary>
    public void RaiseCanExecuteChanged()
    {
        // Notificar a todos los suscriptores que el estado ha cambiado
        // Esto hara que la UI vuelva a llamar a CanExecute
        CommandManager.InvalidateRequerySuggested();
    }
}

// ============================================================
// RESUMEN: FLUJO DE DATOS EN MVVM CON COMANDOS
// ============================================================
//
// 1. Usuario hace clic en boton
//    └─> XAML: Command="{Binding IncrementarCommand}"
//
// 2. WPF llama a CanExecute() para verificar si el boton debe estar habilitado
//    └─> Si CanExecute() devuelve false, el boton se deshabilita automaticamente
//
// 3. Si el boton esta habilitado, WPF llama a Execute()
//    └─> Execute() llama a _execute()
//
// 4. _execute() ejecuta la logica (ej: Contador++)
//    └─> ViewModel cambia el valor de la propiedad
//
// 5. ViewModel dispara PropertyChanged para la propiedad
//    └─> La UI detecta el cambio y actualiza el valor en pantalla
//
// 6. ViewModel dispara CanExecuteChanged
//    └─> La UI vuelve a llamar a CanExecute()
//    └─> Se actualiza el estado de los botones (ej: Decrementar se habilita)
//
// ============================================================
// COMPARACION CON COMMUNITYTOOLKIT.MVVM
// ============================================================
//
// Con CommunityToolkit.Mvvm:
//   [RelayCommand]
//   private void Incrementar() => Contador++;
//
// Sin librerias (este proyecto):
//   private RelayCommand _incrementarCommand;
//   public ICommand IncrementarCommand => _incrementarCommand ??= new RelayCommand(Incrementar);
//   private void Incrementar() => Contador++;
//
// El resultado es el mismo, pero con CommunityToolkit se escribe menos codigo.
// ============================================================
