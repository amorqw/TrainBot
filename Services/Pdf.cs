using iText.IO.Font;
using iText.IO.Image;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Kernel.Pdf.Canvas;
using Microsoft.EntityFrameworkCore;
using TrainBot.Models;
using TrainBot.Data;

namespace TrainBot.Services
{
    public class Pdf
    {
        private readonly AppDbContext _dbContext;
        private readonly string Font = @"./fonts/static/OpenSans-Light.ttf";
        private readonly string Img = "https://www.assured.enterprises/wp-content/uploads/2015/02/gradient.png";

        public Pdf(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }



        public void ManipulatePdf(string dest, List<ExercisesTg> exercises)
        {
            string PdfDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Pdf");
            string FullPdfPath = Path.Combine(PdfDirectory, dest);
            PdfFont font = PdfFontFactory.CreateFont(Font, PdfEncodings.IDENTITY_H);
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(FullPdfPath));

            // Добавляем фоновое изображение
            AddImageToBackground(pdfDoc, Img);

            Document doc = new Document(pdfDoc);

            doc.Add(new Paragraph("Отчет по упражнениям").SetFont(font).SetTextAlignment(TextAlignment.CENTER));

            Table table = new Table(UnitValue.CreatePercentArray(new float[] { 1, 1, 1, 1 }))
                .SetWidth(UnitValue.CreatePercentValue(100))
                .SetMarginTop(10)
                .SetMarginLeft(20)
                .SetMarginRight(20);

            // Заголовки таблицы
            table.AddCell(new Cell().Add(new Paragraph("Упражнение").SetFont(font)));
            table.AddCell(new Cell().Add(new Paragraph("Вес").SetFont(font)));
            table.AddCell(new Cell().Add(new Paragraph("Повторения").SetFont(font)));
            table.AddCell(new Cell().Add(new Paragraph("Дата").SetFont(font)));

            // Добавление данных из списка упражнений
            foreach (var exercise in exercises)
            {
                table.AddCell(new Cell().Add(new Paragraph(exercise.Exercise).SetFont(font)));
                table.AddCell(new Cell().Add(new Paragraph(exercise.Weight.ToString()).SetFont(font)));
                table.AddCell(new Cell().Add(new Paragraph(exercise.Repetitions.ToString()).SetFont(font)));
                table.AddCell(new Cell().Add(new Paragraph(exercise.Date.Value.ToString("dd MM yyyy")).SetFont(font)));
            }


            doc.Add(table);
            doc.Close();
        }

        private void AddImageToBackground(PdfDocument pdfDoc, string imageUrl)
        {
            ImageData imgData = ImageDataFactory.Create(imageUrl);
            PdfCanvas canvas = new PdfCanvas(pdfDoc.AddNewPage());
            float pageWidth = pdfDoc.GetDefaultPageSize().GetWidth();
            float pageHeight = pdfDoc.GetDefaultPageSize().GetHeight();
            canvas.AddImageFittedIntoRectangle(imgData, new iText.Kernel.Geom.Rectangle(0, 0, pageWidth, pageHeight),
                true);
        }

    }
}