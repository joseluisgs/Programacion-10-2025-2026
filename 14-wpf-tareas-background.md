# 14 - Tareas en Background: No Bloquear la Interfaz

## 1. El Problema: UI Bloqueada

En aplicaciones WPF, **toda la interacción con controles visuales debe ocurrir en el hilo principal (UI thread)**. Si ejecutas operaciones largas (descargas, cálculos pesados, acceso a BD) en este hilo, la interfaz se congela y el usuario no puede interactuar.

### Ejemplo de problema

```csharp
// ❌ MAL: Operación larga en el hilo UI
private void Button_Click(object sender, RoutedEventArgs e)
{
    // Esto congela la UI durante 5 segundos
    Thread.Sleep(5000); 
    txtResultado.Text = "Terminado";
}
```

**Consecuencias:**
- ❌ Ventana no responde
- ❌ Usuario no puede cancelar
- ❌ Animaciones y ProgressBar no se actualizan
- ❌ Mala experiencia de usuario

---

## 2. Soluciones: Thread, Task, async/await

### 2.1 Thread (enfoque clásico)

```csharp
// ⚠️ Thread manual - más control pero más complejo
private void Button_Click(object sender, RoutedEventArgs e)
{
    var thread = new Thread(() =>
    {
        // Operación en hilo secundario
        Thread.Sleep(5000);
        
        // ❌ ERROR: No puedes acceder a controles directamente
        // txtResultado.Text = "Terminado"; 
        
        // ✅ BIEN: Usar Dispatcher
        Dispatcher.Invoke(() =>
        {
            txtResultado.Text = "Terminado";
        });
    });
    
    thread.Start();
}
```

**Cuándo usar:**
- Control total sobre el ciclo de vida del hilo
- Operaciones de larga duración
- No recomendado para operaciones I/O

### 2.2 Task.Run (enfoque moderno)

```csharp
// ✅ MEJOR: Task.Run con Dispatcher
private void Button_Click(object sender, RoutedEventArgs e)
{
    Task.Run(() =>
    {
        // Operación en ThreadPool
        Thread.Sleep(5000);
        
        // Actualizar UI
        Dispatcher.Invoke(() =>
        {
            txtResultado.Text = "Terminado";
        });
    });
}
```

**Ventajas:**
- Usa ThreadPool (más eficiente)
- Más fácil de gestionar excepciones
- Integración con async/await

### 2.3 async/await (enfoque recomendado)

```csharp
// ✅ ÓPTIMO: async/await
private async void Button_Click(object sender, RoutedEventArgs e)
{
    // Deshabilitar botón durante operación
    btnEjecutar.IsEnabled = false;
    
    // Operación asíncrona
    await Task.Run(() =>
    {
        Thread.Sleep(5000);
    });
    
    // NO necesitas Dispatcher aquí - async/await lo maneja automáticamente
    txtResultado.Text = "Terminado";
    btnEjecutar.IsEnabled = true;
}
```

**Por qué es mejor:**
- El compilador maneja automáticamente el cambio de contexto
- Código más limpio y legible
- Manejo de excepciones natural con try/catch

---

## 3. Dispatcher: El Puente al Hilo UI

El `Dispatcher` de WPF es el mecanismo para ejecutar código en el hilo principal desde hilos secundarios.

### 3.1 Dispatcher.Invoke (bloqueante)

```csharp
// Espera a que se ejecute en el hilo UI
Dispatcher.Invoke(() =>
{
    txtStatus.Text = "Actualizando...";
});
```

### 3.2 Dispatcher.BeginInvoke (no bloqueante)

```csharp
// Encola la operación y continúa sin esperar
Dispatcher.BeginInvoke(() =>
{
    txtStatus.Text = "Actualizando...";
});
```

### 3.3 Cuándo usar cada uno

| Método | Bloqueante | Uso |
|--------|------------|-----|
| `Invoke` | Sí | Cuando necesitas que se ejecute antes de continuar |
| `BeginInvoke` | No | Cuando no importa el orden de ejecución |

---

## 4. IProgress\<T\>: Reportar Progreso

Para operaciones largas, reporta progreso sin bloquear la UI.

