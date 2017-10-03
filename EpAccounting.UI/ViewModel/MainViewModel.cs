// ///////////////////////////////////
// File: MainViewModel.cs
// Last Change: 18.09.2017  20:48
// Author: Andre Multerer
// ///////////////////////////////////



namespace EpAccounting.UI.ViewModel
{
    using System;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Xml;
    using System.Xml.Linq;
    using EpAccounting.Business;
    using EpAccounting.Model;
    using EpAccounting.Model.Enum;
    using EpAccounting.UI.Properties;
    using EpAccounting.UI.Service;
    using GalaSoft.MvvmLight.Messaging;



    public class MainViewModel : BindableViewModelBase
    {
        #region Fields

        private readonly IRepository repository;
        private readonly IDialogService dialogService;

        private bool _canChangeWorkspace = true;

        private WorkspaceViewModel _currentWorkspace;
        private ClientViewModel _clientWorkspace;
        private BillViewModel _billWorkspace;
        private ArticlesOptionViewModel _articleWorkspace;
        private OptionViewModel _optionWorkspace;

        #endregion



        #region Constructors / Destructor

        public MainViewModel(IRepository repository, IDialogService dialogService)
        {
            this.repository = repository;
            this.dialogService = dialogService;

            this.TryConnectingAtStartup();
            this.InitWorkspaces();

            Messenger.Default.Register<NotificationMessage>(this, this.ExecuteNotificationMessage);
            Messenger.Default.Register<NotificationMessage<bool>>(this, this.ExecuteNotificationMessage);
        }

        #endregion



        #region Properties

        public bool IsConnected
        {
            get { return this.repository.IsConnected; }
        }

        public bool CanChangeWorkspace
        {
            get { return this._canChangeWorkspace; }
            private set { this.SetProperty(ref this._canChangeWorkspace, value); }
        }

        #endregion



