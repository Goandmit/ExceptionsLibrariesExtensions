using AccountsLib;
using ClientsLib;
using RelayCommandLib;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using WorkersLib;

namespace ExceptionsLibrariesExtensions
{
    public class AuthorizationFormVM
    {
        public ObservableCollection<Worker> Workers { get; set; }
        public Worker SelectedWorker { get; set; }

        public AuthorizationFormVM()
        {
            Workers = new ObservableCollection<Worker>
            {
                ProgramManager.consultant,
                ProgramManager.manager
            };
        }

        private RelayCommand oKCommand;
        public RelayCommand OKCommand
        {
            get
            {
                return oKCommand ??
                  (oKCommand = new RelayCommand(obj =>
                  {
                      if (SelectedWorker is Worker)
                      {
                          ProgramManager.AuthorizeUser(SelectedWorker);                          
                      }
                  }));
            }
        }
    }

    public class ClientListVM
    {     
        public bool CreateIsEnabled { get; set; }
        public bool DeleteIsEnabled { get; set; }

        public ObservableCollection<ClientListItem> Clients { get; set; }
        public ClientListItem SelectedClient { get; set; }

        public ClientListVM()
        {
            CreateIsEnabled = ProgramManager.CurrentUser.СreationIsAllowed;
            DeleteIsEnabled = ProgramManager.CurrentUser.DeletionIsAllowed;

            Clients = new ObservableCollection<ClientListItem>();
            Clients = ProgramManager.FillClients(Clients);

            WindowsManager.CurrentClientListVM = this;            
        }        

        private RelayCommand createCommand;
        public RelayCommand CreateCommand
        {
            get
            {
                return createCommand ??
                  (createCommand = new RelayCommand(obj =>
                  {
                      WindowsManager.OpenCreatingClientForm();
                  }));
            }
        }

        private RelayCommand deleteCommand;
        public RelayCommand DeleteCommand
        {
            get
            {
                return deleteCommand ??
                  (deleteCommand = new RelayCommand(obj =>
                  {
                      if (!string.IsNullOrEmpty($"{SelectedClient}"))
                      {
                          ProgramManager.DeleteClient(SelectedClient.Id);                          
                      }
                  }));
            }
        }

        private RelayCommand openCommand;
        public RelayCommand OpenCommand
        {
            get
            {
                return openCommand ??
                  (openCommand = new RelayCommand(obj =>
                  {
                      if (!string.IsNullOrEmpty($"{SelectedClient}"))
                      {                      
                          ProgramManager.ShowClient(SelectedClient.Id);
                      }
                  }));
            }
        }
    }

    public class ClientFormsVM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

        private string id;
        public string Id
        {
            get { return id; }
            set
            {
                id = value;
                OnPropertyChanged($"{nameof(Id)}");
            }
        }

        private string fullName;
        public string FullName
        {
            get { return fullName; }
            set
            {
                fullName = value;
                OnPropertyChanged($"{nameof(FullName)}");
            }
        }        

        public string OrgForm { get; set; }        
        public string Name { get; set; }
        public string INN { get; set; }
        public string PhoneNumber { get; set; }
        public bool VIP { get; set; }
        public string KPP { get; set; }
        public string Surname { get; set; }
        public string Patronymic { get; set; }
        public string PassportSeries { get; set; }
        public string PassportNumber { get; set; }        

        public bool NameIsEnabled { get; set; }
        public bool INNIsEnabled { get; set; }        
        public bool PhoneNumberIsEnabled { get; set; }
        public bool VIPIsEnabled { get; set; }
        public bool KPPIsEnabled { get; set; }        
        public bool SurnameIsEnabled { get; set; }
        public bool PatronymicIsEnabled { get; set; }
        public bool PassportSeriesIsEnabled { get; set; }
        public bool PassportNumberIsEnabled { get; set; }

        public ObservableCollection<Account> Accounts { get; set; }
        public Account SelectedAccount { get; set; }

