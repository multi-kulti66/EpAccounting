// ///////////////////////////////////
// File: SettingBindingExtension.cs
// Last Change: 12.07.2017  16:26
// Author: Andre Multerer
// ///////////////////////////////////



namespace EpAccounting.UI.Markup
{
    using System.Windows.Data;
    using EpAccounting.UI.Properties;



    public class SettingBindingExtension : Binding
    {
        #region Methods

        private void Initialize()
        {
            this.Source = Settings.Default;
            this.Mode = BindingMode.TwoWay;
        }

        #endregion Methods



        #region Constructors

        public SettingBindingExtension()
        {
            this.Initialize();
        }

        public SettingBindingExtension(string path)
            : base(path)
        {
            this.Initialize();
        }

        #endregion Constructors
    }
}