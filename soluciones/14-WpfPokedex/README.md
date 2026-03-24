# 14-WpfPokedex

## Descripción
Aplicación Pokédex que consume la API REST pública **PokéAPI** (`https://pokeapi.co`). El usuario puede buscar Pokémon por nombre o número, ver sus estadísticas e imagen oficial, y navegar por una lista paginada. Se introduce el uso de `HttpClient`, `async/await` y caché de imágenes en WPF.

## Objetivos de Aprendizaje
- Consumir una API REST con `HttpClient` y deserializar JSON con `System.Text.Json`
- Usar `async/await` para no bloquear el hilo UI durante las peticiones de red
- Implementar una caché sencilla de imágenes para evitar descargas repetidas
- Mostrar una lista paginada de elementos con `ListView` e `ItemTemplate`
- Gestionar estados de carga (`IsLoading`), error y sin resultados en la UI
- Manejar excepciones de red de forma robusta y mostrar mensajes al usuario

## Requisitos Funcionales
- RF-01: Lista paginada de los primeros Pokémon con nombre e imagen en miniatura
- RF-02: Campo de búsqueda por nombre o número que consulta la API
- RF-03: Panel de detalle con imagen oficial, nombre, número, tipo(s), altura, peso y estadísticas base
- RF-04: Indicador de carga (`ProgressBar` o spinner) mientras se realizan peticiones
- RF-05: Mensaje de error si la API no está disponible o el Pokémon no existe
- RF-06: Caché en memoria para no repetir descargas de imágenes ya obtenidas
- RF-07: Botones de paginación (Anterior / Siguiente) para navegar por la lista

## Requisitos No Funcionales

| Código | Requisito | Descripción |
|--------|-----------|-------------|
| RNF-01 | Asincronía | Ninguna petición de red debe bloquear la UI |
| RNF-02 | MVVM | El ViewModel gestiona la lógica de red; la vista solo hace binding |
| RNF-03 | Robustez | Se manejan errores de red y respuestas vacías |

## Arquitectura
**MVVM + API REST con CommunityToolkit.Mvvm**

```
PokedexViewModel
    ├── PokeApiService       ← HttpClient + System.Text.Json
    ├── ImageCache           ← Dictionary<string, BitmapImage>
    └── ObservableCollection<PokemonResumen>
```

## Tecnologías
- WPF (.NET 10)
- C# 14
- JetBrains Rider
- CommunityToolkit.Mvvm (NuGet)
- System.Text.Json (incluido en .NET 10)

## Estructura Sugerida
```
14-WpfPokedex/
└── WpfPokedex/
    ├── WpfPokedex.csproj
    ├── App.xaml
    ├── App.xaml.cs
    ├── Models/
    │   ├── Pokemon.cs
    │   └── PokemonResumen.cs
    ├── Services/
    │   ├── IPokeApiService.cs
    │   └── PokeApiService.cs
    ├── ViewModels/
    │   └── PokedexViewModel.cs
    └── Views/
        ├── MainWindow.xaml
        └── MainWindow.xaml.cs
```

## Funcionalidades Clave
- `AsyncRelayCommand` para los comandos de búsqueda y paginación
- `BitmapImage` cargado desde URL con `UriSource` y caché en `Dictionary<string, BitmapImage>`
- Propiedad `IsLoading` que activa/desactiva el spinner y el campo de búsqueda
- Manejo de `HttpRequestException` y `JsonException` con mensajes de error en la UI

## Notas
- Alineado con la teoría de `09-wpf-mvvm-bindings-reactividad.md`
- Usar `HttpClient` como servicio singleton (no crear uno nuevo por petición)
- PokéAPI no requiere clave de API; respetar los límites de uso (no hacer flood de peticiones)
- Para cargar imágenes desde URL directamente en WPF, usar `BitmapImage` con `UriSource = new Uri(url)`
- `AsyncRelayCommand` de CommunityToolkit.Mvvm gestiona el estado `IsRunning` automáticamente
