using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json;
using System.Linq;
using System.Text.RegularExpressions;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static System.Net.Mime.MediaTypeNames;

namespace WebAPI.Services
{
    public class ResourcesService : IResourcesService
    {
        public readonly AmdDBContext _dbContext;
        public readonly IConfiguration _configuration;
        public readonly IMapper _mapper;
        private readonly IStringLocalizer<SharedResource> _sharedResourceLocalizer;
        public ResourcesService(AmdDBContext dbContext, IConfiguration configuration, IMapper mapper, IStringLocalizer<SharedResource> sharedResourceLocalizer)
        {
            _dbContext = dbContext;
            _configuration = configuration;
            _mapper = mapper;
            _sharedResourceLocalizer = sharedResourceLocalizer;
        }

        public async Task<ResponseResult<List<Resources>>> HandleResourcesByUrl(string url)
        {
            ResponseResult<List<Resources>> result = new ResponseResult<List<Resources>>();
            List<Resources> resources = _dbContext.Resources.ToList();

            try
            {
                var enJson = new Dictionary<string, string>();
                var arJson = new Dictionary<string, string>();

                var enResources = resources.Where(x => x.Culture == "en").ToList();
                var arResources = resources.Where(x => x.Culture == "ar").ToList();

                foreach (var res in enResources)
                {
                    enJson.Add(res.Key, res.TextEn);
                }

                foreach (var res in arResources)
                {
                    arJson.Add(res.Key, res.TextAr);
                }

                System.IO.File.WriteAllText(_configuration["ArResourcesPath"], JsonConvert.SerializeObject(arJson));
                System.IO.File.WriteAllText(_configuration["EnResourcesPath"], JsonConvert.SerializeObject(enJson));

                result.Data = resources;
                result.Errors = null;
                result.Status = StatusType.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Errors = new List<string> { ex.Message };
                result.Status = StatusType.Failed;
            }


            return result;
        }

        public async Task<ResponseResult<List<GroupedResources>>> GetResources(string search, int resourceId)
        {
            bool isSearch = !string.IsNullOrEmpty(search);
            List<GroupedResources> model = new List<GroupedResources>();
            ResponseResult<List<GroupedResources>> result = new ResponseResult<List<GroupedResources>>();

            var resources = _dbContext.Resources.ToList();

            var mapModel = _mapper.Map<List<Resources>, List<GroupedResources>>(resources);


            if (isSearch)
            {
                search = search.ToLower();
                mapModel = mapModel.Where(m => ((m.Key != null) ? m.Key.ToLower().Contains(search) : false)
                                        || ((m.English != null) ? m.English.ToLower().Contains(search) : false)
                                        || ((m.Arabic != null) ? m.Arabic.ToLower().Contains(search) : false))
                    .ToList();
            }

            if (resourceId != 0)
            {
                mapModel = mapModel.Where(x => x.Id == resourceId).ToList();
            }

            result.Data = mapModel;
            result.Errors = null;
            result.Status = StatusType.Success;

            return result;
        }

        public async Task<ResponseResult<List<GroupedResources>>> SaveResource(ResourceParamDto body)
        {
            ResponseResult<List<GroupedResources>> result = new ResponseResult<List<GroupedResources>>();
            result.Status = StatusType.Failed;
            bool isSaved = false;

            var resourcesByKey = await _dbContext.Resources.Where(x => x.Key == body.Key && x.Id != body.Id).FirstOrDefaultAsync();

            if (resourcesByKey != null)
            {
                result.Data = null;
                result.Errors = new List<string> { _sharedResourceLocalizer["keyalreadyexist"] };
                result.Status = StatusType.Failed;

                return result;
            }

            Resources resources = new Resources();
            resources.Key = body.Key;
            resources.TextEn = body.English;
            resources.TextAr = body.Arabic;
            resources.Culture = "en";

            if (body.Id < 1)
            {
                _dbContext.Resources.Add(resources);
            }
            else
            {
                resources.Id = body.Id;
                _dbContext.Resources.Update(resources);
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
                result = new ResponseResult<List<GroupedResources>>();
                result.Errors = null;
                result.Status = StatusType.Success;
            }

            return result;
        }

        public async Task<ResponseResult<Resources>> GetResourceByKey(string key)
        {
            throw new NotImplementedException();
        }

        public async Task<ResponseResult<List<ResourcesKeysVM>>> GetResourcesKeys()
        {
            List<ResourcesKeysVM> model = new List<ResourcesKeysVM>();
            ResponseResult<List<ResourcesKeysVM>> result = new ResponseResult<List<ResourcesKeysVM>>();

            var resources = await _dbContext.Resources.ToListAsync();

            var mapModel = _mapper.Map<List<Resources>, List<ResourcesKeysVM>>(resources);

            result.Data = mapModel;
            result.Errors = null;
            result.Status = StatusType.Success;

            return result;
        }

        public async Task<ResponseResult<List<Resources>>> GetHomePageResources()
        {
            ResponseResult<List<Resources>> result = new ResponseResult<List<Resources>>();

            var resources = await _dbContext.Resources.ToListAsync();

            result.Data = resources;
            result.Errors = null;
            result.Status = StatusType.Success;
            return result;
        }
    }
}
