# Tareas en Segundo Plano - WPF

Aplicación de escritorio WPF que demuestra cómo manejar tareas pesadas sin bloquear la interfaz de usuario.

## Descripción del Problema

Cuando ejecutamos operaciones largas (leer archivos grandes, descargar datos, procesar información) en el **hilo principal de la UI**, la aplicación se "congela" porque ese hilo está ocupado.

### El Problema

```mermaid
graph TD
    A[Aplicación WPF] --> B[Hilo Principal UI]
    B --> C[Botones]
    B --> D[Eventos]
    B --> E[Dibujar Ventana]
    B --> F[❌ Trabajo Pesado]
    F --> G[UI BLOQUEADA]
    
    style G fill:#ff6b6b,stroke:#333
    style F fill:#ff6b6b,stroke:#333
```

## Soluciones Demonstradas

### 1. ❌ Tarea Bloqueante (NO USAR)

```csharp
// ESTO BLOQUEA LA UI
public void EjecutarTarea()
{
    for (int i = 0; i <= 100; i++)
    {
        Progreso = i;
        Thread.Sleep(50);  // Bloquea todo
    }
}
```

**Resultado**: La UI se congela completamente.

---

### 2. ✅ Tarea con Task.Run + Dispatcher (CORRECTO)

```csharp
// Esto NO BLOQUEA LA UI
public async Task EjecutarTarea()
{
    await Task.Run(() =>
    {
        for (int i = 0; i <= 100; i++)
        {
            // Actualiza la UI desde el hilo secundario
            Application.Current.Dispatcher.Invoke(() =>
            {
                Progreso = i;
            });
            Thread.Sleep(50);
        }
    });
}
```

**Resultado**: La UI sigue respondiendo.

---

### 3. 🚀 Tarea con Async/Await (RECOMENDADO)

```csharp
// Forma moderna y limpia
public async Task EjecutarTarea()
{
    await Task.Run(async () =>
    {
        for (int i = 0; i <= 100; i++)
        {
            Application.Current.Dispatcher.Invoke(() => Progreso = i);
            await Task.Delay(50);  // Mejor que Thread.Sleep
        }
    });
}
```

**Resultado**: Código limpio, UI responsiva.

---

## Arquitectura del Proyecto

### Tecnologías

| Tecnología | Descripción |
|------------|-------------|
| **WPF** | Interfaz de usuario |
| **.NET 10** | Framework |
| **MVVM** | Patrón arquitectura |
| **CommunityToolkit.Mvvm** | Librería MVVM |
| **Serilog** | Logging |

### Estructura

```mermaid
graph TD
    A[App.xaml] --> B[MainWindow]
    B --> C[MainViewModel]
    C --> D[Comandos]
    C --> E[Propiedades]
    
    style A fill:#3498db,color:#fff
    style B fill:#3498db,color:#fff
    style C fill:#2ecc71,color:#fff
    style D fill:#f39c12,color:#fff
    style E fill:#f39c12,color:#fff
```

```
18-TareasBackground/
├── ViewModels/
│   └── MainViewModel.cs            # Lógica de tareas en background
├── Views/
│   ├── MainWindow.xaml            # Interfaz gráfica
│   └── MainWindow.xaml.cs          # Code-behind
├── Infrastructure/
│   └── DependenciesProvider.cs    # Inyección de dependencias
└── App.xaml                       # Punto de entrada
```

## Conceptos Clave

### Thread vs Task

```mermaid
flowchart LR
    subgraph Thread
        T1[Hilo nativo SO]
        T2[Creación costosa]
        T3[Bloquea]
    end
    
    subgraph Task
        TK1[Abstracción .NET]
        TK2[Ligero]
        TK3[Puede awaiterse]
    end
    
    T1 -.-> TK1
    T2 -.-> TK2
    T3 -.-> TK3
```

| Thread | Task |
|--------|------|
| Hilo nativo del SO | Abstracción administrada |
| Creación costosa | Ligero |
| Se bloquea | Puede awaiterse |
| Manual | Gestionado por .NET |

### Flujo de Ejecución

```mermaid
sequenceDiagram
    participant U as Usuario
    participant UI as Hilo UI
    participant BG as Hilo Background
    
    U->>UI: Clic en botón
    UI->>BG: Task.Run()
    BG->>BG: Procesar tarea...
    BG->>UI: Dispatcher.Invoke
    UI->>U: Actualiza ProgressBar
    
    Note over BG,UI: UI nunca se bloquea
```

### Thread.Sleep vs Task.Delay

```mermaid
flowchart TD
    A[Elegir método] --> B{¿Necesitas await?}
    
    B -->|Sí| C[Task.Delay]
    B -->|No| D[Thread.Sleep]
    
    C --> E[✅ No bloquea]
    D --> F[❌ Bloquea]
    
    style E fill:#2ecc71,color:#fff
    style F fill:#ff6b6b,color:#fff
```

```csharp
// ❌ Thread.Sleep - BLOQUEA el hilo actual
Thread.Sleep(1000);

// ✅ Task.Delay - NO BLOQUEA, solo espera
await Task.Delay(1000);
```

### Dispatcher

En WPF, solo el hilo de la UI puede modificar controles de la UI.

```mermaid
flowchart LR
    A[Hilo Background] -->|Actualizar UI| B[Dispatcher]
    B -->|Invoke| C[Hilo UI]
    
    style A fill:#f39c12,color:#fff
    style B fill:#9b59b6,color:#fff
    style C fill:#3498db,color:#fff
```

```csharp
// Desde hilo secundario, actualizamos la UI
Application.Current.Dispatcher.Invoke(() =>
{
    miBoton.Content = "Nuevo texto";
});
```

## Cómo Ejecutar

```bash
cd 18-TareasBackground
dotnet run
```

## Ejercicios Propuestos

1. **Añadir CancellationToken** para poder cancelar la tarea
2. **Usar IProgress<T>** para reportar progreso de forma limpia
3. **Crear una calculadora** que procese operaciones largas
4. **Simular descarga** de archivo con barra de progreso

---

**Nota**: Este proyecto es con fines educativos para el módulo de Programación.
