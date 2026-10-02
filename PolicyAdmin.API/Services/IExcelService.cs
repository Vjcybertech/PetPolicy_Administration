using System.IO;
using System.Threading.Tasks;

namespace PolicyAdmin.API.Services
{
    public interface IExcelService
    {
        Task<byte[]> ExportPoliciesToExcelAsync();
        Task<int> ImportPoliciesFromExcelAsync(Stream excelStream);
    }
}
