using AutoMapper;
using Entities;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Security.Cryptography;
using WebAPI.ViewModel.Balances;
using WebAPI.ViewModel.Clients;
using WebAPI.ViewModel.ManageClients;
using WebAPI.ViewModel.Users;

namespace WebAPI.Services
{
    public class ClientService : IClientService
    {
        public readonly AmdDBContext _dbContext;
        public readonly IMapper _mapper;
        private readonly IStringLocalizer<SharedResource> _sharedResourceLocalizer;
        public ClientService(AmdDBContext dbContext, IMapper mapper, IStringLocalizer<SharedResource> sharedResourceLocalizer)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _sharedResourceLocalizer = sharedResourceLocalizer;
        }

        public async Task<ResponseResult<List<GetClientsVM>>> GetClients(long userId, string search, int userType)
        {
            bool isSearch = !string.IsNullOrEmpty(search);
            List<User> clients = new List<User>();
            ResponseResult<List<GetClientsVM>> result = new ResponseResult<List<GetClientsVM>>();
            if ((int)UserType.Admin == userType || (int)UserType.SuperiorAdmin == userType)
            {
                clients = _dbContext.Users.Where(u => (u.Type == (int)UserType.Client)).OrderBy(x => x.Name).ToList();
            }
            else
            {
                clients = _dbContext.Users.Where(u => (u.Type == (int)UserType.Client) && u.Id == userId).OrderBy(x => x.Name).ToList();
            }


            if (isSearch)
            {
                search = search.ToLower();
                clients = clients.Where(m => ((m.Name != null) ? m.Name.ToLower().Contains(search) : false)
                                        || ((m.UserName != null) ? m.UserName.ToLower().Contains(search) : false)
                                        || ((m.Address != null) ? m.Address.ToLower().Contains(search) : false)
                                        || ((m.Mobile != null) ? m.Mobile.ToLower().Contains(search) : false))
                    .ToList();
            }

            result.Data = _mapper.Map<List<User>, List<GetClientsVM>>(clients);
            result.Errors = null;
            result.Status = StatusType.Success;
            return result;
        }

        public async Task<ResponseResult<List<GetClientsVM>>> SaveClient(User model, bool editPassword, long loggedInUser)
        {

            ResponseResult<List<GetClientsVM>> result = new ResponseResult<List<GetClientsVM>>();
            result.Status = StatusType.Failed;
            bool isSaved = false;

            var userByName = await _dbContext.Users.FirstOrDefaultAsync(u => u.Name.ToLower() == model.Name && u.Id != model.Id);

            if (userByName != null)
            {
                result.Data = null;
                result.Errors = new List<string> { _sharedResourceLocalizer["NameAlreadyUsed"] };
                result.Status = StatusType.Failed;
                return result;
            }

            var userByUserName = await _dbContext.Users.FirstOrDefaultAsync(u => u.UserName.ToLower() == model.UserName && u.Id != model.Id);

            if (userByUserName != null)
            {
                result.Data = null;
                result.Errors = new List<string> { _sharedResourceLocalizer["Usernamealreadyexist"] };
                result.Status = StatusType.Failed;
                return result;
            }


            if (model.Id < 1)
            {
                model.ActualPass = model.Password;
                model.Password = EncryptPassword(model.Password);
                model.CreationDate = DateTime.Now;
                model.CreationUserId = loggedInUser;
                _dbContext.Users.Add(model);
            }
            else
            {
                model.ActualPass = model.Password;
                model.Password = EncryptPassword(model.Password);
                model.ModificationDate = DateTime.Now;
                model.ModificationUserId = loggedInUser;
                try
                {
                    _dbContext.Users.Update(model);
                }
                catch (Exception ex)
                {
                    ;
                }
            }

            isSaved = await _dbContext.SaveChangesAsync() > 0;

            if (isSaved)
            {
                result = new ResponseResult<List<GetClientsVM>>();  //await GetClients(0, null, (int)UserType.Admin);
                result.Errors = null;
                result.Status = StatusType.Success;
            }
            return result;
        }

