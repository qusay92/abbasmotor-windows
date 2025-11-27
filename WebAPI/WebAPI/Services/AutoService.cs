using AutoMapper;
using Entities;
using Microsoft.EntityFrameworkCore;
using System.Drawing;
using System.Globalization;
using System.Linq;
using WebAPI.General;

namespace WebAPI.Services
{
    public class AutoService : EnumExtension, IAutoService
    {
        public readonly AmdDBContext _dbContext;
        public readonly IConfiguration _configuration;
        public readonly IMapper _mapper;
        private readonly IStringLocalizer<SharedResource> _sharedResourceLocalizer;

        public AutoService(AmdDBContext dbContext, IConfiguration configuration, IMapper mapper, IStringLocalizer<SharedResource> sharedResourceLocalizer)
        {
            _dbContext = dbContext;
            _configuration = configuration;
            _mapper = mapper;
            _sharedResourceLocalizer = sharedResourceLocalizer;
        }

        public async Task<ResponseResult<List<Auto>>> GetAllByUser(long id, AutoFilterParams search)
        {
            try
            {
                ResponseResult<List<Auto>> result = new ResponseResult<List<Auto>>();
                List<Auto> autos = await _dbContext.Autos
                    .Include(x => x.Container)
                    .Include(x => x.Buyer)
                    .Include(x => x.Brand)
                    .Include(x => x.Images)
                    .Include(x => x.Auction)
                    .Include(x => x.City)
                    .Include(x => x.Destination)
                    .Include(x => x.LoadPort)
                    .Include(x => x.BuyingAccount)
                    .Where(c => (c.CreationUserId == id || c.BuyerId == id) && c.IsArchive == 0)
                    .ToListAsync();
                List<LookupValue> lookupValues = _dbContext.LookupValues.ToList();

                var archivedAuto = await _dbContext.Autos.Where(c => (c.CreationUserId == id || c.BuyerId == id) && c.IsArchive == 1)
                                         .ToListAsync();

                List<Auto> models = autos.ToList()
                    .Where(c => ((search.Status > 0) ? SwitchConditional.SwitchConditionalCarStatus(c, c.Container, search.Status.Value) : true)
                    && ((search.loadPort > 0) ? (c.LoadPortId == search.loadPort) : true))
                    .Select(c => new Auto
                    {
                        Id = c.Id,
                        Description = c.Description,
                        CarStatus = c.CarStatus,
                        Type = 0,
                        BrandId = c.BrandId,
                        AuctionId = c.AuctionId,
                        ArrivalDate = c.ContainerId.HasValue ? c.Container.ArrivalDate : null,
                        DepartureDate = c.ContainerId.HasValue ? c.Container.DepartureDate : null,
                        BuyDate = c.BuyDate,
                        BuyDateStr = c.BuyDate.ToString("dd/MM/yyyy", new CultureInfo("en-US")),
                        BuyingAccountId = c.BuyingAccountId,
                        CityId = c.CityId,
                        ColorId = c.ColorId,
                        Color = lookupValues.Where(l => l.Id == c.ColorId).FirstOrDefault().Name,
                        ContainerId = c.ContainerId,
                        //Container = c.Container,
                        CreationDate = c.CreationDate,
                        CreationUserId = c.CreationUserId,
                        DestinationId = c.DestinationId,
                        BuyerId = c.BuyerId,
                        Engine = c.Engine,
                        LoadPortId = c.LoadPortId,
                        VinNo = c.VinNo,
                        DisplayStatus = c.DisplayStatus,
                        Lot = c.Lot,
                        Model = c.Model,
                        RemainingPayment = c.RemainingPayment,
                        PaperStatus = c.PaperStatus,
                        ModificationUserId = c.ModificationUserId,
                        ModificationDate = c.ModificationDate,
                        Name = c.Name,
                        IsArchive = c.IsArchive,
                        CarStr = c.Brand.Name + " " + c.Name + " " + c.Model,
                        RowColor = c.ContainerId.HasValue ? SwitchStatusByDate(c.Container.DepartureDate, c.Container.ArrivalDate).Color : "",
                        Image = c.Images.Count > 0 ? c.Images.FirstOrDefault().Title : "",
                        AuctionName = c.Auction.Name,
                        BrandName = c.Brand.Name,
                        BuyerName = c.Buyer.Name,
                        CityName = c.City.Name,
                        DestinationName = c.Destination.Name,
                        LoadPortName = c.LoadPort.Name,
                        BuyAccountName = c.BuyingAccountId.HasValue ? c.BuyingAccount.Name : "N/A",
                        ContainerSerial = c.Container != null ? c.Container.SerialNumber : "N/A",
                        DisplayStatusStr = System.Enum.GetName(typeof(DisplayStatus), c.DisplayStatus),
                        PaperStatusStr = SwitchPaperStatus(c.PaperStatus.Value),
                        CarStatusStr = c.ContainerId.HasValue ? SwitchStatusByDate(c.Container.DepartureDate, c.Container.ArrivalDate, true).Status : "Bought New",
                        BookNo = c.ContainerId.HasValue ? c.Container.BookNo : null,
                        DeliveredDateStr = c.ContainerId.HasValue ? c.Container.ArrivalDate.Value.ToString("dd/MM/yyyy", new CultureInfo("en-US")) : string.Empty,
                        ContainerArrivalDate = c.ContainerId.HasValue ? c.Container.ArrivalDate : default(DateTime),
                        ArchivedAuto = archivedAuto.Count(),
                        ClientBuyerName = c.ClientBuyerName,
                        UpdatePaymentStatus = c.UpdatePaymentStatus,
                       // CarName = c.CarName,
                    })
                    .OrderByDescending(x => x.BuyDate)
                    .ThenByDescending(x => x.CreationDate)
                    .ToList();

                if (search.isSearch)
                {
                    if (!string.IsNullOrEmpty(search.vinNumber)) models = models.Where(m => m.VinNo.ToLower().Contains(search.vinNumber.ToLower())).ToList();
                    if (!string.IsNullOrEmpty(search.lotNumber)) models = models.Where(m => m.Lot.ToLower().Contains(search.lotNumber.ToLower())).ToList();
                    if (search.client > 0) models = models.Where(m => m.BuyerId == search.client).ToList();
                    if (search.auction > 0) models = models.Where(m => m.AuctionId == search.auction).ToList();
                    if (search.buyAccount > 0) models = models.Where(m => m.BuyingAccountId == search.buyAccount).ToList();
                    if (!string.IsNullOrEmpty(search.container)) models = models.Where(m => m.ContainerSerial.ToLower().Contains(search.container.ToLower())).ToList();
                    if (search.loadPort > 0) models = models.Where(m => m.LoadPortId == search.loadPort).ToList();
                    if (search.destination > 0) models = models.Where(m => m.DestinationId == search.destination).ToList();
                    if (search.city > 0) models = models.Where(m => m.CityId == search.city).ToList();

                    // New Added to search via Client Name from te DB
                    if (!string.IsNullOrEmpty(search.ClientBuyerName))
                    {
                        models = models.Where(m => (m.ClientBuyerName?.ToLower() ?? "").Contains(search.ClientBuyerName.ToLower())).ToList();
                    }
                    // New Added search via Payment Status Values (Done Or Incomplete) from te DB
                    if (!string.IsNullOrEmpty(search.UpdatePaymentStatus))
                    {
                        models = models.Where(m => (m.UpdatePaymentStatus?.ToLower() ?? "").Contains(search.UpdatePaymentStatus.ToLower())).ToList();
                    }


                    //if (search.Status > 0)
                    //    models = models.Where(m => SwitchConditional.SwitchConditionalCarStatus(m, search.Status.Value)).ToList();

                    //Auto search old implementation
                    //if (search.carId > 0) models = models.Where(m => m.Id == search.carId).ToList();
                    
                    // New implementation to search by auto
                    if (!string.IsNullOrEmpty(search.CarName)) models = models.Where(m => m.Name == search.CarName).ToList();


                    if (search.deliveryFromDate.HasValue)
                        if (!search.deliveryToDate.HasValue)
                            models = models.Where(m => m.ContainerArrivalDate?.Date == search.deliveryFromDate.Value.Date).ToList();
                        else
                            models = models.Where(m => m.ContainerArrivalDate?.Date >= search.deliveryFromDate.Value.Date).ToList();

                    if (search.deliveryToDate.HasValue)
                        if (!search.deliveryFromDate.HasValue)
                            models = models.Where(m => m.ContainerArrivalDate?.Date == search.deliveryToDate.Value.Date).ToList();
                        else
                            models = models.Where(m => m.ContainerArrivalDate?.Date <= search.deliveryToDate.Value.Date).ToList();


                    if (search.purchaseFromDate.HasValue)
                        if (!search.purchaseToDate.HasValue)
                            models = models.Where(m => m.BuyDate.Date == search.purchaseFromDate.Value.Date).ToList();
                        else
                            models = models.Where(m => m.BuyDate.Date >= search.purchaseFromDate.Value.Date).ToList();


                    if (search.purchaseToDate.HasValue)
                        if (!search.purchaseFromDate.HasValue)
                            models = models.Where(m => m.BuyDate.Date == search.purchaseToDate.Value.Date).ToList();
                        else
                            models = models.Where(m => m.BuyDate.Date <= search.purchaseToDate.Value.Date).ToList();


                }

                result.Data = models;
                result.Errors = null;
                result.Status = StatusType.Success;

                return result;
            }
            catch (Exception ex)
            {

                throw;
            }

        }

