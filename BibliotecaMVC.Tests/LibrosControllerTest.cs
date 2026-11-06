using BibliotecaMVC.Controllers;
using BibliotecaMVC.Data;
using BibliotecaMVC.Models;
using BibliotecaMVC.Repositories;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace BibliotecaMVC.Tests;

public class LibrosControllerTests
{
    [Fact]
    public async Task Index_ConLibros_DevuelveLaListaDeLibros()
    {
        await using var context = CreateContext();
        context.Libros.AddRange(
            CreateLibro("Cien años de soledad", "Gabriel García Márquez"),
            CreateLibro("La casa de los espíritus", "Isabel Allende"));
        await context.SaveChangesAsync();
        var controller = CreateController(context);

        var result = await controller.Index();

        var view = Assert.IsType<ViewResult>(result);
        var libros = Assert.IsAssignableFrom<IEnumerable<Libro>>(view.Model);
        Assert.NotNull(libros);
        Assert.Equal(2, libros.Count());
    }

    [Fact]
    public async Task Index_SinLibros_DevuelveListaVacia()
    {
        await using var context = CreateContext();
        var controller = CreateController(context);

        var result = await controller.Index();

        var view = Assert.IsType<ViewResult>(result);
        var libros = Assert.IsAssignableFrom<IEnumerable<Libro>>(view.Model);
        Assert.Empty(libros);
    }

    [Fact]
    public async Task Details_ConLibroExistente_DevuelveElLibro()
    {
        await using var context = CreateContext();
        var libro = CreateLibro("Rayuela", "Julio Cortázar");
        context.Libros.Add(libro);
        await context.SaveChangesAsync();
        var controller = CreateController(context);

        var result = await controller.Details(libro.ID);

        var view = Assert.IsType<ViewResult>(result);
        Assert.Same(libro, view.Model);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(999)]
    public async Task Details_ConLibroInexistente_DevuelveNotFoundYRegistraAdvertencia(int id)
    {
        await using var context = CreateContext();
        var logger = new Mock<ILogger<LibrosController>>();
        var controller = CreateController(context, logger);

        var result = await controller.Details(id);

        Assert.IsType<NotFoundResult>(result);
        VerifyLog(logger, LogLevel.Warning, Times.Once());
    }

    [Fact]
    public async Task EditGet_ConLibroExistente_DevuelveElLibro()
    {
        await using var context = CreateContext();
        var libro = CreateLibro("Ficciones", "Jorge Luis Borges");
        context.Libros.Add(libro);
        await context.SaveChangesAsync();
        var controller = CreateController(context);

        var result = await controller.Edit(libro.ID);

        var view = Assert.IsType<ViewResult>(result);
        Assert.Same(libro, view.Model);
    }

