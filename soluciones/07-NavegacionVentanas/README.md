# 07-Navegación de Ventanas en WPF

## Descripción
Este bloque de proyectos explora los mecanismos de navegación en aplicaciones de escritorio con WPF (.NET 10 y C# 14). Se presentan dos enfoques:
1. **NavegacionVentanas**: Enfoque didáctico manual basado en code-behind para entender los fundamentos de `Show()` y `ShowDialog()`.
2. **Enrutador**: Enfoque profesional basado en un **RoutesManager** centralizado (portado de JavaFX/Kotlin) que desacopla la navegación de las vistas.

## Objetivos de Aprendizaje
- Diferenciar entre ventanas modales (`ShowDialog`) y no modales (`Show`).
- Gestionar el ciclo de vida de las ventanas y el paso de datos.
- Implementar el patrón **Router/RoutesManager** para centralizar la navegación.
- Eliminar el acoplamiento directo entre vistas mediante un enrutador centralizado.
- Controlar el flujo de arranque de la aplicación desde el code-behind (`OnStartup`).

---

## 🚀 1. Proyecto: NavegacionVentanas (Enfoque Manual)
Este proyecto es la base para entender cómo interactúan las ventanas en WPF.

### Características Clave:
- **Paso de datos**: Uso de constructores, propiedades y eventos.
- **MVVM Básico**: Introducción al `SharedViewModel` para comunicación en tiempo real.
- **Flujo de Login**: Simulación de flujo de acceso básico.
- **Arquitectura**: MVC/Code-behind para máxima visibilidad de los métodos de WPF.

---

## 🛠️ 2. Proyecto: Enrutador (Enfoque Profesional)
Este proyecto replica las funcionalidades anteriores pero utilizando una arquitectura de enrutamiento centralizada.

### El Patrón RoutesManager
Inspirado en arquitecturas modernas de Web y Mobile, el `RoutesManager` centraliza la creación y visualización de ventanas.

**Ventajas:**
- **Desacoplamiento**: `LoginView` no necesita conocer a `MainView`. Solo pide al enrutador: `RoutesManager.NavigateTo(View.Main)`.
- **Mantenibilidad**: Si una ventana cambia su constructor o su lógica de creación, solo se modifica en el enrutador (`CreateView`).
- **Control Centralizado**: El enrutador sabe qué ventana es la principal y cuál es la activa en todo momento.

### Estructura del Enrutador
- `Infrastructure/RoutesManager.cs`: Clase estática que gestiona el diccionario de rutas y las instancias de `Window`.
- `App.xaml.cs`: Punto de entrada que delega el arranque al enrutador.
- `Views/`: Carpeta con las ventanas de la aplicación.

---

## 📊 Comparativa Técnica

| Característica | Navegación Manual | Navegación con Enrutador |
|----------------|-------------------|-------------------------|
| **Acoplamiento** | Alto (Vistas conocen Vistas) | Nulo (Vistas conocen el Enum de Rutas) |
| **Punto de Entrada** | Estático (`StartupUri` en XAML) | Dinámico (`OnStartup` en C#) |
| **Instanciación** | Descentralizada (en cada botón) | Centralizada (en el `RoutesManager`) |
| **Escalabilidad** | Difícil en apps grandes | Alta (Gestión centralizada de rutas) |

## Tecnologías
- **Plataforma**: .NET 10 (WPF)
- **Lenguaje**: C# 14 (Primary Constructors, Global Usings, Static Classes)
- **IDE**: JetBrains Rider / Visual Studio 2022

## Notas Didácticas
- El proyecto `Enrutador` es una evolución lógica. Se recomienda empezar por `NavegacionVentanas` para entender qué hace WPF "por debajo" antes de saltar a la abstracción del enrutador.
- El `RoutesManager` es una adaptación directa del modelo `RoutesManagerFX` usado en el curso de JavaFX, demostrando que los patrones de diseño son universales.