        public async Task<ResponseResult<List<GetClientsVM>>> Delete(List<User> models)
        {
            ResponseResult<List<GetClientsVM>> result = new ResponseResult<List<GetClientsVM>>();
            result.Status = StatusType.Failed;
            bool isSaved = false;

            try
            {
                List<User> users = new List<User>();
                foreach (User model in models)
                {
                    var user = await _dbContext.Users.FindAsync(model.Id);
                    users.Add(user);
                }

                _dbContext.Users.RemoveRange(users);
                isSaved = await _dbContext.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                ;
            }

            if (isSaved)
            {
                result = new ResponseResult<List<GetClientsVM>>(); //await GetClients(0, null, (int)UserType.Admin);
                result.Errors = null;
                result.Status = StatusType.Success;
            }
            else
            {
                result.Data = null;
                result.Errors = new List<string> { _sharedResourceLocalizer["Cannotdeletethisclientbecauseithasrelateddata"] };
                result.Status = StatusType.Failed;
            }

            return result;
        }

        public async Task<ResponseResult<ClientPaymentDetails>> GetClientDetails(long clientId, string search)
        {
            ResponseResult<ClientPaymentDetails> result = new ResponseResult<ClientPaymentDetails>();
            string clientName = _dbContext.Users.Where(u => u.Id == clientId).FirstOrDefault().Name;
            var payments = await _dbContext.Payments.Include(x => x.Auto).ThenInclude(x => x.Container).Include(x => x.PaymentDetails).Where(p => p.Auto.BuyerId == clientId).OrderBy(x => x.Auto.BuyDate).ToListAsync();
            var paymentDetails = await _dbContext.PaymentDetails.Include(x => x.Payment).ThenInclude(x => x.Auto).ThenInclude(x => x.Container).Where(p => p.Payment.Auto.BuyerId == clientId).ToListAsync();
            List<LookupValue> lookupValues = _dbContext.LookupValues.ToList();
            bool isSearch = !string.IsNullOrEmpty(search);

            var groupedPayments = payments.GroupBy(g => g.AutoId).ToList();

            var details = groupedPayments.Select(p => new Details
            {
                Total = paymentDetails.Where(pd => (pd.Payment.PaymentType == PaymentType.Debit) && ((p.Key == pd.Payment.AutoId))).Sum(x => x.Amount),
                Payed = paymentDetails.Where(pd => (pd.Payment.PaymentType == PaymentType.Credit) && (p.Key == pd.Payment.AutoId)).Sum(x => x.Amount),
                Required = paymentDetails.Where(pd => (pd.Payment.PaymentType == PaymentType.Debit) && (p.Key == pd.Payment.AutoId)).Sum(x => x.Amount) - paymentDetails.Where(pd => (pd.Payment.PaymentType == PaymentType.Credit) && (p.Key == pd.Payment.AutoId)).Sum(x => x.Amount),
                Commission = paymentDetails.Where(pd => (pd.BuyType == BuyType.Commission) && (p.Key == pd.Payment.AutoId) && (pd.Payment.PaymentType == PaymentType.Debit)).Sum(x => x.Amount),
                CustomsClearance = paymentDetails.Where(pd => (pd.BuyType == BuyType.CustomsClearance) && (p.Key == pd.Payment.AutoId) && (pd.Payment.PaymentType == PaymentType.Debit)).Sum(x => x.Amount),
                PurchaseOrder = paymentDetails.Where(pd => (pd.BuyType == BuyType.PurchaseOrder) && (p.Key == pd.Payment.AutoId) && (pd.Payment.PaymentType == PaymentType.Debit)).Sum(x => x.Amount),
                Fees = paymentDetails.Where(pd => (pd.BuyType == BuyType.Fees) && (p.Key == pd.Payment.AutoId) && (pd.Payment.PaymentType == PaymentType.Debit)).Sum(x => x.Amount),
                InnerFreight = paymentDetails.Where(pd => (pd.BuyType == BuyType.InnerFreight) && (p.Key == pd.Payment.AutoId) && (pd.Payment.PaymentType == PaymentType.Debit)).Sum(x => x.Amount),
                Other = paymentDetails.Where(pd => (pd.BuyType == BuyType.Other) && (p.Key == pd.Payment.AutoId) && (pd.Payment.PaymentType == PaymentType.Debit)).Sum(x => x.Amount),
                PurchasePrice = paymentDetails.Where(pd => (pd.BuyType == BuyType.PurchasePrice) && (p.Key == pd.Payment.AutoId) && (pd.Payment.PaymentType == PaymentType.Debit)).Sum(x => x.Amount),
                SeaFreight = paymentDetails.Where(pd => (pd.BuyType == BuyType.SeaFreight) && (p.Key == pd.Payment.AutoId) && (pd.Payment.PaymentType == PaymentType.Debit)).Sum(x => x.Amount),
                StorageFees = paymentDetails.Where(pd => (pd.BuyType == BuyType.StorageFees) && (p.Key == pd.Payment.AutoId) && (pd.Payment.PaymentType == PaymentType.Debit)).Sum(x => x.Amount),
                ArrivalDate = (p.Where(i => i.AutoId == p.Key).FirstOrDefault().Auto.Container != null) ? p.Where(i => i.AutoId == p.Key).FirstOrDefault().Auto.Container.ArrivalDate : null,
                BuyDate = p.Where(i => i.AutoId == p.Key).FirstOrDefault().Auto.BuyDate,
                Container = (p.Where(i => i.AutoId == p.Key).FirstOrDefault().Auto.Container != null) ? p.Where(i => i.AutoId == p.Key).FirstOrDefault().Auto.Container.SerialNumber : null,
                Lot = p.Where(i => i.AutoId == p.Key).FirstOrDefault().Auto.Lot,
                Vin = p.Where(i => i.AutoId == p.Key).FirstOrDefault().Auto.VinNo,
                Car = lookupValues.Where(l => l.Id == p.Where(c => c.AutoId == p.Key).FirstOrDefault().Auto.BrandId).FirstOrDefault().Name + " " + p.Where(i => i.AutoId == p.Key).FirstOrDefault().Auto.Name + " " + p.Where(i => i.AutoId == p.Key).FirstOrDefault().Auto.Model,
                City = lookupValues.Where(l => l.Id == p.Where(c => c.AutoId == p.Key).FirstOrDefault().Auto.CityId).FirstOrDefault().Name,
                Destination = lookupValues.Where(l => l.Id == p.Where(c => c.AutoId == p.Key).FirstOrDefault().Auto.DestinationId).FirstOrDefault().Name,
                LoadPort = lookupValues.Where(l => l.Id == p.Where(c => c.AutoId == p.Key).FirstOrDefault().Auto.LoadPortId).FirstOrDefault().Name
            }).ToList();

            var header = new Header
            {
                Total = details.Sum(d => d.Total),
                Payed = details.Sum(d => d.Payed),
                Required = details.Sum(d => d.Required),
                Commission = details.Sum(d => d.Commission),
                CustomsClearance = details.Sum(d => d.CustomsClearance),
                PurchaseOrder = details.Sum(d => d.PurchaseOrder),
                Fees = details.Sum(d => d.Fees),
                InnerFreight = details.Sum(d => d.InnerFreight),
                Other = details.Sum(d => d.Other),
                PurchasePrice = details.Sum(d => d.PurchasePrice),
                SeaFreight = details.Sum(d => d.SeaFreight),
                StorageFees = details.Sum(d => d.StorageFees),
            };

            var footer = new Footer
            {
                Total = details.Sum(d => d.Total),
                Payed = details.Sum(d => d.Payed),
                Required = details.Sum(d => d.Required),
                Commission = details.Sum(d => d.Commission),
                CustomsClearance = details.Sum(d => d.CustomsClearance),
                PurchaseOrder = details.Sum(d => d.PurchaseOrder),
                Fees = details.Sum(d => d.Fees),
                InnerFreight = details.Sum(d => d.InnerFreight),
                Other = details.Sum(d => d.Other),
                PurchasePrice = details.Sum(d => d.PurchasePrice),
                SeaFreight = details.Sum(d => d.SeaFreight),
                StorageFees = details.Sum(d => d.StorageFees),
            };

            if (isSearch)
            {
                search = search.ToLower();
                details = details.Where(m => ((m.Car != null) ? m.Car.ToLower().Contains(search) : false)
                                        || ((m.Container != null) ? m.Container.ToLower().Contains(search) : false)
                                        || ((m.Lot != null) ? m.Lot.ToLower().Contains(search) : false)
                                        || ((m.Vin != null) ? m.Vin.ToLower().Contains(search) : false))
                    .ToList();

                footer = new Footer
                {
                    Total = details.Sum(d => d.Total),
                    Payed = details.Sum(d => d.Payed),
                    Required = details.Sum(d => d.Required),
                    Commission = details.Sum(d => d.Commission),
                    CustomsClearance = details.Sum(d => d.CustomsClearance),
                    PurchaseOrder = details.Sum(d => d.PurchaseOrder),
                    Fees = details.Sum(d => d.Fees),
                    InnerFreight = details.Sum(d => d.InnerFreight),
                    Other = details.Sum(d => d.Other),
                    PurchasePrice = details.Sum(d => d.PurchasePrice),
                    SeaFreight = details.Sum(d => d.SeaFreight),
                    StorageFees = details.Sum(d => d.StorageFees),
                };
            }

            result.Data = new ClientPaymentDetails
            {
                Header = header,
                Details = details,
                Footer = footer,
                ClientName = clientName
            };

            result.Errors = null;
            result.Status = StatusType.Success;


            return result;
        }

