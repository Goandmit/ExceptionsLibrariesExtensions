using AccountsLib;
using ClientsLib;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using WorkersLib;
using WorkingWithFileLib;

namespace ExceptionsLibrariesExtensions
{
    public class EmptyFieldsException : Exception
    {
        public string ErrorCode { get; set; }

        public EmptyFieldsException(string errorCode)
        {
            ErrorCode = errorCode;
        }
    }

    public static class ProgramManager
    {
        private delegate void Notifications(string message);
        private static event Notifications Notify;

        public static Consultant consultant = new Consultant();
        public static Manager manager = new Manager();

        public static JuridicalPerson jp = new JuridicalPerson();
        public static PhysicalPerson pp = new PhysicalPerson();

        public static DepositAccount da = new DepositAccount();
        public static NonDepositAccount nda = new NonDepositAccount();       

        public static Worker CurrentUser { get; private set; }        
        public static Client CurrentClient { get; private set; }
        public static Account CurrentAccount { get; private set; }
        public static string CurrentOrgForm { get; private set; }

        public static ClientListItem Sender { get; private set; }
        public static Account SenderAccount { get; private set; }
        public static ClientListItem Recipient { get; private set; }
        public static Account RecipientAccount { get; private set; }

        public static void SetCurrentUser(Worker worker) { CurrentUser = worker; }
        public static void SetCurrentClient(Client client) { CurrentClient = client; }
        public static void SetCurrentAccount(Account account) { CurrentAccount = account; }
        public static void SetCurrentOrgForm(string orgForm) { CurrentOrgForm = orgForm; }

        public static void ResetCurrentClient() { CurrentClient = null; }
        public static void ResetCurrentAccount() { CurrentAccount = null; }
        public static void ResetCurrentOrgForm() { CurrentOrgForm = null; }

        public static void SetSender(ClientListItem client) { Sender = client; }
        public static void SetSenderAccount(Account account) { SenderAccount = account; }
        public static void SetRecipient(ClientListItem client) { Recipient = client; }
        public static void SetRecipientAccount(Account account) { RecipientAccount = account; }

        public static string GetOnlyConvertibleIntoDecimal(this string userInput)
        {
            Regex regex = new Regex("[^0-9,.]");

            if (regex.IsMatch(userInput))
            {
                userInput = userInput.Remove(userInput.Length - 1);
            }

            if (userInput.Contains('.'))
            {
                userInput = userInput.Replace('.', ',');
            }

            if (userInput.Contains(','))
            {
                int decimalPlaces = userInput.IndexOf(',') + 3;

                if (userInput.Length > decimalPlaces)
                {
                    userInput = userInput.Remove(userInput.Length - 1);
                }
            }

            return userInput;
        }        

        #region AuthorizationFormVM

        private static void AssignAccessRights()
        {
            if (CurrentUser is Consultant)
            {
                CurrentUser.AccessibleFields.Add(6);
                CurrentUser.AccessibleFields.Add(7);

                CurrentUser.SetViewing(false);
                CurrentUser.SetСreation(false);
                CurrentUser.SetDeletion(false);
            }

            if (CurrentUser is Manager)
            {
                for (int i = 0; i <= 11; i++)
                {
                    CurrentUser.AccessibleFields.Add(i);
                }

                CurrentUser.SetViewing(true);
                CurrentUser.SetСreation(true);
                CurrentUser.SetDeletion(true);
            }
        }

        public static void AuthorizeUser(Worker worker)
        {
            SetCurrentUser(worker);
            AssignAccessRights();            

            WindowsManager.DefineWorkingAreaSize();
            WindowsManager.OpenClientList();
            WindowsManager.CloseWindow<AuthorizationForm>();
        }

        #endregion

        #region ClientListVM

        public static ObservableCollection<ClientListItem> FillClients
            (ObservableCollection<ClientListItem> Clients)
        {
            List<ClientListItem> clients = ClientManager.GetClientListItems();

            foreach (ClientListItem client in clients)
            {
                Clients.Add(client);
            }

            return Clients;
        }        

        public static void RefreshClientList()
        {
            WindowsManager.CurrentClientListVM.Clients.Clear();
            WindowsManager.CurrentClientListVM.Clients
                = FillClients(WindowsManager.CurrentClientListVM.Clients);
        }

        public static void DeleteClient(string clientId)
        {
            bool status = WorkingWithFile.CheckBeforeReading
                (AccountManager.GenerateAccountsFilePath(clientId));

            if (status == true)
            {
                WindowsManager.CallErrorMessageBox("Клиент, имеющий незакрытые счета не может быть удален");
            }
            else
            {
                ClientManager.DeleteClient(clientId);

                Notify += WindowsManager.CallNotification;
                Notify?.Invoke("Клиент удален");

                File.Delete(GenerateChangeLogFilePath(clientId));
            }

            RefreshClientList();
        }

