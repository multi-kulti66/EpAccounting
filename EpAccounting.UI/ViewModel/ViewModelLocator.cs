// ///////////////////////////////////
// File: ViewModelLocator.cs
// Last Change: 17.02.2018, 14:29
// Author: Andre Multerer
// ///////////////////////////////////

namespace EpAccounting.UI.ViewModel
{
    using Business;
    using Data;
    using GalaSoft.MvvmLight.Ioc;
    using Microsoft.Practices.ServiceLocation;
    using Service;


    public class ViewModelLocator
    {
        #region Constructors

        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            SimpleIoc.Default.Register<ISessionManager, NHibernateSessionManager>();
            SimpleIoc.Default.Register<IRepository, NHibernateRepository>();
            SimpleIoc.Default.Register<IDialogService, DialogService>();
            SimpleIoc.Default.Register<MainViewModel>();
        }

        #endregion



        #region Properties, Indexers

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