# WPF Calculadora

## Descripción
Aplicación de calculadora básica construida con WPF. El estudiante diseña una interfaz con botones numéricos y de operación, implementando la lógica aritmética completa en el code-behind. El proyecto consolida el manejo de eventos, la gestión del estado interno y el diseño de layouts con `Grid`.

## Objetivos de Aprendizaje
- Diseñar una interfaz de calculadora con `Grid` y botones uniformes
- Gestionar el estado interno de la calculadora (operando acumulado, operación pendiente)
- Reutilizar un único manejador de evento para múltiples botones
- Manejar casos especiales: división por cero, punto decimal, operaciones encadenadas
- Formatear y mostrar resultados numéricos en un `TextBox` o `Label`

## Requisitos Funcionales
- Operaciones aritméticas básicas: suma, resta, multiplicación y división
- Botones del 0 al 9 y separador decimal
- Botón de igual (`=`) que calcula y muestra el resultado
- Botón de borrado completo (`C`) que reinicia la calculadora
- Botón de borrado de último dígito (`⌫`)
- Mostrar el número en curso y la operación seleccionada en pantalla
- Gestionar correctamente la división por cero con un mensaje de error

## Arquitectura
**MVC — Code-Behind**

La lógica de la calculadora (gestión del estado, cálculo) se implementa directamente en `MainWindow.xaml.cs`. Opcionalmente, se puede extraer a una clase `Calculadora` separada para practicar la separación básica de responsabilidades.

## Tecnologías
- WPF (.NET 10)
- C# 14
- JetBrains Rider

## Estructura Sugerida
```
02-WpfCalculadora/
└── WpfCalculadora/
    ├── WpfCalculadora.csproj
    ├── App.xaml
    ├── App.xaml.cs
    ├── MainWindow.xaml
    ├── MainWindow.xaml.cs
    └── Models/
        └── Calculadora.cs        ← (opcional) lógica de cálculo extraída
```

## Notas
- Usar `Grid` con `RowDefinitions` y `ColumnDefinitions` para alinear los botones en filas y columnas regulares.
- El atributo `Grid.Row`, `Grid.Column` y `Grid.ColumnSpan` permite ocupar varias columnas (p.ej. botón `0` o `=`).
- Estudiar cómo obtener el texto de un botón desde el manejador de evento genérico mediante `((Button)sender).Content`.
