using TinyUrlConsoleApp.Common;
using TinyUrlConsoleApp.Utilities;

namespace TinyUrlConsoleApp
{
    internal interface ILongUrlModel : IUrlModel, IDataMapKey<string>
    {
        void AddOrUpdate(ITinyURLModel tinyURLModel);
        void AddOrUpdate(IEnumerable<ITinyURLModel> tinyURLModels);
        void Delete(ITinyURLModel tinyURLModel);
        void Delete(IEnumerable<ITinyURLModel> tinyURLModels);
        ITinyURLModel? GetTinyURLModelById(string id);
        IEnumerable<ITinyURLModel> AllTinyUrls { get; }
        void IncrementRedirectCount();
        long RedirectCount { get; }

    }

    internal class LongUrlModel : ILongUrlModel
    {
        private readonly string _url;
        private readonly DataMap<string, ITinyURLModel> _tinyURLMap = new("LongUrlModel");
        private readonly object _lockObject = new();
        private long _redirectCount = 0;

        public LongUrlModel(string url)
        {
            _url = url;
        }

        public string Id
        {
            get
            {
                return _url;
            }
        }
        public string Url
        {
            get
            {
                return _url;
            }
        }

        public void AddOrUpdate(ITinyURLModel tinyURLModel)
        {
            lock(_lockObject)
            {
                _tinyURLMap.ModifyData(tinyURLModel, Common.ModifyDataEnum.AddOrUpdate);
            }

        }

        public void AddOrUpdate(IEnumerable<ITinyURLModel> tinyURLModels)
        {
            lock(_lockObject)
            {
                _tinyURLMap.ModifyData(tinyURLModels, Common.ModifyDataEnum.AddOrUpdate);
            }
        }

        public void Delete(ITinyURLModel tinyURLModel)
        {
            lock(_lockObject)
            {
                _tinyURLMap.ModifyData(tinyURLModel, ModifyDataEnum.Delete);
            }

        }

        public void Delete(IEnumerable<ITinyURLModel> tinyURLModels)
        {
            lock(_lockObject)
            {
                _tinyURLMap.ModifyData(tinyURLModels, ModifyDataEnum.Delete);
            }

        }

        public IEnumerable<ITinyURLModel> AllTinyUrls
        {
            get
            {
                lock(_lockObject)
                {
                    return _tinyURLMap.GetAllValues();
                }

            }
        }

        public ITinyURLModel? GetTinyURLModelById(string url)
        {
            lock(_lockObject)
            {
                return _tinyURLMap.GetValue(url);
            }

        }

        public long RedirectCount
        {
            get
            {
                return Interlocked.Read(ref _redirectCount);
            }

        }

        public void IncrementRedirectCount()
        {
            Interlocked.Increment(ref _redirectCount);
        }

    }
}
