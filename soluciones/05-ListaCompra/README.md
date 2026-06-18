# 05-ListaCompra

## Descripción
Aplicación completa de Lista de la Compra que introduce el patrón Repository, Inyección de Dependencias y Binding avanzado en WPF.

## Estructura del Proyecto
- **Views/Main/**: Ventana principal con la lista y el formulario de entrada.
- **Repositories/**: Capa de persistencia de datos (en memoria).
- **Services/**: Lógica de negocio y validación.
- **Infrastructure/**: Registro de dependencias.

## Conceptos Clave
- **ObservableCollection**: Notificación automática de cambios a la UI.
- **IValueConverter**: Transformación de datos en el binding (ej: tachado de texto).
- **Dependency Injection**: Desacoplamiento de componentes.

## Tecnologías
- .NET 10 (WPF)
- C# 14
