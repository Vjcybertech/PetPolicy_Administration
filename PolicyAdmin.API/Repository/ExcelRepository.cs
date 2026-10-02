using PolicyAdmin.API.Services;


namespace PolicyAdmin.API.Repository
{
    public class ExcelRepository:IExcelRepository
    {
         private readonly IExcelService _service;

         public ExcelRepository(IExcelService service)
        {
            _service=service;
        }

        public Task<byte[]> ExportPoliciesToExcelAsync()
        {
            return _service.ExportPoliciesToExcelAsync();
        }

        public Task<int> ImportPoliciesFromExcelAsync(Stream excelStream)
        {
            return _service.ImportPoliciesFromExcelAsync(excelStream);
        }
    }
}