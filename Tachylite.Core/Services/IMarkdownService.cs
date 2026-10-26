using System;

namespace Tachylite.Tests.Core.Services;

public interface IMarkdownService
{
    public string GetMarkdownHtml(string markdown);
}