        public ClientFormsVM()
        {
            Accounts = new ObservableCollection<Account>();
            IdentifyAvailableFields();

            if (!string.IsNullOrEmpty($"{ProgramManager.CurrentClient}"))
            {
                Id = ProgramManager.CurrentClient.Id;
                OrgForm = ProgramManager.CurrentClient.OrgForm;
                FullName = ProgramManager.CurrentClient.FullName;
                Name = ProgramManager.CurrentClient.Name;
                INN = ProgramManager.CurrentClient.INN;
                PhoneNumber = ProgramManager.CurrentClient.PhoneNumber;
                VIP = ProgramManager.CurrentClient.VIP;

                if (ProgramManager.CurrentClient is JuridicalPerson jp)
                {
                    KPP = jp.KPP;
                }

                if (ProgramManager.CurrentClient is PhysicalPerson pp)
                {
                    Surname = pp.Surname;
                    Patronymic = pp.Patronymic;

                    if (ProgramManager.CurrentUser.ViewingIsAllowed == false)
                    {
                        PassportSeries = ProgramManager.HideString(pp.PassportSeries);
                        PassportNumber = ProgramManager.HideString(pp.PassportNumber);
                    }
                    else
                    {
                        PassportSeries = pp.PassportSeries;
                        PassportNumber = pp.PassportNumber;
                    }                    
                }

                Accounts = ProgramManager.FillAccounts(Accounts);
            }

            if (!string.IsNullOrEmpty($"{ProgramManager.CurrentOrgForm}"))
            {
                OrgForm = ProgramManager.CurrentOrgForm;                
            }

            WindowsManager.CurrentClientFormsVM = this;
        }

        private void IdentifyAvailableFields()
        {
            NameIsEnabled = (ProgramManager.CurrentUser.AccessibleFields.Contains(4)) ? true : false;
            INNIsEnabled = (ProgramManager.CurrentUser.AccessibleFields.Contains(5)) ? true : false;
            PhoneNumberIsEnabled = (ProgramManager.CurrentUser.AccessibleFields.Contains(6)) ? true : false;
            VIPIsEnabled = (ProgramManager.CurrentUser.AccessibleFields.Contains(7)) ? true : false;
            KPPIsEnabled = (ProgramManager.CurrentUser.AccessibleFields.Contains(8)) ? true : false;
            SurnameIsEnabled = (ProgramManager.CurrentUser.AccessibleFields.Contains(8)) ? true : false;
            PatronymicIsEnabled = (ProgramManager.CurrentUser.AccessibleFields.Contains(9)) ? true : false;
            PassportSeriesIsEnabled = (ProgramManager.CurrentUser.AccessibleFields.Contains(10)) ? true : false;
            PassportNumberIsEnabled = (ProgramManager.CurrentUser.AccessibleFields.Contains(11)) ? true : false;
        }

        private string EliminateNull(string userInput)
        {
            userInput = (userInput == null) ? String.Empty : userInput.Trim();

            return userInput;
        }

        private void PrepareFields()
        {
            Surname = EliminateNull(Surname);
            Name = EliminateNull(Name);
            Patronymic = EliminateNull(Patronymic);
            INN = EliminateNull(INN);
            KPP = EliminateNull(KPP);
            PhoneNumber = EliminateNull(PhoneNumber);
            PassportSeries = EliminateNull(PassportSeries);
            PassportNumber = EliminateNull(PassportNumber);
        }        

        private bool CheckFields()
        {
            PrepareFields();

            bool status = true;

            try
            {
                if (OrgForm == ProgramManager.pp.OrgFormPP)
                {
                    if (Surname.Length == 0 || Name.Length == 0 || PhoneNumber.Length == 0 ||
                        PassportSeries.Length == 0 || PassportNumber.Length == 0)
                    {
                        throw new EmptyFieldsException(OrgForm);                        
                    }
                }

                if (OrgForm == ProgramManager.pp.OrgFormIB)
                {
                    if (Surname.Length == 0 || Name.Length == 0 || INN.Length == 0 || PhoneNumber.Length == 0 ||
                        PassportSeries.Length == 0 || PassportNumber.Length == 0)
                    {
                        throw new EmptyFieldsException(OrgForm);                        
                    }
                }

                if (OrgForm == ProgramManager.jp.OrgFormJP)
                {
                    if (Name.Length == 0 || INN.Length == 0 || KPP.Length == 0 || PhoneNumber.Length == 0)
                    {
                        throw new EmptyFieldsException(OrgForm);                        
                    }
                }
            }            
            catch (EmptyFieldsException e) when (e.ErrorCode == ProgramManager.pp.OrgFormPP)
            {
                WindowsManager.CallErrorMessageBox("Пустыми могут быть только поля \"Отчество\" и \"ИНН\"");
                status = false;
            }
            catch (EmptyFieldsException e) when (e.ErrorCode == ProgramManager.pp.OrgFormIB)
            {
                WindowsManager.CallErrorMessageBox("Пустым может быть только поле \"Отчество\"");
                status = false;
            }
            catch (EmptyFieldsException e) when (e.ErrorCode == ProgramManager.jp.OrgFormJP)
            {
                WindowsManager.CallErrorMessageBox("Все поля должны быть заполнены");
                status = false;
            }            

            return status;
        }        
        