        public async Task<ResponseResult<List<Auto>>> GetArchiveAllByUser(long id, AutoFilterParams search)
        {
            try
            {
                ResponseResult<List<Auto>> result = new ResponseResult<List<Auto>>();
                List<Auto> autos = await _dbContext.Autos
                    .Include(x => x.Container)
                    .Include(x => x.Buyer)
                    .Include(x => x.Brand)
                    .Include(x => x.Images)
                    .Include(x => x.Auction)
                    .Include(x => x.City)
                    .Include(x => x.Destination)
                    .Include(x => x.LoadPort)
                    .Include(x => x.BuyingAccount)
                    .Where(c => (c.CreationUserId == id || c.BuyerId == id) && c.IsArchive == 1)
                    .ToListAsync();
                List<LookupValue> lookupValues = _dbContext.LookupValues.ToList();

                var archivedAuto = await _dbContext.Autos.Where(c => (c.CreationUserId == id || c.BuyerId == id) && c.IsArchive == 1)
                                         .ToListAsync();

                List<Auto> models = autos
                    .Where(c => ((search.Status > 0) ? SwitchConditional.SwitchConditionalCarStatus(c, c.Container, search.Status.Value) : true)
                    && ((search.loadPort > 0) ? (c.LoadPortId == search.loadPort) : true))
                    .Select(c => new Auto
                    {
                        Id = c.Id,
                        Description = c.Description,
                        CarStatus = c.CarStatus,
                        Type = 0,
                        BrandId = c.BrandId,
                        AuctionId = c.AuctionId,
                        ArrivalDate = c.ContainerId.HasValue ? c.Container.ArrivalDate : null,
                        DepartureDate = c.ContainerId.HasValue ? c.Container.DepartureDate : null,
                        BuyDate = c.BuyDate,
                        BuyDateStr = c.BuyDate.ToString("dd/MM/yyyy", new CultureInfo("en-US")),
                        BuyingAccountId = c.BuyingAccountId,
                        CityId = c.CityId,
                        ColorId = c.ColorId,
                        Color = lookupValues.Where(l => l.Id == c.ColorId).FirstOrDefault().Name,
                        ContainerId = c.ContainerId,
                        //Container = c.Container,
                        CreationDate = c.CreationDate,
                        CreationUserId = c.CreationUserId,
                        DestinationId = c.DestinationId,
                        BuyerId = c.BuyerId,
                        Engine = c.Engine,
                        LoadPortId = c.LoadPortId,
                        VinNo = c.VinNo,
                        DisplayStatus = c.DisplayStatus,
                        Lot = c.Lot,
                        Model = c.Model,
                        RemainingPayment = c.RemainingPayment,
                        PaperStatus = c.PaperStatus,
                        ModificationUserId = c.ModificationUserId,
                        ModificationDate = c.ModificationDate,
                        Name = c.Name,
                        IsArchive = c.IsArchive,
                        CarStr = c.Brand.Name + " " + c.Name + " " + c.Model,
                        RowColor = c.ContainerId.HasValue ? SwitchStatusByDate(c.Container.DepartureDate, c.Container.ArrivalDate).Color : "",
                        Image = c.Images.Count > 0 ? c.Images.FirstOrDefault().Title : "",
                        AuctionName = c.Auction.Name,
                        BrandName = c.Brand.Name,
                        BuyerName = c.Buyer.Name,
                        CityName = c.City.Name,
                        DestinationName = c.Destination.Name,
                        LoadPortName = c.LoadPort.Name,
                        BuyAccountName = c.BuyingAccountId.HasValue ? c.BuyingAccount.Name : "N/A",
                        ContainerSerial = c.Container != null ? c.Container.SerialNumber : "N/A",
                        DisplayStatusStr = System.Enum.GetName(typeof(DisplayStatus), c.DisplayStatus),
                        PaperStatusStr = SwitchPaperStatus(c.PaperStatus.Value),
                        CarStatusStr = c.ContainerId.HasValue ? SwitchStatusByDate(c.Container.DepartureDate, c.Container.ArrivalDate, true).Status : "Bought New",
                        BookNo = c.ContainerId.HasValue ? c.Container.BookNo : null,
                        DeliveredDateStr = c.ContainerId.HasValue ? c.Container.ArrivalDate.Value.ToString("dd/MM/yyyy", new CultureInfo("en-US")) : string.Empty,
                        ContainerArrivalDate = c.ContainerId.HasValue ? c.Container.ArrivalDate : default(DateTime),
                        ArchivedAuto = archivedAuto.Count()
                    })
                    .OrderByDescending(x => x.BuyDate)
                    .ThenByDescending(x => x.CreationDate)
                    .ToList();

                //if (search.isSearch)
                //{
                //    if (!string.IsNullOrEmpty(search.vinNumber)) models = models.Where(m => m.VinNo.ToLower().Contains(search.vinNumber.ToLower())).ToList();
                //    if (!string.IsNullOrEmpty(search.lotNumber)) models = models.Where(m => m.Lot.ToLower().Contains(search.lotNumber.ToLower())).ToList();
                //    if (search.client > 0) models = models.Where(m => m.BuyerId == search.client).ToList();
                //    if (search.auction > 0) models = models.Where(m => m.AuctionId == search.auction).ToList();
                //    if (search.buyAccount > 0) models = models.Where(m => m.BuyingAccountId == search.buyAccount).ToList();
                //    if (!string.IsNullOrEmpty(search.container)) models = models.Where(m => m.ContainerSerial.ToLower().Contains(search.container.ToLower())).ToList();
                //    if (search.loadPort > 0) models = models.Where(m => m.LoadPortId == search.loadPort).ToList();
                //    if (search.destination > 0) models = models.Where(m => m.DestinationId == search.destination).ToList();
                //    if (search.city > 0) models = models.Where(m => m.CityId == search.city).ToList();

                //    if (search.Status > 0)
                //        models = models.Where(m => SwitchConditional.SwitchConditionalCarStatus(m, search.Status.Value)).ToList();

                //    if (search.carId > 0) models = models.Where(m => m.Id == search.carId).ToList();


                //    if (search.deliveryFromDate.HasValue)
                //        if (!search.deliveryToDate.HasValue)
                //            models = models.Where(m => m.ContainerArrivalDate?.Date == search.deliveryFromDate.Value.Date).ToList();
                //        else
                //            models = models.Where(m => m.ContainerArrivalDate?.Date >= search.deliveryFromDate.Value.Date).ToList();

                //    if (search.deliveryToDate.HasValue)
                //        if (!search.deliveryFromDate.HasValue)
                //            models = models.Where(m => m.ContainerArrivalDate?.Date == search.deliveryToDate.Value.Date).ToList();
                //        else
                //            models = models.Where(m => m.ContainerArrivalDate?.Date <= search.deliveryToDate.Value.Date).ToList();


                //    if (search.purchaseFromDate.HasValue)
                //        if (!search.purchaseToDate.HasValue)
                //            models = models.Where(m => m.BuyDate.Date == search.purchaseFromDate.Value.Date).ToList();
                //        else
                //            models = models.Where(m => m.BuyDate.Date >= search.purchaseFromDate.Value.Date).ToList();


                //    if (search.purchaseToDate.HasValue)
                //        if (!search.purchaseFromDate.HasValue)
                //            models = models.Where(m => m.BuyDate.Date == search.purchaseToDate.Value.Date).ToList();
                //        else
                //            models = models.Where(m => m.BuyDate.Date <= search.purchaseToDate.Value.Date).ToList();

                //}

                result.Data = models;
                result.Errors = null;
                result.Status = StatusType.Success;

                return result;
            }
            catch (Exception ex)
            {
                throw;
            }

        }

