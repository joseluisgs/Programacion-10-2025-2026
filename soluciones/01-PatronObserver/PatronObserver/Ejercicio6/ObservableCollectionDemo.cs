// ============================================================
// Ejercicio 6: ObservableCollection<T> y Flujos Fríos vs Calientes
// ============================================================
//
// CONCEPTOS CLAVE:
//
// 1. ObservableCollection<T>:
//    - Colección que implementa INotifyCollectionChanged
//    - Notifica automáticamente cuando se añaden, eliminan o reemplazan elementos
//    - Es la base del Data Binding en WPF (DataGrid, ComboBox, ListBox, etc.)
//    - Sin ella, los controles de UI no se enterarían de los cambios
//
// 2. Flujos Calientes (Hot Streams):
//    - Producen datos independientemente de si hay suscriptores
//    - Ejemplo: un sensor de temperatura que emite datos constantemente
//    - Si nadie escucha, los datos se pierden
//    - ObservableCollection es un flujo caliente: emite eventos CollectionChanged
//      sin importar si alguien los escucha o no
//
// 3. Flujos Fríos (Cold Streams):
//    - Solo producen datos cuando hay un suscriptor
//    - Ejemplo: una consulta a base de datos que se ejecuta al suscribirse
//    - Cada suscriptor obtiene su propia ejecución independiente
//    - IEnumerable<T> es un flujo frío: no produce datos hasta que iteras
//
// 4. ¿Por qué es importante en WPF?
//    - DataGrid, ComboBox, ListBox se suscriben a CollectionChanged
//    - Cuando añades un elemento, la UI se actualiza automáticamente
//    - Si usaras List<T>, la UI NO se enteraría del cambio
//    - Esto es el fundamento del Data Binding reactivo que veremos en UD10

using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace PatronObserver.Ejercicio6;

// ============================================================
// MODELO: Producto (similar al que usaremos en WPF)
// ============================================================

public record Producto(int Id, string Nombre, decimal Precio);

// ============================================================
// DEMO 1: ObservableCollection<T> como flujo caliente
// ============================================================
// ObservableCollection emite eventos INDEPENDIENTEMENTE de si alguien escucha.
// Es un flujo caliente: produce datos (eventos) aunque no haya suscriptores.

public static class DemoObservableCollection
{
    public static void Ejecutar()
    {
        Console.WriteLine("--- Demo 1: ObservableCollection<T> como flujo caliente ---");

        // Creamos la colección con datos iniciales
        var productos = new ObservableCollection<Producto>
        {
            new(1, "Teclado", 29.99m),
            new(2, "Ratón", 15.50m),
            new(3, "Monitor", 199.99m)
        };

        // Nos suscribimos al evento CollectionChanged
        // Esto es como "escuchar" el flujo caliente
        productos.CollectionChanged += (sender, e) =>
        {
            Console.WriteLine($"📢 Evento: {e.Action}");
            if (e.NewItems != null)
            {
                foreach (Producto p in e.NewItems)
                    Console.WriteLine($"   ➕ Añadido: {p.Nombre} ({p.Precio:C})");
            }
            if (e.OldItems != null)
            {
                foreach (Producto p in e.OldItems)
                    Console.WriteLine($"   ➖ Eliminado: {p.Nombre}");
            }
        };

        Console.WriteLine("\n📦 Estado inicial:");
        foreach (var p in productos)
            Console.WriteLine($"   [{p.Id}] {p.Nombre} - {p.Precio:C}");

        // Añadimos un producto → la UI (suscriptor) se entera automáticamente
        Console.WriteLine("\n➕ Añadiendo 'Webcam'...");
        productos.Add(new Producto(4, "Webcam", 45.00m));

        // Eliminamos un producto → la UI se actualiza
        Console.WriteLine("\n➖ Eliminando 'Ratón'...");
        productos.RemoveAt(1);

        // Reemplazamos un producto
        Console.WriteLine("\n🔄 Reemplazando 'Teclado' por 'Teclado Mecánico'...");
        productos[0] = new Producto(5, "Teclado Mecánico", 59.99m);

        Console.WriteLine("\n📦 Estado final:");
        foreach (var p in productos)
            Console.WriteLine($"   [{p.Id}] {p.Nombre} - {p.Precio:C}");
    }
}

