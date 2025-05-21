using Nop.Web.Models.Media;

namespace NopStation.Plugin.Theme.Kingdom.Models;

public class HeaderLinkModel
{
    public HeaderLinkModel()
    {
        Link1 = new LinkModel();
        Link2 = new LinkModel();
    }

    public LinkModel Link1 { get; set; }
    public LinkModel Link2 { get; set; }

    public class LinkModel
    {
        public LinkModel()
        {
            Icon = new PictureModel();
        }

        public bool Enabled { get; set; }
        public string Title { get; set; }
        public PictureModel Icon { get; set; }
        public string Link { get; set; }
    }
}