        public static void ShowClient(string clientId)
        {
            Client client = ClientManager.GetClient(clientId);
            SetCurrentClient(client);
            WindowsManager.OpenClientForm(client.OrgForm);
        }

        #endregion

        #region ClientFormsVM

        public static ObservableCollection<Account> FillAccounts
            (ObservableCollection<Account> Accounts)
        {
            List<Account> accounts = AccountManager.GetAccounts(CurrentClient.Id);

            foreach (Account account in accounts)
            {
                if (account is DepositAccount da)
                {
                    if (da.OverwritingIsRequired == true)
                    {
                        AccountManager.OwerwriteAccount(da);
                    }
                }

                Accounts.Add(account);
            }

            return Accounts;
        }

        public static void RefreshAccounts()
        {
            WindowsManager.CurrentClientFormsVM.Accounts.Clear();
            WindowsManager.CurrentClientFormsVM.Accounts
                = FillAccounts(WindowsManager.CurrentClientFormsVM.Accounts);
        }

        public static string AssignOrReadClientId(string possibleId)
        {
            string id = WorkingWithFile.AssignOrReadId(possibleId, ClientManager.ClientsIdFilePath);

            return id;
        }

        public static void OwerwriteOrWriteClient(Client client, bool closeWindow)
        {
            List<string> changeLog;

            SetCurrentClient(client);

            List<string> changedFields = ClientManager.OwerwriteOrWriteClient(client, CurrentUser.AccessibleFields);            

            if (changedFields.Count == 0)
            {
                changeLog = ClientManager.ChangeLogWrite();
            }
            else
            {
                changeLog = ClientManager.ChangeLogOwerwrite(changedFields);
            }

            WriteChangeLog(changeLog, client.Id);

            if (closeWindow == true)
            {
                WindowsManager.CloseClientForm(client.OrgForm);
            }

            Notify += WindowsManager.CallNotification;
            Notify?.Invoke(changeLog[0]);
        }

        public static void CloseAccount(Account account)
        {
            bool status = AccountManager.CloseAccount(account);

            List<string> changeLog = AccountManager.ChangeLogClose(account.AccountNumber, status);
            WriteChangeLog(changeLog, account.OwnerId);

            if (status == false)
            {
                WindowsManager.CallErrorMessageBox("Баланс счета не является нулевым");
            }
            else
            {
                RefreshAccounts();
                Notify += WindowsManager.CallNotification;
                Notify?.Invoke(changeLog[0]);
            }
        }

        public static void ShowAccount(Account account)
        {
            SetCurrentAccount(account);
            WindowsManager.OpenAccountForm();
        }

        public static string HideString(string normalString)
        {
            string hiddenString = String.Empty;

            for (int i = 0; i < normalString.Length; i++)
            {
                if (normalString[i] != ' ')
                {
                    hiddenString += "*";
                }
                else
                {
                    hiddenString += " ";
                }
            }

            return hiddenString;
        }

        #endregion

        #region AccountFormsVM

        public static void ShowTransferForm(bool transferToYourself)
        {
            ClientListItem client = new ClientListItem(CurrentClient.Id, CurrentClient.OrgForm,
                    CurrentClient.Requisites, CurrentClient.FullName);

            SetSender(client);

            if (transferToYourself == true)
            {
                SetRecipient(client);
            }

            SetSenderAccount(CurrentAccount);

            WindowsManager.OpenTransferForm();            
        }

        public static void ShowReplenishAccountForm()
        {
            SetSenderAccount(CurrentAccount);
            WindowsManager.OpenReplenishAccountForm();            
        }

        #endregion

        #region CreatingClientFormVM

        public static void ShowNewClient(string orgForm)
        {
            SetCurrentOrgForm(orgForm);
            WindowsManager.OpenClientForm(orgForm);
            WindowsManager.CloseWindow<CreatingClientForm>();
        }

        #endregion

        #region CreatingAccountFormVM

        public static void OpenAccount(Account account, string balance)
        {
            if (account is DepositAccount)
            {
                account = new DepositAccount
                    (Convert.ToString(WorkingWithFile.AssignId(AccountManager.AccountsIdFilePath)),
                    CurrentClient.Id, CurrentClient.OrgForm, Convert.ToDecimal(balance), CurrentClient.VIP);

            }
            else
            {
                account = new NonDepositAccount
                    (Convert.ToString(WorkingWithFile.AssignId(AccountManager.AccountsIdFilePath)),
                    CurrentClient.Id, CurrentClient.OrgForm, Convert.ToDecimal(balance));

            }

            AccountManager.OpenAccount(account);

            List<string> changeLog = AccountManager.ChangeLogOpen(account.AccountNumber, account.Balance);
            WriteChangeLog(changeLog, account.OwnerId);

            WindowsManager.CloseWindow<CreatingAccountForm>();            

            Notify += WindowsManager.CallNotification;
            Notify?.Invoke(changeLog[0]);
        }

        #endregion

        #region ReplenishAccountFormVM