        public async Task<ResponseResult<List<Auto>>> SaveAuto(SaveAutoInput model, long UserId)
        {
            ResponseResult<List<Auto>> result = new ResponseResult<List<Auto>>();
            result.Status = StatusType.Failed;
            bool isSaved = false;

            var AutoByVinno = await _dbContext.Autos.FirstOrDefaultAsync(u => u.VinNo == model.VinNo && u.Id != model.Id);

            if (AutoByVinno != null)
            {
                result.Data = null;
                result.Errors = new List<string> { _sharedResourceLocalizer["Vinnoalreadyexist"] };
                result.Status = StatusType.Failed;
                return result;
            }


            model.BuyDate = model.BuyDate.ToLocalTime();
            if (model.ArrivalDate.HasValue)
            {
                model.ArrivalDate = model.ArrivalDate.Value.ToLocalTime();
            }

            var mapModel = _mapper.Map<SaveAutoInput, Auto>(model);
            try
            {
                if (model.Id < 1)
                {
                    mapModel.CreationDate = DateTime.Now;
                    mapModel.CreationUserId = UserId;

                    _dbContext.Autos.Add(mapModel);
                }
                else
                {
                    var AutoById = await _dbContext.Autos.AsNoTracking().FirstOrDefaultAsync(x => x.Id == model.Id);
                    mapModel.ContainerId = AutoById.ContainerId;
                    mapModel.ModificationDate = DateTime.Now;
                    mapModel.ModificationUserId = UserId;

                    _dbContext.Autos.Update(mapModel);
                }

                isSaved = await _dbContext.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                ;
            }

            if (isSaved)
            {
                List<Auto> list = new List<Auto>();
                list.Add(mapModel);

                result.Data = list;
                result.Errors = null;
                result.Status = StatusType.Success;
            }

            return result;
        }

