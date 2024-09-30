using System;
using System.IO;
using iText.IO.Font;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using Microsoft.Extensions.Logging;

namespace TrainBot.Services;
        
public class Pdf
{
    public static readonly string DEST = "TrainBot.pdf";
    private readonly string Font = @"./fonts/static/OpenSans-Light.ttf";
    public static void Main()
    {
        FileInfo file = new FileInfo(DEST);
        file.Directory.Create();
        new Pdf().ManipulatePdf(DEST);
    }

    public void ManipulatePdf(string dest)
    {
        try
        {
            PdfFont font = PdfFontFactory.CreateFont(Font, PdfEncodings.IDENTITY_H);
            
            PdfDocument pdfDoc = new PdfDocument(new PdfWriter(dest));
            Document doc = new Document(pdfDoc);
            doc.Add(new Paragraph("Тест на русском языке").SetFont(font));

            Table table = new Table(UnitValue.CreatePercentArray(new float[] { 100, 100 , 100, 100 }));
            table.SetMarginTop(10);

            // Добавляем заголовки столбцов таблицы с поддержкой кириллицы
            table.AddCell(new Cell().Add(new Paragraph("Упражнение").SetFont(font)));
            table.AddCell(new Cell().Add(new Paragraph("Вес").SetFont(font)));
            table.AddCell(new Cell().Add(new Paragraph("Повторения").SetFont(font)));
            table.AddCell(new Cell().Add(new Paragraph("Дата").SetFont(font)));
        
            // Пример данных
            table.AddCell(new Cell().Add(new Paragraph("Приседания").SetFont(font)));
            table.AddCell(new Cell().Add(new Paragraph("80 кг").SetFont(font)));
            table.AddCell(new Cell().Add(new Paragraph("12").SetFont(font)));
            table.AddCell(new Cell().Add(new Paragraph("30.09.2024").SetFont(font)));

            doc.Add(table);
            doc.Close();
        }
        catch (Exception e)
        {
            Console.WriteLine("ошибка:"+ e.Message);
        }
    }
}