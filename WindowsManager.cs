using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using MessageBox = System.Windows.MessageBox;
using MessageBoxOptions = System.Windows.MessageBoxOptions;
using Window = System.Windows.Window;

namespace ExceptionsLibrariesExtensions
{
    public class WindowsManager
    {
        public static double WorkingAreaWidth { get; private set; }
        public static double WorkingAreaHeight { get; private set; }

        public static ClientListVM CurrentClientListVM { get; set; }
        public static ClientFormsVM CurrentClientFormsVM { get; set; }
        public static TransferFormVM CurrentTransferFormVM { get; set; }
        public static NotificationVM CurrentNotificationVM { get; set; }

        private static Window CurrentClientForm = null;
        private static Window CurrentAccountForm = null;        
        private static Window CurrentCreatingAccountForm = null;
        private static Window CurrentCreatingClientForm = null;
        private static Window CurrentTransferForm = null;
        private static Window CurrentReplenishAccountForm = null;

        public static Window GetAndOpenWindow<T>()
            where T : Window, new()
        {
            T window = new T();
            window.Show();            

            return window;
        }

        public static Window GetCurrentWindow<T>()
            where T : Window, new()
        {
            Window window = new Window();

            foreach (Window win in App.Current.Windows)
            {
                if (win is T)
                {
                    window = win;
                    break;
                }
            }

            return window;
        }        

        public static void CloseWindow<T>()
            where T : Window, new()
        {
            foreach (Window window in App.Current.Windows)
            {
                if (window is T)
                {
                    window.Close();
                    break;
                }
            }
        }

        public static void ActivateWindow<T>()
            where T : Window, new()
        {
            foreach (Window window in App.Current.Windows)
            {
                if (window is T)
                {
                    window.Activate();
                    break;
                }
            }
        }

        #region MessageBox

        public static void CallErrorMessageBox(string text)
        {
            MessageBox.Show(text,
                        "Операция не выполнена",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error,
                        MessageBoxResult.OK,
                        MessageBoxOptions.DefaultDesktopOnly);
        }

        #endregion

        #region Notification

        private static async Task ClosingTasks(Window window)
        {
            await Task.Delay(2500);
            window.Close();
        }

        private static async void CloseWithDelay(Window window)
        {
            await ClosingTasks(window);
        }

        public static void CallNotification(string text)
        {
            Notification window = new Notification();
            CurrentNotificationVM.Message = text;
            window.Left = WorkingAreaWidth - window.Width;
            window.Top = WorkingAreaHeight - window.Height;
            window.Show();
            CloseWithDelay(window);
        }

        #endregion

        #region AuthorizationForm

        public static void DefineWorkingAreaSize()
        {
            WorkingAreaWidth = SystemInformation.WorkingArea.Width;
            WorkingAreaHeight = SystemInformation.WorkingArea.Height;
        }

        #endregion

        #region ClientList

        public static void ClientList_Closed(object sender, EventArgs e)
        {
            foreach (Window window in App.Current.Windows)
            {
                window.Close();
            }
        }

        public static void OpenClientList()
        {
            Window window = GetAndOpenWindow<ClientList>();
            window.Closed += ClientList_Closed;
        }

        #endregion

        #region CreatingClientForm

        public static void CreatingClientForm_Closed(object sender, EventArgs e)
        {
            ProgramManager.ResetCurrentOrgForm();
            CurrentCreatingClientForm = null;
        }

        public static void OpenCreatingClientForm()
        {
            if (CurrentCreatingClientForm == null)
            {
                if (CurrentClientForm == null)
                {
                    Window window = GetAndOpenWindow<CreatingClientForm>();
                    Window owner = GetCurrentWindow<ClientList>();
                    window.Owner = owner;
                    window.Closed += CreatingClientForm_Closed;
                    CurrentCreatingClientForm = window;
                }
                else
                {                    
                    CurrentClientForm.WindowState = WindowState.Normal;
                }                
            }
            else
            {                
                CurrentCreatingClientForm.WindowState = WindowState.Normal;
            }
        }

        #endregion

        #region ClientForm

        public static void ClientForm_Closed(object sender, EventArgs e)
        {
            ActivateWindow<ClientList>();
            ProgramManager.RefreshClientList();
            ProgramManager.ResetCurrentClient();
            CurrentClientForm = null;
        }        

        public static void OpenClientForm(string orgForm)
        {
            if (CurrentClientForm == null)
            {
                Window window = (orgForm == ProgramManager.jp.OrgFormJP) ?
                GetAndOpenWindow<JuridicalPersonForm>() :
                GetAndOpenWindow<PhysicalPersonForm>();

                Window owner = GetCurrentWindow<ClientList>();                

                window.Owner = owner;
                window.Closed += ClientForm_Closed;
                CurrentClientForm = window;
            }
            else
            {                
                CurrentClientForm.WindowState = WindowState.Normal;
            }            
        }

        public static void CloseClientForm(string orgForm)
        {
            if (orgForm == ProgramManager.jp.OrgFormJP)
            {
                CloseWindow<JuridicalPersonForm>();
            }
            else
            {
                CloseWindow<PhysicalPersonForm>();
            }            
        }

        #endregion

        #region ChangeLog