        public async Task<ResponseResult<ClientInfo>> GetClientsInfo(GetBalancesInput input, long UserId)
        {
            ResponseResult<ClientInfo> result = new ResponseResult<ClientInfo>();
            try
            {
                List<Auto> autos = await _dbContext.Autos
                .Include(c => c.Buyer)
                .Include(c => c.Payments).ThenInclude(x => x.PaymentDetails)
                .Where(c =>  (c.CreationUserId == UserId || c.BuyerId == UserId)) // c.IsArchive == 0 &&
                .OrderByDescending(c => c.BuyDate)
                .ToListAsync();

                if (input.IsSearch)
                {
                    if (!string.IsNullOrEmpty(input.VinNo)) autos = autos.Where(m => m.VinNo.Contains(input.VinNo)).ToList();
                    if (input.PurchaseDate.HasValue) autos = autos.Where(m => m.BuyDate.ToShortDateString() == input.PurchaseDate.Value.ToShortDateString()).ToList();
                    if (input.AutoId.HasValue && input.AutoId != 0) autos = autos.Where(m => m.Id == input.AutoId).ToList();
                    if (input.ClientId.HasValue && input.ClientId != 0) autos = autos.Where(m => m.BuyerId == input.ClientId).ToList();
                }

                var autoslist = autos.Select(c => new InfoDetails
                {
                    Id = c.Id,
                    Name = c.Name,
                    VinNo = c.VinNo,
                    BuyDateStr = c.BuyDate.ToString("dd/MM/yyyy", new CultureInfo("en-US")),
                    BuyerName = c.Buyer.Name,
                    Total = c.Payments.Where(p => p.PaymentType == PaymentType.Debit).FirstOrDefault()?.PaymentDetails.Sum(pd => pd.Amount),
                    Paid = c.Payments.Where(p => p.PaymentType == PaymentType.Credit).FirstOrDefault()?.PaymentDetails.Sum(pd => pd.Amount),
                    Required = c.Payments.Where(p => p.PaymentType == PaymentType.Debit).FirstOrDefault()?.PaymentDetails.Sum(pd => pd.Amount) - c.Payments.Where(p => p.PaymentType == PaymentType.Credit).FirstOrDefault()?.PaymentDetails.Sum(pd => pd.Amount),
                    Commission = c.Payments.Where(p => p.PaymentType == PaymentType.Credit).FirstOrDefault()?.PaymentDetails.Where(pd => pd.BuyType == BuyType.Commission).Sum(x => x.Amount),
                    CustomsClearance = c.Payments.Where(p => p.PaymentType == PaymentType.Credit).FirstOrDefault()?.PaymentDetails.Where(pd => pd.BuyType == BuyType.CustomsClearance).Sum(x => x.Amount),
                    PurchaseOrder = c.Payments.Where(p => p.PaymentType == PaymentType.Credit).FirstOrDefault()?.PaymentDetails.Where(pd => pd.BuyType == BuyType.PurchaseOrder).Sum(x => x.Amount),
                    Fees = c.Payments.Where(p => p.PaymentType == PaymentType.Credit).FirstOrDefault()?.PaymentDetails.Where(pd => pd.BuyType == BuyType.Fees).Sum(x => x.Amount),
                    InnerFreight = c.Payments.Where(p => p.PaymentType == PaymentType.Credit).FirstOrDefault()?.PaymentDetails.Where(pd => pd.BuyType == BuyType.InnerFreight).Sum(x => x.Amount),
                    Other = c.Payments.Where(p => p.PaymentType == PaymentType.Credit).FirstOrDefault()?.PaymentDetails.Where(pd => pd.BuyType == BuyType.Other).Sum(x => x.Amount),
                    PurchasePrice = c.Payments.Where(p => p.PaymentType == PaymentType.Credit).FirstOrDefault()?.PaymentDetails.Where(pd => pd.BuyType == BuyType.PurchasePrice).Sum(x => x.Amount),
                    SeaFreight = c.Payments.Where(p => p.PaymentType == PaymentType.Credit).FirstOrDefault()?.PaymentDetails.Where(pd => pd.BuyType == BuyType.SeaFreight).Sum(x => x.Amount),
                    StorageFees = c.Payments.Where(p => p.PaymentType == PaymentType.Credit).FirstOrDefault()?.PaymentDetails.Where(pd => pd.BuyType == BuyType.StorageFees).Sum(x => x.Amount)
                }).ToList();

                autoslist.ForEach(c => c.Required = (c.Total - (c.Paid.HasValue ? c.Paid.Value : 0)));


                var Summation = new InfoFooter();
                var openSummation = new InfoFooter();
                var paidSummation = new InfoFooter();

                if (autoslist.Count > 0)
                {
                    Summation = new InfoFooter
                    {
                        Total = autoslist.Sum(d => d.Total != null ? d.Total.Value : 0),
                        Paid = autoslist.Sum(d => d.Paid != null ? d.Paid.Value : 0),
                        Required = autoslist.Sum(d => d.Required != null ? d.Required.Value : 0),
                        Commission = autoslist.Sum(d => d.Commission != null ? d.Commission.Value : 0),
                        CustomsClearance = autoslist.Sum(d => d.CustomsClearance != null ? d.CustomsClearance.Value : 0),
                        PurchaseOrder = autoslist.Sum(d => d.PurchaseOrder != null ? d.PurchaseOrder.Value : 0),
                        Fees = autoslist.Sum(d => d.Fees != null ? d.Fees.Value : 0),
                        InnerFreight = autoslist.Sum(d => d.InnerFreight != null ? d.InnerFreight.Value : 0),
                        Other = autoslist.Sum(d => d.Other != null ? d.Other.Value : 0),
                        PurchasePrice = autoslist.Sum(d => d.PurchasePrice != null ? d.PurchasePrice.Value : 0),
                        SeaFreight = autoslist.Sum(d => d.SeaFreight != null ? d.SeaFreight.Value : 0),
                        StorageFees = autoslist.Sum(d => d.StorageFees != null ? d.StorageFees.Value : 0),
                    };
                }

                var openAutos = autoslist.Where(c => (c.Required == null || c.Required > 0) && (c.Total == null || c.Total != 0)).ToList();
                if (openAutos.Count > 0)
                {
                    openSummation = new InfoFooter
                    {
                        Total = openAutos.Sum(d => d.Total != null ? d.Total.Value : 0),
                        Paid = openAutos.Sum(d => d.Paid != null ? d.Paid.Value : 0),
                        Required = openAutos.Sum(d => d.Required != null ? d.Required.Value : 0),
                        Commission = openAutos.Sum(d => d.Commission != null ? d.Commission.Value : 0),
                        CustomsClearance = openAutos.Sum(d => d.CustomsClearance != null ? d.CustomsClearance.Value : 0),
                        PurchaseOrder = openAutos.Sum(d => d.PurchaseOrder != null ? d.PurchaseOrder.Value : 0),
                        Fees = openAutos.Sum(d => d.Fees != null ? d.Fees.Value : 0),
                        InnerFreight = openAutos.Sum(d => d.InnerFreight != null ? d.InnerFreight.Value : 0),
                        Other = openAutos.Sum(d => d.Other != null ? d.Other.Value : 0),
                        PurchasePrice = openAutos.Sum(d => d.PurchasePrice != null ? d.PurchasePrice.Value : 0),
                        SeaFreight = openAutos.Sum(d => d.SeaFreight != null ? d.SeaFreight.Value : 0),
                        StorageFees = openAutos.Sum(d => d.StorageFees != null ? d.StorageFees.Value : 0),
                    };
                }

                var paidAutos = autoslist.Where(c => c.Required == 0 && c.Total != 0).ToList();

                if (paidAutos.Count > 0)
                {
                    paidSummation = new InfoFooter
                    {
                        Total = paidAutos.Sum(d => d.Total != null ? d.Total.Value : 0),
                        Paid = paidAutos.Sum(d => d.Paid != null ? d.Paid.Value : 0),
                        Required = paidAutos.Sum(d => d.Required != null ? d.Required.Value : 0),
                        Commission = paidAutos.Sum(d => d.Commission != null ? d.Commission.Value : 0),
                        CustomsClearance = paidAutos.Sum(d => d.CustomsClearance != null ? d.CustomsClearance.Value : 0),
                        PurchaseOrder = paidAutos.Sum(d => d.PurchaseOrder != null ? d.PurchaseOrder.Value : 0),
                        Fees = paidAutos.Sum(d => d.Fees != null ? d.Fees.Value : 0),
                        InnerFreight = paidAutos.Sum(d => d.InnerFreight != null ? d.InnerFreight.Value : 0),
                        Other = paidAutos.Sum(d => d.Other != null ? d.Other.Value : 0),
                        PurchasePrice = paidAutos.Sum(d => d.PurchasePrice != null ? d.PurchasePrice.Value : 0),
                        SeaFreight = paidAutos.Sum(d => d.SeaFreight != null ? d.SeaFreight.Value : 0),
                        StorageFees = paidAutos.Sum(d => d.StorageFees != null ? d.StorageFees.Value : 0),
                    };
                }
                result.Data = new ClientInfo
                {
                    AutoDetails = autoslist,
                    Summation = Summation,
                    OpenAutos = openAutos,
                    PaidAutos = paidAutos,
                    OpenSummation = openSummation,
                    PaidSummation = paidSummation

                };
                result.Errors = null;
                result.Status = StatusType.Success;

            }
            catch (Exception ex)
            {
                ;
            }

            return result;
        }

