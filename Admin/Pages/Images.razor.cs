using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Data;

namespace Admin.Pages
{
    public class ImagesComponent : BaseComponent<Image>
    {
        public ImagesComponent() : base("api/images") { }
    }
}
