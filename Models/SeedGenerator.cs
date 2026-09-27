using Newtonsoft.Json;

namespace Seido.Utilities.SeedGenerator
{
    #region exported types
    public interface ISeed<T>
    {
        //In order to separate from real and seeded instances
        public bool Seeded { get; set; }

        //Seeded The instance
        public T Seed(SeedGenerator seedGenerator);
    }

    public class SeededReviews
    {
        public string Paragraph { get; init; }
        public List<string> Sentences { get; init; }
    }
    public class SeededDescriptions
    {
        public string Description { get; init; }
        public List<string> DescSentences { get; init; }
    }

    public class SeededQuote
    {
        public string Quote { get; init; }
        public string Author { get; init; }
    }
    #endregion

    public class SeedGenerator : Random
    {
        readonly SeedJsonContent _seeds = null;

        #region Names

        public string AttractionFirstName => _seeds.Names.AttractionFirstNames[this.Next(0, _seeds.Names.AttractionFirstNames.Count)];
        public string AttractionSecondName => _seeds.Names.AttractionSecondNames[this.Next(0, _seeds.Names.AttractionSecondNames.Count)];
        public string AttractionCategory => _seeds.Names.AttractionCategories[this.Next(0, _seeds.Names.AttractionCategories.Count)];
        public string FirstName => _seeds.Names.FirstNames[this.Next(0, _seeds.Names.FirstNames.Count)];
        public string LastName => _seeds.Names.LastNames[this.Next(0, _seeds.Names.LastNames.Count)];

        #endregion

        #region Addresses
        public string Country => _seeds.Addresses[this.Next(0, _seeds.Addresses.Count)].Country;
        public string City(string Country = null)
        {
            if (Country != null)
            {
                var adr = _seeds.Addresses.FirstOrDefault(c => c.Country.ToLower() == Country.Trim().ToLower());
                if (adr == null)
                    throw new ArgumentException("Country not found");

                return adr.Cities[this.Next(0, adr.Cities.Count)];
            }

            var tmp = _seeds.Addresses[this.Next(0, _seeds.Addresses.Count)];
            return tmp.Cities[this.Next(0, tmp.Cities.Count)];
        }
        public string StreetAddress(string Country = null)
        {
            if (Country != null)
            {
                var adr = _seeds.Addresses.FirstOrDefault(c => c.Country.ToLower() == Country.Trim().ToLower());
                if (adr == null)
                    throw new ArgumentException("Country not found");

                return $"{adr.Streets[this.Next(0, adr.Streets.Count)]} {this.Next(1, 100)}";
            }

            var tmp = _seeds.Addresses[this.Next(0, _seeds.Addresses.Count)];
            return $"{tmp.Streets[this.Next(0, tmp.Streets.Count)]} {this.Next(1, 100)}";
        }
        public int ZipCode => this.Next(10101, 100000);
        #endregion

        #region Emails and phones
        public string Email(string fname = null, string lname = null)
        {
            fname ??= FirstName;
            lname ??= LastName;

            return $"{fname}.{lname}@{_seeds.Domains.Domains[this.Next(0, _seeds.Domains.Domains.Count)]}";
        }


        public string PhoneNr => $"{this.Next(700, 800)} {this.Next(100, 1000)} {this.Next(100, 1000)}";
        #endregion


        #region Review
        public List<SeededReviews> AllReviews => _seeds.Reviews
            .Select(l => new SeededReviews { Paragraph = l.Paragraph, Sentences = l.Sentences })
            .ToList();

        public List<SeededReviews> ReviewParagraphs(int tryNrOfItems)
        {
            return UniqueIndexPickedFromList(tryNrOfItems, AllReviews);
        }

        public List<string> ReviewSentence(int tryNrOfItems)
        {
            var sRet = new List<string>();
            for (int i = 0; i < tryNrOfItems; i++)
            {
                var pIdx = this.Next(0, AllReviews.Count);
                var sIdx = this.Next(0, AllReviews[pIdx].Sentences.Count);

                sRet.Add(AllReviews[pIdx].Sentences[sIdx]);
            }
            return sRet;
        }

        public string ReviewParagraph => ReviewParagraphs(1).FirstOrDefault()?.Paragraph;
        public string Comment => ReviewSentence(1).FirstOrDefault();
        #endregion

