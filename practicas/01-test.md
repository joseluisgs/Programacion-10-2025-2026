# Test: Interfaces Gráficas con .NET

- [Test: Interfaces Gráficas con .NET](#test-interfaces-gráficas-con-net)
  - [Bloque 1: Fundamentos de Eventos y Patrón Observer](#bloque-1-fundamentos-de-eventos-y-patrón-observer)
  - [Bloque 2: WPF y XAML](#bloque-2-wpf-y-xaml)
  - [Bloque 3: Layouts en WPF](#bloque-3-layouts-en-wpf)
  - [Bloque 4: Componentes y Controles](#bloque-4-componentes-y-controles)
  - [Bloque 5: MVVM y Data Binding](#bloque-5-mvvm-y-data-binding)
  - [Bloque 6: Comandos y Reactividad](#bloque-6-comandos-y-reactividad)
  - [Bloque 7: Navegación y Diálogos](#bloque-7-navegación-y-diálogos)
  - [Bloque 8: Estilos y Recursos](#bloque-8-estilos-y-recursos)

---

#### Bloque 1: Fundamentos de Eventos y Patrón Observer

1. **¿Qué es el patrón Observer?**
   a) Un patrón creacional para crear objetos.
   b) Un patrón de diseño que permite que un objeto notifique a otros objetos de cualquier cambio de estado.
   c) Un patrón estructural para organizar clases.
   d) Un patrón de validación de datos.

2. **En C#, ¿qué palabra clave se usa para declarar un evento?**
   a) `delegate`
   b) `event`
   c) `handler`
   d) `notify`

3. **¿Qué es un delegado en C#?**
   a) Un tipo que define una firma de método y puede referenciar métodos.
   b) Un tipo para acceder a variables privadas.
   c) Un patrón de diseño.
   d) Un tipo de dato primitivo.

4. **¿Cuál es la interfaz fundamental en .NET para implementar propiedades reactivas en WPF?**
   a) `ICommand`
   b) `INotifyPropertyChanged`
   c) `IDataErrorInfo`
   d) `IEnumerable`

5. **¿Qué problema puede causar no desuscribirse de un evento?**
   a) Error de compilación.
   b) Memory leaks (fugas de memoria).
   c) Error de sintaxis.
   d) La aplicación no compila.

6. **¿Qué es un EventAggregator en aplicaciones GUI?**
   a) Un componente visual.
   b) Un patrón para comunicación broadcast entre componentes desacoplados.
   c) Un tipo de datos.
   d) Un método de validación.

7. **¿Cuál es la diferencia entre un evento y un delegado?**
   a) No hay diferencia, son lo mismo.
   b) Un evento es un delegado con protección de invocación externa.
   c) Un delegado puede ser invocado por cualquier clase.
   d) Los eventos solo existen en WPF.

8. **¿Qué significa que un evento es "uno a muchos"?**
   a) Solo puede haber un evento en toda la aplicación.
   b) Un publicador puede tener múltiples suscriptores.
   c) Solo se puede suscribir un objeto a un evento.
   d) Los eventos solo se disparan una vez.

9. **¿Qué es INotifyPropertyChanged?**
   a) Una interfaz para crear eventos personalizados.
   b) Una interfaz que permite notificar a la UI cuando una propiedad cambia.
   c) Una interfaz para validar datos.
   d) Una interfaz para conectar a bases de datos.

10. **¿Qué método se llama cuando una propiedad cambia en INotifyPropertyChanged?**
    a) `OnPropertyChanged()`
    b) `NotifyPropertyChanged()`
    c) `PropertyChanged()`
    d) `FirePropertyChanged()`

---

#### Bloque 2: WPF y XAML

11. **¿Qué es XAML?**
    a) Un lenguaje de programación orientado a objetos.
    b) Un lenguaje de marcado declarativo para crear interfaces de usuario.
    c) Un tipo de base de datos.
    d) Un framework de testing.

12. **¿Cuál es la principal ventaja de WPF sobre WinForms?**
    a) Es más antiguo.
    b) Usa DirectX para renderizado y tiene data binding avanzado.
    c) No requiere .NET.
    d) Solo funciona en Linux.

13. **¿Qué elemento XAML es equivalente a un `div` en HTML?**
    a) `Button`
    b) `Grid`
    c) `StackPanel`
    d) `Canvas`

14. **¿Qué hace el atributo `x:Name` en XAML?**
    a) Define el nombre visual del control.
    b) Crea una variable para acceder al control desde el código.
    c) Define el estilo del control.
    d) Asigna un identificador único para testing.

