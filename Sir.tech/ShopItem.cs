using System.Collections.Generic;

namespace Sir.tech
{
    public class ShopItem
    {
        public string? BlockType { get; set; }
        public List<Skin> Skins { get; set; } = new();
    }
}
