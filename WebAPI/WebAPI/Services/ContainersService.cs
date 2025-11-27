using Entities;
using System.Globalization;
using WebAPI.General;
using WebAPI.ViewModel.Users;

namespace WebAPI.Services
{
    public class ContainersService : EnumExtension, IContainersService
    {
        public readonly AmdDBContext _dbContext;
        public readonly IConfiguration _configuration;
        private readonly IStringLocalizer<SharedResource> _sharedResourceLocalizer;
        public ContainersService(AmdDBContext dbContext, IConfiguration configuration, IStringLocalizer<SharedResource> sharedResourceLocalizer)
        {
            _dbContext = dbContext;
            _configuration = configuration;
            _sharedResourceLocalizer = sharedResourceLocalizer;
        }

        public async Task<ResponseResult<List<Container>>> GetAllByUser(long id, ContainerFilterParams search)
        {

            ResponseResult<List<Container>> result = new ResponseResult<List<Container>>();
            List<Container> containers = await _dbContext.Containers
                .Include(a => a.Autos)
                .Include(a => a.Destination)
                .Include(a => a.ShippingCompany)
                .Include(a => a.DeparturePort)
                .Include(a => a.Images)
                .Where(c => (c.CreationUserId == id || c.Autos.Any(x => x.BuyerId == id)) && c.IsArchive == 0)
                .ToListAsync();

            List<LookupValue> lookupValues = _dbContext.LookupValues.ToList();

            List<Container> models = containers
                .Where(c => ((search.StatusId > 0) ? SwitchConditional.SwitchConditionalContainerStatus(c, search.StatusId.Value) : true))
                .Select(c => new Container
                {
                    Id = c.Id,
                    Name = c.Name,
                    SerialNumber = c.SerialNumber,
                    BookNo = c.BookNo,
                    DepartureDate = c.DepartureDate,
                    DepartureDateStr = c.DepartureDate.Value.ToString("dd/MM/yyyy", new CultureInfo("en-US")),
                    ArrivalDateStr = c.ArrivalDate.Value.ToString("dd/MM/yyyy", new CultureInfo("en-US")),
                    ArrivalDate = c.ArrivalDate,
                    DeparturePortId = c.DeparturePortId,
                    ShippingCompanyId = c.ShippingCompanyId,
                    CreationDate = c.CreationDate,
                    CreationUserId = c.CreationUserId,
                    DestinationId = c.DestinationId,
                    Status = c.Status,
                    ModificationUserId = c.ModificationUserId,
                    ModificationDate = c.ModificationDate,
                    DestinationName = c.Destination.Name,
                    ShippingCompanyName = c.ShippingCompany.Name,
                    DeparturePortName = c.DeparturePort.Name,
                    ContainerStatusStr = SwitchStatusByDate(c.DepartureDate, c.ArrivalDate).Status,
                    IsArchive = c.IsArchive,
                    RowColor = SwitchStatusByDate(c.DepartureDate, c.ArrivalDate).Color,
                    Image = c.Images.Count > 0 ? c.Images.FirstOrDefault().Title : "",
                    AutoIds = c.Autos.Where(a => a.ContainerId == c.Id).Select(x => x.Id).ToList(),
                    AutoVinNos = c.Autos.Where(a => (a.CreationUserId == id || a.BuyerId == id)).Select(a => new Hook<long, string>
                    {
                        Value = a.Id,
                        Text = a.VinNo
                    }).ToList(),
                    SearchDepartureDate =  c.DepartureDate.HasValue ? c.DepartureDate : default(DateTime),
                    SearchArrivalDate =  c.ArrivalDate.HasValue ? c.ArrivalDate : default(DateTime)
                })
                .OrderByDescending(x => x.CreationDate)
                .ToList();

            if (search.IsSearch)
            {
                if (!string.IsNullOrEmpty(search.ContainerNo)) models = models.Where(m => m.SerialNumber.Contains(search.ContainerNo)).ToList();
                if (!string.IsNullOrEmpty(search.BookingNo)) models = models.Where(m => m.BookNo.Contains(search.BookingNo)).ToList();
                if (search.LoadPortId > 0) models = models.Where(m => m.DeparturePortId == search.LoadPortId).ToList();
                if (search.DestinationId > 0) models = models.Where(m => m.DestinationId == search.DestinationId).ToList();
                if (search.ShippingLineId > 0) models = models.Where(m => m.ShippingCompanyId == search.ShippingLineId).ToList();

                if (search.LoadingFromDate.HasValue)
                    if (!search.LoadingToDate.HasValue)
                        models = models.Where(m => m.SearchDepartureDate?.Date == search.LoadingFromDate.Value.Date).ToList();
                    else
                        models = models.Where(m => m.SearchDepartureDate?.Date >= search.LoadingFromDate.Value.Date).ToList();

                if (search.LoadingToDate.HasValue)
                    if (!search.LoadingFromDate.HasValue)
                        models = models.Where(m => m.SearchDepartureDate?.Date == search.LoadingToDate.Value.Date).ToList();
                    else
                        models = models.Where(m => m.SearchDepartureDate?.Date <= search.LoadingToDate.Value.Date).ToList();

                if (search.ArrivalFromDate.HasValue)
                    if (!search.ArrivalToDate.HasValue)
                        models = models.Where(m => m.SearchArrivalDate?.Date == search.ArrivalFromDate.Value.Date).ToList();
                    else
                        models = models.Where(m => m.SearchArrivalDate?.Date >= search.ArrivalFromDate.Value.Date).ToList();


                if (search.ArrivalToDate.HasValue)
                    if (!search.ArrivalFromDate.HasValue)
                        models = models.Where(m => m.SearchArrivalDate?.Date == search.ArrivalToDate.Value.Date).ToList();
                else
                        models = models.Where(m => m.SearchArrivalDate?.Date <= search.ArrivalToDate.Value.Date).ToList();

            }

            result.Data = models;
            result.Errors = null;
            result.Status = StatusType.Success;
            return result;
        }

