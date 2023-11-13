using TinyUrlConsoleApp.Common;

namespace TinyUrlConsoleApp.Utilities
{

    public interface IDataMapKey<TKey>
    {
        TKey Id { get; }
    }

    public interface IDataMap<TData>
    {
        event Action<TData>? OnDataDeleted;
    }

    public class DataMap<TKey, TData> : IDataMap<TData> where TKey : notnull where TData : IDataMapKey<TKey>
    {
        private readonly Dictionary<TKey, TData> _map = new();
        private readonly ReaderWriterLockSlim _readerWriterLockSlim = new();
        private readonly string _mapName;

        public DataMap(string mapName)
        {
            _mapName = mapName;
        }

        public event Action<TData>? OnDataDeleted;

        event Action<TData>? IDataMap<TData>.OnDataDeleted
        {
            add { OnDataDeleted += value; }
            remove { OnDataDeleted -= value; }
        }

        public void RaiseOnDataDeletedEvent(TData data)
        {
            if(OnDataDeleted != null)
            {
                OnDataDeleted(data);
            }
        }

        private void ModifyMap(TData data, ModifyDataEnum operation)
        {
            if(data is null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            _map.TryGetValue(data.Id, out TData? existingData);

            switch(operation)
            {
                case ModifyDataEnum.Add:
                    if(existingData != null)
                    {
                        return;
                    }

                    _map.Add(data.Id, data);

                    break;
                case ModifyDataEnum.AddOrUpdate:

                    if(existingData != null)
                    {
                        _map[existingData.Id] = data;
                    }
                    else
                    {
                        _map.Add(data.Id, data);
                    }

                    break;

                case ModifyDataEnum.Delete:
                    if(existingData != null)
                    {
                        _map.Remove(existingData.Id);

                        if(OnDataDeleted != null)
                        {

                            Reporter.WriteToConsole($"{_mapName} Deleting {existingData.Id}");

                            RaiseOnDataDeletedEvent(existingData);
                        }

                    }
                    break;
            }
        }

        public void ModifyData(TData data, ModifyDataEnum operation)
        {
            if(data is null) return;

            _readerWriterLockSlim.EnterWriteLock();
            try
            {
                ModifyMap(data, operation);
            }
            finally
            {
                _readerWriterLockSlim.ExitWriteLock();
            }
        }

        public void ModifyData(IEnumerable<TData> data, ModifyDataEnum operation)
        {
            if(data is null || !data.Any()) return;

            _readerWriterLockSlim.EnterWriteLock();
            try
            {
                foreach(TData dataItem in data)
                {
                    ModifyMap(dataItem, operation);
                }
            }
            finally
            {
                _readerWriterLockSlim.ExitWriteLock();
            }
        }

        public IEnumerable<TData> GetAllValues()
        {
            _readerWriterLockSlim.EnterReadLock();
            try
            {
                return _map.Values;
            }
            finally
            {
                _readerWriterLockSlim.ExitReadLock();
            }
        }

        public TData? GetValue(TKey key)
        {
            _readerWriterLockSlim.EnterReadLock();
            try
            {
                _map.TryGetValue(key, out TData? existingData);
                return existingData;
            }
            finally
            {
                _readerWriterLockSlim.ExitReadLock();
            }
        }

        public bool HasKey(TKey key)
        {
            _readerWriterLockSlim.EnterReadLock();
            try
            {
                return _map.ContainsKey(key);
            }
            finally
            {
                _readerWriterLockSlim.ExitReadLock();
            }
        }

    }
}
