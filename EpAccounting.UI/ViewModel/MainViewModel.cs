// ///////////////////////////////////
// File: MainViewModel.cs
// Last Change: 14.08.2017  09:39
// Author: Andre Multerer
// ///////////////////////////////////



namespace EpAccounting.UI.ViewModel
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Xml;
    using System.Xml.Linq;
    using EpAccounting.Business;
    using EpAccounting.Model;
    using EpAccounting.UI.Properties;
    using EpAccounting.UI.Service;
    using GalaSoft.MvvmLight.Messaging;



    public class MainViewModel : BindableViewModelBase
    {
        #region Fields

        private readonly IRepository repository;
        private readonly IDialogService dialogService;

        private WorkspaceViewModel _currentWorkspaceViewModel;

        #endregion



        #region Constructors / Destructor

        public MainViewModel(IRepository repository, IDialogService dialogService)
        {
            this.repository = repository;
            this.dialogService = dialogService;

            this.InitWorkspaces();
            this.CurrentWorkspaceViewModel = this.WorkspaceViewModels[0];
            this.TryConnectingAtStartup();

            Messenger.Default.Register<NotificationMessage>(this, this.ExecuteNotificationMessage);
        }

        #endregion



        #region Properties

        public bool IsConnected
        {
            get { return this.repository.IsConnected; }
        }

        public WorkspaceViewModel CurrentWorkspaceViewModel
        {
            get { return this._currentWorkspaceViewModel; }
            set { this.SetProperty(ref this._currentWorkspaceViewModel, value); }
        }

        public ObservableCollection<WorkspaceViewModel> WorkspaceViewModels { get; private set; }

        #endregion



        private void InitWorkspaces()
        {
            this.WorkspaceViewModels = new ObservableCollection<WorkspaceViewModel>
                                       {
                                           new ClientViewModel(Resources.Workspace_Title_Clients, Resources.img_clients, this.repository, this.dialogService),
                                           new BillViewModel(Resources.Workspace_Title_Bills, Resources.img_bills, this.repository, this.dialogService),
                                           new OptionViewModel(Resources.Workspace_Title_Options, Resources.img_options, this.repository, this.dialogService)
                                       };
        }

        private void TryConnectingAtStartup()
        {
            try
            {
                /* new section */
                DatabaseFactory.DeleteTestFolderAndFile();
                DatabaseFactory.CreateTestFile();
                DatabaseFactory.SetSavedFilePath();
                this.repository.LoadDatabase(Settings.Default.DatabaseFilePath);
                this.LoadXmlData();

                foreach (Client client in this.clients)
                {
                    this.repository.SaveOrUpdate(client);
                }
                /* new section */

                // TODO: change back
                //this.repository.LoadDatabase(Settings.Default.DatabaseFilePath);
            }
            catch
            {
                // just try to connect at beginning
                // do not throw exception when no database path was saved
                Settings.Default.DatabaseFilePath = string.Empty;
                Settings.Default.Save();
            }
        }

        private void ExecuteNotificationMessage(NotificationMessage message)
        {
            if (message.Notification == Resources.Messenger_Message_UpdateConnectionStateMessageForMainVM)
            {
                this.UpdateConnectionState();
            }
            else if (message.Notification == Resources.Messenger_Message_CreateNewBillMessageForMainVM)
            {
                this.CurrentWorkspaceViewModel = this.WorkspaceViewModels.First(x => x.Title == Resources.Workspace_Title_Bills);
            }
        }

        private void UpdateConnectionState()
        {
            this.RaisePropertyChanged(() => this.IsConnected);
        }



        #region TestData

        private readonly List<Client> clients = new List<Client>();

        private void LoadXmlData()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            string resourceName = "EpAccounting.UI.Resources.TestData.xml";

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            {
                // ReSharper disable once AssignNullToNotNullAttribute
                using (XmlReader reader = new XmlTextReader(stream))
                {
                    XDocument xDoc = XDocument.Load(reader);

                    // ReSharper disable once PossibleNullReferenceException
                    XElement rootElement = xDoc.Document.Root;

                    // ReSharper disable once PossibleNullReferenceException
                    foreach (XElement clientElement in rootElement.Elements("Client"))
                    {
                        Client client = this.CreateXmlClient(clientElement);

                        foreach (XElement billElement in clientElement.Elements("Bill"))
                        {
                            Bill bill = this.CreateXmlBill(billElement);

                            foreach (XElement billDetailElement in billElement.Elements("BillItem"))
                            {
                                BillItem billItem = this.CreateXmlBillItem(billDetailElement);
                                billItem.Bill = bill;
                                bill.AddBillItem(billItem);
                            }

                            bill.Client = client;
                            client.AddBill(bill);
                        }

                        this.clients.Add(client);
                    }
                }
            }

            Client testClient = new Client();

            for (int i = 0; i < 100; i++)
            {
                Bill bill = new Bill();
                testClient.AddBill(bill);
            }

            this.clients.Add(testClient);
        }

        private Client CreateXmlClient(XElement clientElement)
        {
            int clientID = Convert.ToInt32(clientElement.Attribute("ClientId")?.Value.Trim());
            string title = clientElement.Attribute("Title")?.Value.Trim();
            string firstName = clientElement.Attribute("FirstName")?.Value.Trim();
            string lastName = clientElement.Attribute("LastName")?.Value.Trim();
            string street = clientElement.Attribute("Street")?.Value.Trim();
            string houseNumber = clientElement.Attribute("HouseNumber")?.Value.Trim();
            string postalCode = clientElement.Attribute("PostalCode")?.Value.Trim();
            string city = clientElement.Attribute("City")?.Value.Trim();
            string dateOfBirth = clientElement.Attribute("DateOfBirth")?.Value.Trim();
            string phoneNumber1 = clientElement.Attribute("PhoneNumber1")?.Value.Trim();
            string phoneNumber2 = clientElement.Attribute("PhoneNumber2")?.Value.Trim();
            string mobileNumber = clientElement.Attribute("MobileNumber")?.Value.Trim();
            string telefax = clientElement.Attribute("Telefax")?.Value.Trim();
            string email = clientElement.Attribute("Email")?.Value.Trim();

            Client client = new Client
                            {
                                ClientId = clientID,
                                Title = title,
                                FirstName = firstName,
                                LastName = lastName,
                                Street = street,
                                HouseNumber = houseNumber,
                                PostalCode = postalCode,
                                City = city,
                                DateOfBirth = dateOfBirth,
                                PhoneNumber1 = phoneNumber1,
                                PhoneNumber2 = phoneNumber2,
                                MobileNumber = mobileNumber,
                                Telefax = telefax,
                                Email = email
                            };

            return client;
        }

        private Bill CreateXmlBill(XElement billElement)
        {
            int billID = Convert.ToInt32(billElement.Attribute("BillId")?.Value.Trim());
            string kindOfBill = billElement.Attribute("KindOfBill")?.Value.Trim();
            string kindOfVAT = billElement.Attribute("KindOfVat")?.Value.Trim();
            double VATPercentage = Convert.ToDouble(billElement.Attribute("VatPercentage")?.Value.Trim());
            string date = billElement.Attribute("Date")?.Value.Trim();

            Bill bill = new Bill
                        {
                            BillId = billID,
                            KindOfBill = kindOfBill,
                            KindOfVat = kindOfVAT,
                            VatPercentage = VATPercentage,
                            Date = date
                        };

            return bill;
        }

        private BillItem CreateXmlBillItem(XElement billDetailElement)
        {
            int billDetailID = Convert.ToInt32(billDetailElement.Attribute("Id")?.Value.Trim());
            int position = Convert.ToInt32(billDetailElement.Attribute("Position")?.Value.Trim());
            int articleNumber = Convert.ToInt32(billDetailElement.Attribute("ArticleNumber")?.Value.Trim());
            string description = billDetailElement.Attribute("Description")?.Value.Trim();
            double amount = Convert.ToDouble(billDetailElement.Attribute("Amount")?.Value.Trim());
            double price = Convert.ToDouble(billDetailElement.Attribute("Price")?.Value.Trim());
            double discount = Convert.ToDouble(billDetailElement.Attribute("Discount")?.Value.Trim());

            BillItem billItem = new BillItem
                                {
                                    Id = billDetailID,
                                    Position = position,
                                    ArticleNumber = articleNumber,
                                    Description = description,
                                    Amount = amount,
                                    Price = price,
                                    Discount = discount
                                };

            return billItem;
        }

        #endregion
    }
}