        public async Task<ResponseResult<List<Container>>> GetAllArchiveByUser(long id, ContainerFilterParams search)
        {

            ResponseResult<List<Container>> result = new ResponseResult<List<Container>>();
            List<Container> containers = await _dbContext.Containers
                .Include(a => a.Autos)
                .Include(a => a.Destination)
                .Include(a => a.ShippingCompany)
                .Include(a => a.DeparturePort)
                .Include(a => a.Images)
                .Where(c => (c.CreationUserId == id || c.Autos.Any(x => x.BuyerId == id)) && c.IsArchive == 1)
                .ToListAsync();

            List<LookupValue> lookupValues = _dbContext.LookupValues.ToList();

            List<Container> models = containers
                .Where(c => ((search.StatusId > 0) ? SwitchConditional.SwitchConditionalContainerStatus(c, search.StatusId.Value) : true))
                .Select(c => new Container
                {
                    Id = c.Id,
                    Name = c.Name,
                    SerialNumber = c.SerialNumber,
                    BookNo = c.BookNo,
                    DepartureDate = c.DepartureDate,
                    DepartureDateStr = c.DepartureDate.Value.ToString("dd/MM/yyyy", new CultureInfo("en-US")),
                    ArrivalDateStr = c.ArrivalDate.Value.ToString("dd/MM/yyyy", new CultureInfo("en-US")),
                    ArrivalDate = c.ArrivalDate,
                    DeparturePortId = c.DeparturePortId,
                    ShippingCompanyId = c.ShippingCompanyId,
                    CreationDate = c.CreationDate,
                    CreationUserId = c.CreationUserId,
                    DestinationId = c.DestinationId,
                    Status = c.Status,
                    ModificationUserId = c.ModificationUserId,
                    ModificationDate = c.ModificationDate,
                    DestinationName = c.Destination.Name,
                    ShippingCompanyName = c.ShippingCompany.Name,
                    DeparturePortName = c.DeparturePort.Name,
                    ContainerStatusStr = SwitchStatusByDate(c.DepartureDate, c.ArrivalDate).Status,
                    IsArchive = c.IsArchive,
                    RowColor = SwitchStatusByDate(c.DepartureDate, c.ArrivalDate).Color,
                    Image = c.Images.Count > 0 ? c.Images.FirstOrDefault().Title : "",
                    AutoIds = c.Autos.Where(a => a.ContainerId == c.Id).Select(x => x.Id).ToList(),
                    AutoVinNos = c.Autos.Where(a => (a.CreationUserId == id || a.BuyerId == id)).Select(a => new Hook<long, string>
                    {
                        Value = a.Id,
                        Text = a.VinNo
                    }).ToList(),
                    SearchDepartureDate = c.DepartureDate.HasValue ? c.DepartureDate : default(DateTime),
                    SearchArrivalDate = c.ArrivalDate.HasValue ? c.ArrivalDate : default(DateTime)
                })
                .OrderByDescending(x => x.CreationDate)
                .ToList();

            //if (search.IsSearch)
            //{
            //    if (!string.IsNullOrEmpty(search.ContainerNo)) models = models.Where(m => m.SerialNumber.Contains(search.ContainerNo)).ToList();
            //    if (!string.IsNullOrEmpty(search.BookingNo)) models = models.Where(m => m.BookNo.Contains(search.BookingNo)).ToList();
            //    if (search.LoadPortId > 0) models = models.Where(m => m.DeparturePortId == search.LoadPortId).ToList();
            //    if (search.DestinationId > 0) models = models.Where(m => m.DestinationId == search.DestinationId).ToList();
            //    if (search.ShippingLineId > 0) models = models.Where(m => m.ShippingCompanyId == search.ShippingLineId).ToList();

            //    if (search.LoadingFromDate.HasValue)
            //        if (!search.LoadingToDate.HasValue)
            //            models = models.Where(m => m.SearchDepartureDate?.Date == search.LoadingFromDate.Value.Date).ToList();
            //        else
            //            models = models.Where(m => m.SearchDepartureDate?.Date >= search.LoadingFromDate.Value.Date).ToList();

            //    if (search.LoadingToDate.HasValue)
            //        if (!search.LoadingFromDate.HasValue)
            //            models = models.Where(m => m.SearchDepartureDate?.Date == search.LoadingToDate.Value.Date).ToList();
            //        else
            //            models = models.Where(m => m.SearchDepartureDate?.Date <= search.LoadingToDate.Value.Date).ToList();

            //    if (search.ArrivalFromDate.HasValue)
            //        if (!search.ArrivalToDate.HasValue)
            //            models = models.Where(m => m.SearchArrivalDate?.Date == search.ArrivalFromDate.Value.Date).ToList();
            //        else
            //            models = models.Where(m => m.SearchArrivalDate?.Date >= search.ArrivalFromDate.Value.Date).ToList();


            //    if (search.ArrivalToDate.HasValue)
            //        if (!search.ArrivalFromDate.HasValue)
            //            models = models.Where(m => m.SearchArrivalDate?.Date == search.ArrivalToDate.Value.Date).ToList();
            //        else
            //            models = models.Where(m => m.SearchArrivalDate?.Date <= search.ArrivalToDate.Value.Date).ToList();

            //}

            result.Data = models;
            result.Errors = null;
            result.Status = StatusType.Success;
            return result;
        }