        public async Task<ResponseResult<List<Auto>>> DeleteAutos(List<Auto> models)
        {
            ResponseResult<List<Auto>> result = new ResponseResult<List<Auto>>();
            result.Status = StatusType.Failed;
            bool isSaved = false;

            var depotIds = models.Select(x => x.Id);

            List<AutoImage> dbImagesByAutoId = _dbContext.AutoImages.Where(i => depotIds.Contains(i.AutoId)).ToList();
            if (dbImagesByAutoId.Count > 0)
            {
                await DeleteFiles(dbImagesByAutoId);
                _dbContext.AutoImages.RemoveRange(dbImagesByAutoId);
            }
            _dbContext.Autos.RemoveRange(models);

            isSaved = await _dbContext.SaveChangesAsync() > 0;

            if (isSaved)
            {
                result = new ResponseResult<List<Auto>>();
                result.Errors = null;
                result.Status = StatusType.Success;
            }

            return result;
        }

        public async Task<ResponseResult<List<Auto>>> Delete(long Id)
        {
            ResponseResult<List<Auto>> result = new ResponseResult<List<Auto>>();
            result.Status = StatusType.Failed;
            bool isSaved = false;

            List<AutoImage> dbImagesByAutoId = _dbContext.AutoImages.Where(i => i.AutoId == Id).ToList();
            if (dbImagesByAutoId.Count > 0)
            {
                await DeleteFiles(dbImagesByAutoId);
                _dbContext.AutoImages.RemoveRange(dbImagesByAutoId);
            }
            var client = await _dbContext.Autos.FirstOrDefaultAsync(u => u.Id == Id);
            _dbContext.Autos.Remove(client);

            isSaved = await _dbContext.SaveChangesAsync() > 0;

            if (isSaved)
            {
                result = new ResponseResult<List<Auto>>();
                result.Errors = null;
                result.Status = StatusType.Success;
            }

            return result;
        }

