# WPF Hola Mundo

## Descripción
Proyecto introductorio a WPF. Consiste en una ventana con una etiqueta y un botón que, al pulsarse, cambia el texto mostrado. Es el punto de entrada para familiarizarse con el entorno de desarrollo y la estructura básica de una aplicación WPF.

## Objetivos de Aprendizaje
- Comprender la estructura de un proyecto WPF (.NET 10)
- Conocer el rol de los ficheros `MainWindow.xaml` y `MainWindow.xaml.cs`
- Escribir el primer manejador de evento (`Click`) en el code-behind
- Manipular propiedades de controles desde C# (`Content`, `Text`)
- Entender el ciclo de vida básico de una ventana WPF

## Requisitos Funcionales
- La ventana debe mostrar el texto "¡Hola, Mundo!" al iniciarse
- Un botón con la etiqueta "Cambiar texto" debe modificar el mensaje mostrado al ser pulsado
- El botón debe alternar entre al menos dos mensajes distintos con cada pulsación
- La interfaz debe estar centrada y ser visualmente clara

## Arquitectura
**MVC — Code-Behind**

Toda la lógica de presentación reside en el fichero `MainWindow.xaml.cs`. No se aplica separación de capas avanzada; el objetivo es entender la relación directa entre XAML y C# antes de introducir patrones más complejos.

## Tecnologías
- WPF (.NET 10)
- C# 14
- JetBrains Rider

## Estructura Sugerida
```
01-WpfHolaMundo/
└── WpfHolaMundo/
    ├── WpfHolaMundo.csproj
    ├── App.xaml
    ├── App.xaml.cs
    ├── MainWindow.xaml
    └── MainWindow.xaml.cs
```

## Notas
- Explorar la ventana de propiedades de Rider para modificar atributos XAML visualmente.
- Fijarse en cómo el atributo `x:Name` de un control XAML lo expone como campo en el code-behind.
- No es necesario separar lógica en clases adicionales para este ejercicio.