        public async Task<ResponseResult<List<Container>>> SaveContainer(Container model, long UserId)
        {
            ResponseResult<List<Container>> result = new ResponseResult<List<Container>>();
            List<Container> dbContainsers = await _dbContext.Containers.AsNoTracking().ToListAsync();
            result.Status = StatusType.Failed;
            bool isSaved = false;

            var containsers = await _dbContext.Containers.FirstOrDefaultAsync(u => u.SerialNumber == model.SerialNumber && u.Id != model.Id);
            if (containsers != null)
            {
                result.Data = null;
                result.Errors = new List<string> { _sharedResourceLocalizer["Serialnumberalreadyexist"] };
                result.Status = StatusType.Failed;
                return result;
            }


            if (model.ArrivalDate.HasValue)
            {
                model.ArrivalDate = model.ArrivalDate.Value.ToLocalTime();
            }
            if (model.DepartureDate.HasValue)
            {
                model.DepartureDate = model.DepartureDate.Value.ToLocalTime();
            }

            if (model.Id < 1)
            {
                model.IsArchive = 0;
                model.CreationDate = DateTime.Now;
                model.CreationUserId = UserId;
                model.Status = (model.ArrivalDate.HasValue && model.ArrivalDate <= DateTime.Now) ? (int)ContainerStatus.Arrived : (int)ContainerStatus.Departured;
                _dbContext.Containers.Add(model);
            }
            else
            {
                if (model.IsArchive == null)
                    model.IsArchive = 0;
                model.ModificationDate = DateTime.Now;
                model.ModificationUserId = UserId;
                model.Status = (model.ArrivalDate.HasValue && model.ArrivalDate <= DateTime.Now) ? (int)ContainerStatus.Arrived : (int)ContainerStatus.Departured;
                _dbContext.Containers.Update(model);
            }

            isSaved = await _dbContext.SaveChangesAsync() > 0;

            if (isSaved)
            {
                List<Container> list = new List<Container>();
                list.Add(model);

                result.Data = list;
                await SaveContainerAutos(model, result.Data);
                result.Errors = null;
                result.Status = StatusType.Success;
            }

            return result;
        }