        public dynamic GetSideMenuByUser(long id, byte isArchive)
        {
            dynamic result = null;
            List<Auto> autos = _dbContext.Autos.Include(x => x.Container).Where(c => (c.CreationUserId == id || c.BuyerId == id) && c.IsArchive == isArchive).ToList();
            List<LookupValue> loadPorts = _dbContext.LookupValues.Where(c => c.LookupId == 2).ToList();

            var all = new
            {
                BoughtNew = autos.Where(a => a.ContainerId == null || (a.Container.DepartureDate > DateTime.Now && a.Container.ArrivalDate > DateTime.Now)).Count(),
                Loaded = autos.Where(b => b.ContainerId != null && b.Container.DepartureDate <= DateTime.Now && b.Container.ArrivalDate > DateTime.Now).Count(),
                Arrived = autos.Where(c => c.ContainerId != null && c.Container.ArrivalDate <= DateTime.Now).Count()
            };

            var filteredAutos = loadPorts.Select(item => new
            {
                loadPortId = item.Id,
                loadPort = item.Name,
                BoughtNew = autos.Where(a => a.LoadPortId == item.Id && (a.ContainerId == null || (a.Container.DepartureDate > DateTime.Now && a.Container.ArrivalDate > DateTime.Now))).Count(),
                Loaded = autos.Where(b => b.ContainerId != null && b.Container.DepartureDate <= DateTime.Now && b.Container.ArrivalDate > DateTime.Now && b.LoadPortId == item.Id).Count(),
                Arrived = autos.Where(c => c.ContainerId != null && c.Container.ArrivalDate <= DateTime.Now && c.LoadPortId == item.Id).Count()
            }).ToList();

            result = new { all, filteredAutos };

            return result;
        }

