using POC_Bangchak.Filter;

namespace POC_Bangchak.Services
{
    public interface IUriService
    {
        public Uri GetPageUri(PaginationFilter filter, string route);
    }
}
