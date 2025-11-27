using Castle.Core.Resource;
using Entities;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace WebAPI.Services
{
    public class PaymentService : EnumExtension, IPaymentService
    {
        public readonly AmdDBContext _dbContext;
        public readonly IMapper _mapper;
        private readonly IStringLocalizer<SharedResource> _sharedResourceLocalizer;
        private PaymentDebeitCredit _PaymentDebeitCredit = new PaymentDebeitCredit();
        private IConfiguration _configuration;
        public PaymentService(AmdDBContext dbContext, IMapper mapper, IStringLocalizer<SharedResource> sharedResourceLocalizer, IConfiguration configuration)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _sharedResourceLocalizer = sharedResourceLocalizer;
            _configuration = configuration;
        }

        public async Task<ResponseResult<object>> GetPayment(long autoId)
        {
            decimal debitAmount = 0;
            decimal creditAmount = 0;
            ResponseResult<object> result = new ResponseResult<object>();
            List<PaymentDetails> paymentDetails = await _dbContext.PaymentDetails.Include(x => x.Payment).Where(c => c.Payment.AutoId == autoId).AsNoTracking().ToListAsync();
            List<LookupValue> lookupValues = _dbContext.LookupValues.ToList();
            if (paymentDetails.Count > 0)
            {
                debitAmount = paymentDetails.Where(c => c.Payment.PaymentType == PaymentType.Debit).Sum(x => x.Amount);
                creditAmount = paymentDetails.Where(c => c.Payment.PaymentType == PaymentType.Credit).Sum(x => x.Amount);
            }

            paymentDetails = paymentDetails.Select(c => new PaymentDetails
            {
                Id = c.Id,
                Amount = c.Amount,
                BuyType = c.BuyType,
                CashType = c.CashType,
                Notes = c.Notes,
                PayDate = c.PayDate,
                PayDateStr = c.PayDate.ToString("dd/MM/yyyy", new CultureInfo("en-US")),
                PaymentId = c.PaymentId,
                Payment = c.Payment,
                BuyTypeStr = SwitchBuyType((int)c.BuyType),
                CashTypeStr = _dbContext.LookupValues.FirstOrDefault(x => x.Id == c.CashType).Name,
                DebitAmount = debitAmount,
                PaymentType = (int)c.Payment.PaymentType
            }).OrderByDescending(c => c.PayDate).ToList();

            result.Data = new { paymentDetails, debitAmount, creditAmount };
            result.Errors = null;
            result.Status = StatusType.Success;

            return result;
        }

        public async Task<ResponseResult<object>> SavePaymentDetails(PaymentDetails model)
        {
            ResponseResult<object> result = new ResponseResult<object>();
            result.Status = StatusType.Failed;
            bool isSaved = false;

            model.PayDate = model.PayDate.ToLocalTime();
            if (model.Id < 1)
            {
                _dbContext.PaymentDetails.Add(model);
            }
            else
            {
                _dbContext.PaymentDetails.Update(model);
            }

            isSaved = await _dbContext.SaveChangesAsync() > 0;

            if (isSaved)
            {
                result.Data = null;
                result.Errors = null;
                result.Status = StatusType.Success;
            }

            return result;
        }

        public async Task<ResponseResult<object>> Delete(List<PaymentDetails> models)
        {
            ResponseResult<object> result = new ResponseResult<object>();
            result.Status = StatusType.Failed;
            bool isSaved = false;

            try
            {
                List<PaymentDetails> paymentDetails = new List<PaymentDetails>();
                foreach (PaymentDetails model in models)
                {
                    var paymentDet = await _dbContext.PaymentDetails.FindAsync(model.Id);
                    paymentDetails.Add(paymentDet);
                }
                _dbContext.PaymentDetails.RemoveRange(paymentDetails);
                isSaved = await _dbContext.SaveChangesAsync() > 0;
            }
            catch (Exception)
            {

                throw;
            }


            if (isSaved)
            {
                result.Data = null;
                result.Errors = null;
                result.Status = StatusType.Success;
            }

            return result;
        }

        public async Task<ResponseResult<object>> SavePayment(PaymentInput model, long UserId)
        {
            ResponseResult<object> result = new ResponseResult<object>();
            result.Status = StatusType.Failed;
            bool isSaved = false;


            decimal totalDebit = 0;
            decimal totalAmount = 0;

            var getTotal = await _dbContext.Payments.Include(c => c.PaymentDetails)
                             .Where(x => x.AutoId == model.autoId).ToListAsync();


            if (model.id < 1)
            {
                if (getTotal.Count > 0)
                {
                    foreach (var pay in getTotal)
                    {
                        if (pay.PaymentType == PaymentType.Debit)
                        {
                            foreach (var item in pay.PaymentDetails)
                            {
                                if (item.Id == model.id)
                                {
                                    item.Amount = model.amount;
                                }
                            }
                            totalDebit = pay.PaymentDetails.Sum(x => x.Amount);
                        }

                        if (pay.PaymentType == PaymentType.Credit)
                        {
                            foreach (var item in pay.PaymentDetails)
                            {
                                if (item.Id == model.id)
                                {
                                    item.Amount = model.amount;
                                }
                            }
                            totalAmount = pay.PaymentDetails.Sum(x => x.Amount);
                        }
                    }

                    if (model.paymentType == (int)PaymentType.Credit)
                    {
                        if (model.amount + totalAmount > totalDebit)
                        {
                            result.Data = model;
                            result.Errors = new List<string> { _sharedResourceLocalizer["Totalcreditmustnotexceedthetotaldebit"] };
                            result.Status = StatusType.Failed;
                            return result;
                        }
                    }
                }
                else
                {
                    if (model.paymentType == (int)PaymentType.Credit)
                    {
                        result.Data = model;
                        result.Errors = new List<string> { _sharedResourceLocalizer["Totalcreditmustnotexceedthetotaldebit"] }; ;
                        result.Status = StatusType.Failed;
                        return result;
                    }
                }

                var payment = await _dbContext.Payments.FirstOrDefaultAsync(x => x.AutoId == model.autoId
                                     && x.PaymentType == (PaymentType)model.paymentType);
                if (payment == null)
                {
                    try
                    {
                        var mapModel = _mapper.Map<PaymentInput, Payment>(model);

                        if (model.id < 1)
                        {
                            mapModel.CreationDate = DateTime.Now;
                            mapModel.CreationUserId = UserId;

                            _dbContext.Payments.Add(mapModel);
                            await _dbContext.SaveChangesAsync();

                            var mapDetails = _mapper.Map<PaymentInput, PaymentDetails>(model);
                            mapDetails.BuyType = (BuyType)model.categoryId;
                            mapDetails.PayDate = mapDetails.PayDate.ToLocalTime();
                            mapDetails.PaymentId = mapModel.Id;
                            _dbContext.PaymentDetails.Add(mapDetails);

                        }
                        else
                        {
                            mapModel.ModificationDate = DateTime.Now;
                            mapModel.ModificationUserId = UserId;
                            _dbContext.Payments.Update(mapModel);
                        }
                    }
                    catch (Exception ex)
                    {

                        throw;
                    }

                }
                else
                {
                    var mapDetails = _mapper.Map<PaymentInput, PaymentDetails>(model);
                    mapDetails.BuyType = (BuyType)model.categoryId;
                    mapDetails.PayDate = mapDetails.PayDate.ToLocalTime();
                    mapDetails.PaymentId = payment.Id;
                    _dbContext.PaymentDetails.Add(mapDetails);
                }
            }
            else
            {
                foreach (var pay in getTotal)
                {
                    if (pay.PaymentType == PaymentType.Debit)
                    {
                        foreach (var item in pay.PaymentDetails)
                        {
                            if (item.Id == model.id)
                            {
                                item.BuyType = (BuyType)model.categoryId;
                                item.PayDate = model.payDate.ToLocalTime();
                                item.Amount = model.amount;
                                item.CashType = model.paymentMethod;
                                item.Notes = model.notes;
                            }
                        }
                        totalDebit = pay.PaymentDetails.Sum(x => x.Amount);
                    }

                    if (pay.PaymentType == PaymentType.Credit)
                    {
                        foreach (var item in pay.PaymentDetails)
                        {
                            if (item.Id == model.id)
                            {
                                item.BuyType = (BuyType)model.categoryId;
                                item.PayDate = model.payDate.ToLocalTime();
                                item.Amount = model.amount;
                                item.CashType = model.paymentMethod;
                                item.Notes = model.notes;
                            }
                        }
                        totalAmount = pay.PaymentDetails.Sum(x => x.Amount);
                    }
                }

                if (totalAmount > totalDebit)
                {
                    result.Data = model;
                    result.Errors = new List<string> { _sharedResourceLocalizer["Totalcreditmustnotexceedthetotaldebit"] };
                    result.Status = StatusType.Failed;
                    return result;
                }
            }

            isSaved = await _dbContext.SaveChangesAsync() > 0;

            if (isSaved)
            {
                result.Data = model;
                result.Errors = null;
                result.Status = StatusType.Success;
            }

            return result;
        }

        #region Old Code

        public async Task<ResponseResult<object>> GetPaymentsOLD(GetPaymentsInput input, long UserId)
        {
            decimal debitAmount = 0;
            decimal creditAmount = 0;

            List<long> autos = await _dbContext.Autos
                .Include(c => c.Buyer)
                .Where(c => (c.CreationUserId == UserId || c.BuyerId == UserId)) // c.IsArchive == 0 &&
                .Select(c => c.Id)
                .ToListAsync();

            ResponseResult<object> result = new ResponseResult<object>();
            List<PaymentDetails> paymentDetails = await _dbContext.PaymentDetails
                .Include(x => x.Payment).ThenInclude(r => r.Auto).ThenInclude(b => b.Buyer)
                .Where(c => autos.Contains(c.Payment.AutoId)).ToListAsync();

            if (input.IsSearch)
            {
                if (!string.IsNullOrEmpty(input.VinNo)) paymentDetails = paymentDetails.Where(m => m.Payment.Auto.VinNo.Contains(input.VinNo)).ToList();
                if (input.PurchaseDate.HasValue) paymentDetails = paymentDetails.Where(m => m.PayDate.ToShortDateString() == input.PurchaseDate.Value.ToShortDateString()).ToList();
                if (input.AutoId.HasValue && input.AutoId != 0) paymentDetails = paymentDetails.Where(m => m.Payment.AutoId == input.AutoId).ToList();
                if (input.ClientId.HasValue && input.ClientId != 0) paymentDetails = paymentDetails.Where(m => m.Payment.Auto.BuyerId == input.ClientId).ToList();
            }

            if (paymentDetails.Count > 0)
            {
                debitAmount = paymentDetails.Where(c => c.Payment.PaymentType == PaymentType.Debit).Sum(x => x.Amount);
                creditAmount = paymentDetails.Where(c => c.Payment.PaymentType == PaymentType.Credit).Sum(x => x.Amount);
            }

            paymentDetails = paymentDetails.Select(c => new PaymentDetails
            {
                Id = c.Id,
                Amount = c.Amount,
                BuyType = c.BuyType,
                CashType = c.CashType,
                Notes = c.Notes,
                PayDate = c.PayDate,
                PayDateStr = c.PayDate.ToString("dd/MM/yyyy", new CultureInfo("en-US")),
                PaymentId = c.PaymentId,
                Payment = c.Payment,
                BuyTypeStr = SwitchBuyType((int)c.BuyType),
                CashTypeStr = _dbContext.LookupValues.FirstOrDefault(x => x.Id == c.CashType).Name,
                DebitAmount = debitAmount,
                AutoId = c.Payment.AutoId,
                VinNo = c.Payment.Auto.VinNo,
                Client = c.Payment.Auto.Buyer.Name,
                CarName = c.Payment.Auto.Name,
                PaymentType = (int)c.Payment.PaymentType
            }).OrderByDescending(x => x.PayDate).ToList();




            result.Data = new { paymentDetails, debitAmount, creditAmount };
            result.Errors = null;
            result.Status = StatusType.Success;

            return result;
        }
        public async Task<ResponseResult<object>> GetPaymentsOld2(GetPaymentsInput input, long UserId)
        {
            decimal debitAmount = 0;
            decimal creditAmount = 0;

            List<long> autos = await _dbContext.Autos
                .Include(c => c.Buyer)
                .Where(c => (c.CreationUserId == UserId || c.BuyerId == UserId)) // c.IsArchive == 0 &&
                .Select(c => c.Id)
                .ToListAsync();




            ResponseResult<object> result = new ResponseResult<object>();

            List<PaymentDetails> paymentDetails = new List<PaymentDetails>();
            var paymentDetails2 = await (from pd in _dbContext.PaymentDetails.Where(c => autos.Contains(c.Payment.AutoId))
                                         join p in _dbContext.Payments on pd.PaymentId equals p.Id
                                         join auto in _dbContext.Autos on p.AutoId equals auto.Id
                                         join buyer in _dbContext.Users on auto.BuyerId equals buyer.Id

                                         join lookup in _dbContext.LookupValues on pd.CashType equals lookup.Id

                                         select new
                                         {

                                             Id = pd.Id,
                                             Amount = pd.Amount,
                                             AutoId = pd.AutoId,
                                             BuyType = pd.BuyType,
                                             CarName = pd.CarName,
                                             CashType = pd.CashType,
                                             Client = pd.Client,
                                             DebitAmount = pd.DebitAmount,
                                             Notes = pd.Notes,
                                             PayDate = pd.PayDate,
                                             //PayDateStr = pd.PayDateStr,
                                             Payment = p,
                                             Auto = auto,
                                             Buyer = buyer,
                                             PaymentId = pd.PaymentId,
                                             VinNo = auto.VinNo,
                                             CashTypeStr = lookup.Name,

                                         }
                                ).ToListAsync();

            //if (paymentDetails2.Count > 0)
            //{
            //    debitAmount = paymentDetails2.Where(c => c.Payment.PaymentType == PaymentType.Debit).Sum(x => x.Amount);
            //    creditAmount = paymentDetails2.Where(c => c.Payment.PaymentType == PaymentType.Credit).Sum(x => x.Amount);
            //}

            foreach (var pd in paymentDetails2)
            {
                var pament = pd.Payment;
                pament.Auto = pd.Auto;
                pament.Auto.Buyer = pd.Buyer;
                paymentDetails.Add(new PaymentDetails
                {

                    Id = pd.Id,
                    Amount = pd.Amount,
                    BuyType = pd.BuyType,
                    CashType = pd.CashType,
                    Notes = pd.Notes,
                    PayDate = pd.PayDate,
                    PayDateStr = pd.PayDate.ToString("dd/MM/yyyy", new CultureInfo("en-US")),
                    PaymentId = pd.PaymentId,
                    Payment = pd.Payment,
                    BuyTypeStr = SwitchBuyType((int)pd.BuyType),
                    CashTypeStr = pd.CashTypeStr,
                    DebitAmount = debitAmount,
                    AutoId = pd.Payment.AutoId,
                    VinNo = pd.Payment.Auto.VinNo,
                    Client = pd.Payment.Auto.Buyer.Name,
                    CarName = pd.Payment.Auto.Name,
                    PaymentType = (int)pd.Payment.PaymentType

                });
            }



            if (input.IsSearch)
            {
                if (!string.IsNullOrEmpty(input.VinNo)) paymentDetails = paymentDetails.Where(m => m.Payment.Auto.VinNo.Contains(input.VinNo)).ToList();
                if (input.PurchaseDate.HasValue) paymentDetails = paymentDetails.Where(m => m.PayDate.ToShortDateString() == input.PurchaseDate.Value.ToShortDateString()).ToList();
                if (input.AutoId.HasValue && input.AutoId != 0) paymentDetails = paymentDetails.Where(m => m.Payment.AutoId == input.AutoId).ToList();
                if (input.ClientId.HasValue && input.ClientId != 0) paymentDetails = paymentDetails.Where(m => m.Payment.Auto.BuyerId == input.ClientId).ToList();

                if (string.IsNullOrEmpty(input.VinNo) && !input.PurchaseDate.HasValue
                    && (!input.AutoId.HasValue || input.AutoId == 0) && (!input.ClientId.HasValue || input.ClientId == 0))
                {
                    input.IsSearch = false;
                }


            }

            if (paymentDetails.Count > 0)
            {
                debitAmount = paymentDetails.Where(c => c.Payment.PaymentType == PaymentType.Debit).Sum(x => x.Amount);
                creditAmount = paymentDetails.Where(c => c.Payment.PaymentType == PaymentType.Credit).Sum(x => x.Amount);
            }
            paymentDetails.All(c => { c.DebitAmount = debitAmount; return true; });
            //paymentDetails = paymentDetails.Select(c => new PaymentDetails
            //{
            //    Id = c.Id,
            //    Amount = c.Amount,
            //    BuyType = c.BuyType,
            //    CashType = c.CashType,
            //    Notes = c.Notes,
            //    PayDate = c.PayDate,
            //    PayDateStr = c.PayDate.ToString("dd/MM/yyyy", new CultureInfo("en-US")),
            //    PaymentId = c.PaymentId,
            //    Payment = c.Payment,
            //    BuyTypeStr = SwitchBuyType((int)c.BuyType),
            //    CashTypeStr = _dbContext.LookupValues.FirstOrDefault(x => x.Id == c.CashType).Name,
            //    DebitAmount = debitAmount,
            //    AutoId = c.Payment.AutoId,
            //    VinNo = c.Payment.Auto.VinNo,
            //    Client = c.Payment.Auto.Buyer.Name,
            //    CarName = c.Payment.Auto.Name,
            //    PaymentType = (int)c.Payment.PaymentType
            //}).OrderByDescending(x => x.PayDate).ToList();


            if (!input.IsSearch)
            {
                paymentDetails = paymentDetails.Where(x => x.Payment.AutoId == autos.FirstOrDefault()).OrderByDescending(x => x.PayDate).ToList();
            }



            result.Data = new { paymentDetails, debitAmount, creditAmount };
            result.Errors = null;
            result.Status = StatusType.Success;

            return result;
        }
        #endregion


        public async Task<ResponseResult<object>> GetPayments(GetPaymentsInput input, long UserId)
        {
            decimal debitAmount = 0;
            decimal creditAmount = 0;

            List<long> autos = await _dbContext.Autos
                .Include(c => c.Buyer)
                .Where(c => (c.CreationUserId == UserId || c.BuyerId == UserId)) // c.IsArchive == 0 &&
                .Select(c => c.Id)
                .ToListAsync();




            ResponseResult<object> result = new ResponseResult<object>();

            List<PaymentDetails> paymentDetails = new List<PaymentDetails>();
            int numberOfRecords = Int32.Parse(_configuration["NumberOfRecords"].ToString());
            paymentDetails = await getList(input, autos, numberOfRecords);


            if (paymentDetails.Count > 0)
            {
                if (input.IsSearch)
                {
                    debitAmount = paymentDetails.Where(c => c.Payment.PaymentType == PaymentType.Debit).Sum(x => x.Amount);
                    creditAmount = paymentDetails.Where(c => c.Payment.PaymentType == PaymentType.Credit).Sum(x => x.Amount);
                }
                else
                {
                    debitAmount = _PaymentDebeitCredit.debitAmount;
                    creditAmount = _PaymentDebeitCredit.creditAmount;
                }
            }
            paymentDetails.All(c => { c.DebitAmount = debitAmount; return true; });



            //if (!input.IsSearch)
            //{
            //    paymentDetails = paymentDetails.Where(x => x.Payment.AutoId == autos.FirstOrDefault()).OrderByDescending(x => x.PayDate).ToList();
            //}



            result.Data = new { paymentDetails, debitAmount, creditAmount };
            result.Errors = null;
            result.Status = StatusType.Success;

            return result;
        }
        // comented bt qusay on 25/08/2024 change by hisham via whatsapp
        /* public async Task<List<PaymentDetails>> getList(GetPaymentsInput input,List<long> autos,int nubmerOfRecords=500)
         {
             decimal debitAmount = 0;
             decimal creditAmount = 0;
             try
             {
                 List<PaymentDetails> paymentDetails = new List<PaymentDetails>();
                 var paymentDetails2= new List<PaymentDetails2>();

                 if (input.IsSearch)
                 {
                     if (!string.IsNullOrEmpty(input.VinNo))
                     {
                         //paymentDetails =
                             //paymentDetails.Where(m => m.Payment.Auto.VinNo.Contains(input.VinNo)).ToList();
                             paymentDetails2 = await (from pd in _dbContext.PaymentDetails.Where(c => autos.Contains(c.Payment.AutoId) 
                                                      && c.Payment.Auto.VinNo.Contains(input.VinNo))
                                                      join p in _dbContext.Payments on pd.PaymentId equals p.Id
                                                      join auto in _dbContext.Autos on p.AutoId equals auto.Id
                                                      join buyer in _dbContext.Users on auto.BuyerId equals buyer.Id

                                                      join lookup in _dbContext.LookupValues on pd.CashType equals lookup.Id

                                                      select new PaymentDetails2
                                                      {

                                                          Id = pd.Id,
                                                          Amount = pd.Amount,
                                                          AutoId = pd.AutoId,
                                                          BuyType = pd.BuyType,
                                                          CarName = pd.CarName,
                                                          CashType = pd.CashType,
                                                          Client = pd.Client,
                                                          DebitAmount = pd.DebitAmount,
                                                          Notes = pd.Notes,
                                                          PayDate = pd.PayDate,
                                                          //PayDateStr = pd.PayDateStr,
                                                          Payment = p,
                                                          Auto = auto,
                                                          Buyer = buyer,
                                                          PaymentId = pd.PaymentId,
                                                          VinNo = auto.VinNo,
                                                          CashTypeStr = lookup.Name,

                                                      }
                                          ).OrderByDescending(x => x.VinNo).ThenByDescending(x => x.PayDate).ToListAsync();
                     }
                     if (input.PurchaseDate.HasValue)
                     {
                      //   paymentDetails =
                               //paymentDetails.Where(m => m.PayDate.ToShortDateString() == input.PurchaseDate.Value.ToShortDateString()).ToList();
                               paymentDetails2 = await (from pd in _dbContext.PaymentDetails.Where(c => autos.Contains(c.Payment.AutoId)
                                                      && c.PayDate.ToShortDateString() == input.PurchaseDate.Value.ToShortDateString())
                                                        join p in _dbContext.Payments on pd.PaymentId equals p.Id
                                                        join auto in _dbContext.Autos on p.AutoId equals auto.Id
                                                        join buyer in _dbContext.Users on auto.BuyerId equals buyer.Id

                                                        join lookup in _dbContext.LookupValues on pd.CashType equals lookup.Id

                                                        select new PaymentDetails2
                                                        {

                                                            Id = pd.Id,
                                                            Amount = pd.Amount,
                                                            AutoId = pd.AutoId,
                                                            BuyType = pd.BuyType,
                                                            CarName = pd.CarName,
                                                            CashType = pd.CashType,
                                                            Client = pd.Client,
                                                            DebitAmount = pd.DebitAmount,
                                                            Notes = pd.Notes,
                                                            PayDate = pd.PayDate,
                                                            //PayDateStr = pd.PayDateStr,
                                                            Payment = p,
                                                            Auto = auto,
                                                            Buyer = buyer,
                                                            PaymentId = pd.PaymentId,
                                                            VinNo = auto.VinNo,
                                                            CashTypeStr = lookup.Name,

                                                        }
                                         ).OrderByDescending(x => x.VinNo).ThenByDescending(x => x.PayDate).ToListAsync();
                     }
                     if (input.AutoId.HasValue && input.AutoId != 0)
                     {
                        // paymentDetails =
                              //paymentDetails.Where(m => m.Payment.AutoId == input.AutoId).ToList();
                              paymentDetails2 = await (from pd in _dbContext.PaymentDetails.Where(c=>autos.Contains(c.Payment.AutoId)
                                                      && c.Payment.AutoId == input.AutoId)
                                                       join p in _dbContext.Payments on pd.PaymentId equals p.Id
                                                       join auto in _dbContext.Autos on p.AutoId equals auto.Id
                                                       join buyer in _dbContext.Users on auto.BuyerId equals buyer.Id

                                                       join lookup in _dbContext.LookupValues on pd.CashType equals lookup.Id

                                                       select new PaymentDetails2
                                                       {

                                                           Id = pd.Id,
                                                           Amount = pd.Amount,
                                                           AutoId = pd.AutoId,
                                                           BuyType = pd.BuyType,
                                                           CarName = pd.CarName,
                                                           CashType = pd.CashType,
                                                           Client = pd.Client,
                                                           DebitAmount = pd.DebitAmount,
                                                           Notes = pd.Notes,
                                                           PayDate = pd.PayDate,
                                                           //PayDateStr = pd.PayDateStr,
                                                           Payment = p,
                                                           Auto = auto,
                                                           Buyer = buyer,
                                                           PaymentId = pd.PaymentId,
                                                           VinNo = auto.VinNo,
                                                           CashTypeStr = lookup.Name,

                                                       }
                                          ).OrderByDescending(x => x.VinNo).ThenByDescending(x => x.PayDate).ToListAsync();
                     }
                     if (input.ClientId.HasValue && input.ClientId != 0)
                     {
                         //paymentDetails =
                              //paymentDetails.Where(m => m.Payment.Auto.BuyerId == input.ClientId).ToList();
                              paymentDetails2 = await (from pd in _dbContext.PaymentDetails.Where(c => autos.Contains(c.Payment.AutoId)
                                                      && c.Payment.Auto.BuyerId == input.ClientId)
                                                       join p in _dbContext.Payments on pd.PaymentId equals p.Id
                                                       join auto in _dbContext.Autos on p.AutoId equals auto.Id
                                                       join buyer in _dbContext.Users on auto.BuyerId equals buyer.Id

                                                       join lookup in _dbContext.LookupValues on pd.CashType equals lookup.Id

                                                       select new PaymentDetails2
                                                       {

                                                           Id = pd.Id,
                                                           Amount = pd.Amount,
                                                           AutoId = pd.AutoId,
                                                           BuyType = pd.BuyType,
                                                           CarName = pd.CarName,
                                                           CashType = pd.CashType,
                                                           Client = pd.Client,
                                                           DebitAmount = pd.DebitAmount,
                                                           Notes = pd.Notes,
                                                           PayDate = pd.PayDate,
                                                           //PayDateStr = pd.PayDateStr,
                                                           Payment = p,
                                                           Auto = auto,
                                                           Buyer = buyer,
                                                           PaymentId = pd.PaymentId,
                                                           VinNo = auto.VinNo,
                                                           CashTypeStr = lookup.Name,

                                                       }
                                        ).OrderByDescending(x => x.VinNo).ThenByDescending(x => x.PayDate).ToListAsync();
                     }
                     if (string.IsNullOrEmpty(input.VinNo) && !input.PurchaseDate.HasValue
                         && (!input.AutoId.HasValue || input.AutoId == 0) && (!input.ClientId.HasValue || input.ClientId == 0))
                     {
                         input.IsSearch = false;
                     }


                 }
                 else if(!input.IsSearch)
                 {

                     paymentDetails2 = await (from pd in _dbContext.PaymentDetails.Where(c => autos.Contains(c.Payment.AutoId)).Take(nubmerOfRecords)
                                              join p in _dbContext.Payments on pd.PaymentId equals p.Id
                                              join auto in _dbContext.Autos on p.AutoId equals auto.Id
                                              join buyer in _dbContext.Users on auto.BuyerId equals buyer.Id

                                              join lookup in _dbContext.LookupValues on pd.CashType equals lookup.Id

                                              select new PaymentDetails2
                                              {

                                                  Id = pd.Id,
                                                  Amount = pd.Amount,
                                                  AutoId = pd.AutoId,
                                                  BuyType = pd.BuyType,
                                                  CarName = pd.CarName,
                                                  CashType = pd.CashType,
                                                  Client = pd.Client,
                                                  DebitAmount = pd.DebitAmount,
                                                  Notes = pd.Notes,
                                                  PayDate = pd.PayDate,
                                                  //PayDateStr = pd.PayDateStr,
                                                  Payment = p,
                                                  Auto = auto,
                                                  Buyer = buyer,
                                                  PaymentId = pd.PaymentId,
                                                  VinNo = auto.VinNo,
                                                  CashTypeStr = lookup.Name,

                                              }
                                        ).OrderByDescending(x => x.PayDate).ToListAsync();

                              _PaymentDebeitCredit.debitAmount = _dbContext.PaymentDetails.Where(c => autos.Contains(c.Payment.AutoId) && c.Payment.PaymentType == PaymentType.Debit).Sum(x => x.Amount);
                     _PaymentDebeitCredit.creditAmount = _dbContext.PaymentDetails.Where(c => autos.Contains(c.Payment.AutoId) && c.Payment.PaymentType == PaymentType.Credit).Sum(x => x.Amount);
                 }





                 foreach (var pd in paymentDetails2  )
                 {
                     var pament = pd.Payment;
                     pament.Auto = pd.Auto;
                     pament.Auto.Buyer = pd.Buyer;
                     paymentDetails.Add(new PaymentDetails
                     {

                         Id = pd.Id,
                         Amount = pd.Amount,
                         BuyType = pd.BuyType,
                         CashType = pd.CashType,
                         Notes = pd.Notes,
                         PayDate = pd.PayDate,
                         PayDateStr = pd.PayDate.ToString("dd/MM/yyyy", new CultureInfo("en-US")),
                         PaymentId = pd.PaymentId,
                         Payment = pd.Payment,
                         BuyTypeStr = SwitchBuyType((int)pd.BuyType),
                         CashTypeStr = pd.CashTypeStr,
                         DebitAmount = debitAmount,
                         AutoId = pd.Payment.AutoId,
                         VinNo = pd.Payment.Auto.VinNo,
                         Client = pd.Payment.Auto.Buyer.Name,
                         CarName = pd.Payment.Auto.Name,
                         PaymentType = (int)pd.Payment.PaymentType

                     });
                 }
                 return paymentDetails;
             }
             catch (Exception ex)
             {

                 throw;
             }
         }
     }*/
        // comented bt qusay on 26/09/2024 change to fix the filter by date and client together
        /*public async Task<List<PaymentDetails>> getList(GetPaymentsInput input, List<long> autos, int nubmerOfRecords = 500)
        {
            decimal debitAmount = 0;
            decimal creditAmount = 0;
            try
            {
                List<PaymentDetails> paymentDetails = new List<PaymentDetails>();
                var paymentDetails2 = new List<PaymentDetails2>();

                if (input.IsSearch)
                {
                    if (!string.IsNullOrEmpty(input.VinNo))
                    {
                        //paymentDetails =
                        //paymentDetails.Where(m => m.Payment.Auto.VinNo.Contains(input.VinNo)).ToList();
                        paymentDetails2 = await (from pd in _dbContext.PaymentDetails.Where(c => autos.Contains(c.Payment.AutoId)
                                                 && c.Payment.Auto.VinNo.Contains(input.VinNo)).Take(nubmerOfRecords)
                                                 join p in _dbContext.Payments on pd.PaymentId equals p.Id
                                                 join auto in _dbContext.Autos on p.AutoId equals auto.Id
                                                 join buyer in _dbContext.Users on auto.BuyerId equals buyer.Id

                                                 join lookup in _dbContext.LookupValues on pd.CashType equals lookup.Id

                                                 select new PaymentDetails2
                                                 {

                                                     Id = pd.Id,
                                                     Amount = pd.Amount,
                                                     AutoId = pd.AutoId,
                                                     BuyType = pd.BuyType,
                                                     CarName = pd.CarName,
                                                     CashType = pd.CashType,
                                                     Client = pd.Client,
                                                     DebitAmount = pd.DebitAmount,
                                                     Notes = pd.Notes,
                                                     PayDate = pd.PayDate,
                                                     //PayDateStr = pd.PayDateStr,
                                                     Payment = p,
                                                     Auto = auto,
                                                     Buyer = buyer,
                                                     PaymentId = pd.PaymentId,
                                                     VinNo = auto.VinNo,
                                                     CashTypeStr = lookup.Name,

                                                 }
                                     ).OrderByDescending(x => x.VinNo).ThenByDescending(x => x.PayDate).ToListAsync();
                    }
                    if (input.PurchaseDate.HasValue)
                    {
                        var purchaseDate = input.PurchaseDate.Value.Date; // Get the date part (e.g., 6/9/2024)
                        var nextDay = purchaseDate.AddDays(1); // Get the next day (e.g., 7/9/2024)

                        paymentDetails2 = await (from pd in _dbContext.PaymentDetails
                                                 where autos.Contains(pd.Payment.AutoId)
                                                       && pd.PayDate >= purchaseDate
                                                       && pd.PayDate < nextDay // Compare using a range
                                                 join p in _dbContext.Payments on pd.PaymentId equals p.Id
                                                 join auto in _dbContext.Autos on p.AutoId equals auto.Id
                                                 join buyer in _dbContext.Users on auto.BuyerId equals buyer.Id
                                                 join lookup in _dbContext.LookupValues on pd.CashType equals lookup.Id

                                                 select new PaymentDetails2
                                                 {
                                                     Id = pd.Id,
                                                     Amount = pd.Amount,
                                                     AutoId = pd.AutoId,
                                                     BuyType = pd.BuyType,
                                                     CarName = pd.CarName,
                                                     CashType = pd.CashType,
                                                     Client = pd.Client,
                                                     DebitAmount = pd.DebitAmount,
                                                     Notes = pd.Notes,
                                                     PayDate = pd.PayDate,
                                                     Payment = p,
                                                     Auto = auto,
                                                     Buyer = buyer,
                                                     PaymentId = pd.PaymentId,
                                                     VinNo = auto.VinNo,
                                                     CashTypeStr = lookup.Name,
                                                 })
                                                  .OrderByDescending(x => x.VinNo)
                                                  .ThenByDescending(x => x.PayDate)
                                                  .Take(nubmerOfRecords)
                                                  .ToListAsync();
                    }


                    if (input.AutoId.HasValue && input.AutoId != 0)
                    {
                        // paymentDetails =
                        //paymentDetails.Where(m => m.Payment.AutoId == input.AutoId).ToList();
                        paymentDetails2 = await (from pd in _dbContext.PaymentDetails.Where(c => autos.Contains(c.Payment.AutoId)
                                                && c.Payment.AutoId == input.AutoId).Take(nubmerOfRecords)
                                                 join p in _dbContext.Payments on pd.PaymentId equals p.Id
                                                 join auto in _dbContext.Autos on p.AutoId equals auto.Id
                                                 join buyer in _dbContext.Users on auto.BuyerId equals buyer.Id

                                                 join lookup in _dbContext.LookupValues on pd.CashType equals lookup.Id

                                                 select new PaymentDetails2
                                                 {

                                                     Id = pd.Id,
                                                     Amount = pd.Amount,
                                                     AutoId = pd.AutoId,
                                                     BuyType = pd.BuyType,
                                                     CarName = pd.CarName,
                                                     CashType = pd.CashType,
                                                     Client = pd.Client,
                                                     DebitAmount = pd.DebitAmount,
                                                     Notes = pd.Notes,
                                                     PayDate = pd.PayDate,
                                                     //PayDateStr = pd.PayDateStr,
                                                     Payment = p,
                                                     Auto = auto,
                                                     Buyer = buyer,
                                                     PaymentId = pd.PaymentId,
                                                     VinNo = auto.VinNo,
                                                     CashTypeStr = lookup.Name,

                                                 }
                                    ).OrderByDescending(x => x.VinNo).ThenByDescending(x => x.PayDate).ToListAsync();
                    }
                    if (input.ClientId.HasValue && input.ClientId != 0)
                    {
                        //paymentDetails =
                        //paymentDetails.Where(m => m.Payment.Auto.BuyerId == input.ClientId).ToList();
                        paymentDetails2 = await (from pd in _dbContext.PaymentDetails.Where(c => autos.Contains(c.Payment.AutoId)
                                                && c.Payment.Auto.BuyerId == input.ClientId).Take(nubmerOfRecords)
                                                 join p in _dbContext.Payments on pd.PaymentId equals p.Id
                                                 join auto in _dbContext.Autos on p.AutoId equals auto.Id
                                                 join buyer in _dbContext.Users on auto.BuyerId equals buyer.Id

                                                 join lookup in _dbContext.LookupValues on pd.CashType equals lookup.Id

                                                 select new PaymentDetails2
                                                 {

                                                     Id = pd.Id,
                                                     Amount = pd.Amount,
                                                     AutoId = pd.AutoId,
                                                     BuyType = pd.BuyType,
                                                     CarName = pd.CarName,
                                                     CashType = pd.CashType,
                                                     Client = pd.Client,
                                                     DebitAmount = pd.DebitAmount,
                                                     Notes = pd.Notes,
                                                     PayDate = pd.PayDate,
                                                     //PayDateStr = pd.PayDateStr,
                                                     Payment = p,
                                                     Auto = auto,
                                                     Buyer = buyer,
                                                     PaymentId = pd.PaymentId,
                                                     VinNo = auto.VinNo,
                                                     CashTypeStr = lookup.Name,

                                                 }
                                  ).OrderByDescending(x => x.VinNo).ThenByDescending(x => x.PayDate).ToListAsync();
                    }
                    if (string.IsNullOrEmpty(input.VinNo) && !input.PurchaseDate.HasValue
                        && (!input.AutoId.HasValue || input.AutoId == 0) && (!input.ClientId.HasValue || input.ClientId == 0))
                    {
                        input.IsSearch = false;
                    }


                }
                else if (!input.IsSearch)
                {

                    paymentDetails2 = await (from pd in _dbContext.PaymentDetails.Where(c => autos.Contains(c.Payment.AutoId)).Take(nubmerOfRecords)
                                             join p in _dbContext.Payments on pd.PaymentId equals p.Id
                                             join auto in _dbContext.Autos on p.AutoId equals auto.Id
                                             join buyer in _dbContext.Users on auto.BuyerId equals buyer.Id

                                             join lookup in _dbContext.LookupValues on pd.CashType equals lookup.Id

                                             select new PaymentDetails2
                                             {

                                                 Id = pd.Id,
                                                 Amount = pd.Amount,
                                                 AutoId = pd.AutoId,
                                                 BuyType = pd.BuyType,
                                                 CarName = pd.CarName,
                                                 CashType = pd.CashType,
                                                 Client = pd.Client,
                                                 DebitAmount = pd.DebitAmount,
                                                 Notes = pd.Notes,
                                                 PayDate = pd.PayDate,
                                                 //PayDateStr = pd.PayDateStr,
                                                 Payment = p,
                                                 Auto = auto,
                                                 Buyer = buyer,
                                                 PaymentId = pd.PaymentId,
                                                 VinNo = auto.VinNo,
                                                 CashTypeStr = lookup.Name,

                                             }
                                       ).OrderByDescending(x => x.PayDate).ToListAsync();

                    _PaymentDebeitCredit.debitAmount = _dbContext.PaymentDetails.Where(c => autos.Contains(c.Payment.AutoId) && c.Payment.PaymentType == PaymentType.Debit).Sum(x => x.Amount);
                    _PaymentDebeitCredit.creditAmount = _dbContext.PaymentDetails.Where(c => autos.Contains(c.Payment.AutoId) && c.Payment.PaymentType == PaymentType.Credit).Sum(x => x.Amount);
                }





                foreach (var pd in paymentDetails2)
                {
                    var pament = pd.Payment;
                    pament.Auto = pd.Auto;
                    pament.Auto.Buyer = pd.Buyer;
                    paymentDetails.Add(new PaymentDetails
                    {

                        Id = pd.Id,
                        Amount = pd.Amount,
                        BuyType = pd.BuyType,
                        CashType = pd.CashType,
                        Notes = pd.Notes,
                        PayDate = pd.PayDate,
                        PayDateStr = pd.PayDate.ToString("dd/MM/yyyy", new CultureInfo("en-US")),
                        PaymentId = pd.PaymentId,
                        Payment = pd.Payment,
                        BuyTypeStr = SwitchBuyType((int)pd.BuyType),
                        CashTypeStr = pd.CashTypeStr,
                        DebitAmount = debitAmount,
                        AutoId = pd.Payment.AutoId,
                        VinNo = pd.Payment.Auto.VinNo,
                        Client = pd.Payment.Auto.Buyer.Name,
                        CarName = pd.Payment.Auto.Name,
                        PaymentType = (int)pd.Payment.PaymentType

                    });
                }
                return paymentDetails;
            }
            catch (Exception ex)
            {

                throw;
            }
        }*/
        // new code to accept filter by date and vin number and client
        public async Task<List<PaymentDetails>> getList(GetPaymentsInput input, List<long> autos, int nubmerOfRecords = 500)
        {
            List<PaymentDetails> paymentDetails = new List<PaymentDetails>();

            try
            {
                if (input.IsSearch)
                {
                    var query = from pd in _dbContext.PaymentDetails
                                where autos.Contains(pd.Payment.AutoId)
                                join p in _dbContext.Payments on pd.PaymentId equals p.Id
                                join auto in _dbContext.Autos on p.AutoId equals auto.Id
                                join buyer in _dbContext.Users on auto.BuyerId equals buyer.Id
                                join lookup in _dbContext.LookupValues on pd.CashType equals lookup.Id
                                select new PaymentDetails2
                                {
                                    Id = pd.Id,
                                    Amount = pd.Amount,
                                    AutoId = pd.AutoId,
                                    BuyType = pd.BuyType,
                                    CarName = pd.CarName,
                                    CashType = pd.CashType,
                                    Client = pd.Client,
                                    DebitAmount = pd.DebitAmount,
                                    Notes = pd.Notes,
                                    PayDate = pd.PayDate,
                                    Payment = p,
                                    Auto = auto,
                                    Buyer = buyer,
                                    PaymentId = pd.PaymentId,
                                    VinNo = auto.VinNo,
                                    CashTypeStr = lookup.Name,
                                };

                    // Apply filters
                    if (!string.IsNullOrEmpty(input.VinNo))
                    {
                        query = query.Where(c => c.VinNo.Contains(input.VinNo));
                    }

                    if (input.PurchaseDate.HasValue)
                    {
                        var purchaseDate = input.PurchaseDate.Value.Date;
                        query = query.Where(pd => pd.PayDate.Date == purchaseDate);
                    }

                    if (input.AutoId.HasValue && input.AutoId != 0)
                    {
                        query = query.Where(c => c.AutoId == input.AutoId);
                    }

                    if (input.ClientId.HasValue && input.ClientId != 0)
                    {
                        query = query.Where(c => c.Buyer.Id == input.ClientId);
                    }

                    // Execute the query asynchronously with pagination
                    var paymentDetails2 = await query.OrderByDescending(x => x.PayDate)
                                                      .Take(nubmerOfRecords)
                                                      .ToListAsync();

                    // Map results to the final PaymentDetails list
                    foreach (var pd in paymentDetails2)
                    {
                        paymentDetails.Add(new PaymentDetails
                        {
                            Id = pd.Id,
                            Amount = pd.Amount,
                            BuyType = pd.BuyType,
                            CashType = pd.CashType,
                            Notes = pd.Notes,
                            PayDate = pd.PayDate,
                            PayDateStr = pd.PayDate.ToString("dd/MM/yyyy", new CultureInfo("en-US")),
                            PaymentId = pd.PaymentId,
                            Payment = pd.Payment,
                            BuyTypeStr = SwitchBuyType((int)pd.BuyType),
                            CashTypeStr = pd.CashTypeStr,
                            AutoId = pd.Payment.AutoId,
                            VinNo = pd.Payment.Auto.VinNo,
                            Client = pd.Payment.Auto.Buyer.Name,
                            CarName = pd.Payment.Auto.Name,
                            PaymentType = (int)pd.Payment.PaymentType
                        });
                    }

                    return paymentDetails;
                }
                else // If IsSearch is false, retrieve all payment details
                {
                    var allPaymentsQuery = from pd in _dbContext.PaymentDetails
                                           where autos.Contains(pd.Payment.AutoId)
                                           join p in _dbContext.Payments on pd.PaymentId equals p.Id
                                           join auto in _dbContext.Autos on p.AutoId equals auto.Id
                                           join buyer in _dbContext.Users on auto.BuyerId equals buyer.Id
                                           join lookup in _dbContext.LookupValues on pd.CashType equals lookup.Id
                                           select new PaymentDetails2
                                           {
                                               Id = pd.Id,
                                               Amount = pd.Amount,
                                               AutoId = pd.AutoId,
                                               BuyType = pd.BuyType,
                                               CarName = pd.CarName,
                                               CashType = pd.CashType,
                                               Client = pd.Client,
                                               DebitAmount = pd.DebitAmount,
                                               Notes = pd.Notes,
                                               PayDate = pd.PayDate,
                                               Payment = p,
                                               Auto = auto,
                                               Buyer = buyer,
                                               PaymentId = pd.PaymentId,
                                               VinNo = auto.VinNo,
                                               CashTypeStr = lookup.Name,
                                           };

                    var allPaymentDetails = await allPaymentsQuery.OrderByDescending(x => x.PayDate)
                                                                   .Take(nubmerOfRecords)
                                                                   .ToListAsync();

                    // Map results to the final PaymentDetails list
                    foreach (var pd in allPaymentDetails)
                    {
                        paymentDetails.Add(new PaymentDetails
                        {
                            Id = pd.Id,
                            Amount = pd.Amount,
                            BuyType = pd.BuyType,
                            CashType = pd.CashType,
                            Notes = pd.Notes,
                            PayDate = pd.PayDate,
                            PayDateStr = pd.PayDate.ToString("dd/MM/yyyy", new CultureInfo("en-US")),
                            PaymentId = pd.PaymentId,
                            Payment = pd.Payment,
                            BuyTypeStr = SwitchBuyType((int)pd.BuyType),
                            CashTypeStr = pd.CashTypeStr,
                            AutoId = pd.Payment.AutoId,
                            VinNo = pd.Payment.Auto.VinNo,
                            Client = pd.Payment.Auto.Buyer.Name,
                            CarName = pd.Payment.Auto.Name,
                            PaymentType = (int)pd.Payment.PaymentType
                        });
                    }

                    return paymentDetails;
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions (log the exception, rethrow, etc.)
                throw;
            }
        }

    }
}
class PaymentDetails2: PaymentDetails
{
    public Auto Auto { get; set; }
    public User Buyer { get; set; }
}
class PaymentDebeitCredit
{
    public decimal debitAmount = 0;
    public decimal creditAmount = 0;

}
