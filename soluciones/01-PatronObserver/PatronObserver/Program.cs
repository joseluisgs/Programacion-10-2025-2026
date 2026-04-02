// Program.cs - Punto de entrada de la aplicación
// =============================================
// Este archivo demuestra los 5 ejercicios del Patrón Observer
// Se ejecuta automáticamente al iniciar el programa
//
// NOTA: En C# 10+, podemos usar "top-level statements" (declaraciones de nivel superior)
// Esto significa que no necesitamos un método Main() explícito - el código se ejecuta directamente

// ============================================================
// USO DE ESPACIOS DE NOMBRES (using)
// ============================================================
// using: indica que vamos a usar tipos de otros espacios de nombres
// Esto nos permite usar las clases sin escribir el namespace completo
using PatronObserver.Ejercicio1;
using PatronObserver.Ejercicio2;
using PatronObserver.Ejercicio3;
using PatronObserver.Ejercicio4;
using PatronObserver.Ejercicio5;

// ============================================================
// EJERCICIO 1: Patrón Observer con Interfaces
// ============================================================
// Demuestra el patrón Observer de forma manual usando interfaces
Console.WriteLine("=".PadRight(50, '='));
Console.WriteLine("EJERCICIO 1: Patrón Observer con Interfaces");
Console.WriteLine("=".PadRight(50, '='));

// Crear el publicador (sujeto) - una cafetería que vende café
var cafeteria = new Cafeteria();

// Crear observadores (suscriptores) - clientes que quieren saber cuando hay café
var cliente1 = new Cliente("Luis");
var cliente2 = new Cliente("Ana");

// Suscribir los clientes al publicador
// El método AddObserver() añade el cliente a la lista de suscriptores
cafeteria.AddObserver(cliente1);
cafeteria.AddObserver(cliente2);

// Cuando la cafetería prepara café, notify a todos los clientes suscritos
cafeteria.PrepararCafe();


// ============================================================
// EJERCICIO 2: Eventos con Delegates
// ============================================================
// Demuestra cómo funcionan los eventos en C# usando delegate explícito
Console.WriteLine("\n" + "=".PadRight(50, '='));
Console.WriteLine("EJERCICIO 2: Eventos con Delegates");
Console.WriteLine("=".PadRight(50, '='));

// Crear un botón
var button = new Button();

// Crear un manejador de eventos (clase que define qué hacer cuando se hace click)
var handler = new ButtonClickHandler();

// Suscribir al evento usando el operador +=
// Esto conecta el evento OnClick con el método HandleClick
button.OnClick += handler.HandleClick;

// Simular que el usuario hace clic en el botón
button.Click();


// ============================================================
// EJERCICIO 3: Eventos con Lambdas
// ============================================================
// Demuestra cómo usar funciones lambda como manejadores de eventos
// Las lambdas son funciones anónimas más compactas
Console.WriteLine("\n" + "=".PadRight(50, '='));
Console.WriteLine("EJERCICIO 3: Eventos con Lambdas");
Console.WriteLine("=".PadRight(50, '='));

var buttonLambda = new ButtonLambda();

// Asignar un manejador usando una lambda
// () => expresión es una función sin parámetros que ejecuta la expresión
buttonLambda.SetOnClickHandler(() => Console.WriteLine("🎉 ¡Lambda ejecutada!"));

// Simular click
buttonLambda.Click();


// ============================================================
// EJERCICIO 4: INotifyPropertyChanged
// ============================================================
// Demuestra la interfaz estándar de .NET para propiedades reactivas
// Es la base del Data Binding en WPF/XAML
Console.WriteLine("\n" + "=".PadRight(50, '='));
Console.WriteLine("EJERCICIO 4: INotifyPropertyChanged");
Console.WriteLine("=".PadRight(50, '='));

// Crear una persona con nombre inicial "Juan"
var persona = new Persona("Juan");

// Crear una vista que observa los cambios de la persona
var view = new PersonaView(persona);

// Cambiar el nombre - esto dispara el evento PropertyChanged
// La vista se entera automáticamente del cambio
persona.Nombre = "María";
persona.Nombre = "Carlos";

// Este cambio no dispara evento porque el valor es igual al actual
persona.Nombre = "Juan";


// ============================================================
// EJERCICIO 5: Eventos con Action<T>
// ============================================================
// Demuestra el uso de Action<T> - delegate predefinido genérico
// Más sencillo que definir nuestros propios delegates
Console.WriteLine("\n" + "=".PadRight(50, '='));
Console.WriteLine("EJERCICIO 5: Eventos con Action<T>");
Console.WriteLine("=".PadRight(50, '='));

var contador = new Contador();

// Suscribirse al evento usando una lambda que recibe el valor
// La lambda valor => ... recibe el valor del contador cuando cambia
contador.OnValueChanged += valor => Console.WriteLine($"🔢 El contador ahora vale: {valor}");

// Incrementar y decrementar - cada cambio dispara el evento
contador.Incrementar();
contador.Incrementar();
contador.Decrementar();