        public async Task<ResponseResult<Auto>> GetAutoById(long id)
        {
            ResponseResult<Auto> result = new ResponseResult<Auto>();
            List<Auto> autos = await _dbContext.Autos
                .Include(c => c.Container)
                .Include(c => c.Container).ThenInclude(y => y.ShippingCompany)
                .Include(c => c.Images)
                .Include(x => x.Buyer)
                .Include(x => x.Brand)
                .Include(x => x.Auction)
                .Include(x => x.City)
                .Include(x => x.Destination)
                .Include(x => x.LoadPort)
                .Include(x => x.BuyingAccount)
                .Where(c => c.Id == id)
                .ToListAsync();
            List<LookupValue> lookupValues = _dbContext.LookupValues.ToList();

            Auto models = autos.Select(c => new Auto
            {
                Id = c.Id,
                Description = c.Description,
                CarStatus = c.CarStatus,
                Type = c.Type,
                BrandId = c.BrandId,
                AuctionId = c.AuctionId,
                ArrivalDate = c.ContainerId.HasValue ? c.Container.ArrivalDate : null,
                ContainerArrivalDateStr = c.ContainerId.HasValue ? c.Container.ArrivalDate.Value.ToString("dd/MM/yyyy", new CultureInfo("en-US")) : "",
                BuyDate = c.BuyDate,
                BuyDateStr = c.BuyDate.ToString("dd/MM/yyyy", new CultureInfo("en-US")),
                BuyingAccountId = c.BuyingAccountId,
                CityId = c.CityId,
                Color = lookupValues.Where(l => l.Id == c.ColorId).FirstOrDefault().Name,
                ContainerId = c.ContainerId,
                CreationDate = c.CreationDate,
                CreationUserId = c.CreationUserId,
                DestinationId = c.DestinationId,
                BuyerId = c.BuyerId,
                Engine = c.Engine,
                LoadPortId = c.LoadPortId,
                VinNo = c.VinNo,
                DisplayStatus = c.DisplayStatus,
                Lot = c.Lot,
                Model = c.Model,
                RemainingPayment = c.RemainingPayment,
                PaperStatus = c.PaperStatus,
                ModificationUserId = c.ModificationUserId,
                ModificationDate = c.ModificationDate,
                Name = c.Name,
                IsArchive = c.IsArchive,
                AuctionName = c.Auction.Name,
                BrandName = c.Brand.Name,
                BuyerName = c.Buyer.Name,
                BuyerUsername = _dbContext.Users.Where(u => u.Id == c.BuyerId).FirstOrDefault().UserName,
                BuyerMobile = _dbContext.Users.Where(u => u.Id == c.BuyerId).FirstOrDefault().Mobile,
                CityName = c.City.Name,
                DestinationName = c.Destination.Name,
                LoadPortName = c.LoadPort.Name,
                BuyAccountName = c.BuyingAccountId.HasValue ? c.BuyingAccount.Name : "N/A",
                ContainerSerial = c.ContainerId.HasValue ? c.Container.SerialNumber : null,
                DisplayStatusStr = System.Enum.GetName(typeof(DisplayStatus), c.DisplayStatus),
                PaperStatusStr = SwitchPaperStatus(c.PaperStatus.Value),
                CarStatusStr = SwitchCarStatus(c.CarStatus),
                TypeStr = SwitchCarType(c.CarType),
                BookNo = c.ContainerId.HasValue ? c.Container.BookNo : null,
                DepartureDate = c.ContainerId.HasValue ? c.Container.DepartureDate : null,
                DepartureDateStr = c.ContainerId.HasValue ? c.Container.DepartureDate.Value.ToString("dd/MM/yyyy", new CultureInfo("en-US")) : "",
                ShippingCompany = c.ContainerId.HasValue ? c.Container.ShippingCompany.Name : null,
                Images = c.Images.Select(x => new AutoImage
                {
                    Id = x.Id,
                    Alt = x.Alt,
                    Title = x.Title,
                    PreviewImageSrc = x.PreviewImageSrc,
                    ThumbnailImageSrc = x.ThumbnailImageSrc,
                    AutoId = x.AutoId
                }).ToList()
            }).FirstOrDefault();

            result.Data = models;
            result.Errors = null;
            result.Status = StatusType.Success;
            return result;
        }

