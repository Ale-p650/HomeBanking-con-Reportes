namespace ManejoPresupuestos.Models
{
    public class PaginacionViewModel
    {
        public int Pagina { get; set; } = 1;

        private readonly int _cantidadMaximaDePagina = 50;

        private int _recordsPorPagina = 10;

        public int RecordsPorPagina
        {
            get
            {
                return _recordsPorPagina;
            }
            set
            {
                _recordsPorPagina = 
                    (value > _cantidadMaximaDePagina) ? _cantidadMaximaDePagina : value;
            }
        }

        public int RecordsASaltar => _recordsPorPagina * (Pagina - 1);
    }
}
