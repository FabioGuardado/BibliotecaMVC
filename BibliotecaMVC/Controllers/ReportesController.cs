using BibliotecaMVC.Data;
using BibliotecaMVC.Models;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace BibliotecaMVC.Controllers
{
    [Authorize]
    public class ReportesController : Controller
    {
        private readonly BibliotecaContext _context;
        private readonly ILogger<ReportesController> _logger;

        public ReportesController(BibliotecaContext context, ILogger<ReportesController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [AuthorizeRoles(Roles.Admin, Roles.Bibliotecario)]
        public async Task<IActionResult> VerLibrosPdf()
        {
            _logger.LogInformation("Visualizando el reporte de libros en PDF");

            var libros = await _context.Libros.AsNoTracking().OrderBy(libro => libro.Titulo).ToListAsync();

            return File(GenerarReportePdf(libros), "application/pdf");
        }

        [AuthorizeRoles(Roles.Admin, Roles.Bibliotecario)]
        public async Task<IActionResult> LibrosPdf()
        {
            _logger.LogInformation("Generando el reporte de libros en PDF");

            var libros = await _context.Libros.AsNoTracking().OrderBy(libro => libro.Titulo).ToListAsync();

            return File(GenerarReportePdf(libros), "application/pdf", "ReporteLibros.pdf");
        }

        [AuthorizeRoles(Roles.Admin, Roles.Bibliotecario)]
        public async Task<IActionResult> LibrosExcel()
        {
            _logger.LogInformation("Generando el reporte de libros en Excel");

            var libros = await _context.Libros.AsNoTracking().OrderBy(libro => libro.Titulo).ToListAsync();

            return File(
                GenerarReporteExcel(libros),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "ReporteLibros.xlsx");
        }

        private static byte[] GenerarReportePdf(IReadOnlyCollection<Libro> libros)
        {
            QuestPDF.Settings.License = LicenseType.Community;

            return Document.Create(document => document.Page(page =>
            {
                page.Size(PageSizes.Letter);
                page.Margin(42);
                page.PageColor(Colors.Grey.Lighten5);

                page.Header().Column(column =>
                {
                    column.Item().Text("BIBLIOTECA UNIVERSITARIA").FontSize(11).Bold().FontColor(Colors.Blue.Darken2);
                    column.Item().Text("Reporte de libros")
                    .FontSize(26)
                    .Bold()
                    .FontColor(Colors.Grey.Darken3);
                    column.Item().Text($"Generado el {DateTime.Now:dd/MM/yyyy HH:mm}")
                    .FontSize(10)
                    .FontColor(Colors.Grey.Darken1);
                    column.Item().PaddingTop(14)
                    .LineHorizontal(2)
                    .LineColor(Colors.Blue.Medium);
                });

                page.Content().PaddingVertical(22).Column(column =>
                {
                    column.Item().Background(Colors.Blue.Darken2).Padding(14).Row(row =>
                    {
                        row.RelativeItem().Text("Libros Registrados")
                        .FontSize(11)
                        .Bold()
                        .FontColor(Colors.White);

                        row.ConstantItem(45).AlignRight().Text(libros.Count.ToString()).FontSize(18).Bold().FontColor(Colors.White);
                    });

                    column.Item().PaddingTop(18).Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.ConstantColumn(42);
                            columns.RelativeColumn(2.5f);
                            columns.RelativeColumn(2f);
                            columns.RelativeColumn(1.5f);
                        });

                        table.Header(header =>
                        {
                            header.Cell().Element(HeaderCellStyle).Text("ID");
                            header.Cell().Element(HeaderCellStyle).Text("LIBRO");
                            header.Cell().Element(HeaderCellStyle).Text("AUTOR");
                            header.Cell().Element(HeaderCellStyle).Text("CATEGORÍA");
                        });

                        foreach (var libro in libros)
                        {
                            table.Cell().Element(BodyCellStyle).Text(libro.ID.ToString());
                            table.Cell().Element(BodyCellStyle).Text(libro.Titulo);
                            table.Cell().Element(BodyCellStyle).Text(libro.Autor);
                            table.Cell().Element(BodyCellStyle).Text(libro.Categoria);
                        }
                    });
                });

                page.Footer().AlignCenter().Text(text =>
                {
                    text.Span("Biblioteca Universitaria - Página ");
                    text.CurrentPageNumber();
                    text.Span(" de ");
                    text.TotalPages();
                });
            })).GeneratePdf();

            static IContainer HeaderCellStyle(IContainer container) => container.Background(Colors.Blue.Darken2).PaddingVertical(9).PaddingHorizontal(8).DefaultTextStyle(text => text.FontSize(9).Bold().FontColor(Colors.White));
            static IContainer BodyCellStyle(IContainer container) => container.Background(Colors.White).BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(9).PaddingHorizontal(8).DefaultTextStyle(text => text.FontSize(9).FontColor(Colors.Grey.Darken3));
        }

        private static byte[] GenerarReporteExcel(IReadOnlyCollection<Libro> libros)
        {
            using var workbook = new XLWorkbook();
            var sheet = workbook.Worksheets.Add("Reporte de Libros");

            sheet.Cell(1, 1).Value = "Reporte de libros";
            sheet.Cell(1, 1).Style.Font.Bold = true;
            sheet.Cell(1, 1).Style.Font.FontSize = 16;
            sheet.Cell(2, 1).Value = $"Generado el {DateTime.Now:dd/MM/yyyy HH:mm}";

            const int headerRow = 4;
            string[] headers = ["ID", "Libro", "Autor", "Categoría"];
            for (var i = 0; i < headers.Length; i++)
            {
                sheet.Cell(headerRow, i + 1).Value = headers[i];
            }

            var headerRange = sheet.Range(headerRow, 1, headerRow, headers.Length);
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Font.FontColor = XLColor.White;
            headerRange.Style.Fill.BackgroundColor = XLColor.FromHtml("#1565C0");
            headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            var row = headerRow + 1;
            foreach (var libro in libros)
            {
                sheet.Cell(row, 1).Value = libro.ID;
                sheet.Cell(row, 2).Value = libro.Titulo;
                sheet.Cell(row, 3).Value = libro.Autor;
                sheet.Cell(row, 4).Value = libro.Categoria;
                row++;
            }

            var dataRange = sheet.Range(headerRow, 1, row - 1, headers.Length);
            dataRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            dataRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
            sheet.Range(headerRow + 1, 1, row - 1, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            sheet.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }
    }
}