```csharp
private async void Button_Click(object sender, RoutedEventArgs e)
{
    var progress = new Progress<int>(value =>
    {
        // Automáticamente en el hilo UI
        progressBar.Value = value;
        txtPorcentaje.Text = $"{value}%";
    });
    
    await TareaLargaAsync(progress);
}

private async Task TareaLargaAsync(IProgress<int> progress)
{
    for (int i = 0; i <= 100; i += 10)
    {
        await Task.Delay(500); // Simula trabajo
        progress?.Report(i);
    }
}
```

**Ventajas:**
- `IProgress<T>` automáticamente ejecuta en el hilo UI
- Desacoplamiento entre lógica y actualización de UI

---

## 5. CancellationToken: Cancelar Operaciones

Permite al usuario cancelar operaciones en curso.

```csharp
private CancellationTokenSource? _cts;

private async void BtnIniciar_Click(object sender, RoutedEventArgs e)
{
    _cts = new CancellationTokenSource();
    btnIniciar.IsEnabled = false;
    btnCancelar.IsEnabled = true;
    
    try
    {
        await TareaLargaAsync(_cts.Token);
        txtResultado.Text = "Completado";
    }
    catch (OperationCanceledException)
    {
        txtResultado.Text = "Cancelado por el usuario";
    }
    finally
    {
        btnIniciar.IsEnabled = true;
        btnCancelar.IsEnabled = false;
    }
}

private void BtnCancelar_Click(object sender, RoutedEventArgs e)
{
    _cts?.Cancel();
}

private async Task TareaLargaAsync(CancellationToken token)
{
    for (int i = 0; i < 100; i++)
    {
        token.ThrowIfCancellationRequested(); // Verifica cancelación
        await Task.Delay(100, token);
    }
}
```

---

## 6. HttpClient y operaciones I/O

Para operaciones de red, **nunca uses Thread.Sleep o bloqueos**.

```csharp
// ❌ MAL
private void Button_Click(object sender, RoutedEventArgs e)
{
    var client = new HttpClient();
    var response = client.GetStringAsync("https://api.example.com/data").Result; // ❌ Bloquea y puede causar deadlock
    txtResultado.Text = response;
}

// ✅ BIEN
private async void Button_Click(object sender, RoutedEventArgs e)
{
    using var client = new HttpClient();
    var response = await client.GetStringAsync("https://api.example.com/data");
    txtResultado.Text = response;
}
```

---

## 7. Comandos Asíncronos en MVVM

Con CommunityToolkit.Mvvm, usa `AsyncRelayCommand`.

```csharp
[RelayCommand]
private async Task CargarDatosAsync()
{
    EsCargando = true;
    
    try
    {
        Datos = await _servicio.ObtenerDatosAsync();
    }
    catch (Exception ex)
    {
        MensajeError = ex.Message;
    }
    finally
    {
        EsCargando = false;
    }
}
```

**Binding en XAML:**

```xml
<Button Content="Cargar" Command="{Binding CargarDatosCommand}" />
<ProgressBar IsIndeterminate="{Binding CargarDatosCommand.IsRunning}" />
```

`AsyncRelayCommand` expone `IsRunning` automáticamente, que puedes enlazar a un `ProgressBar` o spinner.

---

## 8. Resumen de patrones

| Enfoque | Complejidad | UI Segura | Progreso | Cancelación | Recomendado |
|---------|-------------|-----------|----------|-------------|-------------|
| Hilo UI directo | Baja | ❌ | ❌ | ❌ | ❌ Nunca |
| `Thread` manual | Alta | ⚠️ Con Dispatcher | Manual | Manual | Solo casos especiales |
| `Task.Run` | Media | ⚠️ Con Dispatcher | Manual | Con CancellationToken | Para CPU-bound |
| `async/await` | Baja | ✅ Automático | Con `IProgress<T>` | Con CancellationToken | ✅ Recomendado |
| `AsyncRelayCommand` | Muy baja | ✅ Automático | `IsRunning` automático | Integrado | ✅ Recomendado en MVVM |

---

## Proyectos relacionados

- [`16-WpfTareasBackground`](../soluciones/16-WpfTareasBackground/) — Demostración de todos los enfoques
- [`17-WpfStarWars`](../soluciones/17-WpfStarWars/) — API REST con background tasks
- [`14-WpfPokedex`](../soluciones/14-WpfPokedex/) — HttpClient asíncrono y caché de imágenes