        public async Task<ResponseResult<ClientBalance>> GetClientsBalance(string search, long userId, UserType type)
        {
            ResponseResult<ClientBalance> result = new ResponseResult<ClientBalance>();
            try
            {
                List<User> clients = new List<User>();
                clients = await _dbContext.Users.Where(u => u.Type == (int)UserType.Client && (type == UserType.Admin ? true : u.Id == userId)).ToListAsync();
                var autos = await _dbContext.Autos.ToListAsync();
                var paymentDetails = await _dbContext.PaymentDetails.Include(x => x.Payment).ThenInclude(x => x.Auto).ThenInclude(x => x.Container).ToListAsync();
                bool isSearch = !string.IsNullOrEmpty(search);

                var details = clients.Select(p => new AutoDetails
                {
                    Id = p.Id,
                    Name = p.Name,
                    CarCount = autos.Where(a => a.BuyerId == p.Id).Count(),
                    Total = paymentDetails.Where(pd => (pd.Payment.PaymentType == PaymentType.Debit) && ((pd.Payment.Auto.BuyerId == p.Id))).Sum(x => x.Amount),
                    Paid = paymentDetails.Where(pd => (pd.Payment.PaymentType == PaymentType.Credit) && (pd.Payment.Auto.BuyerId == p.Id)).Sum(x => x.Amount),
                    Required = paymentDetails.Where(pd => (pd.Payment.PaymentType == PaymentType.Debit) && (pd.Payment.Auto.BuyerId == p.Id)).Sum(x => x.Amount) - paymentDetails.Where(pd => (pd.Payment.PaymentType == PaymentType.Credit) && (pd.Payment.Auto.BuyerId == p.Id)).Sum(x => x.Amount),
                    Commission = paymentDetails.Where(pd => (pd.BuyType == BuyType.Commission) && (pd.Payment.Auto.BuyerId == p.Id) && (pd.Payment.PaymentType == PaymentType.Debit)).Sum(x => x.Amount),
                    CustomsClearance = paymentDetails.Where(pd => (pd.BuyType == BuyType.CustomsClearance) && (pd.Payment.Auto.BuyerId == p.Id) && (pd.Payment.PaymentType == PaymentType.Debit)).Sum(x => x.Amount),
                    PurchaseOrder = paymentDetails.Where(pd => (pd.BuyType == BuyType.PurchaseOrder) && (pd.Payment.Auto.BuyerId == p.Id) && (pd.Payment.PaymentType == PaymentType.Debit)).Sum(x => x.Amount),
                    Fees = paymentDetails.Where(pd => (pd.BuyType == BuyType.Fees) && (pd.Payment.Auto.BuyerId == p.Id) && (pd.Payment.PaymentType == PaymentType.Debit)).Sum(x => x.Amount),
                    InnerFreight = paymentDetails.Where(pd => (pd.BuyType == BuyType.InnerFreight) && (pd.Payment.Auto.BuyerId == p.Id) && (pd.Payment.PaymentType == PaymentType.Debit)).Sum(x => x.Amount),
                    Other = paymentDetails.Where(pd => (pd.BuyType == BuyType.Other) && (pd.Payment.Auto.BuyerId == p.Id) && (pd.Payment.PaymentType == PaymentType.Debit)).Sum(x => x.Amount),
                    PurchasePrice = paymentDetails.Where(pd => (pd.BuyType == BuyType.PurchasePrice) && (pd.Payment.Auto.BuyerId == p.Id) && (pd.Payment.PaymentType == PaymentType.Debit)).Sum(x => x.Amount),
                    SeaFreight = paymentDetails.Where(pd => (pd.BuyType == BuyType.SeaFreight) && (pd.Payment.Auto.BuyerId == p.Id) && (pd.Payment.PaymentType == PaymentType.Debit)).Sum(x => x.Amount),
                    StorageFees = paymentDetails.Where(pd => (pd.BuyType == BuyType.StorageFees) && (pd.Payment.Auto.BuyerId == p.Id) && (pd.Payment.PaymentType == PaymentType.Debit)).Sum(x => x.Amount),
                })
                    .OrderBy(x => x.Name)
                    .ToList();

                var footer = new AutoFooter
                {
                    Total = details.Sum(d => d.Total),
                    Paid = details.Sum(d => d.Paid),
                    Required = details.Sum(d => d.Required),
                    Commission = details.Sum(d => d.Commission),
                    CustomsClearance = details.Sum(d => d.CustomsClearance),
                    PurchaseOrder = details.Sum(d => d.PurchaseOrder),
                    Fees = details.Sum(d => d.Fees),
                    InnerFreight = details.Sum(d => d.InnerFreight),
                    Other = details.Sum(d => d.Other),
                    PurchasePrice = details.Sum(d => d.PurchasePrice),
                    SeaFreight = details.Sum(d => d.SeaFreight),
                    StorageFees = details.Sum(d => d.StorageFees),
                };

                if (isSearch)
                {
                    search = search.ToLower();
                    details = details.Where(m => ((m.Name != null) ? m.Name.ToLower().Contains(search) : false))
                        .ToList();

                    footer = new AutoFooter
                    {
                        Total = details.Sum(d => d.Total),
                        Paid = details.Sum(d => d.Paid),
                        Required = details.Sum(d => d.Required),
                        Commission = details.Sum(d => d.Commission),
                        CustomsClearance = details.Sum(d => d.CustomsClearance),
                        PurchaseOrder = details.Sum(d => d.PurchaseOrder),
                        Fees = details.Sum(d => d.Fees),
                        InnerFreight = details.Sum(d => d.InnerFreight),
                        Other = details.Sum(d => d.Other),
                        PurchasePrice = details.Sum(d => d.PurchasePrice),
                        SeaFreight = details.Sum(d => d.SeaFreight),
                        StorageFees = details.Sum(d => d.StorageFees),
                    };
                }

                result.Data = new ClientBalance
                {
                    AutoDetails = details,
                    AutoFooter = footer
                };
                result.Errors = null;
                result.Status = StatusType.Success;

            }
            catch (Exception ex)
            {
                ;
            }

            return result;
        }