        public async Task<bool> DeleteFiles(List<AutoImage> dbImages)
        {
            string[] files = Directory.GetFiles(_configuration["Attachmentspath"]);
            foreach (string file in files)
            {
                if (dbImages.Any(i => file.Contains(i.Title)))
                {
                    File.Delete(file);
                }
            }
            return true;
        }

        public async Task<ResponseResult<List<AutoNameVM>>> GetCarNameByUser(long Id)
        {

            ResponseResult<List<AutoNameVM>> result = new ResponseResult<List<AutoNameVM>>();
            List<Auto> autos = _dbContext.Autos.Where(c => c.CreationUserId == Id || c.BuyerId == Id)
                .Select(c => new Auto
                {
                    Id = c.Id,
                    Name = c.Name,
                    VinNo = c.VinNo
                })
                .GroupBy(c => c.Name)
                .Select(g => g.First())
                .ToList();
            var models = _mapper.Map<List<Auto>, List<AutoNameVM>>(autos);

            result.Data = models;
            result.Errors = null;
            result.Status = StatusType.Success;

            return result;
        }
        // New Added for Get Client Name from te DB
        public async Task<ResponseResult<List<ClientBuyerNameVM>>> GetClientBuyerNameByUser(long Id)

        {
            ResponseResult<List<ClientBuyerNameVM>> result = new ResponseResult<List<ClientBuyerNameVM>>();

            // Querying the database for cars where the user is the buyer (ClientBuyerName is a column in the Autos table)
            //List<Auto> autos = _dbContext.Autos.Where(c => !string.IsNullOrEmpty(c.ClientBuyerName) && (c.CreationUserId == Id || c.BuyerId == Id)) // You can adjust this condition based on your business logic
            //    .Select(c => new Auto
            //    {
            //        Id = c.Id,
            //        ClientBuyerName = c.ClientBuyerName
            //    }).DistinctBy(c => c.ClientBuyerName)
            //    .ToList();

            List<Auto> autos = _dbContext.Autos
                    .Where(c => !string.IsNullOrEmpty(c.ClientBuyerName) && (c.CreationUserId == Id || c.BuyerId == Id))
                    .GroupBy(c => c.ClientBuyerName)
                    .Select(g => g.First())
                    .ToList();

            var models = _mapper.Map<List<Auto>, List<ClientBuyerNameVM>>(autos);

            result.Data = models;
            result.Errors = null;
            result.Status = StatusType.Success;

            return result;
        }
        // New Added for Get Payment Status Values (Done Or Incomplete) from te DB
        public async Task<ResponseResult<List<UpdatePaymentStatusVM>>> GetUpdatePaymentStatusByUser(long Id)

        {
            ResponseResult<List<UpdatePaymentStatusVM>> result = new ResponseResult<List<UpdatePaymentStatusVM>>();
            List<Auto> autos = _dbContext.Autos
                    .Where(c => !string.IsNullOrEmpty(c.UpdatePaymentStatus) && (c.CreationUserId == Id || c.BuyerId == Id))
                    .GroupBy(c => c.UpdatePaymentStatus)
                    .Select(g => g.First())
                    .ToList();

            var models = _mapper.Map<List<Auto>, List<UpdatePaymentStatusVM>>(autos);

            result.Data = models;
            result.Errors = null;
            result.Status = StatusType.Success;

            return result;
        }


        public async Task<ResponseResult<List<UserVM>>> GetClientNameByUser(long Id)
        {
            ResponseResult<List<UserVM>> result = new ResponseResult<List<UserVM>>();
            List<UserVM> autos = await _dbContext.Autos
                .Include(c => c.Buyer)
                .Where(c => c.CreationUserId == Id || c.BuyerId == Id)
                .Select(c => new UserVM
                {
                    Id = c.BuyerId.Value,
                    Name = c.Buyer.Name
                }).Distinct()
                .ToListAsync();

            result.Data = autos;
            result.Errors = null;
            result.Status = StatusType.Success;

            return result;
        }

