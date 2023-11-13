using TinyUrlConsoleApp.Utilities;

namespace TinyUrlConsoleApp
{
    internal class TinyURLService
    {
        private readonly TinyURLDataRepository _tinyURLDataRepository = new();

        private readonly List<string> _initialMessages = new()
        {
            "Welcome to TinyURL Service!",
            "---",
            "Commands guide:",
            "---",
            "Add new LongURI:                       Add | LongURI",
            "Add new TinyURL:                       Add | LongURI | TinyURL",
            "Add multiple TinyURLs:                 Add | LongURI | TinyURL | TinyURL etc",
            "",
            "Delete LongURI(s):                     DeleteLong | LongURI | LongURI ...N(LongURI) (Will delete all associated TinyURL's)",
            "Delete TinyURL(s):                     DeleteTiny | TinyURL | TinyURL ...N(TinyURL)",
            "",

            "Select EVERYTING:                      EVERYTING",
            "Select LongURI from a TinyURL:         SelectLongFromTiny | TinyURL",
            "Select ALL LongURLs:                   SelectAllLongURLs",
            "Select ALL TinyURLs:                   SelectAllTinyURLs",
            "Select TinyURL's from a LongURI:       SelectAllTinyURLs | LongURI",
            "",
            "To 'click' TinyURL:                    Click | TinyURL",
            "Select 'click' count:                  SelectClicks | TinyURL",
            "Select 'redirect' count:               SelectRedirects | LongURI",
            "---",
            "All commands are case insensitive",
            "All spaces can be ignored",
            "---",
            "Type 'Done' to terminate",
            "",
            "",
            "Have Fun!",
            "",
            "",
        };

        public void Run()
        {
            foreach(string message in _initialMessages)
            {
                Reporter.WriteToConsole(message, false);
            }

            string processInput = Console.ReadLine() ?? "";
            ProcessInput(processInput);
        }

        private void ProcessInput(string input = "")
        {
            string processInput = input;

            if(processInput == "done")
            {
                Environment.Exit(0);
            }
            else
            {
                Reporter.WriteToConsole("*********", false);
                _tinyURLDataRepository.ProcessRequest(processInput);

                processInput = Console.ReadLine() ?? "";
                ProcessInput(processInput);
            }

        }
    }
}
