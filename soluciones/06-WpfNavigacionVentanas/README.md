# 06-WpfNavigacionVentanas

## Descripción
Proyecto dedicado a la navegación entre ventanas en WPF. Explora los diferentes mecanismos para abrir, cerrar y pasar datos entre ventanas: constructor, propiedades, eventos, `DialogResult` y ViewModel compartido. Se compara `Show()` (no modal) con `ShowDialog()` (modal).

## Objetivos de Aprendizaje
- Abrir ventanas secundarias con `Show()` y `ShowDialog()`
- Pasar datos a una ventana secundaria a través del constructor o de propiedades públicas
- Recibir resultados de vuelta desde una ventana modal mediante `DialogResult` o propiedades
- Comunicar ventanas mediante eventos o un ViewModel compartido
- Controlar el ciclo de vida de las ventanas (`Close`, `Hide`, eventos `Closing`/`Closed`)
- Gestionar el flujo de trabajo principal (ventana de inicio de sesión → ventana principal)

## Requisitos Funcionales
- RF-01: Ventana principal con botones que abren distintos tipos de ventanas secundarias
- RF-02: Ventana modal (`ShowDialog`) de ingreso de datos que devuelve el resultado al aceptar o cancelar
- RF-03: Ventana no modal (`Show`) que puede coexistir con la ventana principal
- RF-04: Demostración de paso de datos mediante constructor y mediante propiedades
- RF-05: Demostración de ViewModel compartido entre ventana principal y ventana secundaria
- RF-06: Ventana de inicio de sesión que, al validar credenciales, abre la ventana principal y se cierra

## Requisitos No Funcionales

| Código | Requisito | Descripción |
|--------|-----------|-------------|
| RNF-01 | Claridad didáctica | Cada demo de navegación está claramente diferenciada en la UI |
| RNF-02 | Arquitectura | Preferencia por ViewModel compartido frente a acoplamiento directo entre vistas |

## Arquitectura
**MVC (code-behind) con introducción a MVVM para el ViewModel compartido**

La mayor parte de la lógica de apertura de ventanas reside en el code-behind para mayor claridad didáctica. Un apartado avanzado muestra cómo usar un ViewModel compartido para comunicar vistas.

## Tecnologías
- WPF (.NET 10)
- C# 14
- JetBrains Rider

## Estructura Sugerida
```
06-WpfNavigacionVentanas/
└── WpfNavigacionVentanas/
    ├── WpfNavigacionVentanas.csproj
    ├── App.xaml
    ├── App.xaml.cs
    ├── ViewModels/
    │   └── SharedViewModel.cs       ← ViewModel compartido (demo avanzada)
    └── Views/
        ├── MainWindow.xaml
        ├── MainWindow.xaml.cs
        ├── VentanaModal.xaml
        ├── VentanaModal.xaml.cs
        ├── VentanaNoModal.xaml
        ├── VentanaNoModal.xaml.cs
        ├── LoginWindow.xaml
        └── LoginWindow.xaml.cs
```

## Funcionalidades Clave
- Demo 1 — Modal con `ShowDialog` y `DialogResult`
- Demo 2 — No modal con `Show` y comunicación por eventos
- Demo 3 — Paso de datos por constructor
- Demo 4 — Paso de datos por propiedades
- Demo 5 — ViewModel compartido entre ventanas
- Demo 6 — Flujo Login → Main (ventana de inicio de sesión)

## Notas
- Alineado con la teoría de `10-wpf-navegacion-avanzada.md`
- `DialogResult = true` solo puede establecerse en ventanas abiertas con `ShowDialog()`
- Para el flujo Login → Main, usar `Application.Current.MainWindow` o cambiar `App.xaml` para que arranque con la ventana de login
- El ViewModel compartido debe instanciarse en `App.xaml.cs` y pasarse a ambas ventanas para garantizar la misma referencia