        public async Task<ResponseResult<List<Auto>>> ArchiveAutos(List<Auto> model)
        {
            ResponseResult<List<Auto>> result = new ResponseResult<List<Auto>>();
            result.Status = StatusType.Failed;
            bool isSaved = false;

            var depotIds = model.Select(x => x.Id);

            List<AutoImage> dbImagesByAutoId = _dbContext.AutoImages.Where(i => depotIds.Contains(i.AutoId)).ToList();
            if (dbImagesByAutoId.Count > 0)
            {
                await DeleteFiles(dbImagesByAutoId);
                _dbContext.AutoImages.RemoveRange(dbImagesByAutoId);
            }

            List<Auto> autos = _dbContext.Autos.Where(i => depotIds.Contains(i.Id)).ToList();
            autos.ForEach(a => a.IsArchive = 1);

            _dbContext.Autos.UpdateRange(autos);

            isSaved = await _dbContext.SaveChangesAsync() > 0;

            if (isSaved)
            {
                result = new ResponseResult<List<Auto>>();
                result.Errors = null;
                result.Status = StatusType.Success;
            }

            return result;
        }

        public async Task<ResponseResult<List<Auto>>> UnArchiveAutos(List<Auto> model)
        {
            ResponseResult<List<Auto>> result = new ResponseResult<List<Auto>>();
            result.Status = StatusType.Failed;
            bool isSaved = false;

            var depotIds = model.Select(x => x.Id);

            //List<AutoImage> dbImagesByAutoId = _dbContext.AutoImages.Where(i => depotIds.Contains(i.AutoId)).ToList();
            //if (dbImagesByAutoId.Count > 0)
            //{
            //    await DeleteFiles(dbImagesByAutoId);
            //    _dbContext.AutoImages.RemoveRange(dbImagesByAutoId);
            //}

            List<Auto> autos = _dbContext.Autos.Where(i => depotIds.Contains(i.Id)).ToList();
            autos.ForEach(a => a.IsArchive = 0);

            _dbContext.Autos.UpdateRange(autos);

            isSaved = await _dbContext.SaveChangesAsync() > 0;

            if (isSaved)
            {
                result = new ResponseResult<List<Auto>>();
                result.Errors = null;
                result.Status = StatusType.Success;
            }

            return result;
        }
        public async Task<ResponseResult<List<Auto>>> Archive(long Id)
        {
            ResponseResult<List<Auto>> result = new ResponseResult<List<Auto>>();
            result.Status = StatusType.Failed;
            bool isSaved = false;

            List<AutoImage> dbImagesByAutoId = _dbContext.AutoImages.Where(i => i.AutoId == Id).ToList();
            if (dbImagesByAutoId.Count > 0)
            {
                await DeleteFiles(dbImagesByAutoId);
                _dbContext.AutoImages.RemoveRange(dbImagesByAutoId);
            }

            var client = await _dbContext.Autos.FirstOrDefaultAsync(u => u.Id == Id);
            client.IsArchive = 1;
            _dbContext.Autos.Update(client);

            isSaved = await _dbContext.SaveChangesAsync() > 0;

            if (isSaved)
            {
                result = new ResponseResult<List<Auto>>();
                result.Errors = null;
                result.Status = StatusType.Success;
            }

            return result;
        }

        public async Task<ResponseResult<List<Auto>>> DeleteImages(long Id)
        {
            ResponseResult<List<Auto>> result = new ResponseResult<List<Auto>>();
            result.Status = StatusType.Failed;
            bool isSaved = false;


            List<AutoImage> dbImagesByAutoId = _dbContext.AutoImages.Where(i => i.AutoId == Id).ToList();
            if (dbImagesByAutoId.Count > 0)
            {
                await DeleteFiles(dbImagesByAutoId);
                _dbContext.AutoImages.RemoveRange(dbImagesByAutoId);
                isSaved = await _dbContext.SaveChangesAsync() > 0;
                if (isSaved)
                {
                    result = new ResponseResult<List<Auto>>();
                    result.Errors = null;
                    result.Status = StatusType.Success;
                }
            }
            else
            {
                result = new ResponseResult<List<Auto>>();
                result.Errors = null;
                result.Status = StatusType.Success;
            }
            return result;
        }
    }
}