        #region Description
        public List<SeededDescriptions> AllDescriptions => _seeds.Descriptions
            .Select(l => new SeededDescriptions { Description = l.Description, DescSentences = l.DescSentences })
            .ToList();

        public List<SeededDescriptions> DescriptionParagraphs(int tryNrOfItems)
        {
            return UniqueIndexPickedFromList(tryNrOfItems, AllDescriptions);
        }

        public List<string> DescriptionSentence(int tryNrOfItems)
        {
            var sRet = new List<string>();
            for (int i = 0; i < tryNrOfItems; i++)
            {
                var pIdx = this.Next(0, AllDescriptions.Count);
                var sIdx = this.Next(0, AllDescriptions[pIdx].DescSentences.Count);

                sRet.Add(AllDescriptions[pIdx].DescSentences[sIdx]);
            }
            return sRet;
        }

        public string Description => DescriptionParagraphs(1).FirstOrDefault()?.Description;
        public string DescSentence => DescriptionSentence(1).FirstOrDefault();
        #endregion                

        public TItem FromList<TItem>(List<TItem> items)
        {
            return items[this.Next(0, items.Count)];
        }

        #region Generate seeded List of TItem

        //ISeed<TItem> has to be implemented to use this method
        public List<TItem> ItemsToList<TItem>(int NrOfItems)
            where TItem : ISeed<TItem>, new()
        {
            //Create a list of seeded items
            var _list = new List<TItem>();
            for (int c = 0; c < NrOfItems; c++)
            {
                _list.Add(new TItem() { Seeded = true }.Seed(this));
            }
            return _list;
        }

        //Create a list of unique randomly seeded items
        public List<TItem> UniqueItemsToList<TItem>(int tryNrOfItems, List<TItem> appendToUnique = null)
            where TItem : ISeed<TItem>, IEquatable<TItem>, new()
        {
            //Create a list of uniquely seeded items
            HashSet<TItem> _set = (appendToUnique == null) ? new HashSet<TItem>() : new HashSet<TItem>(appendToUnique);

            while (_set.Count < tryNrOfItems)
            {
                var _item = new TItem() { Seeded = true }.Seed(this);

                int _preCount = _set.Count;
                int tries = 0;
                do
                {
                    _set.Add(_item);

                    if (_set.Count == _preCount)
                    {
                        //Item was already in the _set. Generate a new one
                        _item = new TItem() { Seeded = true }.Seed(this);
                        ++tries;

                        //Does not seem to be able to generate new unique item
                        if (tries > 5)
                            return _set.ToList();
                    }

                } while (_set.Count <= _preCount);
            }

            return _set.ToList();
        }

        //Pick a number of unique items from a list of TItem (the List does not have to be unique)
        //IEquatable<TItem> has to be implemented to use this method
        public List<TItem> UniqueItemsPickedFromList<TItem>(int tryNrOfItems, List<TItem> list)
        where TItem : IEquatable<TItem>
        {
            //Create a list of uniquely seeded items
            HashSet<TItem> _set = new HashSet<TItem>();

            while (_set.Count < tryNrOfItems)
            {
                var _item = list[this.Next(0, list.Count)];

                int _preCount = _set.Count;
                int tries = 0;
                do
                {
                    _set.Add(_item);

                    if (_set.Count == _preCount)
                    {
                        //Item was already in the _set. Pick a new one
                        _item = list[this.Next(0, list.Count)];
                        ++tries;

                        //Does not seem to be able to pick new unique item
                        if (tries > 5)
                            return _set.ToList();
                    }

                } while (_set.Count <= _preCount);
            }

            return _set.ToList();
        }

        //Pick a number of items, all with unique indexes, from a list of TItem
        public List<TItem> UniqueIndexPickedFromList<TItem>(int tryNrOfItems, List<TItem> list)
             where TItem : new()
        {
            //Create a hashed list of unique indexes
            HashSet<int> _set = new HashSet<int>();

            while (_set.Count < tryNrOfItems)
            {
                var _idx = this.Next(0, list.Count);

                int _preCount = _set.Count;
                int tries = 0;
                do
                {
                    _set.Add(_idx);

                    if (_set.Count == _preCount)
                    {
                        //Idx was already in the _set. Generate a new one
                        _idx = this.Next(0, list.Count);
                        ++tries;

                        //Does not seem to be able to generate new unique idx
                        if (tries > 5)
                            break;
                    }

                } while (_set.Count <= _preCount);
            }

            //I have now a set of unique idx
            //return a list of items from a list with indexes
            var retList = new List<TItem>();
            foreach (var item in _set)
            {
                retList.Add(list[item]);
            }
            return retList;
        }
        #endregion

