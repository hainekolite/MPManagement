using MPManagement.ModelHelpers;
using SPManagement.Business;
using SPManagement.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MPManagement.ViewModels
{
    public class SolderingPasteBinnacleVM : ViewModelBase
    {
        #region Properties
        public ObservableCollection<BitacoraDeMovimientos> bitacora { get; set; }

        private readonly BitacoraDeMovimientosBusiness bitacoraDeMovimientosBusiness;
        private readonly RefrigeradorBusiness refrigeradorBusiness;
        private ICollection<Refrigerador> refrigeradores;
        private ICollection<Empleado> empleados;

        #endregion Properties

        #region Constructor
        public SolderingPasteBinnacleVM()
        {
            refrigeradorBusiness = new RefrigeradorBusiness();
            refrigeradores = refrigeradorBusiness.GetAllByIQueryable().ToList();
            empleados = new Collection<Empleado>();
            GetAllEmployeesBySQLRawQuery();
            bitacoraDeMovimientosBusiness = new BitacoraDeMovimientosBusiness();
            bitacora = new ObservableCollection<BitacoraDeMovimientos>(bitacoraDeMovimientosBusiness.GetAllBitacorasByIQueryable().ToList());
            GetTrueValuesForBinnacle();

        }
        #endregion Constructor

        #region SQLRawQueries
        private void GetAllEmployeesBySQLRawQuery()
        {
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["SPManagementDbContext"].ConnectionString;
            string query = "SELECT EmployeeNumber, Name, LastName FROM Users";

            SqlConnection connection = new SqlConnection(connectionString);
            SqlCommand command = new SqlCommand(query, connection);
            connection.Open();
            SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                empleados.Add(new Empleado()
                {
                    NumeroDeEmpleado = (reader["EmployeeNumber"].ToString().ToUpper()),
                    NombreEmpleado = (reader["Name"].ToString().ToUpper()),
                    ApellidoEmpleado = (reader["LastName"].ToString().ToUpper())
                });
            }
            connection.Close();
        }
        #endregion SQLRawQueries

        #region TrueValues
        private void GetTrueValuesForBinnacle()
        {
            foreach (var binnacle in bitacora)
            {
                if (binnacle.RefrigeradorId.Equals(0))
                    binnacle.NombreRefrigerador = "PCS";
                else
                {
                    var query = refrigeradores.Where(r => r.Id == binnacle.RefrigeradorId).FirstOrDefault();
                    if (query != null)
                        binnacle.NombreRefrigerador = query.NumeroDeRefrigerador;
                    else
                        binnacle.NombreRefrigerador = "PCS";
                }
                if (string.IsNullOrEmpty(binnacle.UserId))
                    binnacle.NombreDeEmpleado = "PCS";
                else
                {
                    var query = empleados.Where(e => e.NumeroDeEmpleado == binnacle.UserId).FirstOrDefault();
                    if (query != null)
                        binnacle.NombreDeEmpleado = string.Format("{0} {1}", query.NombreEmpleado, query.ApellidoEmpleado);
                    else
                        binnacle.NombreDeEmpleado = "PCS";
                }
            }
        }

        #endregion TrueValues
    }
}
