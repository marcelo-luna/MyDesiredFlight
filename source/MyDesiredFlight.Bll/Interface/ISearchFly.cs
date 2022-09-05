using System.Threading.Tasks;

namespace MyDesiredFlight.Bll.Interface
{
    public interface ISearchFly
    {
        public Task<decimal> SearchFly(string origin, string destination, string dateFrom, string dateTo, string[] options = null);
    }
}
