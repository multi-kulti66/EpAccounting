// ///////////////////////////////////
// File: ViewModelLocator.cs
// Last Change: 13.03.2017  21:37
// Author: Andre Multerer
// ///////////////////////////////////



namespace EpAccounting.UI.ViewModel
{
    using EpAccounting.Business;
    using EpAccounting.Data;
    using EpAccounting.UI.Service;
    using GalaSoft.MvvmLight.Ioc;
    using Microsoft.Practices.ServiceLocation;



    public class ViewModelLocator
    {
        #region Constructors / Destructor

        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            SimpleIoc.Default.Register<ISessionManager, NHibernateSessionManager>();
            SimpleIoc.Default.Register<IRepository, NHibernateRepository>();
            SimpleIoc.Default.Register<IDialogService, DialogService>();
            SimpleIoc.Default.Register<MainViewModel>();
        }

        #endregion



        #region Properties

        public MainViewModel Main
        {
            get { return ServiceLocator.Current.GetInstance<MainViewModel>(); }
        }

        #endregion



        public static void Cleanup()
        {
            // Clear the ViewModels
        }
    }
}