        private async Task<bool> SaveContainerAutos(Container container, List<Container> containers)
        {
            bool isSaved = false;
            long latestContainerId = 0;
            List<Auto> autos = new List<Auto>();
            List<Auto> autosByContainer = new List<Auto>();
            if (container.Id == 0) // Add mode
            {
                Container latestContainer = containers.OrderByDescending(x => x.CreationDate).FirstOrDefault();
                latestContainerId = latestContainer.Id;
                if ((container.AutoIds != null) && (container.AutoIds.Count > 0))
                {
                    autos = _dbContext.Autos.Where(a => container.AutoIds.Any(c => c == a.Id)).ToList();
                    foreach (var auto in autos)
                    {
                        auto.ContainerId = latestContainerId;
                    }
                }
            }
            else // Update mode
            {
                latestContainerId = container.Id;
                autosByContainer = _dbContext.Autos.Where(a => a.ContainerId == container.Id).ToList();
                foreach (var auto in autosByContainer)
                {
                    auto.ContainerId = null;
                }
                if ((container.AutoIds != null) && (container.AutoIds.Count > 0))
                {
                    autos = _dbContext.Autos.Where(a => container.AutoIds.Any(c => c == a.Id)).ToList();
                    foreach (var auto in autos)
                    {
                        auto.ContainerId = latestContainerId;
                        _dbContext.Autos.Update(auto);
                    }
                }
            }

            isSaved = await _dbContext.SaveChangesAsync() > 0;

            return isSaved;
        }


