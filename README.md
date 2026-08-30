[![](https://img.shields.io/nuget/v/soenneker.extensions.enumerable.document.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.extensions.enumerable.document/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.extensions.enumerable.document/publish-package.yml?style=for-the-badge)](https://github.com/soenneker/soenneker.extensions.enumerable.document/actions/workflows/publish-package.yml)
[![](https://img.shields.io/nuget/dt/soenneker.extensions.enumerable.document.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.extensions.enumerable.document/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.extensions.enumerable.document/codeql.yml?label=CodeQL&style=for-the-badge)](https://github.com/soenneker/soenneker.extensions.enumerable.document/actions/workflows/codeql.yml)

# ![](https://user-images.githubusercontent.com/4441470/224455560-91ed3ee7-f510-4041-a8d2-3fc093025112.png) Soenneker.Extensions.Enumerable.Document

ID projection and lookup extensions for sequences of `IDocument` values.

## Installation

```bash
dotnet add package Soenneker.Extensions.Enumerable.Document
```

## Project IDs

```csharp
using Soenneker.Documents.Document.Abstract;
using Soenneker.Extensions.Enumerable.Document;

IEnumerable<IDocument> documents = GetDocuments();
List<string> ids = documents.ToIds();
```

`ToIds()` creates a new list in source order. Duplicate IDs are preserved, documents are not cloned or modified, and a null source returns an empty list. The source is enumerated once; known counts are used only to pre-size the result.

## Find an ID

```csharp
bool contains = documents.ContainsId("document-42");
```

`ContainsId()` stops at the first match and returns `false` for a null source. Comparison uses the string `==` operator, which is ordinal and case-sensitive for non-null strings. The sequence must not contain null document entries; accessing a null element fails.
