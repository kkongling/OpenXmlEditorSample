# OpenXmlEditorSample

此示例项目展示了如何使用 `OpenXmlGenerator` 在构建时自动生成 OpenXML 的强类型 API，并在运行时使用这些类型来创建和修改 Word 文档。

## 使用步骤

1. 放置生成器 DLL：将 `DocumentFormat.OpenXml.Generator.dll` 复制到项目根目录下的 `libs` 文件夹。
2. 恢复依赖：`dotnet restore`
3. 构建项目：`dotnet build`
   - 编译过程中会在 `obj/GeneratedFiles` 目录看到生成器输出的 `.g.cs` 文件。
4. 运行：`dotnet run`

生成的 `Sample.docx` 会在项目根目录下，并包含示例内容。
