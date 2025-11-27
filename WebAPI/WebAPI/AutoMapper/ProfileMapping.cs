namespace WebAPI.AutoMapper
{
    public class ProfileMapping : Profile
    {
        public ProfileMapping()
        {
            // Clients
            CreateMap<User, GetClientsVM>();

            // Autos
            CreateMap<SaveAutoInput, Auto>()
                .ForMember(x => x.ColorId, c => c.MapFrom(s => s.Color));

            CreateMap<Auto, AutoNameVM>();

            // Lookups
            CreateMap<LookupValue, LookupValueVM>();


            // Users
            CreateMap<User, UserVM>();

            //Payment
            CreateMap<PaymentInput, Payment>();

            //
            CreateMap<PaymentInput, PaymentDetails>()
                .ForMember(x => x.Amount, c => c.MapFrom(s => s.amount))
                .ForMember(x => x.CashType, c => c.MapFrom(s => s.paymentMethod));


            //Resources
            CreateMap<Resources, GroupedResources>()
                .ForMember(s => s.Key, d => d.MapFrom(c => c.Key))
                .ForMember(s => s.Arabic, d => d.MapFrom(c => c.TextAr))
                .ForMember(s => s.English, d => d.MapFrom(c => c.TextEn))
                ;

            CreateMap<Resources,ResourcesKeysVM>();

            CreateMap<Auto, ClientBuyerNameVM>();
            CreateMap<Auto, UpdatePaymentStatusVM>();
        }
    }
}