15. **¿Qué es el ciclo de vida de una aplicación WPF?**
    a) Solo tiene `Main`.
    b) `OnStartup`, `OnExit`, eventos `Loaded`, `Closing`, `Closed`.
    c) Solo `Init` y `Run`.
    d) No tiene ciclo de vida.

16. **¿Qué es el Hot Reload en WPF?**
    a) Una función para reiniciar el equipo.
    b) La capacidad de modificar el código XAML sin reiniciar la aplicación.
    c) Un tipo de compilación.
    d) Un debugger avanzado.

17. **¿Qué es un Resource en WPF?**
    a) Un archivo de imagen.
    b) Un diccionario de estilos y plantillas reutilizables.
    c) Una base de datos.
    d) Un servicio web.

18. **¿Cuál es la diferencia entre `StaticResource` y `DynamicResource`?**
    a) Son iguales.
    b) `StaticResource` se resuelve en tiempo de carga, `DynamicResource` en tiempo de ejecución.
    c) `StaticResource` es más lento.
    d) Solo existe `StaticResource`.

19. **¿Qué significa el atributo `xmlns` en XAML?**
    a) Nombre del espacio de memoria.
    b) Declaración de espacios de nombres XML.
    c) Identificador único.
    d) Tipo de codificación.

20. **¿Qué es `x:Key` en un Resource?**
    a) La clave única para identificar un recurso.
    b) El nombre del control.
    c) Un identificador de evento.
    d) Un tipo de dato.

21. **¿Qué es el DataContext en WPF?**
    a) La conexión a una base de datos.
    b) El objeto que proporciona los datos para el binding.
    c) Un estilo de control.
    d) Un tipo de evento.

22. **¿Qué es el Code-Behind en WPF?**
    a) El archivo XAML.
    b) El archivo C# asociado a una vista XAML.
    c) Un estilo de programación.
    d) Un tipo de recurso.

23. **¿Qué significa `x:Class` en XAML?**
    a) Define la clase CSS.
    b) Especifica la clase parcial del code-behind.
    c) Define una clase de estilo.
    d) Crea una nueva clase.

24. **¿Qué hace `InitializeComponent()` en WPF?**
    a) Inicia la base de datos.
    b) Carga los elementos definidos en XAML.
    c) Inicializa el ViewModel.
    d) Conecta a servicios.

25. **¿Cuál es la extensión de los archivos XAML en WPF?**
    a) `.xml`
    b) `.xaml`
    c) `.wpf`
    d) `.axml`

---

#### Bloque 3: Layouts en WPF

26. **¿Cuál es el contenedor de layout más versátil en WPF?**
    a) `Canvas`
    b) `StackPanel`
    c) `Grid`
    d) `WrapPanel`

27. **¿Qué layout organiza los elementos en una fila o columna?**
    a) `Grid`
    b) `StackPanel`
    c) `Canvas`
    d) `DockPanel`

28. **¿Qué layout permite posicionar elementos en coordenadas específicas?**
    a) `Grid`
    b) `StackPanel`
    c) `Canvas`
    d) `WrapPanel`

29. **¿Qué layout organiza elementos en celdas como una tabla?**
    a) `Grid`
    b) `StackPanel`
    c) `Canvas`
    d) `UniformGrid`

30. **¿Qué propiedad de Grid define el número de columnas?**
    a) `Columns`
    b) `ColumnCount`
    c) `ColumnDefinitions`
    d) `GridColumns`

31. **¿Qué propiedad de Grid define el número de filas?**
    a) `Rows`
    b) `RowCount`
    c) `RowDefinitions`
    d) `GridRows`

32. **¿Qué hace `Grid.Column` attached property?**
    a) Define el número total de columnas.
    b) Especifica en qué columna está un elemento.
    c) Define el ancho de una columna.
    d) Crea una nueva columna.

33. **¿Qué hace `Grid.RowSpan`?**
    a) Define el número de filas.
    b) Permite que un elemento ocupe varias filas.
    c) Define la altura de las filas.
    d) Crea una fila nueva.

34. **¿Qué layout permite a los elementos fluir a la siguiente línea cuando no caben?**
    a) `Grid`
    b) `StackPanel`
    c) `WrapPanel`
    d) `DockPanel`

35. **¿Qué layout permite acoplar elementos a los bordes?**
    a) `Grid`
    b) `StackPanel`
    c) `WrapPanel`
    d) `DockPanel`

36. **¿Qué valor de DockPanel hace que el último elemento ocupe el espacio restante?**
    a) `Dock.Left`
    b) `Dock.Right`
    c) `Dock.Fill`
    d) `Dock.Center`

