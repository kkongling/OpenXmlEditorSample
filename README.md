# OpenXmlEditorSample

此示例项目展示了如何使用 `OpenXmlGenerator` 在构建时自动生成 OpenXML 的强类型 API，并在运行时使用这些类型来创建和修改 Word 文档。

## 使用步骤

1. 确保已包含 `OpenXmlGenerator` 源代码于项目同级目录中。
2. 放置生成器项目：确保 `OpenXmlGenerator` 文件夹与本项目同级，并已包含 `DocumentFormat.OpenXml.Generator` 和 `DocumentFormat.OpenXml.Generator.Models` 子项目。
3. 恢复依赖并构建生成器：

   ```bash
   cd OpenXmlGenerator
   dotnet build
   ```

4. 切换回示例项目根目录，恢复依赖并构建示例项目：

   ```bash
   cd ../OpenXmlEditorSample
   dotnet restore
   dotnet build
   ```

   - 成功编译后，会在 `obj/Debug/net6.0/GeneratedFiles` 或 `obj/Debug/net6.0/generated` 目录看到所有 `.g.cs` 文件。

5. 运行示例：

   ```bash
   dotnet run --project OpenXmlEditorSample.csproj
   ```

   > 生成的 `Sample.docx` 会在项目根目录下，并包含示例内容。

6. **排查仅生成 Attributes.g.cs 的常见原因**
   - **缺少 AdditionalFiles**：确认 `DocumentFormat.OpenXml.Generator` 项目里已将 `.xsd` 和 `.json` 模式文件包含为 `<AdditionalFiles>`，或手动在示例项目中引入：

     ```xml
     <ItemGroup>
       <AdditionalFiles Include="..\OpenXmlGenerator\data\schemas\**\*.json" />
     </ItemGroup>
     ```

   - **检查输出路径**：未指定输出路径时，默认目录是 `obj/Debug/net6.0/generated/`。
   - **配置项**：确认 `.editorconfig` 中的 `build_property.OpenXmlGenerator.*` 均为 `true`（或按需配置）。
   - **生成器加载**：查看编译日志（详细模式），确认 `Initializing OpenXmlGenerator` 无报错。

7. \.g.cs 文件生成依据

   .g.cs 文件并非由运行时已存在的 .docx 文件内容生成，而是在编译阶段由 OpenXmlGenerator 源生成器根据以下输入数据构建：

   1. OpenXML 架构定义（Schemas）
      - Generator 项目中包含的 .xsd 和 .json 文件，通过 `<AdditionalFiles>` 项目项声明，作为编译时输入被 Roslyn 提供给生成器。
      - 这些文件定义了所有 OpenXML Part、Element、Attribute 等元数据。

   2. 编译器配置（AnalyzerConfigOptions）
      - 通过 .editorconfig 中的 build_property.OpenXmlGenerator.* 属性控制生成哪些模块（Parts、Namespaces、Schema、LINQ）。

   3. 生成器内部服务（OpenXmlServices）
      - 读取上述 Schema 数据后，OpenXmlServices 会解析并构建模型（Parts 列表、QName 映射、属性集等），并将其提供给各个代码生成模块。

   4. 增量生成管道
      - 源生成器注册了多个 RegisterSourceOutput 回调，根据模型数据生成对应的 .g.cs 源代码文件，如：
         - Parts.g.cs、Elements.g.cs、Namespaces.g.cs 等。
         - Attributes.g.cs 仅包含属性定义文件，如果其他模块未启用或模型未加载，则可能只生成该文件。

   通过以上流程，OpenXmlGenerator 能够在没有任何现成 OpenXML 文档的情况下，基于架构定义生成完整的强类型 API 源码。

8. 关于使用 .docx 文件生成代码

   OpenXmlGenerator 无法直接读取或解析 .docx 文档来生成 .g.cs 文件，因为它的设计初衷是基于架构（XSD/JSON）元数据进行源生成，而非运行时文档示例。以下几点说明：

   1. .docx 文件结构：.docx 本质上是一个 ZIP 包，包含若干 XML 文件（document.xml、styles.xml 等）和资源。源生成器并不会展开或分析这些文档实例。
   2. 架构 vs 实例：架构定义（XSD/JSON）描述所有可能的元素、属性和其约束，而文档实例只能反映部分用例，无法覆盖全部模式。若使用实例，会导致生成不完整或偏差。
   3. 自定义场景：如果你希望根据某个具体 .docx 文件来生成特定强类型模型，可以自行编写脚本：
      - 使用 System.IO.Packaging.Package.Open 解压 .docx，读取所有 XML 文件。
      - 编写自定义 Roslyn 生成器或工具，将实例中出现的元素类型提取并映射到已有架构类型，生成辅助代码。
   4. 推荐方案：继续使用官方架构文件生成全量强类型 API，通过代码使用后操作具体文档实例；这样既保证完整性，又能兼顾灵活性。

   小结：你无法直接提供 .docx 给 OpenXmlGenerator 来生成代码，它只接受架构定义作为输入。
