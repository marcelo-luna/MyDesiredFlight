using System.Threading.Tasks;

namespace MyDesiredFlight.Interfaces
{
    public interface ISearchFly
    {
        public Task<decimal> SearchFly(string from, string to, string startDate, string endDate, string[] options = null);
    }
}
