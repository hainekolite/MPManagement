using MPManagement.ModelHelpers;
using MPManagement.ViewModels.Commands;
using SPManagement.Business;
using SPManagement.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace MPManagement.ViewModels
{
    public class SolderingPasteBinnacleVM : ViewModelBase
    {
        #region Properties

        private readonly ParameterCommand _filterByDateCommand;
        public ParameterCommand FilterByDateCommand => _filterByDateCommand;

        private readonly RelayCommand _showAllCommand;
        public RelayCommand ShowAllCommand => _showAllCommand;

        private readonly RelayCommand _exportToExcelCommand;
        public RelayCommand ExportToExcelCommand => _exportToExcelCommand;

        public ObservableCollection<BitacoraDeMovimientos> bitacora { get; set; }

        private readonly BitacoraDeMovimientosBusiness bitacoraDeMovimientosBusiness;
        private readonly RefrigeradorBusiness refrigeradorBusiness;
        private ICollection<Refrigerador> refrigeradores;
        private ICollection<Empleado> empleados;

        private DateTime _initialDate;
        public DateTime InitialDate
        {
            get
            {
                return (_initialDate);
            }
            set
            {
                _initialDate = value;
                OnPropertyChanged();
            }
        }

        private DateTime _finishDate;
        public DateTime FinishDate
        {
            get
            {
                return (_finishDate);
            }
            set
            {
                _finishDate = value;
                OnPropertyChanged();
            }
        }



        #endregion Properties

        #region Constructor
        public SolderingPasteBinnacleVM()
        {
            _initialDate = DateTime.Now;
            _finishDate = DateTime.Now;
            empleados = new Collection<Empleado>();
            refrigeradorBusiness = new RefrigeradorBusiness();
            bitacoraDeMovimientosBusiness = new BitacoraDeMovimientosBusiness();
            refrigeradores = refrigeradorBusiness.GetAllByIQueryable().ToList();
            GetAllEmployeesBySQLRawQuery();
            bitacora = new ObservableCollection<BitacoraDeMovimientos>(bitacoraDeMovimientosBusiness.GetAllBitacorasByIQueryableOrderByDescending().ToList());
            GetTrueValuesForBinnacle();
            _filterByDateCommand = new ParameterCommand(FilterRevisionsByDate);
            _showAllCommand = new RelayCommand(ShowAllBinnacles);
            _exportToExcelCommand = new RelayCommand(ExportToExcel);

        }
        #endregion Constructor

        #region DateFiler

        private void FilterRevisionsByDate(object datePickers)
        {
            var values = datePickers as object[];
            DatePicker firstDate = values[0] as DatePicker;
            DatePicker lastDate = values[1] as DatePicker;

            if (firstDate.SelectedDate >= lastDate.SelectedDate)
                MessageBox.Show("La fecha inicial no puede ser mayor a la fecha final, de igual forma no pueden ser iguales", "ERROR");
            else
            {
                if (bitacora != null)
                {
                    _initialDate = ((DateTime)firstDate.SelectedDate);
                    _finishDate = ((DateTime)lastDate.SelectedDate);
                    bitacora = new ObservableCollection<BitacoraDeMovimientos>(bitacoraDeMovimientosBusiness.GetAllBitacorasInDateRange(_initialDate, _finishDate).ToList());
                    GetTrueValuesForBinnacle();
                    OnPropertyChanged("bitacora");
                }
                else
                    MessageBox.Show("No existen datos que filtrar, debe de existir almenos un elemento en la bitacora para realizar el filtrado", "AVISO");
            }
        }

        #endregion DateFiler

        #region ShowAll
        private void ShowAllBinnacles()
        {
            bitacora = new ObservableCollection<BitacoraDeMovimientos>(bitacoraDeMovimientosBusiness.GetAllBitacorasByIQueryableOrderByDescending().ToList());
            GetTrueValuesForBinnacle();
            OnPropertyChanged("bitacora");
        }
        #endregion ShowAll

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

        #region ExcelRelated

        private void ExportToExcel()
        {
            Microsoft.Office.Interop.Excel.Application excel = new Microsoft.Office.Interop.Excel.Application();
            // Create empty workbook
            excel.Workbooks.Add();

            // Create Worksheet from active sheet
            Microsoft.Office.Interop.Excel._Worksheet workSheet = excel.ActiveSheet;

            try
            {
                // ------------------------------------------------
                // Creation of header cells
                // ------------------------------------------------

                workSheet.Cells[1, "A"] = "Reporte de movimientos 'Pasta de Soldar'";
                workSheet.Cells[1, "B"] = string.Format("A: {0}",DateTime.Now.ToString());

                workSheet.Cells[3, "A"] = "Empleado";
                workSheet.Cells[3, "B"] = "Refrigerador";
                workSheet.Cells[3, "C"] = "Cartucho";
                workSheet.Cells[3, "D"] = "Movimiento";
                workSheet.Cells[3, "E"] = "Fecha de Movimiento";
                // ------------------------------------------------
                // Populate sheet with some real data from "cars" list
                // ------------------------------------------------

                int row = 4; // start row (in row 1 are header cells)
                for (int i=0; i< bitacora.Count; i++)
                {
                    workSheet.Cells[row, "A"] = bitacora.ElementAt(i).NombreDeEmpleado;
                    workSheet.Cells[row, "B"] = bitacora.ElementAt(i).NombreRefrigerador;
                    workSheet.Cells[row, "C"] = bitacora.ElementAt(i).NumeroDeCartucho;
                    workSheet.Cells[row, "D"] = bitacora.ElementAt(i).NombreRefrigerador;
                    workSheet.Cells[row, "E"] = bitacora.ElementAt(i).FechaMovimiento.ToString();
                    row++;
                }
                // Apply some predefined styles for data to look nicely :)
                //workSheet.Range["A1"].AutoFormat(Microsoft.Office.Interop.Excel.XlRangeAutoFormat.xlRangeAutoFormatClassic1);

                // Define filename
                //string fileName = string.Format(@"{0}\ExcelData.xlsx", Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory));

                // Save this data as a file
                //workSheet.SaveAs(fileName);

                excel.Visible = true;

                // Display SUCCESS message
                //MessageBox.Show(string.Format("The file '{0}' is saved successfully!", fileName));
            }
            catch (Exception exception)
            {
                MessageBox.Show("Exception");
            }
            finally
            {
                // Quit Excel application
                /*excel.Quit();

                // Release COM objects (very important!)
                if (excel != null)
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(excel);

                if (workSheet != null)
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(workSheet);

                // Empty variables
                excel = null;
                workSheet = null;*/

                // Force garbage collector cleaning
                GC.Collect();
            }
        }

        #endregion ExcelRelated
    }
}
