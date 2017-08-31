// ///////////////////////////////////
// File: ClientMap.cs
// Last Change: 16.08.2017  18:26
// Author: Andre Multerer
// ///////////////////////////////////



namespace EpAccounting.Model.Mapping
{
    using FluentNHibernate.Mapping;



    public class ClientMap : ClassMap<Client>
    {
        #region Constructors / Destructor

        public ClientMap()
        {
            this.Id(x => x.ClientId);
            this.Map(x => x.Title);
            this.Map(x => x.FirstName);
            this.Map(x => x.LastName);
            this.Map(x => x.Street);
            this.Map(x => x.HouseNumber);
            this.Map(x => x.PostalCode);
            this.Map(x => x.City);
            this.Map(x => x.DateOfBirth);
            this.Map(x => x.PhoneNumber1);
            this.Map(x => x.PhoneNumber2);
            this.Map(x => x.MobileNumber);
            this.Map(x => x.Telefax);
            this.Map(x => x.Email);
            this.HasMany(x => x.Bills).Cascade.All().Not.LazyLoad();
        }

        #endregion
    }
}