        public static void OpenChangeLog()
        {
            Window window = GetAndOpenWindow<ChangeLog>();            

            Window owner = (ProgramManager.CurrentClient.OrgForm == ProgramManager.jp.OrgFormJP) ?
                GetCurrentWindow<JuridicalPersonForm>() :
                GetCurrentWindow<PhysicalPersonForm>();

            window.Owner = owner;
        }

        #endregion

        #region CreatingAccountForm

        public static void CreatingAccountForm_Closed(object sender, EventArgs e)
        {            
            ProgramManager.RefreshAccounts();
            ActivateWindow<ClientList>();
            CurrentCreatingAccountForm = null;
        }

        public static void OpenCreatingAccountForm()
        {
            if (CurrentCreatingAccountForm == null)
            {
                Window window = GetAndOpenWindow<CreatingAccountForm>();

                Window owner = (ProgramManager.CurrentClient.OrgForm == ProgramManager.jp.OrgFormJP) ?
                    GetCurrentWindow<JuridicalPersonForm>() :
                    GetCurrentWindow<PhysicalPersonForm>();

                window.Owner = owner;
                window.Closed += CreatingAccountForm_Closed;
                CurrentCreatingAccountForm = window;
            }
            else
            {                
                CurrentCreatingAccountForm.WindowState = WindowState.Normal;
            }
                        
        }

        #endregion

        #region AccountForm

        public static void AccountForm_Closed(object sender, EventArgs e)
        {
            ProgramManager.ResetCurrentAccount();
            CurrentAccountForm = null;
        }

        public static void OpenAccountForm()
        {
            if (CurrentAccountForm == null)
            {
                Window window = (ProgramManager.CurrentAccount.AccountType == ProgramManager.da.AccountType) ?
                GetAndOpenWindow<DepositAccountForm>() :
                GetAndOpenWindow<NonDepositAccountForm>();

                Window owner = (ProgramManager.CurrentClient.OrgForm == ProgramManager.jp.OrgFormJP) ?
                    GetCurrentWindow<JuridicalPersonForm>() :
                    GetCurrentWindow<PhysicalPersonForm>();

                window.Owner = owner;
                window.Closed += AccountForm_Closed;
                CurrentAccountForm = window;
            }
            else
            {
                CurrentAccountForm.WindowState = WindowState.Normal;
            }

        }

        public static void CloseAccountForm()
        {
            if (ProgramManager.CurrentAccount.AccountType == ProgramManager.da.AccountType)
            {
                CloseWindow<DepositAccountForm>();
            }
            else
            {
                CloseWindow<NonDepositAccountForm>();
            }
        }

        #endregion

        #region TransferForm

        public static void TransferForm_Closed(object sender, EventArgs e)
        {
            ProgramManager.ResetTransferInfo();
            ProgramManager.RefreshAccounts();
            ActivateWindow<ClientList>();
            CurrentTransferForm = null;
        }

        public static void OpenTransferForm()
        {
            if(CurrentTransferForm == null)
            {
                Window window = GetAndOpenWindow<TransferForm>();

                Window owner = (ProgramManager.CurrentAccount.AccountType == ProgramManager.da.AccountType) ?
                    GetCurrentWindow<DepositAccountForm>() :
                    GetCurrentWindow<NonDepositAccountForm>();

                window.Owner = owner;
                window.Closed += TransferForm_Closed;
                CurrentTransferForm = window;
            }
            else
            {                
                CurrentTransferForm.WindowState = WindowState.Normal;
            }
        }

        #endregion

        #region ReplenishAccountForm

        public static void ReplenishAccountForm_Closed(object sender, EventArgs e)
        {            
            ProgramManager.RefreshAccounts();
            ActivateWindow<ClientList>();
            CurrentReplenishAccountForm = null;
        }

        public static void OpenReplenishAccountForm()
        {
            if (CurrentReplenishAccountForm == null)
            {
                Window window = GetAndOpenWindow<ReplenishAccountForm>();

                Window owner = (ProgramManager.CurrentAccount.AccountType == ProgramManager.da.AccountType) ?
                    GetCurrentWindow<DepositAccountForm>() :
                    GetCurrentWindow<NonDepositAccountForm>();

                window.Owner = owner;
                window.Closed += ReplenishAccountForm_Closed;
                CurrentReplenishAccountForm = window;
            }
            else
            {                
                CurrentReplenishAccountForm.WindowState = WindowState.Normal;
            }
        }

        #endregion

        #region WhomToTransfer

        public static void WhomToTransfer_Closed(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty($"{ProgramManager.Recipient}"))
            {
                CurrentTransferFormVM.Whom = ProgramManager.Recipient.FullName;
                CurrentTransferFormVM.Where = String.Empty;
            }
        }

        public static void OpenWhomToTransfer()
        {
            Window window = GetAndOpenWindow<WhomToTransfer>();
            Window owner = GetCurrentWindow<TransferForm>();
            window.Owner = owner;
            window.Closed += WhomToTransfer_Closed;            
        }

        #endregion

        #region WhereToTransfer

        public static void WhereToTransfer_Closed(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty($"{ProgramManager.RecipientAccount}"))
            {
                CurrentTransferFormVM.Where = ProgramManager.RecipientAccount.AccountNumber;
            }
        }

        public static void OpenWhereToTransfer()
        {
            Window window = GetAndOpenWindow<WhereToTransfer>();
            Window owner = GetCurrentWindow<TransferForm>();
            window.Owner = owner;
            window.Closed += WhereToTransfer_Closed;
        }

        #endregion         
    }
}
