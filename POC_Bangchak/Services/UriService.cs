using Microsoft.AspNetCore.WebUtilities;
using POC_Bangchak.Filter;

namespace POC_Bangchak.Services
{
    public class UriService : IUriService
    {
        private readonly string _baseUri;
        public UriService(string baseUri)
        {
            _baseUri = baseUri;
        }
        public Uri GetPageUri(PaginationFilter filter, string route)
        {
            Console.WriteLine("BaseURI");
            Console.WriteLine(_baseUri);
            Console.WriteLine("route");
            Console.WriteLine(route);
            var _enpointUri = new Uri(string.Concat(_baseUri, route));
            Console.WriteLine("URL");
            Console.WriteLine(_enpointUri);
            var modifiedUri = QueryHelpers.AddQueryString(_enpointUri.ToString(), "pageNumber", filter.pageNumber.ToString());
            modifiedUri = QueryHelpers.AddQueryString(modifiedUri, "pageSize", filter.pageSize.ToString());
            return new Uri(modifiedUri);
        }
    }
}
