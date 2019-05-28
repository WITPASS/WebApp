using Admin.Shared;
using Data;

namespace Admin.Pages
{
    public class ImagesComponent : BaseComponent<Image>
    {
        public ImagesComponent() : base("api/images") { }
    }
}