        public async Task<ResponseResult<List<Container>>> DeleteContainers(List<Container> models)
        {
            ResponseResult<List<Container>> result = new ResponseResult<List<Container>>();
            result.Status = StatusType.Failed;
            bool isSaved = false;

            var depotIds = models.Select(x => x.Id);

            List<Auto> relatedAutos = new List<Auto>();
            relatedAutos = _dbContext.Autos.Where(i => depotIds.Contains(i.ContainerId.Value)).ToList();
            relatedAutos.ForEach(x => x.ContainerId = null);

            List<ContainerImages> dbImagesByContainerId = _dbContext.ContainerImages.Where(i => depotIds.Contains(i.ContainerId)).ToList();
            await DeleteFiles(dbImagesByContainerId);
            _dbContext.ContainerImages.RemoveRange(dbImagesByContainerId);

            _dbContext.Containers.RemoveRange(models);

            try
            {
                isSaved = await _dbContext.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                ;
            }

            if (isSaved)
            {
                result = new ResponseResult<List<Container>>();
                result.Errors = null;
                result.Status = StatusType.Success;
            }

            return result;
        }

        public dynamic GetSideMenuByUser(long id, byte isArchive)
        {
            dynamic result = null;
            List<Container> containers = _dbContext.Containers.Include(x => x.Autos).Where(c => (c.CreationUserId == id || c.Autos.Any(a => a.BuyerId == id)) && c.IsArchive == isArchive).ToList();
            List<LookupValue> departurePorts = _dbContext.LookupValues.Where(c => c.LookupId == 2).ToList();

            var all = new
            {
                Awaitingload = containers.Where(a => (a.DepartureDate > DateTime.Now && a.ArrivalDate > DateTime.Now)).Count(),
                Departured = containers.Where(b => b.DepartureDate <= DateTime.Now && b.ArrivalDate > DateTime.Now).Count(),
                Arrived = containers.Where(c => c.ArrivalDate <= DateTime.Now).Count()
            };

            var filteredContainers = departurePorts.Select(item => new
            {
                departurePortId = item.Id,
                departurePort = item.Name,
                Awaitingload = containers.Where(a => a.DeparturePortId == item.Id && ((a.DepartureDate > DateTime.Now && a.ArrivalDate > DateTime.Now))).Count(),
                Departured = containers.Where(b => b.DepartureDate <= DateTime.Now && b.ArrivalDate > DateTime.Now && b.DeparturePortId == item.Id).Count(),
                Arrived = containers.Where(c => c.ArrivalDate <= DateTime.Now && c.DeparturePortId == item.Id).Count()
            }).ToList();

            result = new { all, filteredContainers };

            return result;
        }

        public async Task<bool> SaveImages(IFormFileCollection uploadFiles, long containerId, long userId)
        {
            bool isSaved = false;
            string strFolderPath = _configuration["Attachmentspath"];
            List<ContainerImages> dbImagesByAutoId = _dbContext.ContainerImages.Where(x => x.ContainerId == containerId).ToList();
            List<ContainerImages> newImages = new List<ContainerImages>();
            foreach (var file in uploadFiles)
            {
                var newFileName = file.FileName;
                if (file.FileName.Contains("."))
                {
                    var at = file.FileName.IndexOf('.');
                    newFileName = DateTime.Now.ToString("yyMMdd") + "-" + file.FileName.Substring(0, at) + "C" + containerId + file.FileName.Substring(at);
                }
                if (dbImagesByAutoId.Any(x => x.Title == newFileName))
                {
                    continue;
                }
                var filePath = Path.Combine(strFolderPath, newFileName);
                ContainerImages newImage = new ContainerImages()
                {
                    ContainerId = containerId,
                    Alt = newFileName,
                    Title = newFileName,
                    PreviewImageSrc = filePath,
                    ThumbnailImageSrc = filePath,
                    Extintion = null,
                    Path = filePath,
                    CreationDate = DateTime.Now,
                    CreationUserId = userId,
                    ModificationDate = null,
                    ModificationUserId = null
                };
                newImages.Add(newImage);
            }
            _dbContext.ContainerImages.AddRange(newImages);
            isSaved = await _dbContext.SaveChangesAsync() > 0;
            return isSaved;
        }

