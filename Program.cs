using System;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

class Program
{
    static void Main(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("请提供 .docx 文件路径作为参数。");
            return;
        }
        string filepath = args[0];
        if (!System.IO.File.Exists(filepath))
        {
            Console.WriteLine($"文件不存在: {filepath}");
            return;
        }
        string newFilePath = System.IO.Path.Combine(
            System.IO.Path.GetDirectoryName(filepath) ?? string.Empty,
            System.IO.Path.GetFileNameWithoutExtension(filepath) + ".gen.docx"
        );
        System.IO.File.Copy(filepath, newFilePath, true);
        using (var doc = WordprocessingDocument.Open(newFilePath, true))
        {
            var body = doc.MainDocumentPart.Document.Body;
            body.Append(new Paragraph(new Run(new Text("Added by generated API."))));
            doc.MainDocumentPart.Document.Save();
        }
        Console.WriteLine($"文档已修改并保存为: {newFilePath}");
    }
}
