# 24-ListaTareasOpenSilver

## Descripción

Aplicación de **gestión de tareas** (to-do list) desarrollada con **OpenSilver**, un framework que permite ejecutar aplicaciones WPF en un navegador web mediante WebAssembly.

La aplicación permite:
- ✅ Añadir tareas - Campo de texto + botón
- ✅ Listar tareas - Muestra todas las tareas con su estado
- ✅ Marcar completada - Checkbox para cada tarea
- ✅ Eliminar tarea - Botón para borrar
- ✅ Contador - Muestra tareas pendientes
- ✅ Persistencia - Guarda en localStorage del navegador

## Tecnologías

- **OpenSilver** - Framework WPF para navegador (WebAssembly)
- **.NET 10** - Runtime
- **C# 14** - Lenguaje
- **CommunityToolkit.Mvvm** - Soporte MVVM
- **localStorage** - Persistencia en navegador

## Estructura

```
24-ListaTareasOpenSilver/
├── ListaTareasOpenSilver/
│   ├── Models/
│   │   └── Tarea.cs              # Modelo de datos
│   ├── Services/
│   │   └── TareaService.cs       # Servicio con persistencia localStorage
│   ├── ViewModels/
│   │   ├── ViewModelBase.cs      # Clase base para ViewModels
│   │   └── MainWindowViewModel.cs # Lógica de presentación
│   ├── Views/
│   │   ├── MainWindow.xaml       # Interfaz de usuario
│   │   └── MainWindow.xaml.cs    # Code-behind
│   ├── App.xaml                  # Aplicación
│   └── App.xaml.cs               # Punto de entrada
├── ListaTareasOpenSilver.csproj
└── README.md
```

## Comparativa con WPF/Avalonia

| Aspecto | WPF | OpenSilver | Avalonia |
|---------|-----|------------|----------|
| Plataformas | Windows | Navegador (Web) | Win/Linux/macOS |
| XAML | ✅ | ✅ (WPF compatible) | Similar a WPF |
| Persistencia | Archivo local | localStorage | Archivo local |
| Instalación | .NET SDK | OpenSilver SDK | .NET SDK |

## Cómo Configurar OpenSilver

OpenSilver requiere una configuración especial. Para crear un proyecto OpenSilver:

1. **Descarga e instala OpenSilver SDK** desde [opensilver.net](https://opensilver.net)
2. Crea un nuevo proyecto usando la plantilla de OpenSilver
3. Copia los archivos de este ejemplo al proyecto

### Alternativa: Simulación con WPF

Si no tienes OpenSilver instalado, puedes ejecutar este código como una aplicación WPF normal (funcionará excepto la persistencia en localStorage):

```bash
cd ListaTareasOpenSilver
dotnet run
```

## Notas

- OpenSilver permite ejecutar aplicaciones WPF en el navegador
- Usa la misma sintaxis XAML que WPF
- La persistencia se hace mediante `localStorage` del navegador
- Es ideal para migrar aplicaciones WPF existentes a la web