    [Fact]
    public async Task EditGet_ConLibroInexistente_DevuelveNotFound()
    {
        await using var context = CreateContext();
        var controller = CreateController(context);

        var result = await controller.Edit(999);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Create_ConLibroValido_GuardaElLibroYRedirigeAlIndice()
    {
        await using var context = CreateContext();
        var controller = CreateController(context);
        var libro = CreateViewModel("Conversación en La Catedral", "Mario Vargas Llosa");

        var result = await controller.Create(libro);

        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal(nameof(LibrosController.Index), redirect.ActionName);
        Assert.Equal(1, await context.Libros.CountAsync());
        var guardado = await context.Libros.SingleAsync();
        Assert.Equal("Conversación en La Catedral", guardado.Titulo);
        Assert.Equal("Mario Vargas Llosa", guardado.Autor);
        Assert.Equal(25.5m, guardado.Precio);
        Assert.Null(guardado.ImageUrl);
    }

    [Theory]
    [InlineData(null, true)]
    [InlineData(true, true)]
    [InlineData(false, false)]
    public async Task Create_SegunDisponible_GuardaLaDisponibilidadEsperada(bool? disponible, bool esperado)
    {
        await using var context = CreateContext();
        var controller = CreateController(context);
        var libro = CreateViewModel("Pedro Páramo", "Juan Rulfo");
        libro.Disponible = disponible;

        await controller.Create(libro);

        var guardado = await context.Libros.SingleAsync();
        Assert.Equal(esperado, guardado.Disponible);
    }

    [Fact]
    public async Task Create_ConModeloInvalido_NoGuardaYDevuelveLaVista()
    {
        await using var context = CreateContext();
        var logger = new Mock<ILogger<LibrosController>>();
        var controller = CreateController(context, logger);
        controller.ModelState.AddModelError(nameof(CreateLibroViewModel.Titulo), "El título es obligatorio.");
        var libro = new CreateLibroViewModel { Autor = "Autor", Categoria = "Novela" };

        var result = await controller.Create(libro);

        var view = Assert.IsType<ViewResult>(result);
        Assert.Same(libro, view.Model);
        Assert.False(controller.ModelState.IsValid);
        Assert.Empty(context.Libros);
        VerifyLog(logger, LogLevel.Error, Times.Once());
    }

    [Fact]
    public async Task EditPost_ConLibroValido_ActualizaElLibro()
    {
        var databaseName = Guid.NewGuid().ToString();
        int libroId;
        await using (var seedContext = CreateContext(databaseName))
        {
            var libro = CreateLibro("Título original", "Autor original");
            seedContext.Libros.Add(libro);
            await seedContext.SaveChangesAsync();
            libroId = libro.ID;
        }

        await using var context = CreateContext(databaseName);
        var controller = CreateController(context);
        var libroEditado = CreateLibro("Título actualizado", "Autor original");
        libroEditado.ID = libroId;
        libroEditado.Precio = 40m;
        libroEditado.Disponible = false;

        var result = await controller.Edit(libroEditado);

        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal(nameof(LibrosController.Index), redirect.ActionName);
        var guardado = await context.Libros.AsNoTracking().SingleAsync();
        Assert.Equal("Título actualizado", guardado.Titulo);
        Assert.Equal(40m, guardado.Precio);
        Assert.False(guardado.Disponible);
    }

    [Fact]
    public async Task EditPost_ConLibroInexistente_DevuelveNotFound()
    {
        await using var context = CreateContext();
        var controller = CreateController(context);
        var libro = CreateLibro("Libro inexistente", "Autor");
        libro.ID = 999;

        var result = await controller.Edit(libro);

        Assert.IsType<NotFoundResult>(result);
        Assert.Empty(context.Libros);
    }

    [Fact]
    public async Task EditPost_ConModeloInvalido_NoActualizaYDevuelveLaVista()
    {
        var databaseName = Guid.NewGuid().ToString();
        int libroId;
        await using (var seedContext = CreateContext(databaseName))
        {
            var libro = CreateLibro("Título original", "Autor");
            seedContext.Libros.Add(libro);
            await seedContext.SaveChangesAsync();
            libroId = libro.ID;
        }

        await using var context = CreateContext(databaseName);
        var controller = CreateController(context);
        controller.ModelState.AddModelError(nameof(Libro.Titulo), "El título es obligatorio.");
        var libroEditado = new Libro { ID = libroId, Autor = "Autor", Categoria = "Novela" };

        var result = await controller.Edit(libroEditado);

        var view = Assert.IsType<ViewResult>(result);
        Assert.Same(libroEditado, view.Model);
        var guardado = await context.Libros.AsNoTracking().SingleAsync();
        Assert.Equal("Título original", guardado.Titulo);
    }

    [Fact]
    public async Task Delete_ConLibroExistente_EliminaElLibroYRedirigeAlIndice()
    {
        await using var context = CreateContext();
        var libro = CreateLibro("Libro para eliminar", "Autor");
        var otro = CreateLibro("Libro que se conserva", "Autor");
        context.Libros.AddRange(libro, otro);
        await context.SaveChangesAsync();
        var controller = CreateController(context);

        var result = await controller.Delete(libro.ID);

        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal(nameof(LibrosController.Index), redirect.ActionName);
        Assert.Equal(1, await context.Libros.CountAsync());
        Assert.Equal("Libro que se conserva", (await context.Libros.SingleAsync()).Titulo);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(999)]
    public async Task Delete_ConLibroInexistente_DevuelveNotFoundYRegistraAdvertencia(int id)
    {
        await using var context = CreateContext();
        var logger = new Mock<ILogger<LibrosController>>();
        var controller = CreateController(context, logger);

        var result = await controller.Delete(id);

        Assert.IsType<NotFoundResult>(result);
        VerifyLog(logger, LogLevel.Warning, Times.Once());
    }

    [Theory]
    [InlineData(nameof(CreateLibroViewModel.Titulo))]
    [InlineData(nameof(CreateLibroViewModel.Autor))]
    [InlineData(nameof(CreateLibroViewModel.Categoria))]
    [InlineData(nameof(CreateLibroViewModel.Precio))]
    public async Task Create_ConCualquierCampoInvalido_NoGuardaElLibro(string propiedad)
    {
        await using var context = CreateContext();
        var controller = CreateController(context);
        controller.ModelState.AddModelError(propiedad, "Valor inválido.");

        var result = await controller.Create(CreateViewModel("Título", "Autor"));

        Assert.IsType<ViewResult>(result);
        Assert.Empty(context.Libros);
    }

    [Theory]
    [InlineData(nameof(Libro.Titulo))]
    [InlineData(nameof(Libro.Autor))]
    [InlineData(nameof(Libro.Categoria))]
    [InlineData(nameof(Libro.Precio))]
    public async Task EditPost_ConCualquierCampoInvalido_NoActualizaElLibro(string propiedad)
    {
        var databaseName = Guid.NewGuid().ToString();
        int libroId;
        await using (var seedContext = CreateContext(databaseName))
        {
            var libro = CreateLibro("Título original", "Autor");
            seedContext.Libros.Add(libro);
            await seedContext.SaveChangesAsync();
            libroId = libro.ID;
        }

        await using var context = CreateContext(databaseName);
        var controller = CreateController(context);
        controller.ModelState.AddModelError(propiedad, "Valor inválido.");
        var libroEditado = CreateLibro("Título modificado", "Autor");
        libroEditado.ID = libroId;

        var result = await controller.Edit(libroEditado);

        Assert.IsType<ViewResult>(result);
        Assert.Equal("Título original", (await context.Libros.AsNoTracking().SingleAsync()).Titulo);
    }

    [Fact]
    public async Task Index_RegistraLaConsultaDelListado()
    {
        await using var context = CreateContext();
        var logger = new Mock<ILogger<LibrosController>>();
        var controller = CreateController(context, logger);

        await controller.Index();

        VerifyLog(logger, LogLevel.Information, Times.Once());
        VerifyLog(logger, LogLevel.Error, Times.Never());
    }

    [Fact]
    public async Task Index_ConContextoNoDisponible_RegistraElErrorYLoRelanza()
    {
        var context = CreateContext();
        var logger = new Mock<ILogger<LibrosController>>();
        var controller = CreateController(context, logger);
        await context.DisposeAsync();

        await Assert.ThrowsAsync<ObjectDisposedException>(() => controller.Index());

        VerifyLog(logger, LogLevel.Error, Times.Once());
    }

    [Fact]
    public async Task Create_ConImagen_GuardaLaRutaConLaExtensionOriginal()
    {
        var webRootPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        try
        {
            await using var context = CreateContext();
            var controller = CreateController(context, webRootPath: webRootPath);
            var imagen = new Mock<IFormFile>();
            imagen.Setup(f => f.FileName).Returns("portada.png");
            imagen.Setup(f => f.Length).Returns(10);
            imagen.Setup(f => f.CopyToAsync(It.IsAny<Stream>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
            var libro = CreateViewModel("Libro con portada", "Autor");
            libro.ImageFile = imagen.Object;

            await controller.Create(libro);

            var guardado = await context.Libros.SingleAsync();
            Assert.NotNull(guardado.ImageUrl);
            Assert.StartsWith("/Images/", guardado.ImageUrl);
            Assert.EndsWith(".png", guardado.ImageUrl);
            imagen.Verify(f => f.CopyToAsync(It.IsAny<Stream>(), It.IsAny<CancellationToken>()), Times.Once);
        }
        finally
        {
            if (Directory.Exists(webRootPath))
            {
                Directory.Delete(webRootPath, recursive: true);
            }
        }
    }

    [Fact]
    public async Task Acciones_NoUtilizanElRepositorioDeLibros()
    {
        await using var context = CreateContext();
        var repositorio = new Mock<IRepositorioLibro>();
        repositorio.Setup(r => r.ObtenerTodos()).Returns(new List<Libro> { CreateLibro("Libro del repositorio", "Autor") });
        var controller = CreateController(context, repositorio: repositorio);

        var result = await controller.Index();

        var view = Assert.IsType<ViewResult>(result);
        var libros = Assert.IsAssignableFrom<IEnumerable<Libro>>(view.Model);
        Assert.Empty(libros);
        repositorio.Verify(r => r.ObtenerTodos(), Times.Never);
    }

    private static Libro CreateLibro(string titulo, string autor)
    {
        return new Libro { Titulo = titulo, Autor = autor, Categoria = "Novela", Precio = 20m };
    }

    private static CreateLibroViewModel CreateViewModel(string titulo, string autor)
    {
        return new CreateLibroViewModel { Titulo = titulo, Autor = autor, Categoria = "Novela", Precio = 25.5m };
    }

    private static void VerifyLog(Mock<ILogger<LibrosController>> logger, LogLevel level, Times times)
    {
        logger.Verify(
            l => l.Log(
                level,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception?>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            times);
    }

    private static LibrosController CreateController(
        BibliotecaContext context,
        Mock<ILogger<LibrosController>>? logger = null,
        Mock<IRepositorioLibro>? repositorio = null,
        string? webRootPath = null)
    {
        var environment = new Mock<IWebHostEnvironment>();
        environment.Setup(e => e.WebRootPath).Returns(webRootPath ?? Path.GetTempPath());
        logger ??= new Mock<ILogger<LibrosController>>();
        repositorio ??= new Mock<IRepositorioLibro>();
        return new LibrosController(environment.Object, repositorio.Object, context, logger.Object);
    }

    private static BibliotecaContext CreateContext(string? databaseName = null)
    {
        var options = new DbContextOptionsBuilder<BibliotecaContext>()
            .UseInMemoryDatabase(databaseName ?? Guid.NewGuid().ToString())
            .Options;

        return new BibliotecaContext(options);
    }
}
