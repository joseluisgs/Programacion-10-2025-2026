# WPF Galería de Imágenes

## Descripción
Aplicación de galería fotográfica que permite al usuario abrir imágenes desde el disco y visualizarlas en una cuadrícula. El proyecto introduce los diálogos del sistema operativo (`OpenFileDialog`), el control `Image` de WPF y los paneles de disposición dinámica como `WrapPanel` dentro de un `ScrollViewer`.

## Objetivos de Aprendizaje
- Abrir ficheros del sistema usando `OpenFileDialog` con filtros por tipo de archivo
- Cargar y mostrar imágenes con el control `Image` y `BitmapImage`
- Organizar elementos dinámicos en un `WrapPanel` con desplazamiento (`ScrollViewer`)
- Mostrar una vista ampliada al seleccionar una imagen de la galería
- Gestionar la memoria al cargar múltiples imágenes (`BitmapCacheOption`)
- Practicar el diseño de layouts complejos con `Grid` y paneles anidados

## Requisitos Funcionales
- Botón "Abrir imágenes" que lanza un `OpenFileDialog` con selección múltiple (`.jpg`, `.jpeg`, `.png`, `.bmp`, `.gif`)
- Las imágenes seleccionadas se muestran como miniaturas en un `WrapPanel` con desplazamiento vertical
- Al hacer clic en una miniatura, se muestra la imagen a tamaño completo (o ampliado) en un panel lateral o ventana secundaria
- Botón "Eliminar imagen" que retira la miniatura seleccionada de la galería
- Botón "Limpiar galería" que elimina todas las imágenes
- Contador de imágenes cargadas visible en la barra inferior o cabecera

## Arquitectura
**MVC — Code-Behind**

La lógica de apertura de ficheros y actualización de la galería reside en `MainWindow.xaml.cs`. El enfoque es procedimental: el código manipula directamente el `WrapPanel` añadiendo y eliminando controles `Image` o un `ItemsControl` con `DataTemplate`.

## Tecnologías
- WPF (.NET 10)
- C# 14
- JetBrains Rider

## Estructura Sugerida
```
05-WpfGaleriaImagenes/
└── WpfGaleriaImagenes/
    ├── WpfGaleriaImagenes.csproj
    ├── App.xaml
    ├── App.xaml.cs
    ├── MainWindow.xaml
    ├── MainWindow.xaml.cs
    └── Views/
        └── VistaAmpliada.xaml        ← ventana de visualización ampliada (opcional)
        └── VistaAmpliada.xaml.cs
```

## Notas
- Usar `BitmapCacheOption.OnLoad` al crear `BitmapImage` para liberar el fichero en disco tras la carga y evitar bloqueos.
- Las miniaturas deben tener un tamaño fijo (p.ej. 120×90 px) con `Stretch="UniformToFill"` para mantener proporciones.
- `OpenFileDialog.Multiselect = true` permite seleccionar varios ficheros a la vez; la lista de rutas se obtiene con `FileNames`.
- Limitar el número máximo de imágenes cargadas simultáneamente si se quiere evitar un consumo de memoria excesivo.
