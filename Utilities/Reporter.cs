namespace TinyUrlConsoleApp.Utilities
{
    internal class Reporter
    {
        private readonly DataMap<string, ILongUrlModel> _longURLMap;
        private readonly DataMap<string, ITinyURLModel> _tinyURLMap;

        public Reporter(DataMap<string, ILongUrlModel> longURLMap, DataMap<string, ITinyURLModel> tinyURLMap)
        {
            _longURLMap = longURLMap;
            _tinyURLMap = tinyURLMap;
        }

        public void DisplayAllTinyUrls()
        {
            IEnumerable<ITinyURLModel> data = _tinyURLMap.GetAllValues();

            if(!data.Any())
            {
                throw new DataMisalignedException($"{nameof(data)} Not Found");
            }
            else
            {
                WriteToConsole("All TinyUrl's", false);
                WriteToConsole(data.Select(d => d.Url).ToList());
            }
        }

        public void DisplayAllLongUrls(bool includeTinyUrls = false)
        {
            IEnumerable<ILongUrlModel> longUrls = _longURLMap.GetAllValues();

            if(!longUrls.Any())
            {
                WriteToConsole($"{nameof(longUrls)} Not Found");
            }
            else
            {
                foreach(ILongUrlModel longUrlModel in longUrls)
                {
                    if(includeTinyUrls)
                    {
                        DisplayAllTinyUrlsForLongUrl(longUrlModel.Url);
                    }
                    else
                    {
                        WriteToConsole($"LongUrl: {longUrlModel.Url}", false);
                    }
                }
            }

        }

        public void DisplayAllTinyUrlsForLongUrl(string longUrl)
        {
            ILongUrlModel? longUrlModel = _longURLMap.GetValue(longUrl);

            if(longUrlModel != null)
            {
                WriteToConsole($"LongUrl: {longUrlModel.Url}", false);
                WriteToConsole($"All TinyUrl's linked:", false);
                WriteToConsole(longUrlModel.AllTinyUrls.Select(d => d.Url).ToList());
            }
            else
            {
                throw new DataMisalignedException($"{nameof(longUrlModel)} Not Found");
            }
        }

        public void DisplayLongUrl(string tinyUrl)
        {
            ITinyURLModel? model = _tinyURLMap.GetValue(tinyUrl);

            if(model != null)
            {
                WriteToConsole(model.ParentLongUrlModel.Url);
            }
            else
            {
                WriteToConsole("Not Found");
            }

        }

        public void DisplayClickCount(string tinyUrl)
        {
            ITinyURLModel? model = _tinyURLMap.GetValue(tinyUrl);

            if(model != null)
            {
                WriteToConsole($"TinyUrl: {tinyUrl} has been clicked {model.ClickCount} times");
            }
            else
            {
                WriteToConsole("Not Found");
            }
        }

        public void DisplayRedirectsCount(string longUrl)
        {
            ILongUrlModel? model = _longURLMap.GetValue(longUrl);

            if(model != null)
            {
                WriteToConsole($"TinyUrl: {longUrl} has been redirected to {model.RedirectCount} times");
            }
            else
            {
                WriteToConsole("Not Found");
            }
        }

        public static void WriteToConsole(string message, bool addEmptyLine = true)
        {
            Console.WriteLine(message);
            if(addEmptyLine)
            {
                Console.WriteLine();
            }
        }

        public static void WriteToConsole(IEnumerable<string> messages, bool addEmptyLine = true)
        {
            foreach(string message in messages)
            {
                Console.WriteLine(message);
            }

            if(addEmptyLine)
            {
                Console.WriteLine();
            }
        }
    }
}