        #region initialize master content
        SeedJsonContent CreateMasterSeedFile()
        {
            return new SeedJsonContent()
            {
                Reviews = new List<SeedReview> {
                        new SeedReview { jsonParagraph =
                            "Fantastic experience for the whole family! Well-maintained grounds and very friendly staff. Exceeded all our expectations. We will definitely come back next season. An absolute must-visit if you are in the area. Stunning views and a lovely atmosphere throughout." },
                        new SeedReview { jsonParagraph =
                            "Very educational and exciting for both adults and kids. Highly recommended for anyone interested in history. The tour guide was incredibly knowledgeable and engaging. A truly magical place filled with fascinating details. The exhibitions were well thought out and inspiring." },
                        new SeedReview { jsonParagraph =
                            "A wonderful setting to wander around on a sunny afternoon. Very clean and fresh everywhere. The staff went above and beyond to make us feel welcome. Fun activities suitable for all age groups. We ended up staying much longer than originally planned." },
                        new SeedReview { jsonParagraph =
                            "A hidden gem that definitely deserves more attention! Incredibly peaceful and beautifully composed. A perfect destination for a successful day trip. Delicious food in the restaurant made with local ingredients. One of the best experiences we have had all year." },
                        new SeedReview { jsonParagraph =
                            "Impressive architecture and exciting environments to explore. Everything ran smoothly from entrance to exit. The kids talked about the visit the entire way home. Great value considering how much there was to see. Will definitely recommend this to friends and family." },
                        new SeedReview { jsonParagraph =
                            "Decent place overall, but a bit too crowded in the middle of the day. The cafe was quite overpriced for what was offered. Nice facility otherwise and pleasant surroundings. Worth seeing once, but a shorter visit was plenty. Parking was unfortunately very limited." },
                        new SeedReview { jsonParagraph =
                            "Pleasant for a quiet walk, but poorly signposted in several areas. The kids got bored quickly as there were few interactive elements. Cozy environment, but lacked adequate seating along the path. An okay stop if you happen to be passing by. Uneven maintenance on some sections." },
                        new SeedReview { jsonParagraph =
                            "Nice scenery and beautiful views, but the admission fee felt steep. Long wait times for food during the lunch rush. Interesting concept, but the main tour felt rather rushed and short. An acceptable attraction, though probably best visited during the off-season." },
                        new SeedReview { jsonParagraph =
                            "Unfortunately quite run-down and not worth the admission price. Several key areas were closed for maintenance without any prior notice. Far too crowded and noisy during peak hours. Ended up being a major disappointment compared to the photos online. Not what we had hoped for." },
                        new SeedReview { jsonParagraph =
                            "A dull experience with indifferent and unhelpful staff at the counter. Hard to find parking and the queues were poorly organized. Several features were not working properly during our visit. Felt mostly like an overpriced tourist trap with little substance. We will not be returning here again." },
            },
                Descriptions = new List<SeedDescription>
                {
                    new SeedDescription { jsonDescription =
                        "Standing high above the rocky shoreline, this 14th-century fortress features intact stone ramparts and twin watchtowers. Within the outer walls lie vaulted chambers, an armory display, and a central cobblestone courtyard. A narrow perimeter trail provides panoramic vistas across the open sea and adjacent archipelago. The lower bastion houses original iron cannons positioned toward the historic harbor entrance. Paved walkways connect the main gate to the preserved inner keep." },
                    new SeedDescription { jsonDescription =
                        "This expansive Victorian-era glasshouse complex preserves hundreds of tropical and subtropical plant species across distinct climate zones. Meandering brick paths lead visitors through dense palm collections, flowering orchids, and arid desert succulents. Elevated iron walkways offer close-up views of the canopy layer and cascading indoor water features. An adjoining heritage herb garden showcases medicinal and culinary specimens arranged by geographical origin. Natural light filters through the curved iron framework throughout the entire structure."
                    },
                    new SeedDescription { jsonDescription =
                        "Carved by subterranean rivers over millions of years, this underground network features towering stalactites and mineral-rich limestone curtains. Illuminated footpaths wind past subterranean pools and cavernous halls with ceilings rising over twenty meters high. Natural rock formations create dramatic natural arches throughout the main touring gallery. The internal temperature remains constant year-round, accompanied by high humidity and natural echoing acoustics. Wooden footbridges cross the deeper ravines along the designated visitors' route."
                    },
                    new SeedDescription { jsonDescription =
                        "Enclosed by pastel-colored merchant townhouses from the 17th century, this central square serves as the historical heart of the district. The center features an ornate stone fountain surrounded by traditional open-air craft and produce stalls. On the eastern edge stands a brick market hall offering regional delicacies and specialty goods under timber-beamed ceilings. Cobblestone pedestrian alleys radiate outward from the plaza toward nearby canal banks. Historic gas lanterns line the perimeter and illuminate the architectural facades after dusk."
                    },
                    new SeedDescription { jsonDescription =
                        "Spanning a deep alpine gorge, this narrow suspension bridge hangs over a rushing glacial river below. Anchored into solid granite bluffs, the steel-cable structure sways gently in the mountain winds. Marked hiking trails link the bridge landing to panoramic wooden platforms overlooking snowcapped peaks and dense pine forests. Informational signboards along the route detail the region's geological formation and alpine wildlife habitats. The surrounding terrain transitions from subalpine forest into rugged scree slopes."
                    },
                    new SeedDescription { jsonDescription =
                        "Spread across rolling pastureland, this open-air museum preserves over thirty traditional timber homesteads, windmills, and barns. Gravel paths meander through working heritage fields cultivated with heirloom grain varieties and traditional garden vegetables. Heritage livestock breeds, including sheep and draft horses, graze in stone-walled paddocks throughout the grounds. Inside the cottages, period furnishings and traditional hearths demonstrate rural daily life from the late 19th century. A restored blacksmith workshop stands adjacent to the central milling pond."
                    },
                    new SeedDescription { jsonDescription =
                        "Positioned along a refurbished industrial pier, this cultural center features striking geometric architecture crafted from weathered steel and glass. Floor-to-ceiling windows look out over the active shipping channel and coastal islands. Expansive, open-plan galleries host changing exhibitions of large-scale sculptures, digital installations, and modern paintings. An exterior boardwalk wraps around the water-facing facade, connecting the exhibition wings to outdoor sculpture terraces. The building incorporates solar canopies and seawater cooling systems throughout."
                    },
                    new SeedDescription { jsonDescription =
                        "Perched on a sheer limestone promontory, the remains of this medieval monastery overlook an arid valley basin. Crumbling stone cloisters and roofless chapel walls still showcase intricate Romanesque archways and carved pillar capitals. Hand-cut stone staircases wind down from the sanctuary ruins to secluded hillside prayer caves. Wild cypress trees and aromatic scrub brush grow freely among the weathered flagstones and ancient cisterns. The elevated position affords sweeping 360-degree views across the distant mountain ranges."
                    },
                    new SeedDescription { jsonDescription =
                        "This historic waterfront quarter is defined by rows of converted brick warehouses and reflective canal waterways. Traditional wooden barges and heritage canal boats remain moored along the stone quaysides. Pedestrian bridges with cast-iron railings arch over the waterways, connecting quiet residential lanes to bustling waterside promenades. Former cargo hoists and loading bays remain integrated into the contemporary facade restorations. Tree-lined walkways run parallel to the water, bordered by small courtyards and public squares."
                    },
                    new SeedDescription { jsonDescription =
                        "Covering an expansive wetland delta, this protected coastal reserve encompasses dense mangrove stands and tidal marshlands. A continuous elevated wooden boardwalk traverses the marsh floor, leading to several multi-tiered wildlife viewing platforms. The reserve forms an important migratory corridor for wading birds, nesting waterfowl, and coastal marine life. Saltwater inlets weave through the reeds, feeding into a wide brackish lagoon near the outer shoreline. Interpretive panels along the path describe the sensitive estuarine ecosystem and tidal dynamics."}
                },

                Addresses = new List<SeedAddress>
                {
                        new SeedAddress {
                            jsonCountry= "Sweden",
                            jsonStreets = "Svedjevägen, Ringvägen, Vasagatan, Odenplan, Birger Jarlsgatan, Äppelviksvägen, Kvarnbacksvägen, Kungsgatan, Drottninggatan, Storgatan, Linnegatan, Sveavägen, Hornsgatan, Götgatan, Avenyn, Karlavägen, Hamngatan, Odengatan, Skolgatan, Kyrkogatan",
                            jsonCities = "Stockholm, Göteborg, Malmö, Uppsala, Linköping, Örebro, Västerås, Helsingborg, Jönköping, Norrköping, Lund, Umeå, Gävle, Borås, Södertälje, Halmstad, Växjö, Karlstad, Eskilstuna, Sundsvall, Östersund, Trollhättan, Luleå, Lidingö, Borlänge, Tumba, Kristianstad, Kalmar, Falun, Skövde"
                    },
                        new SeedAddress {
                            jsonCountry = "Norway",
                            jsonStreets = "Bygdøy allé, Frognerveien, Pilestredet, Vidars gate, Sagveien, Toftes gate, Gardeveien, Karl Johans gate, Bogstadveien, Thorvald Meyers gate, Storgata, Torggata, Markveien, Henrik Ibsens gate, Dronningens gate, Kirkegata, Strandgaten, Kongens gate, Ullevålsveien, Grünerløkka allé",
                            jsonCities = "Oslo, Bergen, Trondheim, Stavanger, Drammen, Fredrikstad, Kristiansand, Sandnes, Tromsø, Sarpsborg, Skien, Ålesund, Sandefjord, Haugesund, Tønsberg, Moss, Bodø, Arendal, Hamar, Ytrebygda, Larvik, Halden, Askøy, Kongsberg, Harstad, Molde, Lillehammer, Horten, Gjøvik, Mo i Rana"
                    },
                        new SeedAddress {
                            jsonCountry = "Denmark",
                            jsonCities = "København, Aarhus, Odense, Aalborg, Esbjerg, Randers, Kolding, Horsens, Vejle, Roskilde, Herning, Hørsholm, Helsingør, Silkeborg, Næstved, Fredericia, Viborg, Køge, Holstebro, Taastrup, Slagelse, Hillerød, Sønderborg, Svendborg, Hjørring, Holbæk, Frederikshavn, Nørresundby, Ringsted, Haderslev",
                            jsonStreets = "Rolighedsvej, Fensmarkgade, Svanevej, Grøndalsvej, Gothersgade, Classensgade, Moltkesvej, Strøget, Vesterbrogade, Nørrebrogade, Østerbrogade, Amagerbrogade, Bredgade, Store Kongensgade, Frederiksborggade, Istedgade, Vendersgade, Jægersborggade, Kronprinsessegade, Sankt Peders Stræde"
                    },
                        new SeedAddress {
                            jsonCountry = "Finland",
                            jsonCities = "Helsinki, Espoo, Tampere, Vantaa, Oulu, Turku, Jyväskylä, Kuopio, Lahti, Pori, Kouvola, Joensuu, Lappeenranta, Hämeenlinna, Vaasa, Rovaniemi, Seinäjoki, Mikkeli, Kotka, Salo, Porvoo, Kokkola, Hyvinkää, Lohja, Järvenpää, Nurmijärvi, Rauma, Kirkkonummi, Tuusula, Kajaani",
                            jsonStreets = "Arkadiankatu, Liisankatu, Ruoholahdenkatu, Pohjoisranta, Eerikinkatu, Vauhtitie, Itäinen Vaihdekuja, Mannerheimintie, Aleksanterinkatu, Hämeenkatu, Esplanadi, Bulevardi, Fredrikinkatu, Annankatu, Uudenmaankatu, Yrjönkatu, Tehtaankatu, Kasarmikatu, Kauppakatu, Linnankatu"
                    },
                },
                Names = new SeedNames
                {
                    jsonFirstNames = "Erik, Lars, Johan, Anders, Karl, Nils, Sven, Jonas, Henrik, Magnus, Astrid, Elsa, Freja, Karin, Ingrid, Saga, Ebba, Linnea, Maja, Sigrid, Olav, Magnus, Mikkel, Einar, Tuva",
                    jsonLastNames = "Andersson, Johansson, Karlsson, Nilsson, Eriksson, Larsson, Olsson, Persson, Svensson, Lindberg, Hansen, Olsen, Johansen, Larsen, Andersen, Nielsen, Jensen, Møller, Rasmussen, Pedersen, Virtanen, Korhonen, Mäkinen, Nieminen, Laine",
                    jsonAttractionSecondNames = "Old Town, Downtown, Grand,Royal, Central, Secret, Sunset, Golden, Hidden, Crystal, Northern, Southern, Riverside, Mountain, Coastal, Historic ,Harbor, Emerald, Sunken, Highland",
                    jsonAttractionFirstNames = "Mystic, Vintage, Wild, Serene, Urban, Lost, Ancient, Boutique, Midnight, Silent, Rustic, Epic, Scenic, Velvet, Forgotten, Twilight, Endless, Bohemian, Breezy, Hidden",
                    jsonAttractionCategories = "Landmark, Museum, Architecture, Viewpoint, National Park, Beach, Park, Garden, Winery, Bakery, Club, Adventures, Amusement Park, Aquarium, Spa, Market, Bazaar"
                },
                Domains = new SeedDomains
                {
                    jsonDomainNames = "icloud.com, me.com, mac.com, hotmail.com, gmail.com"
                },

            };
        }
        #endregion