37. **¿Qué diferencia hay entre Margin y Padding en WPF?**
    a) Son iguales.
    b) Margin es espacio fuera del borde, Padding es espacio dentro.
    c) Padding es solo para texto.
    d) Margin es solo para contenedores.

38. **¿Qué propiedad define el espacio entre elementos en un StackPanel?**
    a) `Spacing`
    b) `Margin`
    c) `Gap`
    d) No tiene, se usa Margin.

39. **¿Qué hace `VerticalAlignment` en WPF?**
    a) Define la alineación vertical del elemento dentro de su celda.
    b) Define la posición absoluta Y.
    c) Define el alto del elemento.
    d) Define el orden vertical.

40. **¿Qué hace `HorizontalAlignment` en WPF?**
    a) Define la alineación horizontal del elemento dentro de su celda.
    b) Define la posición absoluta X.
    c) Define el ancho del elemento.
    d) Define el orden horizontal.

41. **¿Qué es un UniformGrid?**
    a) Un Grid donde todas las celdas tienen el mismo tamaño.
    b) Un Grid sin líneas.
    c) Un Grid que se adapta al contenido.
    d) Un Grid conscroll.

42. **¿Qué diferencia hay entre `Width` y `MinWidth`?**
    a) Son iguales.
    b) `Width` es fijo, `MinWidth` es el mínimo permitido.
    c) `MinWidth` no existe.
    d) `Width` es el mínimo.

---

#### Bloque 4: Componentes y Controles

43. **¿Qué control WPF se usa para mostrar texto que el usuario no puede editar?**
    a) `TextBox`
    b) `TextBlock`
    c) `Label`
    d) Both b y c.

44. **¿Qué control WPF se usa para entrada de texto editable?**
    a) `TextBlock`
    b) `TextBox`
    c) `Label`
    d) `RichTextBox`

45. **¿Qué control WPF es un botón?**
    a) `Button`
    b) `PushButton`
    c) `ClickButton`
    d) `ActionButton`

46. **¿Qué control WPF muestra una lista de elementos donde solo se puede seleccionar uno?**
    a) `ListBox`
    b) `ComboBox`
    c) `CheckBox`
    d) Both a y b.

47. **¿Qué control WPF es una casilla de verificación?**
    a) `RadioButton`
    b) `CheckBox`
    c) `ToggleButton`
    d) `Switch`

48. **¿Qué control WPF es un botón de opción (selección única)?**
    a) `CheckBox`
    b) `RadioButton`
    c) `ToggleButton`
    d) `OptionButton`

49. **¿Qué control WPF muestra una barra de progreso?**
    a) `ProgressBar`
    b) `ProgressIndicator`
    c) `LoadingBar`
    d) `Bar`

50. **¿Qué control WPF permite mostrar imágenes?**
    a) `Image`
    b) `Picture`
    c) `Photo`
    d) `Bitmap`

51. **¿Qué control WPF es un deslizador para seleccionar valores?**
    a) `Slider`
    b) `ScrollBar`
    c) `TrackBar`
    d) `Range`

52. **¿Qué control WPF muestra una barra de desplazamiento?**
    a) `Slider`
    b) `ScrollBar`
    c) `ScrollViewer`
    d) Both b y c.

53. **¿Qué es un ItemsControl en WPF?**
    a) Un control para mostrar elementos.
    b) La clase base para controles que muestran colecciones.
    c) Un control de edición.
    d) Un control de validación.

54. **¿Qué control WPF muestra una tabla de datos?**
    a) `TableView`
    b) `DataGrid`
    c) `GridView`
    d) `DataTable`

55. **¿Qué es un UserControl en WPF?**
    a) Un control personalizado creado por el usuario.
    b) Un control estándar.
    c) Un control de usuario de sistema.
    d) Un control invisible.

56. **¿Qué hace la propiedad `IsEnabled` en un control?**
    a) Define si es visible.
    b) Define si está habilitado para interacción.
    c) Define si es editable.
    d) Define si está seleccionado.

57. **¿Qué hace la propiedad `Visibility` en un control?**
    a) Define si está habilitado.
    b) Define si es visible (Visible, Collapsed, Hidden).
    c) Define el orden Z.
    d) Define la posición.

58. **¿Qué es un ToolTip en WPF?**
    a) Un menú contextual.
    b) Un texto que aparece al pasar el mouse.
    c) Un título de ventana.
    d) Una etiqueta.

59. **¿Qué control WPF muestra un menú?**
    a) `MenuBar`
    b) `ContextMenu`
    c) `Menu`
    d) Both a y c.