        private Client GetClientFromForm()
        {
            Client client;

            if (OrgForm == ProgramManager.jp.OrgFormJP)
            {
                client = new JuridicalPerson(ProgramManager.AssignOrReadClientId(Id), OrgForm, Name, INN,
                     PhoneNumber, VIP, KPP);
            }
            else
            {
                client = new PhysicalPerson(ProgramManager.AssignOrReadClientId(Id), OrgForm, Name, INN,
                     PhoneNumber, VIP, Surname, Patronymic, PassportSeries, PassportNumber);
            }

            Id = client.Id;
            FullName = client.FullName;

            return client;
        }

        private RelayCommand changeLogCommand;
        public RelayCommand ChangeLogCommand
        {
            get
            {
                return changeLogCommand ??
                  (changeLogCommand = new RelayCommand(obj =>
                  {
                      if (!string.IsNullOrEmpty($"{Id}"))
                      {
                          WindowsManager.OpenChangeLog();
                      }
                  }));
            }
        }

        private RelayCommand writeCommand;
        public RelayCommand WriteCommand
        {
            get
            {
                return writeCommand ??
                  (writeCommand = new RelayCommand(obj =>
                  {
                      bool status = CheckFields();

                      if (status == true)
                      {
                          Client client = GetClientFromForm();
                          ProgramManager.OwerwriteOrWriteClient(client, false);
                      }
                  }));
            }
        }

        private RelayCommand oKCommand;
        public RelayCommand OKCommand
        {
            get
            {
                return oKCommand ??
                  (oKCommand = new RelayCommand(obj =>
                  {
                      bool status = CheckFields();

                      if (status == true)
                      {
                          Client client = GetClientFromForm();
                          ProgramManager.OwerwriteOrWriteClient(client, true);
                      }

                  }));
            }
        }

        private RelayCommand createCommand;
        public RelayCommand CreateCommand
        {
            get
            {
                return createCommand ??
                  (createCommand = new RelayCommand(obj =>
                  {
                      if (!string.IsNullOrEmpty(Id))
                      {
                          WindowsManager.OpenCreatingAccountForm();
                      }

                  }));
            }
        }

        private RelayCommand deleteCommand;
        public RelayCommand DeleteCommand
        {
            get
            {
                return deleteCommand ??
                  (deleteCommand = new RelayCommand(obj =>
                  {
                      if (!string.IsNullOrEmpty(Id))
                      {
                          if (!string.IsNullOrEmpty($"{SelectedAccount}"))
                          {
                              ProgramManager.CloseAccount(SelectedAccount);                             
                          }
                      }
                  }));
            }
        }

