
namespace PolicyAdmin.API.Repository
{
    public interface IExcelRepository
    {
        Task<byte[]> ExportPoliciesToExcelAsync();
        Task<int> ImportPoliciesFromExcelAsync(Stream excelStream);
        
    }
}
