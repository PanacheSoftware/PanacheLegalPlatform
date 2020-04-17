using System;
using System.Collections.Generic;
using System.Text;

namespace PanacheSoftware.Core.Domain.UI
{
    //{
    //                    "title": "Books",
    //                    "expanded": true,
    //                    "folder": true,
    //                    "children": [
    //                        {
    //                        "title": "Art of War",
    //                        "type": "book",
    //                        "author": "Sun Tzu",
    //                        "year": -500,
    //                        "qty": 21,
    //                        "price": 5.95
    //                        },
    //                        {
    //                        "title": "The Hobbit",
    //                        "type": "book",
    //                        "author": "J.R.R. Tolkien",
    //                        "year": 1937,
    //                        "qty": 32,
    //                        "price": 8.97
    //                        },
    //                        {
    //                        "title": "The Little Prince",
    //                        "type": "book",
    //                        "author": "Antoine de Saint-Exupery",
    //                        "year": 1943,
    //                        "qty": 2946,
    //                        "price": 6.82
    //                        },
    //                        {
    //                        "title": "Don Quixote",
    //                        "type": "book",
    //                        "author": "Miguel de Cervantes",
    //                        "year": 1615,
    //                        "qty": 932,
    //                        "price": 15.99
    //                        }
    //                    ]
    //                },
    //                {
    //                    "title": "Music",
    //                    "folder": true,
    //                    "children": [
    //                        {
    //                        "title": "Nevermind",
    //                        "type": "music",
    //                        "author": "Nirvana",
    //                        "year": 1991,
    //                        "qty": 916,
    //                        "price": 15.95
    //                        },
    //                        {
    //                        "title": "Autobahn",
    //                        "type": "music",
    //                        "author": "Kraftwerk",
    //                        "year": 1974,
    //                        "qty": 2261,
    //                        "price": 23.98
    //                        },
    //                        {
    //                        "title": "Kind of Blue",
    //                        "type": "music",
    //                        "author": "Miles Davis",
    //                        "year": 1959,
    //                        "qty": 9735,
    //                        "price": 21.9
    //                        },
    //                        {
    //                        "title": "Back in Black",
    //                        "type": "music",
    //                        "author": "AC/DC",
    //                        "year": 1980,
    //                        "qty": 3895,
    //                        "price": 17.99
    //                        },
    //                        {
    //                        "title": "The Dark Side of the Moon",
    //                        "type": "music",
    //                        "author": "Pink Floyd",
    //                        "year": 1973,
    //                        "qty": 263,
    //                        "price": 17.99
    //                        },
    //                        {
    //                        "title": "Sgt. Pepper's Lonely Hearts Club Band",
    //                        "type": "music",
    //                        "author": "The Beatles",
    //                        "year": 1967,
    //                        "qty": 521,
    //                        "price": 13.98
    //                        }
    //                    ]
    //                }

    public class FolderHeaderStructureModel
    {
        public FolderHeaderStructureModel()
        {
            children = new List<TeamChartModel>();
            folder = true;
        }

        public string title { get; set; }
        public bool folder { get; set; }
        public List<TeamChartModel> children { get; set; }
    }
}
