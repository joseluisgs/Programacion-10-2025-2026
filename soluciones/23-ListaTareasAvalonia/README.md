# 23-ListaTareasAvalonia

## Descripción

Aplicación de **gestión de tareas** (to-do list) desarrollada con **Avalonia UI**, un framework multiplataforma que permite ejecutar la misma aplicación en Windows, Linux y macOS usando XAML similar a WPF.

La aplicación permite:
- ✅ Añadir tareas - Campo de texto + botón
- ✅ Listar tareas - Muestra todas las tareas con su estado
- ✅ Marcar completada - Checkbox para cada tarea
- ✅ Eliminar tarea - Botón para borrar
- ✅ Contador - Muestra tareas pendientes
- ✅ Persistencia - Guarda en archivo JSON local

## Tecnologías

- **Avalonia UI** (v11.x) - Framework multiplataforma
- **.NET 10** - Runtime
- **C# 14** - Lenguaje
- **CommunityToolkit.Mvvm** - Soporte MVVM

## Estructura

```
23-ListaTareasAvalonia/
├── ListaTareasAvalonia/
│   ├── Models/
│   │   └── Tarea.cs              # Modelo de datos
│   ├── Services/
│   │   └── TareaService.cs      # Servicio con persistencia JSON
│   ├── ViewModels/
│   │   └── MainWindowViewModel.cs # Lógica de presentación
│   ├── Views/
│   │   └── MainWindow.axaml     # Interfaz de usuario
│   ├── App.axaml                # Aplicación
│   └── App.axaml.cs             # Punto de entrada
└── README.md
```

## Comparativa con WPF/MAUI

| Aspecto | WPF | Avalonia | MAUI |
|--------|-----|-----------|------|
| Plataformas | Solo Windows | Win/Linux/macOS | Win/Mac/iOS/Android |
| XAML | ✅ | ✅ (similar) | Diferente |
| Binding | {Binding} | {Binding} | {Binding} |
| MVVM | Manual/CommunityToolkit | CommunityToolkit | CommunityToolkit |

## Cómo Ejecutar

```bash
cd 23-ListaTareasAvalonia/ListaTareasAvalonia
dotnet run
```

## Notas

- Avalonia usa una sintaxis XAML casi idéntica a WPF
- El modelo de binding y comandos es muy similar
- La principal diferencia es que funciona en Linux/macOS
- Los estilos y layouts son compatibles entre WPF y Avalonia