// ============================================================
// DEMO 2: List<T> vs ObservableCollection<T>
// ============================================================
// List<T> NO notifica cambios. Si un DataGrid usa List<T>,
// los cambios no se reflejan en la UI.

public static class DemoListVsObservable
{
    public static void Ejecutar()
    {
        Console.WriteLine("\n--- Demo 2: List<T> vs ObservableCollection<T> ---");

        // List<T>: flujo frío, no notifica
        var lista = new List<Producto>
        {
            new(1, "Auriculares", 35.00m)
        };

        Console.WriteLine("📋 List<T>: NO dispara eventos de cambio");
        Console.WriteLine($"   Elementos antes de añadir: {lista.Count}");
        lista.Add(new Producto(2, "Altavoz", 22.00m));
        Console.WriteLine($"   Elementos después de añadir: {lista.Count}");
        Console.WriteLine("   ⚠️  La UI NO se enteraría de este cambio");

        // ObservableCollection<T>: flujo caliente, sí notifica
        var observable = new ObservableCollection<Producto>
        {
            new(1, "Auriculares", 35.00m)
        };

        bool uiNotificada = false;
        observable.CollectionChanged += (_, _) => uiNotificada = true;

        Console.WriteLine("\n📋 ObservableCollection<T>: SÍ dispara eventos");
        observable.Add(new Producto(2, "Altavoz", 22.00m));
        Console.WriteLine($"   ¿La UI fue notificada? {(uiNotificada ? "✅ Sí" : "❌ No")}");
    }
}

// ============================================================
// DEMO 3: Flujo frío (IEnumerable) vs Flujo caliente (ObservableCollection)
// ============================================================
// IEnumerable: los datos se generan solo cuando iteras (cold)
// ObservableCollection: los datos se generan continuamente (hot)

public static class DemoFlujos
{
    // Flujo frío: solo genera datos cuando alguien itera
    private static IEnumerable<int> GenerarNumerosPares(int max)
    {
        Console.WriteLine("   🔄 Generando números pares (flujo frío)...");
        for (int i = 2; i <= max; i += 2)
        {
            Console.WriteLine($"      → Yield return {i}");
            yield return i;
        }
    }

    public static void Ejecutar()
    {
        Console.WriteLine("\n--- Demo 3: Flujo frío vs Flujo caliente ---");

        // FLUJO FRÍO (IEnumerable): No se ejecuta hasta que iteras
        Console.WriteLine("\n❄️  FLUJO FRÍO (IEnumerable<T>):");
        Console.WriteLine("   Creando el enumerable (aún no se ejecuta nada)...");
        var numeros = GenerarNumerosPares(6);
        Console.WriteLine("   Enumerable creado. No se ha generado ningún número.");

        Console.WriteLine("   Primera iteración:");
        foreach (var n in numeros)
        {
            Console.WriteLine($"      Recibido: {n}");
        }

        Console.WriteLine("   Segunda iteración (se regenera todo):");
        foreach (var n in numeros)
        {
            Console.WriteLine($"      Recibido: {n}");
        }

        // FLUJO CALIENTE (ObservableCollection): Ya tiene los datos
        Console.WriteLine("\n🔥 FLUJO CALIENTE (ObservableCollection<T>):");
        var coleccion = new ObservableCollection<int> { 2, 4, 6 };
        Console.WriteLine("   Colección creada con datos: { " + string.Join(", ", coleccion) + " }");
        Console.WriteLine("   Los datos YA existen, no se regeneran al iterar");

        coleccion.CollectionChanged += (_, e) =>
        {
            if (e.Action == NotifyCollectionChangedAction.Add && e.NewItems != null)
            {
                foreach (int n in e.NewItems)
                    Console.WriteLine($"   📢 ¡Nuevo dato en caliente!: {n}");
            }
        };

        Console.WriteLine("   Añadiendo 8 en tiempo real...");
        coleccion.Add(8);
    }
}
