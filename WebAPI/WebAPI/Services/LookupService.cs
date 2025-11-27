using AutoMapper;
using System.Linq;

namespace WebAPI.Services
{
    public class LookupService : ILookupService
    {
        public readonly AmdDBContext _dbContext;
        public readonly IMapper _mapper;
        private readonly IStringLocalizer<SharedResource> _sharedResourceLocalizer;
        public LookupService(AmdDBContext dbContext, IMapper mapper, IStringLocalizer<SharedResource> sharedResourceLocalizer)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _sharedResourceLocalizer = sharedResourceLocalizer;
        }

        public async Task<ResponseResult<List<LookupValueVM>>> GetAll()
        {
            ResponseResult<List<LookupValueVM>> result = new ResponseResult<List<LookupValueVM>>();
            List<LookupValue> lookupValues = await _dbContext.LookupValues.ToListAsync();

            var maplookup = _mapper.Map<List<LookupValue>, List<LookupValueVM>>(lookupValues);
            result.Data = maplookup;
            result.Errors = null;
            result.Status = StatusType.Success;

            return result;
        }

        public async Task<ResponseResult<List<Lookup>>> GetLookups()
        {
            ResponseResult<List<Lookup>> result = new ResponseResult<List<Lookup>>();
            List<Lookup> lookups = _dbContext.Lookups.ToList();

            result.Data = lookups;
            result.Errors = null;
            result.Status = StatusType.Success;

            return result;
        }

        public async Task<ResponseResult<List<LookupValue>>> GetLookupValues(long userId, string search, int LookupId)
        {
            bool isSearch = !string.IsNullOrEmpty(search);
            ResponseResult<List<LookupValue>> result = new ResponseResult<List<LookupValue>>();
            List<LookupValue> lookupValues = _dbContext.LookupValues.Include(x => x.Lookup).ToList();

            List<LookupValue> models = lookupValues
                .Select(c => new LookupValue
                {
                    Id = c.Id,
                    LookupId = c.LookupId,
                    Name = c.Name,
                    Code = c.Code,
                    LookupName = c.Lookup.Name
                })
                .ToList();

            if (isSearch)
            {
                search = search.ToLower();
                models = models.Where(m => ((m.Name != null) ? m.Name.ToLower().Contains(search) : false)
                                        || ((m.LookupName != null) ? m.LookupName.ToLower().Contains(search) : false))
                    .ToList();
            }

            if (LookupId != 0)
            {
                models = models.Where(m => m.LookupId == LookupId).ToList();
            }

            result.Data = models;
            result.Errors = null;
            result.Status = StatusType.Success;

            return result;
        }

        public async Task<ResponseResult<List<LookupValue>>> Save(LookupValue model)
        {
            ResponseResult<List<LookupValue>> result = new ResponseResult<List<LookupValue>>();
            result.Status = StatusType.Failed;
            bool isSaved = false;

            if (model.Id < 1)
            {
                _dbContext.LookupValues.Add(model);
            }
            else
            {
                _dbContext.LookupValues.Update(model);
            }
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
                result = new ResponseResult<List<LookupValue>>();
                result.Errors = null;
                result.Status = StatusType.Success;
            }

            return result;
        }

        public async Task<ResponseResult<List<LookupValue>>> Delete(List<LookupValue> models)
        {
            ResponseResult<List<LookupValue>> result = new ResponseResult<List<LookupValue>>();
            result.Status = StatusType.Failed;
            bool isSaved = false;

            try
            {
                _dbContext.LookupValues.RemoveRange(models);

                isSaved = await _dbContext.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                ;
            }

            if (isSaved)
            {
                result = new ResponseResult<List<LookupValue>>();
                result.Errors = null;
                result.Status = StatusType.Success;
            }
            else
            {
                result.Data = null;
                result.Errors = new List<string> { _sharedResourceLocalizer["Cannotdeletethislookupbecauseithasrelateddata"] };
                result.Status = StatusType.Failed;
            }

            return result;
        }
    }
}
