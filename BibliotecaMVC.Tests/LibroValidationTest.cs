using System.ComponentModel.DataAnnotations;
using BibliotecaMVC.Models;

namespace BibliotecaMVC.Tests;

public class LibroValidationTests
{
    [Theory]
    [InlineData(nameof(Libro.Titulo))]
    [InlineData(nameof(Libro.Autor))]
    [InlineData(nameof(Libro.Categoria))]
    public void CamposObligatoriosNoPuedenEstarVacios(string propiedad)
    {
        var libro = CreateLibroValido();
        typeof(Libro).GetProperty(propiedad)!.SetValue(libro, null);

        var errores = Validate(libro);

        Assert.Contains(errores, error => error.MemberNames.Contains(propiedad));
    }

    [Fact]
    public void LibroCompletoEsValido()
    {
        var libro = CreateLibroValido();

        var errores = Validate(libro);

        Assert.Empty(errores);
    }

    [Fact]
    public void LibroNuevoEstaDisponibleYSinImagenPorDefecto()
    {
        var libro = new Libro();

        Assert.True(libro.Disponible);
        Assert.Null(libro.ImageUrl);
    }

    [Theory]
    [InlineData(nameof(CreateLibroViewModel.Titulo))]
    [InlineData(nameof(CreateLibroViewModel.Autor))]
    [InlineData(nameof(CreateLibroViewModel.Categoria))]
    public void ViewModelCamposObligatoriosNoPuedenEstarVacios(string propiedad)
    {
        var libro = new CreateLibroViewModel { Titulo = "Título", Autor = "Autor", Categoria = "Novela" };
        typeof(CreateLibroViewModel).GetProperty(propiedad)!.SetValue(libro, null);

        var errores = Validate(libro);

        Assert.Contains(errores, error => error.MemberNames.Contains(propiedad));
    }

    [Fact]
    public void ViewModelNuevoNoDefineDisponibilidadPorDefecto()
    {
        var libro = new CreateLibroViewModel();

        Assert.Null(libro.Disponible);
    }

    private static Libro CreateLibroValido()
    {
        return new Libro { Titulo = "Título", Autor = "Autor", Categoria = "Novela", Precio = 10m };
    }

    private static IReadOnlyCollection<ValidationResult> Validate(object modelo)
    {
        var context = new ValidationContext(modelo);
        var errores = new List<ValidationResult>();
        Validator.TryValidateObject(modelo, context, errores, validateAllProperties: true);
        return errores;
    }
}
