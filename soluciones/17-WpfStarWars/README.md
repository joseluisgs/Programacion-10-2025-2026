# 17-WpfStarWars

## Descripción
Aplicación de exploración del universo Star Wars que consume la API pública **SWAPI** (`https://swapi.dev`). Combina el consumo de API REST con la ejecución de tareas en background para mantener la UI responsiva. Incluye búsqueda asíncrona, caché de resultados y navegación entre categorías (personajes, planetas, naves).

## Objetivos de Aprendizaje
- Combinar consumo de API REST (`HttpClient`, `async/await`) con gestión de background tasks
- Implementar caché de resultados en memoria para reducir peticiones a la API
- Construir una búsqueda asíncrona con debounce (esperar a que el usuario deje de escribir)
- Gestionar múltiples `CancellationToken` para cancelar peticiones al hacer una nueva búsqueda
- Mostrar indicadores de progreso para operaciones de red
- Paginar resultados de la API y cargar páginas adicionales bajo demanda

## Requisitos Funcionales
- RF-01: Navegación entre categorías: Personajes, Planetas, Naves Estelares, Vehículos, Especies
- RF-02: Lista de items de la categoría seleccionada con carga paginada
- RF-03: Panel de búsqueda que filtra por nombre con petición asíncrona a la API
- RF-04: Panel de detalle con todos los atributos del item seleccionado
- RF-05: Indicador de carga durante las peticiones de red
- RF-06: Caché de resultados: si ya se cargó una página/búsqueda, no repetir la petición
- RF-07: Manejo de errores de red con mensaje informativo al usuario

## Requisitos No Funcionales

| Código | Requisito | Descripción |
|--------|-----------|-------------|
| RNF-01 | Asincronía | Todas las peticiones de red son asíncronas; la UI no se bloquea |
| RNF-02 | Caché | Los resultados se almacenan para evitar peticiones redundantes |
| RNF-03 | MVVM | Separación estricta entre lógica de red y lógica de presentación |

## Arquitectura
**MVVM + API REST + Background Tasks**

```
StarWarsViewModel
    ├── SwapiService          ← HttpClient + caché + paginación
    ├── Personajes/Planetas/Naves : ObservableCollection
    ├── ItemSeleccionado
    ├── IsLoading
    └── BuscarCommand (AsyncRelayCommand con cancelación)
```

## Tecnologías
- WPF (.NET 10)
- C# 14
- JetBrains Rider
- CommunityToolkit.Mvvm (NuGet)
- System.Text.Json (incluido en .NET 10)

## Estructura Sugerida
```
17-WpfStarWars/
└── WpfStarWars/
    ├── WpfStarWars.csproj
    ├── App.xaml
    ├── App.xaml.cs
    ├── Models/
    │   ├── Personaje.cs
    │   ├── Planeta.cs
    │   └── NaveEstelar.cs
    ├── Services/
    │   ├── ISwapiService.cs
    │   └── SwapiService.cs
    ├── ViewModels/
    │   └── StarWarsViewModel.cs
    └── Views/
        ├── MainWindow.xaml
        └── MainWindow.xaml.cs
```

## Funcionalidades Clave
- `AsyncRelayCommand` con soporte de cancelación para la búsqueda
- Caché implementada con `Dictionary<string, object>` en el servicio
- Debounce de búsqueda con `Task.Delay` y cancelación de la búsqueda anterior
- `TabControl` para navegar entre categorías sin recargar datos ya obtenidos

## Notas
- Alineado con la teoría de `14-wpf-tareas-background.md`
- SWAPI puede estar en mantenimiento; considerar un mirror o datos locales de respaldo
- El debounce evita lanzar una petición por cada tecla pulsada; esperar ~300ms desde el último cambio
- `HttpClient` debe ser un singleton compartido para evitar el agotamiento de sockets (socket exhaustion)
