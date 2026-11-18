using System;
using Tachylite.Tests.Core.Services;
using Markdig;
using Markdig.Syntax;
using System.IO.Pipelines;

namespace Tachylite.Desktop.Services;

public class MarkdownService : IMarkdownService
{
    private const string STYLE = """
    <style>
        html {
            color : white;
            background : black;
            font-family: Verdana, sans-serif;
        }
    </style>
    """;

    public static readonly MarkdownPipeline Pipeline = new MarkdownPipelineBuilder()
            .UseEmphasisExtras()
            .UseTaskLists()
            .UsePipeTables()
            .Build();

    private string AddStyle(string markdown)
    {
        return STYLE + markdown;
    }

    public string GetMarkdownHtml(string markdown)
    {
        return AddStyle(Markdown.ToHtml(markdown, Pipeline));
    }
}
