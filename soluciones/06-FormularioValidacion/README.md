# 05-WpfFormularioValidacion

## Descripción
Formulario de registro con validación completa implementado mediante el patrón MVVM. El proyecto introduce los mecanismos de validación de datos en WPF: `ValidationRules`, `IDataErrorInfo` e `INotifyDataErrorInfo`, mostrando mensajes de error contextuales junto a cada campo.

## Objetivos de Aprendizaje
- Aplicar validación de datos en WPF usando `IDataErrorInfo` o `INotifyDataErrorInfo`
- Definir `ValidationRules` personalizadas para controlar el binding
- Mostrar mensajes de error en la vista mediante `Validation.ErrorTemplate`
- Deshabilitar el botón de envío cuando el formulario contiene errores (`CanExecute`)
- Diferenciar entre validación en la capa de Model y en la capa de ViewModel
- Introducir el patrón MVVM aplicado a formularios con estado de validación

## Requisitos Funcionales
- RF-01: Formulario con campos: nombre, apellidos, correo electrónico, contraseña y confirmación de contraseña
- RF-02: Validación de nombre y apellidos: no vacío, mínimo 2 caracteres
- RF-03: Validación de correo electrónico: formato válido (regex)
- RF-04: Validación de contraseña: mínimo 8 caracteres, al menos un número
- RF-05: Validación cruzada: contraseña y confirmación deben coincidir
- RF-06: Los mensajes de error deben mostrarse junto al campo incorrecto
- RF-07: El botón "Registrar" debe estar deshabilitado mientras haya errores
- RF-08: Al enviar correctamente, mostrar un mensaje de confirmación

## Requisitos No Funcionales

| Código | Requisito | Descripción |
|--------|-----------|-------------|
| RNF-01 | Arquitectura | Patrón MVVM; la vista no contiene lógica de validación |
| RNF-02 | Usabilidad | Los errores se muestran en tiempo real al perder el foco o al escribir |
| RNF-03 | Mantenibilidad | Las reglas de validación son reutilizables y están en clases separadas |

## Arquitectura
**MVVM con validación mediante `IDataErrorInfo`**

```
View  ──binding──►  ViewModel  ──valida──►  Model
(XAML)             (expone errores         (IDataErrorInfo)
                    al formulario)
```

El ViewModel implementa `IDataErrorInfo` (o delega en el Model) para que el motor de binding de WPF pueda consultar el estado de validación de cada propiedad.

## Tecnologías
- WPF (.NET 10)
- C# 14
- JetBrains Rider

## Estructura Sugerida
```
05-WpfFormularioValidacion/
└── WpfFormularioValidacion/
    ├── WpfFormularioValidacion.csproj
    ├── App.xaml
    ├── App.xaml.cs
    ├── Models/
    │   └── RegistroUsuario.cs       ← IDataErrorInfo
    ├── ViewModels/
    │   └── RegistroViewModel.cs
    └── Views/
        ├── MainWindow.xaml
        └── MainWindow.xaml.cs
```

## Funcionalidades Clave
- Validación en tiempo real campo a campo
- Plantilla de error personalizada (`Validation.ErrorTemplate`) con borde rojo y tooltip
- Comando `RegistrarCommand` con `CanExecute` basado en el estado global de validación
- Validación cruzada de contraseña/confirmación

## Notas
- Alineado con la teoría de `08-wpf-arquitectura-mvvm.md`
- `ValidatesOnDataErrors=True` debe activarse en cada binding para que WPF consulte `IDataErrorInfo`
- `UpdateSourceTrigger=PropertyChanged` permite validar mientras el usuario escribe
- Para proyectos más avanzados, considerar `INotifyDataErrorInfo` (soporta errores asíncronos y múltiples errores por propiedad)