        #region create master json file
        public string WriteMasterStream()
        {
            return CreateMasterSeedFile().WriteFile("master-seeds.json");
        }
        #endregion

        #region contructors
        public SeedGenerator()
        {
            _seeds = CreateMasterSeedFile();
        }
        public SeedGenerator(string SeedPathName)
        {
            if (!SeedJsonContent.FileExists(SeedPathName))
            {
                throw new FileNotFoundException(SeedPathName);
            }
            _seeds = SeedJsonContent.ReadFile(SeedPathName);
        }
        #endregion

        #region internal classes
        class SeedReview
        {
            #region Reviews towards json file
            string _jsonParagraph;
            public string jsonParagraph
            {
                get => _jsonParagraph;
                set
                {
                    _jsonParagraph = value;
                    _sentences = new List<string>(_jsonParagraph.Split(". "))
                        .Select(s =>
                        {
                            var _sentence = s.Trim(new char[] { ' ', ',', '.' });
                            return _sentence + '.';
                        }).ToList();
                }
            }
            #endregion

            [JsonIgnore]
            public string Paragraph => _jsonParagraph;

            List<string> _sentences;
            [JsonIgnore]
            public List<string> Sentences => _sentences;

        }
        class SeedDescription
        {
            #region Reviews towards json file
            string _jsonDescription;
            public string jsonDescription
            {
                get => _jsonDescription;
                set
                {
                    _jsonDescription = value;
                    _descSentences = new List<string>(_jsonDescription.Split(". "))
                        .Select(s =>
                        {
                            var _sentence = s.Trim(new char[] { ' ', ',', '.' });
                            return _sentence + '.';
                        }).ToList();
                }
            }
            #endregion

