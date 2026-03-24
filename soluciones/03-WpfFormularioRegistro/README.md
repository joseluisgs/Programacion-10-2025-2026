# WPF Formulario de Registro

## Descripción
Formulario de registro de usuario con múltiples tipos de controles de entrada. El estudiante aprende a recoger datos heterogéneos (texto, listas, fechas, opciones booleanas), validarlos en el code-behind y confirmar la operación mediante un `MessageBox`. Introduce la necesidad de validación antes de procesar datos.

## Objetivos de Aprendizaje
- Utilizar los controles de entrada más comunes: `TextBox`, `ComboBox`, `DatePicker`, `CheckBox` y `RadioButton`
- Leer y escribir propiedades de controles desde C# (`Text`, `SelectedItem`, `SelectedDate`, `IsChecked`)
- Implementar validación manual de campos obligatorios y formatos
- Mostrar mensajes de error y confirmación con `MessageBox`
- Organizar el formulario con `StackPanel`, `Grid` y etiquetas descriptivas (`Label`)

## Requisitos Funcionales
- Campo de nombre completo (`TextBox`) — obligatorio
- Campo de correo electrónico (`TextBox`) — obligatorio, formato básico validado
- Selector de país o provincia (`ComboBox`) con al menos 5 opciones
- Selector de fecha de nacimiento (`DatePicker`) — no puede ser fecha futura
- Selector de género mediante `RadioButton`
- Casilla de aceptación de términos y condiciones (`CheckBox`) — obligatoria
- Botón "Registrar" que valida todos los campos y muestra un `MessageBox` de confirmación con los datos introducidos
- Botón "Limpiar" que restablece todos los controles a su valor por defecto

## Arquitectura
**MVC — Code-Behind**

La validación y la lógica de presentación se encuentran en `MainWindow.xaml.cs`. Se puede introducir una clase `UsuarioRegistro` como modelo de datos simple para separar la estructura del usuario de la lógica de la ventana.

## Tecnologías
- WPF (.NET 10)
- C# 14
- JetBrains Rider

## Estructura Sugerida
```
03-WpfFormularioRegistro/
└── WpfFormularioRegistro/
    ├── WpfFormularioRegistro.csproj
    ├── App.xaml
    ├── App.xaml.cs
    ├── MainWindow.xaml
    ├── MainWindow.xaml.cs
    └── Models/
        └── UsuarioRegistro.cs    ← modelo de datos del usuario
```

## Notas
- Utilizar `string.IsNullOrWhiteSpace()` para comprobar campos de texto vacíos.
- Para validar el formato del correo, basta con comprobar que contiene `@` y un punto posterior; no es necesario usar expresiones regulares.
- El `DatePicker` expone la fecha en `SelectedDate` como `DateTime?`; recordar comprobar si es `null` antes de acceder al valor.
- Colorear el borde de los controles inválidos (`BorderBrush`) es una mejora visual opcional pero recomendada.
