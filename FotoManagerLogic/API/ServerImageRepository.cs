using System.Collections.Generic;
using System.Linq;

namespace FotoManagerLogic.API
{
    public class ServerImageRepository : IServerImageRepository
    {
        /// <inheritdoc />
        public ServerImageRepository()
        {
            Content = new List<ServerImage>();
        }

        private List<ServerImage> Content { get; }

        /// <inheritdoc />
        public void Add(ServerImage entry)
        {
            Content.Add(entry);
        }

        /// <inheritdoc />
        public string GetPath(string id)
        {
            return Content.First(x => x.Id == id).Path;
        }
    }
}