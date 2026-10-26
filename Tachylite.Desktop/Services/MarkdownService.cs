using System;
using Tachylite.Tests.Core.Services;
using Markdig;
using Markdig.Syntax;
using System.IO.Pipelines;

namespace Tachylite.Desktop.Services;

public class MarkdownService : IMarkdownService
{
    public static readonly MarkdownPipeline Pipeline = new MarkdownPipelineBuilder()
            .UseEmphasisExtras()
            .UseTaskLists()
            .UsePipeTables()
            .Build();

    public string GetMarkdownHtml(string markdown)
    {
        if (string.IsNullOrWhiteSpace(markdown)) throw new ArgumentException("String cannot be empty or null");

        return Markdown.ToHtml(markdown, Pipeline);
    }
}