        public async Task<ResponseResult<Container>> GetContainerById(long id)
        {
            ResponseResult<Container> result = new ResponseResult<Container>();
            List<Container> containers = await _dbContext.Containers
                .Include(c => c.Autos)
                .Include(c => c.Images)
                .Where(c => c.Id == id).ToListAsync();
            List<LookupValue> lookupValues = _dbContext.LookupValues.ToList();

            Container models = containers.Select(c => new Container
            {
                Id = c.Id,
                Name = c.Name,
                BookNo = c.BookNo,
                SerialNumber = c.SerialNumber,
                DeparturePortId = c.DeparturePortId,
                DestinationId = c.DestinationId,
                ShippingCompanyId = c.ShippingCompanyId,
                DepartureDateStr = c.DepartureDate.Value.ToString("dd/MM/yyyy", new CultureInfo("en-US")),
                ArrivalDateStr = c.ArrivalDate.Value.ToString("dd/MM/yyyy", new CultureInfo("en-US")),
                //DepartureDate = c.DepartureDate,
                //ArrivalDate = c.ArrivalDate,
                Status = c.Status,
                IsArchive = c.IsArchive,
                DestinationName = lookupValues.Where(l => l.Id == c.DestinationId).FirstOrDefault().Name,
                ShippingCompanyName = lookupValues.Where(l => l.Id == c.ShippingCompanyId).FirstOrDefault().Name,
                DeparturePortName = lookupValues.Where(l => l.Id == c.DeparturePortId).FirstOrDefault().Name,
                ContainerStatusStr = SwitchContainerStatus(c.Status.Value),
                AutoIds = _dbContext.Autos.Where(a => a.ContainerId == c.Id).Select(x => x.Id).ToList(),
                AutoVinNos = c.Autos.Select(a => new Hook<long, string>
                {
                    Value = a.Id,
                    Text = a.VinNo
                }).ToList(),
                CreationDate = c.CreationDate,
                CreationUserId = c.CreationUserId,
                ModificationUserId = c.ModificationUserId,
                ModificationDate = c.ModificationDate,
                Images = c.Images.Select(x => new ContainerImages
                {
                    Id = x.Id,
                    Alt = x.Alt,
                    Title = x.Title,
                    PreviewImageSrc = x.PreviewImageSrc,
                    ThumbnailImageSrc = x.ThumbnailImageSrc,
                    ContainerId = x.ContainerId
                }).ToList()
            }).FirstOrDefault();

            result.Data = models;
            result.Errors = null;
            result.Status = StatusType.Success;

            return result;
        }

        public async Task<bool> DeleteFiles(List<ContainerImages> dbImages)
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

        public async Task<ResponseResult<List<Container>>> Delete(long Id)
        {
            ResponseResult<List<Container>> result = new ResponseResult<List<Container>>();
            result.Status = StatusType.Failed;
            bool isSaved = false;

            List<Auto> relatedAutos = new List<Auto>();
            relatedAutos = _dbContext.Autos.Where(i => i.ContainerId == Id).ToList();
            relatedAutos.ForEach(x => x.ContainerId = null);

            List<ContainerImages> dbImagesByContainerId = _dbContext.ContainerImages.Where(i => i.ContainerId == Id).ToList();
            await DeleteFiles(dbImagesByContainerId);
            _dbContext.ContainerImages.RemoveRange(dbImagesByContainerId);

            var container = await _dbContext.Containers.FirstOrDefaultAsync(u => u.Id == Id);
            _dbContext.Containers.Remove(container);

            try
            {
                isSaved = await _dbContext.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                ;
            }

            if (isSaved)
            {
                result = new ResponseResult<List<Container>>();
                result.Errors = null;
                result.Status = StatusType.Success;
            }

            return result;
        }