            [JsonIgnore]
            public string Description => _jsonDescription;

            List<string> _descSentences;
            [JsonIgnore]
            public List<string> DescSentences => _descSentences;
        }

        class SeedAddress
        {
            #region Country towards json file
            string _jsonCountry;
            public string jsonCountry { get => _jsonCountry; set { _jsonCountry = value; } }
            #endregion

            [JsonIgnore]
            public string Country => _jsonCountry;

            #region Streets towards json file
            string _jsonStreets;
            public string jsonStreets
            {
                get => _jsonStreets;
                set
                {
                    _jsonStreets = value;
                    _streets = _jsonStreets.Split(", ").ToList();
                }
            }
            #endregion

            List<string> _streets;
            [JsonIgnore]
            public List<string> Streets => _streets;

            #region Cities towards json file
            string _jsonCities;
            public string jsonCities
            {
                get => _jsonCities;
                set
                {
                    _jsonCities = value;
                    _cities = _jsonCities.Split(", ").ToList();
                }
            }
            #endregion

            List<string> _cities;
            [JsonIgnore]
            public List<string> Cities => _cities;
        }
        class SeedNames
        {
            #region Names towards json file
            string _jsonFirstNames;
            public string jsonFirstNames
            {
                get => _jsonFirstNames;
                set
                {
                    _jsonFirstNames = value;
                    _firstNames = _jsonFirstNames.Split(", ").ToList();
                }
            }