60. **¿Qué es un ContextMenu en WPF?**
    a) El menú de la aplicación.
    b) Un menú que aparece al hacer clic derecho.
    c) Un menú desplegable.
    d) Una barra de herramientas.

---

#### Bloque 5: MVVM y Data Binding

61. **¿Qué significa MVVM?**
    a) Model-View-ViewModel
    b) Model-Visual-ViewModel
    c) Main-View-ViewModel
    d) Model-View-ViewModel es lo mismo que MVC.

62. **¿Cuál es la principal ventaja del patrón MVVM en WPF?**
    a) Permite escribir código en el code-behind freely.
    b) Separación clara entre UI y lógica, con data binding automático.
    c) Elimina la necesidad de eventos.
    d) No requiere conocer XAML.

63. **¿Qué es el DataContext en MVVM?**
    a) La conexión a base de datos.
    b) El objeto que proporciona datos para binding.
    c) El ViewModel de la vista.
    d) Both b y c.

64. **¿Qué significa el modo de binding `OneWay`?**
    a) Los datos fluyen solo del origen a la UI.
    b) Los datos fluyen en ambas direcciones.
    c) Los datos fluyen solo de la UI al origen.
    d) No hay flujo de datos.

65. **¿Qué significa el modo de binding `TwoWay`?**
    a) Los datos fluyen solo del origen a la UI.
    b) Los datos fluyen en ambas direcciones: origen ↔ UI.
    c) Los datos fluyen solo de la UI al origen.
    d) No hay flujo de datos.

66. **¿Qué significa el modo de binding `OneTime`?**
    a) Se actualiza cada vez que cambia.
    b) Se lee una sola vez al iniciar.
    c) Nunca se actualiza.
    d) Solo se actualiza manualmente.

67. **¿Qué es `UpdateSourceTrigger` en un binding?**
    a) Frecuencia de actualización de la UI.
    b) Cuándo se actualiza el origen desde la UI.
    c) El tipo de datos del binding.
    d) El modo de validación.

68. **¿Qué diferencia hay entre `UpdateSourceTrigger=PropertyChanged` y `UpdateSourceTrigger=LostFocus`?**
    a) Son iguales.
    b) PropertyChanged actualiza en cada tecleo, LostFocus al perder foco.
    c) LostFocus es más rápido.
    d) No hay diferencia en WPF.

69. **¿Qué es un `IValueConverter`?**
    a) Un tipo de datos.
    b) Un componente que transforma valores durante el binding.
    c) Un tipo de comando.
    d) Un tipo de evento.

70. **¿Qué interfaz debe implementar una clase para que sus propiedades sean observables?**
    a) `ICommand`
    b) `INotifyPropertyChanged`
    c) `INotifyCollectionChanged`
    d) `IDataSource`

71. **¿Qué es un `ObservableCollection`?**
    a) Una colección que implementa INotifyCollectionChanged.
    b) Una colección de solo lectura.
    c) Un tipo de array.
    d) Un tipo de diccionario.

72. **¿Qué biblioteca simplifica MVVM en WPF?**
    a) Newtonsoft.Json
    b) CommunityToolkit.Mvvm
    c) Dapper
    d) AutoMapper

73. **¿Qué atributo de CommunityToolkit.Mvvm genera propiedad observable?**
    a) `[ObservableProperty]`
    b) `[Property]`
    c) `[NotifyPropertyChanged]`
    d) `[BindableProperty]`

74. **¿Qué hace `[RelayCommand]` en CommunityToolkit.Mvvm?**
    a) Genera un comando automáticamente a partir de un método.
    b) Crea una nueva ventana.
    c) Define un estilo.
    d) Implementa INotifyPropertyChanged.

75. **¿Cuál es la diferencia entre MVC y MVVM en WPF?**
    a) No hay diferencia.
    b) MVVM usa data binding bidireccional, MVC no.
    c) MVC es solo para web.
    d) MVVM no tiene modelo.

76. **¿Qué es el Source en un binding?**
    a) El control visual.
    b) El objeto del que se obtienen los datos.
    c) El archivo XAML.
    d) El ViewModel.

77. **¿Qué es el Path en un binding?**
    a) La ruta del archivo.
    b) La propiedad del objeto source a la que se bindea.
    c) El nombre del control.
    d) La ubicación del archivo.

78. **¿Qué es un DataTemplate en WPF?**
    a) Una plantilla de datos.
    b) Cómo se visualizan los datos en un control.
    c) Una base de datos.
    d) Un tipo de comando.

79. **¿Qué es un ItemTemplate en un ItemsControl?**
    a) La plantilla para cada elemento de la colección.
    b) La plantilla del contenedor.
    c) Un estilo de lista.
    d) Una plantilla vacía.

