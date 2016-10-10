using MPManagement.ViewModels.Commands;
using SPManagement.Business;
using SPManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

namespace MPManagement.ViewModels
{
    public class ConfigurationVM : ViewModelBase
    {
        #region Properties
        private string _hoursBeforeDek;
        public string HoursBeforeDek
        {
            get
            {
                return (_hoursBeforeDek);
            }
            set
            {
                if (value.Length >= 1)
                {
                    if (regex.IsMatch(value.ElementAt(value.Length-1).ToString()))
                    {
                        _hoursBeforeDek = value;
                        OnPropertyChanged();
                    }
                }
                else if (value.Equals(string.Empty))
                {
                    _hoursBeforeDek = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _hoursReadyForDek;
        public string HoursReadyForDek
        {
            get
            {
                return (_hoursReadyForDek);
            }
            set
            {
                if (value.Length >= 1)
                {
                    if (regex.IsMatch(value.ElementAt(value.Length - 1).ToString()))
                    {
                        _hoursReadyForDek = value;
                        OnPropertyChanged();
                    }
                }
                else if (value.Equals(string.Empty))
                {
                    _hoursReadyForDek = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _hoursAfterReturn;
        public string HoursAfterReturn
        {
            get
            {
                return (_hoursAfterReturn);
            }
            set
            {
                if (value.Length >= 1)
                {
                    if (regex.IsMatch(value.ElementAt(value.Length - 1).ToString()))
                    {
                        _hoursAfterReturn = value;
                        OnPropertyChanged();
                    }
                }
                else if (value.Equals(string.Empty))
                {
                    _hoursAfterReturn = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _secondsForRefresh;
        public string SecondsForRefresh
        {
            get
            {
                return (_secondsForRefresh);
            }
            set
            {
                if (value.Length >= 1)
                {
                    if (regex.IsMatch(value.ElementAt(value.Length - 1).ToString()))
                    {
                        _secondsForRefresh = value;
                        OnPropertyChanged();
                    }
                }
                else if (value.Equals(string.Empty))
                {
                    _secondsForRefresh = value;
                    OnPropertyChanged();
                }
            }
        }

        private readonly RelayCommand _saveTimeChangesCommand;
        public RelayCommand SaveTimeChangesCommand => _saveTimeChangesCommand;

        private readonly RelayCommand _cancelTimeChangesCommand;
        public RelayCommand CancelTimeChangesCommand => _cancelTimeChangesCommand;

        private Tiempo tiempo;
        private readonly TiempoBusiness tiempoBusiness;
        private readonly Regex regex;


        #endregion Properties

        #region Constructor
        public ConfigurationVM(Action closeAction)
        {
            tiempoBusiness = new TiempoBusiness();
            regex = new Regex(@"^[0-9]?$");
            tiempo = tiempoBusiness.GetAll().FirstOrDefault();
            _saveTimeChangesCommand = new RelayCommand(SaveTimeChanges);
            _cancelTimeChangesCommand = new RelayCommand(closeAction);
            if (tiempo != null)
            {
                HoursBeforeDek = tiempo.HorasAmbientacionMin.ToString();
                HoursReadyForDek = tiempo.HorasAmbientacionMax.ToString();
                HoursAfterReturn = tiempo.HorasReposoTrasRetorno.ToString();
                SecondsForRefresh = tiempo.SegundosRefresco.ToString();
            }
        }
        #endregion Constructor

        #region ButtonsRelated

        private void SaveTimeChanges()
        {
            int hoursBeforeDek;
            int hoursReadyForDek;
            int hoursAfterReturn;
            int secondsForRefresh;

            if (string.IsNullOrEmpty(HoursBeforeDek) || string.IsNullOrEmpty(HoursReadyForDek) || string.IsNullOrEmpty(HoursAfterReturn) || string.IsNullOrEmpty(SecondsForRefresh))
                MessageBox.Show("Alguno de los campos no fue llenado, favor de rectificar la informacion", "ERROR");
            else
            {
                Int32.TryParse(Regex.Replace(HoursBeforeDek, " ", string.Empty), out hoursBeforeDek);
                Int32.TryParse(Regex.Replace(HoursReadyForDek, " ", string.Empty), out hoursReadyForDek);
                Int32.TryParse(Regex.Replace(HoursAfterReturn, " ", string.Empty), out hoursAfterReturn);
                Int32.TryParse(Regex.Replace(SecondsForRefresh, " ", string.Empty), out secondsForRefresh);

                if (!(hoursBeforeDek.Equals(0) || hoursReadyForDek.Equals(0) || hoursAfterReturn.Equals(0) || secondsForRefresh.Equals(0)))
                {
                    if (hoursBeforeDek < hoursReadyForDek)
                    {
                        if (tiempo != null)
                        {
                            tiempo.HorasAmbientacionMin = hoursBeforeDek;
                            tiempo.HorasAmbientacionMax = hoursReadyForDek;
                            tiempo.HorasReposoTrasRetorno = hoursAfterReturn;
                            tiempo.SegundosRefresco = secondsForRefresh;
                            tiempoBusiness.UpdateTiempo(tiempo);
                            MessageBox.Show("Cambios realizados con exito", "EXITO");
                            CancelTimeChangesCommand.Execute();
                        }
                        else
                        {
                            tiempo = new Tiempo()
                            {
                                HorasAmbientacionMin = hoursBeforeDek,
                                HorasAmbientacionMax = hoursReadyForDek,
                                HorasReposoTrasRetorno = hoursAfterReturn,
                                SegundosRefresco = secondsForRefresh
                            };
                            tiempoBusiness.InsertTiempo(tiempo);
                            MessageBox.Show("Cambios realizados con exito", "EXITO");
                            CancelTimeChangesCommand.Execute();
                        }
                    }
                    else
                        MessageBox.Show("No es posible adjudicar al tiempo mayor de ambientacion un valor menor al tiempo minimo en ambientacion", "ERROR");
                }
                else
                    MessageBox.Show("No puede adjudicar un tiempo cero, favor de rectificar la informacion", "ERROR");
            }

        }

        #endregion ButtonsRelated
    }
}