        private RelayCommand openCommand;
        public RelayCommand OpenCommand
        {
            get
            {
                return openCommand ??
                  (openCommand = new RelayCommand(obj =>
                  {
                      if (!string.IsNullOrEmpty($"{SelectedAccount}"))
                      {
                          ProgramManager.ShowAccount(SelectedAccount);
                      }
                  }));
            }
        }
    }

    public class AccountFormsVM
    {
        public string AccountType { get; set; }
        public string AccountNumber { get; set; }
        public string OpeningDate { get; set; }
        public string Balance { get; set; }
        public string InterestRate { get; set; }
        public string DateOfNextAccrual { get; set; }
        public string AccrualInCurrentMonth { get; set; }

        public AccountFormsVM()
        {
            AccountType = ProgramManager.CurrentAccount.AccountType;
            AccountNumber = ProgramManager.CurrentAccount.AccountNumber;
            OpeningDate = (ProgramManager.CurrentAccount.OpeningDate).ToShortDateString();
            Balance = ProgramManager.CurrentAccount.Balance.ToString();

            if (ProgramManager.CurrentAccount is DepositAccount da)
            {
                InterestRate = da.InterestRate.ToString();
                DateOfNextAccrual = (da.DateOfNextAccrual).ToShortDateString();
                AccrualInCurrentMonth = da.AccrualInCurrentMonth.ToString();
            }
        }

        private RelayCommand transferCommand;
        public RelayCommand TransferCommand
        {
            get
            {
                return transferCommand ??
                  (transferCommand = new RelayCommand(obj =>
                  {
                      ProgramManager.ShowTransferForm(false);
                  }));
            }
        }

        private RelayCommand transferToYourselfCommand;
        public RelayCommand TransferToYourselfCommand
        {
            get
            {
                return transferToYourselfCommand ??
                  (transferToYourselfCommand = new RelayCommand(obj =>
                  {
                      ProgramManager.ShowTransferForm(true);
                  }));
            }
        }

        private RelayCommand depositCommand;
        public RelayCommand DepositCommand
        {
            get
            {
                return depositCommand ??
                  (depositCommand = new RelayCommand(obj =>
                  {
                      ProgramManager.ShowReplenishAccountForm();
                  }));
            }
        }
    }

    public class CreatingClientFormVM
    {
        public bool JP { get; set; }
        public bool PP { get; set; }
        public bool IB { get; set; }        

        public string JuridicalPerson { get; set; }
        public string PhysicalPerson { get; set; }
        public string IndividualBusinessman { get; set; }

        public CreatingClientFormVM()
        {
            JuridicalPerson = ProgramManager.jp.OrgFormJP;
            PhysicalPerson = ProgramManager.pp.OrgFormPP;
            IndividualBusinessman = ProgramManager.pp.OrgFormIB;
        }

        private RelayCommand oKCommand;
        public RelayCommand OKCommand
        {
            get
            {
                return oKCommand ??
                  (oKCommand = new RelayCommand(obj =>
                  {
                      if (JP == true) { ProgramManager.ShowNewClient(JuridicalPerson); }

                      if (PP == true) { ProgramManager.ShowNewClient(PhysicalPerson); }

                      if (IB == true) { ProgramManager.ShowNewClient(IndividualBusinessman); } 
                  }));
            }
        }
    }

    public class CreatingAccountFormVM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

        public ObservableCollection<Account> Accounts { get; set; }
        public Account SelectedAccount { get; set; }       

        private string balance;
        public string Balance
        {
            get { return balance; }
            set
            {
                balance = value.GetOnlyConvertibleIntoDecimal();                
                OnPropertyChanged($"{nameof(Balance)}");
            }
        }        

        public CreatingAccountFormVM()
        {
            Accounts = new ObservableCollection<Account>
            {
                ProgramManager.da,
                ProgramManager.nda
            };
        }        

        private RelayCommand oKCommand;
        public RelayCommand OKCommand
        {
            get
            {
                return oKCommand ??
                  (oKCommand = new RelayCommand(obj =>
                  {
                      if (SelectedAccount is Account)
                      {
                          if (Balance == null) { Balance = "0,00"; }

                          if (decimal.TryParse(Balance, out var result))
                          {
                              if (!Balance.Contains(',')) { Balance += ",00"; }

                              ProgramManager.OpenAccount(SelectedAccount, Balance);                              
                          }
                      }
                  }));
            }
        }
    }

    public class ReplenishAccountFormVM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

        public string AccountNumber { get; set; }

        private string balance;
        public string Balance
        {
            get { return balance; }
            set
            {
                balance = value.GetOnlyConvertibleIntoDecimal();                
                OnPropertyChanged($"{nameof(Balance)}");
            }
        }

        public ReplenishAccountFormVM()
        {           
            AccountNumber = ProgramManager.CurrentAccount.AccountNumber;            
        }

        private RelayCommand oKCommand;
        public RelayCommand OKCommand
        {
            get
            {
                return oKCommand ??
                  (oKCommand = new RelayCommand(obj =>
                  {
                      if (decimal.TryParse(Balance, out var result))
                      {
                          if (result >= 1)
                          {
                              ProgramManager.DepositAccount(result);
                          }
                      }
                  }));
            }
        }
    }

    public class TransferFormVM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

        private string whom;
        public string Whom
        {
            get { return whom; }
            set
            {
                whom = value;
                OnPropertyChanged($"{nameof(Whom)}");
            }
        }

        private string where;
        public string Where
        {
            get { return where; }
            set
            {
                where = value;
                OnPropertyChanged($"{nameof(Where)}");
            }
        }

        private string amount;
        public string Amount
        {
            get { return amount; }
            set
            {
                amount = value.GetOnlyConvertibleIntoDecimal();                
                OnPropertyChanged($"{nameof(Amount)}");
            }
        }

        public bool WhomIsEnabled { get; set; }

        public TransferFormVM()
        {
            WhomIsEnabled = true;

            if (!string.IsNullOrEmpty($"{ProgramManager.Recipient}"))
            {
                if (ProgramManager.Recipient.Id == ProgramManager.Sender.Id)
                {
                    Whom = ProgramManager.Recipient.FullName;
                    WhomIsEnabled = false;
                }
            }

            WindowsManager.CurrentTransferFormVM = this;
        }        

        private RelayCommand whomCommand;
        public RelayCommand WhomCommand
        {
            get
            {
                return whomCommand ??
                  (whomCommand = new RelayCommand(obj =>
                  {
                      WindowsManager.OpenWhomToTransfer();
                  }));
            }
        }

        private RelayCommand whereCommand;
        public RelayCommand WhereCommand
        {
            get
            {
                return whereCommand ??
                  (whereCommand = new RelayCommand(obj =>
                  {
                      if (!string.IsNullOrEmpty($"{ProgramManager.Recipient}"))
                      {
                          WindowsManager.OpenWhereToTransfer();
                      }
                  }));
            }
        }

        private RelayCommand transferCommand;
        public RelayCommand TransferCommand
        {
            get
            {
                return transferCommand ??
                  (transferCommand = new RelayCommand(obj =>
                  {
                      if (!string.IsNullOrEmpty($"{Whom}") &&
                          !string.IsNullOrEmpty($"{Where}") &&
                          !string.IsNullOrEmpty($"{Amount}"))
                      {
                          if (decimal.TryParse(Amount, out var result))
                          {
                              if (result >= 1)
                              {
                                  ProgramManager.Transfer(result);                                 
                              }
                          }
                      }
                  }));
            }
        }
    }

    public class WhomToTransferVM
    {
        public ObservableCollection<ClientListItem> Clients { get; set; }
        public ClientListItem SelectedClient { get; set; }

        public WhomToTransferVM()
        {
            Clients = new ObservableCollection<ClientListItem>();
            Clients = ProgramManager.FillClientsWithoutSender(Clients);
        }

        private RelayCommand selectionCommand;
        public RelayCommand SelectionCommand
        {
            get
            {
                return selectionCommand ??
                  (selectionCommand = new RelayCommand(obj =>
                  {
                      if (!string.IsNullOrEmpty($"{SelectedClient}"))
                      {
                          ProgramManager.SelectRecipient(SelectedClient);                          
                      }

                  }));
            }
        }
    }

    public class WhereToTransferVM
    {
        public ObservableCollection<Account> Accounts { get; set; }
        public Account SelectedAccount { get; set; }

        public WhereToTransferVM()
        {
            Accounts = new ObservableCollection<Account>();
            Accounts = ProgramManager.FillAccountsWithoutSenderAccount(Accounts);            
        }

        private RelayCommand selectionCommand;
        public RelayCommand SelectionCommand
        {
            get
            {
                return selectionCommand ??
                  (selectionCommand = new RelayCommand(obj =>
                  {
                      if (!string.IsNullOrEmpty($"{SelectedAccount}"))
                      {
                          ProgramManager.SelectRecipientAccount(SelectedAccount);                          
                      }
                  }));
            }
        }
    }

    public class ChangeLogVM
    {
        public ObservableCollection<string> Records { get; set; }

        public ChangeLogVM()
        {
            Records = new ObservableCollection<string>();
            Records = ProgramManager.FillChangeLog(Records);
        }
    }

    public class NotificationVM
    {
        public string Message { get; set; }

        public NotificationVM()
        {
            WindowsManager.CurrentNotificationVM = this;
        }
    }
}