            string _jsonLastNames;
            public string jsonLastNames
            {
                get => _jsonLastNames;
                set
                {
                    _jsonLastNames = value;
                    _lastNames = _jsonLastNames.Split(", ").ToList();
                }
            }


            string _jsonAttractionFirstNames;
            public string jsonAttractionFirstNames
            {
                get => _jsonAttractionFirstNames;
                set
                {
                    _jsonAttractionFirstNames = value;
                    _attractionFirstNames = _jsonAttractionFirstNames.Split(", ").ToList();
                }
            }

            string _jsonAttractionSecondNames;
            public string jsonAttractionSecondNames
            {
                get => _jsonAttractionSecondNames;
                set
                {
                    _jsonAttractionSecondNames = value;
                    _attractionSecondNames = _jsonAttractionSecondNames.Split(", ").ToList();
                }
            }

            string _jsonAttractionCategories;
            public string jsonAttractionCategories
            {
                get => _jsonAttractionCategories;
                set
                {
                    _jsonAttractionCategories = value;
                    _attractionCategories = _jsonAttractionCategories.Split(", ").ToList();
                }
            }
            #endregion

            List<string> _firstNames;
            [JsonIgnore]
            public List<string> FirstNames => _firstNames;

            List<string> _lastNames;
            [JsonIgnore]
            public List<string> LastNames => _lastNames;



