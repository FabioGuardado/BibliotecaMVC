# BibliotecaMVC

Aplicación web desarrollada con **ASP.NET Core MVC** sobre **.NET 10**, creada como proyecto académico para la asignatura de Programación Frontend — UEES, ciclo 4.

## Requisitos previos

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- (Opcional) [Visual Studio 2022/2026](https://visualstudio.microsoft.com/) con la carga de trabajo **Desarrollo web y ASP.NET**
- (Opcional) [Visual Studio Code](https://code.visualstudio.com/) con la extensión [C# Dev Kit](https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.csdevkit)

## Clonar el repositorio

```sh
git clone https://github.com/FabioGuardado/BibliotecaMVC.git
cd BibliotecaMVC
```

## Ejecutar en modo desarrollo

### Opción A — CLI de .NET

```sh
cd BibliotecaMVC
dotnet run
```

La aplicación estará disponible en `https://localhost:5001` (o el puerto que indique la consola).

Para habilitar **hot reload** durante el desarrollo:

```sh
dotnet watch run
```

### Opción B — Visual Studio

1. Abrir `BibliotecaMVC.slnx`.
2. Establecer `BibliotecaMVC` como proyecto de inicio.
3. Presionar **F5** para iniciar con depurador, o **Ctrl+F5** sin depurador.

## Estructura del proyecto

```
BibliotecaMVC/
├── Controllers/        # Controladores MVC
├── Models/             # Modelos de datos
├── Views/              # Vistas Razor (.cshtml)
│   └── Shared/         # Layout y vistas compartidas
├── wwwroot/            # Archivos estáticos (CSS, JS, imágenes)
├── Program.cs          # Punto de entrada y configuración
└── BibliotecaMVC.csproj
```

## Tecnologías utilizadas

- ASP.NET Core MVC (.NET 10)
- Bootstrap 5
- jQuery