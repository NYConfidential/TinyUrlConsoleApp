using TinyUrlConsoleApp.Common;
using TinyUrlConsoleApp.Utilities;

namespace TinyUrlConsoleApp
{
    internal class TinyURLDataRepository
    {
        private readonly DataMap<string, ILongUrlModel> _longURLMap;
        private readonly DataMap<string, ITinyURLModel> _tinyURLMap;

        public TinyURLDataRepository()
        {
            _tinyURLMap = new("tinyURLMap");
            _longURLMap = new("longURLMap");

            _tinyURLMap.OnDataDeleted += TinyURLMap_OnDataDeleted;

        }

        private void TinyURLMap_OnDataDeleted(ITinyURLModel tinyModel)
        {
            string parentUrl = tinyModel.ParentLongUrlModel.Url;

            ILongUrlModel? longModel = _longURLMap.GetValueByKey(parentUrl);

            if(longModel is null)
            {
                throw new DataMisalignedException($"LongUrl for Not Found for TinyUrl: {tinyModel.Url}");
            }

            longModel.Delete(tinyModel);

            if(!longModel.AllTinyUrls.Any())
            {
                Reporter.WriteToConsole("Not Found");

            }
        }

        public void ProcessRequest(string request = "")
        {
            if(request is null || request == string.Empty) return;

            Reporter reporter = new(_longURLMap, _tinyURLMap);

            try
            {
                string[] requestSplit = request.Split("|").Select(item => item.Trim().ToLower()).ToArray();

                string operation = requestSplit[0];
                ArraySegment<string> requestParams = new(requestSplit, 1, requestSplit.Length - 1);

                switch(operation)
                {
                    case "add":
                        Add(requestParams);
                        reporter.DisplayAllTinyUrlsForLongUrl(requestParams[0]);
                        break;

                    case "everyting":
                        reporter.DisplayAllLongUrls(includeTinyUrls: true);
                        break;

                    case "selectalltinyurls":
                        reporter.DisplayAllTinyUrls();
                        break;

                    case "selectalllongurls":
                        reporter.DisplayAllLongUrls();
                        break;

                    case "selectlongfromtiny":
                        reporter.DisplayLongUrl(requestParams[0]);
                        break;

                    case "click":
                        Click(requestParams[0]);
                        break;

                    case "selectclicks":
                        reporter.DisplayClickCount(requestParams[0]);
                        break;

                    case "selectredirects":
                        reporter.DisplayRedirectsCount(requestParams[0]);
                        break;

                    case "deletetiny":
                        DeleteTiny(requestParams);
                        reporter.DisplayAllTinyUrls();
                        break;

                    case "deletelong":
                        DeleteLong(requestParams);
                        reporter.DisplayAllLongUrls();
                        reporter.DisplayAllTinyUrls();
                        break;

                    default:
                        Reporter.WriteToConsole($"Invalid operation: {operation}");
                        break;
                }
            }
            catch(Exception ex)
            {
                Reporter.WriteToConsole($"ERROR: {ex.Message}");
                Reporter.WriteToConsole($"Stack: {ex.StackTrace}");
            }

        }

        public void Add(IList<string> requestParams)
        {
            ILongUrlModel longUrlModel =
                _longURLMap.GetValueByKey(requestParams[0]) ??
                    new LongUrlModel(requestParams[0]);

            List<ITinyURLModel> tinyURLs = new();

            for(int i = 1; i < requestParams.Count; i++)
            {
                string url = requestParams[i].Trim().ToLower();

                if(url != string.Empty)
                {
                    if(_tinyURLMap.HasKey(url))
                    {
                        Reporter.WriteToConsole($"{url} already exists - SKIPPED");
                    }
                    else
                    {
                        TinyURLModel tinyURLModel = new(longUrlModel, url);
                        tinyURLs.Add(tinyURLModel);
                    }
                }

            }

            longUrlModel.AddOrUpdate(tinyURLs);
            _longURLMap.ModifyData(longUrlModel, ModifyDataEnum.AddOrUpdate);
            _tinyURLMap.ModifyData(longUrlModel.AllTinyUrls, ModifyDataEnum.AddOrUpdate);

        }

        public void DeleteLong(IList<string> longUrls)
        {
            foreach(string url in longUrls)
            {
                ILongUrlModel? longModel = _longURLMap.GetValueByKey(url);

                if(longModel != null)
                {
                    foreach(ITinyURLModel tinyModel in longModel.AllTinyUrls)
                    {
                        _tinyURLMap.ModifyData(tinyModel, ModifyDataEnum.Delete);
                    }

                    _longURLMap.ModifyData(longModel, ModifyDataEnum.Delete);
                }
                else
                {
                    throw new ArgumentNullException($"LongUrl:{url} Not Found");
                }
            }
        }

        public void DeleteTiny(IList<string> tinyUrls)
        {
            foreach(string url in tinyUrls)
            {
                ITinyURLModel? tinyModel = _tinyURLMap.GetValueByKey(url);

                if(tinyModel is null)
                {
                    throw new ArgumentNullException($"TinyUrl:{url} Not Found");
                }

                ILongUrlModel? longModel = _longURLMap.GetValueByKey(tinyModel.ParentLongUrlModel.Url);

                if(longModel is null)
                {
                    throw new DataMisalignedException($"LongUrl for Not Found for TinyUrl: {tinyModel.Url}");
                }

                _tinyURLMap.ModifyData(tinyModel, ModifyDataEnum.Delete);

            }

        }

        public void Click(string tinyURLClicked)
        {
            ITinyURLModel? tinyUrl = _tinyURLMap.GetValueByKey(tinyURLClicked);

            if(tinyUrl != null)
            {
                tinyUrl.AddClick();
            }

        }

    }
}
