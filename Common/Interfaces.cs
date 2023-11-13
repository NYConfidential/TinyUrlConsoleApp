using TinyUrlConsoleApp.Utilities;

internal interface IUrlModel : IDataMapKey<string>
{
    string Url { get; }
}