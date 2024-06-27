using TeamServer.Models.Listeners;

namespace TeamServer.Services
{
    public interface IListenerServices
    {
       void AddListener(Listener listener);
        IEnumerable<Listener> GetListeners();
        Listener GetListener(string name);
        void RemoveListener(Listener listener);
    }


    public class ListenerService : IListenerServices
    {

        private readonly List<Listener> _listeners = new();

        public void AddListener(Listener listener)
        {
            _listeners.Add(listener);
        }

        public Listener GetListener(string name)
        {
            return _listeners.FirstOrDefault(l =>  l.Name.Equals(name));
        }

        public IEnumerable<Listener> GetListeners()
        {
            return _listeners;
        }

        public void RemoveListener(Listener listener)
        {
            _listeners.Remove(listener);
        }
    }
}