80. **¿Qué hace `{Binding}` sin más propiedades?**
    a) Usa el DataContext actual como source.
    b) No funciona.
    c) Usa la ventana como source.
    d) Usa null como source.

---

#### Bloque 6: Comandos y Reactividad

81. **¿Qué es un `ICommand` en WPF?**
    a) Un tipo de evento.
    b) Una abstracción para ejecutar acciones desde la UI.
    c) Un tipo de datos.
    d) Un estilo de control.

82. **¿Qué métodos define `ICommand`?**
    a) `Execute()` y `CanExecute()`
    b) `Run()` y `Start()`
    c) `Action()` y `Func()`
    d) `Invoke()` y `Call()`

83. **¿Qué hace `CanExecute` en un comando?**
    a) Ejecuta el comando.
    b) Determina si el comando puede ejecutarse.
    c) Cancela el comando.
    d) Finaliza el comando.

84. **¿Qué es un `RelayCommand`?**
    a) Un comando que reenvía a otro.
    b) Una implementación simple de ICommand.
    c) Un comando especializado.
    d) Un comando complejo.

85. **¿Qué es un `AsyncRelayCommand`?**
    a) Un comando síncrono.
    b) Un comando que ejecuta métodos async.
    c) Un comando que no retorna valor.
    d) Un comando retardado.

86. **¿Para qué sirve `IsRunning` en AsyncRelayCommand?**
    a) Indica si el comando está en ejecución.
    b) Indica si el comando terminó.
    c) Indica si el comando puede ejecutarse.
    d) Indica si el comando está habilitado.

87. **¿Qué es la reactividad en MVVM?**
    a) La capacidad de responder a eventos.
    b) La capacidad de la UI de actualizarse automáticamente cuando cambian los datos.
    c) La capacidad de ejecutar comandos.
    d) La capacidad de validar datos.

88. **¿Qué hace `OnPropertyChanged()` en el setter de una propiedad?**
    a) Notifica a la UI que la propiedad cambió.
    b) Guarda el valor.
    c) Valida el valor.
    d) Nada.

89. **¿Qué es el Change Tracking?**
    a) El seguimiento de cambios en el código.
    b) El seguimiento de cambios en las propiedades para la UI.
    c) El seguimiento de usuarios.
    d) El seguimiento de errores.

90. **¿Qué es un comando con parámetro?**
    a) Un comando que recibe datos.
    b) Un comando que no hace nada.
    c) Un comando simple.
    d) Un comando sin uso.

---

#### Bloque 7: Navegación y Diálogos

91. **¿Qué método muestra una ventana modal en WPF?**
    a) `Show()`
    b) `ShowDialog()`
    c) `Open()`
    d) `Display()`

92. **¿Qué método muestra una ventana no modal en WPF?**
    a) `ShowDialog()`
    b) `Show()`
    c) `OpenModal()`
    d) `Display()`

93. **¿Qué es DialogResult en WPF?**
    a) El resultado de una operación.
    b) El valor retornado por un ShowDialog.
    c) El estado del diálogo.
    d) Both b y c.

94. **¿QuéMessageBoxButton muestra Yes/No/Cancel?**
    a) `MessageBoxButton.OK`
    b) `MessageBoxButton.YesNoCancel`
    c) `MessageBoxButton.YesNo`
    d) `MessageBoxButton.AbortRetryIgnore`

95. **¿Qué devuelve MessageBox.Show()?**
    a) void.
    b) DialogResult.
    c) bool.
    d) string.

96. **¿Qué es OpenFileDialog en WPF?**
    a) Un diálogo para guardar archivos.
    b) Un diálogo para abrir archivos.
    c) Un diálogo para seleccionar carpetas.
    d) Un diálogo de mensaje.

97. **¿Qué es SaveFileDialog en WPF?**
    a) Un diálogo para abrir archivos.
    b) Un diálogo para guardar archivos.
    c) Un diálogo para seleccionar carpetas.
    d) Un diálogo de mensaje.

98. **¿Cómo se pasa datos entre ventanas en WPF?**
    a) No se puede.
    b) A través de propiedades públicas o el constructor.
    c) Solo con eventos.
    d) Solo con base de datos.

99. **¿Qué es la navegación en WPF?**
    a) Moverse entre páginas.
    b) Cambiar de ventana o mostrar dialogs.
    c) Both a y b.
    d) Solo cambiar de aplicación.

100. **¿Qué es un Frame en WPF?**
    a) Un marco de ventana.
    b) Un control que permite navegación entre páginas.
    c) Un contenedor de contenido.
    d) Un estilo de borde.

---
