using TinyUrlConsoleApp.Utilities;

namespace TinyUrlConsoleApp
{
    internal interface ITinyURLModel : IUrlModel, IDataMapKey<string>
    {
        long ClickCount { get; }
        void AddClick();
        ILongUrlModel ParentLongUrlModel { get; }
    }

    internal class TinyURLModel : ITinyURLModel
    {
        private long _clickCount = 0;
        private readonly ILongUrlModel _parentLongUrlModel;
        private readonly string _url;

        public TinyURLModel(ILongUrlModel parentLongUrlModel, string url)
        {
            _parentLongUrlModel = parentLongUrlModel;
            _url = url;
        }
        public long ClickCount => Interlocked.Read(ref _clickCount);

        public void AddClick()
        {
            Interlocked.Increment(ref _clickCount);
            _parentLongUrlModel.IncrementRedirectCount();
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

        public ILongUrlModel ParentLongUrlModel
        {
            get
            {
                return _parentLongUrlModel;
            }
        }
    }
}
