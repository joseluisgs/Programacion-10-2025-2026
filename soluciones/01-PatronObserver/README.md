# 01-PatronObserver

## Patrón Observer - Ejercicios en Consola

### Descripción

Serie de ejercicios prácticos para entender el **Patrón Observer** y los **eventos** en C#, implementados en consola. Son los fundamentos de las interfaces reactivas.

### Ejercicios

| # | Ejercicio | Concepto |
|---|-----------|----------|
| 1 | IObservable/IObserver | Patrón Observer con interfaces |
| 2 | Button con Delegates | Eventos con delegates |
| 3 | Button con Lambdas | Eventos con funciones lambda |
| 4 | INotifyPropertyChanged | Propiedades reactivas |
| 5 | Action\<T\> | Eventos con Action genérico |
| 6 | ObservableCollection\<T\> | Colecciones reactivas y flujos fríos/calientes |

### Estructura

```
01-PatronObserver/
├── PatronObserver.slnx
└── PatronObserver/
    ├── PatronObserver.csproj
    ├── Program.cs
    └── Ejercicio1/
        └── IObservable.cs
    └── Ejercicio2/
        └── Button.cs
    └── Ejercicio3/
        └── ButtonLambda.cs
    └── Ejercicio4/
        └── Persona.cs
    └── Ejercicio5/
        └── Contador.cs
    └── Ejercicio6/
        └── ObservableCollectionDemo.cs
```

### Compilar y ejecutar

```bash
cd 01-PatronObserver
dotnet build
dotnet run
```

### Salida esperada

```
==================================================
EJERCICIO 1: Patrón Observer con Interfaces
==================================================
☕ Preparando café...
✅ Café listo!
📢 Luis recibe: ¡El café está listo!
📢 Ana recibe: ¡El café está listo!

==================================================
EJERCICIO 2: Eventos con Delegates
==================================================
🔘 Botón pulsado
🎉 ¡Manejador de click ejecutado!

==================================================
EJERCICIO 3: Eventos con Lambdas
==================================================
🔘 Lambda Button pulsado
🎉 ¡Lambda ejecutada!

==================================================
EJERCICIO 4: INotifyPropertyChanged
==================================================
🔄 La propiedad 'Nombre' cambió a: María
🔄 La propiedad 'Nombre' cambió a: Carlos

==================================================
EJERCICIO 5: Eventos con Action<T>
==================================================
🔢 El contador ahora vale: 1
🔢 El contador ahora vale: 2
```

### Conceptos aprendidos

- **Patrón Observer**: Un publicador notifica a múltiples suscriptores
- **Delegates**: Tipos que referencian métodos
- **Eventos**: Delegates con protección de invocación
- **Lambdas**: Funciones anónimas
- **INotifyPropertyChanged**: Base del data binding
- **Action\<T\>**: Delegate predefinido genérico
- **ObservableCollection\<T\>**: Colección que notifica cambios (CollectionChanged)
- **Flujos calientes**: Producen datos independientemente de los suscriptores (ObservableCollection)
- **Flujos fríos**: Solo producen datos cuando hay un suscriptor (IEnumerable)
- **Por qué ObservableCollection**: Sin ella, DataGrid/ComboBox/ListBox no se enteran de los cambios