        public async Task<ResponseResult<List<Container>>> ArchiveContainers(List<Container> model)
        {
            ResponseResult<List<Container>> result = new ResponseResult<List<Container>>();
            result.Status = StatusType.Failed;
            bool isSaved = false;

            var depotIds = model.Select(x => x.Id);

            List<ContainerImages> dbImagesBycontainerId = _dbContext.ContainerImages.Where(i => depotIds.Contains(i.ContainerId)).ToList();
            if (dbImagesBycontainerId.Count > 0)
            {
                await DeleteFiles(dbImagesBycontainerId);
                _dbContext.ContainerImages.RemoveRange(dbImagesBycontainerId);
            }

            List<Container> containers = _dbContext.Containers.Where(i => depotIds.Contains(i.Id)).ToList();
            containers.ForEach(a => a.IsArchive = 1);

            _dbContext.Containers.UpdateRange(containers);

            isSaved = await _dbContext.SaveChangesAsync() > 0;

            if (isSaved)
            {
                result = new ResponseResult<List<Container>>();
                result.Errors = null;
                result.Status = StatusType.Success;
            }

            return result;
        }

        public async Task<ResponseResult<List<Container>>> UnArchiveContainers(List<Container> model)
        {
            ResponseResult<List<Container>> result = new ResponseResult<List<Container>>();
            result.Status = StatusType.Failed;
            bool isSaved = false;

            var depotIds = model.Select(x => x.Id);

            //List<ContainerImages> dbImagesBycontainerId = _dbContext.ContainerImages.Where(i => depotIds.Contains(i.ContainerId)).ToList();
            //if (dbImagesBycontainerId.Count > 0)
            //{
            //    await DeleteFiles(dbImagesBycontainerId);
            //    _dbContext.ContainerImages.RemoveRange(dbImagesBycontainerId);
            //}

            List<Container> containers = _dbContext.Containers.Where(i => depotIds.Contains(i.Id)).ToList();
            containers.ForEach(a => a.IsArchive = 0);

            _dbContext.Containers.UpdateRange(containers);

            isSaved = await _dbContext.SaveChangesAsync() > 0;

            if (isSaved)
            {
                result = new ResponseResult<List<Container>>();
                result.Errors = null;
                result.Status = StatusType.Success;
            }

            return result;
        }

        public async Task<ResponseResult<List<Container>>> Archive(long Id)
        {
            ResponseResult<List<Container>> result = new ResponseResult<List<Container>>();
            result.Status = StatusType.Failed;
            bool isSaved = false;

            List<ContainerImages> dbImagesByContainerId = _dbContext.ContainerImages.Where(i => i.ContainerId == Id).ToList();
            if (dbImagesByContainerId.Count > 0)
            {
                await DeleteFiles(dbImagesByContainerId);
                _dbContext.ContainerImages.RemoveRange(dbImagesByContainerId);
            }

            var container = await _dbContext.Containers.FirstOrDefaultAsync(u => u.Id == Id);
            container.IsArchive = 1;
            _dbContext.Containers.Update(container);

            isSaved = await _dbContext.SaveChangesAsync() > 0;

            if (isSaved)
            {
                result = new ResponseResult<List<Container>>();
                result.Errors = null;
                result.Status = StatusType.Success;
            }

            return result;
        }

        public async Task<ResponseResult<List<Container>>> DeleteImages(long Id)
        {
            ResponseResult<List<Container>> result = new ResponseResult<List<Container>>();
            result.Status = StatusType.Failed;
            bool isSaved = false;


            List<ContainerImages> dbImagesBycontainerId = _dbContext.ContainerImages.Where(i => i.ContainerId == Id).ToList();
            if (dbImagesBycontainerId.Count > 0)
            {
                await DeleteFiles(dbImagesBycontainerId);
                _dbContext.ContainerImages.RemoveRange(dbImagesBycontainerId);
                isSaved = await _dbContext.SaveChangesAsync() > 0;
                if (isSaved)
                {
                    result = new ResponseResult<List<Container>>();
                    result.Errors = null;
                    result.Status = StatusType.Success;
                }
            }
            else
            {
                result = new ResponseResult<List<Container>>();
                result.Errors = null;
                result.Status = StatusType.Success;
            }
            return result;
        }
    }
}
