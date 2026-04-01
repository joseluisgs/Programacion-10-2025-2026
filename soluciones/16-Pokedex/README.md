# Pokedex WPF

Aplicación de escritorio WPF que muestra un catálogo de Pokemons con información detallada, implementando el patrón MVVM con CommunityToolkit.Mvvm y programación funcional con ROP (Railway Oriented Programming).

## Características

- **Catálogo de Pokemons**: Visualiza más de 1000 pokemons con información completa
- **Búsqueda**: Busca por nombre o ID
- **Filtros**: Filtra por tipo
- **Ordenación**: Ordena por ID, nombre, tipo o generación
- **Paginación**: Navegación por páginas (20 pokemons por página)
- **Detalles completos**: Ver estadísticas base, habilidades, evoluciones, descripción
- **Imágenes**: Carga de imágenes desde PokeAPI
- **Importar/Exportar**: Guardar y cargar datos en JSON
- **Cache LRU**: Caché de 10 pokemons para mejorar rendimiento
- **Interfaz estilo Pokedex**: Diseño clásico con colores rojo, azul y amarillo

## Tecnologías

- **.NET 10** - Framework de desarrollo
- **WPF** - Interfaz de usuario
- **CommunityToolkit.Mvvm** - Implementación MVVM
- **CSharpFunctionalExtensions** - ROP (Railway Oriented Programming)
- **Microsoft.Extensions.DependencyInjection** - Inyección de dependencias
- **Serilog** - Logging

## Estructura del Proyecto

```
16-Pokedex/
├── Cache/                  # Implementación de caché LRU
├── Config/                 # Configuración de la aplicación
├── Converters/            # Convertidores para bindings
├── Data/                   # Datos JSON (pokedex.json)
├── Dto/                   # Data Transfer Objects
├── Errors/                # Errores de dominio (ROP)
├── Infrastructure/        # Inyección de dependencias
├── Mappers/               # Mapeadores DTO <-> Modelo
├── Models/                # Modelos de dominio
├── Repositories/          # Acceso a datos
├── Resources/             # Recursos (iconos, imágenes)
├── Services/              # Lógica de negocio
├── Storage/               # Persistencia JSON
├── Validators/            # Validadores
├── ViewModels/            # ViewModels MVVM
└── Views/                 # Vistas XAML
```

## Patrones y Principios

- **MVVM**: Separación de responsabilidades con ViewModels
- **ROP**: Manejo de errores con Railway Oriented Programming
- **DI**: Inyección de dependencias
- **Repository**: Abstracción del acceso a datos
- **Singleton**: Repositorio singleton para datos en memoria
- **Cache LRU**: Least Recently Used para optimizar rendimiento

## Ejecutar

```bash
cd 16-Pokedex
dotnet run
```

## Datos

Los datos se cargan desde `data/pokedex.json` que contiene información de más de 1000 pokemons incluyendo:
- ID, nombre, tipos
- Estadísticas base (HP, Attack, Defense, etc.)
- Habilidades
- Evoluciones
- Descripción
- Imágenes (Sprite, Thumbnail, Hires)
- Generación, Habitat, Color
- Y muchos más...
