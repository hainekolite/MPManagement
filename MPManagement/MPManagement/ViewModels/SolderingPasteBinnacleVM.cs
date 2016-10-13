using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
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
using System.Windows.Threading;

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
        private Task updateTask;
        private Object updateLock;


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
            updateLock = new object();

        }
        #endregion Constructor

        #region DateFilter

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

        #endregion DateFilter

        #region ShowAll
        private void ShowAllBinnacles()
        {
            if (updateTask == null || updateTask.IsCompleted)
            {
                updateTask = Task.Run(() =>
                {
                    lock (updateLock)
                    {
                        bitacora = new ObservableCollection<BitacoraDeMovimientos>(bitacoraDeMovimientosBusiness.GetAllBitacorasByIQueryableOrderByDescending().ToList());
                        GetTrueValuesForBinnacle();
                        OnPropertyChanged("bitacora");
                    }
                });
            }
            else
                MessageBox.Show("Favor de esperar a que se termine la carga de datos");

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
            string path = "C:\\Users\\Jaime\\Desktop\\hola.xlsx";
            using (SpreadsheetDocument spreadsheet = SpreadsheetDocument.Create(path, SpreadsheetDocumentType.Workbook))
            {
                WorkbookPart workbookPart = spreadsheet.AddWorkbookPart();
                workbookPart.Workbook = new Workbook();
                Workbook wb = new Workbook();
                FileVersion fv = new FileVersion();
                fv.ApplicationName = "Microsoft Office Excel";

                Worksheet workSheet = new Worksheet();
                WorksheetPart worksheetPart = workbookPart.AddNewPart<WorksheetPart>();

                SheetData sheetData = CreateSheetData();
                workSheet.Append(sheetData);
                worksheetPart.Worksheet = workSheet;
                worksheetPart.Worksheet.Save();

                Sheets sheets = new Sheets();
                Sheet sheet = new Sheet();
                sheet.Id = spreadsheet.WorkbookPart.GetIdOfPart(worksheetPart);
                sheet.SheetId = 1; //sheet Id, anything but unique
                sheet.Name = "Reporte"; //sheet name
                sheets.Append(sheet);

                wb.Append(sheets);
                spreadsheet.WorkbookPart.Workbook = wb;
                spreadsheet.WorkbookPart.Workbook.Save();
                spreadsheet.Clone();
            }
        }


        private SheetData CreateSheetData()
        {
            SheetData sheetData = new SheetData();
            //Generate Data 
            Row header = new Row();
            Row topHeader = new Row();
            int topHeaderIndex = 3;
            int rowIndexForHeader = 1;
            header.RowIndex = (uint)rowIndexForHeader;
            topHeader.RowIndex = (uint)topHeaderIndex;
            /*Header */
            Cell HeaderCellOne = new Cell()
            {
                CellReference = string.Format("A1"),
                DataType = CellValues.InlineString,
                InlineString = new InlineString() { Text = new Text("Reporte de administracion -  Pasta de soldar") },
            };
            Cell HeaderCellTwo = new Cell()
            {
                CellReference = string.Format("B1"),
                DataType = CellValues.InlineString,
                InlineString = new InlineString() { Text = new Text(string.Format("A: {0}",DateTime.Now.ToString()))},
            };

            /*Top Header*/
            Cell TopHeaderCellOne = new Cell()
            {
                CellReference = string.Format("A3"),
                DataType = CellValues.InlineString,
                InlineString = new InlineString() { Text = new Text("Empleado") },
            };
            Cell TopHeaderCellTwo = new Cell()
            {
                CellReference = string.Format("B3"),
                DataType = CellValues.InlineString,
                InlineString = new InlineString() { Text = new Text("Refrigerador") },
            };
            Cell TopHeaderCellThree = new Cell()
            {
                CellReference = string.Format("C3"),
                DataType = CellValues.InlineString,
                InlineString = new InlineString() { Text = new Text("Numero de cartucho") },
            };
            Cell TopHeaderCellFour = new Cell()
            {
                CellReference = string.Format("D3"),
                DataType = CellValues.InlineString,
                InlineString = new InlineString() { Text = new Text("Movimiento") },
            };
            Cell TopHeaderCellFive = new Cell()
            {
                CellReference = string.Format("E3"),
                DataType = CellValues.InlineString,
                InlineString = new InlineString() { Text = new Text("Fecha de movimiento") },
            };


            header.Append(HeaderCellOne);
            header.Append(HeaderCellTwo);
            topHeader.Append(TopHeaderCellOne);
            topHeader.Append(TopHeaderCellTwo);
            topHeader.Append(TopHeaderCellThree);
            topHeader.Append(TopHeaderCellFour);
            topHeader.Append(TopHeaderCellFive);
            sheetData.Append(header);
            sheetData.Append(topHeader);

            string movimiento = string.Empty;
            for (int i = 0; i < bitacora.Count; i++)
            {
                Row row = new Row();
                int rowIndex = i+4;
                row.RowIndex = (uint)rowIndex;  

                if (bitacora.ElementAt(i).TipoMovimiento == 0)
                    movimiento = "Entrada a refrigerador";
                else if (bitacora.ElementAt(i).TipoMovimiento == 1)
                    movimiento = "Salida a ambientacion";
                else if (bitacora.ElementAt(i).TipoMovimiento == 2)
                    movimiento = "insertada a maquina DEK";
                else if (bitacora.ElementAt(i).TipoMovimiento == 3)
                    movimiento = "Retorno a refrigerador";
                else if (bitacora.ElementAt(i).TipoMovimiento == 4)
                    movimiento = "Cartucho terminado";
                else
                    movimiento = "Movimiento no catalogado";

                Cell Acell = new Cell()
                {
                    CellReference = string.Format("A{0}", row.RowIndex.ToString()),
                    DataType = CellValues.InlineString,
                    InlineString = new InlineString() { Text = new Text(bitacora.ElementAt(i).NombreDeEmpleado) },
                };

                Cell Bcell = new Cell()
                {
                    CellReference = string.Format("B{0}", row.RowIndex.ToString()),
                    DataType = CellValues.InlineString,
                    InlineString = new InlineString() { Text = new Text(bitacora.ElementAt(i).NombreRefrigerador) },
                };

                Cell Ccell = new Cell()
                {
                    CellReference = string.Format("C{0}", row.RowIndex.ToString()),
                    DataType = CellValues.InlineString,
                    InlineString = new InlineString() { Text = new Text(bitacora.ElementAt(i).NumeroDeCartucho) },
                };

                Cell Dcell = new Cell()
                {
                    CellReference = string.Format("D{0}", row.RowIndex.ToString()),
                    DataType = CellValues.InlineString,
                    InlineString = new InlineString() { Text = new Text(movimiento) },
                };
                Cell Ecell = new Cell()
                {
                    CellReference = string.Format("E{0}", row.RowIndex.ToString()),
                    DataType = CellValues.InlineString,
                    InlineString = new InlineString() { Text = new Text(bitacora.ElementAt(i).FechaMovimiento.ToString()) },
                };

                row.Append(Acell);
                row.Append(Bcell);
                row.Append(Ccell);
                row.Append(Dcell);
                row.Append(Ecell);

                sheetData.Append(row);
            }
            return (sheetData);
        }



        #endregion ExcelRelated
    }
}