        private void TryConnectingAtStartup()
        {
            try
            {
                // TODO: can be deleted after testing is finished
                DatabaseFactory.DeleteTestFolderAndFile();
                DatabaseFactory.CreateTestFile();
                DatabaseFactory.SetSavedFilePath();
                this.repository.LoadDatabase(Settings.Default.DatabaseFilePath);
                this.LoadXmlData();

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

        private void UpdateConnectionState()
        {
            this.RaisePropertyChanged(() => this.IsConnected);
        }

        private void ChangeToBillWorkspace()
        {
            this.CurrentWorkspace = this.BillWorkspace;
        }



        #region Messenger

        private void ExecuteNotificationMessage(NotificationMessage message)
        {
            if (message.Notification == Resources.Message_UpdateConnectionStateForMainVM)
            {
                this.UpdateConnectionState();
            }
            else if (message.Notification == Resources.Message_ChangeToBillWorkspaceForMainVM)
            {
                this.ChangeToBillWorkspace();
            }
            else if (message.Notification == Resources.Message_CreateNewBillMessageForMainVM)
            {
                this.CurrentWorkspace = this.WorkspaceViewModels.First(x => x.Title == Resources.Workspace_Title_Bills);
            }
            else if (message.Notification == Resources.Message_ChangeToBillWorkspaceForMainVM)
            {
                this.CurrentWorkspace = this.WorkspaceViewModels.First(x => x.Title == Resources.Workspace_Title_Bills);
            }
        }

        private void ExecuteNotificationMessage(NotificationMessage<bool> message)
        {
            if (message.Notification == Resources.Message_WorkspaceEnableStateForMainVM)
            {
                this.CanChangeWorkspace = message.Content;
            }
        }

        #endregion



        #region Workspaces

        public ObservableCollection<WorkspaceViewModel> WorkspaceViewModels { get; private set; }

        public WorkspaceViewModel CurrentWorkspace
        {
            get { return this._currentWorkspace; }
            set { this.SetProperty(ref this._currentWorkspace, value); }
        }

        private ClientViewModel ClientWorkspace
        {
            get
            {
                if (this._clientWorkspace == null)
                {
                    this._clientWorkspace = new ClientViewModel(Resources.Workspace_Title_Clients, Resources.img_clients,
                                                                this.repository, this.dialogService);
                }

                return this._clientWorkspace;
            }
        }

        private BillViewModel BillWorkspace
        {
            get
            {
                if (this._billWorkspace == null)
                {
                    this._billWorkspace = new BillViewModel(Resources.Workspace_Title_Bills, Resources.img_bills,
                                                            this.repository, this.dialogService);
                }

                return this._billWorkspace;
            }
        }

        private ArticlesOptionViewModel ArticleWorkspace
        {
            get
            {
                if (this._articleWorkspace == null)
                {
                    this._articleWorkspace = new ArticlesOptionViewModel(Resources.Workspace_Title_Articles, Resources.img_articles,
                                                                         this.repository, this.dialogService);
                }

                return this._articleWorkspace;
            }
        }

        private OptionViewModel OptionWorkspace
        {
            get
            {
                if (this._optionWorkspace == null)
                {
                    this._optionWorkspace = new OptionViewModel(Resources.Workspace_Title_Options, Resources.img_options,
                                                                this.repository, this.dialogService);
                }

                return this._optionWorkspace;
            }
        }

        private void InitWorkspaces()
        {
            this.WorkspaceViewModels = new ObservableCollection<WorkspaceViewModel>
                                       {
                                           this.ClientWorkspace,
                                           this.BillWorkspace,
                                           this.ArticleWorkspace,
                                           this.OptionWorkspace
                                       };

            this.CurrentWorkspace = this.ClientWorkspace;
        }

        #endregion



        #region TestData 

        // TODO: region can be commented out after testing is finished
        private void LoadXmlData()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            string resourceName = "EpAccounting.UI.Resources.testdata.xml";

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

                        this.repository.SaveOrUpdate(client);
                    }

                    foreach (XElement articleElement in rootElement.Elements("Article"))
                    {
                        Article article = this.CreateXmlArticle(articleElement);
                        this.repository.SaveOrUpdate(article);
                    }
                }
            }
        }

        private Client CreateXmlClient(XElement clientElement)
        {
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
                                Title = (ClientTitle)Enum.Parse(typeof(ClientTitle), title, true),
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
            string kindOfBill = billElement.Attribute("KindOfBill")?.Value.Trim();
            string kindOfVAT = billElement.Attribute("KindOfVAT")?.Value.Trim();
            double VATPercentage = Convert.ToDouble(billElement.Attribute("VatPercentage")?.Value.Trim());
            string date = billElement.Attribute("Date")?.Value.Trim();

            Bill bill = new Bill
                        {
                            // ReSharper disable once AssignNullToNotNullAttribute
                            KindOfBill = (KindOfBill)Enum.Parse(typeof(KindOfBill), kindOfBill, true),

                            // ReSharper disable once AssignNullToNotNullAttribute
                            KindOfVat = (KindOfVat)Enum.Parse(typeof(KindOfVat), kindOfVAT, true),
                            VatPercentage = VATPercentage,
                            Date = date
                        };

            return bill;
        }

        private BillItem CreateXmlBillItem(XElement billDetailElement)
        {
            int position = Convert.ToInt32(billDetailElement.Attribute("Position")?.Value.Trim());
            int articleNumber = Convert.ToInt32(billDetailElement.Attribute("ArticleNumber")?.Value.Trim());
            string description = billDetailElement.Attribute("Description")?.Value.Trim();
            double amount = Convert.ToDouble(billDetailElement.Attribute("Amount")?.Value.Trim());
            decimal price = Convert.ToDecimal(billDetailElement.Attribute("Price")?.Value.Trim());
            double discount = Convert.ToDouble(billDetailElement.Attribute("Discount")?.Value.Trim());

            BillItem billItem = new BillItem
                                {
                                    Position = position,
                                    ArticleNumber = articleNumber,
                                    Description = description,
                                    Amount = amount,
                                    Price = price,
                                    Discount = discount
                                };

            return billItem;
        }

        private Article CreateXmlArticle(XElement articleElement)
        {
            int articleNumber = Convert.ToInt32(articleElement.Attribute("ArticleNumber")?.Value.Trim());
            string description = articleElement.Attribute("Description")?.Value.Trim();
            double amount = Convert.ToDouble(articleElement.Attribute("Amount")?.Value.Trim());
            decimal price = Convert.ToDecimal(articleElement.Attribute("Price")?.Value.Trim());

            Article article = new Article
                              {
                                  ArticleNumber = articleNumber,
                                  Description = description,
                                  Amount = amount,
                                  Price = price
                              };

            return article;
        }

        #endregion
    }
}