        public async Task<ResponseResult<GetClientsVM>> GetClientById(long Id)
        {
            ResponseResult<GetClientsVM> result = new ResponseResult<GetClientsVM>();
            result.Status = StatusType.Failed;

            try
            {
                var user = await _dbContext.Users.FindAsync(Id);
                var mapuser = _mapper.Map<User, GetClientsVM>(user);
                result.Data = mapuser;
                result.Errors = null;
                result.Status = StatusType.Success;
            }
            catch (Exception ex)
            {
                ;
            }
            return result;

        }
        private bool ValidatePassword(string password, User user)
        {
            string savedPasswordHash = user.Password;
            byte[] hashBytes = Convert.FromBase64String(savedPasswordHash);
            byte[] salt = new byte[16];
            Array.Copy(hashBytes, 0, salt, 0, 16);

            var pdkdf2 = new Rfc2898DeriveBytes(password, salt, 10000);
            byte[] hash = pdkdf2.GetBytes(20);

            int ok = 1;
            for (int i = 0; i < 20; i++)
            {
                if (hashBytes[i + 16] != hash[i])
                    ok = 0;
            }
            if (ok == 1)
            {
                return true;
            }

            return false;
        }

        private string EncryptPassword(string password)
        {
            byte[] salt;
            new RNGCryptoServiceProvider().GetBytes(salt = new byte[16]);

            var pdk = new Rfc2898DeriveBytes(password, salt, 10000);
            byte[] hash = pdk.GetBytes(20);
            byte[] hashBytes = new byte[36];

            Array.Copy(salt, 0, hashBytes, 0, 16);
            Array.Copy(hash, 0, hashBytes, 16, 20);

            string savedPasswordHash = Convert.ToBase64String(hashBytes);

            return savedPasswordHash;
        }

        public async Task<ResponseResult<bool>> UpdatePassword(long UserId, string Password)
        {
            ResponseResult<bool> result = new ResponseResult<bool>();
            result.Status = StatusType.Failed;

            try
            {
                var user = await _dbContext.Users.FindAsync(UserId);
                user.ActualPass = Password;
                user.Password = EncryptPassword(Password);
                _dbContext.Users.Update(user);
                await _dbContext.SaveChangesAsync();

                result.Data = true;
                result.Errors = null;
                result.Status = StatusType.Success;
            }
            catch (Exception ex)
            {
                ;
            }
            return result;
        }
    }
}
