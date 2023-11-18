using AngleSharp.Dom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace extractor.network.hemlfetcher
{
    internal interface IHTMLFetcher : IDisposable
    {
        Task<string> GetPageHtml(Uri url);
    }
}
