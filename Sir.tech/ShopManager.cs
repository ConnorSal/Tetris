using System.Collections.Generic;
using System.Linq;

namespace Sir.tech
{
    public class ShopManager
    {
        private readonly Dictionary<string, string> defaultTilePaths = new()
{
    { "I", "Assets/TileCyan.png" },
    { "J", "Assets/TileBlue.png" },
    { "L", "Assets/TileOrange.png" },
    { "O", "Assets/TileYellow.png" },
    { "S", "Assets/TileGreen.png" },
    { "T", "Assets/TilePurple.png" },
    { "Z", "Assets/TileRed.png" }
};


        public List<ShopItem> Items { get; private set; } = new();
        public PlayerData Player { get; private set; }

        public ShopManager()
        {
            Player = PlayerData.Load();
            InitializeShop();
        }

        private void InitializeShop()
        {
            Items = new List<ShopItem>
    {
        //I Block
        new ShopItem
{
    BlockType = "I",
    Skins = new List<Skin>
    {
        new Skin
        {
            Id = "I_Default",
            DisplayName = "Default Cyan",
            ImagePath = "Assets/Block-I.png",
            TilePath = "Assets/TileCyan.png",
            Price = 0
        },
        new Skin
        {
            Id = "I_Synth",
            DisplayName = "Synth-Wave",
            ImagePath = "Assets/Block-I-Synth.png",
            TilePath = "Assets/TileSynth.png",
            Price = 10
        },
        new Skin
        {
            Id = "I_Gold",
            DisplayName = "Golden Beam",
            ImagePath = "Assets/Blocks/I_Gold.png",
            TilePath = "Assets/TileYellow.png",
            Price = 100
        }
    }
},

        //J Block
        new ShopItem
        {
            BlockType = "J",
            Skins = new List<Skin>
           {
        new Skin
        {
            Id = "J_Default",
            DisplayName = "Default Blue",
            ImagePath = "Assets/Block-J.png",
            TilePath = "Assets/TileBlue.png",
            Price = 0
        },
        new Skin
        {
            Id = "J_Synth",
            DisplayName = "Synth-Wave",
            ImagePath = "Assets/Block-J-Synth.png",
            TilePath = "Assets/TileSynth.png",
            Price = 10
        },
        new Skin
        {
            Id = "J_Gold",
            DisplayName = "Golden Beam",
            ImagePath = "Assets/Blocks/I_Gold.png",
            TilePath = "Assets/TileYellow.png",
            Price = 100
        }
    }
},
        //L Block
        new ShopItem
        {
            BlockType = "L",
            Skins = new List<Skin>
           {
        new Skin
        {
            Id = "L_Default",
            DisplayName = "Default Orange",
            ImagePath = "Assets/Block-L.png",
            TilePath = "Assets/TileOrange.png",
            Price = 0
        },
        new Skin
        {
            Id = "L_Synth",
            DisplayName = "Synth-Wave",
            ImagePath = "Assets/Block-L-Synth.png",
            TilePath = "Assets/TileSynth.png",
            Price = 10
        },
        new Skin
        {
            Id = "L_Gold",
            DisplayName = "Golden Beam",
            ImagePath = "Assets/Blocks/I_Gold.png",
            TilePath = "Assets/TileYellow.png",
            Price = 100
        }
    }
},

        //O Block
        new ShopItem
        {
            BlockType = "O",
            Skins = new List<Skin>
           {
        new Skin
        {
            Id = "O_Default",
            DisplayName = "Default Yellow",
            ImagePath = "Assets/Block-O.png",
            TilePath = "Assets/TileYellow.png",
            Price = 0
        },
        new Skin
        {
            Id = "O_Synth",
            DisplayName = "Synth-Wave",
            ImagePath = "Assets/Block-O-Synth.png",
            TilePath = "Assets/TileSynth.png",
            Price = 10
        },
        new Skin
        {
            Id = "O_Gold",
            DisplayName = "Golden Beam",
            ImagePath = "Assets/Blocks/I_Gold.png",
            TilePath = "Assets/TileYellow.png",
            Price = 100
        }
    }
},

        //S Block
        new ShopItem
        {
            BlockType = "S",
            Skins = new List<Skin>
            {
        new Skin
        {
            Id = "S_Default",
            DisplayName = "Default Green",
            ImagePath = "Assets/Block-S.png",
            TilePath = "Assets/TileGreen.png",
            Price = 0
        },
        new Skin
        {
            Id = "S_Synth",
            DisplayName = "Synth-Wave",
            ImagePath = "Assets/Block-S-Synth.png",
            TilePath = "Assets/TileSynth.png",
            Price = 10
        },
        new Skin
        {
            Id = "S_Gold",
            DisplayName = "Golden Beam",
            ImagePath = "Assets/Blocks/I_Gold.png",
            TilePath = "Assets/TileYellow.png",
            Price = 100
        }
    }
},

        //T Block
        new ShopItem
        {
            BlockType = "T",
            Skins = new List<Skin>
            {
        new Skin
        {
            Id = "T_Default",
            DisplayName = "Default Purple",
            ImagePath = "Assets/Block-T.png",
            TilePath = "Assets/TilePurple.png",
            Price = 0
        },
        new Skin
        {
            Id = "T_Synth",
            DisplayName = "Synth-Wave",
            ImagePath = "Assets/Block-T-Synth.png",
            TilePath = "Assets/TileSynth.png",
            Price = 10
        },
        new Skin
        {
            Id = "T_Gold",
            DisplayName = "Golden Beam",
            ImagePath = "Assets/Blocks/I_Gold.png",
            TilePath = "Assets/TileYellow.png",
            Price = 100
        }
    }
},

        //Z Block
        new ShopItem
        {
            BlockType = "Z",
            Skins = new List<Skin>
           {
        new Skin
        {
            Id = "Z_Default",
            DisplayName = "Default Red",
            ImagePath = "Assets/Block-Z.png",
            TilePath = "Assets/TileRed.png",
            Price = 0
        },
        new Skin
        {
            Id = "Z_Synth",
            DisplayName = "Synth-Wave",
            ImagePath = "Assets/Block-Z-Synth.png",
            TilePath = "Assets/TileSynth.png",
            Price = 10
        },
        new Skin
        {
            Id = "Z_Gold",
            DisplayName = "Golden Beam",
            ImagePath = "Assets/Blocks/I_Gold.png",
            TilePath = "Assets/TileYellow.png",
            Price = 100
        }
      }
    }
  };

            foreach (var item in Items)
            {
                string defaultId = $"{item.BlockType}_Default";
                if (!Player.OwnedSkins.ContainsKey(item.BlockType))
                    Player.OwnedSkins[item.BlockType] = new List<string>();

                if (!Player.OwnedSkins[item.BlockType].Contains(defaultId))
                    Player.OwnedSkins[item.BlockType].Add(defaultId);

                //Also equip if nothing is equipped yet
                if (!Player.EquippedSkins.ContainsKey(item.BlockType))
                    Player.EquippedSkins[item.BlockType] = defaultId;
            }

            Player.Save();

        }


        public bool PurchaseSkin(string blockType, string skinId)
        {
            Player = PlayerData.Load();

            if (Player.OwnedSkins == null)
                Player.OwnedSkins = new Dictionary<string, List<string>>();
            if (Player.EquippedSkins == null)
                Player.EquippedSkins = new Dictionary<string, string>();
            if (!Player.OwnedSkins.ContainsKey(blockType))
                Player.OwnedSkins[blockType] = new List<string>();

            var item = Items.FirstOrDefault(i => i.BlockType == blockType);
            if (item == null) return false;

            var skin = item.Skins.FirstOrDefault(s => s.Id == skinId);
            if (skin == null) return false;

            if (Player.OwnedSkins[blockType].Contains(skinId))
                return false;

            if (Player.Coins < skin.Price)
                return false;

            Player.Coins -= skin.Price;
            Player.OwnedSkins[blockType].Add(skinId);

            if (!Player.EquippedSkins.ContainsKey(blockType) ||
                string.IsNullOrEmpty(Player.EquippedSkins[blockType]))
            {
                Player.EquippedSkins[blockType] = skinId;
            }

            Player.Save();
            return true;
        }

    }
}