            List<string> _attractionFirstNames;
            [JsonIgnore]
            public List<string> AttractionFirstNames => _attractionFirstNames;

            List<string> _attractionSecondNames;
            [JsonIgnore]
            public List<string> AttractionSecondNames => _attractionSecondNames;

            List<string> _attractionCategories;
            [JsonIgnore]
            public List<string> AttractionCategories => _attractionCategories;
        }
        class SeedDomains
        {
            #region Domains towards json file
            string _jsonDomainNames;
            public string jsonDomainNames
            {
                get => _jsonDomainNames;
                set
                {
                    _jsonDomainNames = value;
                    _domainNames = _jsonDomainNames.Split(", ").ToList();
                }
            }
            #endregion

            List<string> _domainNames;
            [JsonIgnore]
            public List<string> Domains => _domainNames;
        }

        class SeedJsonContent
        {
            public List<SeedReview> Reviews { get; set; } = new List<SeedReview>();
            public List<SeedDescription> Descriptions { get; set; } = new List<SeedDescription>();
            public List<SeedAddress> Addresses { get; set; } = new List<SeedAddress>();
            public SeedNames Names { get; set; } = new SeedNames();
            public SeedDomains Domains { get; set; } = new SeedDomains();


            public string WriteFile(string FileName) => WriteFile(this, FileName);
            public static string WriteFile(SeedJsonContent Seeds, string FileName)
            {
                var fn = fname(FileName);
                using (Stream s = File.Create(fn))
                using (TextWriter writer = new StreamWriter(s))
                {
                    writer.Write(JsonConvert.SerializeObject(Seeds, Formatting.Indented));
                }

                return fn;
            }

            public static SeedJsonContent ReadFile(string PathName)
            {
                SeedJsonContent seeds = null;
                using (Stream s = File.OpenRead(PathName))
                using (TextReader reader = new StreamReader(s))

                    seeds = JsonConvert.DeserializeObject<SeedJsonContent>(reader.ReadToEnd());

                return seeds;
            }

            static string fname(string name)
            {
                var documentPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                documentPath = Path.Combine(documentPath, "SeedGenerator");
                if (!Directory.Exists(documentPath)) Directory.CreateDirectory(documentPath);
                return Path.Combine(documentPath, name);
            }

            public static bool FileExists(string FileName)
            {

                var fn = Path.GetFileName(FileName);
                if (fn == FileName)
                {
                    //no path in FileName use default directory
                    return File.Exists(fname(FileName));
                }

                return File.Exists(FileName);
            }
        }
        #endregion
    }
}

