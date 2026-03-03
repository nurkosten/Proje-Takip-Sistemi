namespace ProjeHavuzu.Data.DTOs.Common
{
    /// <summary>
    /// jQuery DataTables server-side processing request parametreleri.
    /// </summary>
    public class DataTableRequest
    {
        public int Draw { get; set; }
        public int Start { get; set; }
        public int Length { get; set; }
        public DataTableSearch Search { get; set; } = new();
        public List<DataTableOrder> Order { get; set; } = new();
        public List<DataTableColumn> Columns { get; set; } = new();
    }

    public class DataTableSearch
    {
        public string Value { get; set; } = "";
        public bool Regex { get; set; }
    }

    public class DataTableOrder
    {
        public int Column { get; set; }
        public string Dir { get; set; } = "asc";
    }

    public class DataTableColumn
    {
        public string Data { get; set; } = "";
        public string Name { get; set; } = "";
        public bool Searchable { get; set; }
        public bool Orderable { get; set; }
        public DataTableSearch Search { get; set; } = new();
    }

    /// <summary>
    /// jQuery DataTables server-side processing response.
    /// </summary>
    public class DataTableResponse<T>
    {
        public int Draw { get; set; }
        public int RecordsTotal { get; set; }
        public int RecordsFiltered { get; set; }
        public List<T> Data { get; set; } = new();
        public string? Error { get; set; }
    }
}
