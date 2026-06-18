# Cuestionario: Interfaces Gráficas con .NET

- [Cuestionario: Interfaces Gráficas con .NET](#cuestionario-interfaces-gráficas-con-net)
  - [Eventos y Patrón Observer](#eventos-y-patrón-observer)
  - [Windows Forms vs WPF](#windows-forms-vs-wpf)
  - [MVVM y Data Binding](#mvvm-y-data-binding)
  - [Blazor Server](#blazor-server)
  - [.NET MAUI](#net-maui)
  - [Estilos y Temas](#estilos-y-temas)
  - [Tareas en Background](#tareas-en-background)

---

## Eventos y Patrón Observer

1. **Patrón Observer vs Eventos:** Investiga y explica la diferencia entre el patrón Observer clásico (con interfaces) y el sistema de eventos de C#. ¿Cuáles son las ventajas de usar eventos sobre interfaces?

2. **INotifyPropertyChanged:** Investiga cómo funciona esta interfaz en profundidad. ¿Por qué es fundamental para el data binding en WPF? ¿Cómo se implementa manualmente y cómo lo hace CommunityToolkit.Mvvm automáticamente?

3. **EventAggregator:** Investiga este patrón de comunicación. ¿En qué escenarios es útil? Proporciona un ejemplo de implementación básica.

4. **Weak References en Eventos:** ¿Por qué es importante usar `WeakReference` en implementaciones del patrón Observer para evitar memory leaks?

5. **Eventos vs Commands:** Investiga la diferencia entre usar eventos (`event Handler`) y comandos (`ICommand`) en WPF/MVVM. ¿Cuándo es preferible usar cada uno?

---

## Windows Forms vs WPF

6. **WinForms vs WPF:** Investiga las diferencias fundamentales entre WinForms y WPF. Considera: modelo de programación (imperativo vs declarativo), renderizado (GDI+ vs DirectX), data binding, y flexibilidad de UI.

7. **Anchor vs Dock:** Explica en detalle cómo funcionan las propiedades `Anchor` y `Dock` en WinForms. ¿Cuándo usarías cada una?

8. **Ciclo de vida de un Form:** Investiga los eventos del ciclo de vida de un formulario WinForms: `Load`, `Shown`, `Closing`, `Closed`, `Disposed`. ¿En qué orden se disparan?

9. **GDI+ vs DirectX:** Investiga las diferencias técnicas en el renderizado. ¿Por qué WPF usa DirectX y qué ventajas ofrece?

10. **Code-Behind vs XAML:** En WinForms todo el código está en code-behind. En WPF tenemos XAML. Investiga las ventajas de separar la definición de la interfaz (XAML) del código (C#).

---

## MVVM y Data Binding

11. **Historia de MVVM:** Investiga quién inventó MVVM y en qué contexto. ¿Cómo evolucionó desde MVC?

12. **INotifyPropertyChanged vs ICommand:** Explica la diferencia y cómo se complementan en MVVM. ¿Por qué son las dos piezas fundamentales?

13. **Modos de Binding:** Investiga los diferentes modos de binding en WPF: `OneWay`, `TwoWay`, `OneWayToSource`, `OneTime`. ¿Cuándo usar cada uno?

14. **UpdateSourceTrigger:** Investiga esta propiedad del binding. ¿Cuál es la diferencia entre `Default`, `PropertyChanged`, `LostFocus` y `Explicit`?

15. **IValueConverter:** Investiga cómo crear convertidores de valores para el binding. Proporciona un ejemplo de un convertidor para formatear moneda.

16. **CommunityToolkit.Mvvm:** Investiga esta biblioteca oficial de Microsoft. ¿Qué source generators aporta? ¿Cómo simplifica MVVM?

17. **ObservableCollection vs List:** Investiga la diferencia. ¿Por qué `ObservableCollection` actualiza la UI automáticamente y `List` no?

18. **DataContext inheritance:** Investiga cómo funciona la herencia de DataContext en WPF. ¿Cómo afectan los cambios de DataContext a los bindings?

---

## Blazor Server

19. **SignalR:** Investiga cómo funciona SignalR. ¿Por qué Blazor Server lo usa? ¿Cómo mantiene la conexión en tiempo real?

20. **Blazor Server vs Blazor WebAssembly:** Investiga las diferencias. ¿Cuándo es preferible cada uno?

21. **Ciclo de vida de un componente Blazor:** Investiga los métodos del ciclo de vida: `OnInitialized`, `OnParametersSet`, `OnAfterRender`, `StateHasChanged`. ¿En qué orden se ejecutan?

22. **EventCallback vs EventCallback<T>:** Investiga la diferencia. ¿Cuándo usar cada uno?

23. **Blazor vs WPF:** Investiga las similitudes y diferencias. ¿Cómo se compara el modelo de componentes de Blazor con los controles de WPF?

24. **Rendering en Blazor:** Investiga cómo funciona el rendering en Blazor Server. ¿Qué es el Virtual DOM y cómo se compara con otros frameworks?

25. **Protección de Circuitos:** Investiga qué son los circuitos en Blazor Server. ¿Cómo se maneja la desconexión de un cliente?

---

## .NET MAUI

26. **Historia de MAUI:** Investiga la evolución: Xamarin.Forms → .NET MAUI. ¿Qué cambios importantes introdujo?

27. **Shell en MAUI:** Investiga el sistema de navegación Shell de MAUI. ¿Qué ventajas tiene sobre la navegación tradicional?

28. **Platform-Specific Code:** Investiga cómo escribir código específico por plataforma en MAUI. ¿Qué es conditional compilation?

29. **MAUI vs WPF:** Investiga las diferencias principales. ¿Qué tiene MAUI que WPF no tiene y viceversa?

30. **.NET MAUI Essentials:** Investiga qué APIs multiplataforma proporciona. Nombra al menos 10.

31. **Hot Reload en MAUI:** Investiga las capacidades de Hot Reload. ¿Qué limitaciones tiene?

32. **MAUI vs Flutter vs React Native:** Investiga las diferencias entre estos tres frameworks multiplataforma. ¿Cuáles son las ventajas de cada uno?

---

## Estilos y Temas

33. **StaticResource vs DynamicResource:** Investiga en detalle cuándo usar cada uno. ¿Cuál tiene mejor rendimiento?

34. **Estilos implícitos vs explícitos:** Investiga la diferencia. ¿Cómo se aplica un estilo sin `StaticResource`?

35. **BasedOn en estilos:** Investiga cómo heredar y extender estilos. Proporciona un ejemplo.

36. **Triggers en WPF:** Investiga los diferentes tipos: Property Triggers, Data Triggers, MultiTriggers, Event Triggers. Proporciona ejemplos de cada uno.

37. **ControlTemplate:** Investiga qué es y para qué sirve. ¿Cómo personalizar la apariencia de un Button?

38. **DataTemplate:** Investiga diferentes usos: ItemTemplate, ContentTemplate, DataTemplateSelector.

39. **MaterialDesignInXaml:** Investiga esta biblioteca. ¿Cómo se instala y configura? Proporciona ejemplos de uso.

40. **MahApps.Metro:** Investiga esta biblioteca. ¿Qué tipo de aplicaciones mejoran?

---

## Tareas en Background

41. **UI Thread:** Investiga por qué las aplicaciones GUI tienen un hilo principal dedicado a la UI. ¿Qué problemas ocurren si se bloquea?

42. **async/await en GUI:** Investiga cómo usar async/await para no bloquear la UI. ¿Qué es el patrón async void?

43. **Dispatcher:** Investiga cómo funciona en WPF. ¿Cuándo necesitas usar `Dispatcher.Invoke`?

44. **BackgroundWorker vs Task:** Investiga las diferencias. ¿Por qué Task es preferible en código moderno?

45. **IProgress y CancellationToken:** Investiga cómo reportar progreso y permitir cancelación en operaciones asíncronas.

46. **ConfigureAwait:** Investiga para qué sirve esta méthode. ¿Por qué a veces es necesario?

47. **Task.WhenAll vs Task.WaitAll:** Investiga la diferencia. ¿Cuándo usar cada uno?

48. **Sincronización de hilos:** Investiga los problemas de concurrencia cuando se actualiza la UI desde un hilo secundario. ¿Qué mecanismos existen para evitarlo?
