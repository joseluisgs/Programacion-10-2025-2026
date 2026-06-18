// ============================================================
// ContadorViewModel.cs - ViewModel con INotifyPropertyChanged manual
// ============================================================
// Este archivo implementa el patron ViewModel de forma manual,
// sin usar librerias externas como CommunityToolkit.Mvvm.
//
// CONCEPTOS IMPORTANTES:
//
// 1. INotifyPropertyChanged:
//    - Interfaz que permite notificar a la UI cuando una propiedad cambia
//    - Evento: PropertyChanged
//    - Metodo: OnPropertyChanged(string propertyName)
//
// 2. NOTIFICACION DE CAMBIOS:
//    - Cuando изменяется una propiedad, se dispara PropertyChanged
//    - La UI escucha este evento y actualiza los valores enlazados
//    - Esto es lo que hace que la UI se actualice automaticamente
//
// 3. VIEWMODEL:
//    - Clase que contiene la logica de presentacion
//    - Expone propiedades y comandos para la Vista
//    - No conoce la Vista (separacion de responsabilidades)
//
// 4. DATA BINDING:
//    - XAML: Text="{Binding Contador}"
//    - Cuando Contador cambia, la UI se actualiza automaticamente

using System.ComponentModel;
using System.Runtime.CompilerServices;
using WpfMVVMBasico.Commands;

namespace WpfMVVMBasico.ViewModels;

/// <summary>
/// ViewModel para un contador simple.
/// Implementa INotifyPropertyChanged de forma manual.
///
/// Este es el equivalente de usar [ObservableProperty] y [RelayCommand]
/// de CommunityToolkit.Mvvm.
///
/// Ejemplo de uso en XAML:
///   <TextBlock Text="{Binding Contador}"/>
///   <Button Command="{Binding IncrementarCommand}"/>
/// </summary>
public class ContadorViewModel : INotifyPropertyChanged
{
    // ============================================================
    // EVENTO PropertyChanged
    // ============================================================

    /// <summary>
    /// Evento que se dispara cuando cambia el valor de una propiedad.
    /// La UI escucha este evento para actualizar los valores enlazados.
    /// 
    /// La interfaz INotifyPropertyChanged requiere este evento.
    /// </summary>
    public event PropertyChangedEventHandler? PropertyChanged;

    // ============================================================
    // ATRIBUTOS (backing fields)
    // ============================================================

    /// <summary>
    /// Campo privado que almacena el valor del contador.
    /// Es el "backing field" de la propiedad Contador.
    /// </summary>
    private int _contador;

    // ============================================================
    // PROPIEDADES
    // ============================================================

    /// <summary>
    /// Propiedad que representa el valor del contador.
    /// Al cambiarla, notifica a la UI mediante PropertyChanged.
    /// 
    /// NOTA: En CommunityToolkit.Mvvm se usaria [ObservableProperty]
    ///       y se generaria automaticamente este codigo.
    /// </summary>
    public int Contador
    {
        get => _contador;
        set
        {
            // Solo actualizar si el valor es diferente
            if (_contador != value)
            {
                _contador = value;
                OnPropertyChanged();
            }
        }
    }

    // ============================================================
    // COMANDOS
    // ============================================================

    /// <summary>
    /// Comando para incrementar el contador.
    /// Se usa en XAML: Command="{Binding IncrementarCommand}"
    /// 
    /// NOTA: En CommunityToolkit.Mvvm se usaria [RelayCommand]
    ///       y el metodo se llamaria Incrementar() (sin sufijo).
    /// </summary>
    public RelayCommand IncrementarCommand { get; }

    /// <summary>
    /// Comando para decrementar el contador.
    /// CanExecute limita que no pueda decrementar por debajo de 0.
    /// </summary>
    public RelayCommand DecrementarCommand { get; }

    /// <summary>
    /// Comando para reiniciar el contador a 0.
    /// </summary>
    public RelayCommand ReiniciarCommand { get; }

    // ============================================================
    // CONSTRUCTOR
    // ============================================================

    /// <summary>
    /// Constructor del ViewModel.
    /// Inicializa los comandos con las acciones correspondientes.
    /// </summary>
    public ContadorViewModel()
    {
        // Crear comando de incrementar
        // Execute: Incrementa el contador
        IncrementarCommand = new RelayCommand(
            () => Contador++  // Action: incrementar
        );

        // Crear comando de decrementar
        // Execute: Decrementa el contador (solo si es mayor que 0)
        // CanExecute: Solo permite decrementar si Contador > 0
        DecrementarCommand = new RelayCommand(
            () => Contador--,               // Action: decrementar
            () => Contador > 0               // CanExecute: no decrementar si es 0
        );

        // Crear comando de reiniciar
        // Execute: Reinicia el contador a 0
        ReiniciarCommand = new RelayCommand(
            () => Contador = 0              // Action: reiniciar
        );
    }

    // ============================================================
    // METODOS
    // ============================================================

    /// <summary>
    /// Metodo que dispara el evento PropertyChanged.
    /// Se llama cuando cambia el valor de una propiedad.
    /// </summary>
    /// <param name="propertyName">Nombre de la propiedad que ha cambiado</param>
    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        // Dispara el evento para notificar a la UI
        // El ? (null-conditional) evita excepciones si no hay suscriptores
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

// ============================================================
// RESUMEN: FLUJO DE ACTUALIZACION DE LA UI
// ============================================================
//
// 1. Usuario hace clic en boton "Incrementar"
//    └─> XAML: Command="{Binding IncrementarCommand}"
//
// 2. Se ejecuta el metodo del comando (Contador++)
//    └─> Se cambia el valor de _contador
//
// 3. Se dispara PropertyChanged para "Contador"
//    └─> OnPropertyChanged() llama a PropertyChanged?.Invoke()
//
// 4. La UI detecta el cambio de propiedad
//    └─> El binding Text="{Binding Contador}" se actualiza
//
// 5. Tambien se actualiza CanExecute de DecrementarCommand
//    └─> Si Contador pasa de 0 a 1, el boton se habilita
//    └─> Si Contador pasa de 1 a 0, el boton se deshabilita
//
// ============================================================
// COMPARACION CON COMMUNITYTOOLKIT.MVVM
// ============================================================
//
// Version manual (este proyecto):
//   private int _contador;
//   public int Contador
//   {
//       get => _contador;
//       set { _contador = value; OnPropertyChanged(); }
//   }
//   public RelayCommand IncrementarCommand { get; }
//
// Version con CommunityToolkit.Mvvm:
//   [ObservableProperty]
//   private int _contador;
//   
//   [RelayCommand]
//   private void Incrementar() => Contador++;
//
// El resultado es identico, pero CommunityToolkit genera el codigo automaticamente.
// ============================================================