        public static void DepositAccount(decimal amount)
        {
            AccountManager.DepositAccount(SenderAccount, amount);

            List<string> changeLog = AccountManager.ChangeLogDeposit(SenderAccount.AccountNumber, amount);
            WriteChangeLog(changeLog, SenderAccount.OwnerId);

            WindowsManager.CloseWindow<ReplenishAccountForm>();
            WindowsManager.CloseAccountForm();

            Notify += WindowsManager.CallNotification;
            Notify?.Invoke(changeLog[0]);
            
            ResetTransferInfo();
        }

        #endregion

        #region TransferFormVM

        public static void Transfer(decimal amount)
        {
            bool status = AccountManager.Transfer(SenderAccount, RecipientAccount, amount);

            if (status == true)
            {
                List<string> changeLog;

                if (SenderAccount.OwnerId == RecipientAccount.OwnerId)
                {
                    changeLog = AccountManager.ChangeLogYourself(SenderAccount.AccountNumber,
                        RecipientAccount.AccountNumber, amount);

                    WriteChangeLog(changeLog, Sender.Id);
                }
                else
                {
                    changeLog = AccountManager.ChangeLogRecipient(SenderAccount.AccountNumber,
                        RecipientAccount.AccountNumber, amount, Sender.FullName);

                    WriteChangeLog(changeLog, Recipient.Id);

                    changeLog = AccountManager.ChangeLogSender(SenderAccount.AccountNumber,
                        RecipientAccount.AccountNumber, amount, Recipient.FullName);

                    WriteChangeLog(changeLog, Sender.Id);
                }

                WindowsManager.CloseWindow<TransferForm>();
                WindowsManager.CloseAccountForm();

                Notify += WindowsManager.CallNotification;
                Notify?.Invoke(changeLog[0]);                
            }
            else
            {
                WindowsManager.CallErrorMessageBox("Недостаточно средств");
            }
        }

        public static void ResetTransferInfo()
        {
            Sender = null;
            SenderAccount = null;
            Recipient = null;
            RecipientAccount = null;
        }

        #endregion

        #region WhomToTransferVM

        public static ObservableCollection<ClientListItem> FillClientsWithoutSender
            (ObservableCollection<ClientListItem> Clients)
        {
            List<ClientListItem> clients = ClientManager.GetClientListItems();

            foreach (ClientListItem client in clients)
            {
                if (client.Id == Sender.Id)
                {
                    continue;
                }

                Clients.Add(client);
            }           

            return Clients;
        }

        public static void SelectRecipient(ClientListItem client)
        {
            SetRecipient(client);
            WindowsManager.CloseWindow<WhomToTransfer>();
        }

        #endregion

        #region WhereToTransferVM

        public static ObservableCollection<Account> FillAccountsWithoutSenderAccount
            (ObservableCollection<Account> Accounts)
        {
            List<Account> accounts = AccountManager.GetAccounts(Recipient.Id);

            foreach (Account account in accounts)
            {
                if (Recipient.Id == Sender.Id)
                {
                    if (account.Id == SenderAccount.Id)
                    {
                        continue;
                    }
                }

                Accounts.Add(account);
            }

            return Accounts;
        }

        public static void SelectRecipientAccount(Account account)
        {
            SetRecipientAccount(account);
            WindowsManager.CloseWindow<WhereToTransfer>();
        }

        #endregion

        #region ChangeLogVM

        private static string GenerateChangeLogFilePath(string id)
        {
            string filePath = $"ChangeLog{id}.txt";
            return filePath;
        }

        public static ObservableCollection<string> FillChangeLog
            (ObservableCollection<string> ChangeLog)
        {
            List<string> records = GetChangeLog(CurrentClient.Id);

            foreach (string record in records)
            {
                if (CurrentUser.ViewingIsAllowed == false)
                {
                    if (record.Contains("Серия паспорта"))
                    {
                        ChangeLog.Add("Серия паспорта");
                        continue;
                    }

                    if (record.Contains("Номер паспорта"))
                    {
                        ChangeLog.Add("Номер паспорта");
                        continue;
                    }                   
                }
                
                ChangeLog.Add(record);
            }

            return ChangeLog;
        }

        public static List<string> GetChangeLog(string clientId)
        {
            List<string> changeLog = new List<string>();

            string filePath = GenerateChangeLogFilePath(clientId);

            bool status = WorkingWithFile.CheckBeforeReading(filePath);

            if (status == true)
            {
                changeLog = WorkingWithFile.GetRecords(filePath);
            }

            return changeLog;
        }        

        public static void WriteChangeLog(List<string> changeLog, string id)
        {
            string filePath = GenerateChangeLogFilePath(id);

            changeLog.Add($"Сотрудник: {CurrentUser.Position}");
            changeLog.Add($"Дата и время операции: {DateTime.Now}");
            changeLog.Add(String.Empty);

            WorkingWithFile.OverwriteOrWriteToFile(changeLog, filePath, true);
        }

        #endregion        
    }
}
